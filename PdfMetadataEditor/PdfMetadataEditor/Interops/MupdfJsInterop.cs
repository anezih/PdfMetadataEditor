using Microsoft.JSInterop;

namespace PdfMetadataEditor.Interops;

public class MupdfJsInterop : IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> moduleTask;

    public MupdfJsInterop(IJSRuntime jsRuntime)
    {
        moduleTask = new Lazy<Task<IJSObjectReference>>(() => jsRuntime.InvokeAsync<IJSObjectReference>("import", "/js/mupdfWrapper.js").AsTask());
    }

    public async Task<MupdfDoc> Initialize(byte[] data)
    {
        var module = await moduleTask.Value;
        var doc = await module.InvokeAsync<IJSInProcessObjectReference>("initialize", data);
        return new MupdfDoc(doc);
    }

    public async ValueTask DisposeAsync()
    {
        if (moduleTask.IsValueCreated)
        {
            var module = await moduleTask.Value;
            await module.DisposeAsync();
        }
    }
}