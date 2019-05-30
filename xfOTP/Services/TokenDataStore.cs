using Newtonsoft.Json;
using Plugin.SimpleLogger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using xfOTP.Models;

[assembly: Dependency(typeof(xfOTP.Services.TokenDataStore))]
namespace xfOTP.Services
{
    public class TokenDataStore : ITokenStore
    {
        private const string StoredTokensKeys = nameof(StoredTokensKeys);

        List<Token> tokensCache = new List<Token>();

        public async Task<bool> SaveTokenAsync(Token token)
        {
            try
            {
                if (tokensCache.All(t => t.Id != token.Id))
                {
                    await SecureStorage.SetAsync(token.Id.ToString(), JsonConvert.SerializeObject(token));
                    tokensCache.Add(token);
                    await AddTokenKeyAsync(token.Id.ToString());
                }

                return true;
            }
            catch (Exception ex)
            {
                CrossSimpleLogger.Current.Error("Error adding a token to the storage", ex);
                return false;
            }
        }

        public async Task<bool> DeleteTokenAsync(string id)
        {
            try
            {
                SecureStorage.Remove(id);
                await RemoveTokenKeyAsync(id);

                var toRemove = tokensCache.FirstOrDefault(t => t.Id.ToString() == id);
                if (toRemove != null)
                {
                    tokensCache.Remove(toRemove);
                }

                return true;
            }
            catch (Exception ex)
            {
                CrossSimpleLogger.Current.Error($"Error deleting token from the storage (id:{id})", ex);
                return false;
            }
        }

        public async Task<bool> DeleteAllTokens()
        {
            try
            {
                var tokensKeys = await GetStoredTokensKeysAsync();
                foreach (var tokenKey in tokensKeys)
                {
                    await DeleteTokenAsync(tokenKey);
                }
                return true;
            }
            catch (Exception ex)
            {
                CrossSimpleLogger.Current.Error($"Error deleting all tokens from the storage", ex);
                return false;
            }
        }

        public async Task<Token> GetTokenAsync(string id)
        {
            try
            {
                if (tokensCache.All(t => t.Id.ToString() != id))
                {
                    await GetTokensAsync(true);
                }

                return tokensCache.FirstOrDefault(t => t.Id.ToString() == id);
            }
            catch (Exception ex)
            {
                CrossSimpleLogger.Current.Error($"Error getting token from the storage (id:{id})", ex);
                return null;
            }
        }

        public async Task<IEnumerable<Token>> GetTokensAsync(bool forceRefresh = false)
        {
            try
            {
                if (forceRefresh || tokensCache.Count == 0)
                {
                    tokensCache.Clear();

                    var tokensKeys = await GetStoredTokensKeysAsync();
                    foreach (var tokenKey in tokensKeys)
                    {
                        var token = JsonConvert.DeserializeObject<Token>(await SecureStorage.GetAsync(tokenKey));
                        if (token != null)
                        {
                            tokensCache.Add(token);
                        }
                    }
                }

                return tokensCache;
            }
            catch (Exception ex)
            {
                CrossSimpleLogger.Current.Error($"Error getting tokens from the storage (forceRefresh:{forceRefresh})", ex);
                return new List<Token>();
            }
        }

        private async Task AddTokenKeyAsync(string id)
        {
            var storedKeys = await GetStoredTokensKeysAsync();

            if (storedKeys.All(k => k != id))
            {
                storedKeys.Add(id);
                await SaveStoredTokensKeys(storedKeys);
            }
        }

        private async Task RemoveTokenKeyAsync(string id)
        {
            var storedKeys = await GetStoredTokensKeysAsync();

            if (storedKeys.Any(k => k == id))
            {
                storedKeys.Remove(id);
                await SaveStoredTokensKeys(storedKeys);
            }
        }

        private async Task<List<string>> GetStoredTokensKeysAsync()
        {
            var storedKeys = new List<string>();

            var storedKeysString = await SecureStorage.GetAsync(StoredTokensKeys);
            if (storedKeysString != null)
            {
                storedKeys = JsonConvert.DeserializeObject<List<string>>(storedKeysString);
            }

            return storedKeys;
        }

        private Task SaveStoredTokensKeys(List<string> tokensKeys)
        {
            return SecureStorage.SetAsync(StoredTokensKeys, JsonConvert.SerializeObject(tokensKeys));
        }
    }
}