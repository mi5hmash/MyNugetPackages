using System.Windows;
using System.Windows.Controls;
using Mi5hmasH.Utilities.Enums;
using Mi5hmasH.WpfHelper.Interfaces;
using Mi5hmasH.WpfHelper.Models;
using Mi5hmasH.WpfHelper.Services;
using Microsoft.Extensions.DependencyInjection;
using static Mi5hmasH.Utilities.Enums.LifetimeEnum;
using NavigationService = Mi5hmasH.WpfHelper.Services.NavigationService;

namespace Mi5hmasH.WpfHelper.Helpers;

/// <summary>
/// Provides extension methods for registering views, view models, and services with the dependency injection container.
/// </summary>
public static class ObjectRegistrationHelper
{
    /// <summary>
    /// Extension methods for registering views, view models, and services with the dependency injection container.
    /// </summary>
    /// <param name="services">The IServiceCollection to add the services to.</param>
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Registers a view and its associated view model with the specified lifetime.
        /// </summary>
        /// <typeparam name="TView">The type of the view to register.</typeparam>
        /// <typeparam name="TViewModel">The type of the view model to register.</typeparam>
        /// <param name="lifetime">The lifetime of the registered services.</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private void RegisterObject<TView, TViewModel>(LifetimeEnum lifetime = Singleton) 
            where TView : class 
            where TViewModel : ViewModel
        {
            switch (lifetime)
            {
                case Singleton:
                    services.AddSingleton<TView>();
                    services.AddSingleton<TViewModel>();
                    break;
                case Scoped:
                    services.AddScoped<TView>();
                    services.AddScoped<TViewModel>();
                    break;
                case Transient:
                    services.AddTransient<TView>();
                    services.AddTransient<TViewModel>();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null);
            }
        }

        /// <summary>
        /// Registers a user control and its associated view model with the specified lifetime.
        /// </summary>
        /// <typeparam name="TView">The type of the user control to register.</typeparam>
        /// <typeparam name="TViewModel">The type of the view model to register.</typeparam>
        /// <param name="lifetime">The lifetime of the registered services.</param>
        public void RegisterUserControl<TView, TViewModel>(LifetimeEnum lifetime = Singleton) 
            where TView : UserControl 
            where TViewModel : ViewModel 
            => services.RegisterObject<TView, TViewModel>(lifetime);

        /// <summary>
        /// Registers a window and its associated view model with the specified lifetime.
        /// </summary>
        /// <typeparam name="TView">The type of the window to register.</typeparam>
        /// <typeparam name="TViewModel">The type of the view model to register.</typeparam>
        /// <param name="lifetime">The lifetime of the registered services.</param>
        public void RegisterWindow<TView, TViewModel>(LifetimeEnum lifetime = Singleton) 
            where TView : Window 
            where TViewModel : ViewModel 
            => services.RegisterObject<TView, TViewModel>(lifetime);

        /// <summary>
        /// Registers the navigation service and its associated pages with the specified page registry.
        /// </summary>
        /// <param name="pageRegistry">The page registry containing the pages to register.</param>
        public void RegisterNavigationService(IPageRegistry pageRegistry)
        {
            services.AddSingleton<INavigationService>(sp => new NavigationService(sp, pageRegistry.Pages));
            foreach (var page in pageRegistry.Pages)
                services.AddSingleton(page.ViewModelType);
        }

        /// <summary>
        /// Registers the localization service with the specified culture.
        /// </summary>
        /// <param name="culture">The culture to use for localization.</param>
        public void RegisterLocalizationService(string culture) 
            => services.AddSingleton<ILocalizationService>(new LocalizationService(culture));
    }
}