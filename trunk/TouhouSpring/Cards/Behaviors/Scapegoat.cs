using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Triggers;
using System.Threading;

namespace TouhouSpring.Behaviors
{
    public class Scapegoat : BaseBehavior<Scapegoat.ModelType>, ITrigger<PreCardDamageContext>
    {
        public void Trigger(PreCardDamageContext context)
        {
			if (context.CardToDamage == Host)
			{
				if (Host.Owner.Mana < Model.ManaCost)
				{
					context.Cancel = true;
					context.Reason = "Insufficient mana";
					return;
				}
				BaseCard aliceDoll = context.Game.PlayerPlayer.CardsOnBattlefield.
					FirstOrDefault(card => card.Model.Id == "alicedoll");
				if (aliceDoll == null)
					return;

				var result = new Interactions.MessageBox(context.Game.PlayerController
														 , "是否施放人偶替身？"
														 , Interactions.MessageBox.Button.Yes | Interactions.MessageBox.Button.No).Run();
				if (result == Interactions.MessageBox.Button.No)
				{
					context.Cancel = false;
					context.Reason = string.Empty;
					return;
				}
				var aliceDollWarriorBhv = aliceDoll.Behaviors.Get<Behaviors.Warrior>();
				aliceDollWarriorBhv.AccumulatedDamage += context.DamageToDeal;
				Host.Behaviors.Get<Warrior>().AccumulatedDamage -= context.DamageToDeal;
				context.Reason = string.Empty;
			}
        }

        [BehaviorModel(typeof(Scapegoat), DefaultName = "人偶替身")]
        public class ModelType : BehaviorModel
        {
            public int ManaCost { get; set; }
        }
    }
}
