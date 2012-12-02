using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Commands;

namespace TouhouSpring.Behaviors
{
    public class Passive_AllFriendWarriorAttackUpAndDefenseUp :
        BaseBehavior<Passive_AllFriendWarriorAttackUpAndDefenseUp.ModelType>,
        IEpilogTrigger<PlayCard>,
        ITrigger<Triggers.CardLeftBattlefieldContext>
    {
        private readonly Func<int, int> m_attackMod = x => x + 1;
        private readonly Func<int, int> m_defenseMod = y => y + 1;

        void IEpilogTrigger<PlayCard>.Run(CommandContext<PlayCard> context)
        {
            if (context.Command.CardToPlay == Host)
            {
                foreach (var card in context.Game.PlayerPlayer.CardsOnBattlefield)
                {
                    if (card.Behaviors.Get<Warrior>() == null)
                        continue;
                    if (card.Behaviors.Get<Hero>() != null)
                        continue;

                    throw new NotImplementedException();
                    // TODO: issue commands for the following:
                    //card.Behaviors.Get<Warrior>().Attack.AddModifierToTail(m_attackMod);
                    //card.Behaviors.Get<Warrior>().Defense.AddModifierToTail(m_defenseMod);
                }
                return;
            }
            if (context.Command.CardToPlay.Owner == Host.Owner
                && IsOnBattlefield
                && context.Command.CardToPlay.Behaviors.Get<Warrior>() != null)
            {
                throw new NotImplementedException();
                // TODO: issue commands for the following:
                //context.Command.CardToPlay.Behaviors.Get<Warrior>().Attack.AddModifierToTail(m_attackMod);
                //context.Command.CardToPlay.Behaviors.Get<Warrior>().Defense.AddModifierToTail(m_defenseMod);
            }
        }

        public void Trigger(Triggers.CardLeftBattlefieldContext context)
        {
            if (context.CardToLeft == Host)
            {
                foreach (var card in Host.Owner.CardsOnBattlefield)
                {
                    if (card.Behaviors.Get<Warrior>() != null)
                    {
                        card.Behaviors.Get<Warrior>().Attack.RemoveModifier(m_attackMod);
                        card.Behaviors.Get<Warrior>().Defense.RemoveModifier(m_defenseMod);
                    }
                }
                return;
            }
            if (context.CardToLeft.Owner == Host.Owner
                && IsOnBattlefield)
            {
                context.CardToLeft.Behaviors.Get<Warrior>().Attack.RemoveModifier(m_attackMod);
                context.CardToLeft.Behaviors.Get<Warrior>().Defense.RemoveModifier(m_defenseMod);
            }
        }


        [BehaviorModel(typeof(Passive_AllFriendWarriorAttackUpAndDefenseUp), DefaultName = "秘药")]
        public class ModelType : BehaviorModel
        { }
    }
}
