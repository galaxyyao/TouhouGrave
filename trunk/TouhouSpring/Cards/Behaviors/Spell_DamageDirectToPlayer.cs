using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Spell_DamageDirectToPlayer :
        BaseBehavior<Spell_DamageDirectToPlayer.ModelType>,
        Commands.ICause,
        ICastableSpell
    {
        public void RunSpell(Commands.CastSpell command)
        {
            // TODO: select player to deal damage to
            Game.QueueCommands(new Commands.SubtractPlayerLife(
                Game.Players.First(player => player != Host.Owner),
                Model.Damage, this));
        }

        [BehaviorModel(typeof(Spell_DamageDirectToPlayer), DefaultName = "五道难题")]
        public class ModelType : BehaviorModel
        {
            public int Damage { get; set; }
        }
    }
}
