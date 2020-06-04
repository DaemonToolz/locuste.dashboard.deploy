using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using locuste.dashboard.deploy.uwp.Web.Http;

// Pour en savoir plus sur le modèle d'élément Contrôle utilisateur, consultez la page https://go.microsoft.com/fwlink/?LinkId=234236

namespace locuste.dashboard.deploy.uwp.Frames
{
    public sealed partial class PackageUploaderControl : UserControl
    {
        public PackageUploaderControl()
        {
            this.InitializeComponent();
            
        }

        public ObservableCollection<StorageFile> Files { get; } = new ObservableCollection<StorageFile>();

        private void OnFileDragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Copy;

            if (e.DragUIOverride != null)
            {
                e.DragUIOverride.Caption = "Add file";
                e.DragUIOverride.IsContentVisible = true;
            }

            this.AddFilePanel.Visibility = Visibility.Visible;
        }

        private void OnFileDragLeave(object sender, DragEventArgs e)
        {
            this.AddFilePanel.Visibility = Visibility.Collapsed;
        }


        private async void OnFileDrop(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                var items = await e.DataView.GetStorageItemsAsync();
                if (items.Count > 0)
                {
                    foreach (var appFile in items.OfType<StorageFile>())
                    {
                        Files.Clear(); // Multi-files upload integration in the future; so we that logic
                        this.Files.Add(appFile);
                    }
                }
            }

            this.AddFilePanel.Visibility = Visibility.Collapsed;
        }

        public HttpClient MyHttpClient
        {
            get => (HttpClient)GetValue(MyHttpClientProperty);
            set => SetValue(MyHttpClientProperty, value);
        }

        public static readonly DependencyProperty MyHttpClientProperty =
            DependencyProperty.Register("MyHttpClient", typeof(HttpClient), typeof(PackageUploaderControl), null);

        private async void LoadFileBtn_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker()
            {
                ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };
            picker.FileTypeFilter.Add(".zip");
            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                Files.Clear();
                Files.Add(file);
            }
        }

        private void SendFilesBtn_Click(object sender, RoutedEventArgs e)
        {
            if(Files.Count > 0 && Version.Trim() != "")
            {
                MyHttpClient.SendInstallPackage(Version, Files[0]);
                Files.Clear();
            }
        }

        private string _version = "";

        public string Version
        {
            get => _version;
            set => SetField(ref _version, value);

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
