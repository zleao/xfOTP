using xfOTP.Models;

namespace xfOTP.ViewModels
{
    public class TokenViewModel : BaseViewModel
    {
        public Token Token { get; }
        public string Id => Token?.Id.ToString();

        public TokenViewModel(Token token)
        {
            Token = token;
        }

        //var key = Base32Encoding.ToBytes("rzxqknjjok63dneq");
        //var totp = new Totp(key);
        //TokenCode = totp.ComputeTotp(DateTime.UtcNow);
    }
}
