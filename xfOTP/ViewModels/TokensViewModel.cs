using OtpNet;
using Plugin.Toasts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
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
        public Command TokenSelectedCommand { get; set; }
        public Command EditCommand { get; set; }

        #endregion

        #region Constructor

        public TokensViewModel()
        {
            Title = "Tokens";
            Tokens = new ObservableCollection<TokenViewModel>();
            RefreshTokensCommand = new Command(async () => await ExecuteRefreshTokensCommand());
            AddTokenCommand = new Command(async () => await ExecuteAddTokenCommand());
            TokenSelectedCommand = new Command<int>(async (i) => await ExecuteTokenSelectedCommand(i));
            EditCommand = new Command(async () => await ExecuteEditCommand());
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
                ResetTokens();
                var tokens = await TokenStore.GetTokensAsync(true);
                foreach (var token in tokens)
                {
                    UpdateTokensList(token);
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

        private async Task ExecuteTokenSelectedCommand(int tokenIndex)
        {
            var notificator = DependencyService.Get<IToastNotificator>();
            var options = new NotificationOptions()
            {
                Title = "Token",
                Description = "Code Copied"
            };

            await notificator.Notify(options);
        }

        private async Task ExecuteEditCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                ResetTokens();
                await TokenStore.DeleteAllTokensAsync();
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

        private void UpdateTokensList(Token token)
        {
            if (Tokens.Any(t => t.Token.Id == token.Id))
            {
                return;
            }

            var vm = new TokenViewModel(token);
            Tokens.Add(vm);
            vm.StartTokenMonitoring();

        }

        private void ResetTokens()
        {
            Tokens.ForEach(t => t.StopTokenMonitoring());
            Tokens.Clear();
        }

        #endregion
    }
}