using System.Windows;

namespace Mi5hmasH.WpfHelper.ControlProperties;

public static class DropProperties
{
    public static readonly DependencyProperty DropOperationTypeProperty =
        DependencyProperty.RegisterAttached(
            "DropOperationType",
            typeof(string),
            typeof(DropProperties),
            new PropertyMetadata(null));

    public static void SetDropOperationType(UIElement element, string value)
        => element.SetValue(DropOperationTypeProperty, value);

    public static string GetDropOperationType(UIElement element)
        => (string)element.GetValue(DropOperationTypeProperty);
}