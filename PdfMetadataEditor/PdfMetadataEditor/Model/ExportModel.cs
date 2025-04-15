namespace PdfMetadataEditor.Model
{
    public class ExportModel
    {
        public Metadata? Metadata { get; set; }
        public int PageOffset { get; set; }
        public List<Entry>? Entries { get; set; }
    }
}
