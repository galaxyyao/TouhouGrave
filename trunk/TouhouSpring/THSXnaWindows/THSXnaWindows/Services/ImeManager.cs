using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Services
{
    class ImeManager : GameService
    {
        private Ime.ImeContext m_context;

        public override void Startup()
        {
            m_context = new Ime.ImeContext(GameApp.Instance.Window.Handle);
        }

        public override void Shutdown()
        {
            m_context.Dispose();
        }

        public void BeginIme()
        {
            m_context.BeginIme();
        }

        public void EndIme()
        {
            m_context.EndIme();
        }
    }
}
