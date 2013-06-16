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

        public void NeedTargets(Behaviors.IBehavior user, IIndexable<CardInstance> candidates, string message)
        {
            m_resolveContextStack.Peek().NeedTargets(user, true, candidates, message);
        }

        public void NeedOptionalTargets(Behaviors.IBehavior user, IIndexable<CardInstance> candidates, string message)
        {
            m_resolveContextStack.Peek().NeedTargets(user, false, candidates, message);
        }

        public IIndexable<CardInstance> GetTargets(Behaviors.IBehavior user)
        {
            return m_resolveContextStack.Peek().GetTargets(user);
        }

        internal bool IsCardPlayable(CardInstance card)
        {
            return IsCommandRunnable(new Commands.InitiativeMoveCard<Commands.Hand, Commands.Battlefield>(card));
        }

        internal bool IsCardActivatable(CardInstance card)
        {
            return IsCommandRunnable(new Commands.ActivateAssist(card));
        }

        internal bool IsCardRedeemable(CardInstance card)
        {
            return IsCommandRunnable(new Commands.InitiativeMoveCard<Commands.Sacrifice, Commands.Hand>(card));
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
