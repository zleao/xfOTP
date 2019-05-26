using System;
using System.Threading.Tasks;

namespace xfOTP.Services
{
    public interface ITokensService
    {
        /// <summary>
        /// Tries to create a new token based on an otpauth uri
        /// </summary>
        /// <param name="otpAuthString">uri based otpauth info (e.g. otpauth://totp/Example:alice@google.com?secret=mv4gc3lqnrssa43fmnzgk5a&issuer=Example)</param>
        /// <returns>Guid of the newlly created token</returns>
        Task<Guid> CreateNewTokenAsync(string otpAuthString);
    }
}
