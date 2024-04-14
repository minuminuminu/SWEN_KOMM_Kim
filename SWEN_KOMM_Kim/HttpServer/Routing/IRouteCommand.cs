using SWEN_KOMM_Kim.HttpServer.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWEN_KOMM_Kim.HttpServer.Routing
{
    internal interface IRouteCommand
    {
        HttpResponse Execute();
    }
}
