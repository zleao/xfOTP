using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using xfOTP.Models;

namespace xfOTP.Services
{
    public interface ITokenStore
    {
        Task<bool> SaveTokenAsync(Token token);
        Task<bool> DeleteTokenAsync(string id);
        Task<bool> DeleteAllTokens();
        Task<Token> GetTokenAsync(string id);
        Task<IEnumerable<Token>> GetTokensAsync(bool forceRefresh = false);
    }
}
