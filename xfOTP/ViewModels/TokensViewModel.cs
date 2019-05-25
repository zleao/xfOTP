using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;
using xfOTP.Models;

namespace xfOTP.ViewModels
{
    public class TokensViewModel : BaseViewModel
    {
        public ObservableCollection<Item> Tokens { get; set; }
        public Command LoadTokensCommand { get; set; }

        public TokensViewModel()
        {
            Title = "Tokens";
            Tokens = new ObservableCollection<Item>();
            LoadTokensCommand = new Command(async () => await ExecuteLoadItemsCommand());
        }

        async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Tokens.Clear();
                var items = await DataStore.GetItemsAsync(true);
                foreach (var item in items)
                {
                    Tokens.Add(item);
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
    }
}