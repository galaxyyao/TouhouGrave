using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;

namespace GameServerChan
{
    public class Log4NetHelper
    {
        private static bool _isConfigured = false;
        private static ILog _logger;

        public Log4NetHelper()
        {
            EnsureConfigured();
        }

        private void EnsureConfigured()
        {
            if (!_isConfigured)
            {
                log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo("log4net.cfg.xml"));
                _logger = LogManager.GetLogger("THSLogger");
                _isConfigured = true;
            }
        }

        public void WriteErrorLog(string info)
        {
            EnsureConfigured();
            _logger.Error(info);
        }
    }
}
