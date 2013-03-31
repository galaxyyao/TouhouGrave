using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Configuration;
using TouhouSpring.ServerCore;
using Common;

namespace GameServerChan
{
    class Program
    {
        private static Server _server = null;

        static void Main(string[] args)
        {
            //AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            int port = Convert.ToInt32(ConfigurationManager.AppSettings["port"]);
            _server = new Server(port);//temporaily set port to 13389
            _server.Start();

            Console.WriteLine("Server started.");

            while (Console.KeyAvailable == false || Console.ReadKey().Key != ConsoleKey.Escape)
            {
                _server.Listen();
            }
            _server.Shutdown();
            Console.ReadKey();
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine(e.ExceptionObject.ToString());
            Log4NetHelper.Instance.WriteErrorLog(e.ExceptionObject.ToString());
            Console.WriteLine("Press Enter to continue");
            Console.ReadLine();
            Environment.Exit(1);
        }
    }
}
