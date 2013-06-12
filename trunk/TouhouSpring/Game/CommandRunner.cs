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
            var result = RunPrerequisite(tCommand, command.Context.Game.m_commonTargets);
            if (!result.Canceled)
            {
                var additionalTargets = GetAdditionalCommandTargets(command);
                if (additionalTargets != null)
                {
                    result = RunPrerequisite(tCommand, additionalTargets);
                }
            }
            return result;
        }

        private CommandResult RunPrerequisite(TCommand command, IEnumerable<Behaviors.IBehavior> targets)
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
            RunProlog(tCommand, command.Context.Game.m_commonTargets);
            var additionalTargets = GetAdditionalCommandTargets(command);
            if (additionalTargets != null)
            {
                RunProlog(tCommand, additionalTargets);
            }
        }

        private void RunProlog(TCommand command, IEnumerable<Behaviors.IBehavior> targets)
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
            var newStack = RunPreemptive(tCommand, command.Context.Game.m_commonTargets, firstTimeTriggering);
            if (newStack == null)
            {
                var additionalTargets = GetAdditionalCommandTargets(command);
                if (additionalTargets != null)
                {
                    newStack = RunPreemptive(tCommand, additionalTargets, firstTimeTriggering);
                }
            }
            return newStack;
        }

        private ResolveContext RunPreemptive(TCommand command, IEnumerable<Behaviors.IBehavior> targets, bool firstTimeTriggering)
        {
            foreach (var trigger in targets)
            {
                var t = trigger as IPreemptiveTrigger<TCommand>;
                if (t != null)
                {
                    // TODO: get the new stack
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
            RunEpilog(tCommand, command.Context.Game.m_commonTargets);
            var additionalTargets = GetAdditionalCommandTargets(command);
            if (additionalTargets != null)
            {
                RunEpilog(tCommand, additionalTargets);
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

        private Behaviors.BehaviorList GetAdditionalCommandTargets(Commands.BaseCommand command)
        {
            if (command.ExecutionPhase < Commands.CommandPhase.Main)
            {
                var playCard = command as Commands.PlayCard;
                if (playCard != null)
                {
                    return playCard.CardToPlay.Behaviors;
                }

                var activateAssist = command as Commands.ActivateAssist;
                if (activateAssist != null)
                {
                    return activateAssist.CardToActivate.Behaviors;
                }
            }
            else
            {
                var kill = command as Commands.Kill;
                if (kill != null && kill.LeftBattlefield)
                {
                    return kill.Target.Behaviors;
                }

                var deactivateAssist = command as Commands.DeactivateAssist;
                if (deactivateAssist != null)
                {
                    return deactivateAssist.CardToDeactivate.Behaviors;
                }
            }

            var redeem = command as Commands.Redeem;
            if (redeem != null)
            {
                return redeem.Target.Behaviors;
            }

            return null;
        }
    }
}
