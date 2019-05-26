using System.Threading.Tasks;

namespace xfOTP.Services
{
    public interface IQrScanningService
    {
        Task<string> ScanAsync();
    }
}
