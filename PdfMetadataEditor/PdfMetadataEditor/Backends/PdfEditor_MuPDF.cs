//using Mu = MuPDF.NET;
//using PdfMetadataEditor.Interface;
//using PdfMetadataEditor.Model;

//namespace PdfMetadataEditor.Backends;;

//public class PdfEditor_MuPDF : IPdfEditor
//{
//    private Mu.Document? doc;

//    public bool IsCreated { get; set; } = false;

//    public void Create(byte[] pdfBytes)
//    {
//        doc = new(stream: pdfBytes);
//        IsCreated = true;
//    }

//    public List<Entry> GetOutline()
//    {
//        List<Entry> entries = new();

//        return entries;
//    }

//    public Metadata GetPdfMetadata()
//    {
//        var pdfMetadata = doc!.MetaData;
//        Metadata metadata = new Metadata
//        {
//            Author = pdfMetadata["author"] ?? string.Empty,
//            Creator = pdfMetadata["creator"] ?? string.Empty,
//            Keywords = pdfMetadata["keywords"] ?? string.Empty,
//            Producer = pdfMetadata["producer"] ?? string.Empty,
//            Subject = pdfMetadata["subject"] ?? string.Empty,
//            Title = pdfMetadata["title"] ?? string.Empty,
//        };
//        return metadata;
//    }

//    public byte[] SavePdf()
//    {
//        var res = doc!.Write();
//        return res;
//    }

//    public void SetOutline(List<Entry> entries)
//    {

//    }

//    public void SetPdfMetadata(Metadata metadata)
//    {
//        var metadataDictionary = metadata.ToDictionary();
//        doc!.SetMetadata(metadataDictionary);
//    }
//}
