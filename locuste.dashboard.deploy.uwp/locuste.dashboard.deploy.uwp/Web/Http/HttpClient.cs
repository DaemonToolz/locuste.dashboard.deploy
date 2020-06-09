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
            try
            {
                return _client.GetAsync<List<string>>(new RestRequest("versions"));
            }
            catch
            {
                return null;
            }
        }

        public void StartInstallationProcedure(string version)
        {
            try
            {
                _client.GetAsync<dynamic>(new RestRequest($"install/{version}"));
            }
            catch 
            {
                // TODO Trouver un moyen de remonter l'erreur
            }
        }

        public void DeleteVersionNumber(string version)
        {
            try
            {
                _client.Delete<dynamic>(new RestRequest($"delete/{version}"));
            }
            catch
            {
                // TODO Trouver un moyen de remonter l'erreur
            }
        }

        public Task<ProjectVersion> GetInstalledVersion()
        {
            try
            {
                return _client.GetAsync<ProjectVersion>(new RestRequest($"version/current"));
            }
            catch
            {
                return null;
            }
        }


        public Task<ProjectVersion> Uninstall()
        {
            try
            {
                return _client.GetAsync<ProjectVersion>(new RestRequest("uninstall"));
            }
            catch
            {
                return null;
            }
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
            catch 
            {
                // TODO Trouver un moyen de remonter l'erreur
            }
        }
    }
}
