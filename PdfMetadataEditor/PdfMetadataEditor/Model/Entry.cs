namespace PdfMetadataEditor.Model;

public class Entry
{
    public string Id = Guid.NewGuid().ToString();
    public string Heading { get; set; } = "";
    public int PageNo { get; set; }

    public List<Entry> SubHeadings { get; set; } = new();
}
