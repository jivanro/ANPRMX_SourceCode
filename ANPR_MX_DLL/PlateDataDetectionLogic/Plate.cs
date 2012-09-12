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

using ANPR_MX.Globals;
using OpenCvSharp;

namespace ANPR_MX.PlateDataDetectionLogic
{
    public class Plate
    {
        private IplImage image = null;
        private Plate plateCopy = null;
        private Int32 numberOfCandidates = Constants.MAX_NUMBER_OF_CHARACTERS_EXTRACTED_FROM_PLATE;
        private Int32 horizontalDetectionType = Constants.PLATEGRAPH_DETECTION_MODE;
        private Graph.ProbabilityDistributor distributor = new Graph.ProbabilityDistributor(0.0, 0.0, 0, 0);
        private PlateGraph graphHandle = null;

        public IplImage GetImage { get { return image; } set { image = value; } }
        public Int32 GetWidth { get { return image.Width; } }
        public Int32 GetHeight { get { return image.Height; } }
        public Int32 GetSquare { get { return (image.Width*image.Height); } }

        public Plate() { image = null; plateCopy = null; }

        public Plate(IplImage pImage)
        {
            image = pImage.Clone();
            plateCopy = new Plate(image, true);
            plateCopy.image = AdaptiveThresholding(plateCopy.image);
          
        }

        public Plate(IplImage pImage, Boolean isCopy)
        {
            image = pImage.Clone();
        }


        public List<Char> GetChars()
        {
            List<Char> output = new List<Char>();

            List<Graph.Peak> peaks = ComputeGraph();
            Int32 peakSize = (null==peaks)?0:peaks.Count;

            for(Int32 i=0; i<peakSize;i++)
            {
                Graph.Peak p = peaks[i];
                if (p.GetDiff <= 0)
                    continue;
                
                output.Add(new Char(GetSubImage(image,new CvRect(p.Left,0,p.GetDiff, image.Height)),
                                    GetSubImage(plateCopy.image,new CvRect(p.Left,0,p.GetDiff, image.Height)),
                                    new PositionInPlate(p.Left, p.Right)));
            }

            return output;
        }

        public Double GetAveragePieceSaturation(List<Char> chars)
        {
            Double averageSaturation = 0.0;

            foreach (Char chr in chars)
                averageSaturation += chr.StatisticAverageSaturation;

            averageSaturation /= ((Double)chars.Count);

            return averageSaturation;
        }

        public Double GetAveragePieceHue(List<Char> chars)
        {
            Double averageHue = 0.0;

            foreach (Char chr in chars)
                averageHue += chr.StatisticAverageHue;

            averageHue /= ((Double)chars.Count);

            return averageHue;
        }  

        public Double GetAveragePieceBrightness(List<Char> chars)
        {
            Double averageBrightness = 0.0;

            foreach (Char chr in chars)
                averageBrightness += chr.StatisticAverageBrightness;

            averageBrightness /= ((Double)chars.Count);
            return averageBrightness;
        }

        public Double GetAveragePieceHeight(List<Char> chars)
        {
            Double averageHeight = 0.0;

            foreach (Char chr in chars)
                averageHeight += ((Double)chr.PieceHeight);

            averageHeight /= ((Double)chars.Count);
            return averageHeight;
        }

        public Double GetAveragePieceContrast(List<Char> chars)
        {
            Double averageContrast = 0.0;

            foreach (Char chr in chars)
                averageContrast += chr.StatisticContrast;

            averageContrast /= ((Double)chars.Count);
            return averageContrast;
        }  

        public Double GetCharsWidthDispertion(List<Char> chars)
        {
            Double averageDispersion = 0.0;
            Double averageWidth = GetAverageCharWidth(chars);

            foreach (Char chr in chars)
            {
                averageDispersion += (Math.Abs(averageWidth - ((Double)chr.FullWidth)));
            }
            averageDispersion /= ((Double)chars.Count);

            return (averageDispersion / averageWidth);
        }

        public void SavePlateCopyImage(String filePath)
        {
            if(null!=plateCopy.image)
                    plateCopy.image.SaveImage(filePath);
        }

        public void SaveMainImage(String filePath)
        {
            if (null != image)
                image.SaveImage(filePath);
        }

        public void ClearImage()
        {
            if (null != image)
            {
                Cv.ReleaseImage(image);
                image = null;
            }
            if (null != plateCopy)
            {
                plateCopy.ClearImage();
                plateCopy = null;
            }
        }

