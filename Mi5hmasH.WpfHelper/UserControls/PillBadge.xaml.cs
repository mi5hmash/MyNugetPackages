using System.Windows;

namespace Mi5hmasH.WpfHelper.UserControls;

public partial class PillBadge
{
    public PillBadge()
    {
        InitializeComponent();
    }
    
    // Text
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(PillBadge));
    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }
    
    // FontWeight
    public new static readonly DependencyProperty FontWeightProperty =
        DependencyProperty.Register(nameof(FontWeight), typeof(FontWeight), typeof(PillBadge), new PropertyMetadata(FontWeights.Normal));

    public new FontWeight FontWeight
    {
        get => (FontWeight)GetValue(FontWeightProperty);
        set => SetValue(FontWeightProperty, value);
    }
}
