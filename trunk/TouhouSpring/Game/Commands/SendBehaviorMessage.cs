using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    public class SendBehaviorMessage : ICommand
    {
        private static readonly Type[] s_simpleTypes = new Type[]
        {
            typeof(int), typeof(string), typeof(float)
        };

        public string Token
        {
            get { return "SendBhvMsg"; }
        }

        // TODO: change to serializable behavior ID
        public Behaviors.IBehavior Target
        {
            get; set;
        }

        public string Message
        {
            get; set;
        }

        public object[] Args
        {
            get; set;
        }

        public void Validate(Game game)
        {
            if (Target == null)
            {
                throw new CommandValidationFailException("Target can't be null.");
            }
            else if (Message == null || Message == String.Empty)
            {
                throw new CommandValidationFailException("Message can't be empty.");
            }
            // TODO: proper serialization of complex data type
            //else if (Args != null && Args.Any(arg => !s_simpleTypes.Contains(arg.GetType())))
            //{
            //    throw new CommandValidationFailException("Arguments can only be of some simple types.");
            //}
        }

        public void RunMain(Game game)
        {
            Target.OnMessage(Message, Args);
        }
    }
}
