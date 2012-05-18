using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

namespace TouhouSpring.TextureAtlas
{
	partial class View
	{
		private int m_backgroundTileSize = 16;
		private Color m_backgroundTileAlternateColor = Color.Silver;
		private Color m_maskColor = Color.FromArgb(192, Color.Black);

		private Brush m_backgroundTileBrush;
		private Brush m_maskBrush;

        public Bitmap BackgroundImage
		{
			get; private set;
		}

		public Bitmap ContentImage
		{
			get; private set;
		}

		public Bitmap FinalComposedImage
		{
			get; private set;
		}

		public int BackgroundTileSize
		{
			get { return m_backgroundTileSize; }
			set
			{
				if (value != m_backgroundTileSize)
				{
					m_backgroundTileSize = value;
					CreateBackgroundTileBrush();
				}
			}
		}

		public Color BackgroundTileAlternateColor
		{
			get { return m_backgroundTileAlternateColor; }
			set
			{
				if (value != m_backgroundTileAlternateColor)
				{
					m_backgroundTileAlternateColor = value;
					CreateBackgroundTileBrush();
				}
			}
		}

		public Color MaskColor
		{
			get { return m_maskColor; }
			set
			{
				if (value != m_maskColor)
				{
					m_maskColor = value;
					CreateMaskBrush();
				}
			}
		}

		public SubTexture SelectedSubTexture
		{
			get; set;
		}

		public View()
		{
			CreateBackgroundTileBrush();
			CreateMaskBrush();
		}

		public void Reload(Document doc)
		{
			Resize(doc.Atlas.Width, doc.Atlas.Height);
			DrawContentImage(doc);
			DrawFinalImage();
		}

		public void RedrawFinalImage()
		{
			DrawFinalImage();
		}

		private void CreateBackgroundTileBrush()
		{
			if (m_backgroundTileBrush != null)
			{
				m_backgroundTileBrush.Dispose();
				m_backgroundTileBrush = null;
			}

			var tileBmp = new Bitmap(BackgroundTileSize, BackgroundTileSize);

			using (var g = Graphics.FromImage(tileBmp))
			using (var b = new SolidBrush(BackgroundTileAlternateColor))
			{
				g.Clear(Color.White);
				var halfSize = BackgroundTileSize / 2;
				g.FillRectangle(b, 0, 0, halfSize, halfSize);
				g.FillRectangle(b, halfSize, halfSize, halfSize, halfSize);
			}

			m_backgroundTileBrush = new TextureBrush(tileBmp);
		}

		private void CreateMaskBrush()
		{
			if (m_maskBrush != null)
			{
				m_maskBrush.Dispose();
				m_maskBrush = null;
			}

			m_maskBrush = new SolidBrush(MaskColor);
		}
	}
}
