using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Spell_DamageDirectToPlayer :
        BaseBehavior<Spell_DamageDirectToPlayer.ModelType>,
        ICastableSpell
    {
        void ICastableSpell.Run(Commands.CastSpell command)
        {
            // TODO: select player to deal damage to
            Game.IssueCommands(new Commands.DealDamageToPlayer(
                Game.Players.First(player => player != Host.Owner),
                this, Model.Damage));
        }

        [BehaviorModel(typeof(Spell_DamageDirectToPlayer), DefaultName = "五道难题")]
        public class ModelType : BehaviorModel
        {
            public int Damage { get; set; }
        }
    }
}
