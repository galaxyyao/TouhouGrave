using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Spell_DirectDamage_NDamage :
        BaseBehavior<Spell_DirectDamage_NDamage.ModelType>,
        Commands.ICause,
        IPrerequisiteTrigger<Commands.PlayCard>,
        IEpilogTrigger<Commands.PlayCard>
    {
        public CommandResult RunPrerequisite(Commands.PlayCard command)
        {
            if (command.CardToPlay == Host)
            {
                Game.NeedTarget(this,
                    Game.Players.Where(player => player != Host.Owner)
                    .SelectMany(player => player.CardsOnBattlefield)
                    .Where(card => card.Behaviors.Has<Warrior>()).ToArray().ToIndexable(),
                    "指定1张对手的卡，造成伤害");
            }

            return CommandResult.Pass;
        }

        public void RunEpilog(Commands.PlayCard command)
        {
            if (command.CardToPlay == Host)
            {
                Game.IssueCommands(new Commands.DealDamageToCard(Game.GetTarget(this)[0], Model.DamageToDeal, this));
            }
        }

        [BehaviorModel(typeof(Spell_DirectDamage_NDamage), Category = "v0.5/Spell", DefaultName = "直接伤害")]
        public class ModelType : BehaviorModel
        {
            public int DamageToDeal
            {
                get; set;
            }
        }
    }
}
