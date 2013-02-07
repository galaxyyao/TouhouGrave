using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Spell_ManaCannon:
        BaseBehavior<Spell_ManaCannon.ModelType>,
        Commands.ICause,
        IPrerequisiteTrigger<Commands.CastSpell>,
        ICastableSpell
    {
        public CommandResult RunPrerequisite(Commands.CastSpell command)
        {
            if (command.Spell == this)
            {
                Game.NeedRemainingMana(Host.Owner);
                Game.NeedTarget(this,
                    Game.Players.Where(player => player != Host.Owner)
                    .SelectMany(player => player.CardsOnBattlefield)
                    .Where(card => card.Behaviors.Has<Warrior>()).ToArray().ToIndexable(),
                    "指定1张对手的卡，造成伤害");
            }
            return CommandResult.Pass;
        }

        public void RunSpell(Commands.CastSpell command)
        {
            Game.IssueCommands(new Commands.DealDamageToCard(Game.GetTarget(this)[0], Game.GetRemainingMana(Host.Owner), this));
        }

        [BehaviorModel(Category = "v0.5/Spell", DefaultName = "灵力炮")]
        public class ModelType : BehaviorModel<Spell_ManaCannon>
        {
        }
    }
}
