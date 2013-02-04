using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
    public partial class Game
    {
        private interface ICommandRunner
        {
            void Run(Commands.BaseCommand command);
            CommandResult RunPrerequisite(Commands.BaseCommand command);
        }

        private class CommandRunner<TCommand> : ICommandRunner
            where TCommand : Commands.BaseCommand
        {
            public void Run(Commands.BaseCommand genericCommand)
            {
                var command = genericCommand as TCommand;
                command.ValidateOnRun();

                ////////////////////////////////////////////

                command.ExecutionPhase = Commands.CommandPhase.Prerequisite;
                foreach (var trigger in EnumerateCommandTargets(command).SelectMany(card =>
                                           card.Behaviors.OfType<IPrerequisiteTrigger<TCommand>>()
                                           ).ToList())
                {
                    var result = trigger.RunPrerequisite(command);
                    if (result.Canceled)
                    {
                        command.Game.Controller.OnCommandCanceled(command, result.Reason);
                        command.Game.ClearConditions();
                        return;
                    }
                }

                ////////////////////////////////////////////

                command.ExecutionPhase = Commands.CommandPhase.Condition;
                var conditionResult = command.Game.ResolveConditions(false);
                if (conditionResult.Canceled)
                {
                    command.Game.Controller.OnCommandCanceled(command, conditionResult.Reason);
                    command.Game.ClearConditions();
                    return;
                }

                ////////////////////////////////////////////

                command.ExecutionPhase = Commands.CommandPhase.Prolog;
                command.Game.Controller.OnCommandBegin(command);
                EnumerateCommandTargets(command).SelectMany(card => card.Behaviors.OfType<IPrologTrigger<TCommand>>())
                    .ToList().ForEach(trigger => trigger.RunProlog(command));

                ////////////////////////////////////////////

                command.ExecutionPhase = Commands.CommandPhase.Main;
                command.RunMain();

                ////////////////////////////////////////////

                command.ExecutionPhase = Commands.CommandPhase.Epilog;
                EnumerateCommandTargets(command).SelectMany(card => card.Behaviors.OfType<IEpilogTrigger<TCommand>>())
                    .ToList().ForEach(trigger => trigger.RunEpilog(command));
                command.Game.Controller.OnCommandEnd(command);

                command.Game.ClearConditions();
            }

            public CommandResult RunPrerequisite(Commands.BaseCommand genericCommand)
            {
                var command = genericCommand as TCommand;
                command.ExecutionPhase = Commands.CommandPhase.Prerequisite;
                foreach (var trigger in EnumerateCommandTargets(command)
                    .SelectMany(card => card.Behaviors.OfType<IPrerequisiteTrigger<TCommand>>()).ToList())
                {
                    var result = trigger.RunPrerequisite(command);
                    if (result.Canceled)
                    {
                        command.Game.ClearConditions();
                        return result;
                    }
                }

                var ret = command.Game.ResolveConditions(true);
                command.Game.ClearConditions();
                return ret;
            }

            private IEnumerable<BaseCard> EnumerateCommandTargets(Commands.BaseCommand command)
            {
                if (command is Commands.PlayCard && command.ExecutionPhase < Commands.CommandPhase.Epilog)
                {
                    var cardToPlay = (command as Commands.PlayCard).CardToPlay;
                    if (cardToPlay != null)
                    {
                        yield return cardToPlay;
                    }
                }
                else if (command is Commands.ActivateAssist && command.ExecutionPhase < Commands.CommandPhase.Epilog)
                {
                    var cardToActivate = (command as Commands.ActivateAssist).CardToActivate;
                    if (cardToActivate != null)
                    {
                        yield return cardToActivate;
                    }
                }
                else if (command is Commands.ActivateAssist && command.ExecutionPhase == Commands.CommandPhase.Epilog)
                {
                    var previouslyActivated = (command as Commands.ActivateAssist).PreviouslyActivatedCard;
                    if (previouslyActivated != null)
                    {
                        yield return previouslyActivated;
                    }
                }
                else if (command is Commands.Redeem)
                {
                    var cardToRedeem = (command as Commands.Redeem).Target;
                    if (cardToRedeem != null)
                    {
                        yield return cardToRedeem;
                    }
                }
                else if (command is Commands.Kill && command.ExecutionPhase == Commands.CommandPhase.Epilog)
                {
                    var cardKilled = (command as Commands.Kill).Target;
                    if (cardKilled != null)
                    {
                        yield return cardKilled;
                    }
                }

                foreach (var player in command.Game.Players)
                {
                    foreach (var card in player.CardsOnBattlefield)
                    {
                        yield return card;
                    }
                    if (player.ActivatedAssist != null)
                    {
                        yield return player.ActivatedAssist;
                    }
                }
            }
        }
    }
}
