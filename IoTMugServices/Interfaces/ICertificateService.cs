using System;
using System.Collections.Generic;
using System.Text;

namespace IoTMug.Services.Interfaces
{
    public interface ICertificateService
    {
        byte[] GenerateDeviceCertificate(string commonName, string password);
    }
}
