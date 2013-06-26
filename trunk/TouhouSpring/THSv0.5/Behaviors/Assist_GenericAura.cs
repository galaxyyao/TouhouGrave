using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Assist_GenericAura : BaseBehavior<Assist_GenericAura.ModelType>,
        ILocalEpilogTrigger<Commands.ActivateAssist>,
        ILocalEpilogTrigger<Commands.DeactivateAssist>,
        IGlobalEpilogTrigger<Commands.IMoveCard>
    {
        private ValueModifier m_modifier;

        void ILocalEpilogTrigger<Commands.ActivateAssist>.RunLocalEpilog(Commands.ActivateAssist command)
        {
            foreach (var player in Game.Players)
            {
                if (player == Host.Owner && (Model.Affects & ModelType.AuraAffects.Ally) != 0
                    || player != Host.Owner && (Model.Affects & ModelType.AuraAffects.Enemy) != 0)
                {
                    foreach (var card in player.CardsOnBattlefield)
                    {
                        var warrior = card.Behaviors.Get<Warrior>();
                        if (warrior == null)
                            continue;

                        Game.QueueCommands(
                            new Commands.SendBehaviorMessage(warrior, "AttackModifiers", new object[] { "add", m_modifier }));
                    }
                }
            }
        }

        void ILocalEpilogTrigger<Commands.DeactivateAssist>.RunLocalEpilog(Commands.DeactivateAssist command)
        {
            foreach (var player in Game.Players)
            {
                if (player == Host.Owner && (Model.Affects & ModelType.AuraAffects.Ally) != 0
                    || player != Host.Owner && (Model.Affects & ModelType.AuraAffects.Enemy) != 0)
                {
                    foreach (var card in player.CardsOnBattlefield)
                    {
                        var warrior = card.Behaviors.Get<Warrior>();
                        if (warrior == null)
                            continue;

                        Game.QueueCommands(
                            new Commands.SendBehaviorMessage(warrior, "AttackModifiers", new object[] { "remove", m_modifier }));
                    }
                }
            }
        }

        public void RunGlobalEpilog(Commands.IMoveCard command)
        {
            if (Host.IsActivatedAssist && Host != command.Subject
                && (command.Subject.Owner == Host.Owner && (Model.Affects & ModelType.AuraAffects.Ally) != 0
                    || command.Subject.Owner != Host.Owner && (Model.Affects & ModelType.AuraAffects.Enemy) != 0))
            {
                if (command.FromZoneType != ZoneType.OnBattlefield
                    && command.ToZoneType == ZoneType.OnBattlefield)
                {
                    var warrior = command.Subject.Behaviors.Get<Warrior>();
                    if (warrior != null)
                    {
                        Game.QueueCommands(
                            new Commands.SendBehaviorMessage(warrior, "AttackModifiers", new object[] { "add", m_modifier }));
                    }
                }
                else if (command.FromZoneType == ZoneType.OnBattlefield
                         && command.ToZoneType != ZoneType.OnBattlefield)
                {
                    var warrior = command.Subject.Behaviors.Get<Warrior>();
                    if (warrior != null)
                    {
                        Game.QueueCommands(
                            new Commands.SendBehaviorMessage(warrior, "AttackModifiers", new object[] { "remove", m_modifier }));
                    }
                }
            }
        }

        protected override void OnInitialize()
        {
            m_modifier = new ValueModifier(Model.Operator, Model.Amount);
        }

        protected override void OnTransferFrom(IBehavior original)
        {
            m_modifier = (original as Assist_GenericAura).m_modifier;
        }

        [BehaviorModel(typeof(Assist_GenericAura), Category = "v0.5/Assist", DefaultName = "攻击光环（通用）")]
        public class ModelType : BehaviorModel
        {
            public enum AuraAffects
            {
                Ally    = 0x01,
                Enemy   = 0x02,
                Both    = Ally | Enemy,
            }

            public AuraAffects Affects { get; set; }
            public ValueModifierOperator Operator { get; set; }
            public int Amount { get; set; }
        }
    }
}
