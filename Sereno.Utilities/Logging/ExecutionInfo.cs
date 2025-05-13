using System;

namespace Sereno.Logging
{

    /// <summary>
    /// Ausführungszeit
    /// </summary>

    public class ExecutionInfo
    {

        /// <summary>
        /// Wieviele Einträge wurden verarbeitet
        /// </summary>

        public Nullable<int> ItemsProcessed { get; set; }

        /// <summary>
        /// Bezeichnung für einen Unterteil der Methode
        /// </summary>

        public string? MethodPart { get; set; }


        /// <summary>
        /// Benötigte Ausführungszeit 
        /// </summary>

        public TimeSpan ExecutionTimeSpan { get; set; }


        /// <summary>
        /// Startzeit der Ausführung
        /// </summary>

        public DateTime Start { get; set; }


        /// <summary>
        /// Endzeit der Ausführung
        /// </summary>

        public DateTime End { get; set; }

    }
}
