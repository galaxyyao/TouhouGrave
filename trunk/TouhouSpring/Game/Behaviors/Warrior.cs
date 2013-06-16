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

    public sealed partial class Warrior : BaseBehavior<Warrior.ModelType>,
        Commands.ICause,
        IPrerequisiteTrigger<Commands.IMoveTo<Commands.Battlefield>>,
        IEpilogTrigger<Commands.IMoveFrom<Commands.Battlefield>>
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

        public CommandResult RunPrerequisite(Commands.IMoveTo<Commands.Battlefield> command)
        {
            if (command.Subject == Host)
            {
                if (Model.Unique && Host.Owner.CardsOnBattlefield.Any(card => card.Model == Host.Model))
                {
                    return CommandResult.Cancel();
                }
            }
            return CommandResult.Pass;
        }

        public void RunEpilog(Commands.IMoveFrom<Commands.Battlefield> command)
        {
            if (command.Subject == Host)
            {
                State = WarriorState.StandingBy;
                m_attackModifiers.Clear();
                Attack = Model.Attack;
                Life = Model.Life;
            }
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

        protected override void OnMessage(string message, object[] args)
        {
            if (message == "GoCoolingDown")
            {
                if (args != null)
                {
                    throw new ArgumentException("Formation of args is not expected.");
                }

                State = WarriorState.CoolingDown;
            }
            else if (message == "GoStandingBy")
            {
                if (args != null)
                {
                    throw new ArgumentException("Formation of args is not expected.");
                }

                State = WarriorState.StandingBy;
            }
            else if (message == "AttackModifiers")
            {
                if (args == null || args.Length != 2
                    || args[0].GetType() != typeof(string) || args[1].GetType() != typeof(ValueModifier))
                {
                    throw new ArgumentException("Formation of args is not expected.");
                }
                if ((string)args[0] == "add")
                {
                    var mod = (ValueModifier)args[1];
                    if (m_attackModifiers.Contains(mod))
                    {
                        throw new ArgumentException("The modifier has already been added.");
                    }
                    m_attackModifiers.Add(mod);
                }
                else if ((string)args[0] == "remove")
                {
                    var mod = (ValueModifier)args[1];
                    if (!m_attackModifiers.Contains(mod))
                    {
                        throw new ArgumentException("The modifier has not been added.");
                    }
                    m_attackModifiers.Remove(mod);
                }
                Attack = m_attackModifiers.Aggregate(InitialAttack, (i, v) => v.Process(i));
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
