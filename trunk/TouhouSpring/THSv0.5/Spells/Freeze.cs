using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Behaviors;

namespace TouhouSpring.THSv0_5.Spells
{
    public sealed class Freeze : BaseBehavior<Freeze.ModelType>,
        ILocalPrerequisiteTrigger<Commands.PlayCard>,
        ILocalEpilogTrigger<Commands.PlayCard>
    {
        private class Effect : Utilities.LastingEffect.EffectUponPhaseEnd<Effect.ModelType>,
            IUnattackable, IStatusEffect
        {
            public string IconUri { get { return "atlas:Textures/Icons/Icons0$Freeze"; } }
            public string Text { get { return "冰冻\n该角色无法进攻。"; } }

            [BehaviorModel(typeof(Effect), HideFromEditor = true)]
            public class ModelType : Utilities.LastingEffect.ModelType { }
        }

        public CommandResult RunLocalPrerequisite(Commands.PlayCard command)
        {
            if (!Game.Players.Where(player => player != Host.Owner)
                    .SelectMany(player => player.CardsOnBattlefield)
                    .Any())
            {
                return CommandResult.Cancel("No card can be affected.");
            }

            return CommandResult.Pass;
        }

        public void RunLocalEpilog(Commands.PlayCard command)
        {
            foreach (var card in Game.Players.Where(player => player != Host.Owner)
                .SelectMany(player => player.CardsOnBattlefield))
            {
                var effect = new Effect.ModelType { PhaseName = "Cleanup", LastingTurns = 2 }.CreateInitialized();
                Game.QueueCommands(new Commands.AddBehavior(card, effect));
            }
        }

        [BehaviorModel(typeof(Freeze), Category = "v0.5/Spell", DefaultName = "冰冻")]
        public class ModelType : BehaviorModel
        { }
    }
}
