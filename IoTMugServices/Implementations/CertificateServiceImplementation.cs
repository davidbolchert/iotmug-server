using IoTMug.Services.Interfaces;
using IoTMug.Services.Shared;
using Microsoft.Extensions.Options;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using IoTMug.Shared.Extensions;
using IoTMug.Shared.Helpers;

namespace IoTMug.Services.Implementations
{
    public class CertificateServiceImplementation : ICertificateService
    {
        private readonly CertificateServiceSettings _settings;
        private readonly X509Certificate2 _intermediateCertificate;
        public CertificateServiceImplementation(IOptions<CertificateServiceSettings> settings)
        {
            _settings = settings.Value;
            _intermediateCertificate = new X509Certificate2(LocationHelpers.GetLocationFromAssembly(_settings.PfxCertificatePath), _settings.PrivateKeyPassword);
        }

        public X509Certificate2 GenerateDeviceCertificate(string commonName)
        {
            using RSA rsa = RSA.Create(2048);
            var distinguishedName = new X500DistinguishedName($"C = FR, S = Alsace,L = Strasbourg,O = MUG,CN={commonName}", X500DistinguishedNameFlags.UseCommas | X500DistinguishedNameFlags.DoNotUseQuotes);
            var certificateRequest = new CertificateRequest(distinguishedName, rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

            certificateRequest.CertificateExtensions.Add(new X509BasicConstraintsExtension(false, false, 0, false));
            certificateRequest.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.NonRepudiation | X509KeyUsageFlags.KeyEncipherment, true));
            certificateRequest.CertificateExtensions.Add(new X509EnhancedKeyUsageExtension(new OidCollection { new Oid("1.3.6.1.5.5.7.3.2"), new Oid("1.3.6.1.5.5.7.3.4") }, false));
            certificateRequest.CertificateExtensions.Add(new X509SubjectKeyIdentifierExtension(certificateRequest.PublicKey, false));
            certificateRequest.AddAuthorityKeyIdentifier(_intermediateCertificate);

            var certificateResult = certificateRequest.Create(_intermediateCertificate, DateTimeOffset.UtcNow.AddMinutes(-10), DateTimeOffset.UtcNow.AddDays(365), Encoding.UTF8.GetBytes(DateTime.Now.Ticks.ToString()));
            return certificateResult.CopyWithPrivateKey(rsa);
        }


        // Work Around to avoid registrastion issues
        public X509Certificate2 ExportAsPfxAndReOpen(X509Certificate2 certificate)
        {
            var password =  CryptographyHelper.GetHashString(certificate.GetCommonName());
            var pfxData = certificate.Export(X509ContentType.Pfx, password);
            return new X509Certificate2(pfxData, password);
        }
    }
}
