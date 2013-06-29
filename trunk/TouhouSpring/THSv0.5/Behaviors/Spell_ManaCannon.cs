using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Spell_ManaCannon:
        BaseBehavior<Spell_ManaCannon.ModelType>,
        Commands.ICause,
        ILocalPrerequisiteTrigger<Commands.CastSpell>,
        ICastableSpell
    {
        public CommandResult RunLocalPrerequisite(Commands.CastSpell command)
        {
            Game.NeedMana(1);
            Game.NeedTargets(this,
                Game.Players.Where(player => player != Host.Owner)
                .SelectMany(player => player.CardsOnBattlefield)
                .Where(card => card.Behaviors.Has<Warrior>()),
                "指定1张对手的卡，造成伤害");
            return CommandResult.Pass;
        }

        public void RunSpell(Commands.CastSpell command)
        {
            Game.QueueCommands(new Commands.SubtractPlayerMana(Host.Owner, Host.Owner.Mana, this));
            Game.QueueCommands(new Commands.DealDamageToCard(Game.GetTargets(this)[0], Host.Owner.Mana + 1, this));
        }

        [BehaviorModel(typeof(Spell_ManaCannon), Category = "v0.5/Spell", DefaultName = "灵力炮")]
        public class ModelType : BehaviorModel
        {
        }
    }
}
