// ANPR_MX is a simple Automatic Plate Recognition System
// for North American type plates.
// Developed by Jivan Miranda Rodriguez
// For suggestions & feedback contact me at
// jivanro@hotmail.com
// Copyright (c) 2012, Jivan Miranda Rodriguez
// All rights reserved

// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
// * Redistributions of source code must retain the above copyright notice, this list
//   of conditions and the following disclaimer.
// * Redistributions in binary form must reproduce the above copyright notice, this list
//   of conditions and the following disclaimer in the documentation and/or other 
//   materials provided with the distribution.
// * Neither the name of Jivan Miranda Rodriguez nor the names of its contributors may be used
//   to endorse or promote products derived from this software without specific prior written permission.

// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO,
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS
// BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY,
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE,
// EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

// ---- N O T E S ----
// This project source code links to the original version of the following libraries:
// OpenCV 2.3 - license type: BSD 2 - http://opencv.willowgarage.com
// OpenCvSharp 2.3 - license type: LGPL 3 - http://code.google.com/p/opencvsharp
// Third party copyrights are property of their respective owners.
// ---- N O T E S ----

// ---- I M P O R T A N T ----
// This work is a derivate of the JavaANPR. JavaANPR is a intellectual 
// property of Ondrej Martinsky. Please visit http://javaanpr.sourceforge.net 
// for more info about JavaANPR. 
// ---- I M P O R T A N T ----

// If you want to alter upon this work, you MUST attribute it in 
// a) all source files
// b) on every place, where is the copyright of derivated work
// exactly by the following label :

// ---- label begin ----
// This work is a derivate of ANPR_MX & the JavaANPR. ANPR_MX is an intellectual property
// of Jivan Miranda Rodriguez while JavaANPR is an intellectual 
// property of Ondrej Martinsky. Please visit http://javaanpr.sourceforge.net 
// for more info about JavaANPR. 
// ----  label end  ----

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

using OpenCvSharp;
using ANPR_MX.Globals;

namespace ANPR_MX.PlateDataDetectionLogic
{
    public class Char
    {
        private Boolean normalized = false;
        private PixelMap.Piece bestPiece = null;
        private IplImage image = null;
        private IplImage thresholdedImage = null;
        private PositionInPlate positionInPlate = null;
        private Int32 fullWidth;
        private Int32 fullHeight;
        private Int32 pieceWidth;
        private Int32 pieceHeight;

        private Double statisticAverageBrightness;
        private Double statisticMinimumBrightness;
        private Double statisticMaximumBrightness;
        private Double statisticContrast;
        private Double statisticAverageHue;
        private Double statisticAverageSaturation;

        public IplImage GetImage {get{return image;}}
        public IplImage GetThresholdedImage { get { return thresholdedImage; } }
        public Int32 FullWidth { get { return fullWidth; } }
        public Int32 FullHeight { get { return fullHeight; } }
        public Int32 PieceHeight { get { return pieceHeight; } }
        public Int32 PieceWidth { get { return pieceWidth; } }
        public Double StatisticContrast { get { return statisticContrast; } }
        public Double StatisticAverageBrightness { get { return statisticAverageBrightness; } }
        public Double StatisticAverageHue { get { return statisticAverageHue; } }
        public Double StatisticAverageSaturation { get { return statisticAverageSaturation; } }
        public PositionInPlate PositionInPlate { get { return positionInPlate; } }


        public Char(String filePath)
        {
            
            image = new IplImage(filePath);
            IplImage origin = image.Clone();
            thresholdedImage = AdaptiveThresholding(image);
            image = origin;
            Init();
        }

        public Char(IplImage bi, IplImage pThresholdedImage, PositionInPlate pPositionInPlate)
        {
            image = bi;
            thresholdedImage = pThresholdedImage;
            positionInPlate = pPositionInPlate;
            Init();
        }

        public void ClearImage()
        {
            if (null != image)
            {
                Cv.ReleaseImage(image);
                image = null;
            }

            if (null != thresholdedImage)
            {
                Cv.ReleaseImage(thresholdedImage);
                thresholdedImage = null;
            }
           
        }

        public List<Double> ExtractFeatures()
        {
            return (ExtractMapFeatures());
        }

