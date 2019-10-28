using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IoTMug.Services.Interfaces
{
    public interface IIoTHubService
    {
        Task UpdateDeviceTwin(string deviceId, JObject jsonTwinDesired);
        Task<int> ExecuteMethodOnDevice(string methodName, string deviceId);
        Task<bool> IsDeviceConnected(string deviceId);
    }
}
