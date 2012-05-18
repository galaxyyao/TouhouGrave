using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace TouhouSpring.TextureAtlas
{
    using Rectangle = System.Drawing.Rectangle;

	partial class View
	{
		private int m_width = 0;
		private int m_height = 0;

		private void Resize(int width, int height)
		{
			if (width != m_width || height != m_height)
			{
				if (BackgroundImage != null)
				{
					BackgroundImage.Dispose();
					BackgroundImage = null;
				}

				BackgroundImage = new Bitmap(width, height);
				using (var g = Graphics.FromImage(BackgroundImage))
				{
					g.FillRectangle(m_backgroundTileBrush, 0, 0, BackgroundImage.Width, BackgroundImage.Height);
				}

				if (ContentImage != null)
				{
					ContentImage.Dispose();
					ContentImage = null;
				}

				ContentImage = new Bitmap(width, height);

				if (FinalComposedImage != null)
				{
					FinalComposedImage.Dispose();
					FinalComposedImage = null;
				}

				FinalComposedImage = new Bitmap(width, height);

				m_width = width;
				m_height = height;
			}
		}

		private void DrawContentImage(Document doc)
		{
            AtlasWriter.CompositeImage(ContentImage, doc.Atlas, doc.SubImages);
		}

		private void DrawFinalImage()
		{
			using (var g = Graphics.FromImage(FinalComposedImage))
			{
				g.DrawImage(BackgroundImage, new Rectangle(0, 0, m_width, m_height));
				g.DrawImage(ContentImage, new Rectangle(0, 0, m_width, m_height));

				if (SelectedSubTexture != null)
				{
					Region rgn = new Region(new Rectangle(0, 0, m_width, m_height));
					rgn.Exclude(new Rectangle(SelectedSubTexture.Left, SelectedSubTexture.Top,
											  SelectedSubTexture.Width, SelectedSubTexture.Height));
					g.FillRegion(m_maskBrush, rgn);
				}
			}
		}
	}
}
