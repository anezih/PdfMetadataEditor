using iText.Kernel.Pdf;

namespace PdfMetadataEditor;

public class Utils
{
    public static DateTime? FromPdfDate(string? pdfDate) => string.IsNullOrEmpty(pdfDate) ? DateTime.Now : PdfDate.Decode(pdfDate);
    public static string ToPdfDate(DateTime? pdfDate) => new PdfDate(pdfDate ?? DateTime.Now).GetPdfObject().GetValue();
}