using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using locuste.dashboard.deploy.uwp.Models;
using locuste.dashboard.deploy.uwp.Utils;
using SocketIOClient;

namespace locuste.dashboard.deploy.uwp.Web.SocketIO
{
    public class SocketIoWrapper
    {
        private string _uri;
        private SocketIOClient.SocketIO _client;

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
                OnProgressUpdateEvent(new ProgressIndicatorArgs(value));
            });

        }

        public async Task<bool> Connect()
        {
            try
            {
                await _client.ConnectAsync();
                
            }
            catch  // Error case => on connection failed (e.g. target unavailable)
            {
                return false;
            }
            return true;
        }

        public async void Disconnect()
        {
            try
            {
                await _client.DisconnectAsync();
               
            }
            catch
            { 
               // Find a way to display the error
            }
        }

        public event EventHandler<ProgressIndicatorArgs> ProgressUpdateHandler;
        public event EventHandler<FileCopyInfoArgs> FileCopyInfoHandler;

        protected virtual void OnProgressUpdateEvent(ProgressIndicatorArgs e) {
            var handler = ProgressUpdateHandler;
            handler?.Invoke(this, e);
        }

 
        protected virtual void OnFileCopyInfoHandlerEvent(FileCopyInfoArgs e) {
            var handler = FileCopyInfoHandler;
            handler?.Invoke(this, e);
        }


        private async void onConnected(object sender, EventArgs args )
        {
     
        }

        private  async void onDisconnected(object sender, string args)
        {
     
        }
  

    }
}