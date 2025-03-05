namespace Sereno.Office.Word.SimpleStructure
{
    public class ColumnInfo
    {
        public string ColumnName { get; set; } = "";

        public double? ColumnWidth { get; set; }

        public override string ToString()
        {
            return this.ColumnName;
        }
    }
}
