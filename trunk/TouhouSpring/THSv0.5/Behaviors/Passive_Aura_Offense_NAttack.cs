using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_Aura_Offense_NAttack:
        BaseBehavior<Passive_Aura_Offense_NAttack.ModelType>,
        Commands.ICause,
        ILocalEpilogTrigger<Commands.ActivateAssist>,
        ILocalEpilogTrigger<Commands.DeactivateAssist>,
        IGlobalEpilogTrigger<Commands.IMoveCard>
    {
        private ValueModifier m_attackMod;

        public void RunLocalEpilog(Commands.ActivateAssist command)
        {
            foreach (var card in Host.Owner.CardsOnBattlefield)
            {
                if (card.Warrior != null)
                {
                    Game.QueueCommands(
                        new Commands.SendBehaviorMessage(card.Warrior, "AttackModifiers", new object[] { "add", m_attackMod }));
                }
            }
        }

        public void RunLocalEpilog(Commands.DeactivateAssist command)
        {
            foreach (var card in Host.Owner.CardsOnBattlefield)
            {
                if (card.Warrior != null)
                {
                    Game.QueueCommands(
                        new Commands.SendBehaviorMessage(card.Warrior, "AttackModifiers", new object[] { "remove", m_attackMod }));
                }
            }
        }

        public void RunGlobalEpilog(Commands.IMoveCard command)
        {
            if (Host.IsActivatedAssist
                && Host.Owner == command.Subject.Owner
                && Host != command.Subject
                && command.Subject.Warrior != null)
            {
                if (command.FromZoneType != ZoneType.OnBattlefield
                    && command.ToZoneType == ZoneType.OnBattlefield)
                {
                    Game.QueueCommands(
                        new Commands.SendBehaviorMessage(command.Subject.Warrior, "AttackModifiers", new object[] { "add", m_attackMod }));
                }
                else if (command.FromZoneType == ZoneType.OnBattlefield
                         && command.ToZoneType != ZoneType.OnBattlefield)
                {
                    Game.QueueCommands(
                        new Commands.SendBehaviorMessage(command.Subject.Warrior, "AttackModifiers", new object[] { "remove", m_attackMod }));
                }
            }
        }

        protected override void OnInitialize()
        {
            m_attackMod = new ValueModifier(ValueModifierOperator.Add, 1);
        }

        protected override void OnTransferFrom(IBehavior original)
        {
            m_attackMod = (original as Passive_Aura_Offense_NAttack).m_attackMod;
        }

        [BehaviorModel(typeof(Passive_Aura_Offense_NAttack), Category = "v0.5/Passive", DefaultName = "进攻光环")]
        public class ModelType : BehaviorModel
        {
        }
    }
}
