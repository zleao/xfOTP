using OtpNet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using xfOTP.Models;
using xfOTP.Services;

namespace xfOTP.ViewModels
{
    public class TokensViewModel : BaseViewModel
    {
        #region Properties

        public ObservableCollection<TokenViewModel> Tokens { get; set; }

        public IQrScanningService ScannerService => DependencyService.Get<IQrScanningService>();

        public ITokensService TokensService => DependencyService.Get<ITokensService>();

        #endregion

        #region Commands

        public Command LoadTokensCommand { get; set; }
        public Command AddTokenCommand { get; set; }

        #endregion

        #region Constructor

        public TokensViewModel()
        {
            Title = "Tokens";
            Tokens = new ObservableCollection<TokenViewModel>();
            LoadTokensCommand = new Command(async () => await ExecuteLoadItemsCommand());
            AddTokenCommand = new Command(async () => await ExecuteAddTokenCommand());
        }

        #endregion

        #region Methods

        private Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return Task.CompletedTask;

            IsBusy = true;

            try
            {
                //Tokens.Clear();
                //var items = await DataStore.GetItemsAsync(true);
                //foreach (var item in items)
                //{
                //    Tokens.Add(item);
                //}
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }

            return Task.CompletedTask;
        }

        private async Task ExecuteAddTokenCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                var qrCode = await ScannerService.ScanAsync();
                if (qrCode.Length > 0)
                {
                    UpdateTokensList(await TokensService.CreateNewTokenAsync(qrCode));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }

            return;
        }

        private void UpdateTokensList(Guid tokenId)
        {
            if (Tokens.Any(t => t.Token.Id == tokenId))
            {
                return;
            }

            Tokens.Add(new TokenViewModel(new Token() { Id = tokenId }));
        }

        #endregion
    }
}