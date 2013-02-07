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
            //if (!String.IsNullOrEmpty(io.Message))
            //{
                
            //    m_currentIo = io;

            //    //on get message, io.Respond()
            //    //_client.callback += ()=>
            //    //{
            //    //    io.Respond();
            //    //}
            //    return true;
            //}

            return false;
        }

        public override void OnTacticalPhase(Interactions.TacticalPhase io)
        {
            _client.CurrentIo = io;
            //// sacrifice
            //var sacrifice = Sacrifice_MakeChoice2(io);
            //if (sacrifice != null)
            //{
            //    io.RespondSacrifice(sacrifice);
            //    return;
            //}

            //var playcard = PlayOrActivateCard_MakeChoice(io);
            //if (io.PlayCardCandidates.Contains(playcard))
            //{
            //    // play
            //    io.RespondPlay(playcard);
            //    return;
            //}
            //else if (io.ActivateAssistCandidates.Contains(playcard))
            //{
            //    // activate
            //    io.RespondActivate(playcard);
            //    return;
            //}
        }

        public override void OnSelectCards(Interactions.SelectCards io)
        {
        }

        public override void OnMessageBox(Interactions.MessageBox io)
        {
            throw new NotImplementedException();
            //base.OnMessageBox(io);
        }
    }
}
