using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Style.Properties
{
	class TransformProperty : BaseProperty
	{
		private struct SRT
		{
			public string m_matrix;
			public string m_scaling;
			public string m_rotation;
			public string m_translation;
		}

		private List<SRT> m_srt = new List<SRT>();

		public TransformProperty(IStyleContainer parent)
			: base(parent)
		{ }

		public override void Initialize()
		{
			if (Parent.Definition == null || !(Parent.Target is UI.ITransformNode))
			{
				return;
			}

			foreach (var elem in Parent.Definition.Elements("Transform"))
			{
				var matrixAttr = elem.Attribute("Matrix");
				var scalingAttr = elem.Attribute("Scaling");
				var rotationAttr = elem.Attribute("Rotation");
				var translationAttr = elem.Attribute("Translation");

				if (matrixAttr != null && (scalingAttr != null || rotationAttr != null || translationAttr != null))
				{
					throw new MutalExclusiveAttributeException("Matrix", "Scaling, Rotation, Translation");
				}

				m_srt.Add(new SRT
				{
					m_matrix = matrixAttr != null ? matrixAttr.Value : null,
					m_scaling = scalingAttr != null ? scalingAttr.Value : null,
					m_rotation = rotationAttr != null ? rotationAttr.Value : null,
					m_translation = translationAttr != null ? translationAttr.Value : null
				});
			}
		}

		public override void Apply()
		{
			if (Parent.Definition == null || !(Parent.Target is UI.ITransformNode))
			{
				return;
			}

			var transformNode = Parent.Target as UI.ITransformNode;
			var transform = MatrixHelper.Identity;

			for (int i = 0; i < m_srt.Count; ++i)
			{
				SRT srt = m_srt[i];

				if (srt.m_matrix != null)
				{
					transform *= MatrixHelper.Deserialize(BindValuesFor(srt.m_matrix));
				}
				else
				{
					if (srt.m_scaling != null)
					{
						string[] tokens = srt.m_scaling.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
						float sx = tokens.Length > 0 ? float.Parse(tokens[0]) : 1.0f;
						float sy = tokens.Length > 1 ? float.Parse(tokens[1]) : 1.0f;
						float sz = tokens.Length > 2 ? float.Parse(tokens[2]) : 1.0f;
						if (tokens.Length > 3)
						{
							throw new StyleException("Attribute 'Scale' can only have three components.");
						}

						transform *= MatrixHelper.Scale(sx, sy, sz);
					}

					if (srt.m_rotation != null)
					{
						float degree = float.Parse(srt.m_rotation);
						transform *= MatrixHelper.RotateZ(degree * MathUtils.PI / 180.0f);
					}

					if (srt.m_translation != null)
					{
						string[] tokens = srt.m_translation.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
						float tx = tokens.Length > 0 ? float.Parse(tokens[0]) : 1.0f;
						float ty = tokens.Length > 1 ? float.Parse(tokens[1]) : 1.0f;
						if (tokens.Length > 2)
						{
							throw new StyleException("Attribute 'Translation' can only have two components.");
						}

						transform *= MatrixHelper.Translate(tx, ty);
					}
				}
			}

			transformNode.Transform = transform * transformNode.Transform;
		}
	}
}
