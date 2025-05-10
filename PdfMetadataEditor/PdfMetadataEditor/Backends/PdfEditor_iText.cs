using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Navigation;
using PdfMetadataEditor.Interface;
using PdfMetadataEditor.Model;

namespace PdfMetadataEditor.Backends;

public class PdfEditor_iText : IPdfEditor
{
    private MemoryStream outMs = new();
    private PdfDocument? pdfDocument;
    private PdfReader? reader;
    private PdfWriter? writer;
    private PdfNameTree? nameTree;

    public bool IsCreated { get; set; } = false;

    public int LastPageNumber => pdfDocument!.GetNumberOfPages();

    public async Task Create(byte[] pdfBytes)
    {
        MemoryStream inMs = new MemoryStream(pdfBytes);

        reader = new(inMs);
        writer = new(outMs);
        writer.SetCloseStream(false);
        pdfDocument = new(reader, writer);
        nameTree = pdfDocument.GetCatalog().GetNameTree(PdfName.Dests);

        IsCreated = true;

        await Task.CompletedTask;
    }

    private PdfExplicitDestination CreateDestinationFromPageNum(int pageNum)
        => PdfExplicitDestination.CreateFit(pdfDocument!.GetPage(pageNum));

    private int GetPageNumberFromDestination(PdfOutline outline)
    {
        var dest = outline.GetDestination();
        if (dest == null)
            return 1;
        return pdfDocument!.GetPageNumber((PdfDictionary)dest.GetDestinationPage(nameTree));
    }

    public List<Entry> GetOutline()
    {
        List<Entry> entries = new();

        if (!pdfDocument!.HasOutlines())
            return entries;
        PdfOutline rootOutline = pdfDocument.GetOutlines(true);
        var stack = new Stack<(PdfOutline, Entry)>();

        foreach (var topLevel in rootOutline.GetAllChildren())
        {
            Entry entry = new Entry
            {
                Heading = topLevel.GetTitle(),
                PageNo = GetPageNumberFromDestination(topLevel),
            };
            entries.Add(entry);
            stack.Push((topLevel, entry));
        }

        while (stack.Count > 0)
        {
            var (currentOutline, currentEntry) = stack.Pop();
            var children = currentOutline.GetAllChildren();

            foreach (var childOutline in children)
            {
                Entry childEntry = new Entry
                {
                    Heading = childOutline.GetTitle(),
                    PageNo = GetPageNumberFromDestination(childOutline),
                };
                currentEntry.SubHeadings.Add(childEntry);
                stack.Push((childOutline, childEntry));
            }
        }
        return entries;
    }

    public void SetOutline(List<Entry> entries, int pageOffset = 0)
    {
        // remove existing outline
        if (pdfDocument!.HasOutlines())
            pdfDocument.GetOutlines(true).RemoveOutline();
        pdfDocument.InitializeOutlines();
        PdfOutline rootOutline = pdfDocument.GetOutlines(true);

        var stack = new Stack<(PdfOutline, Entry)>();
        foreach (var entry in entries)
        {
            var childOutline = rootOutline.AddOutline(entry.Heading);
            childOutline.AddDestination(CreateDestinationFromPageNum(entry.PageNo+pageOffset));
            stack.Push((childOutline, entry));
        }

        while (stack.Count > 0)
        {
            var (currentOutline, currentEntry) = stack.Pop();
            foreach (var subEntry in currentEntry.SubHeadings)
            {
                var subOutline = currentOutline.AddOutline(subEntry.Heading);
                subOutline.AddDestination(CreateDestinationFromPageNum(subEntry.PageNo+pageOffset));
                stack.Push((subOutline, subEntry));
            }
        }
    }

    public Metadata GetPdfMetadata()
    {
        var docinfo = pdfDocument!.GetDocumentInfo();
        Metadata metadata = new Metadata
        {
            Author           = docinfo.GetAuthor(),
            Creator          = docinfo.GetCreator(),
            Keywords         = docinfo.GetKeywords(),
            Subject          = docinfo.GetSubject(),
            Title            = docinfo.GetTitle(),
            Producer         = docinfo.GetProducer(),
            CreationDate     = Utils.FromPdfDate(docinfo.GetMoreInfo("CreationDate")),
            ModificationDate = Utils.FromPdfDate(docinfo.GetMoreInfo("ModDate")),
        };
        return metadata;
    }

    public void SetPdfMetadata(Metadata metadata)
    {
        var docinfo = pdfDocument!.GetDocumentInfo();

        docinfo.SetAuthor(metadata.Author ?? string.Empty);
        docinfo.SetCreator(metadata.Creator ?? string.Empty);
        docinfo.SetKeywords(metadata.Keywords ?? string.Empty);
        docinfo.SetSubject(metadata.Subject ?? string.Empty);
        docinfo.SetTitle(metadata.Title ?? string.Empty);
        docinfo.SetMoreInfo("CreationDate", Utils.ToPdfDate(metadata.CreationDate));
        docinfo.SetMoreInfo("ModDate", Utils.ToPdfDate(metadata.ModificationDate));
    }

    public byte[] SavePdf()
    {
        pdfDocument!.Close();
        reader!.Close();
        writer!.Close();
        var res = outMs.ToArray();
        outMs = new();
        return res;
    }
}