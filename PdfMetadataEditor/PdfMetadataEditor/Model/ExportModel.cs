namespace PdfMetadataEditor.Model
{
    public class ExportModel
    {
        public required Metadata? Metadata { get; set; }
        public required int PageOffset { get; set; }
        public required List<Entry>? Entries { get; set; }
    }
}
