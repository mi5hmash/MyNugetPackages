using System.Reflection;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mi5hmasH.WpfHelper.Attributes;
using Mi5hmasH.WpfHelper.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Mi5hmasH.WpfHelper.Services;

/// <summary>
/// Defines a service for navigating between pages in the application.
/// </summary>
public interface INavigationService
{
    ViewModel? CurrentViewModel { get; set; }
    PageModel? CurrentPageModel { get; set; }
    Window? ParentWindow { get; set; }
    IEnumerable<PageModel> PickablePages { get; set; }
    void NavigateTo(Type vmType);
    void NavigateTo<TViewModel>() where TViewModel : ViewModel;
    void Initialize(Window parentWindow);
}

/// <summary>
/// Navigation service for navigating between pages in the application.
/// </summary>
public partial class NavigationService : ObservableObject, INavigationService
{
    private bool _isInitialized;
    private readonly IServiceProvider _provider;
    private readonly IEnumerable<PageModel> _pages;
    private readonly Dictionary<Type, PageModel> _pagesMap = [];
    private readonly Dictionary<Type, List<ShortcutCommandModel>> _viewModelsCommands;

    /// <summary>
    /// Gets or sets the current view model being displayed in the application.
    /// </summary>
    [ObservableProperty] 
    public partial ViewModel? CurrentViewModel { get; set; }

    /// <summary>
    /// Gets or sets the current page model being displayed in the application.
    /// </summary>
    [ObservableProperty] 
    public partial PageModel? CurrentPageModel { get; set; }

    /// <summary>
    /// Gets or sets the collection of pages that can be navigated to, excluding the current page.
    /// </summary>
    [ObservableProperty]
    public partial IEnumerable<PageModel> PickablePages { get; set; } = null!;

    /// <summary>
    /// Gets or sets the parent window of the navigation service. This is used to register input bindings for navigation commands.
    /// </summary>
    public Window? ParentWindow { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationService"/> class with the specified service provider and pages.
    /// </summary>
    /// <param name="provider">The service provider used to resolve view models.</param>
    /// <param name="pages">The collection of pages available for navigation.</param>
    public NavigationService(IServiceProvider provider, IEnumerable<PageModel> pages)
    {
        _provider = provider;

        _pages = pages;
        foreach (var page in _pages)
        {
            page.NavigateToCommand = new RelayCommand(() => NavigateTo(page.ViewModelType));
            _pagesMap.Add(page.ViewModelType, page);
        }

        _viewModelsCommands = BuildInputBindingsDictionary();
    }

    /// <summary>
    /// Initializes the navigation service with the specified parent window. 
    /// This method should be called after the parent window is created and before any navigation occurs.
    /// </summary>
    /// <param name="parentWindow">The parent window of the navigation service.</param>
    public void Initialize(Window parentWindow)
    {
        if (_isInitialized) return;
        ParentWindow = parentWindow;
        AddPageTemplatesToDictionary();
        RegisterNavigationInputBindings();
        NavigateTo(_pagesMap.FirstOrDefault().Key);
        _isInitialized = true;
    }
    
    /// <summary>
    /// Navigates to the specified view model type.
    /// </summary>
    /// <param name="vmType">The type of the view model to navigate to.</param>
    public void NavigateTo(Type vmType)
    {
        UnregisterInputBindings();
        CurrentViewModel = (ViewModel)_provider.GetRequiredService(vmType);
        CurrentPageModel = _pagesMap[vmType];
        PickablePages = _pages.Where(p => p.ViewModelType != vmType);
        RegisterInputBindings();
    }
    
    /// <summary>
    /// Navigates to the specified view model type.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model to navigate to.</typeparam>
    public void NavigateTo<TViewModel>() where TViewModel : ViewModel 
        => NavigateTo(typeof(TViewModel));

    /// <summary>
    /// Error message for when the NavigationService is constructed without a valid parent window.
    /// </summary>
    private const string ErrorParentWindowIsNull = "NavigationService was constructed without a valid parent window.";

    /// <summary>
    /// Registers input bindings for navigation commands based on the hotkeys defined in the registered pages.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the parent window is not set.</exception>
    private void RegisterNavigationInputBindings()
    {
        if (ParentWindow is null)
            throw new InvalidOperationException(ErrorParentWindowIsNull);
        
        foreach (var page in _pages)
        {
            if (page.Hotkey is not null && page.NavigateToCommand is not null)
            {
                ParentWindow.InputBindings.Add(new KeyBinding(
                    page.NavigateToCommand,
                    page.Hotkey.Key,
                    page.Hotkey.Modifiers));
            }
        }
    }

    /// <summary>
    /// Adds DataTemplates for all registered pages to the parent window's resource dictionary.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the parent window is not set.</exception>
    private void AddPageTemplatesToDictionary()
    {
        if (ParentWindow is null)
            throw new InvalidOperationException(ErrorParentWindowIsNull);
        
        ParentWindow.Resources.MergedDictionaries.Add(GetPageTemplates());
    }  

    /// <summary>
    /// Gets the page templates for all registered pages and returns them as a ResourceDictionary.
    /// </summary>
    /// <remarks>
    /// Example of a DataTemplate returned by this method:
    /// <![CDATA[
    /// <DataTemplate DataType="{x:Type vm:HomePageViewModel}">
    ///     <v:HomePageView />
    /// </DataTemplate>
    /// ]]>
    /// </remarks>
    /// <returns>A ResourceDictionary containing DataTemplates for each page.</returns>
    private ResourceDictionary GetPageTemplates()
    {        
        var dict = new ResourceDictionary();
        
        foreach (var page in _pages)
        {
            var factory = new FrameworkElementFactory(page.ViewType);

            var template = new DataTemplate
            {
                DataType = page.ViewModelType,
                VisualTree = factory
            };

            dict.Add(new DataTemplateKey(page.ViewModelType), template);
        }

        return dict;
    }

    /// <summary>
    /// Builds a dictionary that maps view model types to their associated shortcut commands and gestures.
    /// </summary>
    /// <returns>A dictionary mapping view model types to lists of shortcut command models.</returns>
    /// <exception cref="NotSupportedException">Thrown if a gesture is not supported.</exception>
    private Dictionary<Type, List<ShortcutCommandModel>> BuildInputBindingsDictionary()
    {
        var dictionary = new Dictionary<Type, List<ShortcutCommandModel>>();
        var vmTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => typeof(ViewModel).IsAssignableFrom(t) && !t.IsAbstract);

        var methodGroups = vmTypes
            .SelectMany(t => t.GetMethods(BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance))
            .Where(m => m.GetCustomAttribute<ShortcutAttribute>() != null)
            .GroupBy(m => m.DeclaringType);
        
        var keyGestureConverter = new KeyGestureConverter();
        
        foreach (var group in methodGroups)
        {
            var listOfCommands = new List<ShortcutCommandModel>();
            foreach (var method in group)
            {
                var attr = method.GetCustomAttribute<ShortcutAttribute>();
                var gesture = (KeyGesture?)keyGestureConverter.ConvertFromString(attr!.Gesture);
                if (gesture is null) 
                    throw new NotSupportedException($"Gesture {attr.Gesture} is not supported.");
            
                var declaringType = method.DeclaringType;
                var commandProperty = declaringType?.GetProperty(
                    $"{method.Name}Command",
                    BindingFlags.Instance | BindingFlags.Public);
                var vm = _provider.GetRequiredService(declaringType!);
                var command = (ICommand?)commandProperty?.GetValue(vm);
                listOfCommands.Add(new ShortcutCommandModel
                {
                    Command = command!,
                    Gesture = gesture,
                }); 
            }
            dictionary.Add(group.Key!, listOfCommands);
        }

        return dictionary;
    }
    
