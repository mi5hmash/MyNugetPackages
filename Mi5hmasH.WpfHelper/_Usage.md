# USAGE
## DropProperties
### In "MainWindow.xaml.cs" add region
```csharp
#region FILE_DROP

    private void FileDrop_Drop(object sender, DragEventArgs e)
    {
        if (e.Data is not DataObject dataObject || !dataObject.ContainsFileDropList()) return;
        if (sender is not UIElement element) return;
        var dropOperationType = DropProperties.GetDropOperationType(element);
        ViewModel.OnFileDrop(dropOperationType, dataObject.GetFileDropList());
    }

    private void FileDrop_PreviewDragEnter(object sender, DragEventArgs e)
    {
        if (e.Data is not DataObject dataObject || !dataObject.ContainsFileDropList()) return;
        e.Effects = DragDropEffects.Copy;
    }

    private void FileDrop_PreviewDragOver(object sender, DragEventArgs e) => e.Handled = true;
    
#endregion
```
### In "MainWindow.xaml" add namespace to Window and properties to a container like this:
```xml
<Window
    xmlns:p="clr-namespace:Mi5hmasH.WpfHelper.ControlProperties;assembly=Mi5hmasH.WpfHelper">
    <Grid
        p:DropProperties.DropOperationType="FileDropped"
        AllowDrop="True"
        Drop="FileDrop_Drop"
        PreviewDragEnter="FileDrop_PreviewDragEnter"
        PreviewDragOver="FileDrop_PreviewDragOver">
        <!-- Your UI elements go here -->
    </Grid>
</Window>
```
### In "MainWindowViewModel.cs" add a method
```csharp
public void OnFileDrop(string operationType, StringCollection filePaths)
{
    if (filePaths.Count < 1) return;
    var path = filePaths[0] ?? string.Empty;
    if (operationType != "FileDropped") return;
    var extension = Path.GetExtension(path);
    // Handle the dropped file logic goes here
}
```

## Converters
### In "App.xaml" add the following:
```xml
<Application xmlns:c="clr-namespace:Mi5hmasH.Utilities.Converters;assembly=Mi5hmasH.WpfHelper">
    <Application.Resources>
        <ResourceDictionary>
            <!--  Converters  -->
            <c:Base64ToImageConverter x:Key="Base64ToImageConverter" />
        </ResourceDictionary>
    </Application.Resources>
</Application>
```

## WpfThemeAccent
### In "App.xaml" add the following:
```xml
<Application>
    <Application.Resources>
        <ResourceDictionary>
            <ResourceDictionary.MergedDictionaries>
                <!--  Load Fluent.xaml  -->
                <ResourceDictionary Source="pack://application:,,,/PresentationFramework.Fluent;component/Themes/Fluent.xaml" />
            </ResourceDictionary.MergedDictionaries>
        </ResourceDictionary>
    </Application.Resources>
</Application>
```
### Override the OnStartup method in "App.xaml.cs" like this:
```csharp
protected override void OnStartup(StartupEventArgs e)
{
    base.OnStartup(e);
    // Set the theme accent
    var colorAccent = new ColorAccentModel(
        Color.FromRgb(155, 106, 63),
        Color.FromRgb(181, 128, 74),
        Color.FromRgb(213, 185, 139),
        Color.FromRgb(237, 218, 170),
        Color.FromRgb(130, 83, 49),
        Color.FromRgb(99, 53, 32),
        Color.FromRgb(62, 19, 11));
    WpfThemeAccent.SetThemeAccent(colorAccent);
}
```

### Color Accents
```csharp
// Brown
new ColorAccentModel(
    Color.FromRgb(155, 106, 63),
    Color.FromRgb(181, 128, 74),
    Color.FromRgb(213, 185, 139),
    Color.FromRgb(237, 218, 170),
    Color.FromRgb(130, 83, 49),
    Color.FromRgb(99, 53, 32),
    Color.FromRgb(62, 19, 11));

// Orange
new ColorAccentModel(
    Color.FromRgb(255, 140, 0),
    Color.FromRgb(255, 153, 16),
    Color.FromRgb(255, 182, 52),
    Color.FromRgb(255, 209, 85),
    Color.FromRgb(225, 157, 0),
    Color.FromRgb(155, 93, 0),
    Color.FromRgb(92, 33, 0));

// Silver
new ColorAccentModel(
    Color.FromRgb(104, 118, 138),
    Color.FromRgb(124, 138, 156),
    Color.FromRgb(173, 187, 197),
    Color.FromRgb(217, 230, 234),
    Color.FromRgb(88, 101, 121),
    Color.FromRgb(52, 60, 81),
    Color.FromRgb(19, 23, 45));

// Gold
new ColorAccentModel(
    Color.FromRgb(132, 117, 69),
    Color.FromRgb(157, 141, 82),
    Color.FromRgb(194, 185, 134),
    Color.FromRgb(233, 229, 176),
    Color.FromRgb(116, 99, 59),
    Color.FromRgb(78, 59, 35),
    Color.FromRgb(43, 21, 12));

// Yellow
new ColorAccentModel(
    Color.FromRgb(255, 185, 0),
    Color.FromRgb(255, 194, 13),
    Color.FromRgb(255, 213, 42),
    Color.FromRgb(255, 232, 69),
    Color.FromRgb(225, 157, 0),
    Color.FromRgb(155, 93, 0),
    Color.FromRgb(92, 33, 0));

// Green
new ColorAccentModel(
    Color.FromRgb(16, 124, 16),
    Color.FromRgb(25, 161, 21),
    Color.FromRgb(69, 229, 50),
    Color.FromRgb(149, 239, 129),
    Color.FromRgb(14, 109, 14),
    Color.FromRgb(8, 75, 8),
    Color.FromRgb(3, 43, 3));

// Blue
new ColorAccentModel(
    Color.FromRgb(0, 120, 212),
    Color.FromRgb(0, 145, 248),
    Color.FromRgb(76, 194, 255),
    Color.FromRgb(153, 235, 255),
    Color.FromRgb(0, 103, 192),
    Color.FromRgb(0, 62, 146),
    Color.FromRgb(0, 26, 104));

// Blue
new ColorAccentModel(
    Color.FromRgb(0, 120, 212),
    Color.FromRgb(0, 145, 248),
    Color.FromRgb(76, 194, 255),
    Color.FromRgb(153, 235, 255),
    Color.FromRgb(0, 103, 192),
    Color.FromRgb(0, 62, 146),
    Color.FromRgb(0, 26, 104));

// Neon Blue
new ColorAccentModel(
    Color.FromRgb(0, 183, 195),
    Color.FromRgb(0, 213, 225),
    Color.FromRgb(41, 247, 255),
    Color.FromRgb(105, 252, 255),
    Color.FromRgb(0, 159, 170),
    Color.FromRgb(0, 103, 112),
    Color.FromRgb(0, 52, 59));

// Purple
new ColorAccentModel(
    Color.FromRgb(177, 70, 194),
    Color.FromRgb(189, 91, 203),
    Color.FromRgb(216, 141, 225),
    Color.FromRgb(241, 187, 244),
    Color.FromRgb(158, 58, 176),
    Color.FromRgb(111, 35, 130),
    Color.FromRgb(70, 13, 90));

// Red
new ColorAccentModel(
    Color.FromRgb(232, 17, 35),
    Color.FromRgb(239, 39, 51),
    Color.FromRgb(244, 103, 98),
    Color.FromRgb(251, 157, 139),
    Color.FromRgb(210, 14, 30),
    Color.FromRgb(158, 9, 18),
    Color.FromRgb(111, 3, 6));
```