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
        [ThreadStatic]
        private static Behaviors.IBehavior[] s_singleBehaviorArray;

        public CommandResult RunPrerequisite(Commands.BaseCommand command)
        {
            Debug.Assert(command is Commands.IInitiativeCommand);
            Debug.Assert(command.ExecutionPhase == Commands.CommandPhase.Prerequisite);

            var tCommand = command as TCommand;
            foreach (var targetList in command.Context.Game.m_globalTargetLists)
            {
                Behaviors.StaticBehaviorHost.Host = targetList.Host;
                foreach (var trigger in targetList)
                {
                    var t = trigger as IGlobalPrerequisiteTrigger<TCommand>;
                    if (t != null)
                    {
                        var result = t.RunGlobalPrerequisite(tCommand);
                        if (result.Canceled)
                        {
                            return result;
                        }
                    }
                }
            }

            CardInstance localHost;
            var localTargets = GetLocalTargets(command, out localHost);
            if (localTargets != null)
            {
                Behaviors.StaticBehaviorHost.Host = localHost;
                foreach (var trigger in localTargets)
                {
                    var t = trigger as ILocalPrerequisiteTrigger<TCommand>;
                    if (t != null)
                    {
                        var result = t.RunLocalPrerequisite(tCommand);
                        if (result.Canceled)
                        {
                            return result;
                        }
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
                Behaviors.StaticBehaviorHost.Host = targetList.Host;
                foreach (var trigger in targetList)
                {
                    var t = trigger as IGlobalPrologTrigger<TCommand>;
                    if (t != null)
                    {
                        t.RunGlobalProlog(tCommand);
                    }
                }
            }

            CardInstance localHost;
            var localTargets = GetLocalTargets(command, out localHost);
            if (localTargets != null)
            {
                Behaviors.StaticBehaviorHost.Host = localHost;
                foreach (var trigger in localTargets)
                {
                    var t = trigger as ILocalPrologTrigger<TCommand>;
                    if (t != null)
                    {
                        t.RunLocalProlog(tCommand);
                    }
                }
            }
        }

        public ResolveContext RunPreemptive(Commands.BaseCommand command, bool firstTimeTriggering)
        {
            Debug.Assert(command.ExecutionPhase == Commands.CommandPhase.Preemptive);

            var tCommand = command as TCommand;
            foreach (var targetList in command.Context.Game.m_globalTargetLists)
            {
                Behaviors.StaticBehaviorHost.Host = targetList.Host;
                foreach (var trigger in targetList)
                {
                    var t = trigger as IGlobalPreemptiveTrigger<TCommand>;
                    if (t != null)
                    {
                        var newStack = t.RunGlobalPreemptive(tCommand, firstTimeTriggering);
                        if (newStack != null)
                        {
                            return newStack;
                        }
                    }
                }
            }

            CardInstance localHost;
            var localTargets = GetLocalTargets(command, out localHost);
            if (localTargets != null)
            {
                Behaviors.StaticBehaviorHost.Host = localHost;
                foreach (var trigger in localTargets)
                {
                    var t = trigger as ILocalPreemptiveTrigger<TCommand>;
                    if (t != null)
                    {
                        var newStack = t.RunLocalPreemptive(tCommand, firstTimeTriggering);
                        if (newStack != null)
                        {
                            return newStack;
                        }
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
                Behaviors.StaticBehaviorHost.Host = targetList.Host;
                foreach (var trigger in targetList)
                {
                    var t = trigger as IGlobalEpilogTrigger<TCommand>;
                    if (t != null)
                    {
                        t.RunGlobalEpilog(tCommand);
                    }
                }
            }

            CardInstance localHost;
            var localTargets = GetLocalTargets(command, out localHost);
            if (localTargets != null)
            {
                Behaviors.StaticBehaviorHost.Host = localHost;
                foreach (var trigger in localTargets)
                {
                    var t = trigger as ILocalEpilogTrigger<TCommand>;
                    if (t != null)
                    {
                        t.RunLocalEpilog(tCommand);
                    }
                }
            }
        }

        private IEnumerable<Behaviors.IBehavior> GetLocalTargets(Commands.BaseCommand command, out CardInstance host)
        {
            if (s_singleBehaviorArray == null)
            {
                s_singleBehaviorArray = new Behaviors.IBehavior[1];
            }

            var moveCard = command as Commands.IMoveCard;
            if (moveCard != null && moveCard.Subject != null)
            {
                host = moveCard.Subject;
                return moveCard.Subject.Behaviors;
            }

            var dealDamageToCard = command as Commands.DealDamageToCard;
            if (dealDamageToCard != null && dealDamageToCard.Target != null)
            {
                host = dealDamageToCard.Target;
                return dealDamageToCard.Target.Behaviors;
            }

            var healCard = command as Commands.HealCard;
            if (healCard != null && healCard.Target != null)
            {
                host = healCard.Target;
                return healCard.Target.Behaviors;
            }

            var castSpell = command as Commands.CastSpell;
            if (castSpell != null)
            {
                // TODO: include spell host in the command
                host = null;
                s_singleBehaviorArray[0] = castSpell.Spell;
                return s_singleBehaviorArray;
            }

            var addBehavior = command as Commands.AddBehavior;
            if (addBehavior != null)
            {
                host = addBehavior.Target;
                s_singleBehaviorArray[0] = addBehavior.Behavior;
                return s_singleBehaviorArray;
            }

            var removeBehavior = command as Commands.RemoveBehavior;
            if (removeBehavior != null)
            {
                host = removeBehavior.Target;
                s_singleBehaviorArray[0] = removeBehavior.Behavior;
                return s_singleBehaviorArray;
            }

            var activateAssist = command as Commands.ActivateAssist;
            if (activateAssist != null)
            {
                host = activateAssist.CardToActivate;
                return activateAssist.CardToActivate.Behaviors;
            }

            var deactivateAssist = command as Commands.DeactivateAssist;
            if (deactivateAssist != null)
            {
                host = deactivateAssist.CardToDeactivate;
                return deactivateAssist.CardToDeactivate.Behaviors;
            }

            host = null;
            return null;
        }
    }
}
