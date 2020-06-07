using System;
using System.Reactive;
using Catharsis.Navigation.Abstractions;
using CatharsisSampleForms.Helpers;
using ReactiveUI;

namespace CatharsisSampleForms.ViewModels
{
    public class RedPageViewModel : BaseViewModel, IViewModel
    {
        //public ReactiveCommand<Unit, Unit> PopModal { get; set; }

        public ReactiveCommand<Unit, Unit> PushPage { get; set; }

        public ReactiveCommand<Unit, Unit> PopPage { get; set; }

        public ReactiveCommand<Unit, Unit> PopToRoot { get; set; }

        public override string Id => nameof(RedPageViewModel);

        public RedPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            //PopModal = ReactiveCommand
            //    .CreateFromObservable(() =>
            //        NavigationService.PopModal(),
            //        outputScheduler: RxApp.MainThreadScheduler);

            PopPage = ReactiveCommand
                .CreateFromObservable(() =>
                    NavigationService.PopPage(),
                    outputScheduler: RxApp.MainThreadScheduler);

            PushPage = ReactiveCommand
                .CreateFromObservable(() =>
                    NavigationService.PushPage(new RedPageViewModel(NavigationService)),
                    outputScheduler: RxApp.MainThreadScheduler);

            PopToRoot = ReactiveCommand
                .CreateFromObservable(() =>
                    NavigationService.PopToRootPage(),
                    outputScheduler: RxApp.MainThreadScheduler);

            //PopModal.Subscribe(x => System.Diagnostics.Debug.WriteLine("PagePushed"));
            //PopModal.ThrownExceptions.Subscribe(error => Interactions.ErrorMessage.Handle(error).Subscribe());
            PopPage.ThrownExceptions.Subscribe(error => Interactions.ErrorMessage.Handle(error).Subscribe());
            PushPage.ThrownExceptions.Subscribe(error => Interactions.ErrorMessage.Handle(error).Subscribe());
            PopToRoot.ThrownExceptions.Subscribe(error => Interactions.ErrorMessage.Handle(error).Subscribe());
        }
    }
}