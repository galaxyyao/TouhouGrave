﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Graphics;
using DDW.Swf;
using DDW.V2D;
using DDW.VexTo2DPhysics;
using System.IO;

namespace DDW.VexPipeline
{
    [ContentProcessor]
	public class SwfProcessor : ContentProcessor<string, V2DContentHolder>
    {
		public override V2DContentHolder Process(string fileName, ContentProcessorContext context)
        {
            //System.Diagnostics.Debugger.Launch();
			V2DContent result = null;
			V2DContentHolder vch = null;
            //System.Diagnostics.Debugger.Launch();

            if (File.Exists(fileName))
            {
                fileName = Path.GetFullPath(fileName);
                Directory.SetCurrentDirectory(Path.GetDirectoryName(fileName));
                FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);

                string name = Path.GetFileNameWithoutExtension(fileName);
                SwfReader r = new SwfReader(br.ReadBytes((int)fs.Length));
                SwfCompilationUnit scu = new SwfCompilationUnit(r, name);
                if (scu.IsValid)
                {
                    vch = DDW.VexTo2DPhysics.V2D.SwfToV2DContent(scu, context);
                }
            }

            if (vch == null)
            {
                throw new InvalidContentException("invalid swf content");
            }
			result = new V2DContent();
            return vch;
        }
    }

}
