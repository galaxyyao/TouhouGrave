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
        private const int HalfWindow = (VisibleItems - 1) / 2;
        private List<CardControl> m_cardControlPool = new List<CardControl>();
        private List<CardControl> m_aliveCardControls = new List<CardControl>();

        private CardControl TakeCardControl()
        {
            CardControl cc;
            if (m_cardControlPool.Count != 0)
            {
                cc = m_cardControlPool[m_cardControlPool.Count - 1];
                m_cardControlPool.RemoveAt(m_cardControlPool.Count - 1);
            }
            else
            {
                var cardStyle = GameApp.Service<Services.Styler>().GetCardStyle("Normal");
                var ccStyle = new Style.CardControlStyle(cardStyle, -1);
                ccStyle.Initialize();

                cc = ccStyle.TypedTarget;
                cc.Addins.Add(new CardControlAddins.SelectorLocationAnimation(cc, this));
            }

            return cc;
        }

        private void RecycleCardControl(CardControl cardControl)
        {
            cardControl.Dispatcher = null;
            cardControl.SetCardDesign(null);
            m_cardControlPool.Add(cardControl);
        }

        void ModalDialog.IContent.OnUpdate(float deltaTime)
        {
            for (int i = 0; i < m_aliveCardControls.Count; ++i)
            {
                var cc = m_aliveCardControls[i];
                cc.Update(deltaTime);
                var locationAnim = cc.GetAddin<CardControlAddins.SelectorLocationAnimation>();
                if (Math.Abs(locationAnim.LocationIndex - Center) > HalfWindow + 1)
                {
                    m_aliveCardControls.RemoveAt(i--);
                    RecycleCardControl(cc);
                    System.Diagnostics.Debug.WriteLine("Alive:" + m_aliveCardControls.Count.ToString());
                }
            }

            for (int i = Center - HalfWindow - 1; i <= Center + HalfWindow + 1; i++)
            {
                if (i < 0 || i >= Candidates.Count)
                {
                    continue;
                }

                if (m_aliveCardControls.Any(cc => cc.GetAddin<CardControlAddins.SelectorLocationAnimation>().LocationIndex == i))
                {
                    continue;
                }

                var cardControl = TakeCardControl();
                cardControl.CardData = new CardModelData { CardModel = Candidates[i] };
                cardControl.GetAddin<CardControlAddins.SelectorLocationAnimation>().ResetLocationIndex(i);
                cardControl.SetCardDesign("Full");
                cardControl.Dispatcher = m_cardContainer;
                m_aliveCardControls.Add(cardControl);
                System.Diagnostics.Debug.WriteLine("Alive:" + m_aliveCardControls.Count.ToString());
            }
        }

        public XnaMatrix GetLocation(int index)
        {
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
