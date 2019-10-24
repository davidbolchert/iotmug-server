using IoTMug.Services.Interfaces;
using IoTMug.Services.Shared;
using Microsoft.Extensions.Options;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System;
using System.Reflection;
using System.Security.Cryptography;

namespace IoTMug.Services.Implementations
{
    public class CertificateServiceImplementation : ICertificateService
    {
        private readonly CertificateServiceSettings _settings;
        private readonly X509Certificate2 _intermediateCertificate;
        public CertificateServiceImplementation(IOptions<CertificateServiceSettings> settings)
        {
            _settings = settings.Value;
            var certificatePath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), _settings.PfxCertificatePath);
            _intermediateCertificate = new X509Certificate2(certificatePath, _settings.PrivateKeyPassword);
        }

        public X509Certificate2 GenerateDeviceCertificate(string commonName)
        {
            commonName = commonName.Replace("_", "__").Replace(' ', '_');

            using RSA rsa = RSA.Create(2048);
            var distinguishedName = new X500DistinguishedName($"C = FR, S = Alsace,L = Strasbourg,O = MUG,CN={commonName}", X500DistinguishedNameFlags.UseCommas | X500DistinguishedNameFlags.DoNotUseQuotes);
            var certificateRequest = new CertificateRequest(distinguishedName, rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

            certificateRequest.CertificateExtensions.Add(new X509BasicConstraintsExtension(false, true, 0, false));
            certificateRequest.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.KeyEncipherment | X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.NonRepudiation, false));
            var certificateResult = certificateRequest.Create(_intermediateCertificate, DateTimeOffset.UtcNow.AddMinutes(-10), DateTimeOffset.UtcNow.AddDays(365), new byte[] { 1, 2, 7, 4 });
            var cert = certificateResult.CopyWithPrivateKey(rsa);
            
            return cert;
        }
    }
}
