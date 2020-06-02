using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

using locuste.dashboard.deploy.uwp.Frames;
// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace locuste.dashboard.deploy.uwp
{
    /// <summary>
    ///     Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            TopFrame.Navigate(typeof(ActionPage));
        }

        public async Task<bool> IsFilePresent(string fileName)
        {
            var item = await ApplicationData.Current.LocalFolder.TryGetItemAsync(fileName);
            return item != null;
        }

        private void AddDeviceBtn_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            TopFrame.Navigate(typeof(RegisterPage));
        }

        private void UpdaterMenuBtn_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            TopFrame.Navigate(typeof(ActionPage));
        }
    }
}