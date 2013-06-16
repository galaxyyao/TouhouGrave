using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    public interface IGenericMoveFrom : ICommand
    {
        CardInstance Subject { get; }
        int FromZone { get; }
    }

    public interface IMoveFrom<TFromZone> : IGenericMoveFrom
        where TFromZone : IZoneToken, new()
    { }

    public interface IGenericMoveTo : ICommand
    {
        CardInstance Subject { get; }
        int ToZone { get; }
    }

    public interface IMoveTo<TToZone> : IGenericMoveTo
        where TToZone : IZoneToken, new()
    { }

    public interface IGenericMoveCard :
        IGenericMoveFrom, IGenericMoveTo
    { }

    public interface IMoveCard<TFromZone, TToZone> :
        IGenericMoveCard,
        IMoveFrom<TFromZone>, IMoveTo<TToZone>
        where TFromZone : IZoneToken, new()
        where TToZone : IZoneToken, new()
    { }
}
