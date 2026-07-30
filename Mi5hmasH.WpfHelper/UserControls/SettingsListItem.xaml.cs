using System.Windows;
using System.Windows.Media;

namespace Mi5hmasH.WpfHelper.UserControls;

public partial class SettingsListItem
{
    public SettingsListItem()
    {
        InitializeComponent();
    }

    // FontFamily
    public static readonly DependencyProperty IconFontProperty =
        DependencyProperty.Register(nameof(IconFont), typeof(FontFamily), typeof(SettingsListItem));
    public FontFamily IconFont
    {
        get => (FontFamily)GetValue(IconFontProperty);
        set => SetValue(IconFontProperty, value);
    }


    // Icon
    public static readonly DependencyProperty IconProperty =
        DependencyProperty.Register(nameof(Icon), typeof(string), typeof(SettingsListItem));
    public string Icon
    {
        get => (string)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    // Title
    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(SettingsListItem));
    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    // Caption
    public static readonly DependencyProperty CaptionProperty =
        DependencyProperty.Register(nameof(Caption), typeof(string), typeof(SettingsListItem));
    public string Caption
    {
        get => (string)GetValue(CaptionProperty);
        set => SetValue(CaptionProperty, value);
    }
    
    // SettingControl
    public static readonly DependencyProperty SettingControlProperty =
        DependencyProperty.Register(nameof(SettingControl), typeof(UIElement), typeof(SettingsListItem));
    public UIElement SettingControl
    {
        get => (UIElement)GetValue(SettingControlProperty);
        set => SetValue(SettingControlProperty, value);
    }
}