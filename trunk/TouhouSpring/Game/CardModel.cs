using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
    public interface ICardModel
    {
        string Id { get; }
        string Name { get; }
        string Description { get; }
        string ArtworkUri { get; }
        IList<Behaviors.IBehaviorModel> Behaviors { get; }
    }

    public class CardModel : ICardModel
    {
        public class Database : Dictionary<string, CardModel> { }

        public string Id
        {
            get; set;
        }

        public string Name
        {
            get; set;
        }

        public string Description
        {
            get; set;
        }

        public string ArtworkUri
        {
            get; set;
        }

        public IList<Behaviors.IBehaviorModel> Behaviors
        {
            get; set;
        }
    }
}
