using System;
using System.Reactive;
using System.Reactive.Linq;
using Catharsis.Navigation.Abstractions;
using CatharsisSampleForms.Helpers;
using ReactiveUI;

namespace CatharsisSampleForms.ViewModels
{
    public class FirstModalViewModel : BaseViewModel, IDestructible
    {
        public ReactiveCommand<Unit, Unit> OpenModal { get; set; }

        public ReactiveCommand<Unit, Unit> PopModal { get; set; }

        public override string Id => nameof(FirstModalViewModel);

        public FirstModalViewModel(INavigationService navigationService) : base(navigationService)
        {
            OpenModal = ReactiveCommand
                        .CreateFromObservable(() =>
                            NavigationService.PushModal(new SecondModalViewModel(NavigationService)),
                            outputScheduler: RxApp.MainThreadScheduler);

            PopModal = ReactiveCommand
                        .CreateFromObservable(() =>
                            NavigationService.PopModal(),
                            outputScheduler: RxApp.MainThreadScheduler);

            OpenModal.Subscribe(x => System.Diagnostics.Debug.WriteLine("PagePushed"));
            PopModal.Subscribe(x => System.Diagnostics.Debug.WriteLine("PagePopped"));
            PopModal.ThrownExceptions.Subscribe(error => Interactions.ErrorMessage.Handle(error).Subscribe());
        }


        public void Destroy()
        {
            System.Diagnostics.Debug.WriteLine($"Destroy: {nameof(FirstModalViewModel)}");
        }
    }
}