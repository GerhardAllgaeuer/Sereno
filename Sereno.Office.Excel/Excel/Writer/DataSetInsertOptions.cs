using Sereno.Office.Excel.Excel.Writer;
using System.Collections.Generic;
using static Sereno.Office.Excel.Writer.ExcelWriterUtility;

namespace Sereno.Office.Excel.Writer
{
    public class DataSetInsertOptions
    {

        /// <summary>
        /// Name des Quell-Tabelle im DataSet.
        /// Wenn leer, dann die erste Tabelle, die gefunden wird.
        /// </summary>
        public string DataSetTableName { get; set; }

        /// <summary>
        /// Name des Arbeitsblatts im Excel, in das die Tabelle eingefügt wird. 
        /// Wenn leer, dann das 1. Arbeitsblatt
        /// </summary>
        public string ExcelWorkSheetName { get; set; }

        /// <summary>
        /// Übernimm die Formeln aus der ersten Zeile
        /// </summary>
        public bool WithFormulas { get; set; } = true;


        /// <summary>
        /// 0 bei Zahlen auf leer setzen. 
        /// Im Standard ja.
        /// </summary>
        public bool ZeroAsEmpty { get; set; } = true;

        /// <summary>
        /// Zeile, bei der im Arbeitsblatt gestartet wird. 
        /// Wenn keine Angabe, dann bei Zeile 2.
        /// </summary>
        public int StartRow { get; set; } = 2;


        /// <summary>
        /// Einfügen, mit Anlegen von neuen Zeilen
        /// </summary>
        public bool LineAdd { get; set; } = true;


        /// <summary>
        /// Wie viele Leerzeilen sollen nach dem Ergebnis angefügt werden?
        /// </summary>
        public int EmptyRowCountAfter { get; set; } = 0;


        /// <summary>
        /// Callback zum Aufruf, nachdem ein Wert geschrieben wurde
        /// </summary>
        public AfterSetCellDelegate AfterSetCell { get; set; } = null;

        /// <summary>
        /// Callback zum Aufruf, nachdem eine Zeile geschrieben wurde
        /// </summary>
        public AfterSetRowDelegate AfterSetRow { get; set; } = null;


        /// <summary>
        /// Callback zum Aufruf, für das Formatieren einer Zeile
        /// </summary>
        public FormatRowDelegate FormatRow { get; set; } = null;


        /// <summary>
        /// Parameter, die auch dem Callback weitergereicht werden
        /// </summary>
        public Dictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();


        /// <summary>
        /// Sollen Werte anonymisiert werden
        /// </summary>
        public bool Anonymize { get; set; }


        /// <summary>
        /// Spalten aus dem DataSet sollen anonymisiert werden
        /// </summary>
        public List<string> AnonymizeColumns { get; set; } = new List<string>();


        /// <summary>
        /// Tabellenformat erstellen
        /// </summary>
        public bool CreateTable { get; set; } = true;



        /// <summary>
        /// Spalten generieren und den Header aus dem DataSet verwenden (für Pivot Auflistungen)
        /// </summary>
        public bool CreateColumnsWithHeader { get; set; } = false;


        /// <summary>
        /// Sart Spalte für das Erstellen von Spalten (für Pivot Auflistungen)
        /// </summary>
        public int CreateColumnsStart { get; set; } = 0;


        /// <summary>
        /// Die Tabelle wird um die Anzahl der Spalten erweitert
        /// z.B. wenn eine Summenspalte am Ende ist
        /// </summary>
        public int TableExpandColumns { get; set; } = 0;


        /// <summary>
        /// Aufhören bei Zeile (Debug)
        /// </summary>
        public int? StopAt { get; set;}


        /// <summary>
        /// Farbe für die Tabelle
        /// </summary>
        public string TableColor { get; set; } = TableColors.Green;


    }
}
