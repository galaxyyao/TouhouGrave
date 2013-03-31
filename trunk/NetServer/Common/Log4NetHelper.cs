using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;

namespace Common
{
    public class Log4NetHelper
    {
        private static Log4NetHelper m_instance;
        private bool m_isConfigured = false;
        private ILog m_logger;

        public static Log4NetHelper Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new Log4NetHelper();
                }
                return m_instance;
            }
        }

        private  Log4NetHelper()
        {
            EnsureConfigured();
        }

        private void EnsureConfigured()
        {
            if (!m_isConfigured)
            {
                log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo("log4net.cfg.xml"));
                m_logger = LogManager.GetLogger("THSLogger");
                m_isConfigured = true;
            }
        }

        public void WriteErrorLog(string info)
        {
            EnsureConfigured();
            m_logger.Error(info);
        }
    }
}
