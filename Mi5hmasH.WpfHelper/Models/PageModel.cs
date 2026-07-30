using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace Mi5hmasH.WpfHelper.Models;

public class PageModel
{
    public required string Title { get; init; }
    public required string Icon { get; init; }
    public required Type ViewType { get; init; }
    public required Type ViewModelType { get; init; }
    public IRelayCommand? NavigateToCommand { get; set; }
    public KeyGesture? Hotkey { get; init; }
}