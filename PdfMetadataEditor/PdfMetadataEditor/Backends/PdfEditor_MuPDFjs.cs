using Microsoft.JSInterop;
using PdfMetadataEditor.Interface;
using PdfMetadataEditor.Interops;
using PdfMetadataEditor.Model;

namespace PdfMetadataEditor.Backends;

public class PdfEditor_MuPDFjs : IPdfEditor
{
    private MupdfJsInterop? mupdfJsInterop;
    private MupdfDoc? mupdfDoc { get; set; }

    public bool IsCreated { get; set; } = false;

    public int LastPageNumber => mupdfDoc!.LastPageNumber;

    public PdfEditor_MuPDFjs(IJSRuntime js)
    {
        this.mupdfJsInterop = new MupdfJsInterop(js);
    }

    public async Task Create(byte[] pdfBytes)
    {
        mupdfDoc = await mupdfJsInterop!.Initialize(pdfBytes);
        IsCreated = true;
    }

    public List<Entry> GetOutline()
    {
        List<Entry> entries = new();

        var outlines = mupdfDoc?.GetOutline();
        if (outlines == null)
            return entries;
        var stack = new Stack<(MuOutlineItem, Entry)>();
        foreach (var topLevel in outlines!)
        {
            Entry entry = new Entry
            {
                Heading = topLevel.Title!,
                PageNo = topLevel.Page+1,
            };
            entries.Add(entry);
            stack.Push((topLevel, entry));
        }
        while (stack.Count > 0)
        {
            var (currentOutline, currentEntry) = stack.Pop();
            if (currentOutline.Down == null)
                continue;
            foreach (var childOutline in currentOutline.Down)
            {
                Entry childEntry = new Entry
                {
                    Heading = childOutline.Title!,
                    PageNo = childOutline.Page + 1,
                };
                currentEntry.SubHeadings.Add(childEntry);
                stack.Push((childOutline, childEntry));
            }
        }
        return entries;
    }

    public Metadata GetPdfMetadata()
    {
        Metadata metadata = new Metadata
        {
            Author = mupdfDoc?.GetAuthor(),
            Creator = mupdfDoc?.GetCreator(),
            Keywords = mupdfDoc?.GetKeywords(),
            Subject = mupdfDoc?.GetSubject(),
            Title = mupdfDoc?.GetTitle(),
            Producer = mupdfDoc?.GetProducer(),
        };
        return metadata;
    }

    public byte[] SavePdf()
    {
        return mupdfDoc!.Save();
    }

    public void SetOutline(List<Entry> entries, int pageOffset = 0)
    {
        List<MuOutlineItem> muOutline = new();

        var stack = new Stack<(MuOutlineItem, Entry)>();
        foreach (var entry in entries)
        {
            var pageNum = entry.PageNo - 1 + pageOffset;
            MuOutlineItem muOutlineItem = new MuOutlineItem { Title = entry.Heading, Uri = mupdfDoc!.FormatLinkURIForPageNumber(pageNum) };
            muOutline.Add(muOutlineItem);
            stack.Push((muOutlineItem, entry));
        }
        while (stack.Count > 0)
        {
            var (currentOutline, currentEntry) = stack.Pop();
            foreach (var subEntry in currentEntry.SubHeadings)
            {
                var pageNum = subEntry.PageNo - 1 + pageOffset;
                MuOutlineItem muOutlineItem = new MuOutlineItem { Title = subEntry.Heading, Uri = mupdfDoc!.FormatLinkURIForPageNumber(pageNum) };
                if (currentOutline.Down == null)
                    currentOutline.Down = new();
                currentOutline.Down.Add(muOutlineItem);
                stack.Push((muOutlineItem, subEntry));
            }
        }
        mupdfDoc?.SetOutline(muOutline);
    }

    public void SetPdfMetadata(Metadata metadata)
    {
        mupdfDoc?.SetAuthor(metadata.Author ?? string.Empty);
        mupdfDoc?.SetCreator(metadata.Creator ?? string.Empty);
        mupdfDoc?.SetKeywords(metadata.Keywords ?? string.Empty);
        mupdfDoc?.SetSubject(metadata.Subject ?? string.Empty);
        mupdfDoc?.SetTitle(metadata?.Title ?? string.Empty);
    }
}