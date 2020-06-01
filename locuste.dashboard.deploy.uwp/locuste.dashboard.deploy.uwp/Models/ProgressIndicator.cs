using Windows.Devices.Geolocation;

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
        public EventStatus Status;
        public string Message;
    }
}