        public void Normalize(Int32 method)
        {
            IplImage tempImage = null;
            Plate clone1 = this.Clone();
            VerticalEdgeDetector(clone1.GetImage);
           //clone1.GetImage.SaveImage(@"C:\temp\newImages\newVerticalEdgeDetector.png");
            PlateVerticalGraph vertical = clone1.HistogramYaxis(clone1.GetImage);
            tempImage = CutTopBottom(image, vertical);
            Cv.ReleaseImage(image);
            image = tempImage;
            // JIVAN
            //plateCopy.image.SaveImage(@"C:\temp\newImages\newBEFOREVerticalCutTopBottom.png");
            tempImage = CutTopBottom(plateCopy.image, vertical);
            Cv.ReleaseImage(plateCopy.image);
            plateCopy.image = tempImage;
            // JIVAN
            //plateCopy.image.SaveImage(@"C:\temp\newImages\newVerticalCutTopBottom.png");
            Plate clone2 = this.Clone();
            if (1 == horizontalDetectionType)
            {
               
                HorizontalEdgeDetector(clone2.GetImage);
                //TODO JIVAN Remove these 2 lines when project gest done
                //clone2.ClearImage();
                //clone2 = new Plate(new IplImage(@"C:\temp\HorizontalEdgeDetector.png"));
                //clone2.GetImage.SaveImage(@"C:\temp\newImages\newCloneHorizontalEdgeDetector.png");
            }

            PlateHorizontalGraph horizontal = clone1.HistogramXaxis(clone2.GetImage);
            tempImage = CutLeftRight(image, horizontal, method);
            Cv.ReleaseImage(image);
            image = tempImage;
            tempImage = CutLeftRight(plateCopy.image, horizontal, method);
            Cv.ReleaseImage(plateCopy.image);
            plateCopy.image = tempImage;
            // JIVAN
            //plateCopy.image.SaveImage(@"C:\temp\newImages\newHorizontalCutLeftRight.png");
            
            clone2.ClearImage();
            clone1.ClearImage();
            
        }

