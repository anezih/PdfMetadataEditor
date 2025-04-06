using PdfMetadataEditor.Model;

namespace PdfMetadataEditor.Interface;

public interface IPdfEditor
{
    bool IsCreated { get; set; }
    void Create(byte[] pdfBytes);
    void SetOutline(List<Entry> entries);
    void SetPdfMetadata(Metadata metadata);
    List<Entry> GetOutline();
    Metadata GetPdfMetadata();
    byte[] SavePdf();
}