using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaColor = Microsoft.Xna.Framework.Color;

namespace TouhouSpring.Services.UIStates
{
    class BlockPhase : IUIState
    {
        private XnaColor[] m_markColors = new XnaColor[]
        {
            XnaColor.Red,
            XnaColor.Orange,
            XnaColor.Yellow,
            XnaColor.Green,
            XnaColor.Cyan,
            XnaColor.Blue,
            XnaColor.Purple,
            XnaColor.DarkRed,
            XnaColor.Magenta,
            XnaColor.Teal
        };

        private UI.CardControl m_selectedCardToPlay;
        private UI.CardControl m_selectedBlocker;
        private UI.CardControl[][] m_declaredBlockers;

        private GameUI m_gameUI = GameApp.Service<GameUI>();
        private Interactions.BlockPhase m_io;

        public IIndexable<BaseCard> DeclaredAttackers
        {
            get { return m_io.DeclaredAttackers; }
        }

        public UI.CardControl[][] DeclaredBlockers
        {
            get { return m_declaredBlockers; }
        }

        public Player Player
        {
            get { return m_io.Player; }
        }

        public Interactions.BaseInteraction InteractionObject
        {
            get { return m_io; }
        }

        public void OnEnter(Interactions.BaseInteraction io)
        {
            m_io = (Interactions.BlockPhase)io;
            m_declaredBlockers = new UI.CardControl[m_io.DeclaredAttackers.Count][];
            m_io.DeclaredAttackers.Count.Repeat(i => m_declaredBlockers[i] = new UI.CardControl[1]);
            m_gameUI.SetContextButtons("Skip");
        }

        public void OnLeave()
        {
            foreach (var card in m_io.DeclaredAttackers)
            {
                UI.CardControl cc;
                m_gameUI.TryGetCardControl(card, out cc);
                cc.GetAddin<UI.CardControlAddins.Glow>().GlowColor = XnaColor.Transparent;
            }
            foreach (var blockerArray in m_declaredBlockers)
            {
                foreach (var cc in blockerArray)
                {
                    if (cc != null)
                    {
                        cc.GetAddin<UI.CardControlAddins.Glow>().GlowColor = XnaColor.Transparent;
                    }
                }
            }
        }

        public void OnCardClicked(UI.CardControl cardControl)
        {
            var card = cardControl.Card;

            if (m_io.PlayableCandidates.Contains(card))
            {
                m_selectedCardToPlay = m_selectedCardToPlay == cardControl ? null : cardControl;
                m_selectedBlocker = null;
            }
            else if (m_io.BlockerCandidates.Contains(card))
            {
                m_selectedCardToPlay = null;
                m_selectedBlocker = m_selectedBlocker == cardControl ? null : cardControl;
            }
            else if (m_io.BlockableAttackers.Contains(card) && m_selectedBlocker != null)
            {
                var attackerIndex = m_io.DeclaredAttackers.IndexOf(card);
                var nullIndex = Array.IndexOf(m_declaredBlockers[attackerIndex], null);
                bool toggleBlockOff = false;

                for (int i = 0; i < m_declaredBlockers.Length; ++i)
                {
                    var blockerIndex = Array.IndexOf(m_declaredBlockers[i], m_selectedBlocker);
                    if (blockerIndex == -1)
                    {
                        continue;
                    }

                    toggleBlockOff = m_io.DeclaredAttackers[i] == card;

                    // new blockers can be declared (not full) on this attacker
                    if (nullIndex != -1 || toggleBlockOff)
                    {
                        // remove the blocker from other attacker's blocker array
                        m_declaredBlockers[i][blockerIndex] = null;
                        if (m_declaredBlockers[i].All(c => c == null))
                        {
                            UI.CardControl cc;
                            m_gameUI.TryGetCardControl(m_io.DeclaredAttackers[i], out cc);
                            cc.GetAddin<UI.CardControlAddins.Glow>().GlowColor = XnaColor.Transparent;
                        }

                        if (toggleBlockOff)
                        {
                            m_selectedBlocker.GetAddin<UI.CardControlAddins.Glow>().GlowColor = XnaColor.Transparent;
                        }
                    }

                    break;
                }

                if (nullIndex != -1 && !toggleBlockOff)
                {
                    m_declaredBlockers[attackerIndex][nullIndex] = m_selectedBlocker;
                    m_selectedBlocker.GetAddin<UI.CardControlAddins.Glow>().GlowColor
                        = cardControl.GetAddin<UI.CardControlAddins.Glow>().GlowColor
                        = m_markColors[attackerIndex];
                }

                m_gameUI.SetContextButtons(m_declaredBlockers.Any(ba => ba.Any(b => b != null)) ? "Done" : "Skip");
            }
        }

        public void OnSpellClicked(UI.CardControl cardControl, Behaviors.ICastableSpell spell)
        {
            throw new InvalidOperationException("Impossible");
        }

        public void OnContextButton(string buttonText)
        {
            if (m_selectedCardToPlay != null)
            {
                m_io.Respond(m_selectedCardToPlay.Card);
            }
            else
            {
                var arr = new IIndexable<BaseCard>[m_io.DeclaredAttackers.Count];
                arr.Length.Repeat(i => arr[i] = m_declaredBlockers[i].Where(cc => cc != null).Select(cc => cc.Card).ToArray().ToIndexable());
                m_io.Respond(arr.ToIndexable());
            }
            m_gameUI.LeaveState();
        }

        public bool IsCardClickable(UI.CardControl cardControl)
        {
            var card = cardControl.Card;
            return m_io.BlockerCandidates.Contains(card)
                   || m_io.PlayableCandidates.Contains(card)
                   || m_io.BlockableAttackers.Contains(card);
        }

        public bool IsCardSelected(UI.CardControl cardControl)
        {
            return cardControl == m_selectedBlocker || cardControl == m_selectedCardToPlay;
        }

        public bool IsCardSelectedForCastSpell(UI.CardControl cardControl)
        {
            return false;
        }
    }
}
