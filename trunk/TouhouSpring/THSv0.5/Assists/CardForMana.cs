using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Behaviors;

namespace TouhouSpring.THSv0_5.Assists
{
    public sealed class CardForMana : BaseBehavior<CardForMana.ModelType>,
        ICastableSpell,
        IGlobalEpilogTrigger<Commands.EndPhase>,
        Commands.ICause
    {
        private bool m_hasEatenThisTurn;

        public CommandResult RunLocalPrerequisite(Commands.CastSpell command)
        {
            if (m_hasEatenThisTurn)
                return CommandResult.Cancel("现在吃饱了！");
            Game.NeedTargets(this,
                Host.Owner.CardsOnHand,
                "指定1张手牌，送入冥界");
            return CommandResult.Pass;
        }

        protected override void OnInitialize()
        {
            m_hasEatenThisTurn = false;
        }

        void IGlobalEpilogTrigger<Commands.EndPhase>.RunGlobalEpilog(Commands.EndPhase command)
        {
            if (Game.ActingPlayer == Host.Owner && command.PreviousPhase == "Upkeep")
            {
                m_hasEatenThisTurn = false;
            }
        }

        public void RunSpell(Commands.CastSpell command)
        {
            Game.QueueCommands(new Commands.AddPlayerMaxMana(Host.Owner, 1, this));
            m_hasEatenThisTurn = true;
        }

        [Behaviors.BehaviorModel(typeof(CardForMana), DefaultName = "食符", Category = "v0.5/Assist")]
        public class ModelType : BehaviorModel
        { }
    }
}
