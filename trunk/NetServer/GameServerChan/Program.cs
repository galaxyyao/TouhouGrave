using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Configuration;
using TouhouSpring.ServerCore;

namespace GameServerChan
{
    class Program
    {
        private static Server _server = null;
        private static Log4NetHelper log4NetHelper = new Log4NetHelper();

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
            Console.ReadKey();
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine(e.ExceptionObject.ToString());
            log4NetHelper.WriteErrorLog(e.ExceptionObject.ToString());
            Console.WriteLine("Press Enter to continue");
            Console.ReadLine();
            Environment.Exit(1);
        }
    }
}
