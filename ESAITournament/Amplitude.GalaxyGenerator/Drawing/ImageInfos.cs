// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageInfos.cs" company="AMPLITUDE Studios">
//   Copyright AMPLITUDE Studios. All rights reserved.
//   
//   This Source Code Form is subject to the terms of the Mozilla Public
//   License, v. 2.0. If a copy of the MPL was not distributed with this
//   file, You can obtain one at http://mozilla.org/MPL/2.0/ .
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Paloma;

namespace Amplitude.GalaxyGenerator.Drawing
{
    public enum ImageType
    {
        Targa
    }

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ImageInfos
    {
        /// <summary>
        /// an array of colors, representing the image loaded
        /// </summary>
        private Color[,] pixel;

        public ImageInfos(ImageType imageType, string fileName)
        {
            switch (imageType)
            {
                case ImageType.Targa:
                    TargaImage image = new TargaImage(fileName);
                    this.pixel = new Color[image.Header.Height, image.Header.Width];
                    int bytesPerPixel = image.Header.BytesPerPixel;

                    for (int i = 0; i < this.Height; i++)
                    {
                        for (int j = 0; j < this.Width; j++)
                        {
                            byte a = 0, r = 0, g = 0, b = 0;
                            switch (bytesPerPixel)
                            {
                                case 3:
                                    a = 255;
                                    b = image.BinaryImageData[(i * this.Height * 3) + (j * 3)];
                                    g = image.BinaryImageData[(i * this.Height * 3) + (j * 3) + 1];
                                    r = image.BinaryImageData[(i * this.Height * 3) + (j * 3) + 2];
                                    break;
                            }

                            this.pixel[j, i] = Color.FromArgb(a, r, g, b);
                        }
                    }

                    break;
            }
        }

        public int Width
        {
            get { return this.pixel.GetLength(0); }
        }

        public int Height
        {
            get { return this.pixel.GetLength(1); }
        }

        public Color GetPixel(int x, int y)
        {
            return this.pixel[x, y];
        }
    }
}
