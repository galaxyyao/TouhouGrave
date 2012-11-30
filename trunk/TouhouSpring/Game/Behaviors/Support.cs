using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Commands;

namespace TouhouSpring.Behaviors
{
    public class Support : BaseBehavior<Support.ModelType>,
        ISetupTrigger<PlayCard>,
        IPrologTrigger<PlayCard>
    {
        private bool m_chargeSkill = false;

        CommandResult ISetupTrigger<PlayCard>.Run(CommandContext<PlayCard> context)
        {
            if (context.Command.CardToPlay == Host)
            {
                //TODO: add logic (what kind of logic?)
                var cardsOnBattlefield = context.Game.PlayerPlayer.CardsOnBattlefield;
                bool hasSupportOnBattlefield = cardsOnBattlefield.Any(card => card.Behaviors.Get<Behaviors.Support>() != null);
                if (hasSupportOnBattlefield)
                {
                    var result = new Interactions.MessageBox(context.Game.PlayerController
                        , "场上已有一张支援卡，要直接从手牌补魔么？"
                        , Interactions.MessageBox.Button.Yes | Interactions.MessageBox.Button.No).Run();
                    if (result == Interactions.MessageBox.Button.Yes)
                    {
                        m_chargeSkill = true;
                    }
                    else
                    {
                        return CommandResult.Cancel();
                    }
                }
            }

            return CommandResult.Pass;
        }

        void IPrologTrigger<PlayCard>.Run(CommandContext<PlayCard> context)
        {
            if (context.Command.CardToPlay == Host && m_chargeSkill)
            {
                throw new NotImplementedException();
                // TODO: issue commands for doing the following:
                //context.Game.PlayerPlayer.IsSkillCharged = true;
                //context.CardToPlay.Behaviors.Add(new Instant());
            }
        }

        [BehaviorModel(typeof(Support), Category = "Core", Description = "The card is served as the support character.")]
        public class ModelType : BehaviorModel
        { }
    }
}
