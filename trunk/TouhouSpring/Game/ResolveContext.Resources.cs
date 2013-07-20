using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
    public class GameResource : Commands.ICause
    { }

    public partial class ResolveContext
    {
        private static GameResource s_resourceCause = new GameResource();

        private struct ResourceConditions
        {
            // fixed mana and life condition
            public int m_manaNeeded;
            public int m_lifeNeeded;

            // mana or life condition
            public bool m_manaOrLifeNeeded;
            public int m_manaOrLifeMana;
            public int m_manaOrLifeLife;
            public int m_manaOrLifeFinalMana;
            public int m_manaOrLifeFinalLife;
        }

        private class InteractionCondition
        {
            public Behaviors.IBehavior m_user;
            public int m_ticket;
            public bool m_compulsory;
            public Interactions.IQuickInteraction m_io;
            public Func<Interactions.IQuickInteraction> m_deferredIO;
            public object m_result;
        }

        private ResourceConditions m_resourceConditions;
        private List<InteractionCondition> m_interactionConditions = new List<InteractionCondition>();

        internal void NeedMana(int amount)
        {
            if (amount == 0)
            {
                throw new ArgumentOutOfRangeException("amount", "Amount must be other than zero.");
            }

            CheckInPrerequisite();
            m_resourceConditions.m_manaNeeded += amount;
        }

        internal void NeedLife(int amount)
        {
            if (amount == 0)
            {
                throw new ArgumentOutOfRangeException("amount", "Amount must be other than zero.");
            }

            CheckInPrerequisite();
            m_resourceConditions.m_lifeNeeded += amount;
        }

        internal void NeedManaOrLife(int mana, int life)
        {
            if (mana <= 0)
            {
                throw new ArgumentOutOfRangeException("mana", "Mana must be greater than zero.");
            }
            else if (life <= 0)
            {
                throw new ArgumentOutOfRangeException("life", "Life must be greater than zero.");
            }

            CheckInPrerequisite();
            if (m_resourceConditions.m_manaOrLifeNeeded)
            {
                throw new InvalidOperationException("ManaOrLife has already been claimed.");
            }
            m_resourceConditions.m_manaOrLifeNeeded = true;
            m_resourceConditions.m_manaOrLifeMana = mana;
            m_resourceConditions.m_manaOrLifeLife = life;
        }

        internal void NeedInteraction(Behaviors.IBehavior user, int ticket, bool compulsory, Interactions.IQuickInteraction io)
        {
            if (io == null)
            {
                throw new ArgumentNullException("io");
            }

            AddInteractionCondition(user, ticket, compulsory, io, null);
        }

        internal void NeedInteraction(Behaviors.IBehavior user, int ticket, bool compulsory, Func<Interactions.IQuickInteraction> deferredIO)
        {
            if (deferredIO == null)
            {
                throw new ArgumentNullException("deferredIO");
            }

            AddInteractionCondition(user, ticket, compulsory, null, deferredIO);
        }

        internal object GetInteractionResult(Behaviors.IBehavior user, int ticket)
        {
            foreach (var cond in m_interactionConditions)
            {
                if (cond.m_user == user && cond.m_ticket == ticket)
                {
                    return cond.m_result;
                }
            }
            throw new InvalidOperationException("Interaction condition with the ticket is not registered for the behavior.");
        }

        internal bool CheckCompulsoryTargets()
        {
            foreach (var cond in m_interactionConditions)
            {
                if (cond.m_compulsory
                    && cond.m_result is IIndexable<CardInstance>)
                {
                    foreach (var card in (IIndexable<CardInstance>)cond.m_result)
                    {
                        if (card.IsDestroyed)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private void AddInteractionCondition(Behaviors.IBehavior user, int ticket, bool compulsory, Interactions.IQuickInteraction io, Func<Interactions.IQuickInteraction> deferredIO)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            CheckInPrerequisite();

            foreach (var cond in m_interactionConditions)
            {
                if (cond.m_user == user && cond.m_ticket == ticket)
                {
                    throw new InvalidOperationException("Interaction condition with the same ticket has already been registered for the behavior.");
                }
            }

            m_interactionConditions.Add(new InteractionCondition
            {
                m_user = user,
                m_ticket = ticket,
                m_compulsory = compulsory,
                m_io = io,
                m_deferredIO = deferredIO,
                m_result = null
            });
        }

        private void ClearConditions()
        {
            m_resourceConditions.m_manaNeeded = 0;
            m_resourceConditions.m_lifeNeeded = 0;
            m_resourceConditions.m_manaOrLifeNeeded = false;
            m_resourceConditions.m_manaOrLifeMana = 0;
            m_resourceConditions.m_manaOrLifeLife = 0;
            m_interactionConditions.Clear();
        }

        private CommandResult ResolveConditions(bool prerequisiteOnly)
        {
            Debug.Assert(RunningCommand.ExecutionPhase == Commands.CommandPhase.Condition);
            var initiator = (RunningCommand as Commands.IInitiativeCommand).Initiator;

            if (m_resourceConditions.m_manaNeeded > 0)
            {
                m_resourceConditions.m_manaNeeded = initiator.CalculateFinalManaSubtract(m_resourceConditions.m_manaNeeded);
                if (initiator.Mana < m_resourceConditions.m_manaNeeded)
                {
                    return CommandResult.Cancel("Insufficient mana.");
                }
            }
            if (m_resourceConditions.m_lifeNeeded > 0)
            {
                m_resourceConditions.m_lifeNeeded = initiator.CalculateFinalLifeSubtract(m_resourceConditions.m_lifeNeeded);
                if (initiator.Health < m_resourceConditions.m_lifeNeeded)
                {
                    return CommandResult.Cancel("Insufficient life.");
                }
            }

            if (m_resourceConditions.m_manaOrLifeNeeded)
            {
                m_resourceConditions.m_manaOrLifeFinalMana = initiator.CalculateFinalManaSubtract(m_resourceConditions.m_manaOrLifeMana);
                m_resourceConditions.m_manaOrLifeFinalLife = initiator.CalculateFinalManaSubtract(m_resourceConditions.m_manaOrLifeLife);
                if (initiator.Mana < m_resourceConditions.m_manaNeeded + m_resourceConditions.m_manaOrLifeFinalMana
                    && initiator.Health < m_resourceConditions.m_lifeNeeded + m_resourceConditions.m_manaOrLifeFinalLife + 1)
                {
                    return CommandResult.Cancel("Insufficient mana or life.");
                }
            }

            foreach (var cond in m_interactionConditions)
            {
                if (cond.m_compulsory && cond.m_io != null && !cond.m_io.HasCandidates())
                {
                    return CommandResult.Cancel("No target.");
                }
            }

            if (!prerequisiteOnly)
            {
                if (m_resourceConditions.m_manaOrLifeNeeded
                    && initiator.Mana >= m_resourceConditions.m_manaNeeded + m_resourceConditions.m_manaOrLifeFinalMana
                    && initiator.Health > m_resourceConditions.m_lifeNeeded + m_resourceConditions.m_manaOrLifeFinalLife)
                {
                    var sb = new StringBuilder();
                    sb.Append("支付 [color:Red]");
                    sb.Append(m_resourceConditions.m_manaOrLifeMana.ToString());
                    sb.Append("[/color]灵力 或 [color:Red]");
                    sb.Append(m_resourceConditions.m_manaOrLifeLife.ToString());
                    sb.Append("[/color]生命？");

                    // TODO: change button text from Yes/No to Mana/Life
                    var choice = new Interactions.MessageBox(initiator, sb.ToString(),
                        Interactions.MessageBoxButtons.Yes | Interactions.MessageBoxButtons.No | Interactions.MessageBoxButtons.Cancel).Run();
                    if (choice == Interactions.MessageBoxButtons.Cancel)
                    {
                        return CommandResult.Cancel("Canceled.");
                    }
                    else if (choice == Interactions.MessageBoxButtons.Yes)
                    {
                        m_resourceConditions.m_manaNeeded += m_resourceConditions.m_manaOrLifeFinalMana;
                    }
                    else if (choice == Interactions.MessageBoxButtons.No)
                    {
                        m_resourceConditions.m_lifeNeeded += m_resourceConditions.m_manaOrLifeFinalLife;
                    }
                }

                foreach (var cond in m_interactionConditions)
                {
                    if (cond.m_io == null)
                    {
                        Debug.Assert(cond.m_deferredIO != null);
                        cond.m_io = cond.m_deferredIO();
                        if (cond.m_io == null)
                        {
                            continue;
                        }
                    }

                    cond.m_result = cond.m_io.HasCandidates() ? cond.m_io.Run() : null;
                    if (cond.m_result == null && cond.m_compulsory)
                    {
                        return CommandResult.Cancel("Canceled.");
                    }
                }

                if (m_resourceConditions.m_manaNeeded != 0)
                {
                    QueueResourceCommand(new Commands.SubtractPlayerMana(initiator, m_resourceConditions.m_manaNeeded, true, s_resourceCause));
                }
                if (m_resourceConditions.m_lifeNeeded != 0)
                {
                    QueueResourceCommand(new Commands.SubtractPlayerLife(initiator, m_resourceConditions.m_lifeNeeded, true, s_resourceCause));
                }
            }

            return CommandResult.Pass;
        }

        private void CheckInPrerequisite()
        {
            if (RunningCommand == null || RunningCommand.ExecutionPhase != Commands.CommandPhase.Prerequisite)
            {
                throw new InvalidOperationException("Conditions can only be registered in Prerequisite phase.");
            }
        }

        private void CheckNotInPrerequisite()
        {
            if (RunningCommand == null || RunningCommand.ExecutionPhase == Commands.CommandPhase.Prerequisite)
            {
                throw new InvalidOperationException("Can't get condition result in Prerequisite phase.");
            }
        }
    }
}
