using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    public interface IMoveCard : ICommand
    {
        CardInstance Subject { get; }
        int FromZone { get; }
        ZoneType FromZoneType { get; }
        int ToZone { get; }
        ZoneType ToZoneType { get; }

        void PatchZoneMoveTo(int newToZone);
    }

    public interface IInitiativeMoveCard : IMoveCard, IInitiativeCommand
    { }
}
