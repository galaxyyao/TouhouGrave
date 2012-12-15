using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.UI
{
    partial class CardControl : Style.IBindingProvider
    {
        public bool TryGetValue(string id, out string replacement)
        {
            switch (id)
            {
                case "Card.Description":
                    replacement = Card.Model.Description;
                    break;
                case "Card.ImageUri":
                    replacement = Card.Model.ArtworkUri != string.Empty ? Card.Model.ArtworkUri : "Textures/DefaultArtwork";
                    break;
                case "Card.Name":
                    replacement = Card.Model.Name;
                    break;
                case "Card.SummonCost":
                    var manaCost = Card.IsOnHand ? Card.Behaviors.Get<Behaviors.ManaCost_PrePlay>() : null;
                    replacement = manaCost != null ? manaCost.Cost.ToString() : "";
                    break;
                case "Card.Values":
                    var warrior = Card.Behaviors.Get<Behaviors.Warrior>();
                    if (warrior != null)
                    {
                        var attackColor = warrior.Attack > warrior.InitialAttack
                                          ? "Green" : warrior.Attack < warrior.InitialAttack ? "Red" : "Black";
                        var defenseColor = warrior.Defense > warrior.InitialDefense
                                           ? "Green" : warrior.Defense < warrior.InitialDefense ? "Red" : "Black";
                        var sb = new StringBuilder();
                        sb.Append("[color:");
                        sb.Append(attackColor);
                        sb.Append("]");
                        sb.Append(warrior.Attack.ToString());
                        sb.Append("[/color] [color:Black]|[/color] [color:");
                        sb.Append(defenseColor);
                        sb.Append("]");
                        sb.Append(warrior.Defense.ToString());
                        sb.Append("[/color]");
                        replacement = sb.ToString();
                    }
                    else
                    {
                        replacement = "";
                    }
                    break;
                default:
                    replacement = null;
                    return false;
            }

            return true;
        }
    }
}
