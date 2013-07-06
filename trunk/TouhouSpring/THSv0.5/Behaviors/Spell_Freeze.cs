using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Spell_Freeze : BaseBehavior<Spell_Freeze.ModelType>,
        ILocalPrerequisiteTrigger<Commands.PlayCard>,
        ILocalEpilogTrigger<Commands.PlayCard>
    {
        private class Effect : BaseBehavior<Effect.ModelType>, IUnattackable, IStatusEffect
        {
            public string IconUri { get { return "atlas:Textures/Icons/Icons0$Freeze"; } }
            public string Text { get { return "冰冻\n该角色无法进攻。"; } }

            [BehaviorModel(typeof(Effect), HideFromEditor = true)]
            public class ModelType : BehaviorModel { }
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
                var lasting = new LastingEffect.ModelType { Duration = 2 }.CreateInitialized();
                var neutralize = new Effect.ModelType().CreateInitialized();
                (lasting as LastingEffect).CleanUps.Add(neutralize);
                Game.QueueCommands(
                    new Commands.AddBehavior(card, neutralize),
                    new Commands.AddBehavior(card, lasting));
            }
        }

        [BehaviorModel(typeof(Spell_Freeze), Category = "v0.5/Spell", DefaultName = "冰冻")]
        public class ModelType : BehaviorModel
        {
        }
    }
}
