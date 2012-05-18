using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
	/// <summary>
	/// Base class for heroes.
	/// </summary>
	public class Hero
	{
		/// <summary>
		/// This attribute must be attached to the classes inheriting Hero.
		/// </summary>
		public class PropertiesAttribute : Attribute
		{
			public string Name { get; set; }
			public int Health { get; set; }
			public int MaxMana { get; set; }
		}

		public string Name
		{
			get; private set;
		}

		public int Health
		{
			get; private set;
		}

		public int MaxMana
		{
			get; private set;
		}

		public Hero()
		{
			if (!this.GetType().HasAttribute<PropertiesAttribute>())
			{
				throw new IncompleteTypeDefinitionException(typeof(PropertiesAttribute));
			}

			var attr = this.GetType().GetAttribute<PropertiesAttribute>();
			if (attr.Name == null)
			{
				throw new InvalidDataException("Name must be present in BaseCard.PropertiesAttribute.");
			}

			Name = attr.Name;
			Health = attr.Health;
			MaxMana = attr.MaxMana;
		}
	}
}
