using System.Reactive.Disposables;
using CatharsisSampleForms.Helpers;
using CatharsisSampleForms.ViewModels;
using ReactiveUI;
using ReactiveUI.XamForms;

namespace CatharsisSampleForms.Views
{
    public partial class MainPage : ReactiveContentPage<MainViewModel>
    {
        public MainPage()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.BindCommand(ViewModel, x => x.OpenFirstModalPage, x => x.FirstModalButton).DisposeWith(disposables);
                this.BindCommand(ViewModel, x => x.PushRedPage, x => x.PushPageButton).DisposeWith(disposables);
                //this.BindCommand(ViewModel, x => x.PushGenericPage, x => x.PushGenericPage).DisposeWith(disposables);
            });

            Interactions
                .ErrorMessage
                .RegisterHandler(async x =>
                {
                    await DisplayAlert("Error", x.Input.Message, "Done");
                    x.SetOutput(true);
                });
        }
    }
}