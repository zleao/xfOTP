using OtpNet;
using System;

namespace xfOTP.Models
{
    public class Token
    {
        public Guid Id { get; set; }
        public string Issuer { get; set; }
        public string Account { get; set; }
        public string Secret { get; set; }
        public int Digits { get; set; } = 6;
        public OtpHashMode HashMode { get; set; } = OtpHashMode.Sha1;
    }
}