        public void Normalize()
        {
            if (normalized)
                return;

            IplImage tempImage = null;
            IplImage colorImage = image;
            image = thresholdedImage;

            // JIVAN
            //this.image.SaveImage(@"C:\temp\newImages\testImage.png");

            PixelMap pixelMap = GetPixelMap;
            bestPiece = pixelMap.GetBestPiece();

            tempImage = GetBestPieceInFullColor(colorImage, bestPiece);
            Cv.ReleaseImage(colorImage);
            colorImage = tempImage;

            ComputeStatisticBrightness(colorImage);
            ComputeStatisticContrast(colorImage);
            ComputeStatisticHue(colorImage);
            ComputeStatisticSaturation(colorImage);

            tempImage = pixelMap.Render(bestPiece);

            
            Cv.ReleaseImage(image);
            image = tempImage;

            if(null==image)
                image = new IplImage(1, 1, BitDepth.U8, 3 );

            pieceWidth = image.Width;
            pieceHeight = image.Height;

            NormalizeResizeOnly();
            normalized = true;


        }

        private IplImage ArrayToBufferedImage(Double[,] array, Int32 width, Int32 height, BitDepth bitDepth, Int32 nChannels)
        {
            IplImage newImage = new IplImage(new CvSize(width, height), bitDepth, nChannels);

            for (Int32 x = 0; x < width; x++)
            {
                for (Int32 y = 0; y < height; y++)
                {
                    SetBrightness(newImage, x, y, array[x, y]);
                }
            }


            return newImage;
        }

        private Double[,] BufferedImageToArray(IplImage image, Int32 width, Int32 height)
        {
            IplImage imageCopy = null;
            Double[,] value = new Double[width, height];

            try
            {
                imageCopy = new IplImage(image.Size, image.Depth, image.NChannels);
                Cv.CvtColor(image, imageCopy, ColorConversion.RgbToHsv);

                for (Int32 x = 0; x < width; x++)
                {
                    for (Int32 y = 0; y < height; y++)
                    {
                        value[x, y] = GetBrightness(imageCopy, x, y);
                    }
                }
            }
            finally
            {
                if (null != imageCopy)
                    Cv.ReleaseImage(imageCopy);
            }


            return value;
        }

        private void CopyBufferedImage(Double[,] srcArray, Int32 width, Int32 height, out Double[,] destArray)
        {
            destArray = new Double[width, height];

            for (Int32 x = 0; x < width; x++)
            {
                for (Int32 y = 0; y < height; y++)
                {
                    destArray[x, y] = srcArray[x, y];
                }
            }

        }

        private IplImage AdaptiveThresholding(IplImage image)
        {
            IplImage newImage = null;
            Int32 radius = Constants.ADAPTATIVE_THRESHOLDING_RADIUS;

            Int32 w = image.Width;
            Int32 h = image.Height;

            Double[,] destArray = null;

            Double[,] sourceArray = BufferedImageToArray(image, w, h);
            CopyBufferedImage(sourceArray, w, h, out destArray);


            Int32 count = 0;
            Double neighborhood = 0.0;

            for (Int32 x = 0; x < w; x++)
            {
                for (Int32 y = 0; y < h; y++)
                {

                    count = 0;
                    neighborhood = 0;
                    for (Int32 ix = x - radius; ix <= x + radius; ix++)
                    {
                        for (Int32 iy = y - radius; iy <= y + radius; iy++)
                        {
                            if (ix >= 0 && iy >= 0 && ix < w && iy < h)
                            {
                                neighborhood += sourceArray[ix, iy];
                                count++;
                            }

                        }
                    }
                    neighborhood /= count;

                    if (destArray[x, y] < neighborhood)
                    {
                        destArray[x, y] = 0f;
                    }
                    else
                    {
                        destArray[x, y] = 1f;
                    }
                }
            }


            newImage = ArrayToBufferedImage(destArray, w, h, image.Depth, image.NChannels);
            Cv.ReleaseImage(image);
            return newImage;

        }

        private List<Double> ExtractMapFeatures()
        {
            IplImage imageCopy = null;
            List<Double> vectorInput = new List<Double>();

            try
            {
                imageCopy = new IplImage(image.Size, image.Depth, image.NChannels);
                Cv.CvtColor(image, imageCopy, ColorConversion.RgbToHsv);

                for (Int32 y = 0; y < image.Height; y++)
                    for (Int32 x = 0; x < image.Width; x++)
                        vectorInput.Add(GetBrightness(imageCopy, x, y));
            }
            finally
            {
                if (null != imageCopy)
                    Cv.ReleaseImage(imageCopy);
            }

            return vectorInput;
        }

        private void NormalizeResizeOnly()
        { 

            Int32 x = Constants.INTELLIGENCE_CHAR_NORMALIZED_DIMENSIONS_X;
            Int32 y = Constants.INTELLIGENCE_CHAR_NORMALIZED_DIMENSIONS_Y;

            if (x == 0 || y == 0)
                return;
            
            LinearResize(x, y);
            NormalizeBrightness(0.5);
        }

