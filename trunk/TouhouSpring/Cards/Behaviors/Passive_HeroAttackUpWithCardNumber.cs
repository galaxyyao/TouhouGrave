using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Commands;

namespace TouhouSpring.Behaviors
{
    public class Passive_HeroAttackUpWithCardNumber
        : BaseBehavior<Passive_HeroAttackUpWithCardNumber.ModelType>,
        ITrigger<Triggers.CardLeftBattlefieldContext>,
        IEpilogTrigger<PlayCard>
    {
        private List<AttackModifier> attackMods = new List<AttackModifier>();

        void IEpilogTrigger<PlayCard>.Run(CommandContext<PlayCard> context)
        {
            if (context.Command.CardToPlay == Host)
            {
                int warriorNumber = context.Game.PlayerPlayer.CardsOnBattlefield.Count(card => card.Behaviors.Has<Warrior>());
                warriorNumber -= 1; //exclude Hero Card
                for (int i = 0; i < warriorNumber; i++)
                {
                    var attackMod = new AttackModifier(x => x + 1);
                    attackMods.Add(attackMod);

                    throw new NotImplementedException();
                    // TODO: issue command for the following:
                    //Host.Owner.Hero.Host.Behaviors.Add(attackMod);
                }
                return;
            }
            if (IsOnBattlefield && context.Command.CardToPlay.Owner == Host.Owner)
            {
                var attackMod = new AttackModifier(x => x + 1);
                attackMods.Add(attackMod);

                throw new NotImplementedException();
                // TODO: issue command for the following:
                //Host.Owner.Hero.Host.Behaviors.Add(attackMod);
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

        [BehaviorModel(typeof(Passive_HeroAttackUpWithCardNumber), DefaultName = "未来永劫斩")]
        public class ModelType : BehaviorModel
        { }
    }
}
