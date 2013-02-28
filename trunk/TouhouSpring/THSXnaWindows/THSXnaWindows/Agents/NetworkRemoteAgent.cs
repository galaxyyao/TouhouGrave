using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Agents
{
    class NetworkRemoteAgent : BaseAgent
    {
        Network.Client _client = null;

        public NetworkRemoteAgent(Network.Client client)
        {
            _client = client;
        }

        public override bool OnCardPlayCanceled(Interactions.NotifyCardEvent io)
        {
            return false;
        }

        public override void OnTacticalPhase(Interactions.TacticalPhase io)
        {
            _client.CurrentIo = io;
        }

        public override void OnSelectCards(Interactions.SelectCards io)
        {
            _client.CurrentIo = io;
        }

        public override void OnMessageBox(Interactions.MessageBox io)
        {
            throw new NotImplementedException();
            //base.OnMessageBox(io);
        }
    }
}
