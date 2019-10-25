using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace IoTMug.Shared.Extensions
{
    public static class CertificateRequestExtension
    {
        public static void AddAuthorityKeyIdentifier(this CertificateRequest request, X509Certificate2 signingCertificate)
        {
            // set the AuthorityKeyIdentifier. There is no built-in 
            // support, so it needs to be copied from the Subject Key 
            // Identifier of the signing certificate and massaged slightly.
            // AuthorityKeyIdentifier is "KeyID="
            var issuerSubjectKey = signingCertificate.Extensions.OfType<X509SubjectKeyIdentifierExtension>().First().RawData;
            var segment = new ArraySegment<byte>(issuerSubjectKey, 2, issuerSubjectKey.Length - 2); //new ArraySegment(issuerSubjectKey, 2, issuerSubjectKey.Length - 2);
            var authorityKeyIdentifer = new byte[segment.Count + 4];
            // these bytes define the "KeyID" part of the AuthorityKeyIdentifer
            authorityKeyIdentifer[0] = 0x30;
            authorityKeyIdentifer[1] = 0x16;
            authorityKeyIdentifer[2] = 0x80;
            authorityKeyIdentifer[3] = 0x14;
            segment.CopyTo(authorityKeyIdentifer, 4);
            request.CertificateExtensions.Add(new X509Extension("2.5.29.35", authorityKeyIdentifer, false));
        }
    }
}
