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

    public async Task<Scribe> GetScribe(int workerN = 6)
    {
        var module = await moduleTask.Value;
        var obj = await module.InvokeAsync<IJSInProcessObjectReference>("getScribe", workerN);
        return new Scribe(obj, module);
    }
}
