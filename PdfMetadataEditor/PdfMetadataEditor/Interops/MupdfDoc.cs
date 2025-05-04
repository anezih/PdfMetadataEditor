using Microsoft.JSInterop;

namespace PdfMetadataEditor.Interops;

public class MupdfDoc : IAsyncDisposable
{
    private readonly IJSInProcessObjectReference docRef;
    private readonly IJSInProcessObjectReference outlineIterator;

    public MupdfDoc(IJSInProcessObjectReference docRef)
    {
        this.docRef = docRef;
        this.outlineIterator = GetOutlineIterator(docRef);
    }

    #region Metadata

    public int LastPageNumber => docRef.Invoke<int>("countPages");

    public string GetTitle() => docRef.Invoke<string>("getMetaData", "info:Title");
    public string GetAuthor() => docRef.Invoke<string>("getMetaData", "info:Author");
    public string GetSubject() => docRef.Invoke<string>("getMetaData", "info:Subject");
    public string GetKeywords() => docRef.Invoke<string>("getMetaData", "info:Keywords");
    public string GetCreator() => docRef.Invoke<string>("getMetaData", "info:Creator");
    public string GetProducer() => docRef.Invoke<string>("getMetaData", "info:Producer");

    public void SetTitle(string title) => docRef.InvokeVoid("setMetaData", "info:Title", title);
    public void SetAuthor(string author) => docRef.InvokeVoid("setMetaData", "info:Author", author);
    public void SetSubject(string subject) => docRef.InvokeVoid("setMetaData", "info:Subject", subject);
    public void SetKeywords(string keywords) => docRef.InvokeVoid("setMetaData", "info:Keywords", keywords);
    public void SetCreator(string creator) => docRef.InvokeVoid("setMetaData", "info:Creator", creator);

    #endregion

    #region Outline

    public List<MuOutlineItem>? GetOutline()
    {
        return docRef.Invoke<List<MuOutlineItem>>("loadOutline");
    }

    public void SetOutline(List<MuOutlineItem> outline)
    {
        ClearOutline();
        var stack = new Stack<(MuOutlineItem, int)>();

        foreach (var item in outline)
            stack.Push((item, 0)); // Start at depth 0

        int currentDepth = 0;

        while (stack.Count > 0)
        {
            var (currentItem, itemDepth) = stack.Pop();

            while (currentDepth < itemDepth)
            {
                outlineIterator.InvokeVoid("down");
                currentDepth++;
            }

            while (currentDepth > itemDepth)
            {
                outlineIterator.InvokeVoid("up");
                currentDepth--;
            }

            outlineIterator.InvokeVoid("insert", currentItem);
            outlineIterator.InvokeVoid("prev");

            if (currentItem.Down != null)
            {
                for (int i = 0; i < currentItem.Down.Count; i++)
                {
                    stack.Push((currentItem.Down[i], itemDepth + 1));
                }
            }
        }
    }

    private void ClearOutline()
    {
        var firstItem = outlineIterator.Invoke<MuOutlineItem>("item");
        if (firstItem != null)
        {
            var result = outlineIterator.Invoke<int>("delete");
            while (result == 0)
                result = outlineIterator.Invoke<int>("delete");
        }
    }

    public string FormatLinkURIForPageNumber(int pageNumber)
    {
        return docRef.Invoke<string>("formatLinkURI", new {page = pageNumber, type = "XYZ"});
    }

    private IJSInProcessObjectReference GetOutlineIterator(IJSInProcessObjectReference docRef)
    {
        return docRef.Invoke<IJSInProcessObjectReference>("outlineIterator");
    }

    #endregion

    public byte[] Save()
    {
        var buffer = docRef.Invoke<IJSInProcessObjectReference>("saveToBuffer");
        byte[] bytes = buffer.Invoke<byte[]>("asUint8Array");
        return bytes;
    }

    public async ValueTask DisposeAsync() => await docRef.InvokeVoidAsync("destroy");
}