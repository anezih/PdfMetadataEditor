namespace PdfMetadataEditor.Model;

public class Entry
{
    public string Id = Guid.NewGuid().ToString();
    public string Heading { get; set; } = "";
    public int PageNo { get; set; }
    public bool IsBold { get; set; }
    public bool IsItalic { get; set; }
    public bool IsBoldItalic { get; set; }

    public List<Entry> SubHeadings { get; set; } = new();
}