    /// <summary>
    /// Registers input bindings for the current view model's shortcut commands.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the parent window is not set.</exception>
    private void RegisterInputBindings()
    {
        if (CurrentViewModel is null) return;
        _viewModelsCommands.TryGetValue(CurrentViewModel.GetType(), out var shortcutCommandModels);
        if (shortcutCommandModels is null) return;
        if (ParentWindow is null)
            throw new InvalidOperationException(ErrorParentWindowIsNull);
        
        foreach (var shortcutCommandModel in shortcutCommandModels)
        {
            ParentWindow.InputBindings.Add(new KeyBinding(
                shortcutCommandModel.Command,
                shortcutCommandModel.Gesture.Key,
                shortcutCommandModel.Gesture.Modifiers));
        }
    }
    
    /// <summary>
    /// Unregisters input bindings for the current view model's shortcut commands.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the parent window is not set.</exception>
    private void UnregisterInputBindings()
    {
        if (CurrentViewModel is null) return;
        _viewModelsCommands.TryGetValue(CurrentViewModel.GetType(), out var shortcutCommandModels);
        if (shortcutCommandModels is null) return;
        if (ParentWindow is null)
            throw new InvalidOperationException(ErrorParentWindowIsNull);
        
        foreach (var keyBinding in shortcutCommandModels
                     .Select(shortcutCommandModel => ParentWindow.InputBindings
                     .OfType<KeyBinding>()
                     .FirstOrDefault(kb =>
                         kb.Command == shortcutCommandModel.Command &&
                         kb.Key == shortcutCommandModel.Gesture.Key &&
                         kb.Modifiers == shortcutCommandModel.Gesture.Modifiers)))
        {
            ParentWindow.InputBindings.Remove(keyBinding);
        }
    } 
}