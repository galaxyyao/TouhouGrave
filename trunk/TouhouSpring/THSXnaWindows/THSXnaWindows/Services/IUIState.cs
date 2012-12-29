using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Services
{
    interface IUIState
    {
        Interactions.BaseInteraction InteractionObject
        {
            get;
        }

        void OnEnter(Interactions.BaseInteraction io);
        void OnLeave();
        void OnCardClicked(UI.CardControl cardControl);
        void OnSpellClicked(UI.CardControl cardControl, Behaviors.ICastableSpell spell);
        void OnContextButton(string buttonText);

        bool IsCardClickable(UI.CardControl cardControl);
        bool IsCardSelected(UI.CardControl cardControl);
        bool IsCardSelectedForCastSpell(UI.CardControl cardControl);
    }
}
