using Sereno.Office.Word.SimpleStructure;

namespace Sereno.Office.Word.Word.SimpleStructure.Export
{
    public abstract class ExportBase
    {
        public virtual void Export(ExportOptions options)
        {
            foreach (DocumentGroup group in options.Groups)
            {
                ProcessGroup(group);
            }
        }

        protected abstract void ProcessGroup(DocumentGroup group);

    }
}
