using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public enum WarriorState
    {
        StandingBy,
        CoolingDown
    }

    public struct WarriorMessage
    {
        public const int GoCoolingDown          = 0;
        public const int GoStandingBy           = 1;
        public const int AddAttackModifier      = 2;
        public const int RemoveAttackModifier   = 3;
        public const int ResetMaxLife           = 4;
    }

    public sealed partial class Warrior : BaseBehavior<Warrior.ModelType>,
        Commands.ICause,
        ILocalPrerequisiteTrigger<Commands.IInitiativeMoveCard>
    {
        private List<ValueModifier> m_attackModifiers = new List<ValueModifier>();

        public WarriorState State
        {
            get; private set;
        }

        public int Attack
        {
            get; private set;
        }

        // set by DealDamageToCard
        public int Life
        {
            get; internal set;
        }

        public int InitialAttack
        {
            get { return Model.Attack; }
        }

        public int InitialLife
        {
            get { return Model.Life; }
        }

        public int MaxLife
        {
            get; internal set;
        }

        public CommandResult RunLocalPrerequisite(Commands.IInitiativeMoveCard command)
        {
            if (command.ToZoneType == ZoneType.OnBattlefield
                && command.FromZoneType != ZoneType.OnBattlefield)
            {
                if (Model.Unique && Host.Owner.CardsOnBattlefield.Any(card => card.Model == Host.Model))
                {
                    return CommandResult.Cancel();
                }
            }
            return CommandResult.Pass;
        }

        protected override void OnInitialize()
        {
            State = WarriorState.StandingBy;
            Attack = Model.Attack;
            Life = Model.Life;
            MaxLife = Life;
        }

        protected override void OnTransferFrom(IBehavior original)
        {
            var warrior = original as Warrior;
            State = warrior.State;
            Attack = warrior.Attack;
            Life = warrior.Life;
            MaxLife = warrior.MaxLife;
            for (int i = 0; i < warrior.m_attackModifiers.Count; ++i)
            {
                // modifiers are immutable objects
                // thus sharing them is safe
                m_attackModifiers.Add(warrior.m_attackModifiers[i]);
            }
        }

        protected override void OnMessage(int messageId, object arg)
        {
            switch (messageId)
            {
                case WarriorMessage.GoCoolingDown:
                    {
                        if (arg != null)
                        {
                            throw new ArgumentException("Warrior::GoCoolingDown: arg must be null.");
                        }
                        State = WarriorState.CoolingDown;
                    }
                    break;
                case WarriorMessage.GoStandingBy:
                    {
                        if (arg != null)
                        {
                            throw new ArgumentException("Warrior::GoStandingBy: arg must be null.");
                        }
                        State = WarriorState.StandingBy;
                    }
                    break;
                case WarriorMessage.AddAttackModifier:
                    {
                        var mod = arg as ValueModifier;
                        if (mod == null)
                        {
                            throw new ArgumentException("Warrior::AddAttackModifier: arg must be ValueModifier.");
                        }
                        m_attackModifiers.Add(mod);
                        Attack = m_attackModifiers.Aggregate(InitialAttack, (i, v) => v.Process(i));
                    }
                    break;
                case WarriorMessage.RemoveAttackModifier:
                    {
                        var mod = arg as ValueModifier;
                        if (mod == null)
                        {
                            throw new ArgumentException("Warrior::AddAttackModifier: arg must be ValueModifier.");
                        }
                        m_attackModifiers.Remove(mod);
                        Attack = m_attackModifiers.Aggregate(InitialAttack, (i, v) => v.Process(i));
                    }
                    break;
                case WarriorMessage.ResetMaxLife:
                    {
                        if (arg.GetType() != typeof(int))
                        {
                            throw new ArgumentException("Warrior::ResetMaxLife: arg must be int.");
                        }
                        int value = (int)arg;
                        if (value <= 0)
                        {
                            throw new ArgumentException("Warrior::ResetMaxLife: arg must be greater than zero.");
                        }
                        MaxLife = (int)arg;
                        Life = MaxLife;
                    }
                    break;
                default:
                    throw new NotSupportedException("Unknown message.");
            }
        }

        [BehaviorModel(typeof(Warrior), Category = "Core", Description = "The card is capable of being engaged into combats.")]
        public class ModelType : BehaviorModel
        {
            public bool Unique { get; set; }
            public int Attack { get; set; }
            public int Life { get; set; }

            public ModelType()
            {
                Unique = true;
            }
        }
    }
}
