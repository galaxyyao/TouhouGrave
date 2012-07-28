using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    public interface ICommand
    {
        string Token { get; }
        void Validate(Game game);
        void RunMain(Game game);
    }

    public class CommandValidationFailException : Exception
    {
        public CommandValidationFailException()
        { }

        public CommandValidationFailException(string message)
            : base(message)
        { }
    }
}
