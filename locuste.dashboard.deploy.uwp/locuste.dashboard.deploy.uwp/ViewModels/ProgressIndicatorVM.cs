using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using locuste.dashboard.deploy.uwp.Models;

namespace locuste.dashboard.deploy.uwp.ViewModels
{
    public class ProgressIndicatorVM : Bindable
    {

        private ProgressIndicator _indicator;
        private UIStatus _icon;
        public ProgressIndicator Indicator { get => _indicator; set => SetField(ref _indicator, value); }
        public UIStatus Icon{ get => _icon; set => SetField(ref _icon, value); }

        public ProgressIndicatorVM()
        {

            Indicator = new ProgressIndicator()
            {
                Status = EventStatus.Unknown,
                Message = "En attente d'instruction"
            };
            Icon = Statuses.GetStatus(Indicator.Status);
        }
        public ProgressIndicatorVM(ProgressIndicator info)
        {
            Indicator = info;
            Icon = Statuses.GetStatus(info.Status);
        }

        public ProgressIndicatorVM(ProgressIndicatorArgs info)
        {
            Indicator = new ProgressIndicator()
            {
                Message = info.Message,
                Status = info.Status
            };

            Icon = Statuses.GetStatus(info.Status);
        }

    }
}
