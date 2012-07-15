using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class Assault_SingleWarriorBoostForXRounds :
        BaseBehavior<Assault_SingleWarriorBoostForXRounds.ModelType>,
        ITrigger<Triggers.PreCardPlayContext>, 
        IPlayable
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

            if (!Host.Owner.CardsOnBattlefield.Any(c => c.Behaviors.Has<Warrior>()))
            {
                context.Cancel = true;
                context.Reason = "No card can be affected.";
                return;
            }

            var selectedCard = new Interactions.SelectCards(
                context.Game.OpponentController, // TODO: host's controller
                Host.Owner.CardsOnBattlefield.Where(c => c.Behaviors.Has<Warrior>()).ToArray().ToIndexable(),
                Interactions.SelectCards.SelectMode.Single,
                "Select a card to boost its attack and defense.").Run();

            if (selectedCard.Count == 0)
            {
                context.Cancel = true;
                context.Reason = "Boost is canceled.";
                return;
            }

            var lasting = new LastingEffect(Model.Duration);
            if (Model.AttackBoost > 0)
            {
                var attackMod = new AttackModifier(x => x + Model.AttackBoost);
                lasting.CleanUps.Add(attackMod);
                selectedCard[0].Behaviors.Add(attackMod);
            }
            if (Model.DefenseBoost > 0)
            {
                var defenseMod = new DefenseModifier(x => x + Model.DefenseBoost);
                lasting.CleanUps.Add(defenseMod);
                selectedCard[0].Behaviors.Add(defenseMod);
            }

            selectedCard[0].Behaviors.Add(lasting);
            context.Game.UpdateMana(Host.Owner, -Model.ManaCost);
        }

        public bool IsPlayable(Game game)
        {
            return Host.Owner.Mana >= Model.ManaCost && Host.Owner.CardsOnBattlefield.Any(c => c.Behaviors.Has<Warrior>());
        }

        [BehaviorModel(typeof(Assault_SingleWarriorBoostForXRounds), DefaultName = "鬼神")]
        public class ModelType : BehaviorModel
        {
            public int AttackBoost { get; set; }
            public int DefenseBoost { get; set; }
            public int Duration { get; set; }
            public int ManaCost { get; set; }
        }
    }
}
