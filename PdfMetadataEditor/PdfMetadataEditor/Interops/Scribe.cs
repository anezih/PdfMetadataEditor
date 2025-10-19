using Microsoft.JSInterop;
using PdfMetadataEditor.Enums;

namespace PdfMetadataEditor.Interops;

public class Scribe : IAsyncDisposable
{
    private readonly IJSInProcessObjectReference _ref;
    private readonly IJSObjectReference module;

    public Scribe(IJSInProcessObjectReference _ref, IJSObjectReference module)
    {
        this._ref = _ref;
        this.module = module;
    }

    public async Task Init() => await _ref.InvokeVoidAsync("init");

    public async Task Clear() => await _ref.InvokeVoidAsync("clear");

    public async Task<byte[]> ExportData(ScribeExportFormat format = ScribeExportFormat.pdf, int minPage = 0, int maxPage = -1)
    {
        string _format = Enum.GetName(format)!;
        var res = await _ref.InvokeAsync<byte[]>("exportData", _format, minPage, maxPage);
        return res;
    }

    public async Task ImportFiles(byte[] pdfBytes)
    {
        await Clear();
        await module.InvokeVoidAsync("ImportPdf", pdfBytes);
    }

    public async Task Recognize(ScribeRecognitionOptions scribeRecognitionOptions)
    {
        await _ref.InvokeVoidAsync("recognize", scribeRecognitionOptions);
    }

    public async Task Terminate() => await _ref.InvokeVoidAsync("terminate");

    public async ValueTask DisposeAsync() => await Terminate();
}