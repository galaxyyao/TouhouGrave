using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class Ripples : BaseBehavior<Ripples.ModelType>,
        ITrigger<Triggers.PreCardPlayContext>, IPlayable
    {
        public void Trigger(Triggers.PreCardPlayContext context)
        {
            if (context.CardToPlay != Host)
            {
                return;
            }

            if (Host.Owner.Mana < Model.ManaCost)
            {
                context.Cancel = true;
                context.Reason = "Insufficient mana.";
                return;
            }
            else if (Host.Owner.CardsOnHand.Count < 2)
            {
                context.Cancel = true;
                context.Reason = "No card can be sacrificed.";
                return;
            }
            else if (context.Game.OpponentPlayer.CardsOnBattlefield.Count == 0)
            {
                context.Cancel = true;
                context.Reason = "No card can be affected.";
                return;
            }

            var sacrifice = new Interactions.SelectCards(context.Game.PlayerController,
                Host.Owner.CardsOnHand.Where(card => card != Host).ToArray().ToIndexable(),
                Interactions.SelectCards.SelectMode.Single,
                "Select a card as the sacrifice.").Run();
            if (sacrifice.Count == 0)
            {
                context.Cancel = true;
                context.Reason = "Spell canceled.";
                return;
            }

            var target = new Interactions.SelectCards(context.Game.PlayerController,
                context.Game.OpponentPlayer.CardsOnBattlefield,
                Interactions.SelectCards.SelectMode.Single,
                "Select a card to destroy instantly.").Run();
            if (target.Count == 0)
            {
                context.Cancel = true;
                context.Reason = "Spell canceld.";
                return;
            }

            context.Game.DestroyCard(sacrifice[0]);
            context.Game.DestroyCard(target[0]);
            context.Game.UpdateMana(Host.Owner, -Model.ManaCost);
        }

        public bool IsPlayable(Game game)
        {
            return Host.Owner.Mana >= Model.ManaCost && Host.Owner.CardsOnHand.Count >= 2 && game.OpponentPlayer.CardsOnBattlefield.Count > 0;
        }

        [BehaviorModel(typeof(Ripples))]
        public class ModelType : BehaviorModel
        {
            public int ManaCost { get; set; }
        }
    }
}
