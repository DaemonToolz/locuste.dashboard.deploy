using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using H.Socket.IO;
using H.Socket.IO.EventsArgs;
using H.WebSockets.Args;
using locuste.dashboard.deploy.uwp.Models;

namespace locuste.dashboard.deploy.uwp.Web.SocketIO
{
    public class SocketIoWrapper : IDisposable
    {
        private string _uri;
        private SocketIoClient _client;

        public SocketIoWrapper(string target)
        {
            _uri = target;
            _client = new SocketIoClient();
            _client.Connected += onConnected;
            _client.Disconnected += onDisconnected;
            _client.EventReceived += (sender, args) =>
            {
                Debug.WriteLine(
                        $"EventReceived: Namespace: {args.Namespace}, Value: {args.Value}, IsHandled: {args.IsHandled}");
            };
            _client.HandledEventReceived += (sender, args) =>
            {
                Debug.WriteLine($"HandledEventReceived: Namespace: {args.Namespace}, Value: {args.Value}");
            };
            _client.UnhandledEventReceived += (sender, args) =>
            {
                Debug.WriteLine($"UnhandledEventReceived: Namespace: {args.Namespace}, Value: {args.Value}");
            };
            _client.ErrorReceived += (sender, args) =>
            {
                Debug.WriteLine($"ErrorReceived: Namespace: {args.Namespace}, Value: {args.Value}");
            };
            _client.ExceptionOccurred += (sender, args) =>
            {
                Debug.WriteLine($"ExceptionOccurred: {args.Value}");
            };

            _client.On<FileCopyInfo>("progress", data =>
            {
                Debug.WriteLine(data);
            }, "/");

            _client.On<ProgressIndicator>("install", data =>
            {
                Debug.WriteLine(data);
            }, "/");


        }

        public async Task<bool> Connect()
        {
            try
            {
                await _client.ConnectAsync(new Uri($"ws://{_uri}:31000/socket.io/?EIO=4&transport=websocket"));
            }
            catch
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

        private void onConnected(object sender, SocketIoEventEventArgs args )
        {
            
        }

        private void onDisconnected(object sender, WebSocketCloseEventArgs args)
        {
          
        }
        private void ReleaseUnmanagedResources()
        {
            _client.Dispose();
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
        }
    }
}