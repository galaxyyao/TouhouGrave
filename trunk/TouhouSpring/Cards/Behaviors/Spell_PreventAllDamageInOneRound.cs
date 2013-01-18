﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Spell_PreventAllDamageInOneRound :
        BaseBehavior<Spell_PreventAllDamageInOneRound.ModelType>,
        ICastableSpell,
        IPrologTrigger<Commands.DealDamageToPlayer>,
        IPrologTrigger<Commands.DealDamageToCard>,
        IEpilogTrigger<Commands.EndTurn>
    {
        private bool m_isProtected = false;
        private Player m_currentPlayer;
        private Player m_spellCaster;

        public void RunSpell(Commands.CastSpell command)
        {
            m_isProtected = true;
            m_currentPlayer = Game.ActingPlayer;
            m_spellCaster = Host.Owner;
        }

        public void RunProlog(Commands.DealDamageToPlayer command)
        {
            if(m_isProtected&& command.Player == m_spellCaster)
                command.PatchDamageToDeal(0);
        }

        public void RunProlog(Commands.DealDamageToCard command)
        {
            if (m_isProtected && command.Target.Owner == m_spellCaster)
                command.PatchDamageToDeal(0);
        }

        public void RunEpilog(Commands.EndTurn command)
        {
            if (Game.ActingPlayer != m_currentPlayer && m_isProtected)
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
