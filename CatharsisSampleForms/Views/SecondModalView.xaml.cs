using CatharsisSampleForms.Helpers;
using CatharsisSampleForms.ViewModels;
using ReactiveUI;
using ReactiveUI.XamForms;

namespace CatharsisSampleForms.Views
{
    public partial class SecondModalView : ReactiveContentPage<SecondModalViewModel>
    {
        public SecondModalView()
        {
            InitializeComponent();

            this.BindCommand(ViewModel, x => x.PushPage, x => x.PushPageButton);
            this.BindCommand(ViewModel, x => x.PopModal, x => x.PopModalButton);

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