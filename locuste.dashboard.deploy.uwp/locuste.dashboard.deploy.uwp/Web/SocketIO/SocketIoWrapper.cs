using System;
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
            _client.On<FileCopyInfo>("progress", data => { });

            _client.On<ProgressIndicator>("install", data => { });


        }

        public async void Connect()
        {
            await _client.ConnectAsync(new Uri(_uri));
        }

        public async void Disconnect()
        {
            await _client.DisconnectAsync();
        }

        private void onConnected(object sender, SocketIoEventEventArgs args )
        {
            Console.WriteLine($"Connected: {args.Namespace}");
        }

        private void onDisconnected(object sender, WebSocketCloseEventArgs args)
        {
            Console.WriteLine($"Disconnected. Reason: {args.Reason}, Status: {args.Status:G}");
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