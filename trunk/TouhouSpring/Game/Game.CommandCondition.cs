using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
    public partial class Game
    {
        private class TargetCondition
        {
            public Behaviors.IBehavior m_user;
            public IIndexable<BaseCard> m_candidates;
            public string m_message;
            public IIndexable<BaseCard> m_selection;
        }

        private int[] m_manaNeeded;
        private bool[] m_remainingManaNeeded;
        private List<TargetCondition> m_targetConditions = new List<TargetCondition>();

        public void NeedMana(Player player, int amount)
        {
            if (player == null)
            {
                throw new ArgumentNullException("player");
            }
            else if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException("amount", "Amount must be greater than zero.");
            }
            else if (!Players.Contains(player))
            {
                throw new ArgumentException("Player is invalid.", "player");
            }

            CheckInPrerequisite();
            m_manaNeeded[Players.IndexOf(player)] += amount;
        }

        public void NeedRemainingMana(Player player)
        {
            if (player == null)
            {
                throw new ArgumentNullException("player");
            }
            else if (!Players.Contains(player))
            {
                throw new ArgumentException("Player is invalid.", "player");
            }

            CheckInPrerequisite();
            int pid = Players.IndexOf(player);
            if (m_remainingManaNeeded[pid])
            {
                throw new InvalidOperationException("The remaining mana has already been claimed.");
            }
            m_remainingManaNeeded[pid] = true;
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

        public int GetRemainingMana(Player player)
        {
            if (player == null)
            {
                throw new ArgumentNullException("player");
            }
            else if (!Players.Contains(player))
            {
                throw new ArgumentException("Player is invalid.", "player");
            }

            CheckNotInPrerequisite();
            return player.Mana - m_manaNeeded[Players.IndexOf(player)];
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
            for (int i = 0; i < Players.Count; ++i)
            {
                m_manaNeeded[i] = 0;
                m_remainingManaNeeded[i] = false;
            }
            m_targetConditions.Clear();
        }

        internal CommandResult ResolveConditions(bool prerequisiteOnly)
        {
            for (int i = 0; i < Players.Count; ++i)
            {
                var player = Players[i];
                if (player.Mana < m_manaNeeded[i]
                    || player.Mana == m_manaNeeded[i] && m_remainingManaNeeded[i])
                {
                    return CommandResult.Cancel("Insufficient mana.");
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
                foreach (var tgt in m_targetConditions)
                {
                    tgt.m_selection = new Interactions.SelectCards(
                        ActingPlayer,
                        tgt.m_candidates,
                        Interactions.SelectCards.SelectMode.Single,
                        tgt.m_message).Run();
                    if (tgt.m_selection.Count == 0)
                    {
                        return CommandResult.Cancel("Canceled.");
                    }
                }

                for (int i = 0; i < Players.Count; ++i)
                {
                    if (m_remainingManaNeeded[i])
                    {
                        IssueCommand(new Commands.UpdateMana(Players[i], -Players[i].Mana, null));
                    }
                    else if (m_manaNeeded[i] != 0)
                    {
                        IssueCommand(new Commands.UpdateMana(Players[i], -m_manaNeeded[i], null));
                    }
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
