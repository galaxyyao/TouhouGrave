using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class Passive_AllFriendWarriorAttackUpAndDefenseUp :
        BaseBehavior<Passive_AllFriendWarriorAttackUpAndDefenseUp.ModelType>,
        ITrigger<Triggers.PostCardPlayedContext>,
        ITrigger<Triggers.CardLeftBattlefieldContext>
    {
        private readonly Func<int, int> m_attackMod = x => x + 1;
        private readonly Func<int, int> m_defenseMod = y => y + 1;

        public void Trigger(Triggers.PostCardPlayedContext context)
        {
            if (context.CardPlayed == Host)
            {
                foreach (var card in context.Game.PlayerPlayer.CardsOnBattlefield)
                {
                    if (card.Behaviors.Get<Warrior>() == null)
                        continue;
                    if (card.Behaviors.Get<Hero>() != null)
                        continue;
                    card.Behaviors.Get<Warrior>().Attack.AddModifierToTail(m_attackMod);
                    card.Behaviors.Get<Warrior>().Defense.AddModifierToTail(m_defenseMod);
                }
                return;
            }
            if (context.CardPlayed.Owner == Host.Owner
                && IsOnBattlefield
                && context.CardPlayed.Behaviors.Get<Warrior>() != null)
            {
                context.CardPlayed.Behaviors.Get<Warrior>().Attack.AddModifierToTail(m_attackMod);
                context.CardPlayed.Behaviors.Get<Warrior>().Defense.AddModifierToTail(m_defenseMod);
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
