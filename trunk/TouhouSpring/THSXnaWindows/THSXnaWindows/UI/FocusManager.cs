using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.UI
{
    class FocusManager : EventListener,
        IEventListener<KeyPressedEventArgs>,
        IEventListener<KeyReleasedEventArgs>
    {
        private List<IFocusable> m_focusableItems = new List<IFocusable>();

        public IFocusable Focus
        {
            get; private set;
        }

        public void RaiseEvent(KeyPressedEventArgs e)
        {
            int focusIndex = m_focusableItems.IndexOf(Focus);

            if (Focus != null && focusIndex == -1)
            {
                Focus.OnLostFocus();
                Focus = null;
            }

            if (e.KeyPressed[(int)Microsoft.Xna.Framework.Input.Keys.Tab])
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
                    var newFocusItem = m_focusableItems[newIndex];
                    if (newFocusItem != Focus)
                    {
                        Focus.OnLostFocus();
                        Focus = newFocusItem;
                        Focus.OnGotFocus();
                    }
                }
                else if (m_focusableItems.Count > 0)
                {
                    Focus = m_focusableItems[0];
                    Focus.OnGotFocus();
                }
            }

            if (Focus != null)
            {
                Focus.OnFocusedKeyPressed(e);
            }

            m_focusableItems.Clear();
        }

        public void RaiseEvent(KeyReleasedEventArgs e)
        {
            int focusIndex = m_focusableItems.IndexOf(Focus);

            if (Focus != null && focusIndex == -1)
            {
                Focus.OnLostFocus();
                Focus = null;
            }

            if (Focus != null)
            {
                Focus.OnFocusedKeyReleased(e);
            }

            m_focusableItems.Clear();
        }

        // called each time a focusable item receives KeyPressed or KeyReleased event
        // to let FocusManager collect them
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
    }

    interface IFocusGroup
    {
        FocusManager FocusManager { get; }
    }
}
