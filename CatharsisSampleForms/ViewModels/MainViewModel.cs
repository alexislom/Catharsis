using Catharsis.Navigation.Abstractions;
using Splat;

namespace CatharsisSampleForms.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public override string Id => nameof(MainViewModel);

        public MainViewModel() : base(Locator.Current.GetService<INavigationService>())
        {
        }
    }
}