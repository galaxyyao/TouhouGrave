using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.UI
{
    interface ITextReceiver
    {
        bool ImeEnabled { get; } // shall be constant throughout object's lifetime (for now)
        void OnChar(char code);
        void OnInputLanguageChange(string lang);
        void OnStartComposition();
        void OnComposition(string compositionString, Ime.ClauseAttribute[] attr, int cursorPos);
        void OnEndComposition();
        void OnCandidateListUpdate(Ime.CandidateListData data);
    }
}
