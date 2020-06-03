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

        public ProgressIndicator Indicator { get => _indicator; set => SetField(ref _indicator, value); }

        public ProgressIndicatorVM() { }
        public ProgressIndicatorVM(ProgressIndicator info)
        {
            Indicator = info;
        }

        public ProgressIndicatorVM(ProgressIndicatorArgs info)
        {
            Indicator = new ProgressIndicator()
            {
                Message = info.Message,
                Status = info.Status
            };
        }

    }
}
