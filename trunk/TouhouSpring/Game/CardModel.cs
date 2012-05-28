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
        string Level { get; }
        string Description { get; }
        string ArtworkUri { get; }
        List<Behaviors.BehaviorModel> Behaviors { get; }
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

        public string Level
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

        public List<Behaviors.BehaviorModel> Behaviors
        {
            get; set;
        }
    }
}
