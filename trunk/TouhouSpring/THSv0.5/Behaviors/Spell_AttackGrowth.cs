using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Spell_AttackGrowth:
        BaseBehavior<Spell_AttackGrowth.ModelType>,
        Commands.ICause,
        IPrerequisiteTrigger<Commands.PlayCard>,
        IEpilogTrigger<Commands.PlayCard>
    {
        private ValueModifier m_attackMod;

        public CommandResult RunPrerequisite(Commands.PlayCard command)
        {
            if (command.Subject == Host)
            {
                Game.NeedTargets(this,
                    Host.Owner.CardsOnBattlefield.Where(card => card.Behaviors.Has<Warrior>()).ToArray().ToIndexable(),
                    "指定1张己方的卡，增加3点攻击力");
            }

            return CommandResult.Pass;
        }

        public void RunEpilog(Commands.PlayCard command)
        {
            if (command.Subject == Host)
            {
                Game.QueueCommands(new Commands.SendBehaviorMessage(
                    Game.GetTargets(this)[0].Behaviors.Get<Warrior>(),
                    "AttackModifiers",
                    new object[] { "add", m_attackMod }));
            }
        }

        protected override void OnInitialize()
        {
            m_attackMod = new ValueModifier(ValueModifierOperator.Add, 3);
        }

        protected override void OnTransferFrom(IBehavior original)
        {
            m_attackMod = (original as Spell_AttackGrowth).m_attackMod;
        }

        [BehaviorModel(typeof(Spell_AttackGrowth), Category = "v0.5/Spell", DefaultName = "变巨术")]
        public class ModelType : BehaviorModel
        {
        }
    }
}
