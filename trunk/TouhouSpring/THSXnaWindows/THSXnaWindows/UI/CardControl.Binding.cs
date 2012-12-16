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
            var warrior = Card.Behaviors.Get<Behaviors.Warrior>();
            switch (id)
            {
                case "Card.Description":
                    replacement = Card.Model.Description;
                    break;
                case "Card.ImageUri":
                    replacement = Card.Model.ArtworkUri != string.Empty ? Card.Model.ArtworkUri : "Textures/DefaultArtwork";
                    break;
                case "Card.InitialAttack":
                    if (warrior != null)
                    {
                        replacement = warrior.InitialAttack.ToString();
                    }
                    else
                    {
                        replacement = "";
                    }
                    break;
                case "Card.InitialDefense":
                    if (warrior != null)
                    {
                        replacement = warrior.InitialDefense.ToString();
                    }
                    else
                    {
                        replacement = "";
                    }
                    break;
                case "Card.Name":
                    replacement = Card.Model.Name;
                    break;
                case "Card.SummonCost":
                    var manaCost = Card.IsOnHand ? Card.Behaviors.Get<Behaviors.ManaCost_PrePlay>() : null;
                    replacement = manaCost != null ? manaCost.Cost.ToString() : "";
                    break;
                case "Card.SystemClass":
                    if (Card.Behaviors.Has<Behaviors.Hero>())
                    {
                        replacement = "主角";
                    }
                    else if (Card.Behaviors.Has<Behaviors.Warrior>())
                    {
                        replacement = "战士";
                    }
                    else if (Card.Behaviors.Has<Behaviors.Support>())
                    {
                        replacement = "支援";
                    }
                    else if (Card.Behaviors.Has<Behaviors.Instant>())
                    {
                        replacement = "突袭";
                    }
                    else
                    {
                        replacement = "迷";
                    }
                    break;
                case "Card.Values":
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
