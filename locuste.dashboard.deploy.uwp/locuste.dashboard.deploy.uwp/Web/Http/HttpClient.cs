using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Flurl.Http;
using RestSharp;

namespace locuste.dashboard.deploy.uwp.Web.Http
{
    public class HttpClient
    {

        private RestClient _client;
        private string _uri;
        public HttpClient(string _turi) {
            _client = new RestClient($"http://{_uri = _turi}:30000/");
        }

        public Task<List<string>> GetVersions()
        {
            return _client.GetAsync<List<string>>(new RestRequest("versions"));
        }

        public void StartInstallationProcedure(string version)
        {
            _client.GetAsync<dynamic>(new RestRequest($"install/{version}"));
        }

        public void DeleteVersionNumber(string version)
        {
            _client.Delete<dynamic>(new RestRequest($"delete/{version}"));
        }


        public async void SendInstallPackage(string versionNumber, StorageFile file) {
            try
            {
                IBuffer buffer = await Windows.Storage.FileIO.ReadBufferAsync(file);
                await $"http://{_uri}:30000/upload/{versionNumber}".PostMultipartAsync(mp => mp
                    .AddFile("file", buffer.AsStream(), file.Name, null, (int)buffer.Length)
                );

            }
            catch
            {

            }
        }
    }
}
