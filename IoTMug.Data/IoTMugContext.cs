using IoTMug.Core;
using Microsoft.EntityFrameworkCore;

namespace IoTMug.Data
{
    public class IoTMugContext: DbContext
    {
        public IoTMugContext(DbContextOptions<IoTMugContext> contextOption) : base(contextOption) { }

        public DbSet<Device> Devices { get; set; }
        public DbSet<DeviceType> DeviceTypes { get; set; }
    }
}
