using Microsoft.FluentUI.AspNetCore.Components.Extensions;
using Microsoft.FluentUI.AspNetCore.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace PdfMetadataEditor.Shared;
public partial class SiteSettingsPanel
{
    public DesignThemeModes Mode { get; set; }
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

    private async Task OnLuminanceChanged(LuminanceChangedEventArgs e)
    {
        if (e.IsDark)
        {
            await JSRuntime!.InvokeVoidAsync("changeAntThemeToDark");
            StateHasChanged();
        }
        else
        {
            await JSRuntime!.InvokeVoidAsync("changeAntThemeToLight");
            StateHasChanged();
        }
    }
}