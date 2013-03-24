using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.UI
{
    partial class CardControl
    {
        public enum CardClass
        {
            Warrior,
            Instant,
            Assist,
            Hero,
            Enigma
        }

        public interface ICardData
        {
            int OwnerPlayerIndex { get; }
            CardClass SystemClass { get; }
            string ModelName { get; }
            string Description { get; }
            string ArtworkUri { get; }
            int SummonCost { get; }
            Tuple<int, int> AttackAndInitialAttack { get; }
            Tuple<int, int> LifeAndInitialLife { get; }
            bool IsCoolingDown { get; }
        }

        private class InternalCardData : ICardData
        {
            public int OwnerPlayerIndex { get; set; }
            public CardClass SystemClass { get; set; }
            public string ModelName { get; set; }
            public string Description { get; set; }
            public string ArtworkUri { get; set; }
            public int SummonCost { get; set; }
            public Tuple<int, int> AttackAndInitialAttack { get; set; }
            public Tuple<int, int> LifeAndInitialLife { get; set; }
            public bool IsCoolingDown { get; set; }

            public InternalCardData()
            {
                OwnerPlayerIndex = -1;
                SystemClass = CardClass.Enigma;
                ModelName = "";
                Description = "";
                ArtworkUri = "";
                SummonCost = -1;
                AttackAndInitialAttack = new Tuple<int, int>(-1, -1);
                LifeAndInitialLife = new Tuple<int, int>(-1, -1);
                IsCoolingDown = false;
            }
        }

        private InternalCardData m_cardData = new InternalCardData();

        public ICardData CardData
        {
            get { return m_cardData; }
        }

        public void OnEvaluate(CardInstance card)
        {
            m_cardData.OwnerPlayerIndex = card.Owner.Index;
            m_cardData.ModelName = card.Model.Name ?? String.Empty;
            m_cardData.Description = card.Model.Description ?? String.Empty;
            m_cardData.ArtworkUri = !String.IsNullOrEmpty(card.Model.ArtworkUri) ? card.Model.ArtworkUri : "";

            var manaCost = card.Behaviors.Get<Behaviors.ManaCost>();
            m_cardData.SummonCost = manaCost != null ? manaCost.Cost : -1;

            var warrior = card.Behaviors.Get<Behaviors.Warrior>();
            m_cardData.AttackAndInitialAttack = new Tuple<int, int>(warrior != null ? warrior.Attack : -1, warrior != null ? warrior.InitialAttack : -1);
            m_cardData.LifeAndInitialLife = new Tuple<int, int>(warrior != null ? warrior.Life : -1, warrior != null ? warrior.InitialLife : -1);
            m_cardData.IsCoolingDown = warrior != null && warrior.State == Behaviors.WarriorState.CoolingDown;

            if (warrior != null)
            {
                m_cardData.SystemClass = CardClass.Warrior;
            }
            else if (card.Behaviors.Has<Behaviors.Instant>())
            {
                m_cardData.SystemClass = CardClass.Instant;
            }
            else if (card.Behaviors.Has<Behaviors.Assist>())
            {
                m_cardData.SystemClass = CardClass.Assist;
            }
            else if (card.Behaviors.Has<Behaviors.Hero>())
            {
                m_cardData.SystemClass = CardClass.Hero;
            }
            else
            {
                m_cardData.SystemClass = CardClass.Enigma;
            }
        }
    }
}
