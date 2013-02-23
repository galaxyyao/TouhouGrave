using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
    public partial class Game
    {
        private struct PlayerResourceConditions
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

        private PlayerResourceConditions[] m_resourceConditions;
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
            else if (player.Game != this)
            {
                throw new ArgumentException("Player is invalid.", "player");
            }

            CheckInPrerequisite();
            m_resourceConditions[Players.IndexOf(player)].m_manaNeeded += amount;
        }

        public void NeedLife(Player player, int amount)
        {
            if (player == null)
            {
                throw new ArgumentNullException("player");
            }
            else if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException("amount", "Amount must be greater than zero.");
            }
            else if (player.Game != this)
            {
                throw new ArgumentException("Player is invalid.", "player");
            }

            CheckInPrerequisite();
            m_resourceConditions[Players.IndexOf(player)].m_lifeNeeded += amount;
        }

        public void NeedManaOrLife(Player player, int mana, int life)
        {
            if (player == null)
            {
                throw new ArgumentNullException("player");
            }
            else if (mana <= 0)
            {
                throw new ArgumentOutOfRangeException("mana", "Mana must be greater than zero.");
            }
            else if (life <= 0)
            {
                throw new ArgumentOutOfRangeException("life", "Life must be greater than zero.");
            }
            else if (player.Game != this)
            {
                throw new ArgumentException("Player is invalid.", "player");
            }

            CheckInPrerequisite();
            var pid = Players.IndexOf(player);
            if (m_resourceConditions[pid].m_manaOrLifeNeeded)
            {
                throw new InvalidOperationException("ManaOrLife has already been claimed.");
            }
            m_resourceConditions[pid].m_manaOrLifeNeeded = true;
            m_resourceConditions[pid].m_manaOrLifeMana = mana;
            m_resourceConditions[pid].m_manaOrLifeLife = life;
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
            else if (player.Game != this)
            {
                throw new ArgumentException("Player is invalid.", "player");
            }

            CheckNotInPrerequisite();
            return player.Mana - m_resourceConditions[Players.IndexOf(player)].m_manaNeeded;
        }

        public int GetRemainingLife(Player player)
        {
            if (player == null)
            {
                throw new ArgumentNullException("player");
            }
            else if (player.Game != this)
            {
                throw new ArgumentException("Player is invalid.", "player");
            }

            CheckNotInPrerequisite();
            return player.Health - m_resourceConditions[Players.IndexOf(player)].m_lifeNeeded;
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
                m_resourceConditions[i].m_manaNeeded = 0;
                m_resourceConditions[i].m_lifeNeeded = 0;
                m_resourceConditions[i].m_manaOrLifeNeeded = false;
                m_resourceConditions[i].m_manaOrLifeMana = 0;
                m_resourceConditions[i].m_manaOrLifeLife = 0;
            }
            m_targetConditions.Clear();
        }

        internal CommandResult ResolveConditions(bool prerequisiteOnly)
        {
            for (int i = 0; i < Players.Count; ++i)
            {
                var player = Players[i];
                var resourceConditions = m_resourceConditions[i];

                if (resourceConditions.m_manaNeeded != 0)
                {
                    resourceConditions.m_manaNeeded = player.CalculateFinalManaSubtract(resourceConditions.m_manaNeeded);
                    if (player.Mana < resourceConditions.m_manaNeeded)
                    {
                        return CommandResult.Cancel("Insufficient mana.");
                    }
                }
                if (resourceConditions.m_lifeNeeded != 0)
                {
                    resourceConditions.m_lifeNeeded = player.CalculateFinalLifeSubtract(resourceConditions.m_lifeNeeded);
                    if (player.Health < resourceConditions.m_lifeNeeded)
                    {
                        return CommandResult.Cancel("Insufficient life.");
                    }
                }

                if (resourceConditions.m_manaOrLifeNeeded)
                {
                    resourceConditions.m_manaOrLifeFinalMana = player.CalculateFinalManaSubtract(resourceConditions.m_manaOrLifeMana);
                    resourceConditions.m_manaOrLifeFinalLife = player.CalculateFinalManaSubtract(resourceConditions.m_manaOrLifeLife);
                    if (player.Mana < resourceConditions.m_manaNeeded + resourceConditions.m_manaOrLifeFinalMana
                        && player.Health < resourceConditions.m_lifeNeeded + resourceConditions.m_manaOrLifeFinalLife)
                    {
                        return CommandResult.Cancel("Insufficient mana or life.");
                    }
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
                for (int i = 0; i < Players.Count; ++i)
                {
                    var player = Players[i];
                    var resourceConditions = m_resourceConditions[i];

                    if (resourceConditions.m_manaOrLifeNeeded
                        && player.Mana >= resourceConditions.m_manaNeeded + resourceConditions.m_manaOrLifeFinalMana
                        && player.Health > resourceConditions.m_lifeNeeded + resourceConditions.m_manaOrLifeFinalLife)
                    {
                        var sb = new StringBuilder();
                        sb.Append("支付 [color:Red]");
                        sb.Append(resourceConditions.m_manaOrLifeMana.ToString());
                        sb.Append("[/color]灵力 或 [color:Red]");
                        sb.Append(resourceConditions.m_manaOrLifeLife.ToString());
                        sb.Append("[/color]生命？");

                        // TODO: change button text from Yes/No to Mana/Life
                        var choice = new Interactions.MessageBox(player, sb.ToString(),
                            Interactions.MessageBoxButtons.Yes | Interactions.MessageBoxButtons.No | Interactions.MessageBoxButtons.Cancel).Run();
                        if (choice == Interactions.MessageBoxButtons.Cancel)
                        {
                            return CommandResult.Cancel("Canceled.");
                        }
                        else if (choice == Interactions.MessageBoxButtons.Yes)
                        {
                            resourceConditions.m_manaNeeded += resourceConditions.m_manaOrLifeFinalMana;
                        }
                        else if (choice == Interactions.MessageBoxButtons.No)
                        {
                            resourceConditions.m_lifeNeeded += resourceConditions.m_manaOrLifeFinalLife;
                        }
                    }
                }

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
                    var resourceConditions = m_resourceConditions[i];
                    if (resourceConditions.m_manaNeeded != 0)
                    {
                        IssueCommand(new Commands.SubtractPlayerMana(Players[i], resourceConditions.m_manaNeeded, true, null));
                    }
                    if (resourceConditions.m_lifeNeeded != 0)
                    {
                        IssueCommand(new Commands.SubtractPlayerLife(Players[i], resourceConditions.m_lifeNeeded, true, null));
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
