using System.Threading.Tasks;
using Xamarin.Forms;
using xfOTP.Services;
using ZXing.Mobile;

[assembly: Dependency(typeof(xfOTP.iOS.Services.QrScanningService))]  
namespace xfOTP.iOS.Services
{
    public class QrScanningService : IQrScanningService
    {
        public async Task<string> ScanAsync()
        {
            var options = new MobileBarcodeScanningOptions();

            var scanner = new MobileBarcodeScanner()
            {
                TopText = "Scan the QR Code",
                BottomText = "Please Wait",
            };

            var scanResult = await scanner.Scan(options);
            return scanResult?.Text;
        }
    }
}