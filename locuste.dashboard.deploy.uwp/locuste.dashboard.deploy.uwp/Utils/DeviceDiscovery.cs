using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using locuste.dashboard.deploy.uwp.Models;
using locuste.dashboard.deploy.uwp.ViewModels;

namespace locuste.dashboard.deploy.uwp.Utils
{
    public static class DeviceDiscovery
    {

        public static ObservableCollection<DeviceInfoVM> DiscoverRegisteredDevices()
        {
            var storage = new ObservableCollection<DeviceInfoVM>();
            var localSettings = ApplicationData.Current.LocalSettings;
            foreach (var pair in localSettings.Values)
            {
                if (pair.Key.StartsWith("Device_"))
                {
                    storage.Add(new DeviceInfoVM()
                    {
                        Device = new DeviceInfo()
                        {
                            Name = (pair.Value as ApplicationDataCompositeValue)["Name"].ToString(),
                            IPAddress = (pair.Value as ApplicationDataCompositeValue)["IPAddress"].ToString()
                        }

                    });
                }
            }

            return storage;
        }
    }
}
