using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Support : BaseBehavior<Support.ModelType>,
        Commands.ICause,
        ISetupTrigger<Commands.PlayCard>,
        IEpilogTrigger<Commands.PlayCard>
    {
        private bool m_chargeSkill = false;

        CommandResult ISetupTrigger<Commands.PlayCard>.Run(Commands.PlayCard command)
        {
            if (command.CardToPlay == Host)
            {
                //TODO: add logic (what kind of logic?)
                var cardsOnBattlefield = Host.Owner.CardsOnBattlefield;
                bool hasSupportOnBattlefield = cardsOnBattlefield.Any(card => card.Behaviors.Get<Behaviors.Support>() != null);
                if (hasSupportOnBattlefield)
                {
                    var result = new Interactions.MessageBox(Host.Owner,
                        "场上已有一张支援卡，要直接从手牌补魔么？",
                        Interactions.MessageBoxButtons.Yes | Interactions.MessageBoxButtons.No).Run();
                    if (result == Interactions.MessageBoxButtons.Yes)
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

        void IEpilogTrigger<Commands.PlayCard>.Run(Commands.PlayCard command)
        {
            if (command.CardToPlay == Host && m_chargeSkill)
            {
                Game.IssueCommands(
                    new Commands.Charge(Host.Owner),
                    new Commands.Kill(command.CardToPlay, this));
            }
        }

        [BehaviorModel(typeof(Support), Category = "Core", Description = "The card is served as the support character.")]
        public class ModelType : BehaviorModel
        { }
    }
}
