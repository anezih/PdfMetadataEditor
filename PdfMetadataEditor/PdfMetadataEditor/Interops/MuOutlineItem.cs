namespace PdfMetadataEditor.Interops;

public class MuOutlineItem
{
    public string? Title { get; set; }
    public string? Uri { get; set; }
    public bool Open { get; set; } = true;
    public List<MuOutlineItem>? Down { get; set; }

    /// <summary>
    /// 0-indexed
    /// </summary>
    public int Page { get; set; }

    public override string ToString()
    {
        return $"MuOutlineItem {{ Title: {Title}, Uri: {Uri}, Open: {Open}, Down: {Down?.Count ?? 0} items, Page: {Page+1} }}";
    }
}