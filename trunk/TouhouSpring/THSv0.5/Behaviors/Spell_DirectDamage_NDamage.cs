using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Spell_DirectDamage_NDamage :
        BaseBehavior<Spell_DirectDamage_NDamage.ModelType>,
        Commands.ICause,
        ILocalPrerequisiteTrigger<Commands.PlayCard>,
        ILocalEpilogTrigger<Commands.PlayCard>
    {
        public CommandResult RunLocalPrerequisite(Commands.PlayCard command)
        {
            Game.NeedTargets(this,
                Game.Players.Where(player => player != Host.Owner)
                .SelectMany(player => player.CardsOnBattlefield)
                .Where(card => card.Behaviors.Has<Warrior>()),
                "指定1张对手的卡，造成伤害");
            return CommandResult.Pass;
        }

        public void RunLocalEpilog(Commands.PlayCard command)
        {
            Game.QueueCommands(new Commands.DealDamageToCard(Game.GetTargets(this)[0], Model.DamageToDeal, this));
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
