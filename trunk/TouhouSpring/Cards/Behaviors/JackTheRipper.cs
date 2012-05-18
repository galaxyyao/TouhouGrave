using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Triggers;

namespace TouhouSpring.Behaviors
{
	public class JackTheRipper : BaseBehavior<JackTheRipper.ModelType> ,ITrigger<PostPlayerDamagedContext>
	{
		public void Trigger(PostPlayerDamagedContext context)
		{
			if (context.Cause == Host.Behaviors.Get<Warrior>())
			{
				Host.Behaviors.Get<Warrior>().Attack.AddModifierToTail(value => value + 1);
			}
		}

        [BehaviorModel("开膛手杰克", typeof(JackTheRipper))]
        public class ModelType : BehaviorModel
        { }
	}
}
