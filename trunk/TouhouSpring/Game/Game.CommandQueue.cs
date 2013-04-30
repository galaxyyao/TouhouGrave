using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TouhouSpring
{
    public partial class Game
    {
        private ResolveContext m_currentResolveContext;

        public void QueueCommands(params Commands.BaseCommand[] commands)
        {
            m_currentResolveContext.QueueCommands(commands);
        }

        public void NeedMana(int amount)
        {
            m_currentResolveContext.NeedMana(amount);
        }

        public void NeedLife(int amount)
        {
            m_currentResolveContext.NeedLife(amount);
        }

        public void NeedManaOrLife(int mana, int life)
        {
            m_currentResolveContext.NeedManaOrLife(mana, life);
        }

        public void NeedTarget(Behaviors.IBehavior user, IIndexable<CardInstance> candidates, string message)
        {
            m_currentResolveContext.NeedTarget(user, candidates, message);
        }

        public int GetRemainingMana()
        {
            return m_currentResolveContext.GetRemainingMana();
        }

        public int GetRemainingLife()
        {
            return m_currentResolveContext.GetRemainingLife();
        }

        public IIndexable<CardInstance> GetTarget(Behaviors.IBehavior user)
        {
            return m_currentResolveContext.GetTarget(user);
        }

        internal bool IsCardPlayable(CardInstance card)
        {
            return IsCommandRunnable(new Commands.PlayCard(card));
        }

        internal bool IsCardActivatable(CardInstance card)
        {
            return IsCommandRunnable(new Commands.ActivateAssist(card));
        }

        internal bool IsCardRedeemable(CardInstance card)
        {
            return IsCommandRunnable(new Commands.Redeem(card));
        }

        internal bool IsSpellCastable(Behaviors.ICastableSpell spell)
        {
            return IsCommandRunnable(new Commands.CastSpell(spell));
        }

        bool IsCommandRunnable(Commands.BaseCommand command)
        {
            Debug.Assert(m_currentResolveContext == null);
            m_currentResolveContext = new ResolveContext(this);
            bool ret = m_currentResolveContext.IsCommandRunnable(command);
            m_currentResolveContext = null;
            return ret;
        }
    }
}
