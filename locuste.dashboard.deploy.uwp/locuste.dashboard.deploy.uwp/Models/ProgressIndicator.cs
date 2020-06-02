using Windows.Devices.Geolocation;
using Newtonsoft.Json;

namespace locuste.dashboard.deploy.uwp.Models
{
    public enum EventStatus
    {
        Success = 0,
        InProgress,
        Error
    }

    public class ProgressIndicator
    {

        [JsonProperty("status")]
        public EventStatus Status;

        [JsonProperty("message")]
        public string Message;
    }
}