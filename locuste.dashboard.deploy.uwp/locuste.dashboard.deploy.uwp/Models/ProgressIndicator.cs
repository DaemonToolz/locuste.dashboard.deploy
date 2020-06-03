using System;
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
        [System.Runtime.Serialization.DataMember(Name = "status")]
        [JsonProperty("status")]
        public EventStatus Status;

        [System.Runtime.Serialization.DataMember(Name = "message")]
        [JsonProperty("message")]
        public string Message;
    }


    public class ProgressIndicatorArgs : EventArgs
    {

        public EventStatus Status;
        public string Message;
        public ProgressIndicatorArgs(ProgressIndicator original)
        {
            Status = original.Status;
            Message = original.Message;
        }
    }
}