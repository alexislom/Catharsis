using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Catharsis.Navigation.Abstractions;
using Catharsis.Navigation.Extensions;

namespace Catharsis.Navigation.Services
{
    /// <summary>
    /// Service implementation to handle navigation stack updates.
    /// </summary>
    /// <seealso cref="INavigationService" />
    public class NavigationService : INavigationService, IDisposable
    {
        private BehaviorSubject<IImmutableList<IViewModel>> _modalSubject;
        private BehaviorSubject<IImmutableList<IViewModel>> _navigationSubject;
        // To detect redundant calls
        private bool disposedValue = false;

        /// <summary>
        /// Gets the current view on the stack.
        /// </summary>
        public IView View { get; private set; }

        /// <summary>
        /// Gets the page navigation stack.
        /// </summary>
        public IObservable<IImmutableList<IViewModel>> NavigationStack => _navigationSubject.AsObservable();

        /// <summary>
        /// Gets the modal navigation stack.
        /// </summary>
        public IObservable<IImmutableList<IViewModel>> ModalStack => _modalSubject.AsObservable();

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationService"/> class.
        /// </summary>
        /// <param name="view">The view.</param>
        public NavigationService(IView view)
        {
            View = view ?? throw new ArgumentNullException(nameof(view));
            _modalSubject = new BehaviorSubject<IImmutableList<IViewModel>>(ImmutableList<IViewModel>.Empty);
            _navigationSubject = new BehaviorSubject<IImmutableList<IViewModel>>(ImmutableList<IViewModel>.Empty);

            View
                .PagePopped
                .Do(poppedPage =>
                {
                    var currentPageStack = _navigationSubject.Value;
                    if (currentPageStack.Count > 0 && poppedPage == currentPageStack[currentPageStack.Count - 1])
                    {
                        var removedPage = PopStackAndTick(_navigationSubject);
                        System.Diagnostics.Debug.WriteLine($"Removed page '{removedPage.Id}' from stack.");
                    }
                })
                .SubscribeSafe();
        }

        /// <summary>
        /// Pushes the <see cref="IViewModel" /> onto the stack.
        /// </summary>
        /// <param name="viewModel">The page.</param>
        /// <param name="contract">The contract.</param>
        /// <param name="resetStack">if set to <c>true</c> [reset stack].</param>
        /// <param name="animate">if set to <c>true</c> [animate].</param>
        /// <returns></returns>
        public IObservable<Unit> PushPage(IViewModel viewModel, string contract = null, bool resetStack = false, bool animate = true)
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            return View
                .PushPage(viewModel, contract, resetStack, animate)
                .Do(_ =>
                {
                    AddToStackAndTick(_navigationSubject, viewModel, resetStack);
                    System.Diagnostics.Debug.WriteLine($"Added page '{viewModel.Id}' (contract '{contract}') to stack.");
                });
        }

        public IObservable<Unit> PushPage<TViewModel>(string contract = null, bool resetStack = false, bool animate = true) where TViewModel : IViewModel
        {
            //var viewModel = ViewModelFactory.Current.Create<TViewModel>();
            //return PushPage(viewModel, contract, resetStack, animate);
            throw new NotImplementedException();
        }

        /// <summary>
        /// Pops the <see cref="IViewModel" /> off the stack.
        /// </summary>
        /// <param name="animate">if set to <c>true</c> [animate].</param>
        /// <returns></returns>
        public IObservable<Unit> PopPage(bool animate = true)
        {
            var top = TopPage().Wait();
            return View.PopPage(animate).Do(_ =>
            {
                top.InvokeViewModelAction<IDestructible>(x => x.Destroy());
            });
        }

        public IObservable<Unit> PopToRootPage(bool animate = true)
        {
            return View.PopToRootPage(animate).Do(_ => PopRootAndTick(_navigationSubject));
        }

        public IObservable<Unit> PushModal(IViewModel modal, string contract = null, bool withNavigationPage = true)
        {
            if (modal == null)
            {
                throw new ArgumentNullException(nameof(modal));
            }

            return View
                .PushModal(modal, contract, withNavigationPage)
                .Do(_ =>
                {
                    AddToStackAndTick(_modalSubject, modal, false);
                    System.Diagnostics.Debug.WriteLine($"Added modal '{modal.Id}' (contract '{contract}') to stack.");
                });
        }

        public IObservable<Unit> PushModal<TViewModel>(string contract = null, bool withNavigationPage = true) where TViewModel : IViewModel
        {
            //var viewmodel = ViewModelFactory.Current.Create<TViewModel>(contract);
            //return PushModal(viewmodel, contract, withNavigationPage);
            throw new NotImplementedException();
        }

