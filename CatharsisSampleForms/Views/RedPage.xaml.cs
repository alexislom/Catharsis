using CatharsisSampleForms.Helpers;
using CatharsisSampleForms.ViewModels;
using ReactiveUI;
using ReactiveUI.XamForms;

namespace CatharsisSampleForms.Views
{
    public partial class RedPage : ReactiveContentPage<RedPageViewModel>
    {
        public RedPage()
        {
            InitializeComponent();

            this.BindCommand(ViewModel, x => x.PopModal, x => x.PopModal);
            this.BindCommand(ViewModel, x => x.PushPage, x => x.PushPage);
            this.BindCommand(ViewModel, x => x.PopPage, x => x.PopPage);
            this.BindCommand(ViewModel, x => x.PopToRoot, x => x.PopToRoot);

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