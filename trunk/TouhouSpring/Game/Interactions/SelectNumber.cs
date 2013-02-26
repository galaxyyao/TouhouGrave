using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Interactions
{
    public class SelectNumber : BaseInteraction
    {
        public Player Player
        {
            get; private set;
        }

        public string Message
        {
            get; private set;
        }

        public int Minimum
        {
            get; private set;
        }

        public int Maximum
        {
            get; private set;
        }

        public SelectNumber(Player player, int min, int max)
            : this(player, min, max, null)
        { }

        public SelectNumber(Player player, int min, int max, string message)
            : base(player.Game)
        {
            if (player == null)
            {
                throw new ArgumentNullException("player");
            }
            else if (max < min)
            {
                throw new ArgumentOutOfRangeException("max", "max must be greater than or equal to min.");
            }

            Player = player;
            Minimum = min;
            Maximum = max;
            Message = message ?? String.Empty;
        }

        public virtual int? Run()
        {
            var result = NotifyAndWait<int?>();
            Validate(result);
            return result;
        }

        public virtual void Respond(int? number)
        {
            Validate(number);
            RespondBack(number);
        }

        protected void Validate(int? number)
        {
            if (number != null)
            {
                int n = number.Value;
                if (n < Minimum || n > Maximum)
                {
                    throw new InteractionValidationFailException("Selected number is out of range.");
                }
            }
        }
    }
}
