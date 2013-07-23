using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Spell_AssignAbility : BaseBehavior<Spell_AssignAbility.ModelType>,
        ILocalPrerequisiteTrigger<Commands.PlayCard>,
        ILocalEpilogTrigger<Commands.PlayCard>
    {
        CommandResult ILocalPrerequisiteTrigger<Commands.PlayCard>.RunLocalPrerequisite(Commands.PlayCard command)
        {
            Game.NeedTargets(
                this,
                Host.Owner.CardsOnBattlefield.Where(card => card.Warrior != null),
                "Select a card to whom the ability is assigned.");
            return CommandResult.Pass;
        }

        void ILocalEpilogTrigger<Commands.PlayCard>.RunLocalEpilog(Commands.PlayCard command)
        {
            Game.QueueCommands(new Commands.AddBehavior(Game.GetTargets(this)[0], Model.AssignedBehaviorModel.CreateInitialized()));
        }

        [BehaviorModel(typeof(Spell_AssignAbility), Category = "v0.5/Spell", DefaultName = "赋予技能")]
        public class ModelType : BehaviorModel
        {
            public IBehaviorModel AssignedBehaviorModel { get; set; }
        }
    }
}
