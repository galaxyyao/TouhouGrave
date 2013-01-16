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
        ISetupTrigger<Commands.PlayCard>,
        IEpilogTrigger<Commands.PlayCard>
    {
        private readonly Warrior.ValueModifier m_attackMod = new Warrior.ValueModifier(Warrior.ValueModifierOperator.Add, 3);
        private BaseCard castTarget = null;

        CommandResult IPrerequisiteTrigger<Commands.PlayCard>.Run(Commands.PlayCard command)
        {
            if (!Host.Owner.CardsOnBattlefield.Where(card => card.Behaviors.Has<Warrior>()).Any())
            {
                return CommandResult.Cancel("没有可以释放的对象");
            }

            return CommandResult.Pass;
        }

        CommandResult ISetupTrigger<Commands.PlayCard>.Run(Commands.PlayCard command)
        {
            var selectedCard = new Interactions.SelectCards(
                Host.Owner,
                Host.Owner.CardsOnBattlefield.Where(card => card.Behaviors.Has<Warrior>()).ToArray().ToIndexable(),
                Interactions.SelectCards.SelectMode.Single,
                "指定1张己方的卡，增加3点攻击力"
                ).Run();

            if (selectedCard.Count == 0)
            {
                return CommandResult.Cancel("取消施放");
            }

            castTarget = selectedCard[0];
            

            return CommandResult.Pass;
        }

        void IEpilogTrigger<Commands.PlayCard>.Run(Commands.PlayCard command)
        {
            if (command.CardToPlay == Host
                && castTarget != null)
            {
                Game.IssueCommands(new Commands.SendBehaviorMessage(castTarget.Behaviors.Get<Warrior>(), "AttackModifiers", new object[] { "add", m_attackMod }));
                castTarget = null;
            }
        }

        [BehaviorModel(typeof(Spell_AttackGrowth), DefaultName = "变巨术")]
        public class ModelType : BehaviorModel
        {
        }
    }
}
