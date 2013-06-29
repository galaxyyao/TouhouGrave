using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Assault_SingleWarriorBoostForXRounds :
        BaseBehavior<Assault_SingleWarriorBoostForXRounds.ModelType>,
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

        [BehaviorModel(typeof(Assault_SingleWarriorBoostForXRounds), DefaultName = "鬼神")]
        public class ModelType : BehaviorModel
        {
            public int AttackBoost { get; set; }
            public int Duration { get; set; }
        }
    }
}
