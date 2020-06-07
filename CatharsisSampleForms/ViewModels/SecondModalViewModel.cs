using System;
using System.Reactive;
using Catharsis.Navigation.Abstractions;
using CatharsisSampleForms.Helpers;
using ReactiveUI;

namespace CatharsisSampleForms.ViewModels
{
    public class SecondModalViewModel : BaseViewModel
    {
        //public ReactiveCommand<Unit, Unit> PushPage { get; set; }

        public ReactiveCommand<Unit, Unit> PopModal { get; set; }

        public override string Id => nameof(SecondModalViewModel);

        public SecondModalViewModel(INavigationService viewStackService) : base(viewStackService)
        {
            //PushPage = ReactiveCommand
            //    .CreateFromObservable(() =>
            //        NavigationService.PushPage(new RedPageViewModel(NavigationService)),
            //        outputScheduler: RxApp.MainThreadScheduler);

            PopModal = ReactiveCommand
                .CreateFromObservable(() =>
                    NavigationService.PopModal(),
                    outputScheduler: RxApp.MainThreadScheduler);

            //PushPage.Subscribe(x => System.Diagnostics.Debug.WriteLine("PagePushed"));
            PopModal.Subscribe(x => System.Diagnostics.Debug.WriteLine("PagePoped"));

            //PushPage.ThrownExceptions.Subscribe(error => Interactions.ErrorMessage.Handle(error).Subscribe());
            PopModal.ThrownExceptions.Subscribe(error => Interactions.ErrorMessage.Handle(error).Subscribe());
        }
    }
}