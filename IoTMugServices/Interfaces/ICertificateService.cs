using System.Security.Cryptography.X509Certificates;

namespace IoTMug.Services.Interfaces
{
    public interface ICertificateService
    {
        X509Certificate2 GenerateDeviceCertificate(string commonName);
    }
}
