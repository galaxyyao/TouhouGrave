using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace TouhouSpring.TextureAtlas
{
    [ContentSerializerRuntimeType("TouhouSpring.TextureAtlas.Atlas, Fountain.TextureAtlas")]
    public class AtlasContent
    {
        public Dictionary<string, SubTexture> SubTextures;
        public int Width;
        public int Height;
        public TextureContent Texture;
    }

    public class AtlasDescriptor
    {
        public Atlas Atlas;
        public ContentIdentity Identity;
    }

    [ContentImporter(".xml", DisplayName = "Atlas - Fountain Framework", DefaultProcessor = "AtlasWriter")]
    public class AtlasImporter : ContentImporter<AtlasDescriptor>
    {
        public override AtlasDescriptor Import(string filename, ContentImporterContext context)
        {
            return new AtlasDescriptor
            {
                Atlas = new XmlImporter().Import(filename, context) as Atlas,
                Identity = new ContentIdentity(new FileInfo(filename).FullName, "AtlasImporter")
            };
        }
    }

    [ContentProcessor(DisplayName = "Atlas - Fountain Framework")]
    public class AtlasWriter : ContentProcessor<AtlasDescriptor, AtlasContent>
    {
        [System.ComponentModel.DefaultValue(false)]
        [System.ComponentModel.DisplayName("Generate Mipmaps")]
        public bool GenerateMipmaps { get; set; }

        [System.ComponentModel.DefaultValue(true)]
        [System.ComponentModel.DisplayName("Premultiply Alpha")]
        public bool PremultiplyAlpha { get; set; }

        [System.ComponentModel.DefaultValue(false)]
        [System.ComponentModel.DisplayName("Resize to Power of Two")]
        public bool ResizeToPowerOfTwo { get; set; }

        [System.ComponentModel.DefaultValue(TextureProcessorOutputFormat.Color)]
        [System.ComponentModel.DisplayName("Texture Format")]
        public TextureProcessorOutputFormat TextureFormat { get; set; }

        public AtlasWriter()
        {
            GenerateMipmaps = false;
            PremultiplyAlpha = true;
            ResizeToPowerOfTwo = false;
            TextureFormat = TextureProcessorOutputFormat.Color;
        }

        public override AtlasContent Process(AtlasDescriptor input, ContentProcessorContext context)
        {
            var atlas = input.Atlas;

            if (ResizeToPowerOfTwo)
            {
                atlas.Width = MathUtils.RoundToNextPowerOfTwo(atlas.Width);
                atlas.Height = MathUtils.RoundToNextPowerOfTwo(atlas.Height);
            }

            TextureProcessor textureProcessor = new TextureProcessor
            {
                GenerateMipmaps = GenerateMipmaps,
                PremultiplyAlpha = PremultiplyAlpha,
                ResizeToPowerOfTwo = false,
                TextureFormat = TextureFormat
            };

            var bmp = CompositeImage(atlas, Path.GetDirectoryName(input.Identity.SourceFilename));
            var tmpFileName = Path.ChangeExtension(Path.GetTempFileName(), ".png");
            bmp.Save(tmpFileName, ImageFormat.Png);

            TextureContent texture = null;
            try
            {
                texture = textureProcessor.Process(
                    new TextureImporter().Import(tmpFileName, null),
                    context);
            }
            finally
            {
                File.Delete(tmpFileName);
            }

            return new AtlasContent
            {
                SubTextures = atlas.SubTextures,
                Width = atlas.Width,
                Height = atlas.Height,
                Texture = texture
            };
        }

        public static Bitmap CompositeImage(Atlas atlas, string baseDirectory)
        {
            Uri baseUri = new Uri(baseDirectory + "\\");

            Dictionary<string, Bitmap> subImages = new Dictionary<string, Bitmap>();
            foreach (var kvp in atlas.SubTextures)
            {
                var relativeUri = new Uri(kvp.Value.SourceFile, UriKind.Relative);
                subImages.Add(kvp.Key, new Bitmap(new Uri(baseUri, relativeUri).LocalPath));
            }

            Bitmap ret = new Bitmap(atlas.Width, atlas.Height);
            CompositeImage(ret, atlas, subImages);

            foreach (var bmp in subImages.Values)
            {
                bmp.Dispose();
            }

            return ret;
        }

        public static void CompositeImage(Bitmap bmp, Atlas atlas, Dictionary<string, Bitmap> subImages)
        {
            Graphics g = Graphics.FromImage(bmp);

            g.Clear(Color.Transparent);
            foreach (var kvp in subImages)
            {
                DrawSubTexture(g, kvp.Value, atlas.SubTextures[kvp.Key]);
            }
        }

        private static void DrawSubTexture(Graphics g, Bitmap subImage, SubTexture subTexture)
        {
            var w = subImage.Width;
            var h = subImage.Height;

            var mtx = new Matrix(subTexture.FlipHorizontal ? -1 : 1, 0,
                                 0, subTexture.FlipVertical ? -1 : 1,
                                 subTexture.FlipHorizontal ? w : 0,
                                 subTexture.FlipVertical ? h : 0);
            switch (subTexture.Rotation)
            {
                case Rotation.Rotation_90:
                    mtx.Rotate(90, MatrixOrder.Append);
                    mtx.Translate(h, 0, MatrixOrder.Append);
                    break;
                case Rotation.Rotation_180:
                    mtx.Rotate(180, MatrixOrder.Append);
                    mtx.Translate(w, h, MatrixOrder.Append);
                    break;
                case Rotation.Rotation_270:
                    mtx.Rotate(270);
                    mtx.Translate(0, w, MatrixOrder.Append);
                    break;
            }
            mtx.Translate(subTexture.Left, subTexture.Top, MatrixOrder.Append);
            g.Transform = mtx;
            g.DrawImage(subImage, new System.Drawing.Rectangle(0, 0, w, h));
        }
    }
}
