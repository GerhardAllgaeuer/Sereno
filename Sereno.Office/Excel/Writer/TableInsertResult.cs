using ClosedXML.Excel;

namespace Sereno.Office.Excel.Writer
{
    public class TableInsertResult
    {
        public IXLRow HeaderRow { get; set; }

        public IXLRow FirstRow { get; set; }
        public IXLRow LastRow { get; set; }

        public IXLCell FirstCell { get; set; }
        public IXLCell LastCell { get; set; }

    }
}
