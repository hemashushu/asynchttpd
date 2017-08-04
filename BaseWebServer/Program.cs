using System;
using System.Collections.Generic;
using System.Text;

using Doms.BaseWebServer.SimpleHttpHandler;
using Doms.HttpService;

namespace Doms.BaseWebServer
{
    class Program
    {
        static void Main(string[] args)
        {
            SimpleHandler handler = new SimpleHandler();
            HttpServiceControler server = new HttpServiceControler();
            server.AddHandler(handler);
            server.Start();

            Console.WriteLine("Server has start.");
            Console.WriteLine("Try to type http://localhost:8080/ in web broswer.");
            Console.ReadLine();

            server.Stop();
        }
    }
}
