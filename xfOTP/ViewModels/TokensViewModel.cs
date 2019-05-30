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

        public ITokenStore TokenStore => DependencyService.Get<ITokenStore>();

        public ObservableCollection<TokenViewModel> Tokens { get; set; }

        public IQrScanningService ScannerService => DependencyService.Get<IQrScanningService>();

        public ITokensService TokensService => DependencyService.Get<ITokensService>();

        #endregion

        #region Commands

        public Command RefreshTokensCommand { get; set; }
        public Command AddTokenCommand { get; set; }

        #endregion

        #region Constructor

        public TokensViewModel()
        {
            Title = "Tokens";
            Tokens = new ObservableCollection<TokenViewModel>();
            RefreshTokensCommand = new Command(async () => await ExecuteRefreshTokensCommand());
            AddTokenCommand = new Command(async () => await ExecuteAddTokenCommand());
        }

        #endregion

        #region Methods

        private async Task ExecuteRefreshTokensCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Tokens.Clear();
                var tokens = await TokenStore.GetTokensAsync(true);
                foreach (var token in tokens)
                {
                    Tokens.Add(new TokenViewModel(token));
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
                    await UpdateTokensListAsync(await TokensService.CreateNewTokenAsync(qrCode));
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

        private async Task UpdateTokensListAsync(Guid tokenId)
        {
            if (Tokens.Any(t => t.Token.Id == tokenId))
            {
                return;
            }

            Tokens.Add(new TokenViewModel(await TokenStore.GetTokenAsync(tokenId.ToString())));
        }

        #endregion
    }
}