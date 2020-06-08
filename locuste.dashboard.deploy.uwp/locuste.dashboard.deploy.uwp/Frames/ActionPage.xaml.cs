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
using System.Net.Sockets;
using System.Threading.Tasks;
using Windows.ApplicationModel.ExtendedExecution.Foreground;
using Windows.Storage.Pickers;
using Windows.UI.Core;
using locuste.dashboard.deploy.uwp.Controls.Dialogs;
using locuste.dashboard.deploy.uwp.Utils;
using locuste.dashboard.deploy.uwp.Web;
using locuste.dashboard.deploy.uwp.Web.Http;
using Microsoft.Toolkit.Uwp.UI.Extensions;


namespace locuste.dashboard.deploy.uwp.Frames
{

    public sealed partial class ActionPage : Page, INotifyPropertyChanged
    {
        private SocketIoWrapper SocketListener;

        private HttpClient _client;
        private Rect _size;
        private UIStatus _lastStatus;
        public ActionPage()
        {
            RegisteredDevices = DeviceDiscovery.DiscoverRegisteredDevices();
            this.InitializeComponent();
            InstallInfo = new ProgressIndicatorVM();
            var sz = Window.Current.Bounds;
            sz.Width /= 2;
            Size = sz;
            this.SizeChanged += MainPage_SizeChanged;
  
            NavigationCacheMode = NavigationCacheMode.Enabled;
            MainSection.DataContext = this;
        }



        private DeviceInfoVM _targetDevice = new DeviceInfoVM()
        {
            Device = new DeviceInfo()
        };


        private ProgressIndicatorVM _installInfo = new ProgressIndicatorVM()
        {
            Indicator = new ProgressIndicator()
        };


        public ProgressIndicatorVM InstallInfo
        {
            get => _installInfo;
            private set => SetField(ref _installInfo, value);
        }

        public UIStatus LastStatus
        {
            get => _lastStatus;
            private set => SetField(ref _lastStatus, value);
        }

        public DeviceInfoVM TargetDevice
        {
            get => _targetDevice;
            private  set => SetField(ref _targetDevice, value);
        }

        private ObservableCollection<DeviceInfoVM> _registeredDevices = new ObservableCollection<DeviceInfoVM>();

        public ObservableCollection<DeviceInfoVM> RegisteredDevices
        {
            get => _registeredDevices;
            private set => SetField(ref _registeredDevices, value);

        }

        private bool _hasSelection = false;

        public bool HasSelection
        {
            get => _hasSelection;
            private set => SetField(ref _hasSelection, value);

        }

        private bool _isFree = true;

        public bool IsFree
        {
            get => _isFree;
            private  set => SetField(ref _isFree, value);

        }

        public HttpClient Client
        {
            get => _client;
            set => SetField(ref _client, value);

        }

        private bool _versionLoadingPanel = false;

        private bool VersionLoadingPanel
        {
            get => _versionLoadingPanel;
            set => SetField(ref _versionLoadingPanel, value);

        }

        private bool _isConnecting = false;

        public bool IsConnecting
        {
            get => _isConnecting;
            private set => SetField(ref _isConnecting, value);

        }


        private bool _ongoingOperation = false;

        public bool OngoingOperation
        {
            get => _ongoingOperation;
            set => SetField(ref _ongoingOperation, value);

        }

        public Rect Size
        {
            get => _size;
            set => SetField(ref _size, value);

        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

 
        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            HasSelection = false;
            if (TargetDevice == null){ return;}

            IsConnecting = true;
            if (SocketListener != null)
            {
                SocketListener.FileCopyInfoHandler -= FileCopyInfoReceived;
                SocketListener.ProgressUpdateHandler -= ProgressReceived;
                SocketListener.OnConnectionEvent -= ConnexionEventReceived;
                SocketListener.DisconnectedByUser = true;
                SocketListener.Disconnect();
            }

            SocketListener = new SocketIoWrapper(TargetDevice.Device.IPAddress);
            SocketListener.OnConnectionEvent += ConnexionEventReceived;

            SocketListener.Connect().ContinueWith(result =>
            {
             
                void  AgileCallback()
                {
                    IsConnecting = false;
                    HasSelection = result.Result;
                    Client = new HttpClient(TargetDevice.Device.IPAddress);
                }

                SocketListener.FileCopyInfoHandler += FileCopyInfoReceived;
                SocketListener.ProgressUpdateHandler += ProgressReceived;
               
                Dispatcher?.RunAsync(CoreDispatcherPriority.Normal, AgileCallback);
            });
        }

        private void SessionRevoked(object sender, ExtendedExecutionForegroundRevokedEventArgs args)
        {

        }

        private void FileCopyInfoReceived(object sender, FileCopyInfoArgs args)
        {
            _ = Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    OngoingOperation = args.FileCount != args.FileIndex;
                    IsFree = !OngoingOperation;
                });
        }


        private void ConnexionEventReceived(object sender, ConnectionEventArgs args)
        {
            Task.Delay(10000).ContinueWith(t =>
                Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { LastStatus = null; })
            );

            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                LastStatus = args.Status;
            });
        }


        private void ProgressReceived(object sender, ProgressIndicatorArgs args)
        { 
            _   = Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    InstallInfo = new ProgressIndicatorVM(args);
                    IsFree = InstallInfo.Indicator.Status != EventStatus.InProgress;
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

        private void VersionUploadBtn_Click(object sender, RoutedEventArgs e)
        {
            MonitorAction(typeof(VersionUploaderPage));
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

        private void InstallVersionBtn_Click(object sender, RoutedEventArgs e)
        {
            MonitorAction(typeof(InstallProcessPage));
        }

        private void MainPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var sz = Window.Current.Bounds;
            sz.Width /= 2;
            Size = sz;
        }
        private DependencyObject FindChildControl<T>(DependencyObject control, string ctrlName)
        {
            int childNumber = VisualTreeHelper.GetChildrenCount(control);
            for (int i = 0; i < childNumber; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(control, i);
                FrameworkElement fe = child as FrameworkElement;
                // Not a framework element or is null
                if (fe == null) return null;

                if (child is T && fe.Name == ctrlName)
                {
                    // Found the control so return
                    return child;
                }
                else
                {
                    // Not found it - search children
                    DependencyObject nextLevel = FindChildControl<T>(child, ctrlName);
                    if (nextLevel != null)
                        return nextLevel;
                }
            }
            return null;
        }

        private void StartOrStopAppBtn_Click(object sender, RoutedEventArgs e)
        {
            MonitorAction(typeof(ManageAppPage));
        }

        private void MonitorAction(Type t)
        {

            if (FindChildControl<Frame>(MainSection, "MonitorFrame") is Frame res && res.SourcePageType != t)
            {
                res?.Navigate(t, new WebClientParam
                {
                    Client = Client,
                    Wrapper = SocketListener
                });
            }
        }
    }
}
