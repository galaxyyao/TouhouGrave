using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class Equipment : BaseBehavior<Equipment.ModelType>,
        ITrigger<Triggers.PreCardPlayContext>, IPlayable,
        ITrigger<Triggers.CardLeftBattlefieldContext>
    {
        public BaseCard Holder
        {
            get; private set;
        }

        public void Trigger(Triggers.PreCardPlayContext context)
        {
            if (context.CardToPlay != Host)
            {
                return;
            }

            if (Host.Owner.Mana < Model.ManaCost)
            {
                context.Cancel = true;
                context.Reason = "Insufficient mana";
                return;
            }

            var warriors = Host.Owner.CardsOnBattlefield.Where(card => card.Behaviors.Has<Warrior>());

            if (!warriors.Any())
            {
                context.Cancel = true;
                context.Reason = "No card can be equipped with " + Host.Model.Name + ".";
                return;
            }

            var cardToBeEquipped = new Interactions.SelectCards(
                context.Game.PlayerController, // TODO: send to Host.Owner's controller
                warriors.ToArray().ToIndexable(),
                Interactions.SelectCards.SelectMode.Single,
                "Select one card to equip " + Host.Model.Name + ".").Run();

            if (cardToBeEquipped.Count == 0)
            {
                context.Cancel = true;
                context.Reason = "Equipping is canceled.";
                return;
            }

            Holder = cardToBeEquipped[0];
            cardToBeEquipped[0].Behaviors.Get<Warrior>().Equipments.Add(Host);
            context.Game.UpdateMana(Host.Owner, -Model.ManaCost);
        }

        public bool IsPlayable(Game game)
        {
            return Host.Owner.Mana >= Model.ManaCost && Host.Owner.CardsOnBattlefield.Any(card => card.Behaviors.Has<Warrior>());
        }

        public void Trigger(Triggers.CardLeftBattlefieldContext context)
        {
            if (context.Card == Host)
            {
                Holder = null;
            }
            else if (context.Card == Holder)
            {
                Holder.Behaviors.Get<Warrior>().Equipments.Remove(Host);
                context.Game.DestroyCard(Host);
            }
        }

        [BehaviorModel("Equipment", typeof(Equipment), Description = "The card must be equipped on another warrior.")]
        public class ModelType : BehaviorModel
        {
            public int ManaCost { get; set; }
        }
    }
}
