using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace TouhouSpring.Particle
{
	public class ModifierList : List<Modifier>
	{
		public class ContentReaders : ContentTypeReader<ModifierList>
		{
			public override bool CanDeserializeIntoExistingObject
			{
				get { return true; }
			}

			protected override ModifierList Read(ContentReader input, ModifierList existingInstance)
			{
				if (existingInstance == null)
				{
					throw new InvalidOperationException("ModifierList can only be deserialized from an existing instance.");
				}

				int count = input.ReadInt32();
				for (int i = 0; i < count; ++i)
				{
					existingInstance.Add(input.ReadObject<Modifier>((Modifier)null));
				}

				return existingInstance;
			}
		}

        public Effect Effect
        {
            get; internal set;
        }
	}
}
