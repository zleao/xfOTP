using System;
using System.ComponentModel;
using Xamarin.Forms;
using xfOTP.ViewModels;

namespace xfOTP.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class TokensPage : ContentPage
    {
        TokensViewModel viewModel;

        public TokensPage()
        {
            InitializeComponent();

            BindingContext = viewModel = new TokensViewModel();
        }

        private void AddToken_Clicked(object sender, EventArgs e)
        {
            viewModel.AddTokenCommand.Execute(null);
        }

        async void EditTokens_Clicked(object sender, EventArgs e)
        {
            await DisplayAlert("Edit Tokens", "Comming soon...", "Ok");
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (viewModel.Tokens.Count == 0)
                viewModel.LoadTokensCommand.Execute(null);
        }
    }
}