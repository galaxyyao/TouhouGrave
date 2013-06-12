﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace TouhouSpring.Commands
{
    [Serializable]
    public class CommandValidationFailException : Exception
    {
        public CommandValidationFailException()
        { }

        public CommandValidationFailException(string message)
            : base(message)
        { }

        public CommandValidationFailException(string message, Exception innerException)
            : base(message, innerException)
        { }

        protected CommandValidationFailException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }

    public abstract partial class BaseCommand
    {
        public static void FailValidation(string format, params object[] args)
        {
            throw new CommandValidationFailException(String.Format(CultureInfo.CurrentCulture, format, args));
        }

        internal bool DefaultValidateOnRun(ResolveContext ctx)
        {
            var causeBhv = Cause as Behaviors.IBehavior;
            if (causeBhv != null)
            {
                var causeBhvHost = causeBhv.Host;
                if (causeBhvHost == null || causeBhvHost.IsDestroyed || !causeBhvHost.Owner.CardsOnBattlefield.Contains(causeBhvHost))
                {
                    return false;
                }
            }
            if (this is IInitiativeCommand)
            {
                if (!ctx.CheckCompulsoryTargets())
                {
                    return false;
                }
            }
            return true;
        }

        protected void Validate(Player player)
        {
            if (player == null)
            {
                FailValidation("Player can't be null.");
            }
            else if (!Context.Game.Players.Contains(player))
            {
                FailValidation("The player is not registered in game.");
            }
        }

        protected void Validate(CardInstance card)
        {
            if (card == null)
            {
                FailValidation("Card can't be null.");
            }
            else
            {
                Validate(card.Owner);
            }
        }

        protected void Validate(Behaviors.IBehavior behavior)
        {
            if (behavior == null)
            {
                FailValidation("Behavior to be added can't be null.");
            }
            else if (behavior.Host == null)
            {
                FailValidation("Behavior hasn't been bound.");
            }

            Validate(behavior.Host);
        }

        protected void ValidateOrNull(Behaviors.IBehavior behavior)
        {
            if (behavior != null)
            {
                Validate(behavior);
            }
        }
    }
}
