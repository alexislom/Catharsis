using System;
using System.Reactive.Concurrency;
using Catharsis.Navigation.Abstractions;
using Catharsis.Navigation.Services;
using ReactiveUI;
using Splat;

namespace Catharsis.Navigation.Forms.Mixins
{
    /// <summary>
    /// Extension methods associated with the IMutableDependencyResolver interface.
    /// </summary>
    public static class DependencyResolverMixins
    {
        /// <summary>
        /// Gets the navigation view key.
        /// </summary>
        public static string NavigationView => nameof(NavigationView);

        /// <summary>
        /// Initializes the catharsis.
        /// </summary>
        /// <param name="dependencyResolver">The dependency resolver.</param>
        /// <returns>The dependencyResolver.</returns>
        public static IMutableDependencyResolver RegisterNavigationView(this IMutableDependencyResolver dependencyResolver)
        {
            var vLocator = Locator.Current.GetService<IViewLocator>();

            dependencyResolver.RegisterLazySingleton(() => new NavigationView(RxApp.MainThreadScheduler, RxApp.TaskpoolScheduler, vLocator), typeof(IView), NavigationView);
            return dependencyResolver;
        }

        /// <summary>
        /// Initializes catharsis.
        /// </summary>
        /// <param name="dependencyResolver">The dependency resolver.</param>
        /// <param name="mainThreadScheduler">The main scheduler.</param>
        /// <param name="backgroundScheduler">The background scheduler.</param>
        /// <returns>The dependencyResolver.</returns>
        public static IMutableDependencyResolver RegisterNavigationView(this IMutableDependencyResolver dependencyResolver, IScheduler mainThreadScheduler, IScheduler backgroundScheduler)
        {
            var vLocator = Locator.Current.GetService<IViewLocator>();

            dependencyResolver.RegisterLazySingleton(() => new NavigationView(mainThreadScheduler, backgroundScheduler, vLocator), typeof(IView), NavigationView);
            return dependencyResolver;
        }

        /// <summary>
        /// Registers a value for navigation.
        /// </summary>
        /// <typeparam name="TView">The type of view to register.</typeparam>
        /// <param name="dependencyResolver">The dependency resolver.</param>
        /// <param name="navigationViewFactory">The navigation view factory.</param>
        /// <returns>The dependencyResolver.</returns>
        public static IMutableDependencyResolver RegisterNavigationView<TView>(this IMutableDependencyResolver dependencyResolver, Func<TView> navigationViewFactory)
            where TView : IView
        {
            if (dependencyResolver is null)
            {
                throw new ArgumentNullException(nameof(dependencyResolver));
            }

            if (navigationViewFactory is null)
            {
                throw new ArgumentNullException(nameof(navigationViewFactory));
            }

            var navigationView = navigationViewFactory();
            var viewStackService = new NavigationService(navigationView);

            dependencyResolver.RegisterLazySingleton<INavigationService>(() => viewStackService);
            dependencyResolver.RegisterLazySingleton<IView>(() => navigationView, NavigationView);
            return dependencyResolver;
        }

        /// <summary>
        /// Gets the navigation view.
        /// </summary>
        /// <param name="dependencyResolver">The dependency resolver.</param>
        /// <param name="contract">The contract.</param>
        /// <returns>The navigation view.</returns>
        public static NavigationView GetNavigationView(this IReadonlyDependencyResolver dependencyResolver, string contract = null)
        {
            if (dependencyResolver is null)
            {
                throw new ArgumentNullException(nameof(dependencyResolver));
            }

            return dependencyResolver.GetService<IView>(contract ?? NavigationView) as NavigationView;
        }
    }
}