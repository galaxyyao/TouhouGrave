using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class PhaseSpecific : BaseBehavior<PhaseSpecific.ModelType>,
        ITrigger<Triggers.PreCardPlayContext>, IPlayable
    {
        public void Trigger(Triggers.PreCardPlayContext context)
        {
            if (context.CardToPlay == Host && !IsPlayable(context.Game))
            {
                context.Cancel = true;
                context.Reason = String.Format("{0} can't be played in {1} phase.", Host.Model.Name, context.Game.CurrentPhase);
            }
        }

        public bool IsPlayable(Game game)
        {
            return Model.TacticalPhase && game.CurrentPhase == "Tactical" && game.PlayerPlayer == Host.Owner
                   || Model.BlockPhase && game.CurrentPhase == "Combat/Block" && game.OpponentPlayer == Host.Owner;
        }

        [BehaviorModel(typeof(PhaseSpecific))]
        public class ModelType : BehaviorModel
        {
            public bool TacticalPhase { get; set; }
            public bool BlockPhase { get; set; }

            public ModelType()
            {
                TacticalPhase = true;
            }
        }
    }
}
