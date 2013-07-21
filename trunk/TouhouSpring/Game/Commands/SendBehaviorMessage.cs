using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    public class SendBehaviorMessage : BaseCommand, ISilentCommand
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

        public int MessageId
        {
            get; private set;
        }

        public object Arg
        {
            get; private set;
        }

        public SendBehaviorMessage(Behaviors.IBehavior target, int messageId, object arg)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            Target = target;
            MessageId = messageId;
            Arg = arg;
        }

        public void PatchMessageArg(object arg)
        {
            CheckPatchable("Arg");
            Arg = arg;
        }

        internal override void ValidateOnIssue()
        {
            Validate(Target);
        }

        internal override bool ValidateOnRun()
        {
            return !Target.Host.IsDestroyed && Target.Host.Behaviors.Contains(Target);
        }

        internal override void RunMain()
        {
            (Target as Behaviors.IInternalBehavior).ReceiveMessage(MessageId, Arg);
        }
    }
}
