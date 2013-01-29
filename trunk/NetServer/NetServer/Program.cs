using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace TouhouSpring.NetServerCore
{
    class Program
    {
        private static Server _server = null;
        private static Log4NetHelper log4NetHelper = new Log4NetHelper();

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            while (true)
            {
                Console.WriteLine("Master, please let me know your next command.");
                Console.WriteLine("You could input question mark (?) for available command list.");
                Console.WriteLine(":");
                string command = Console.ReadLine();
                if (command == "exit")
                    break;
                Act(command);
            }
            _server.Shutdown();
            Console.WriteLine("End");
            Console.Read();
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine(e.ExceptionObject.ToString());
            log4NetHelper.WriteErrorLog(e.ExceptionObject.ToString());
            Console.WriteLine("Press Enter to continue");
            Console.ReadLine();
            Environment.Exit(1);
        }

        public static void Act(string command)
        {
            if (command == "?")
            {
                Console.WriteLine("start - Start the server");
                Console.WriteLine("listen - Listen to incoming messages");
                Console.WriteLine("exit - Temporarily say goodbye");
                Console.WriteLine("================================");
                Console.WriteLine();
            }

            if (_server == null || !_server.IsRunning)
            {
                if (command == "start")
                {
                    _server = new Server(13389);//temporaily set port to 13389
                    _server.Start();
                    _server.Listen();
                    return;  
                }
                else
                {
                    Console.WriteLine("Master, please start the server first!");
                    return;
                }
            }

            switch (command)
            {
                case "start":
                    if (_server.IsRunning)
                    {
                        Console.WriteLine("Master, the server is already running!");
                        return;
                    }
                    throw new Exception(string.Format("The server is not in right status. the current status is {0}", _server.Status));
                case "listen":
                    if(_server.IsRunning)
                    {
                        _server.Listen();
                        return;
                    }
                    throw new Exception(string.Format("The server is not in right status. the current status is {0}", _server.Status));
                default:
                    Console.WriteLine("Invalid command!");
                    break;
            }
        }
    }
}
