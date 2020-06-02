using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using locuste.dashboard.deploy.uwp.Models;
using locuste.dashboard.deploy.uwp.Utils;
using locuste.dashboard.deploy.uwp.ViewModels;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace locuste.dashboard.deploy.uwp.Frames
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class RegisterPage : Page, INotifyPropertyChanged
    {
        private DeviceInfoVM _targetDevice = new DeviceInfoVM()
        {
            Device = new DeviceInfo()
        };

        public DeviceInfoVM TargetDevice
        {
            get => _targetDevice;
            set => SetField(ref _targetDevice, value);
        }

        private ObservableCollection<DeviceInfoVM> _registeredDevices = new ObservableCollection<DeviceInfoVM>();

        public ObservableCollection<DeviceInfoVM> RegisteredDevices
        {
            get => _registeredDevices;
            set => SetField(ref _registeredDevices, value);

        }
        public RegisterPage()
        {
            RegisteredDevices = DeviceDiscovery.DiscoverRegisteredDevices();
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;
       
        }

        private void ValidateBtn_Click(object sender, RoutedEventArgs e)
        {
            if(String.IsNullOrWhiteSpace(TargetDevice.Device.Name)) return;
            if (String.IsNullOrWhiteSpace(TargetDevice.Device.IPAddress)) return;
            var localSettings = ApplicationData.Current.LocalSettings;
            

            ApplicationDataCompositeValue storedValue = new ApplicationDataCompositeValue
            {
                ["Name"] = TargetDevice.Device.Name, ["IPAddress"] = TargetDevice.Device.IPAddress
            };

            localSettings.Values[$"Device_{TargetDevice.Device.Name}"] = storedValue;
            RegisteredDevices.Add(new DeviceInfoVM()
            {
                Device = TargetDevice.Device
            });
            TargetDevice.Device = new DeviceInfo();
            
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        private void DeleteDeviceBtn_Click(object sender, RoutedEventArgs e)
        {
            string valueToDelete = (sender as FrameworkElement).Tag as string;
            ApplicationData.Current.LocalSettings.Values.Remove($"Device_{valueToDelete}");
            RegisteredDevices.Remove(RegisteredDevices.Single(device => device.Device.Name == valueToDelete));
        }
    }
}
