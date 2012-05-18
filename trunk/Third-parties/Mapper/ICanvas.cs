﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mapper
{
    /// <summary>
    /// A canvas is a rectangle of a given size that lets you add smaller rectangle.
    /// The canvas will place each rectangle so that it doesn't overlap with any other rectangle that is already 
    /// on the canvas.
    /// </summary>
    public interface ICanvas
    {
        /// <summary>
        /// Value denoting an unlimited width or height. You would pass this in to SetCanvasDimensions.
        /// </summary>
        int UnlimitedSize { get; }

        /// <summary>
        /// Sets the dimensions of the canvas.
        /// If there were already rectangles on the canvas when this is called, those rectangles will be removed.
        /// 
        /// Be sure to call this method before you call AddRectangle for the first time.
        /// </summary>
        /// <param name="canvasWidth">New width of the canvas</param>
        /// <param name="canvasHeight">New height of the canvas</param>
        void SetCanvasDimensions(int canvasWidth, int canvasHeight);

        /// <summary>
        /// Adds a rectangle
        /// </summary>
        /// <param name="rectangleWidth">Width of the rectangle</param>
        /// <param name="rectangleHeight">Height of the rectangle</param>
        /// <param name="rectangleXOffset">X position where rectangle has been placed</param>
        /// <param name="rectangleYOffset">Y position where rectangle has been placed</param>
        /// <param name="lowestFreeHeightDeficit">
        /// Lowest free height deficit for all the rectangles placed since the last call to SetCanvasDimensions.
        /// 
        /// This will be set to Int32.MaxValue if there was never any free height deficit.
        /// </param>
        /// <returns>
        /// true: rectangle placed
        /// false: rectangle not placed because there was no room
        /// </returns>
        bool AddRectangle(
            int rectangleWidth, int rectangleHeight, out int rectangleXOffset, out int rectangleYOffset,
            out int lowestFreeHeightDeficit);

        /// <summary>
        /// The canvas keeps statistics, on for example the number of times a FreeAreas is generated.
        /// Use this method to fill an object that implements ICanvasStats with these statistics.
        /// 
        /// Note that calling SetCanvasDimensions resets all counters.
        /// </summary>
        /// <param name="canvasStats">
        /// Reference to object to be filled.
        /// 
        /// If this is null, nothing happens (so there is no exception).
        /// </param>
        void GetStatistics(ICanvasStats canvasStats);


    }
}
