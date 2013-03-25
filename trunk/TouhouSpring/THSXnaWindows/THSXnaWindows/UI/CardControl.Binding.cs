using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.UI
{
    partial class CardControl : Style.IBindingProvider
    {
        bool Style.IBindingProvider.EvaluateBinding(string id, out string replacement)
        {
            switch (id)
            {
                case "Card.Description":
                    replacement = CardData.Description;
                    break;
                case "Card.ImageUri":
                    replacement = !String.IsNullOrEmpty(CardData.ArtworkUri) ? CardData.ArtworkUri : "Textures/DefaultArtwork";
                    break;
                case "Card.InitialAttack":
                    replacement = CardData.AttackAndInitialAttack.Item2 >= 0 ? CardData.AttackAndInitialAttack.Item2.ToString() : "";
                    break;
                case "Card.InitialLife":
                    replacement = CardData.LifeAndInitialLife.Item2 >= 0 ? CardData.LifeAndInitialLife.Item2.ToString() : "";
                    break;
                case "Card.Name":
                    replacement = CardData.ModelName;
                    break;
                case "Card.SummonCost":
                    replacement = CardData.SummonCost >= 0 ? CardData.SummonCost.ToString() : "";
                    break;
                case "Card.SystemClass":
                    if (CardData.IsHero)
                    {
                        replacement = "主角";
                    }
                    else if (CardData.IsWarrior)
                    {
                        replacement = "战士";
                    }
                    else if (CardData.IsAssist)
                    {
                        replacement = "支援";
                    }
                    else if (CardData.IsInstant)
                    {
                        replacement = "突袭";
                    }
                    else
                    {
                        replacement = "迷";
                    }
                    break;
                case "Card.Values":
                    if (CardData.IsWarrior)
                    {
                        int sign = Math.Sign(CardData.AttackAndInitialAttack.Item1 - CardData.AttackAndInitialAttack.Item2);
                        var attackColor = sign > 0 ? "Green" : sign < 0 ? "Red" : "Black";
                        var sb = new StringBuilder();
                        sb.Append("[color:");
                        sb.Append(attackColor);
                        sb.Append("]");
                        sb.Append(CardData.AttackAndInitialAttack.Item1.ToString());
                        sb.Append("[/color] [color:Black]| ");
                        sb.Append(CardData.LifeAndInitialLife.Item1.ToString());
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
