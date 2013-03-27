using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace TouhouSpring.Services
{
    public class CurrentProfile : GameService
    {
        public override void Startup()
        {
            AppSettings.Instance.ReadProfile();
        }
    }
}
