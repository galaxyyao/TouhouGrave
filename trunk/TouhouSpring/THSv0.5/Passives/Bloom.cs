using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Behaviors;

namespace TouhouSpring.THSv0_5.Passives
{
    public sealed class Bloom : BaseBehavior<Bloom.ModelType>,
        IGlobalEpilogTrigger<Commands.IMoveCard>,
        IGlobalEpilogTrigger<Commands.SummonMove>
    {
        private class HimawariSummon : Commands.ICause
        {
            public IBehavior Summoner;
            public int Life;
        }

        void IGlobalEpilogTrigger<Commands.IMoveCard>.RunGlobalEpilog(Commands.IMoveCard command)
        {
            if (command.Subject != null
                && command.Subject.Owner != Host.Owner
                && command.Subject.Warrior != null
                && command.Subject.Warrior.InitialLife > 0
                && command.ToZone == SystemZone.Graveyard)
            {
                Game.QueueCommands(new Commands.SummonMove(
                    Model.HimawariModel.Value,
                    Host.Owner,
                    SystemZone.Battlefield,
                    new HimawariSummon { Summoner = this, Life = command.Subject.Warrior.InitialLife }));
            }
        }

        void IGlobalEpilogTrigger<Commands.SummonMove>.RunGlobalEpilog(Commands.SummonMove command)
        {
            if (command.ToZone == SystemZone.Battlefield
                && command.Subject.Owner == Host.Owner
                && command.Subject.Warrior != null)
            {
                var cause = command.Cause as HimawariSummon;
                if (cause != null && cause.Summoner == this)
                {
                    Game.QueueCommands(new Commands.SendBehaviorMessage(command.Subject.Warrior, WarriorMessage.ResetMaxLife, cause.Life));
                }
            }
        }

        [BehaviorModel(typeof(Bloom), Category = "v0.5/Passive", DefaultName = "花开")]
        public class ModelType : BehaviorModel
        {
            public CardModelReference HimawariModel { get; set; }
        }
    }
}
