using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography.X509Certificates;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using locuste.dashboard.deploy.uwp.Web.Http;

// Pour plus d'informations sur le modèle d'élément Boîte de dialogue de contenu, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace locuste.dashboard.deploy.uwp.Controls.Dialogs
{
    public sealed partial class VersionInstallDialog : ContentDialog
    {
        private List<string> Versions;
        private HttpClient _client;
        public VersionInstallDialog(List<string> data, HttpClient client)
        {
            _client = client;
            Versions = data;
            this.InitializeComponent();
            VersionList.ItemsSource = data;

        }

        private void VersionInstallDialog_OnPrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if ( _client != null && VersionList.SelectedItem != null)
            {
                _client.StartInstallationProcedure(VersionList.SelectedItem.ToString());
            }
        }
    }
}
