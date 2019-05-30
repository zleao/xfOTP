using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using xfOTP.Exceptions;
using xfOTP.Models;

[assembly: Dependency(typeof(xfOTP.Services.TokensService))]
namespace xfOTP.Services
{
    public class TokensService : ITokensService
    {

        protected ITokenStore TokenStore => DependencyService.Get<ITokenStore>();

        public async Task<Token> CreateNewTokenAsync(string otpAuthString)
        {
            try
            {
                //Create the correspondent URI to facilitate the parsing
                var otpAuthUri = new Uri(otpAuthString);

                //validate the otp uri
                var token = ValidateAndExtractTokenValues(otpAuthUri);

                //Add token to a secure data store
                await TokenStore.SaveTokenAsync(token);

                return token;
                
            }
            catch (Exception e)
            {
                throw new NewTokenUnexpectedException(e);
            }
        }

        private Token ValidateAndExtractTokenValues(Uri otpAuthUri)
        {
            if(!otpAuthUri.Scheme.Equals("otpauth", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new OtpAuthInvalidUriException($"Invalid scheme: {otpAuthUri}");
            }

            if(!otpAuthUri.Host.Equals("totp", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new OtpAuthInvalidUriException($"Invalid host: {otpAuthUri}");
            }

            //try to extract the account info
            var account = otpAuthUri.LocalPath.Split(':').LastOrDefault();
            if (!(account?.Length > 0))
            {
                throw new OtpAuthInvalidUriException($"Not able to extract account info {otpAuthUri}");
            }

            if (!(otpAuthUri.Query?.Length > 0))
            {
                throw new OtpAuthInvalidUriException($"Query string is empty. {otpAuthUri}");
            }

            var queryDictionary = System.Web.HttpUtility.ParseQueryString(otpAuthUri.Query);
            if(!queryDictionary.HasKeys())
            {
                throw new OtpAuthInvalidUriException($"Unable to extract parameters from query string. {otpAuthUri}");
            }
            if(queryDictionary.AllKeys.All(k => !k.Equals("secret", StringComparison.InvariantCulture)))
            {
                throw new OtpAuthInvalidUriException($"Unable to extract secret from query string. {otpAuthUri}");
            }

            var secret = queryDictionary["secret"];
            var issuer = queryDictionary.AllKeys.Any(k => k.Equals("issuer", StringComparison.InvariantCulture)) ? queryDictionary["issuer"] : string.Empty;

            return new Token()
            {
                Id = Guid.NewGuid(),
                Account = account,
                Issuer = issuer,
                Secret = secret
            };
        }
    }
}
