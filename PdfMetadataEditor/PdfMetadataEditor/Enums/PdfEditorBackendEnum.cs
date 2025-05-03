using System.ComponentModel;

namespace PdfMetadataEditor.Enums
{
    public enum PdfEditorBackendEnum
    {
        iText = 0,

        PdfSharpCore = 1,

        [Description("MuPDF.js")]
        MuPDFjs = 2,
    }
}