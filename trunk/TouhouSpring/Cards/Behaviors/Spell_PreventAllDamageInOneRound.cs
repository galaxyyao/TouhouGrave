using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class Spell_PreventAllDamageInOneRound :
        BaseBehavior<Spell_PreventAllDamageInOneRound.ModelType>,
        ICastableSpell,
        IPrologTrigger<Commands.DealDamageToPlayer>,
        IPrologTrigger<Commands.DealDamageToCard>,
        IEpilogTrigger<Commands.EndTurn>
    {
        private bool m_isProtected = false;
        private Player m_currentPlayer;
        private Player m_spellCaster;

        void ICastableSpell.Run(CommandContext<Commands.CastSpell> context)
        {
            m_isProtected = true;
            m_currentPlayer = context.Game.PlayerPlayer;
            m_spellCaster = Host.Owner;
        }

        void IPrologTrigger<Commands.DealDamageToPlayer>.Run(CommandContext<Commands.DealDamageToPlayer> context)
        {
            if(m_isProtected&& context.Command.Player == m_spellCaster)
                context.Command.DamageToDeal = 0;
        }

        void IPrologTrigger<Commands.DealDamageToCard>.Run(CommandContext<Commands.DealDamageToCard> context)
        {
            if (m_isProtected && context.Command.Target.Owner == m_spellCaster)
                context.Command.DamageToDeal = 0;
        }

        void IEpilogTrigger<Commands.EndTurn>.Run(CommandContext<Commands.EndTurn> context)
        {
            if (context.Game.PlayerPlayer != m_currentPlayer && m_isProtected)
            {
                m_currentPlayer = null;
                m_isProtected = false;
                m_spellCaster = null;
            }
        }

        [BehaviorModel(typeof(Spell_PreventAllDamageInOneRound), DefaultName = "梦想封印")]
        public class ModelType : BehaviorModel
        { }
    }
}
