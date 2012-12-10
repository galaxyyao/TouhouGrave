using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class Support : BaseBehavior<Support.ModelType>,
        ISetupTrigger<Commands.PlayCard>,
        IPrologTrigger<Commands.PlayCard>
    {
        private bool m_chargeSkill = false;

        CommandResult ISetupTrigger<Commands.PlayCard>.Run(Commands.PlayCard command)
        {
            if (command.CardToPlay == Host)
            {
                //TODO: add logic (what kind of logic?)
                var cardsOnBattlefield = command.Game.PlayerPlayer.CardsOnBattlefield;
                bool hasSupportOnBattlefield = cardsOnBattlefield.Any(card => card.Behaviors.Get<Behaviors.Support>() != null);
                if (hasSupportOnBattlefield)
                {
                    var result = new Interactions.MessageBox(command.Game.PlayerController
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

        void IPrologTrigger<Commands.PlayCard>.Run(Commands.PlayCard command)
        {
            if (command.CardToPlay == Host && m_chargeSkill)
            {
                command.Game.IssueCommands(
                    new Commands.Charge(Host.Owner),
                    new Commands.AddBehavior(command.CardToPlay, new Instant()));
            }
        }

        [BehaviorModel(typeof(Support), Category = "Core", Description = "The card is served as the support character.")]
        public class ModelType : BehaviorModel
        { }
    }
}
