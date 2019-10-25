using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace IoTMug.Shared.Extensions
{
    public static class X509Certificate2Extensions
    {
        public static string GetCommonName(this X509Certificate2 certificate) => certificate.GetNameInfo(X509NameType.SimpleName, false);
    }
}
