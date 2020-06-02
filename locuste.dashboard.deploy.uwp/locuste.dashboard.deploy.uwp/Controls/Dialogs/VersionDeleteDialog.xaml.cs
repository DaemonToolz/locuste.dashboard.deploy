using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Cryptography.Core;
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
    public sealed partial class VersionDeleteDialog : ContentDialog
    {
        public List<string> Versions;

        private HttpClient _client;
        public VersionDeleteDialog(List<string> data, HttpClient client)
        {
            _client = client;
            Versions = data;
            this.InitializeComponent();
            VersionList.ItemsSource = data;

        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void VersionDeleteDialog_OnSecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (VersionList.SelectedItems != null && VersionList.SelectedItems.Count > 0)
            {
                foreach (var versionListSelectedItem in VersionList.SelectedItems)
                {
                    _client.DeleteVersionNumber(versionListSelectedItem.ToString());
                }
            }
        }
    }
}
