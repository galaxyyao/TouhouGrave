using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;

namespace TouhouSpring.TextureAtlas
{
    class Document
    {
        public Atlas Atlas
        {
            get; private set;
        }

        public Dictionary<string, Bitmap> SubImages
        {
            get; private set;
        }

        public string FileName
        {
            get; private set;
        }

        public Document(int width, int height)
        {
            FileName = null;

            Atlas = new Atlas
            {
                Width = width,
                Height = height,
            };

            SubImages = new Dictionary<string, Bitmap>();
        }

        public Document(string fileName)
        {
            FileName = fileName;

            using (XmlReader xr = XmlReader.Create(fileName))
            {
                Atlas = IntermediateSerializer.Deserialize<Atlas>(xr, Path.GetDirectoryName(fileName));
            }

            SubImages = new Dictionary<string, Bitmap>();

            Uri baseUri = new Uri(Path.GetDirectoryName(fileName) + "\\");
            foreach (var kvp in Atlas.SubTextures)
            {
                var relativeUri = new Uri(kvp.Value.SourceFile, UriKind.Relative);
                SubImages.Add(kvp.Key, new Bitmap(new Uri(baseUri, relativeUri).LocalPath));
            }
        }

        public void SaveAs(string fileName)
        {
            if (FileName == null)
            {
                FileName = fileName;
                var fileUri = new Uri(FileName);

                foreach (var subTexture in Atlas.SubTextures.Values)
                {
                    subTexture.SourceFile = fileUri.MakeRelativeUri(new Uri(subTexture.SourceFile)).ToString();
                }
            }

            using (XmlWriter xw = XmlWriter.Create(fileName, new XmlWriterSettings() { Indent = true }))
            {
                IntermediateSerializer.Serialize(xw, Atlas, Path.GetDirectoryName(fileName));
            }
        }

        public void AddSubTexture(string id, string source)
        {
            if (id == null)
            {
                throw new ArgumentNullException("id");
            }
            else if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            else if (Atlas.SubTextures.ContainsKey(id))
            {
                throw new ArgumentException(String.Format("Sub-texture with key '{0}' already existed.", id));
            }

            if (FileName != null)
            {
                var relativeUri = new Uri(FileName).MakeRelativeUri(new Uri(source));
                source = relativeUri.ToString();
            }

            var bmp = new Bitmap(source);
            var subTexture = new SubTexture(bmp.Width, bmp.Height)
            {
                SourceFile = source,
                Left = 0,
                Top = 0,
                Rotation = Rotation.Rotation_0,
                FlipHorizontal = false,
                FlipVertical = false
            };
            Atlas.SubTextures.Add(id, subTexture);
            SubImages.Add(id, bmp);
        }

        public void RemoveSubTexture(string id)
        {
            if (id == null)
            {
                throw new ArgumentNullException("id");
            }

            Atlas.SubTextures.Remove(id);
            SubImages[id].Dispose();
            SubImages.Remove(id);
        }
    }
}
