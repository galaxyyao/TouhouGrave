using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
    partial class Game
    {
        public void SimulateMainPhase()
        {
            if (m_gameFlowThread != null)
            {
                throw new InvalidOperationException("Can't run simulation on this Game object.");
            }
            else if (CurrentPhase != "Main")
            {
                throw new InvalidOperationException("Simulate can only be invoked when the game in Main phase.");
            }

            MainPhase();
        }
    }
}
