using Sereno.Office.Word.SimpleStructure;

namespace Sereno.Office.Word.Word.SimpleStructure.Export
{
    public abstract class ExportBase
    {
        public ExportOptions Options { get; set; }

        protected virtual void Init()
        {
        }

        protected virtual void Finish()
        {
        }


        public virtual void Export(ExportOptions options)
        {
            this.Options = options;

            if (options == null)
                options = new ExportOptions();


            this.Init();

            foreach (DocumentGroup group in options.Groups)
            {
                ProcessGroup(group);
            }

            this.Finish();
        }


        protected abstract void ProcessGroup(DocumentGroup group);

    }
}
