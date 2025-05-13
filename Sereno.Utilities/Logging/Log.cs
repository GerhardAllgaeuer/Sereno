using System;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;
using NLog;
using NLog.Config;
using System.Text.RegularExpressions;
using System.Linq;

namespace Sereno.Logging
{
    /// <summary>
    /// Logging Utility
    /// </summary>
    public sealed class Log
    {
        // Von Framework unabhängiger Loglevel
        // kann auch von außen verwendet werden um
        // z.B. den aktuellen Loglevel abzufragen
        public enum Level
        {
            Info,
            Debug,
            Error,
            Fatal,
            Trace,
            Warn
        }

        public static LoggingConfiguration? Configuration { get { return LogManager.Configuration; } }

        #region Singleton

        /// <summary>
        /// Konstruktor, private ausnahmsweise
        /// </summary>
        private Log()
        {
        }

        /// <summary>
        /// Liefert die eindeutige Instanz dieser Klasse
        /// </summary>
        public static Log Instance
        {
            get
            {
                return Nested.instance;
            }
        }

        /// <summary>
        /// Interne Klasse, die als Instanz zurückgeliefert wird
        /// </summary>
        private class Nested
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }

            internal static readonly Log instance = new Log();
        }

        #endregion

        private void WriteLine(LogLevel level, Context? context, string message, params LogDetail[] args)
        {
            string loggerName = GetMethodName();
            Logger logger = LogManager.GetLogger(loggerName);

            LogEventInfo logEvent = new LogEventInfo(level, loggerName, message);

            // Add Context information
            if (context != null)
            {
                logEvent.Properties["UserName"] = context.UserName;
                logEvent.Properties["MachineName"] = context.MachineName;
                logEvent.Properties["IpAddress"] = context.IpAddress;
                logEvent.Properties["ClientVersion"] = context.ClientVersion;
            }

            foreach (LogDetail detail in args)
            {
                string logDetailString = string.Format(" {0}: {1}", detail.Description, detail.Value);
                message += logDetailString;
                logEvent.Properties[detail.Description] = detail.Value;
            }
            logEvent.Message = message;

            logger.Log(logEvent);
        }

        #region Execution

        /// <summary>
        /// Erstellen eines Objekts für die Ausführungs Protokollierung von Methoden (Laufzeit, usw.)
        /// </summary>
        public ExecutionInfo CreateExecutionInfo(Context? context, params LogDetail[] args)
        {
            return CreateExecutionInfoInternal(context, "", null, args);
        }

        /// <summary>
        /// Erstellen eines Objekts für die Ausführungs Protokollierung von Methoden (Laufzeit, usw.)
        /// </summary>
        public ExecutionInfo CreateExecutionInfo(Context? context, string methodPart, params LogDetail[] args)
        {
            return CreateExecutionInfoInternal(context, methodPart, null, args);
        }

        /// <summary>
        /// Erstellen eines Objekts für die Ausführungs Protokollierung von Methoden (Laufzeit, usw.)
        /// </summary>
        public ExecutionInfo CreateExecutionInfo(Context? context, string methodPart, int itemsProcessed, params LogDetail[] args)
        {
            return CreateExecutionInfoInternal(context, methodPart, itemsProcessed, args);
        }

        /// <summary>
        /// Erstellen eines Objekts für die Ausführungs Protokollierung von Methoden (Laufzeit, usw.)
        /// </summary>
        public ExecutionInfo CreateExecutionInfo(Context? context, int itemsProcessed, params LogDetail[] args)
        {
            return CreateExecutionInfoInternal(context, "", itemsProcessed, args);
        }

        /// <summary>
        /// Erstellen eines Objekts für die Ausführungs Protokollierung von Methoden (Laufzeit, usw.)
        /// </summary>
        private ExecutionInfo CreateExecutionInfoInternal(Context? context, string methodPart, int? itemsProcessed, params LogDetail[] args)
        {
            ExecutionInfo result = new ExecutionInfo();
            result.Start = DateTime.Now;
            result.End = DateTime.Now;
            result.ItemsProcessed = itemsProcessed;
            result.MethodPart = methodPart;

            LogDetail[] newArgs = new LogDetail[args.Length + 2];
            Array.Copy(args, newArgs, args.Length);
            newArgs[args.Length] = new LogDetail("ItemsProcessed", itemsProcessed?.ToString() ?? "null");
            newArgs[args.Length + 1] = new LogDetail("MethodPart", methodPart);

            WriteLine(LogLevel.Trace, context, "Start ExecutionInfo", newArgs);

            return result;
        }

        /// <summary>
        /// Protokollierung der Ausführungsinformationen
        /// </summary>
        /// <param name="executionInfo">Ausführungsdaten</param>
        public void Execution(Context? context, ExecutionInfo executionInfo)
        {
            executionInfo.End = DateTime.Now;
            TimeSpan timeSpan = executionInfo.End - executionInfo.Start;

            WriteLine(LogLevel.Trace, context, "End ExecutionInfo",
                new LogDetail("MethodPart", executionInfo.MethodPart),
                new LogDetail("ItemsProcessed", executionInfo.ItemsProcessed?.ToString() ?? "null"),
                new LogDetail("ExecutionTime", timeSpan.TotalMilliseconds));
        }

        #endregion

        #region Error

        /// <summary>
        /// Protokollierung eines Fehlers mit Details
        /// </summary>
        /// <param name="context">Aktueller Kontext</param>
        /// <param name="message">Nachricht, die protokolliert werden soll</param>
        /// <param name="args">LogDetails, die übergeben werden</param>
        public void Error(Context? context, string message, params LogDetail[] args)
        {
            WriteLine(LogLevel.Error, context, message, args);
        }

        /// <summary>
        /// Protokollierung eines Fehlers mit Details
        /// </summary>
        /// <param name="context">Aktueller Kontext</param>
        /// <param name="exception">Fehlermeldung die protokolliert werden soll.</param>
        /// <param name="args">LogDetails, die übergeben werden</param>
        public void Error(Context? context, Exception? exception, params LogDetail[] args)
        {
            Error(context, exception?.Message ?? string.Empty, args);
        }

        /// <summary>
        /// Protokollierung eines Fehlers
        /// </summary>
        /// <param name="context">Aktueller Kontext</param>
        /// <param name="exception">Fehlermeldung die protokolliert werden soll.</param>
        public void Error(Context? context, Exception? exception)
        {
            Error(context,
                exception?.Message ?? string.Empty,
                new LogDetail("Exception", exception?.ToString() ?? "null"));

            if (exception?.InnerException != null)
            {
                Error(context, exception.InnerException);
            }
        }

        #endregion

        #region Warning

        /// <summary>
        /// Protokollierung einer Warnung
        /// </summary>
        /// <param name="context">Aktueller Kontext</param>
        /// <param name="message">Nachricht, die protokolliert werden soll</param>
        /// <param name="args">LogDetails, die übergeben werden</param>
        public void Warning(Context? context, string message, params LogDetail[] args)
        {
            WriteLine(LogLevel.Warn, context, message, args);
        }

        #endregion

        #region Info

        /// <summary>
        /// Protokollierung einer Information
        /// </summary>
        /// <param name="context">Aktueller Kontext</param>
        /// <param name="message">Nachricht, die protokolliert werden soll</param>
        /// <param name="args">LogDetails, die übergeben werden</param>
        public void Info(Context? context, string message, params LogDetail[] args)
        {
            WriteLine(LogLevel.Info, context, message, args);
        }

        #endregion

        #region Debug

        /// <summary>
        /// Protokollierung einer DebugInformation
        /// </summary>
        /// <param name="context">Aktueller Kontext</param>
        /// <param name="message">Nachricht, die protokolliert werden soll</param>
        /// <param name="args">LogDetails, die übergeben werden</param>
        public void Debug(Context? context, string message, params LogDetail[] args)
        {
            WriteLine(LogLevel.Debug, context, message, args);
        }

        #endregion

        #region Context

        /// <summary>
        /// Protokollierung einer Information
        /// </summary>
        /// <param name="context">Aktueller Kontext</param>
        public void ContextInfo(Context? context)
        {
            List<LogDetail> logDetails = new List<LogDetail>();

            if (context == null)
            {
                logDetails.Add(new LogDetail("ContextInfoError", "Context is null"));
            }
            else
            {
                logDetails.Add(new LogDetail("Username", context.UserName));
            }

            WriteLine(LogLevel.Trace, context ?? new Context { UserName = "Unknown", MachineName = Environment.MachineName }, "Context", logDetails.ToArray());
        }

        #endregion

        /// <summary>
        /// Überprüfung auf den aktuellen LogLevel
        /// </summary>
        /// <param name="level">Loglevel auf welchen überprüft werden soll</param>
        /// <returns>true wenn der aktuelle Loglevel dem übergebenen entspricht</returns>
        public bool IsCurrentLogLevel(Level level)
        {
            string loggerName = GetMethodName();
            Logger logger = LogManager.GetLogger(loggerName);

            switch (level)
            {
                case Level.Info:
                    return logger.IsInfoEnabled;
                case Level.Debug:
                    return logger.IsDebugEnabled;
                case Level.Error:
                    return logger.IsErrorEnabled;
                case Level.Fatal:
                    return logger.IsFatalEnabled;
                case Level.Trace:
                    return logger.IsTraceEnabled;
                case Level.Warn:
                    return logger.IsWarnEnabled;
                default:
                    return false;
            }
        }

        private string GetMethodName()
        {
            var st = new StackTrace(true);
            var frames = st.GetFrames();
            if (frames == null)
            {
                return "Unknown Method";
            }

            foreach (var frame in frames)
            {
                bool extractMethodName = true;
                var method = frame?.GetMethod();
                if (method?.DeclaringType?.FullName?.Contains("System.Runtime") == true)
                    extractMethodName = false;

                if (method?.DeclaringType?.FullName?.Contains("Connexia.Logging") == true)
                    extractMethodName = false;

                if (extractMethodName)
                {
                    var name = method?.Name;
                    if (name == "MoveNext" && method?.DeclaringType?.Name?.Contains("<") == true)
                    {
                        // Versuche den Originalnamen aus dem Typnamen zu extrahieren
                        var originalName = method.DeclaringType.Name;
                        var cleaned = Regex.Match(originalName, @"<(.+)>d__\d+");
                        return $"{method.DeclaringType.DeclaringType?.FullName}.{cleaned.Groups[1].Value}";
                    }
                    else
                    {
                        return $"{method?.ReflectedType?.FullName}.{method?.Name}";
                    }
                }
            }

            return "Unknown Method";
        }

        public List<LogDetail> GetLogDetailsForObject(object? objectToLog, bool deep)
        {
            List<LogDetail> logDetailList = new List<LogDetail>();

            if (objectToLog == null)
            {
                return logDetailList;
            }

            Type objectType = objectToLog.GetType();
            if (objectType == null)
            {
                return logDetailList;
            }

            PropertyInfo[] properties = objectType.GetProperties(
              BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            // Erzeugen der LogDetails
            foreach (PropertyInfo pi in properties)
            {
                if (!deep)
                {
                    object? value = pi.GetValue(objectToLog);
                    LogDetail logDetail = new LogDetail(pi.Name, value);
                    logDetailList.Add(logDetail);
                }
                else
                {
                    // TODO: gesamten ObjectTree durchlaufen
                    //    Type fiType = fi.GetType();
                    //    logDetailList.AddRange(GetLogDetailsForObject(fi.GetValue(objectToLog), true));
                }
            }

            return logDetailList;
        }
    }
}





