using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Services
{
    [LifetimeDependency(typeof(GameManager))]
    class CardDataManager : GameService
    {
        public enum CardLocation
        {
            Offboard,
            Hand,
            Sacrifice,
            Assist,
            Battlefield,
            Graveyard
        }

        public interface ICounterData
        {
            string Name { get; }
            string IconUri { get; }
            int Count { get; }
        }

        public interface ICardData
        {
            int Guid { get; }
            CardLocation Location { get; }
            int LocationIndex { get; }
            int OwnerPlayerIndex { get; }
            string ModelName { get; }
            string Description { get; }
            string ArtworkUri { get; }
            int SummonCost { get; }
            bool IsWarrior { get; }
            Tuple<int, int> AttackAndInitialAttack { get; }
            Tuple<int, int> LifeAndInitialLife { get; }
            bool IsWarriorCoolingDown { get; }
            bool IsAssist { get; }
            bool IsAssistActivated { get; }
            bool IsInstant { get; }

            IIndexable<ICounterData> Counters { get; }
        }

        private class InternalCounterData : ICounterData
        {
            public string Name { get; private set; }
            public string IconUri { get; private set; }
            public int Count { get; private set; }

            public InternalCounterData(string name, string iconUri, int count)
            {
                Name = name;
                IconUri = iconUri;
                Count = count;
            }
        }

        private class InternalCardData : ICardData
        {
            public int Guid { get; private set; }
            public CardLocation Location { get; private set; }
            public int LocationIndex { get; private set; }
            public int OwnerPlayerIndex { get; private set; }
            public string ModelName { get; private set; }
            public string Description { get; private set; }
            public string ArtworkUri { get; private set; }
            public int SummonCost { get; private set; }
            public bool IsWarrior { get; private set; }
            public Tuple<int, int> AttackAndInitialAttack { get; private set; }
            public Tuple<int, int> LifeAndInitialLife { get; private set; }
            public bool IsWarriorCoolingDown { get; private set; }
            public bool IsAssist { get; private set; }
            public bool IsAssistActivated { get; private set; }
            public bool IsInstant { get; private set; }
            public IIndexable<ICounterData> Counters { get; private set; }

            public InternalCardData(CardInstance card, CardLocation location, int locationIndex)
            {
                Guid = card.Guid;
                Location = location;
                LocationIndex = locationIndex;

                OwnerPlayerIndex = card.Owner.Index;
                ModelName = card.Model.Name ?? String.Empty;
                Description = card.Model.Description ?? String.Empty;
                ArtworkUri = !String.IsNullOrEmpty(card.Model.ArtworkUri) ? card.Model.ArtworkUri : "";

                var manaCost = card.Behaviors.Get<Behaviors.ManaCost>();
                SummonCost = manaCost != null ? manaCost.Cost : -1;

                var warrior = card.Warrior;
                IsWarrior = warrior != null;
                AttackAndInitialAttack = new Tuple<int, int>(warrior != null ? warrior.Attack : -1, warrior != null ? warrior.InitialAttack : -1);
                LifeAndInitialLife = new Tuple<int, int>(warrior != null ? warrior.Life : -1, warrior != null ? warrior.InitialLife : -1);
                IsWarriorCoolingDown = warrior != null && warrior.State == Behaviors.WarriorState.CoolingDown;

                IsAssist = card.Behaviors.Has<Behaviors.Assist>();
                IsAssistActivated = card.IsActivatedAssist;

                IsInstant = card.Behaviors.Has<Behaviors.Instant>();

                var counters = card.Counters;
                var statusEffects = card.Behaviors.OfType<Behaviors.IStatusEffect>();
                var counterArray = new InternalCounterData[counters.Count() + statusEffects.Count()];
                int i = 0;
                foreach (var counter in counters)
                {
                    counterArray[i++] = new InternalCounterData(counter.Text, counter.IconUri, card.GetCounterCount(counter.GetType()));
                }
                foreach (var statusEffect in statusEffects)
                {
                    counterArray[i++] = new InternalCounterData(statusEffect.Text, statusEffect.IconUri, 0);
                }
                Counters = counterArray.ToIndexable();
            }

            public InternalCardData(int ownerPlayerIndex)
            {
                Guid = -1;
                OwnerPlayerIndex = ownerPlayerIndex;
            }
        }

        private Dictionary<int, InternalCardData> m_cards = new Dictionary<int, InternalCardData>();
        private GameEvaluator m_evaluator;

        public IEnumerable<ICardData> Collection
        {
            get { return m_cards.Values; }
        }

        public ICardData TryGetCardData(int cardGuid)
        {
            InternalCardData data;
            return m_cards.TryGetValue(cardGuid, out data) ? data : null;
        }

        public ICardData CreateDummyCardData(int ownerPlayerIndex)
        {
            return new InternalCardData(ownerPlayerIndex);
        }

        public override void Startup()
        {
            m_evaluator = GameApp.Service<GameManager>().CreateGameEvaluator(game => OnGameEvaluate(game));
        }

        private void OnGameEvaluate(Game game)
        {
            var newCards = new Dictionary<int, InternalCardData>();

            foreach (var cardAndLocation in EnumerateGameCards(game))
            {
                var card = cardAndLocation.Item1;
                newCards.Add(card.Guid, new InternalCardData(card, cardAndLocation.Item2, cardAndLocation.Item3));
            }

            m_cards = newCards;
        }

        private IEnumerable<Tuple<CardInstance, CardLocation, int>> EnumerateGameCards(Game game)
        {
            foreach (var player in game.Players)
            {
                foreach (var card in player.CardsOnHand)
                {
                    yield return new Tuple<CardInstance, CardLocation, int>(
                        card,
                        CardLocation.Hand,
                        player.CardsOnHand.IndexOf(card));
                }
                foreach (var card in player.CardsSacrificed)
                {
                    yield return new Tuple<CardInstance, CardLocation, int>(
                        card,
                        CardLocation.Sacrifice,
                        player.CardsSacrificed.IndexOf(card));
                }
                foreach (var card in player.CardsOnBattlefield)
                {
                    yield return new Tuple<CardInstance, CardLocation, int>(
                        card,
                        CardLocation.Battlefield,
                        player.CardsOnBattlefield.IndexOf(card));
                }
                foreach (var card in player.Assists)
                {
                    yield return new Tuple<CardInstance, CardLocation, int>(
                        card,
                        CardLocation.Assist,
                        player.Assists.IndexOf(card));
                }
            }
        }
    }
}
