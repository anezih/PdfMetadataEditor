using PdfMetadataEditor.Model;

namespace PdfMetadataEditor.Interface;

public interface IPdfEditor
{
    bool IsCreated { get; set; }
    bool IsBoldOutlineSupported { get; }
    bool IsItalicOutlineSupported { get; }
    bool IsBoldItalicOutlineSupported { get; }
    int LastPageNumber { get; }
    Task Create(byte[] pdfBytes);
    void SetOutline(List<Entry> entries, int pageOffset = 0);
    void SetPdfMetadata(Metadata metadata);
    List<Entry> GetOutline();
    Metadata GetPdfMetadata();
    byte[] SavePdf();
}