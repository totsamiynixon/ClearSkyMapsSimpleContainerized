using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Web;

namespace Web.Areas.Admin.Emulation
{
    public class WebServer
    {
        private readonly HttpListener _listener = new HttpListener();
        private readonly Func<HttpListenerRequest, string> _responderMethod;

        public delegate void RequestEventHandler(HttpListenerContext context);
        public event RequestEventHandler OnRequest;

        public WebServer(string baseUrl, IReadOnlyCollection<string> routes)
        {
            if (!HttpListener.IsSupported)
            {
                throw new NotSupportedException("Needs Windows XP SP2, Server 2003 or later.");
            }
            foreach (var route in routes)
            {
                _listener.Prefixes.Add(baseUrl + "/" + route);
            }
        }

        public bool IsOn()
        {
            return _listener.IsListening;
        }

        public void Run()
        {
            _listener.Start();
            ThreadPool.QueueUserWorkItem(o =>
            {
                Console.WriteLine("Webserver running...");
                try
                {
                    while (_listener.IsListening)
                    {
                        ThreadPool.QueueUserWorkItem(c =>
                        {
                            var ctx = c as HttpListenerContext;
                            try
                            {
                                if (ctx == null)
                                {
                                    return;
                                }

                                OnRequest(ctx);
                            }
                            catch
                            {
                                // ignored
                            }
                            finally
                            {
                                // always close the stream
                                if (ctx != null)
                                {
                                    ctx.Response.OutputStream.Close();
                                }
                            }
                        }, _listener.GetContext());
                    }
                }
                catch (Exception ex)
                {
                    // ignored
                }
            });
        }

        public void Stop()
        {
            _listener.Stop();
            _listener.Close();
        }

        public static T GetRequestBody<T>(HttpListenerRequest request)
        {
            string text;
            using (var reader = new StreamReader(request.InputStream,
                                       request.ContentEncoding))
            {
                text = reader.ReadToEnd();
            }
            return JsonConvert.DeserializeObject<T>(text);
        }

        public static void SetReponse(HttpListenerContext ctx, HttpResponseMessage message)
        {
            var buf = Encoding.UTF8.GetBytes(message.ToString());
            ctx.Response.ContentLength64 = buf.Length;
            ctx.Response.OutputStream.Write(buf, 0, buf.Length);
            ctx.Response.OutputStream.Close();
        }
    }
}