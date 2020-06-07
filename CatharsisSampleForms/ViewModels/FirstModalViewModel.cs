using System.Reactive;
using Catharsis.Navigation.Abstractions;
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
            //OpenModal = ReactiveCommand
            //            .CreateFromObservable(() =>
            //                ViewStackService.PushModal(new SecondModalViewModel(viewStackService)),
            //                outputScheduler: RxApp.MainThreadScheduler);

            //PopModal = ReactiveCommand
            //            .CreateFromObservable(() =>
            //                ViewStackService.PopModal(),
            //                outputScheduler: RxApp.MainThreadScheduler);

            //OpenModal.Subscribe(x => Debug.WriteLine("PagePushed"));
            //PopModal.Subscribe(x => Debug.WriteLine("PagePopped"));
            //PopModal.ThrownExceptions.Subscribe(error => Interactions.ErrorMessage.Handle(error).Subscribe());
        }


        public void Destroy()
        {
            System.Diagnostics.Debug.WriteLine($"Destroy: {nameof(FirstModalViewModel)}");
        }
    }
}