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
using locuste.dashboard.deploy.uwp.Models;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;

namespace locuste.dashboard.deploy.uwp.Web.Http
{
    public class HttpClient
    {

        private RestClient _client;
        private string _uri;
        private string _httpURL;
        public HttpClient(string _turi)
        {
            _httpURL = $"http://{_uri = _turi}:30000/";
            _client = new RestClient(_httpURL);

            _client.UseNewtonsoftJson();
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

        public Task<ProjectVersion> GetInstalledVersion()
        {
            return _client.GetAsync<ProjectVersion>(new RestRequest($"version/current"));
        }


        public Task<ProjectVersion> Uninstall()
        {
            return _client.GetAsync<ProjectVersion>(new RestRequest("uninstall"));
        }

        public Task<HttpResponseMessage> SendCommand(ExecCommand cmd)
        {
            return $"{_httpURL}version/run".PostJsonAsync(cmd);
        }


        public async void SendInstallPackage(string versionNumber, StorageFile file) {
            try
            {
                IBuffer buffer = await Windows.Storage.FileIO.ReadBufferAsync(file);
                await $"{_httpURL}upload/{versionNumber}".PostMultipartAsync(mp => mp
                    .AddFile("file", buffer.AsStream(), file.Name, null, (int)buffer.Length)
                );

            }
            catch // Trouver un moeyen de remonter l'erreur'
            {

            }
        }
    }
}
