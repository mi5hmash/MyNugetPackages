namespace Mi5hmasH.WpfHelper.Attributes;

/// <summary>
/// Defines a keyboard shortcut for a method.
/// </summary>
/// <param name="gesture">A keyboard shortcut like: <c>Ctrl+A</c></param>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class ShortcutAttribute(string gesture) : Attribute
{
    public string Gesture { get; } = gesture;
}
