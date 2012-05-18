using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
	public class AliceDollSummon : BaseBehavior<AliceDollSummon.ModelType>, ICastableSpell
	{
		public bool Cast(Game game, out string reason)
		{
			if (Host.Owner.Mana < Model.ManaCost)
			{
				reason = "Insufficient mana";
				return false;
			}
			int aliceDollCount = Model.Amount;
            int cost = Model.ManaCost;
			if (Host.Owner.Mana >= Model.ManaCost + 1)
			{
				var result = new Interactions.MessageBox(game.PlayerController
														 , "是否对召唤家佣人偶进行增幅？"
														 , Interactions.MessageBox.Button.Yes | Interactions.MessageBox.Button.No).Run();
				if (result == Interactions.MessageBox.Button.Yes)
				{
					aliceDollCount++;
					cost++;
				}
			}
            aliceDollCount.Repeat(i =>
            {
                var card = new BaseCard(Model.SummonType.Target, Host.Owner);
                game.PlayCard(card);
            });
			game.UpdateMana(Host.Owner, -cost);
			Host.State = CardState.CoolingDown;
			reason = string.Empty;
			return true;
		}

        [BehaviorModel("召唤家佣人偶", typeof(AliceDollSummon))]
		public class ModelType : BehaviorModel
		{
            public CardModelReference SummonType { get; set; }
            public int ManaCost { get; set; }
            public int Amount { get; set; }
		}
	}
}
