using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class GenericAura<T> : BaseBehavior<T>,
        IGlobalEpilogTrigger<Commands.IMoveCard>
        where T : GenericAuraModelType
    {
        void IGlobalEpilogTrigger<Commands.IMoveCard>.RunGlobalEpilog(Commands.IMoveCard command)
        {
            if ((Host.IsActivatedAssist || Host.IsOnBattlefield)
                && Host != command.Subject
                && command.Subject.Warrior != null
                && (command.Subject.Owner == Host.Owner && Model.AffectAlly
                    || command.Subject.Owner != Host.Owner && Model.AffectEnemy))
            {
                if (command.FromZoneType != ZoneType.OnBattlefield
                    && command.ToZoneType == ZoneType.OnBattlefield)
                {
                    EnterAura(command.Subject.Warrior);
                }
                else if (command.FromZoneType == ZoneType.OnBattlefield
                         && command.ToZoneType != ZoneType.OnBattlefield)
                {
                    LeaveAura(command.Subject.Warrior);
                }
            }
        }

        protected void OnBeginEffect()
        {
            foreach (var player in Game.Players)
            {
                if (player == Host.Owner && Model.AffectAlly
                    || player != Host.Owner && Model.AffectEnemy)
                {
                    foreach (var card in player.CardsOnBattlefield)
                    {
                        if (card.Warrior != null
                            && (Model.AffectSelf || card != Host))
                        {
                            EnterAura(card.Warrior);
                        }
                    }
                }
            }
        }

        protected void OnEndEffect()
        {
            foreach (var player in Game.Players)
            {
                if (player == Host.Owner && Model.AffectAlly
                    || player != Host.Owner && Model.AffectEnemy)
                {
                    foreach (var card in player.CardsOnBattlefield)
                    {
                        if (card.Warrior != null
                            && (Model.AffectSelf || card != Host))
                        {
                            LeaveAura(card.Warrior);
                        }
                    }
                }
            }
        }

        private void EnterAura(Warrior warrior)
        {
            Game.QueueCommands(new Commands.SendBehaviorMessage(warrior, WarriorMessage.AddAttackModifier, Model.Modifier));
        }

        private void LeaveAura(Warrior warrior)
        {
            Game.QueueCommands(new Commands.SendBehaviorMessage(warrior, WarriorMessage.RemoveAttackModifier, Model.Modifier));
        }
    }

    public class GenericAuraModelType : BehaviorModel
    {
        [System.ComponentModel.Category("Affects")]
        public bool AffectAlly { get; set; }
        [System.ComponentModel.Category("Affects")]
        public bool AffectEnemy { get; set; }
        [System.ComponentModel.Category("Affects")]
        public bool AffectSelf { get; set; }

        public ValueModifierOperator Operator
        {
            get { return Modifier.Operator; }
            set { Modifier = new ValueModifier(value, Amount, false); }
        }

        public int Amount
        {
            get { return Modifier.Amount; }
            set { Modifier = new ValueModifier(Operator, value, false); }
        }

        [System.ComponentModel.Browsable(false)]
        public ValueModifier Modifier { get; private set; }

        public GenericAuraModelType()
        {
            Modifier = new ValueModifier(ValueModifierOperator.Add, 1);
        }
    }
}
