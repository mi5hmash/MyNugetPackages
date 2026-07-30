using System.Windows.Input;

namespace Mi5hmasH.WpfHelper.Models;

public class ShortcutCommandModel
{
    public required ICommand Command { get; init; }
    public required KeyGesture Gesture { get; init; }
}