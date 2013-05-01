using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
    public partial class Game
    {
        internal interface ICommandRunner
        {
            void Run(Commands.BaseCommand command);
            CommandResult RunPrerequisite(Commands.BaseCommand command);
        }

        internal class CommandRunner<TCommand> : ICommandRunner
            where TCommand : Commands.BaseCommand
        {
            [ThreadStatic]
            private static List<Behaviors.IBehavior> s_tempBhvContainer;

            public void Run(Commands.BaseCommand genericCommand)
            {
                var command = genericCommand as TCommand;
                command.ValidateOnRun();

                var targets = GenerateTargetList(command);

                ////////////////////////////////////////////

                command.ExecutionPhase = Commands.CommandPhase.Prolog;
                command.Context.Game.Controller.OnCommandBegin(command);
                foreach (var trigger in targets)
                {
                    var t = trigger as IPrologTrigger<TCommand>;
                    if (t != null)
                    {
                        t.RunProlog(command);
                    }
                }

                ////////////////////////////////////////////

                command.ExecutionPhase = Commands.CommandPhase.Main;
                command.RunMain();

                ////////////////////////////////////////////

                command.ExecutionPhase = Commands.CommandPhase.Epilog;
                foreach (var trigger in targets)
                {
                    var t = trigger as IEpilogTrigger<TCommand>;
                    if (t != null)
                    {
                        t.RunEpilog(command);
                    }
                }
                command.Context.Game.Controller.OnCommandEnd(command);
            }

            public CommandResult RunPrerequisite(Commands.BaseCommand genericCommand)
            {
                Debug.Assert(genericCommand is Commands.IInitiativeCommand);
                Debug.Assert(genericCommand.ExecutionPhase == Commands.CommandPhase.Prerequisite);

                var command = genericCommand as TCommand;
                foreach (var trigger in GenerateTargetList(command))
                {
                    var t = trigger as IPrerequisiteTrigger<TCommand>;
                    if (t != null)
                    {
                        var result = t.RunPrerequisite(command);
                        if (result.Canceled)
                        {
                            return result;
                        }
                    }
                }

                return CommandResult.Pass;
            }

            private List<Behaviors.IBehavior> GenerateTargetList(Commands.BaseCommand command)
            {
                Behaviors.BehaviorList additionalTarget = null;

                if (command is Commands.PlayCard)
                {
                    additionalTarget = (command as Commands.PlayCard).CardToPlay.Behaviors;
                }
                else if (command is Commands.ActivateAssist)
                {
                    additionalTarget = (command as Commands.ActivateAssist).CardToActivate.Behaviors;
                }
                else if (command is Commands.Redeem)
                {
                    additionalTarget = (command as Commands.Redeem).Target.Behaviors;
                }

                if (s_tempBhvContainer == null)
                {
                    s_tempBhvContainer = new List<Behaviors.IBehavior>();
                }

                s_tempBhvContainer.Clear();
                var newSize = command.Context.Game.m_commonTargets.Count + (additionalTarget != null ? additionalTarget.Count : 0);
                s_tempBhvContainer.Capacity = Math.Max(newSize, s_tempBhvContainer.Capacity);

                for (int i = 0; i < command.Context.Game.m_commonTargets.Count; ++i)
                {
                    s_tempBhvContainer.Add(command.Context.Game.m_commonTargets[i]);
                }

                if (additionalTarget != null)
                {
                    for (int i = 0; i < additionalTarget.Count; ++i)
                    {
                        s_tempBhvContainer.Add(additionalTarget[i]);
                    }
                }

                return s_tempBhvContainer;
            }
        }

        private List<Behaviors.IBehavior> m_commonTargets = new List<Behaviors.IBehavior>();

        internal void SubscribeCardToCommands(CardInstance card)
        {
            m_commonTargets.AddRange(card.Behaviors);
        }

        internal void UnsubscribeCardFromCommands(CardInstance card)
        {
            m_commonTargets.RemoveRange(Math.Max(m_commonTargets.IndexOf(card.Behaviors.FirstOrDefault()), 0), card.Behaviors.Count);
        }

        internal void SubscribeBehaviorToCommands(CardInstance card, Behaviors.IBehavior behavior)
        {
            if (card.Owner.ActivatedAssits.Contains(card)
                || card.Owner.m_battlefieldCards.Contains(card))
            {
                if (card.Behaviors.Count > 1)
                {
                    m_commonTargets.Insert(m_commonTargets.IndexOf(card.Behaviors[card.Behaviors.Count - 2]) + 1, behavior);
                }
                else
                {
                    m_commonTargets.Add(behavior);
                }
            }
        }

        internal void UnsubscribeBehaviorFromCommands(CardInstance card, Behaviors.IBehavior behavior)
        {
            if (card.Owner.ActivatedAssits.Contains(card)
                || card.Owner.m_battlefieldCards.Contains(card))
            {
                m_commonTargets.Remove(behavior);
            }
        }
    }
}
