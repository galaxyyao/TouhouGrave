using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_Aura_Offense_NAttack:
        BaseBehavior<Passive_Aura_Offense_NAttack.ModelType>,
        Commands.ICause,
        IEpilogTrigger<Commands.ActivateAssist>,
        IEpilogTrigger<Commands.DeactivateAssist>,
        IEpilogTrigger<Commands.PlayCard>
    {
        private ValueModifier m_attackMod;

        public void RunEpilog(Commands.ActivateAssist command)
        {
            if (command.CardToActivate == Host)
            {
                foreach (var card in Host.Owner.CardsOnBattlefield)
                {
                    if (card.Behaviors.Get<Warrior>() == null)
                        continue;

                    var warrior = card.Behaviors.Get<Warrior>();
                    Game.IssueCommands(
                        new Commands.SendBehaviorMessage(warrior, "AttackModifiers", new object[] { "add", m_attackMod }));
                }
            }
        }

        public void RunEpilog(Commands.DeactivateAssist command)
        {
            if (command.CardToDeactivate == Host)
            {
                foreach (var card in Host.Owner.CardsOnBattlefield)
                {
                    if (card.Behaviors.Get<Warrior>() == null)
                        continue;

                    var warrior = card.Behaviors.Get<Warrior>();
                    Game.IssueCommands(
                        new Commands.SendBehaviorMessage(warrior, "AttackModifiers", new object[] { "remove", m_attackMod }));
                }
            }
        }

        public void RunEpilog(Commands.PlayCard command)
        {
            if (Host.IsActivatedAssist
                && Host.Owner == command.CardToPlay.Owner)
            {
                var warrior = command.CardToPlay.Behaviors.Get<Warrior>();
                if (warrior != null)
                {
                    Game.IssueCommands(
                        new Commands.SendBehaviorMessage(warrior, "AttackModifiers", new object[] { "add", m_attackMod }));
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

        [BehaviorModel(Category = "v0.5/Passive", DefaultName = "进攻光环")]
        public class ModelType : BehaviorModel<Passive_Aura_Offense_NAttack>
        {
        }
    }
}
