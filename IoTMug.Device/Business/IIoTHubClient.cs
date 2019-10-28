using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IoTMug.Device.Business
{
    public interface IIoTHubClient
    {
        Task ConnectAsync();
        Task DisconnectAsync();
        Task GetTwinConfigurationAsync();
        Task Run(CancellationToken token);
    }
}
