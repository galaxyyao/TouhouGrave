﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mapper
{
    /// <summary>
    /// Defines an image that has been mapped to a specific location, for example within a sprite.
    /// </summary>
    public interface IMappedImageInfo
    {
        int X { get; }
        int Y { get; }
        IImageInfo ImageInfo { get; }
    }
}
