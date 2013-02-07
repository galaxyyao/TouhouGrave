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
            Game.IssueCommands(new Commands.DealDamageToPlayer(
                Game.Players.First(player => player != Host.Owner),
                Model.Damage, this));
        }

        [BehaviorModel(DefaultName = "五道难题")]
        public class ModelType : BehaviorModel<Spell_DamageDirectToPlayer>
        {
            public int Damage { get; set; }
        }
    }
}
