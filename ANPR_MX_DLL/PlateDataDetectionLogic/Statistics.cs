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

using OpenCvSharp;

namespace ANPR_MX.PlateDataDetectionLogic
{
    public class Statistics
    {
        private Double maximum;
        private Double minimum;
        private Double average;
        private Double dispersion;
        private IplImage image;

        public Statistics (IplImage pImage)
        {
            image = pImage;
            Double sum = 0.0;
            Double sum2 = 0.0;
            Int32 w = image.Width;
            Int32 h = image.Height;
            IplImage imageCopy = null;
            

            try
            {
                imageCopy = new IplImage(image.Size, image.Depth, image.NChannels);
                Cv.CvtColor(image, imageCopy, ColorConversion.RgbToHsv);

                for (Int32 x = 0; x < w; x++)
                {
                    for (Int32 y = 0; y < h; y++)
                    {
                        Double pixelValue = GetBrightness(imageCopy, x, y);
                        maximum = Math.Max(pixelValue, maximum);
                        minimum = Math.Min(pixelValue, minimum);
                        sum += pixelValue;
                        sum2 += (pixelValue * pixelValue);
                    }
                }
            }
            finally
            {
                if (null != imageCopy)
                    Cv.ReleaseImage(imageCopy);
            }

            Double count = (Double)((w * h));
            average = sum / count;
            
            dispersion = (sum2 / count) - (average * average);
        }

        public Double ThresholdBrightness(Double value, Double coef)
        {
            Double output;
            
            if (value > average)
                output = coef + (1.0 - coef) * (value - average) / (maximum - average);
            else
                output = (1.0 - coef) * (value - minimum) / (average - minimum);

            return output;
        }

        private Double GetBrightness(IplImage image, Int32 x, Int32 y)
        {
            Double value = 0.0;

            CvScalar cvValues = image[y, x];
            value = (Double)(cvValues.Val2 / 255.0);

            return value;
        }
    }
}