        /// <summary>
        /// Pops the <see cref="IViewModel" /> off the stack.
        /// </summary>
        /// <param name="animate">if set to <c>true</c> [animate].</param>
        /// <returns>An observable that signals when the pop is complete.</returns>
        public IObservable<Unit> PopModal(bool animate = true)
        {
            return View.PopModal()
                       .Do(_ =>
                       {
                           var modal = PopStackAndTick(_modalSubject);
                           modal.InvokeViewModelAction<IDestructible>(x => x.Destroy());
                       });
        }

        /// <summary>
        /// Returns the top modal from the current modal stack.
        /// </summary>
        /// <returns>An observable that signals the top modal view model.</returns>
        public IObservable<IViewModel> TopModal() => _modalSubject.FirstAsync().Select(x => x[x.Count - 1]);

        /// <summary>
        /// Returns the top page from the current navigation stack.
        /// </summary>
        /// <returns>An observable that signals the top page view model.</returns>
        public IObservable<IViewModel> TopPage() => _navigationSubject.FirstAsync().Select(x => x[x.Count - 1]);

        /// <summary>
        /// Adds to stack and tick.
        /// </summary>
        /// <typeparam name="T">The view model type.</typeparam>
        /// <param name="stackSubject">The stack subject.</param>
        /// <param name="item">The item.</param>
        /// <param name="reset">if set to <c>true</c> [reset].</param>
        protected static void AddToStackAndTick<T>(BehaviorSubject<IImmutableList<T>> stackSubject, T item, bool reset)
        {
            if (stackSubject is null)
            {
                throw new ArgumentNullException(nameof(stackSubject));
            }

            var stack = stackSubject.Value;

            if (reset)
            {
                stack = new[] { item }.ToImmutableList();
            }
            else
            {
                stack = stack.Add(item);
            }

            stackSubject.OnNext(stack);
        }

        /// <summary>
        /// Pops the stack and notifies observers.
        /// </summary>
        /// <typeparam name="T">The view model type.</typeparam>
        /// <param name="stackSubject">The stack subject.</param>
        /// <returns>The view model popped.</returns>
        /// <exception cref="InvalidOperationException">Stack is empty.</exception>
        protected static T PopStackAndTick<T>(BehaviorSubject<IImmutableList<T>> stackSubject)
        {
            if (stackSubject is null)
            {
                throw new ArgumentNullException(nameof(stackSubject));
            }

            var stack = stackSubject.Value;

            if (stack.Count == 0)
            {
                throw new InvalidOperationException("Stack is empty.");
            }

            var removedItem = stack[stack.Count - 1];
            stack = stack.RemoveAt(stack.Count - 1);
            stackSubject.OnNext(stack);
            return removedItem;
        }

        /// <summary>
        /// Pops the root and notifies observers.
        /// </summary>
        /// <typeparam name="T">The view model type.</typeparam>
        /// <param name="stackSubject">The stack subject.</param>
        /// <exception cref="InvalidOperationException">Stack is empty.</exception>
        protected static void PopRootAndTick<T>(BehaviorSubject<IImmutableList<T>> stackSubject)
        {
            IImmutableList<T> poppedStack = ImmutableList<T>.Empty;
            if (stackSubject?.Value == null || !stackSubject.Value.Any())
            {
                throw new InvalidOperationException("Stack is empty.");
            }

            stackSubject
               .Take(1)
               .Where(stack => stack != null)
               .Subscribe(stack =>
               {
                   if (stack.Count > 1)
                   {
                       poppedStack = stack.RemoveRange(stack.IndexOf(stack[1]), stack.Count - 1);

                       foreach (T popped in stack.RemoveRange(poppedStack).Reverse())
                       {
                           if (popped == null)
                           {
                               continue;
                           }

                           popped.InvokeViewModelAction<IDestructible>(x => x.Destroy());
                       }
                   }
                   else
                   {
                       poppedStack = stack;
                   }
               });

            stackSubject.OnNext(poppedStack);
        }

        #region IDisposable Support

        public void Dispose()
        {
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.            // GC.SuppressFinalize(this);
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.        // ~NavigationService()        // {        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.        //   Dispose(false);        // }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)            {                if (disposing)                {
                    _modalSubject?.Dispose();
                    _navigationSubject?.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer.
                // TODO: set large fields to null.

                disposedValue = true;            }
        }

        #endregion IDisposable Support
    }
}