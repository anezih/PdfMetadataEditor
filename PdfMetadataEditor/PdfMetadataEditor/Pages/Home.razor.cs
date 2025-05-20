using AntDesign;
using KristofferStrube.Blazor.FileAPI;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.JSInterop;
using PdfMetadataEditor.Backends;
using PdfMetadataEditor.Enums;
using PdfMetadataEditor.Interface;
using PdfMetadataEditor.Model;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace PdfMetadataEditor.Pages;
public partial class Home
{
    [Inject]
    private IJSRuntime? JS {  get; set; }

    [Inject]
    private IURLService? URL {  get; set; }

    [Inject]
    private IToastService? ToastService { get; set; }

    BlobInProcess? blobInProcess;
    List<Entry> entries = new();
    Metadata metadata = new();
    IPdfEditor pdfEditor = new PdfEditor_iText();

    FluentMenu? outlineContextMenu;
    Tree<Entry>? tree;
    TreeNode<Entry>? selectedNode;

    byte[]? pdfBytes;

    bool disablePageJump = true;
    bool isConfirmPopoverOpen = false;
    bool isEditorInitialized = false;
    bool isHelpOverlayOpen = false;
    bool isSaving = false;
    bool showPdfViewer = false;
    bool tocButtonsDisabled = false;

    int lastPdfPage = 1;
    int pageOffset = 0;

    string lastEditNodeId = string.Empty;
    string baseBlobUri = string.Empty;
    string blobUri = string.Empty;
    string originalFileName = string.Empty;

