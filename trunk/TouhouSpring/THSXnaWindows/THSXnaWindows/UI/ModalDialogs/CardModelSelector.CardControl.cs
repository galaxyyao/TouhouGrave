using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaMatrix = Microsoft.Xna.Framework.Matrix;

namespace TouhouSpring.UI.ModalDialogs
{
    partial class CardModelSelector : CardControlAddins.SelectorLocationAnimation.IWindow
    {
        private class CardModelData : Services.CardDataManager.ICardData
        {
            private ICardModel m_cardModel;

            public ICardModel CardModel
            {
                get { return m_cardModel; }
                set
                {
                    if (m_cardModel != value)
                    {
                        m_cardModel = value;
                        CardModelChanged();
                    }
                }
            }

            public int Guid { get { return -1; } }
            public Services.CardDataManager.CardLocation Location { get { return Services.CardDataManager.CardLocation.Offboard; } }
            public int LocationIndex { get { return -1; } }
            public int OwnerPlayerIndex { get { return -1; } }
            public string ModelName { get { return CardModel.Name; } }
            public string Description { get { return CardModel.Description; } }
            public string ArtworkUri { get { return CardModel.ArtworkUri; } }
            public int SummonCost { get; private set; }
            public bool IsWarrior { get; private set; }
            public Tuple<int, int> AttackAndInitialAttack { get; private set; }
            public Tuple<int, int> LifeAndInitialLife { get; private set; }
            public bool IsWarriorCoolingDown { get { return false; } }
            public bool IsAssist { get; private set; }
            public bool IsAssistActivated { get { return false; } }
            public bool IsInstant { get; private set; }
            public IIndexable<Services.CardDataManager.ICounterData> Counters { get { return Indexable.Empty<Services.CardDataManager.ICounterData>(); } }

            private void CardModelChanged()
            {
                var manaCost = m_cardModel.Behaviors.FirstOrDefault(bm => bm is Behaviors.ManaCost.ModelType);
                SummonCost = manaCost != null ? (manaCost as Behaviors.ManaCost.ModelType).Cost : 0;
                var warrior = m_cardModel.Behaviors.FirstOrDefault(bm => bm is Behaviors.Warrior.ModelType) as Behaviors.Warrior.ModelType;
                IsWarrior = warrior != null;
                if (IsWarrior)
                {
                    AttackAndInitialAttack = new Tuple<int, int>(warrior.Attack, warrior.Attack);
                    LifeAndInitialLife = new Tuple<int, int>(warrior.Life, warrior.Life);
                }
                else
                {
                    AttackAndInitialAttack = LifeAndInitialLife = new Tuple<int, int>(0, 0);
                }
                IsAssist = m_cardModel.Behaviors.Any(bm => bm is Behaviors.Assist.ModelType);
                IsInstant = m_cardModel.Behaviors.Any(bm => bm is Behaviors.Instant.ModelType);
            }
        }

        private const int VisibleItems = 7;
        private CardControl[] m_cardControls = new CardControl[VisibleItems];

        public void AddCardControl(ICardModel cardModel)
        {
            var cardStyle = GameApp.Service<Services.Styler>().GetCardStyle("Normal");

            for (int i = 0; i < m_cardControls.Length; ++i)
            {
                var ccStyle = new Style.CardControlStyle(cardStyle, -1);
                ccStyle.Initialize();
                
                var cardControl = ccStyle.TypedTarget;
                cardControl.CardData = new CardModelData { CardModel = cardModel };
                cardControl.Addins.Add(new CardControlAddins.SelectorLocationAnimation(cardControl, i, this));
                cardControl.SetCardDesign("Full");
                cardControl.Dispatcher = m_cardContainer;

                m_cardControls[i] = cardControl;
            }
        }

        public XnaMatrix GetLocation(int index)
        {
            const int HalfWindow = (VisibleItems - 1) / 2;
            const float Interval = 1f;
            const float ShrinkRatio = 0.7f;

            int diff = index - Center;
            int absDiff = Math.Abs(diff);
            int signDiff = Math.Sign(diff);

            var s = (float)Math.Pow(ShrinkRatio, Math.Min(absDiff, HalfWindow));
            var t = (1 - s) / (1 - ShrinkRatio);

            if (absDiff > HalfWindow)
            {
                t += (absDiff - HalfWindow) * (float)Math.Pow(ShrinkRatio, HalfWindow) * (0.5f + 0.5f / ShrinkRatio);
            }

            t *= Interval * signDiff;

            return MatrixHelper.Translate(-0.5f, 0.5f * (300.0f / 210.0f)) * MatrixHelper.Scale(s, s) * MatrixHelper.Translate(t, 0);
        }
    }
}
