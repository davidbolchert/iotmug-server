using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IoTMug.Services.Interfaces
{
    public struct IoTHubMethods
    {
        public const string UPDATE_TWIN = "UpdateTwin";
    }


    public interface IIoTHubService
    {
        Task AddDevice(string deviceId);
        Task UpdateDeviceTwin(string deviceId, JObject jsonTwinDesired);
        Task<int> ExecuteMethodOnDevice(string methodName, string deviceId);
    }
}
