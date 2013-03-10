using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.UI
{
    interface IFocusable
    {
        void OnGotFocus();
        void OnLostFocus();
        void OnFocusedKeyPressed(KeyPressedEventArgs e);
        void OnFocusedKeyReleased(KeyPressedEventArgs e);
    }

    class FocusableProxy : EventListener,
        IEventListener<KeyPressedEventArgs>,
        IEventListener<KeyReleasedEventArgs>
    {
        public EventDispatcher Target
        {
            get; private set;
        }

        public FocusableProxy(EventDispatcher target)
        {
            if (!(target is IFocusable))
            {
                throw new ArgumentException("Target must be an IFocusable.");
            }

            Target = target;
            Dispatcher = target;
        }

        public void RaiseEvent(KeyPressedEventArgs e)
        {
            RegisterToFocusManager();
        }

        public void RaiseEvent(KeyReleasedEventArgs e)
        {
            RegisterToFocusManager();
        }

        private void RegisterToFocusManager()
        {
            for (var i = Target; i != null; i = i.Dispatcher)
            {
                if (i is IFocusGroup)
                {
                    var focusManager = (i as IFocusGroup).FocusManager;
                    focusManager.RegisterFocusable(Target as IFocusable);
                    return;
                }
            }
        }
    }
}
