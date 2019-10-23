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
    public class CertificateService : ICertificateService
    {
        private readonly CertificateServiceSettings _settings;
        private readonly X509Certificate2 _intermediateCertificate;
        public CertificateService(IOptions<CertificateServiceSettings> settings)
        {
            _settings = settings.Value;
            var certificatePath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), _settings.PfxCertificatePath);
            _intermediateCertificate = new X509Certificate2(certificatePath, _settings.PrivateKeyPassword);
        }

        public byte[] GenerateDeviceCertificate(string commonName, string password)
        {
            commonName = commonName.Replace("_", "__").Replace(' ', '_');

            using RSA rsa = RSA.Create(2048);
            var certificateRequest = new CertificateRequest(new X500DistinguishedName($"CN={commonName}"), rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

            certificateRequest.CertificateExtensions.Add(new X509BasicConstraintsExtension(false, false, 0, false));
            certificateRequest.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.NonRepudiation, false));
            certificateRequest.CertificateExtensions.Add(new X509EnhancedKeyUsageExtension(new OidCollection { new Oid("1.3.6.1.5.5.7.3.8") }, true));
            certificateRequest.CertificateExtensions.Add(new X509SubjectKeyIdentifierExtension(certificateRequest.PublicKey, false));

            return certificateRequest
                .Create(_intermediateCertificate, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddDays(360), new byte[] { 1, 2, 3, 4 })
                .Export(X509ContentType.Pfx, password);
        }
    }
}
