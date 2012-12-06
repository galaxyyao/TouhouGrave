﻿using System;
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
				case "Card.Attack":
					replacement = Card.Behaviors.Has<Behaviors.Warrior>() ? Card.Behaviors.Get<Behaviors.Warrior>().Attack.ToString() : "?";
					break;
				case "Card.Defense":
					replacement = Card.Behaviors.Has<Behaviors.Warrior>() ? Card.Behaviors.Get<Behaviors.Warrior>().Defense.ToString() : "?";
					break;
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
                    var manaCost = Card.Behaviors.Get<Behaviors.ManaCost_PrePlay>();
                    replacement = manaCost != null ? manaCost.Model.Cost.ToString() : "";
                    break;
				default:
					replacement = null;
					return false;
			}

			return true;
		}
	}
}
