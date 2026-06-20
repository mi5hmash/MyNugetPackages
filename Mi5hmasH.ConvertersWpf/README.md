# Disclaimer
This NuGet package was created exclusively for use in my own open‑source projects published on my GitHub profile.
While it is publicly available, it is not intended to serve as a general‑purpose library, nor is it designed or documented for external production use.
Feel free to explore the code, but please keep in mind that its primary purpose is to support my personal project ecosystem.

# Usage
## Converters
### In "App.xaml" add the following:
```xml
<Application xmlns:c="clr-namespace:Mi5hmasH.ConvertersWpf;assembly=Mi5hmasH.ConvertersWpf">
    <Application.Resources>
        <ResourceDictionary>
            <!--  Converters  -->
            <c:Base64ToImageConverter x:Key="Base64ToImageConverter" />
            <c:FileNameWithoutExtensionConverter x:Key="FileNameWithoutExtensionConverter" />
            <c:InverseBooleanConverter x:Key="InverseBooleanConverter" />
        </ResourceDictionary>
    </Application.Resources>
</Application>
```