using OtpNet;
using System;
using System.Threading;
using xfOTP.Models;

namespace xfOTP.ViewModels
{
    public class TokenViewModel : BaseViewModel
    {
        #region Fields

        private readonly byte[] _secretBytes = null;
        private Totp _totp = null;
        private Timer _timer = null;

        #endregion

        #region Properties

        public Token Token { get; }
        public string Id => Token?.Id.ToString();

        public string Code
        {
            get { return _code; }
            set { SetProperty(ref _code, value); }
        }
        private string _code;

        public double TokenProgress
        {
            get { return _tokenProgress; }
            set { SetProperty(ref _tokenProgress, value); }
        }
        private double _tokenProgress;

        #endregion

        #region Constructor

        public TokenViewModel(Token token)
        {
            Token = token;
            _secretBytes = Base32Encoding.ToBytes(Token.Secret);
            ResetData();
        }

        #endregion

        #region Methods

        public void StartTokenMonitoring()
        {
            StopTokenMonitoring();

            _totp = new Totp(_secretBytes, Token.DurationSeconds, Token.HashMode, Token.Digits);
            _timer = new Timer(TimerTick, null, 0, 100);
        }

        private void TimerTick(object state)
        {
            Code = _totp.ComputeTotp();
            var remainingProgress = ((double)_totp.RemainingSeconds() / (double)Token.DurationSeconds);
            TokenProgress = 1.0 - remainingProgress;
        }

        public void StopTokenMonitoring()
        {
            _timer?.Dispose();
            _timer = null;
            ResetData();
        }

        private void ResetData()
        {
            Code = string.Empty;
            TokenProgress = 0.0;
        }

        #endregion
    }
}
