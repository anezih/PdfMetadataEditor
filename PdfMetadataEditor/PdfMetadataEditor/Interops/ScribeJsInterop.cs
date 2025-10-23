using Microsoft.JSInterop;

namespace PdfMetadataEditor.Interops;

public class ScribeJsInterop
{
    private readonly Lazy<Task<IJSObjectReference>> moduleTask;
    private readonly IJSRuntime jsRuntime;

    public ScribeJsInterop(IJSRuntime jsRuntime)
    {
        moduleTask = new Lazy<Task<IJSObjectReference>>(() => jsRuntime.InvokeAsync<IJSObjectReference>("import", "./js/scribeWrapper.js").AsTask());
        this.jsRuntime = jsRuntime;
    }

    public async Task<Scribe> GetScribe<T>(DotNetObjectReference<T> dotNetObjectReference, int workerN = 6) where T : class
    {
        var module = await moduleTask.Value;
        var obj = await module.InvokeAsync<IJSInProcessObjectReference>("getScribe", workerN, dotNetObjectReference);
        return new Scribe(obj, module);
    }
}
