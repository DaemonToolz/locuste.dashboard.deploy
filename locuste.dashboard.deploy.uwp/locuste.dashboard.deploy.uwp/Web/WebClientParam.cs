using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using locuste.dashboard.deploy.uwp.Web.Http;
using locuste.dashboard.deploy.uwp.Web.SocketIO;

namespace locuste.dashboard.deploy.uwp.Web
{
    public class WebClientParam
    {
        public HttpClient Client;
        public SocketIoWrapper Wrapper;

    }
}
