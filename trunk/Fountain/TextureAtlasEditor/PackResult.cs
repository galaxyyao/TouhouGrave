using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mapper;

namespace TouhouSpring.TextureAtlas
{
	class PackResult : ISprite
	{
		public List<IMappedImageInfo> MappedImages
		{
			get; private set;
		}

		public int Width
		{
			get; private set;
		}

		public int Height
		{
			get; private set;
		}

		public int Area
		{
			get { return Width * Height; }
		}

		public PackResult()
		{
			MappedImages = new List<IMappedImageInfo>();
			Width = 0;
			Height = 0;
		}

		public void AddMappedImage(IMappedImageInfo imageLocation)
		{
			MappedImages.Add(imageLocation);

			Height = Math.Max(Height, imageLocation.Y + imageLocation.ImageInfo.Height);
			Width = Math.Max(Width, imageLocation.X + imageLocation.ImageInfo.Width);
		}
	}
}
