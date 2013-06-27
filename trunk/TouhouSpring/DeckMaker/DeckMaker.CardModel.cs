using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
    public class DeckMaker
    {
        public class CardModel : ICloneable
        {
            public enum CardTypeEnum
            {
                Warrior, Assist, Spell
            }

            public string Id
            {
                get;
                set;
            }

            public string Name
            {
                get;
                set;
            }

            public string Description
            {
                get;
                set;
            }

            public string ArtworkUri
            {
                get;
                set;
            }

            public CardTypeEnum CardType
            {
                get;
                set;
            }

            public int ManaCost
            {
                get;
                set;
            }

            public int? Attack
            {
                get;
                set;
            }

            public int? Life
            {
                get;
                set;
            }

            public object Clone()
            {
                CardModel newModel = new CardModel
                {
                    Id = this.Id,
                    Name = this.Name,
                    Description = this.Description,
                    ArtworkUri = this.ArtworkUri,
                    CardType = this.CardType,
                    ManaCost = this.ManaCost,
                    Attack = this.Attack,
                    Life = this.Life
                };
                return (object)newModel;
            }
        }
    }
}
