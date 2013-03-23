using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Network;

namespace TouhouSpring.Agents
{
    class RemoteAgent : BaseAgent
    {
        Network.Client m_NetworkClient = null;

        public RemoteAgent()
        {
            m_NetworkClient = GameApp.Service<Services.Network>().THSClient;
        }

        public override void OnTacticalPhase(Interactions.TacticalPhase io)
        {
            m_NetworkClient.RemoteEnterInteraction(io);
        }

        public override void OnSelectCards(Interactions.SelectCards io)
        {
            m_NetworkClient.RemoteEnterInteraction(io);
        }

        public override void OnSelectNumber(Interactions.SelectNumber io)
        {
            m_NetworkClient.RemoteEnterInteraction(io);
        }

        public override void OnMessageBox(Interactions.MessageBox io)
        {
            throw new NotImplementedException();
            //base.OnMessageBox(io);
        }
    }
}
