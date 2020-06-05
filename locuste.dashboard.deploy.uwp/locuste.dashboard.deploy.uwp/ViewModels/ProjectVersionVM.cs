using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using locuste.dashboard.deploy.uwp.Models;

namespace locuste.dashboard.deploy.uwp.ViewModels
{
    public class ProjectVersionVM : Bindable {
        private ProjectVersion _version;

        public ProjectVersion Version { get => _version; set => SetField(ref _version, value); }

        public ProjectVersionVM() { }
        public ProjectVersionVM(ProjectVersion version)
        {
            Version = version;
        }

    }
}
