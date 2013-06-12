using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    public class SendBehaviorMessage : BaseCommand
    {
        private static readonly Type[] s_simpleTypes = new Type[]
        {
            typeof(int), typeof(string), typeof(float)
        };

        // TODO: change to serializable behavior ID
        public Behaviors.IBehavior Target
        {
            get; private set;
        }

        public string Message
        {
            get; private set;
        }

        public object[] Args
        {
            get; private set;
        }

        public SendBehaviorMessage(Behaviors.IBehavior target, string message, object[] args)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }
            else if (String.IsNullOrEmpty(message))
            {
                throw new ArgumentNullException("message");
            }

            Target = target;
            Message = message;
            Args = args;
        }

        public void PatchMessageArgs(object[] args)
        {
            CheckPatchable("Args");
            Args = args;
        }

        internal override void ValidateOnIssue()
        {
            Validate(Target);
            if (String.IsNullOrEmpty(Message))
            {
                FailValidation("Message can't be empty.");
            }
        }

        internal override bool ValidateOnRun()
        {
            return !Target.Host.IsDestroyed && Target.Host.Behaviors.Contains(Target);
        }

        internal override void RunMain()
        {
            (Target as Behaviors.IInternalBehavior).ReceiveMessage(Message, Args);
        }
    }
}
