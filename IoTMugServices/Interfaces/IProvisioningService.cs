using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace IoTMug.Services.Interfaces
{ 
    public interface IProvisioningService
    {
        Task<bool> RegisterAsync(X509Certificate2 certificate);
    }
}
