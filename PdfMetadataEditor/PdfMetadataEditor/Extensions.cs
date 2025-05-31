using iText.Kernel.Pdf;
using System.ComponentModel;

namespace PdfMetadataEditor;

public static class Extensions
{
    public static string Description(this Enum enumValue)
    {
        var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());
        if (fieldInfo == null)
            return enumValue.ToString();
        if (Attribute.GetCustomAttribute(fieldInfo, typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
            return attribute.Description;
        return enumValue.ToString();
    }

    /// <summary>
    /// Sets the font style of <see cref="PdfOutline"/>. <paramref name="style"/> should be in the range of [1,3] where 1: Italic, 2: Bold, 3: Bold-Italic
    /// </summary>
    /// <param name="outline"></param>
    /// <param name="style"></param>
    /// <returns></returns>
    public static PdfOutline SetStyleEx(this PdfOutline outline, int style)
    {
        if (1 <= style && style <= 3)
        {
            outline.GetContent().Put(PdfName.F, new PdfNumber(style));
        }
        return outline;
    }
}