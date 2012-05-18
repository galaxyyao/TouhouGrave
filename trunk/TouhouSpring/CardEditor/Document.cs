using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;

namespace TouhouSpring
{
    class Document
    {
        public string FileName
        {
            get; private set;
        }

        public CardModelList Cards
        {
            get; private set;
        }

        public Document()
        {
            FileName = null;
            Cards = new CardModelList();
        }

        public Document(string fileName)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }

            FileName = fileName;

            using (XmlReader xr = XmlReader.Create(fileName))
            {
                Cards = IntermediateSerializer.Deserialize<CardModelList>(xr, Path.GetDirectoryName(fileName));
            }
        }

        public void SaveAs(string fileName)
        {
            if (FileName == null)
            {
                FileName = fileName;
            }

            using (XmlWriter xw = XmlWriter.Create(fileName, new XmlWriterSettings() { Indent = true }))
            {
                IntermediateSerializer.Serialize(xw, Cards, Path.GetDirectoryName(fileName));
            }
        }
    }
}
