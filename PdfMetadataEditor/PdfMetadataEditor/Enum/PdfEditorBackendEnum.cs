namespace PdfMetadataEditor.Enum
{
    public enum PdfEditorBackendEnum
    {
        iText = 0,
        PdfSharpCore = 1,
        //MuPDF = 1, // currently mupdf does not provide wasm native libs
    }
}