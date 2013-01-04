using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.UI
{
    partial class CardControl : Style.IBindingProvider
    {
        public bool EvaluateBinding(string id, out string replacement)
        {
            var warrior = Card.Behaviors.Get<Behaviors.Warrior>();
            switch (id)
            {
                case "Card.Description":
                    replacement = Card.Model.Description;
                    break;
                case "Card.ImageUri":
                    replacement = String.IsNullOrEmpty(Card.Model.ArtworkUri) ? "Textures/DefaultArtwork" : Card.Model.ArtworkUri;
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
                case "Card.InitialLife":
                    if (warrior != null)
                    {
                        replacement = warrior.InitialLife.ToString();
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
                    var manaCost = Card.Behaviors.Get<Behaviors.ManaCost>();
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
                    else if (Card.Behaviors.Has<Behaviors.Assist>())
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
                        var sb = new StringBuilder();
                        sb.Append("[color:");
                        sb.Append(attackColor);
                        sb.Append("]");
                        sb.Append(warrior.Attack.ToString());
                        sb.Append("[/color] [color:Black]| ");
                        sb.Append(warrior.Life.ToString());
                        replacement = sb.ToString();
                    }
                    else
                    {
                        replacement = "";
                    }
                    break;
                case "PileBackOffset":
                    replacement = "0";
                    break;
                default:
                    replacement = null;
                    return false;
            }

            return true;
        }
    }
}
