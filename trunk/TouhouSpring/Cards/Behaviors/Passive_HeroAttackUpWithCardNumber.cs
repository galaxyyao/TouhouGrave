using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class Passive_HeroAttackUpWithCardNumber
        : BaseBehavior<Passive_HeroAttackUpWithCardNumber.ModelType>,
        ITrigger<Triggers.CardLeftBattlefieldContext>,
        ITrigger<Triggers.PostCardPlayedContext>
    {
        private List<AttackModifier> attackMods = new List<AttackModifier>();

        public void Trigger(Triggers.PostCardPlayedContext context)
        {
            if (context.CardPlayed == Host)
            {
                int warriorNumber = 0;
                foreach (var card in context.Game.PlayerPlayer.CardsOnBattlefield)
                {
                    if (card.Behaviors.Get<Warrior>() != null)
                        warriorNumber++;
                }
                warriorNumber -= 1; //exclude Hero Card
                for (int i = 0; i < warriorNumber; i++)
                {
                    var attackMod = new AttackModifier(x => x + 1);
                    attackMods.Add(attackMod);
                    Host.Owner.Hero.Host.Behaviors.Add(attackMod);
                }
                return;
            }
            if (IsOnBattlefield && context.CardPlayed.Owner == Host.Owner)
            {
                var attackMod = new AttackModifier(x => x + 1);
                attackMods.Add(attackMod);
                Host.Owner.Hero.Host.Behaviors.Add(attackMod);
            }
        }

        public void Trigger(Triggers.CardLeftBattlefieldContext context)
        {
            if (context.CardToLeft != Host && IsOnBattlefield)
            {
                Host.Owner.Hero.Host.Behaviors.Remove(attackMods.LastOrDefault());
                return;
            }
            if (context.CardToLeft == Host)
            {
                foreach (var attackMod in attackMods)
                {
                    Host.Owner.Hero.Host.Behaviors.Remove(attackMod);
                }
                return;
            }
        }

        [BehaviorModel("未来永劫斩", typeof(Passive_HeroAttackUpWithCardNumber))]
        public class ModelType : BehaviorModel
        { }
    }
}
