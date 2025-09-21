using System.Reflection;
using static System.IO.Path;

namespace Mi5hmasH.AppInfo;

/// <summary>
/// Provides global access to application details such as the name, version, author, product title, description, copyright, and debug status.
/// </summary>
public class MyAppInfo(string appName = "MyApp")
{
    public static readonly string RootPath = AppDomain.CurrentDomain.BaseDirectory;
    
    #region APP INFO

    public string Name => appName;

    public static readonly string Version = GetAssemblyVersion();

    public static readonly string Author = GetCompany();

    public static readonly string ProductTitle = GetProductTitle();

    public static readonly string Description = GetDescription();

    public static readonly string Copyright = GetCopyright();

    #endregion
    
    #region METHODS

    /// <summary>
    /// Gets the attribute value of the assembly.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="resolveFunc"></param>
    /// <param name="defaultResult"></param>
    /// <returns></returns>
    private static string GetAttributeValue<T>(Func<T, string> resolveFunc, string defaultResult = "") where T : Attribute
    {
        var attributes = Assembly.GetEntryAssembly()?.GetCustomAttributes(typeof(T), false);
        return attributes is not { Length: > 0 } ? defaultResult : resolveFunc((T)attributes[0]);
    }

    /// <summary>
    /// Gets the application version.
    /// </summary>
    /// <returns></returns>
    private static string GetAssemblyVersion() => Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "1.0.0.0";

    /// <summary>
    /// Gets the product title.
    /// </summary>
    /// <returns></returns>
    private static string GetProductTitle() => GetAttributeValue<AssemblyTitleAttribute>(a => a.Title, GetFileNameWithoutExtension(AppDomain.CurrentDomain.FriendlyName));

    /// <summary>
    /// Gets the description about the application.
    /// </summary>
    /// <returns></returns>
    private static string GetDescription() => GetAttributeValue<AssemblyDescriptionAttribute>(a => a.Description);

    /// <summary>
    /// Gets the copyright information for the product.
    /// </summary>
    /// <returns></returns>
    private static string GetCopyright() => GetAttributeValue<AssemblyCopyrightAttribute>(a => a.Copyright);

    /// <summary>
    /// Gets the company information for the product.
    /// </summary>
    /// <returns></returns>
    private static string GetCompany() => GetAttributeValue<AssemblyCompanyAttribute>(a => a.Company);

    /// <summary>
    /// Checks if the application is running in debug mode.
    /// </summary>
    /// <returns></returns>
    public static bool IsDebug()
    {
        #if DEBUG
                return true;
        #else
                return false;
        #endif
    }

    #endregion
}