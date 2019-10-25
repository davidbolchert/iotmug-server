using IoTMug.Core;
using IoTMug.Shared.Helpers;
using System.Security.Cryptography.X509Certificates;

namespace IoTMug.Shared.Extensions
{
    public static class DeviceExtensions
    {
        public static string GetPassword(this Device device) => CryptographyHelper.GetHashString(device.CommonName);
        public static X509Certificate2 GetRegistrationCertificate(this Device device) => new X509Certificate2(device.PfxCertificate, device.GetPassword());
    }
}
