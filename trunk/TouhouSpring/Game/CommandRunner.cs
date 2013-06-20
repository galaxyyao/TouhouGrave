using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
    internal interface ICommandRunner
    {
        CommandResult RunPrerequisite(Commands.BaseCommand command);
        void RunProlog(Commands.BaseCommand command);
        ResolveContext RunPreemptive(Commands.BaseCommand command, bool firstTimeTriggering);
        void RunMainAndEpilog(Commands.BaseCommand command);
    }

    internal class CommandRunner<TCommand> : ICommandRunner
        where TCommand : Commands.BaseCommand
    {
        public CommandResult RunPrerequisite(Commands.BaseCommand command)
        {
            Debug.Assert(command is Commands.IInitiativeCommand);
            Debug.Assert(command.ExecutionPhase == Commands.CommandPhase.Prerequisite);

            var tCommand = command as TCommand;
            foreach (var targetList in command.Context.Game.m_globalTargetLists)
            {
                var result = RunPrerequisite(tCommand, targetList);
                if (result.Canceled)
                {
                    return result;
                }
            }

            var localTargets = GetLocalTargets(command);
            return localTargets != null
                   ? RunPrerequisite(tCommand, localTargets)
                   : CommandResult.Pass;
        }

        private CommandResult RunPrerequisite(TCommand command, Behaviors.BehaviorList targets)
        {
            foreach (var trigger in targets)
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

        public void RunProlog(Commands.BaseCommand command)
        {
            Debug.Assert(command.ExecutionPhase == Commands.CommandPhase.Prolog);

            var tCommand = command as TCommand;
            foreach (var targetList in command.Context.Game.m_globalTargetLists)
            {
                RunProlog(tCommand, targetList);
            }

            var localTargets = GetLocalTargets(command);
            if (localTargets != null)
            {
                RunProlog(tCommand, localTargets);
            }
        }

        private void RunProlog(TCommand command, Behaviors.BehaviorList targets)
        {
            foreach (var trigger in targets)
            {
                var t = trigger as IPrologTrigger<TCommand>;
                if (t != null)
                {
                    t.RunProlog(command);
                }
            }
        }

        public ResolveContext RunPreemptive(Commands.BaseCommand command, bool firstTimeTriggering)
        {
            Debug.Assert(command.ExecutionPhase == Commands.CommandPhase.Preemptive);

            var tCommand = command as TCommand;
            foreach (var targetList in command.Context.Game.m_globalTargetLists)
            {
                var newStack = RunPreemptive(tCommand, targetList, firstTimeTriggering);
                if (newStack != null)
                {
                    return newStack;
                }
            }

            var localTargets = GetLocalTargets(command);
            return localTargets != null
                   ? RunPreemptive(tCommand, localTargets, firstTimeTriggering)
                   : null;
        }

        private ResolveContext RunPreemptive(TCommand command, Behaviors.BehaviorList targets, bool firstTimeTriggering)
        {
            foreach (var trigger in targets)
            {
                var t = trigger as IPreemptiveTrigger<TCommand>;
                if (t != null)
                {
                    var ctx = t.RunPreemptive(command, firstTimeTriggering);
                    if (ctx != null)
                    {
                        return ctx;
                    }
                }
            }
            return null;
        }

        public void RunMainAndEpilog(Commands.BaseCommand command)
        {
            Debug.Assert(command.ExecutionPhase == Commands.CommandPhase.Main);

            var tCommand = command as TCommand;

            ////////////////////////////////////////////

            tCommand.ExecutionPhase = Commands.CommandPhase.Main;
            tCommand.RunMain();

            ////////////////////////////////////////////

            tCommand.ExecutionPhase = Commands.CommandPhase.Epilog;
            foreach (var targetList in command.Context.Game.m_globalTargetLists)
            {
                RunEpilog(tCommand, targetList);
            }
            var localTargets = GetLocalTargets(command);
            if (localTargets != null)
            {
                RunEpilog(tCommand, localTargets);
            }
        }

        private void RunEpilog(TCommand command, IEnumerable<Behaviors.IBehavior> targets)
        {
            foreach (var trigger in targets)
            {
                var t = trigger as IEpilogTrigger<TCommand>;
                if (t != null)
                {
                    t.RunEpilog(command);
                }
            }
        }

        // TODO: implement real local target
        private Behaviors.BehaviorList GetLocalTargets(Commands.BaseCommand command)
        {
            if (command.ExecutionPhase < Commands.CommandPhase.Main)
            {
                var playCard = command as Commands.MoveCard<Commands.Hand, Commands.Battlefield>;
                if (playCard != null)
                {
                    return playCard.Subject.Behaviors;
                }

                var activateAssist = command as Commands.ActivateAssist;
                if (activateAssist != null)
                {
                    return activateAssist.CardToActivate.Behaviors;
                }
            }
            else
            {
                var kill = command as Commands.KillMove<Commands.Battlefield>;
                if (kill != null)
                {
                    return kill.Subject.Behaviors;
                }

                var deactivateAssist = command as Commands.DeactivateAssist;
                if (deactivateAssist != null)
                {
                    return deactivateAssist.CardToDeactivate.Behaviors;
                }
            }

            var redeem = command as Commands.MoveCard<Commands.Sacrifice, Commands.Hand>;
            if (redeem != null)
            {
                return redeem.Subject.Behaviors;
            }

            return null;
        }
    }
}
