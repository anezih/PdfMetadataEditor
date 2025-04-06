using PdfMetadataEditor.Interface;
using PdfMetadataEditor.Model;

namespace PdfMetadataEditor.Backends;

public class PdfEditor_PdfSharpCore : IPdfEditor
{
    PdfSharpCore.Pdf.PdfDocument? doc;

    public bool IsCreated { get; set; } = false;

    public void Create(byte[] pdfBytes)
    {
        MemoryStream inMs = new MemoryStream(pdfBytes);
        doc = PdfSharpCore.Pdf.IO.PdfReader.Open(inMs);
        IsCreated = true;
    }

    public List<Entry> GetOutline()
    {
        List<Entry> entries = new();

        return entries;
    }

    public Metadata GetPdfMetadata()
    {
        var docInfo = doc!.Info;
        Metadata metadata = new Metadata
        {
            Author = docInfo.Author,
            Creator = docInfo.Creator,
            Keywords = docInfo.Keywords,
            Subject = docInfo.Subject,
            Title = docInfo.Title,
            Producer = docInfo.Producer,
        };
        return metadata;
    }

    public byte[] SavePdf()
    {
        MemoryStream outMs = new MemoryStream();
        doc!.Save(outMs, closeStream: false);
        doc.Dispose();
        doc.Close();
        return outMs.ToArray();
    }

    public void SetOutline(List<Entry> entries)
    {

    }

    public void SetPdfMetadata(Metadata metadata)
    {
        var docInfo = doc!.Info;
        docInfo.Author = metadata.Author ?? string.Empty;
        docInfo.Creator = metadata.Creator ?? string.Empty;
        docInfo.Keywords = metadata.Keywords ?? string.Empty;
        docInfo.Subject = metadata.Subject ?? string.Empty;
        docInfo.Title = metadata.Title ?? string.Empty;
    }
}