        private void NormalizeBrightness(Double coef)
        {
            IplImage imageCopy = null;
            // JIVAN
            //image.SaveImage(@"C:\temp\newImages\normalizeImage.png");
            Statistics stats = new Statistics(image);
            try
            {
                imageCopy = new IplImage(image.Size, image.Depth, image.NChannels);
                Cv.CvtColor(image, imageCopy, ColorConversion.RgbToHsv);

                for (Int32 x = 0; x < image.Width; x++)
                {
                    for (Int32 y = 0; y < image.Height; y++)
                    {
                        SetBrightness(image, x, y,
                                stats.ThresholdBrightness(GetBrightness(imageCopy, x, y), coef));
                    }
                }
            }
            finally
            {
                if (null != imageCopy)
                    Cv.ReleaseImage(imageCopy);
            }
        }

        public void LinearResize(int width, int height)
        {
            IplImage tempImage;
            tempImage = LinearResizeBi(this.image, width, height);
            Cv.ReleaseImage(image);
            image = tempImage;
        }

        private void AverageResize(Int32 width, Int32 height)
        {
            IplImage tempImage;
            tempImage = AverageResizeBi(image, width, height);
            Cv.ReleaseImage(image);
            image = tempImage;
        }
        


        private IplImage LinearResizeBi(IplImage origin, Int32 width, Int32 height)
        {
            
            IplImage resizedImage = new IplImage(new CvSize(width, height), origin.Depth, origin.NChannels);
            Cv.Resize(origin,resizedImage, Interpolation.Linear);
           

           return resizedImage;

        }

        
        private IplImage AverageResizeBi(IplImage origin, Int32 width, Int32 height)
        {
            IplImage imageCopy = null;

            if (origin.Width < width || origin.Height < height)
                return LinearResizeBi(origin, width, height); 
           
            IplImage resized = new IplImage(width, height, BitDepth.U8, 3);

            Double xScale = (Double)(((Double)origin.Width) / ((Double)width));
            Double yScale = (Double)(((Double)origin.Height) / ((Double)height));

            try
            {
                imageCopy = new IplImage(origin.Size, origin.Depth, origin.NChannels);
                Cv.CvtColor(origin, imageCopy, ColorConversion.RgbToHsv);

                for (Int32 x = 0; x < width; x++)
                {
                    Int32 x0min = System.Convert.ToInt32(Math.Round(((Double)x) * xScale));
                    Int32 x0max = System.Convert.ToInt32(Math.Round(((Double)(x + 1)) * xScale));

                    for (Int32 y = 0; y < height; y++)
                    {
                        Int32 y0min = System.Convert.ToInt32(Math.Round(((Double)y) * yScale));
                        Int32 y0max = System.Convert.ToInt32(Math.Round(((Double)(y + 1)) * yScale));

                        Double sum = 0;
                        Int32 sumCount = 0;

                        for (Int32 x0 = x0min; x0 < x0max; x0++)
                        {
                            for (Int32 y0 = y0min; y0 < y0max; y0++)
                            {
                                sum += GetBrightness(imageCopy, x0, y0);
                                sumCount++;
                            }
                        }
                        sum /= sumCount;
                        SetBrightness(resized, x, y, sum);

                    }
                }

            }
            finally
            {
                if (null != imageCopy)
                    Cv.ReleaseImage(imageCopy);
            }

            return resized;
        }


        private void ComputeStatisticHue(IplImage image)
        {

            Double sum = 0.0;
            Int32 w = image.Width;
            Int32 h = image.Height;

            for (Int32 x = 0; x < w; x++)
            {
                for (Int32 y = 0; y < h; y++)
                {
                    sum += GetHue(image, x, y);
                }
            }


            statisticAverageHue = sum / (Double)((w * h));
        }

        private void ComputeStatisticSaturation(IplImage image)
        {
            IplImage imageCopy = null;
            Double sum = 0.0;
            Int32 w = image.Width;
            Int32 h = image.Height;

            try
            {
                imageCopy = new IplImage(image.Size, image.Depth, image.NChannels);
                Cv.CvtColor(image, imageCopy, ColorConversion.RgbToHsv);

                for (Int32 x = 0; x < w; x++)
                {
                    for (Int32 y = 0; y < h; y++)
                    {
                        sum += GetSaturation(imageCopy, x, y);
                    }
                }
            }
            finally
            {
                if (null != imageCopy)
                    Cv.ReleaseImage(imageCopy);
            }

            statisticAverageSaturation = sum / (Double)((w * h));
        }

