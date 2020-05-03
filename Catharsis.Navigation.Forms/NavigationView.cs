using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Catharsis.Navigation.Abstractions;
using ReactiveUI;
using Xamarin.Forms;

namespace Catharsis.Navigation.Forms
{
    /// <summary>
    /// The main navigation view for xamarin forms.
    /// </summary>
    public class NavigationView : NavigationPage, IView
    {
        private readonly IScheduler _backgroundScheduler;
        private readonly IScheduler _mainScheduler;
        private readonly IViewLocator _viewLocator;

        public IObservable<IViewModel> PagePopped { get; }

        public IScheduler MainThreadScheduler { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationView"/> class.
        /// </summary>
        public NavigationView() : this(RxApp.MainThreadScheduler, RxApp.TaskpoolScheduler, ViewLocator.Current)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationView"/> class.
        /// </summary>
        /// <param name="mainScheduler">The main scheduler to scheduler UI tasks on.</param>
        /// <param name="backgroundScheduler">The background scheduler.</param>
        /// <param name="viewLocator">The view locator which will find views associated with view models.</param>
        public NavigationView(IScheduler mainScheduler, IScheduler backgroundScheduler, IViewLocator viewLocator)
        {
            _backgroundScheduler = backgroundScheduler;
            _mainScheduler = mainScheduler;
            _viewLocator = viewLocator;

            PagePopped = Observable
                    .FromEventPattern<NavigationEventArgs>(x => Popped += x, x => Popped -= x)
                    .Select(ep => ep.EventArgs.Page.BindingContext as IViewModel)
                    .WhereNotNull();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationView"/> class.
        /// </summary>
        /// <param name="mainScheduler">The main scheduler to scheduler UI tasks on.</param>
        /// <param name="backgroundScheduler">The background scheduler.</param>
        /// <param name="viewLocator">The view locator which will find views associated with view models.</param>
        /// <param name="rootPage">The starting root page.</param>
        public NavigationView(IScheduler mainScheduler, IScheduler backgroundScheduler, IViewLocator viewLocator, Page rootPage) : base(rootPage)
        {
            _backgroundScheduler = backgroundScheduler;
            _mainScheduler = mainScheduler;
            _viewLocator = viewLocator;

            PagePopped = Observable
                    .FromEventPattern<NavigationEventArgs>(x => Popped += x, x => Popped -= x)
                    .Select(ep => ep.EventArgs.Page.BindingContext as IViewModel)
                    .WhereNotNull();
        }

        public IObservable<Unit> PushPage(IViewModel viewModel, string contract, bool resetStack, bool animate = true)
        {
            return Observable
                .Start(
                    () =>
                    {
                        var page = LocatePageFor(viewModel, contract);
                        SetPageTitle(page, viewModel.Id);
                        return page;
                    },
                    CurrentThreadScheduler.Instance)
                .ObserveOn(CurrentThreadScheduler.Instance)
                .SelectMany(
                    page =>
                    {
                        if (resetStack)
                        {
                            if (Navigation.NavigationStack.Count == 0)
                            {
                                return Navigation.PushAsync(page, animated: false).ToObservable();
                            }

                            // XF does not allow us to pop to a new root page. Instead, we need to inject the new root page and then pop to it.
                            Navigation.InsertPageBefore(page, Navigation.NavigationStack[0]);

                            return Navigation
                                .PopToRootAsync(animated: false)
                                .ToObservable();
                        }

                        return Navigation
                            .PushAsync(page, animate)
                            .ToObservable();
                    });
        }

        public IObservable<Unit> PopPage(bool animate = true)
        {
            return Navigation
                .PopAsync(animate)
                .ToObservable()
                .ToSignal()
                // XF completes the pop operation on a background thread
                .ObserveOn(_mainScheduler);
        }

        public IObservable<Unit> PopToRootPage(bool animate = true)
        {
            return Navigation
                .PopToRootAsync(animate)
                .ToObservable()
                .ToSignal()
                // XF completes the pop operation on a background thread
                .ObserveOn(_mainScheduler);
        }

        public IObservable<Unit> PushModal(IViewModel modalViewModel, string contract, bool withNavigationPage = true)
        {
            return Observable
                .Start(
                    () =>
                    {
                        var page = LocatePageFor(modalViewModel, contract);
                        SetPageTitle(page, modalViewModel.Id);
                        if (withNavigationPage)
                        {
                            return new NavigationPage(page);
                        }

                        return page;
                    },
                    CurrentThreadScheduler.Instance)
                .ObserveOn(CurrentThreadScheduler.Instance)
                .SelectMany(
                    page =>
                        Navigation
                            .PushModalAsync(page)
                            .ToObservable());
        }

        public IObservable<Unit> PopModal()
        {
            return Navigation
                .PopModalAsync()
                .ToObservable()
                .ToSignal()
                // XF completes the pop operation on a background thread
                .ObserveOn(_mainScheduler);
        }

        private Page LocatePageFor(object viewModel, string contract)
        {
            var view = _viewLocator.ResolveView(viewModel, contract);
            var page = view as Page;

            if (view == null)
            {
                throw new InvalidOperationException($"No view could be located for type '{viewModel.GetType().FullName}', contract '{contract}'. Be sure Splat has an appropriate registration.");
            }

            if (view == null)
            {
                throw new InvalidOperationException($"Could not find view for type '{viewModel.GetType().FullName}', contract '{contract}' does not implement IViewFor.");
            }

            if (page == null)
            {
                throw new InvalidOperationException($"Resolved view '{view.GetType().FullName}' for type '{viewModel.GetType().FullName}', contract '{contract}' is not a Page.");
            }

            view.ViewModel = viewModel;

            return page;
        }

        private void SetPageTitle(Page page, string resourceKey)
        {
            // var title = Localize.GetString(resourceKey);
            // TODO: ensure resourceKey isn't null and is localized.
            page.Title = resourceKey;
        }
    }
}