    private JsonSerializerOptions jsonOptions = new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };

    private static List<Option<PdfEditorBackendEnum>> backendOptions = Enum.GetValues<PdfEditorBackendEnum>()
        .Select(x => new Option<PdfEditorBackendEnum> { Value = x, Text = x.Description(), Selected = x == PdfEditorBackendEnum.iText })
        .ToList();

    private void collapseAll()
    {
        if (tree != null)
            tree.CollapseAll();
    }

    private void onBackendSelectionChanged(Option<PdfEditorBackendEnum> backendEnum)
    {
        pdfEditor = backendEnum.Value switch
        {
            PdfEditorBackendEnum.iText => new PdfEditor_iText(),
            PdfEditorBackendEnum.PdfSharpCore => new PdfEditor_PdfSharpCore(),
            PdfEditorBackendEnum.MuPDFjs => new PdfEditor_MuPDFjs(JS!),
            _ => throw new ArgumentException("Invalid enum value: ", backendEnum.Value.ToString()),
        };
    }

    private void onDoubleClick(TreeEventArgs<Entry> e)
    {
        lastEditNodeId = e.Node.DataItem.Id;
        StateHasChanged();
    }

    private void onDoubleClick2(string id)
    {
        lastEditNodeId = id;
        StateHasChanged();
    }

    private void onDrop(TreeEventArgs<Entry> e) => StateHasChanged();

    private void onEditEnd()
    {
        lastEditNodeId = string.Empty;
        StateHasChanged();
    }

    private void expandAll()
    {
        if (tree != null)
            tree.ExpandAll();
    }

    private void clearOutline()
    {
        isConfirmPopoverOpen = false;
        entries.Clear();
        StateHasChanged();
    }

    private void onEscDown(KeyboardEventArgs e)
    {
        if (e.Key == "Escape")
        {
            lastEditNodeId = string.Empty;
            StateHasChanged();
        }
        if (e.CtrlKey & e.Key == "Enter")
        {
            lastEditNodeId = string.Empty;
            StateHasChanged();
        }
    }

    private void onSelect(TreeEventArgs<Entry> e)
    {
        selectedNode = e.Node;
    }

    private void onTabChanged(FluentTab tab)
    {
        tocButtonsDisabled = tab.Id == "tab-2";
        StateHasChanged();
    }

    void onClickGoToPage(int pageNum)
    {
        if (!disablePageJump && !string.IsNullOrEmpty(blobUri) & pageNum != lastPdfPage)
        {
            blobUri = $"{baseBlobUri}#page={pageNum}";
            lastPdfPage = pageNum;
            StateHasChanged();
        }
    }

    private async Task CreateBlobUris()
    {
        blobInProcess = await BlobInProcess.CreateAsync(
            JS!,
            blobParts: new BlobPart[] { pdfBytes! },
            options: new() { Type = "application/pdf" }
        );
        await RevokeBlobUris();
        baseBlobUri = await URL!.CreateObjectURLAsync(blobInProcess);
        blobUri = $"{baseBlobUri}";
    }

    private async Task RevokeBlobUris()
    {
        if (!string.IsNullOrEmpty(baseBlobUri))
        {
            await URL!.RevokeObjectURLAsync(baseBlobUri);
            baseBlobUri = string.Empty;
            blobUri = string.Empty;
        }
    }

    private async Task<bool> InitializePdf()
    {
        try
        {
            await Task.Yield();
            await pdfEditor.Create(pdfBytes!);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            return false;
        }

        try
        {
            metadata = pdfEditor.GetPdfMetadata();
        }
        catch (Exception)
        {
            ToastService!.ShowToast(ToastIntent.Error, $"Could not parse metadata with the current Pdf Editor Backend, check the console logs.");
        }

        try
        {
            entries = pdfEditor.GetOutline();
        }
        catch (Exception)
        {
            ToastService!.ShowToast(ToastIntent.Error, $"Could not parse outline with the current Pdf Editor Backend, check the console logs.");
        }

        return true;
    }

    private async Task SaveAndReloadPdf()
    {
        if (pdfEditor.IsCreated && PageOffsetSanityCheck())
        {
            isSaving = true;
            await Task.Delay(1);
            pdfEditor.SetPdfMetadata(metadata);
            pdfEditor.SetOutline(entries, pageOffset);
            pdfBytes = pdfEditor.SavePdf();
            await CreateBlobUris();
            var pdfInitializationResult = await InitializePdf();
            pageOffset = 0;
            isSaving = false;
        }
    }

    private async Task DownloadPdf()
    {
        if (!string.IsNullOrEmpty(baseBlobUri))
        {
            string baseName = Path.GetFileNameWithoutExtension(originalFileName);
            string ext = Path.GetExtension(originalFileName);
            string downloadName = $"{baseName}_Edited{ext}";
            await JS!.InvokeVoidAsync("downloadFile", downloadName, baseBlobUri);
        }
    }

    private async Task OnFileUploadCompletedAsync(IEnumerable<FluentInputFileEventArgs> files)
    {
        var pdfFile = files.First();
        originalFileName = pdfFile.Name;
        pdfBytes = new byte[pdfFile.Stream!.Length];
        await pdfFile.Stream!.ReadExactlyAsync(pdfBytes);
        await CreateBlobUris();
        var pdfInitializationResult = await InitializePdf();
        if (!pdfInitializationResult)
        {
            await RevokeBlobUris();
            ToastService!.ShowToast(ToastIntent.Error, $"Could not initialize editor with the current Pdf Editor Backend, check the console logs.");
        }
        showPdfViewer = pdfInitializationResult;
        isEditorInitialized = pdfInitializationResult;
    }

    private async Task ExportChanges()
    {
        ExportModel exportModel = new ExportModel { Entries = entries, Metadata = metadata, PageOffset = pageOffset };
        byte[] json = JsonSerializer.SerializeToUtf8Bytes<ExportModel>(exportModel, jsonOptions);

        var exportJson = await BlobInProcess.CreateAsync(
            JS!,
            blobParts: new BlobPart[] { json },
            options: new() { Type = "text/json" }
        );
        string jsonUrl = await URL!.CreateObjectURLAsync(exportJson);
        string date = DateTime.Now.ToString("ddMMyyyy_HHmm");
        string originalFileNameSafe = string.Join('_', Path.GetFileNameWithoutExtension(originalFileName).Split(Path.GetInvalidFileNameChars()));
        string downloadName = $"PdfMetadataEditor_{originalFileNameSafe}_{date}.json";
        await JS!.InvokeVoidAsync("downloadFile", downloadName, jsonUrl);
        await URL.RevokeObjectURLAsync(jsonUrl);
    }

    private async Task ImportChanges(IEnumerable<FluentInputFileEventArgs> files)
    {
        ExportModel? exportModel = null;
        try
        {
            var jsonFile = files.First();
            if (!string.IsNullOrEmpty(jsonFile.ErrorMessage))
            {
                Console.WriteLine($"Error at Json file import: {jsonFile.ErrorMessage}");
                ToastService!.ShowToast(ToastIntent.Error, $"Error at Json file import: {jsonFile.ErrorMessage}");
                return;
            }
            var jsonBytes = new byte[jsonFile.Stream!.Length];
            await jsonFile.Stream!.ReadExactlyAsync(jsonBytes);
            exportModel = JsonSerializer.Deserialize<ExportModel>(jsonBytes, jsonOptions);
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Invalid Json file: {ex.Message}");
            ToastService!.ShowToast(ToastIntent.Error, $"Invalid Json file");
        }

        if (exportModel != null)
        {
            entries = exportModel.Entries ?? new List<Entry>();
            metadata = exportModel.Metadata ?? new Metadata();
            pageOffset = exportModel.PageOffset;
        }
    }

    public class BackendStyleSupport()
    {
        [Display(Name = "Backend")]
        public string? Backend { get; set; }
        [Display(Name = "Bold")]
        public bool Bold { get; set; }
        [Display(Name = "Italic")]
        public bool Italic { get; set; }
        [Display(Name = "Bold & Italic")]
        public bool BoldItalic { get; set; }
    }

    public IQueryable<BackendStyleSupport> GetBackendStyleSupport()
    {
        return new[]
        {
            new BackendStyleSupport() { Backend = "iText", Bold = true, Italic = true, BoldItalic = false },
            new BackendStyleSupport() { Backend = "PdfSharpCore", Bold = true, Italic = true, BoldItalic = true },
            new BackendStyleSupport() { Backend = "MuPDF.js", Bold = false, Italic = false, BoldItalic = false },
        }.AsQueryable();
    }

    private bool PageOffsetSanityCheck()
    {
        if (pageOffset == 0 || entries.Count == 0)
            return true;
        int minPage = int.MaxValue;
        int maxPage = int.MinValue;
        var stack = new Stack<Entry>();
        foreach (var entry in entries)
        {
            stack.Push(entry);
        }

        while (stack.Count > 0)
        {
            var currentEntry = stack.Pop();
            minPage = Math.Min(currentEntry.PageNo, minPage);
            maxPage = Math.Max(currentEntry.PageNo, maxPage);

            foreach (var subEntry in currentEntry.SubHeadings)
            {
                stack.Push(subEntry);
            }
        }

        if (minPage + pageOffset <= 0)
        {
            ToastService!.ShowToast(ToastIntent.Error, $"Applying the page offset will result in the lowest page number dropping below 1.");
            return false;
        }

        if (maxPage + pageOffset > pdfEditor.LastPageNumber)
        {
            ToastService!.ShowToast(ToastIntent.Error, $"Applying the page offset will result in the highest page number to exceed the total number PDF pages.");
            return false;
        }

        return true;
    }

    private void addNewEntry()
    {
        if (showPdfViewer && entries!.Count == 0)
        {
            entries.Add(new() { Heading = "Heading 1", PageNo = 1 });
            return;
        }
        if (selectedNode != null)
        {
            string id = selectedNode.DataItem.Id;
            int knownDepth = selectedNode.TreeLevel;
            var stack = new Stack<(Entry node, List<Entry> parentList, int index, int depth)>();

            for (int i = entries.Count - 1; i >= 0; i--)
            {
                stack.Push((entries[i], entries, i, 0));
            }

            while (stack.Count > 0)
            {
                var (node, parentList, index, depth) = stack.Pop();
                if (depth == knownDepth && node.Id == id)
                {
                    parentList.Insert
                    (
                        index + 1,
                        new() { Heading = $"{selectedNode.DataItem.Heading} (Copy)", PageNo = Math.Min(selectedNode.DataItem.PageNo + 1, pdfEditor.LastPageNumber) }
                    );
                    StateHasChanged();
                    return;
                }

                if (depth < knownDepth && node.SubHeadings != null && node.SubHeadings.Count > 0)
                {
                    for (int i = node.SubHeadings.Count - 1; i >= 0; i--)
                    {
                        stack.Push((node.SubHeadings[i], node.SubHeadings, i, depth + 1));
                    }
                }
            }
        }
    }

    private void deleteEntry()
    {
        if (selectedNode != null)
        {
            string id = selectedNode.DataItem.Id;
            int knownDepth = selectedNode.TreeLevel;
            var stack = new Stack<(Entry node, List<Entry> parentList, int index, int depth)>();

            for (int i = entries!.Count - 1; i >= 0; i--)
            {
                stack.Push((entries[i], entries, i, 0));
            }

            while (stack.Count > 0)
            {
                var (node, parentList, index, depth) = stack.Pop();
                if (depth == knownDepth && node.Id == id)
                {
                    parentList.Remove(node);
                    StateHasChanged();
                    return;
                }
                if (depth < knownDepth && node.SubHeadings != null && node.SubHeadings.Count > 0)
                {
                    for (int i = node.SubHeadings.Count - 1; i >= 0; i--)
                    {
                        stack.Push((node.SubHeadings[i], node.SubHeadings, i, depth + 1));
                    }
                }
            }
        }
    }

    protected override async Task OnInitializedAsync()
    {
        bool isDark = await JS!.InvokeAsync<bool>("isDark");
        if (isDark)
            await JS!.InvokeVoidAsync("changeAntThemeToDark");
    }
}