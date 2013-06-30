using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Spell_DirectDamage_ToPlayer :
        BaseBehavior<Spell_DirectDamage_ToPlayer.ModelType>,
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

        [BehaviorModel(typeof(Spell_DirectDamage_ToPlayer), DefaultName = "直击", Category = "v0.5/Spell")]
        public class ModelType : BehaviorModel
        {
            public int Damage { get; set; }
        }
    }
}
