using System;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Core;
using Windows.UI.Notifications;
using locuste.dashboard.deploy.uwp.Models;
using locuste.dashboard.deploy.uwp.Utils;
using locuste.dashboard.deploy.uwp.ViewModels;
using SocketIOClient;

namespace locuste.dashboard.deploy.uwp.Web.SocketIO
{
    public class SocketIoWrapper
    {
        private string _uri;
        private SocketIOClient.SocketIO _client;
        public bool DisconnectedByUser = false;
        public SocketIoWrapper(string target)
        {
            _uri = target;
            _client = new SocketIOClient.SocketIO(new Uri($"ws://{_uri}:31000/"));
            _client.OnConnected += onConnected;
     
            _client.OnDisconnected += onDisconnected;
            _client.On("progress", data => {
                var value = data.GetValue<FileCopyInfo>();
                OnFileCopyInfoHandlerEvent(new FileCopyInfoArgs(value));
            });
            _client.On("install", data =>
            {
                var value = data.GetValue<ProgressIndicator>();
                ShowToast("Locuste Launcher",value.Message);
                OnProgressUpdateEvent(new ProgressIndicatorArgs(value));
            });
            _client.On("update", data =>
            {
                var value = data.GetValue<AppVersion>();
                ShowToast("Locuste Launcher", $"Mise à jour de {value.Name} (v{value.Version})");
                OnUpdateHandlerEvent(new AppVersionEventArgs(value));
            });
        }

        private void ShowToast(string title, string content)
        {
            ToastNotifier ToastNotifier = ToastNotificationManager.CreateToastNotifier();
            Windows.Data.Xml.Dom.XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
            Windows.Data.Xml.Dom.XmlNodeList toastNodeList = toastXml.GetElementsByTagName("text");
            toastNodeList.Item(0).AppendChild(toastXml.CreateTextNode(title));
            toastNodeList.Item(1).AppendChild(toastXml.CreateTextNode(content));
            Windows.Data.Xml.Dom.IXmlNode toastNode = toastXml.SelectSingleNode("/toast");
            Windows.Data.Xml.Dom.XmlElement audio = toastXml.CreateElement("audio");
            audio.SetAttribute("src", "ms-winsoundevent:Notification.SMS");

            ToastNotification toast = new ToastNotification(toastXml);
            toast.ExpirationTime = DateTime.Now.AddSeconds(4);
            ToastNotifier.Show(toast);
        }

        public async Task<bool> Connect()
        {
            try
            {
                DisconnectedByUser = false;
                await _client.ConnectAsync();
               
            }
            catch  // Error case => on connection failed (e.g. target unavailable)
            {
                var Status = Statuses.GetStatus(EventStatus.Error);
                Status.Description = "Impossible de se connecter au serveur";

                OnUpdateHandlerEvent(new ConnectionEventArgs()
                {
                    Status = Status
                });
            
                Task.Delay(5000).ContinueWith(t => Connect());

                return false;
            }
            return true;
        }

        public async void Disconnect()
        {
            try
            {
                DisconnectedByUser = true; 
                await _client.DisconnectAsync();
               
            }
            catch
            {
           
                // Find a way to display the error
            }
        }

        public event EventHandler<ProgressIndicatorArgs> ProgressUpdateHandler;
        public event EventHandler<FileCopyInfoArgs> FileCopyInfoHandler;
        public event EventHandler<AppVersionEventArgs> OnUpdateEvent;
        public event EventHandler<ConnectionEventArgs> OnConnectionEvent;

        protected virtual void OnUpdateHandlerEvent(ConnectionEventArgs e)
        {
            OnConnectionEvent?.Invoke(this, e);
        }

        protected virtual void OnProgressUpdateEvent(ProgressIndicatorArgs e) {
            ProgressUpdateHandler?.Invoke(this, e);
        }

 
        protected virtual void OnFileCopyInfoHandlerEvent(FileCopyInfoArgs e) {
            FileCopyInfoHandler?.Invoke(this, e);
        }

        protected virtual void OnUpdateHandlerEvent(AppVersionEventArgs e)
        {
            OnUpdateEvent?.Invoke(this, e);
        }


        private async void onConnected(object sender, EventArgs args )
        {
            var stat = Statuses.GetStatus(EventStatus.Success);
            stat.Description = "Connection réussie";
            OnUpdateHandlerEvent(new ConnectionEventArgs()
            {
                Status = stat
            });
        }

        private  async void onDisconnected(object sender, string args)
        {
            if(!DisconnectedByUser)
            {
                var stat = Statuses.GetStatus(EventStatus.Error);
                stat.Description = "Déconnecté du serveur, reconnection en cours";
                OnUpdateHandlerEvent(new ConnectionEventArgs()
                {
                    Status = stat
                });

                await Connect();
            }
        }
  

    }
}