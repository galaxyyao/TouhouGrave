using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaColor = Microsoft.Xna.Framework.Color;

namespace TouhouSpring.Services
{
    partial class GameUI
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

        public void BlockPhase_Enter(Interactions.BlockPhase io)
        {
            m_declaredBlockers = new UI.CardControl[io.DeclaredAttackers.Count][];
            io.DeclaredAttackers.Count.Repeat(i => m_declaredBlockers[i] = new UI.CardControl[2]);

            SetNextButton(NextButton.Skip);
        }

        public void BlockerPhase_ClearSelected()
        {
            m_selectedCardToPlay = null;
        }

        private void BlockPhase_Leave()
        {
            var io = InteractionObject as Interactions.BlockPhase;

            foreach (var card in io.DeclaredAttackers)
            {
                UI.CardControl cc;
                TryGetCardControl(card, out cc);
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

            m_selectedBlocker = null;
            m_declaredBlockers = null;
        }

        private void BlockPhase_OnCardClicked(UI.CardControl control, Interactions.BlockPhase io)
        {
            var card = control.Card;

            if (io.PlayableCandidates.Contains(card))
            {
                m_selectedCardToPlay = m_selectedCardToPlay == control ? null : control;
                m_selectedBlocker = null;
            }
            else if (io.BlockerCandidates.Contains(card))
            {
                m_selectedCardToPlay = null;
                m_selectedBlocker = m_selectedBlocker == control ? null : control;
            }
            else if (io.DeclaredAttackers.Contains(card) && m_selectedBlocker != null)
            {
                var attackerIndex = io.DeclaredAttackers.IndexOf(control.Card);
                var nullIndex = Array.IndexOf(m_declaredBlockers[attackerIndex], null);
                bool toggleBlockOff = false;

                for (int i = 0; i < m_declaredBlockers.Length; ++i)
                {
                    var blockerIndex = Array.IndexOf(m_declaredBlockers[i], m_selectedBlocker);
                    if (blockerIndex == -1)
                    {
                        continue;
                    }

                    toggleBlockOff = io.DeclaredAttackers[i] == control.Card;

                    // new blockers can be declared (not full) on this attacker
                    if (nullIndex != -1 || toggleBlockOff)
                    {
                        // remove the blocker from other attacker's blocker array
                        m_declaredBlockers[i][blockerIndex] = null;
                        if (m_declaredBlockers[i].All(c => c == null))
                        {
                            UI.CardControl cc;
                            TryGetCardControl(io.DeclaredAttackers[i], out cc);
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
                        = control.GetAddin<UI.CardControlAddins.Glow>().GlowColor
                        = m_markColors[attackerIndex];
                }

                SetNextButton(m_declaredBlockers.Any(ba => ba.Any(b => b != null)) ? NextButton.Done : NextButton.Skip);
            }
        }

        private void BlockPhase_OnNextButton(Interactions.BlockPhase io)
        {
            if (m_selectedCardToPlay != null)
            {
                io.Respond(m_selectedCardToPlay.Card);
            }
            else
            {
                var arr = new IIndexable<BaseCard>[io.DeclaredAttackers.Count];
                arr.Length.Repeat(i => arr[i] = m_declaredBlockers[i].Where(cc => cc != null).Select(cc => cc.Card).ToArray().ToIndexable());
                io.Respond(arr.ToIndexable());
            }
            BlockPhase_Leave();
        }

        private bool BlockPhase_ShouldBeHighlighted(BaseCard card)
        {
            return m_selectedBlocker != null && m_selectedBlocker.Card == card
                   || m_selectedCardToPlay != null && m_selectedCardToPlay.Card == card;
        }
    }
}
