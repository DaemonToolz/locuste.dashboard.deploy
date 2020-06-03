﻿using System;
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

        private HttpClient _client;
        private Rect _size;
        public ActionPage()
        {
            RegisteredDevices = DeviceDiscovery.DiscoverRegisteredDevices();
            this.InitializeComponent();
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


        private FileCopyInfoVM _copyInfo = new FileCopyInfoVM()
        {
            Info = new FileCopyInfo()
        };

        private ProgressIndicatorVM _installInfo = new ProgressIndicatorVM()
        {
            Indicator = new ProgressIndicator()
        };

        public FileCopyInfoVM CopyInfo
        {
            get => _copyInfo;
            private set => SetField(ref _copyInfo, value);
        }

        public ProgressIndicatorVM InstallInfo
        {
            get => _installInfo;
            private set => SetField(ref _installInfo, value);
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
            set => SetField(ref _registeredDevices, value);

        }

        private bool _hasSelection = false;

        public bool HasSelection
        {
            get => _hasSelection;
            set => SetField(ref _hasSelection, value);

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
            SocketListener?.Disconnect();

            SocketListener = new SocketIoWrapper(TargetDevice.Device.IPAddress);
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

        private void FileCopyInfoReceived(object sender, FileCopyInfoArgs args)
        {
      
            _ = Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    CopyInfo = new FileCopyInfoVM(args);

                    OngoingOperation = args.FileCount != args.FileIndex;
                });

        }


        private void ProgressReceived(object sender, ProgressIndicatorArgs args)
        { 
            _   = Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    InstallInfo = new ProgressIndicatorVM(args);
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
            VersionLoadingPanel = !VersionLoadingPanel;
          
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
            Client.GetVersions().ContinueWith(results =>
            {
                Dispatcher?.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    var dialog = new VersionInstallDialog(results.Result, _client)
                    {
                        Title = "Installer une version",
                    };

                    await dialog.ShowAsync();
                });

            });
          
        }

        private void MainPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var sz = Window.Current.Bounds;
            sz.Width /= 2;
            Size = sz;
        }
    }
}