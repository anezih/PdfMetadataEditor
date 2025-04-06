namespace PdfMetadataEditor.Model;

public class Metadata
{
    public string? Title { get; set; }
    public string? Author { get; set; }
    public string? Subject { get; set; }
    public string? Keywords { get; set; }
    public string? Creator { get; set; }
    public string? Producer { get; set; }

    public Dictionary<string, string> ToDictionary()
    {
        return new Dictionary<string, string>()
        {
            { "title", Title ?? string.Empty },
            { "author", Author ?? string.Empty },
            { "subject", Subject ?? string.Empty },
            { "keywords", Keywords ?? string.Empty },
            { "creator", Creator ?? string.Empty },
            { "producer", Producer ?? string.Empty },
        };
    }
}