        public IplImage AdaptiveThresholding(IplImage image)
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
                                neighborhood += sourceArray[ix,iy];
                                count++;
                            }

                        }
                    }
                    neighborhood /= count;
                    
                    if (destArray[x,y] < neighborhood)
                    {
                        destArray[x,y] = 0f;
                    }
                    else
                    {
                        destArray[x,y] = 1f;
                    }
                }
            }

            
            newImage = ArrayToBufferedImage(destArray, w, h, image.Depth, image.NChannels);
            Cv.ReleaseImage(image);
            return newImage;
            
        }


        private Double GetAverageCharWidth(List<Char> chars)
        {
            Double averageWidth = 0;
            foreach (Char chr in chars)
            {
                averageWidth += ((Double)chr.FullWidth);
            }
            averageWidth /= ((Double)chars.Count);
            return averageWidth;
        }

        private List<Graph.Peak> ComputeGraph()
        {
            if (null != graphHandle)
                return graphHandle.GetPeaks;

            graphHandle = Histogram(plateCopy.image);
            graphHandle.ApplyProbabilityDistributor(distributor);
            graphHandle.FindPeaks(numberOfCandidates);

            return graphHandle.GetPeaks;
        }

        private IplImage CutTopBottom(IplImage origin, PlateVerticalGraph graph)
        { 
           graph.ApplyProbabilityDistributor(new Graph.ProbabilityDistributor(0.0,0.0,2,2));
           Graph.Peak p = graph.FindPeak(3)[0];
           return (GetSubImage(origin, new CvRect(0,p.Left,origin.Width,p.GetDiff)));
        }

        private IplImage CutLeftRight(IplImage origin, PlateHorizontalGraph graph, Int32 method)
        {
            graph.ApplyProbabilityDistributor(new Graph.ProbabilityDistributor(0.0, 0.0, 2, 2));
            List<Graph.Peak> peaks = graph.FindPeak(3, method);

            if (peaks.Count != 0)
            {
                Graph.Peak p = peaks[0];
                return (GetSubImage(origin,new CvRect(p.Left, 0, p.GetDiff, image.Height)));
            }
            else
              return origin.Clone();
        }

        private IplImage GetSubImage(IplImage image, CvRect rect)
        {
            IplImage imgDst = new IplImage(rect.Size, image.Depth, image.NChannels);

            image.SetROI(rect);
            Cv.Copy(image, imgDst);
            image.ResetROI();

            return imgDst;
        }


        private PlateGraph Histogram(IplImage image)
        {
            Int32 startY;
            IplImage imageCopy = null;
            PlateGraph graph = new PlateGraph(this);

            try
            {
                imageCopy = new IplImage(image.Size, image.Depth, image.NChannels);
                Cv.CvtColor(image, imageCopy, ColorConversion.RgbToHsv);
                startY = (image.Height < 3 ? 0 : 3);

                for (Int32 x = 0; x < image.Width; x++)
                {
                    Double counter = 0;
                    for (Int32 y = startY; y < image.Height; y++)
                        counter += GetBrightness(imageCopy, x, y);
                    graph.AddPeak(counter);
                }
            }
            finally
            {
                if (null != imageCopy)
                    Cv.ReleaseImage(imageCopy);
            }
            return graph;
        }

        private PlateGraph Histogram_Old(IplImage image)
        {
            IplImage imageCopy = null;
            PlateGraph graph = new PlateGraph(this);

            try
            {
                imageCopy = new IplImage(image.Size, image.Depth, image.NChannels);
                Cv.CvtColor(image, imageCopy, ColorConversion.RgbToHsv);

                for (Int32 x = 0; x < image.Width; x++)
                {
                    Double counter = 0;
                    for (Int32 y = 0; y < image.Height; y++)
                        counter += GetBrightness(imageCopy, x, y);
                    graph.AddPeak(counter);
                }
            }
            finally
            {
                if (null != imageCopy)
                    Cv.ReleaseImage(imageCopy);
            }
            return graph;
        }

        private PlateHorizontalGraph HistogramXaxis(IplImage image)
        {
            PlateHorizontalGraph graph = new PlateHorizontalGraph(this);
            IplImage imageCopy = null;

            Int32 w = image.Width;
            Int32 h = image.Height;

            try
            {
                imageCopy = new IplImage(image.Size, image.Depth, image.NChannels);
                Cv.CvtColor(image, imageCopy, ColorConversion.RgbToHsv);

                for (Int32 x = 0; x < w; x++)
                {
                    Double counter = 0;
                    for (Int32 y = 0; y < h; y++)
                    {
                        counter += GetBrightness(imageCopy, x, y);
                    }
                    graph.AddPeak(counter);
                }
            }
            finally
            { 
                if (null != imageCopy)
                    Cv.ReleaseImage(imageCopy);
            }

            return graph;
        } 

        private PlateVerticalGraph HistogramYaxis(IplImage image)
        {
            PlateVerticalGraph graph = new PlateVerticalGraph(this);
            IplImage imageCopy = null;

            Int32 w = image.Width;
            Int32 h = image.Height;

            try
            {
                imageCopy = new IplImage(image.Size, image.Depth, image.NChannels);
                Cv.CvtColor(image, imageCopy, ColorConversion.RgbToHsv);

                for (Int32 y = 0; y < h; y++)
                {
                    Double counter = 0.0;
                    for (Int32 x = 0; x < w; x++)
                    {
                        counter += GetBrightness(imageCopy, x, y);
                    }
                    graph.AddPeak(counter);
                }
            }
            finally
            {
                if (null != imageCopy)
                    Cv.ReleaseImage(imageCopy);
            }

                return graph;
        }

        private void HorizontalEdgeDetector(IplImage image)
        {
            IplImage tmpImage = null;
            Double[] matrix = { 1.0,2.0,1.0,
                                0.0,0.0,0.0,
                                -1.0,-2.0,-1.0 };
            try
            {
                tmpImage = image.Clone();
                CvMat kernel = new CvMat(3, 3, MatrixType.F64C1, matrix);
                Cv.Filter2D(tmpImage, image, kernel);

            }
            finally
            {
                if (null != tmpImage)
                    Cv.ReleaseImage(tmpImage);
            }
        }

        private void VerticalEdgeDetector(IplImage image)
        {
          IplImage tmpImage = null;
          Double[] matrix = {1.0,0.0,-1.0};
          try
          {
              tmpImage = image.Clone();
              CvMat kernel = new CvMat(1, 3, MatrixType.F64C1, matrix);
              Cv.Filter2D(tmpImage, image, kernel);
              
          }
          finally
          {
              if (null != tmpImage)
                  Cv.ReleaseImage(tmpImage);
          }
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

        private IplImage ArrayToBufferedImage(Double[,] array, Int32 width, Int32 height, BitDepth bitDepth, Int32 nChannels)
        {
            IplImage newImage= new IplImage(new CvSize(width, height), bitDepth, nChannels);

            for (Int32 x = 0; x < width; x++)
            {
                for (Int32 y = 0; y < height; y++)
                {
                    SetBrightness(newImage, x, y, array[x,y]);
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

        private Plate Clone()
        {
            Plate newPlate = null;

            newPlate = new Plate(this.image);

            return newPlate;
        }

        private Double GetBrightness(IplImage image, Int32 x, Int32 y)
        {
            Double value = 0.0;

            CvScalar cvValues = image[y, x];
            value = (Double)(cvValues.Val2/255.0);
            
            return value;
        }

        private void SetBrightness(IplImage image, Int32 x, Int32 y, Double newValue)
        {
            newValue = (newValue > 1.0) ? 255.0 : (newValue*255.0);
            CvColor color = new CvColor((Byte)newValue, (Byte)newValue, (Byte)newValue);
            image[y, x] = color;
        }

        private Double GetSaturation(IplImage image, Int32 x, Int32 y)
        {
            Double value = 0.0;

            IplImage imageCopy = new IplImage(image.Size, image.Depth, image.NChannels);
            try
            {
                Cv.CvtColor(image, imageCopy, ColorConversion.RgbToHsv);
                CvScalar cvValues = imageCopy[y, x];
                value = cvValues.Val1/255.0;
            }
            finally
            {
                if (null != imageCopy)
                    Cv.ReleaseImage(imageCopy);
            }

            return value;
        }

        private Double GetHue(IplImage image, Int32 x, Int32 y)
        {
            Double value = 0.0;

            IplImage imageCopy = new IplImage(image.Size, image.Depth, image.NChannels);
            try
            {
                Cv.CvtColor(image, imageCopy, ColorConversion.RgbToHsv);
                CvScalar cvValues = imageCopy[y, x];
                value = (cvValues.Val0/180.0)*3.6;
            }
            finally
            {
                if (null != imageCopy)
                    Cv.ReleaseImage(imageCopy);
            }

            return value;
        }

    }
}
