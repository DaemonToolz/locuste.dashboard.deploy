﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using locuste.dashboard.deploy.uwp.Web;
using locuste.dashboard.deploy.uwp.Web.Http;

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=234238

namespace locuste.dashboard.deploy.uwp.Frames
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class VersionUploaderPage : Page, INotifyPropertyChanged
    {
        public VersionUploaderPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;
        }

        private HttpClient Client;

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
                        this.Files.Add(appFile);
                    }
                }
            }

            this.AddFilePanel.Visibility = Visibility.Collapsed;
        }

        private async void LoadFileBtn_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker()
            {
                ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };
            picker.FileTypeFilter.Add(".zip");
            var files = await picker.PickMultipleFilesAsync();
            if (files == null) return;
            foreach (var file in files)
            {
                Files.Add(file);
            }
          
        }

        private void SendFilesBtn_Click(object sender, RoutedEventArgs e)
        {
    
            if (Files.Count > 0 && Version.Trim() != "")
            {
                IsLoading = true;
                Task.Run(() =>
                {
                    foreach (var file in Files)
                    {
                        Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { FileUploadTextbox.Text = file.Name; });
                        Client.SendInstallPackage(Version, file);
                    }

                    Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        Files.Clear(); 
                        IsLoading = false; });
                });
            
               
            }
          
        }

        private string _version = "";

        public string Version
        {
            get => _version;
            set => SetField(ref _version, value);
        }

        private bool _isLoading = false;

        public bool IsLoading
        {
            get => _isLoading;
            set => SetField(ref _isLoading, value);
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
            Client = (e.Parameter as WebClientParam)?.Client;
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            Files.Clear();
        }
    }
}
