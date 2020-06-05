using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
using locuste.dashboard.deploy.uwp.Models;
using locuste.dashboard.deploy.uwp.ViewModels;
using locuste.dashboard.deploy.uwp.Web;
using locuste.dashboard.deploy.uwp.Web.Http;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace locuste.dashboard.deploy.uwp.Frames
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class ManageAppPage : Page, INotifyPropertyChanged
    {
        private HttpClient Client;
        public ManageAppPage()
        {
            this.InitializeComponent();
        } // Client.GetInstalledVersion();

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);


            Client = (e.Parameter as WebClientParam)?.Client;
            Client?.GetInstalledVersion().ContinueWith(async task =>
                {
                    task.Wait();
                    
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                        () =>
                        {
                            ProjectVersion.Version = task.Result;
                        });
                });
        }


        private ProjectVersionVM _projectVersion = new ProjectVersionVM();


        public ProjectVersionVM ProjectVersion
        {
            get => _projectVersion;
            private set => SetField(ref _projectVersion, value);
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

    }
}
