using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Interactions
{
    public interface IQuickInteraction
    {
        // A quick interaction can be issued in command triggering phases,
        // especially with Game.NeedInteraction in Prerequisite phase

        // returns null means the interaction is canceled
        object Run();

        bool HasCandidates();
    }
}
