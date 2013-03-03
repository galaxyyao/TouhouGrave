﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Network;

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
            Client.RemoteCommand remoteCommand = _client.RemoteCommandDequeue();
            if (remoteCommand == null)
                return;
            if (remoteCommand.RemoteAction != Client.RemoteCommand.RemoteActionEnum.SelectCards)
                throw new ArgumentException("Remote Action is wrong");
            List<CardInstance> selectedCards = new List<CardInstance>();
            foreach (int i in remoteCommand.ResultParameters)
            {
                selectedCards.Add(io.Candidates[i]);
            }
            io.Respond(selectedCards.ToIndexable());
            _client.CurrentIo = null;
        }

        public override void OnMessageBox(Interactions.MessageBox io)
        {
            throw new NotImplementedException();
            //base.OnMessageBox(io);
        }
    }
}
