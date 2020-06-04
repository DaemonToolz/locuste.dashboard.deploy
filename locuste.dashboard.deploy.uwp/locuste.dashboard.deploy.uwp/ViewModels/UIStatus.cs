using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;
using locuste.dashboard.deploy.uwp.Models;

namespace locuste.dashboard.deploy.uwp.ViewModels
{
    public class UIStatus
    {
        public string Content;
        public Brush Foreground;

        public UIStatus() { }

        public UIStatus(string content, Brush brush)
        {
            Content = content;
            Foreground = brush;
        }
    }


    public static class Statuses
    {
        private static readonly Dictionary<EventStatus, UIStatus> StatusDict;

        public static UIStatus GetStatus(EventStatus stat) => StatusDict[stat];

        static Statuses()
        { 
            StatusDict = new Dictionary<EventStatus, UIStatus>()
            {
                {  EventStatus.Unknown , new UIStatus("\xE946", new SolidColorBrush(Colors.RoyalBlue))},
                {  EventStatus.Success , new UIStatus("\xE73D", new SolidColorBrush(Colors.Green))},
                {  EventStatus.Error , new UIStatus("\xE783", new SolidColorBrush(Colors.Red))},
                {  EventStatus.InProgress , new UIStatus("\xEC20", new SolidColorBrush(Colors.RoyalBlue))},
            };
        }
    }

}
