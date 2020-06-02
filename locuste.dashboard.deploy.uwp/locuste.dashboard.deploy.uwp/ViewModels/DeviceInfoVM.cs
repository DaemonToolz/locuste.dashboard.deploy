using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using locuste.dashboard.deploy.uwp.Models;

namespace locuste.dashboard.deploy.uwp.ViewModels
{
    public class DeviceInfoVM : Bindable{
     
        private DeviceInfo _device;

        public DeviceInfo Device { get => _device; set => SetField(ref _device, value); }

        public DeviceInfoVM() { }
        public DeviceInfoVM(DeviceInfo device) {
            Device = device;
        }
        
    }
}
