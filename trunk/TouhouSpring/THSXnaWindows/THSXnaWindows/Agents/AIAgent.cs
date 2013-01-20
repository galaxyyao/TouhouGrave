using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Agents
{
    class AIAgent : BaseAgent
    {
        public override void OnTacticalPhase(Interactions.TacticalPhase io)
        {
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
