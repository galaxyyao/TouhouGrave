using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Agents
{
    partial class AIAgent : BaseAgent
    {
        private struct CardScorePair
        {
            public BaseCard Card;
            public double Score;
        }

        public override void OnTacticalPhase(Interactions.TacticalPhase io)
        {
            // sacrifice
            var sacrifice = Sacrifice_MakeChoice2(io);
            if (sacrifice != null)
            {
                io.RespondSacrifice(sacrifice);
                return;
            }

            var playcard = PlayOrActivateCard_MakeChoice(io);
            if (io.PlayCardCandidates.Contains(playcard))
            {
                // play
                io.RespondPlay(playcard);
                return;
            }
            else if (io.ActivateAssistCandidates.Contains(playcard))
            {
                // activate
                io.RespondActivate(playcard);
                return;
            }

            io.RespondPass();
        }

        public override void OnSelectCards(Interactions.SelectCards io)
        {
            io.Respond(io.SelectFromSet.Count != 0
                       ? new BaseCard[1] { io.SelectFromSet[0] }.ToIndexable()
                       : Indexable.Empty<BaseCard>());
        }

        public override void OnMessageBox(Interactions.MessageBox io)
        {
            throw new NotImplementedException();
            //base.OnMessageBox(io);
        }
    }
}
