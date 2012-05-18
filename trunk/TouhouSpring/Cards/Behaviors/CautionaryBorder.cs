using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class CautionaryBorder : BaseBehavior<CautionaryBorder.ModelType>,
        ITrigger<Triggers.PostPlayerDamagedContext>
    {
        public void Trigger(Triggers.PostPlayerDamagedContext context)
        {
            if (context.Cause is Warrior
                && context.Cause.Host.Owner != Host.Owner
                && Host.Owner.Mana >= Model.ManaCost)
            {
                var playerController = context.Game.PlayerPlayer == Host.Owner ? context.Game.PlayerController : context.Game.OpponentController;
                var choice = new Interactions.MessageBox(playerController,
                    String.Format("警醒阵效果：是否发动并破坏{0}？", context.Cause.Host.Model.Name),
                    Interactions.MessageBox.Button.Yes | Interactions.MessageBox.Button.No).Run();

                if (choice == Interactions.MessageBox.Button.Yes)
                {
                    context.Game.UpdateMana(Host.Owner, -Model.ManaCost);
                    context.Game.DestroyCard(context.Cause.Host);
                    context.Game.DestroyCard(Host);
                }
            }
        }

        [BehaviorModel("Cautionary Border", typeof(CautionaryBorder))]
        public class ModelType : BehaviorModel
        {
            public int ManaCost { get; set; }
        }
    }
}
