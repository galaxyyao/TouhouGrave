using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XnaColor = Microsoft.Xna.Framework.Color;
using XnaEffect = Microsoft.Xna.Framework.Graphics.Effect;

namespace TouhouSpring.Particle
{
	partial class Canvas
	{
		private readonly int GridSize = 14; // 14x14 grid just like 3ds Max

		private XnaEffect m_gridFx;

		private VertexDeclaration m_gridVertDecl;
		private VertexBuffer m_gridVertices;
		private IndexBuffer m_gridIndices;
		private IndexBuffer m_meridianIndices;

		public XnaColor GridColor
		{
			get; set;
		}

		private float m_gridScale = 5.0f;

		private void Initialize_Grid()
		{
			m_gridFx = new XnaEffect(GraphicsDevice, EffectLoader.CompileFromFile(
				Program.PathUtils.ToDiskPath("Effects/GridShader", "fx")));
			m_gridFx.CurrentTechnique = m_gridFx.Techniques[0];

			m_gridVertDecl = new VertexDeclaration(
				new VertexElement(0, VertexElementFormat.Vector2, VertexElementUsage.Position, 0)
			);
			m_gridVertices = new VertexBuffer(GraphicsDevice, m_gridVertDecl, (GridSize + 1) * (GridSize + 1), BufferUsage.None);

			int HalfGridSize = GridSize / 2;
			Vector2[] grids = new Vector2[(GridSize + 1) * (GridSize + 1)];
			for (int y = -HalfGridSize; y <= HalfGridSize; ++y)
			{
				for (int x = -HalfGridSize; x <= HalfGridSize; ++x)
				{
					grids[(y + HalfGridSize) * (GridSize + 1) + x + HalfGridSize] = new Vector2(x, y);
				}
			}
			m_gridVertices.SetData(grids);

			m_gridIndices = new IndexBuffer(GraphicsDevice, IndexElementSize.SixteenBits, GridSize * 4, BufferUsage.None);
			UInt16[] lineIndices = new UInt16[GridSize * 4];
			int writePos = 0;

			// longitudes
			for (int x = -HalfGridSize; x <= HalfGridSize; ++x)
			{
				if (x == 0)
				{
					continue;
				}
				lineIndices[writePos++] = (UInt16)(x + HalfGridSize);
				lineIndices[writePos++] = (UInt16)((GridSize + 1) * GridSize + x + HalfGridSize);
			}

			// latitudes
			for (int y = -HalfGridSize; y <= HalfGridSize; ++y)
			{
				if (y == 0)
				{
					continue;
				}
				lineIndices[writePos++] = (UInt16)((y + HalfGridSize) * (GridSize + 1));
				lineIndices[writePos++] = (UInt16)((y + HalfGridSize) * (GridSize + 1) + GridSize);
			}

			m_gridIndices.SetData(lineIndices);

			m_meridianIndices = new IndexBuffer(GraphicsDevice, IndexElementSize.SixteenBits, 4, BufferUsage.None);
			UInt16[] meridians = new UInt16[]
			{
				(UInt16)(HalfGridSize),
				(UInt16)((GridSize + 1) * GridSize + HalfGridSize),
				(UInt16)(HalfGridSize * (GridSize + 1)),
				(UInt16)(HalfGridSize * (GridSize + 1) + GridSize)
			};
			m_meridianIndices.SetData(meridians);

			GridColor = XnaColor.Gray;
		}

		private void Render_Grid()
		{
			m_gridFx.Parameters["Transform"].SetValue(Matrix.CreateScale(m_gridScale) * ViewMatrix * ProjectionMatrix);
			GraphicsDevice.SetVertexBuffer(m_gridVertices);

			m_gridFx.Parameters["Color"].SetValue(GridColor.ToVector4());
			GraphicsDevice.Indices = m_gridIndices;
			foreach (var pass in m_gridFx.CurrentTechnique.Passes)
			{
				pass.Apply();
				GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.LineList,
					0, 0, (GridSize + 1) * (GridSize + 1),
					0, GridSize * 2);
			}

			m_gridFx.Parameters["Color"].SetValue(XnaColor.Black.ToVector4());
			GraphicsDevice.Indices = m_meridianIndices;
			foreach (var pass in m_gridFx.CurrentTechnique.Passes)
			{
				pass.Apply();
				GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.LineList,
					0, 0, (GridSize + 1) * (GridSize + 1),
					0, 2);
			}
		}
	}
}
