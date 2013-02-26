using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
    public partial class Game
    {
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

        private class TargetCondition
        {
            public Behaviors.IBehavior m_user;
            public IIndexable<BaseCard> m_candidates;
            public string m_message;
            public IIndexable<BaseCard> m_selection;
        }

        private ResourceConditions m_resourceConditions;
        private List<TargetCondition> m_targetConditions = new List<TargetCondition>();

        public void NeedMana(int amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException("amount", "Amount must be greater than zero.");
            }

            CheckInPrerequisite();
            m_resourceConditions.m_manaNeeded += amount;
        }

        public void NeedLife(int amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException("amount", "Amount must be greater than zero.");
            }

            CheckInPrerequisite();
            m_resourceConditions.m_lifeNeeded += amount;
        }

        public void NeedManaOrLife(int mana, int life)
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

        public void NeedTarget(Behaviors.IBehavior user, IIndexable<BaseCard> candidates, string message)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            else if (candidates == null)
            {
                throw new ArgumentNullException("candidates");
            }
            else if (String.IsNullOrEmpty(message))
            {
                throw new ArgumentNullException("message");
            }
            CheckInPrerequisite();

            if (m_targetConditions.Any(tgt => tgt.m_user == user))
            {
                throw new InvalidOperationException("Multiple target conditions can't be registered for one behavior.");
            }

            m_targetConditions.Add(new TargetCondition
            {
                m_user = user,
                m_candidates = candidates,
                m_message = message,
                m_selection = null
            });
        }

        public int GetRemainingMana()
        {
            CheckNotInPrerequisite();
            var player = (RunningCommand as Commands.IInitiativeCommand).Initiator;
            return player.Mana - m_resourceConditions.m_manaNeeded;
        }

        public int GetRemainingLife()
        {
            CheckNotInPrerequisite();
            var player = (RunningCommand as Commands.IInitiativeCommand).Initiator;
            return player.Health - m_resourceConditions.m_lifeNeeded;
        }

        public IIndexable<BaseCard> GetTarget(Behaviors.IBehavior user)
        {
            CheckNotInPrerequisite();
            var targetCondition = m_targetConditions.FirstOrDefault(tgt => tgt.m_user == user);
            if (targetCondition == null)
            {
                throw new InvalidOperationException("No target is registered for this behavior before.");
            }
            return targetCondition.m_selection;
        }

        internal void ClearConditions()
        {
            m_resourceConditions.m_manaNeeded = 0;
            m_resourceConditions.m_lifeNeeded = 0;
            m_resourceConditions.m_manaOrLifeNeeded = false;
            m_resourceConditions.m_manaOrLifeMana = 0;
            m_resourceConditions.m_manaOrLifeLife = 0;
            m_targetConditions.Clear();
        }

        internal CommandResult ResolveConditions(bool prerequisiteOnly)
        {
            var initiator = (RunningCommand as Commands.IInitiativeCommand).Initiator;

            if (m_resourceConditions.m_manaNeeded != 0)
            {
                m_resourceConditions.m_manaNeeded = initiator.CalculateFinalManaSubtract(m_resourceConditions.m_manaNeeded);
                if (initiator.Mana < m_resourceConditions.m_manaNeeded)
                {
                    return CommandResult.Cancel("Insufficient mana.");
                }
            }
            if (m_resourceConditions.m_lifeNeeded != 0)
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
                    && initiator.Health < m_resourceConditions.m_lifeNeeded + m_resourceConditions.m_manaOrLifeFinalLife)
                {
                    return CommandResult.Cancel("Insufficient mana or life.");
                }
            }

            foreach (var tgt in m_targetConditions)
            {
                if (tgt.m_candidates.Count == 0)
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

                foreach (var tgt in m_targetConditions)
                {
                    tgt.m_selection = new Interactions.SelectCards(
                        initiator,
                        tgt.m_candidates,
                        Interactions.SelectCards.SelectMode.Single,
                        tgt.m_message).Run();
                    if (tgt.m_selection.Count == 0)
                    {
                        return CommandResult.Cancel("Canceled.");
                    }
                }

                if (m_resourceConditions.m_manaNeeded != 0)
                {
                    IssueCommand(new Commands.SubtractPlayerMana(initiator, m_resourceConditions.m_manaNeeded, true, null));
                }
                if (m_resourceConditions.m_lifeNeeded != 0)
                {
                    IssueCommand(new Commands.SubtractPlayerLife(initiator, m_resourceConditions.m_lifeNeeded, true, null));
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
