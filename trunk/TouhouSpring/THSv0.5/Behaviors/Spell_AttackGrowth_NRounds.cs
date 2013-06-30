using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Spell_AttackGrowth_NRounds :
        BaseBehavior<Spell_AttackGrowth_NRounds.ModelType>,
        ILocalPrerequisiteTrigger<Commands.PlayCard>,
        ILocalEpilogTrigger<Commands.PlayCard>
    {
        public CommandResult RunLocalPrerequisite(Commands.PlayCard command)
        {
            Game.NeedTargets(this,
                Host.Owner.CardsOnBattlefield.Where(c => c.Behaviors.Has<Warrior>()),
                "Select a card to boost its attack.");
            return CommandResult.Pass;
        }

        public void RunLocalEpilog(Commands.PlayCard command)
        {
            if (Model.AttackBoost > 0 )
            {
                var lasting = new LastingEffect.ModelType { Duration = Model.Duration }.CreateInitialized();
                var enhanceMod = new Enhance.ModelType { AttackBoost = Model.AttackBoost }.CreateInitialized();
                (lasting as LastingEffect).CleanUps.Add(enhanceMod);
                Game.QueueCommands(
                    new Commands.AddBehavior(Game.GetTargets(this)[0], enhanceMod),
                    new Commands.AddBehavior(Game.GetTargets(this)[0], lasting));
            }
        }

        [BehaviorModel(typeof(Spell_AttackGrowth_NRounds), DefaultName = "短时间持续攻击增长", Category = "v0.5/Spell")]
        public class ModelType : BehaviorModel
        {
            public int AttackBoost { get; set; }
            public int Duration { get; set; }
        }
    }
}
