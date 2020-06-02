using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using locuste.dashboard.deploy.uwp.ViewModels;
using locuste.dashboard.deploy.uwp.Web.SocketIO;
using System.ComponentModel;
using Windows.Storage.Pickers;
using Windows.UI.Core;
using locuste.dashboard.deploy.uwp.Controls.Dialogs;
using locuste.dashboard.deploy.uwp.Utils;
using locuste.dashboard.deploy.uwp.Web.Http;


namespace locuste.dashboard.deploy.uwp.Frames
{

    public sealed partial class ActionPage : Page, INotifyPropertyChanged
    {
        private SocketIoWrapper SocketListener;
        private HttpClient Client;
        public ActionPage()
        {
            RegisteredDevices = DeviceDiscovery.DiscoverRegisteredDevices();
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
            MainSection.DataContext = this;
        }



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

        private bool _hasSelection = false;

        public bool HasSelection
        {
            get => _hasSelection;
            set => SetField(ref _hasSelection, value);

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

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            HasSelection = false;
            if (TargetDevice == null){ return;}

            SocketListener?.Disconnect();

            SocketListener = new SocketIoWrapper(TargetDevice.Device.IPAddress);
            SocketListener.Connect().ContinueWith(result =>
            {
                void  AgileCallback()
                {
                    HasSelection = result.Result;
                }

                Dispatcher?.RunAsync(CoreDispatcherPriority.Normal, AgileCallback);
                Client = new HttpClient(TargetDevice.Device.IPAddress);

            });
        }

        private void ListVersionBtn_Click(object sender, RoutedEventArgs e)
        {
            Client.GetVersions().ContinueWith( results =>
            {
                Dispatcher?.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    var dialog = new VersionListDialog(results.Result)
                    {
                        Title = "Liste des versions disponibles",
                    };

                    await dialog.ShowAsync();
                });

            });
        }

        private async void VersionUploadBtn_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker()
            {
                ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };
            picker.FileTypeFilter.Add(".zip");
            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                Client.SendInstallPackage("beta-test", file);
            }
        }

        private void DeleteVersionBtn_Click(object sender, RoutedEventArgs e)
        {
            Client.GetVersions().ContinueWith(results =>
            {
                Dispatcher?.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    var dialog = new VersionDeleteDialog(results.Result, Client)
                    {
                        Title = "Supprimer une ou des versions",
                    };

                    await dialog.ShowAsync();
                });

            });
        }
    }
}