        private void ComputeStatisticContrast(IplImage image)
        {
            IplImage imageCopy = null;
            Double sum = 0.0;
            Int32 w = image.Width;
            Int32 h = image.Height;

            try
            {
                imageCopy = new IplImage(image.Size, image.Depth, image.NChannels);
                Cv.CvtColor(image, imageCopy, ColorConversion.RgbToHsv);

                for (Int32 x = 0; x < w; x++)
                {
                    for (Int32 y = 0; y < h; y++)
                    {
                        sum += Math.Abs(statisticAverageBrightness - GetBrightness(imageCopy, x, y));
                    }
                }
            }
            finally
            {
                if (null != imageCopy)
                    Cv.ReleaseImage(imageCopy);
            }
            
            statisticContrast = sum / (Double)((w * h));
        }

        private void ComputeStatisticBrightness(IplImage image)
        {
            IplImage imageCopy = null;
            Double sum = 0.0;
            Double min = Double.PositiveInfinity;
            Double max = Double.NegativeInfinity;

            Int32 w = image.Width;
            Int32 h = image.Height;

            try
            {
                imageCopy = new IplImage(image.Size, image.Depth, image.NChannels);
                Cv.CvtColor(image, imageCopy, ColorConversion.RgbToHsv);

                for (Int32 x = 0; x < w; x++)
                {
                    for (Int32 y = 0; y < h; y++)
                    {
                        Double value = GetBrightness(imageCopy, x, y);
                        sum += value;
                        min = Math.Min(min, value);
                        max = Math.Max(max, value);
                    }
                }
            }
            finally
            {
                if (null != imageCopy)
                    Cv.ReleaseImage(imageCopy);
            }

            statisticAverageBrightness = sum / (Double)((w * h));
            statisticMinimumBrightness = min;
            statisticMaximumBrightness = max;
        }

        private IplImage GetBestPieceInFullColor(IplImage bi, PixelMap.Piece piece)
        {
            if (piece.width <= 0 || piece.height <= 0) 
                return bi.Clone();
            return (GetSubImage(bi, new CvRect(piece.mostLeftPoint, piece.mostTopPoint, piece.width, piece.height)));
        }



        private PixelMap GetPixelMap { get { return(new PixelMap(this.image));} }

     
        private void SetBrightness(IplImage image, Int32 x, Int32 y, Double newValue)
        {
            newValue = (newValue > 1.0) ? 255.0 : (newValue * 255.0);
            CvColor color = new CvColor((Byte)newValue, (Byte)newValue, (Byte)newValue);
            image[y, x] = color;
        }

        private Double GetBrightness(IplImage image, Int32 x, Int32 y)
        {
            Double value = 0.0;

            CvScalar cvValues = image[y, x];
            value = (Double)(cvValues.Val2 / 255.0);

            return value;
        }

        private Double GetHue(IplImage image, Int32 x, Int32 y)
        {
            Single value = 0.0f;
            Single max = 0.0f;
            Single min = 0.0f;
            Single delta = 0.0f;
            Single red = 0.0f;
            Single green = 0.0f;
            Single blue = 0.0f;

            CvColor cvColor = image[y, x];

            red = ((Single)cvColor.R) / 255.0f;
            green = ((Single)cvColor.G) / 255.0f;
            blue = ((Single)cvColor.B) / 255.0f;

            max = Math.Max(red,green);
            max = Math.Max(max, blue);

            min = Math.Min(red, green);
            min = Math.Min(min, blue);

            delta = max - min;

            if (delta == 0.0f)
                return 0.0;

            if (max == red)
            {
                value = (60.0f * (green - blue)) / delta;
            }
            else if (max == green)
            {
                value = 120.0f + ((60.0f * (blue - red)) / delta);
            }
            else
            {
                value = 240.0f + ((60.0f * (red - green)) / delta);
            }

            if (value >= 360.0f)
                value -= 359.0f;

            if (value < 0.0f)
                value += 360.0f;

            value /= 360.0f;

            return ((Double)(value));
        }

        private Double GetSaturation(IplImage image, Int32 x, Int32 y)
        {
            Double value = 0.0;

            CvScalar cvValues = image[y, x];
            value = (Double)(cvValues.Val1 / 255.0);

            return value;
        }

        

        private IplImage GetSubImage(IplImage image, CvRect rect)
        {
            IplImage imgDst = new IplImage(rect.Size, image.Depth, image.NChannels);

            image.SetROI(rect);
            Cv.Copy(image, imgDst);
            image.ResetROI();

            return imgDst;
        }

        private void Init()
        {
            fullWidth = image.Width;
            fullHeight = image.Height;
        }


    }
}
