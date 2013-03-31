using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.UI
{
    class KeyboardInputManager : EventListener,
        IEventListener<KeyPressedEventArgs>,
        IEventListener<KeyReleasedEventArgs>
    {
        private List<IFocusable> m_focusableItems = new List<IFocusable>();
        private Ime.ImeContext m_imeContext;

        public IFocusable Focus
        {
            get; private set;
        }

        public KeyboardInputManager(Ime.ImeContext imeContext)
        {
            m_imeContext = imeContext;
            m_imeContext.OnChar += new Ime.KeyMessageHandler(ImeContext_OnChar);
            m_imeContext.OnInputLangChange += new Ime.InputLangChangeHandler(ImeContext_OnInputLangChange);
            m_imeContext.OnComposition += new Ime.CompositionMessageHandler(ImeContext_OnComposition);
            m_imeContext.OnEndComposition += new Ime.EndCompositionMessageHandler(ImeContext_OnEndComposition);
        }

        public void RaiseEvent(KeyPressedEventArgs e)
        {
            int focusIndex = m_focusableItems.IndexOf(Focus);

            if (focusIndex == -1)
            {
                SetFocus(null);
            }

            if (e.KeyPressed == (char)Microsoft.Xna.Framework.Input.Keys.Tab)
            {
                if (focusIndex != -1)
                {
                    int delta = e.KeyboardState.KeyPressed[(int)Microsoft.Xna.Framework.Input.Keys.LeftShift]
                                || e.KeyboardState.KeyPressed[(int)Microsoft.Xna.Framework.Input.Keys.RightShift]
                                ? -1 : 1;
                    var newIndex = focusIndex + delta;
                    if (newIndex == m_focusableItems.Count)
                    {
                        newIndex = 0;
                    }
                    else if (newIndex == -1)
                    {
                        newIndex = m_focusableItems.Count - 1;
                    }
                    SetFocus(m_focusableItems[newIndex]);
                }
                else if (m_focusableItems.Count > 0)
                {
                    SetFocus(m_focusableItems[0]);
                }
            }
            else
            {
                Focus.OnFocusedKeyPressed(e);
            }

            m_focusableItems.Clear();
        }

        public void RaiseEvent(KeyReleasedEventArgs e)
        {
            int focusIndex = m_focusableItems.IndexOf(Focus);

            if (focusIndex == -1)
            {
                SetFocus(null);
            }

            if (Focus != null)
            {
                Focus.OnFocusedKeyReleased(e);
            }

            m_focusableItems.Clear();
        }

        // called each time a focusable item receives KeyPressed or KeyReleased event
        // to let KeyboardInputManager collect them
        public void RegisterFocusable(IFocusable item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            else if (m_focusableItems.Contains(item))
            {
                throw new InvalidOperationException("Item already registered.");
            }

            m_focusableItems.Add(item);
        }

        private void ImeContext_OnChar(char code)
        {
            if (Char.IsControl(code))
            {
                return;
            }
            else if (code == '\t')
            {
                // we ignore tab
            }

            if (Focus is ITextReceiver)
            {
                (Focus as ITextReceiver).OnChar(code);
            }
        }

        private void ImeContext_OnInputLangChange(string lang)
        {
            if (Focus is ITextReceiver && (Focus as ITextReceiver).ImeEnabled)
            {
                (Focus as ITextReceiver).OnInputLanguageChange(lang);
            }
        }

        private void ImeContext_OnComposition(string compositionString, Ime.ClauseAttribute[] attr, int cursorPos)
        {
            if (Focus is ITextReceiver && (Focus as ITextReceiver).ImeEnabled)
            {
                (Focus as ITextReceiver).OnComposition(compositionString, attr, cursorPos);
            }
        }

        private void ImeContext_OnEndComposition()
        {
            if (Focus is ITextReceiver && (Focus as ITextReceiver).ImeEnabled)
            {
                (Focus as ITextReceiver).OnEndComposition();
            }
        }

        private void SetFocus(IFocusable newFocus)
        {
            if (newFocus != Focus)
            {
                if (Focus != null)
                {
                    Focus.OnLostFocus();
                    if ((Focus is ITextReceiver) && (Focus as ITextReceiver).ImeEnabled)
                    {
                        m_imeContext.EndIme();
                    }
                }

                Focus = newFocus;
                if (Focus != null)
                {
                    if ((Focus is ITextReceiver) && (Focus as ITextReceiver).ImeEnabled)
                    {
                        m_imeContext.BeginIme();
                        (Focus as ITextReceiver).OnInputLanguageChange(m_imeContext.IndicatorString);
                    }
                    Focus.OnGotFocus();
                }
            }
        }
    }
}
