using System;
using Catharsis.Navigation.Abstractions;
using Catharsis.Navigation.Forms.Mixins;
using Catharsis.Navigation.Mixins;
using CatharsisSampleForms.Helpers;
using CatharsisSampleForms.ViewModels;
using CatharsisSampleForms.Views;
using ReactiveUI;
using Splat;
using Xamarin.Forms;
using static Catharsis.Navigation.Catharsis;

namespace CatharsisSampleForms
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            RxApp.DefaultExceptionHandler = new DefaultExceptionHandler();

            Instance.InitializeForms();

            //Register all views and view models
            Locator
                .CurrentMutable
                .RegisterView<MainPage, MainViewModel>()
                .RegisterView<FirstModalView, FirstModalViewModel>();
            //.RegisterView<SecondModalView, SecondModalViewModel>()
            //.RegisterView<RedView, RedViewModel>()
            //.RegisterView<GreenView, GreenViewModel>()
            //.RegisterNavigationView(() => new BlueNavigationView())
            //.RegisterViewModel(() => new GreenViewModel(Locator.Current.GetService<IViewStackService>()));

            //Push root page to navigation stack
            Locator
                .Current
                .GetService<INavigationService>()
                .PushPage(new MainViewModel(), contract: null, resetStack: true, animate: false)
                .Subscribe();

            //Get entry point
            MainPage = Locator.Current.GetNavigationView();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}