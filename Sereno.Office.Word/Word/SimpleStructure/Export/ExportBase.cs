using Sereno.Office.Word.SimpleStructure;
using System.Collections.Generic;

namespace Sereno.Office.Word.Word.SimpleStructure.Export
{
    public abstract class ExportBase
    {
        protected ExportOptions Options { get; set; }

        protected virtual void Init()
        {
        }

        protected virtual void Finish()
        {
        }


        public virtual void Export(List<DocumentGroup> groups, ExportOptions options)
        {
            this.Options = options;

            if (this.Options == null)
                this.Options = new ExportOptions();


            this.Init();

            foreach (DocumentGroup group in groups)
            {
                ProcessGroup(group);
            }

            this.Finish();
        }


        protected abstract void ProcessGroup(DocumentGroup group);

    }
}
