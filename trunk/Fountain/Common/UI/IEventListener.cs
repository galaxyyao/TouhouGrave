using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.UI
{
    public interface IEventListener<in TEventArgs>
        where TEventArgs : EventArgs
    {
        void RaiseEvent(TEventArgs e);
    }
}
