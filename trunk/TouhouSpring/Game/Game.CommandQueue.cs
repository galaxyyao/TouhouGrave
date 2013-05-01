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
        public void QueueCommands(params Commands.BaseCommand[] commands)
        {
            m_resolveContextStack.Peek().QueueCommands(commands);
        }

        public void NeedMana(int amount)
        {
            m_resolveContextStack.Peek().NeedMana(amount);
        }

        public void NeedLife(int amount)
        {
            m_resolveContextStack.Peek().NeedLife(amount);
        }

        public void NeedManaOrLife(int mana, int life)
        {
            m_resolveContextStack.Peek().NeedManaOrLife(mana, life);
        }

        public void NeedTarget(Behaviors.IBehavior user, IIndexable<CardInstance> candidates, string message)
        {
            m_resolveContextStack.Peek().NeedTarget(user, candidates, message);
        }

        public int GetRemainingMana()
        {
            return m_resolveContextStack.Peek().GetRemainingMana();
        }

        public int GetRemainingLife()
        {
            return m_resolveContextStack.Peek().GetRemainingLife();
        }

        public IIndexable<CardInstance> GetTarget(Behaviors.IBehavior user)
        {
            return m_resolveContextStack.Peek().GetTarget(user);
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
            var ctx = new ResolveContext(this);
            m_resolveContextStack.Push(ctx);
            bool ret = ctx.IsCommandRunnable(command);
            m_resolveContextStack.Pop();
            return ret;
        }
    }
}
