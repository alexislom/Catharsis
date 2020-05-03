using Catharsis.Navigation.Abstractions;
using ReactiveUI;

namespace CatharsisSampleForms.ViewModels
{
    public abstract class BaseViewModel : ReactiveObject, IViewModel
    {
        protected readonly INavigationService NavigationService;

        protected BaseViewModel(INavigationService navigationService)
        {
            NavigationService = navigationService;
        }

        public virtual string Id { get; }
    }
}