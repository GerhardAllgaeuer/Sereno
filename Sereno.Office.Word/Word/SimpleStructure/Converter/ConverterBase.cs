using DocumentFormat.OpenXml.Drawing.Charts;
using Sereno.Office.Word.SimpleStructure;
using Sereno.Utilities;
using System.Collections.Generic;
using System.IO;

namespace Sereno.Office.Word.Word.SimpleStructure.Converter
{
    public abstract class ConverterBase
    {
        public Dictionary<string, byte[]> Files { get; set; } = new Dictionary<string, byte[]>();

        public string Document { get; set; }


        protected virtual void Init()
        {
        }

        protected virtual void Finish()
        {
        }


        public virtual void Convert(List<DocumentGroup> groups)
        {
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
