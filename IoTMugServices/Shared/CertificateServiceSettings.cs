using System;
using System.Collections.Generic;
using System.Text;

namespace IoTMug.Services.Shared
{
    public class CertificateServiceSettings
    {
        public string PfxCertificatePath { get; set; }
        public string PrivateKeyPassword { get; set; }
    }
}
