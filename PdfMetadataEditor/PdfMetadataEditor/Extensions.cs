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
}