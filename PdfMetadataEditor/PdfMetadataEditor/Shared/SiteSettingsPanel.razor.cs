using Microsoft.FluentUI.AspNetCore.Components.Extensions;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace PdfMetadataEditor.Shared;
public partial class SiteSettingsPanel
{
    private DesignThemeModes _mode;
    public DesignThemeModes Mode
    {
        get { return _mode; }
        set
        {
            _mode = value;
            InvokeAsync(ChangeAntTheme);
        }
    }
    public OfficeColor? OfficeColor { get; set; }

    [Inject]
    private IJSRuntime? JSRuntime { get; set; }

    private static IEnumerable<DesignThemeModes> AllModes => Enum.GetValues<DesignThemeModes>();

    private static string? GetCustomColor(OfficeColor? color)
    {
        return color switch
        {
            null => OfficeColorUtilities.GetRandom(true).ToAttributeValue(),
            Microsoft.FluentUI.AspNetCore.Components.OfficeColor.Default => "#036ac4",
            _ => color.ToAttributeValue(),
        };
    }

    private async Task ChangeAntTheme()
    {
        if (Mode == DesignThemeModes.Dark)
        {
            await JSRuntime!.InvokeVoidAsync("changeAntThemeToDark");
        }
        else if (Mode == DesignThemeModes.Light)
        {
            await JSRuntime!.InvokeVoidAsync("changeAntThemeToLight");
        }
        else if (_mode == DesignThemeModes.System)
        {
            await JSRuntime!.InvokeVoidAsync("changeAntThemeToLight");
        }
    }
}