using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using locuste.dashboard.deploy.uwp.Controls.Dialogs;
using locuste.dashboard.deploy.uwp.Models;
using locuste.dashboard.deploy.uwp.ViewModels;
using locuste.dashboard.deploy.uwp.Web;
using locuste.dashboard.deploy.uwp.Web.Http;
using locuste.dashboard.deploy.uwp.Web.SocketIO;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace locuste.dashboard.deploy.uwp.Frames
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class InstallProcessPage : Page, INotifyPropertyChanged
    {
        private SocketIoWrapper SocketListener;
        private HttpClient Client;
        private FileCopyInfoVM _copyInfo = new FileCopyInfoVM()
        {
            Info = new FileCopyInfo()
        };

        public FileCopyInfoVM CopyInfo
        {
            get => _copyInfo;
            private set => SetField(ref _copyInfo, value);
        }

        public InstallProcessPage()
        {
            this.InitializeComponent();
        }

        private bool _isBusy = false;

        public bool IsBusy
        {
            get => _isBusy;
            private set => SetField(ref _isBusy, value);

        }


        private bool _ongoingOperation = false;

        public bool OngoingOperation
        {
            get => _ongoingOperation;
            private set => SetField(ref _ongoingOperation, value);

        }


        private ProgressIndicatorVM _installInfo = new ProgressIndicatorVM();

        public ProgressIndicatorVM InstallInfo
        {
            get => _installInfo;
            private set => SetField(ref _installInfo, value);
        }

        private void FileCopyInfoReceived(object sender, FileCopyInfoArgs args)
        {

            _ = Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    CopyInfo = new FileCopyInfoVM(args);
                    OngoingOperation = true;
                    IsBusy = args.FileCount != args.FileIndex;
                    if (InstallInfo.Indicator.Status != EventStatus.InProgress)
                    {
                        InstallInfo = new ProgressIndicatorVM(new ProgressIndicator()
                        {
                            Status =  EventStatus.InProgress,
                            Message = "Installation en cours"
                        });
                    }
                });

        }


        private void ProgressReceived(object sender, ProgressIndicatorArgs args)
        {
            _ = Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    InstallInfo = new ProgressIndicatorVM(args);
                    OngoingOperation = IsBusy = InstallInfo.Indicator.Status == EventStatus.InProgress;
                });
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            
            SocketListener = (e.Parameter as WebClientParam)?.Wrapper;
            if (SocketListener != null)
            {
                SocketListener.FileCopyInfoHandler += FileCopyInfoReceived;
                SocketListener.ProgressUpdateHandler += ProgressReceived;
            }
            Client = (e.Parameter as WebClientParam)?.Client;
        }

        private void InstallVersionBtn_Click(object sender, RoutedEventArgs e)
        {

            Client.GetVersions().ContinueWith(results => {
                Dispatcher?.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    var dialog = new VersionInstallDialog(results.Result, Client)
                    {
                        Title = "Installer une version",
                    };

                    await dialog.ShowAsync();

                });

            });
        }

        private void UninstallBtn_Click(object sender, RoutedEventArgs e)
        {
            Client.Uninstall();
        }
    }
}
