using SWEN_KOMM_Kim.API;
using SWEN_KOMM_Kim.Exceptions;
using SWEN_KOMM_Kim.HttpServer.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SWEN_KOMM_Kim.HttpServer
{
    internal class HttpServer
    {
        private readonly Router _router;
        private readonly TcpListener _listener;
        private bool _listening;

        public HttpServer(Router router, IPAddress address, int port)
        {
            _router = router;
            _listener = new TcpListener(address, port);
            _listening = false;
        }

        public void Start()
        {
            _listener.Start();
            _listening = true;

            while (_listening)
            {
                var client = _listener.AcceptTcpClient();
                var clientHandler = new HttpClientHandler(client);
                Task.Run(() => HandleClient(clientHandler));
            }
        }

        public void Stop()
        {
            _listening = false;
            _listener.Stop();
        }

        private void HandleClient(HttpClientHandler handler)
        {
            var request = handler.ReceiveRequest();
            HttpResponse response;

            if (request is null)
            {
                response = new HttpResponse(StatusCode.BadRequest);
            }
            else
            {
                try
                {
                    var command = _router.Resolve(request);
                    if (command is null)
                    {
                        response = new HttpResponse(StatusCode.BadRequest);
                    }
                    else
                    {
                        response = command.Execute();
                    }
                }
                catch (RouteNotAuthenticatedException)
                {
                    response = new HttpResponse(StatusCode.Unauthorized);
                }
            }

            handler.SendResponse(response);
        }
    }
}
