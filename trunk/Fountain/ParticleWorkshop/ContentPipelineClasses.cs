using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using XnaRect = Microsoft.Xna.Framework.Rectangle;

namespace TouhouSpring.Particle
{
	class ContentTypeWriters
	{
        [ContentProcessor(DisplayName = "Particle - Fountain Framework")]
		public class ParticleSystemTextureAtlasResolve : ContentProcessor<ParticleSystem, ParticleSystem>
		{
			public override ParticleSystem Process(ParticleSystem input, ContentProcessorContext context)
			{
                TextureAtlas.Atlas atlas = null;
                if (input.Effects.Any(fx => !fx.UVBoundsName.StartsWith("{") && !fx.UVBoundsName.EndsWith("}")))
                {
                    atlas = MyResourceLoader.ResolveAtlas(input.TextureName);
                }

                foreach (var fx in input.Effects)
                {
                    if (fx.UVBoundsName.StartsWith("{") || fx.UVBoundsName.EndsWith("}"))
                    {
                        continue;
                    }

                    if (atlas == null || !atlas.SubTextures.ContainsKey(fx.UVBoundsName))
                    {
                        fx.UVBoundsName = null;
                    }

                    var subTexture = atlas.SubTextures[fx.UVBoundsName];
                    fx.UVBoundsName = String.Format("{{x:{0} y:{1} w:{2} h:{3}}}", subTexture.Left, subTexture.Top, subTexture.Width, subTexture.Height);
                }

				return input;
			}
		}

		[ContentTypeWriter]
		public class EffectListWriter : ContentTypeWriter<EffectList>
		{
			public override bool CanDeserializeIntoExistingObject
			{
				get { return true; }
			}

			public override string GetRuntimeReader(TargetPlatform targetPlatform)
			{
				return typeof(EffectList.ContentReaders).AssemblyQualifiedName;
			}

			protected override void Write(ContentWriter output, EffectList value)
			{
				output.Write(value.Count);
				for (int i = 0; i < value.Count; ++i)
				{
					output.WriteObject(value[i]);
				}
			}
		}

		[ContentTypeWriter]
		public class ModifierListWriter : ContentTypeWriter<ModifierList>
		{
			public override bool CanDeserializeIntoExistingObject
			{
				get { return true; }
			}

			public override string GetRuntimeReader(TargetPlatform targetPlatform)
			{
				return typeof(ModifierList.ContentReaders).AssemblyQualifiedName;
			}

			protected override void Write(ContentWriter output, ModifierList value)
			{
				output.Write(value.Count);
				for (int i = 0; i < value.Count; ++i)
				{
					output.WriteObject(value[i]);
				}
			}
		}
	}
}
