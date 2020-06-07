using System;
using System.Reactive;
using System.Reactive.Linq;
using Catharsis.Navigation.Abstractions;
using CatharsisSampleForms.Helpers;
using ReactiveUI;
using Splat;

namespace CatharsisSampleForms.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public override string Id => nameof(MainViewModel);

        public ReactiveCommand<Unit, Unit> OpenFirstModalPage { get; set; }

        public MainViewModel() : base(Locator.Current.GetService<INavigationService>())
        {
            OpenFirstModalPage = ReactiveCommand
                .CreateFromObservable(() =>
                    NavigationService.PushModal(new FirstModalViewModel(NavigationService)),
                    outputScheduler: RxApp.MainThreadScheduler);

            OpenFirstModalPage.ThrownExceptions.Subscribe(error => Interactions.ErrorMessage.Handle(error).Subscribe());
        }
    }
}