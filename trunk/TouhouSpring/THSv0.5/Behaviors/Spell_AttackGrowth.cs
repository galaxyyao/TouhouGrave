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
        private readonly Warrior.ValueModifier m_attackMod = new Warrior.ValueModifier(Warrior.ValueModifierOperator.Add, 3);

        public CommandResult RunPrerequisite(Commands.PlayCard command)
        {
            if (command.CardToPlay == Host)
            {
                Game.NeedTarget(this,
                    Host.Owner.CardsOnBattlefield.Where(card => card.Behaviors.Has<Warrior>()).ToArray().ToIndexable(),
                    "指定1张己方的卡，增加3点攻击力");
            }

            return CommandResult.Pass;
        }

        public void RunEpilog(Commands.PlayCard command)
        {
            if (command.CardToPlay == Host)
            {
                Game.IssueCommands(new Commands.SendBehaviorMessage(
                    Game.GetTarget(this)[0].Behaviors.Get<Warrior>(),
                    "AttackModifiers",
                    new object[] { "add", m_attackMod }));
            }
        }

        [BehaviorModel(typeof(Spell_AttackGrowth), DefaultName = "变巨术")]
        public class ModelType : BehaviorModel
        {
        }
    }
}
