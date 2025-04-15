using PdfMetadataEditor.Interface;
using PdfMetadataEditor.Model;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;

namespace PdfMetadataEditor.Backends;

public class PdfEditor_PdfSharpCore : IPdfEditor
{
    private PdfDocument? doc;
    private PdfPages? pages;

    public bool IsCreated { get; set; } = false;

    public void Create(byte[] pdfBytes)
    {
        MemoryStream inMs = new MemoryStream(pdfBytes);
        doc = PdfReader.Open(inMs, PdfSharpCore.Pdf.IO.enums.PdfReadAccuracy.Moderate);
        pages = doc.Pages;
        IsCreated = true;
    }

    public List<Entry> GetOutline()
    {
        List<Entry> entries = new();
        var outlines = doc!.Outlines;
        if (outlines.Count == 0)
            return entries;
        Stack<(PdfOutline, Entry)> stack = new();
        
        foreach (var topLevel in outlines)
        {
            Entry entry = new Entry
            {
                Heading = topLevel.Title,
                PageNo = GetPageNumberFromPdfPage(topLevel.DestinationPage)
            };
            entries.Add(entry);
            stack.Push((topLevel, entry));
        }

        while (stack.Count > 0)
        {
            var (currentOutline, currentEntry) = stack.Pop();
            foreach (var childOutline in currentOutline.Outlines)
            {
                Entry childEntry = new Entry
                {
                    Heading = childOutline.Title,
                    PageNo = GetPageNumberFromPdfPage(childOutline.DestinationPage),
                };
                currentEntry.SubHeadings.Add(childEntry);
                stack.Push((childOutline, childEntry));
            }
        }
        if
        (
            entries.Count == 1 && 
            entries[0].SubHeadings.Count == 0 & 
            entries[0].Heading == doc.Info.Title & 
            entries[0].PageNo == 1
        )
        {
            entries.Clear();
        }
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

    public void SetOutline(List<Entry> entries, int pageOffset = 0)
    {
        var outlines = doc!.Outlines;
        outlines.Clear();
        Stack<(PdfOutline, Entry)> stack = new();
        
        foreach (var entry in entries)
        {
            var outline = outlines.Add(entry.Heading, GetPdfPageFromPageNumber(entry.PageNo+pageOffset));
            stack.Push((outline, entry));
        }

        while (stack.Count > 0)
        {
            var (currentOutline, currentEntry) = stack.Pop();
            foreach (var subEntry in currentEntry.SubHeadings)
            {
                var subOutline = currentOutline.Outlines.Add(subEntry.Heading, GetPdfPageFromPageNumber(subEntry.PageNo+pageOffset));
                stack.Push((subOutline, subEntry));
            }
        }
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

    private int GetPageNumberFromPdfPage(PdfPage page)
    {
        for (int i = 0; i < pages!.Count; i++)
        {
            if (pages[i] == page)
            {
                return i + 1;
            }
        }
        return 1;
    }

    private PdfPage GetPdfPageFromPageNumber(int page)
    {
        int num = Math.Max(Math.Min(page, pages!.Count), 0);
        return pages[num - 1];
    }
}