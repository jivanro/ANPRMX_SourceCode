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

namespace ANPR_MX.PlateDataDetectionLogic
{
    public class Graph
    {
        private Boolean actualAverageValue = false; 
        private Boolean actualMaximumValue = false; 
        private Boolean actualMinimumValue = false; 
        private Double averageValue;
        private Double maximumValue;
        private Double minimumValue;

        protected List<Double> yValues = null;
        protected List<Peak> peaks = null;

        public Graph()
        {
            yValues = new List<Double>();
        }

        public void ApplyProbabilityDistributor(Graph.ProbabilityDistributor probability)
        { 
          yValues =probability.Distribute(yValues);
          DeActualizeFlags();
        }

        public void AddPeak(Double value)
        {
            yValues.Add(value);
            DeActualizeFlags();
        }

        protected Int32 IndexOfLeftPeakRel(Int32 peak, Double peakFootConstantRel)
        {
            Int32 index = peak;
            for (Int32 i = peak; i >= 0; i--)
            {
                index = i;
                if (yValues[index] < peakFootConstantRel * yValues[peak])
                    break;
            }
            return Math.Max(0, index);
        }


        protected Int32 IndexOfRightPeakRel(Int32 peak, Double peakFootConstantRel)
        {
            Int32 index = peak;
            Int32 size = yValues.Count;
            for (Int32 i = peak; i < size; i++)
            {
                index = i;
                if (yValues[index] < peakFootConstantRel * yValues[peak]) 
                    break;
            }
            return Math.Min(size, index);
        }

        protected Boolean AllowedInterval(List<Peak> peaks, Int32 xPosition) 
        {
            foreach (Peak peak in peaks)
            {
                if (peak.Left <= xPosition && xPosition <= peak.Right) 
                    return false;
            }
            return true;
        }

        protected Double GetAverageValue(Int32 method)
        {
            if (!actualAverageValue)
            {
                averageValue = GetAverageValue(0, yValues.Count, method);
                actualAverageValue = true;
            }
            return averageValue;
        }

      
        private Double GetAverageValue(Int32 a, Int32 b, Int32 method)
        {
            Dictionary<Double, Int32> valuesStorage = new Dictionary<Double, Int32>();
            Double sum = 0.0;
            Double temp = Double.NegativeInfinity;
            Int32 tempCounter = 0;


            if (Constants.PEAK_AVERAGE_METHOD_NORMAL == method)
            {
                for (Int32 i = a; i < b; i++)
                {
                    sum += yValues[i];
                }
                return (sum / (Double)(yValues.Count()));
            }
            else
            {
                for (Int32 i = a; i < b; i++)
                {
                    if (valuesStorage.ContainsKey(yValues[i]))
                        valuesStorage[yValues[i]]++;
                    else if (0.0 < yValues[i])
                        valuesStorage.Add(yValues[i], 0);

                }

                if (valuesStorage.Count > 0)
                {
                    foreach (Double key in valuesStorage.Keys)
                    {
                        tempCounter = Math.Max(valuesStorage[key], tempCounter);
                        if (tempCounter == valuesStorage[key])
                        {
                            temp = key;
                        }
                    }

                    sum = temp;
                }
            }
            return sum;
        }


        protected Double GetMaxValue()
        {
            if (!actualMaximumValue)
            {
                maximumValue = GetMaxValue(0, yValues.Count);
                actualMaximumValue = true;
            }
            return maximumValue;
        }
        
        private Double GetMaxValue(int a, int b)
        {
            Double maxValue = 0.0;
            for (Int32 i = a; i < b; i++)
                maxValue = Math.Max(maxValue, yValues[i]);
            return maxValue;
        }

        protected Double GetMinValue()
        {
            if (!actualMinimumValue)
            { 
               minimumValue =GetMinValue(0,yValues.Count);
               actualMinimumValue = true;
            }

            return minimumValue;
        }

        private Double GetMinValue(Int32 a, Int32 b)
        {
            Double minValue = Double.PositiveInfinity;
            for (Int32 i = a; i < b; i++)
                minValue = Math.Min(minValue, yValues[i]);

            return minValue;
        }

        protected Int32 GetMinValueIndex(Int32 a, Int32 b)
        {
            Double minValue = Double.PositiveInfinity;
            Int32 minIndex = b;
            for (Int32 i = a; i < b; i++)
            {
                if (yValues[i] <= minValue)
                {
                    minValue = yValues[i];
                    minIndex = i;
                }
            }
            return minIndex;
        }   

        protected void DeActualizeFlags()
        {
            actualAverageValue = false;
            actualMaximumValue = false;
            actualMinimumValue = false;
        }

        public class Peak
        {
            private Int32 left;
            private Int32 center;
            private Int32 right;

            public Peak(Int32 pLeft, Int32 pCenter, Int32 pRight)
            {
                left = pLeft;
                center = pCenter;
                right = pRight;
            }

            public Peak(Int32 pLeft, Int32 pRight)
            {
                left = pLeft;
                right = pRight;
                center =(Int32) ((left + right) / 2);
            }

            public Int32 Left { get { return left; } }
            public Int32 Right { get { return right; } }
            public Int32 Center { get { return center; } }
            public Int32 GetDiff { get { return(right-left);} }

        }

        public class ProbabilityDistributor
        {
            private Double center;
            private Double power;
            private Int32 leftMargin;
            private Int32 rightMargin;

            public ProbabilityDistributor(Double pCenter, Double pPower, Int32 pLefTMargin, Int32 pRightMargin)
            {
                center = pCenter;
                power = pPower;
                leftMargin = (Int32)System.Math.Max(1.0,pLefTMargin);
                rightMargin = (Int32)System.Math.Max(1.0,pRightMargin);
            }

            public List<Double> Distribute(List<Double> peaks)
            {
                List<Double> distributedPeaks = new List<Double>();
                Int32 peaksSize = peaks.Count;

                for (Int32 i = 0; i < peaksSize; i++)
                {
                    if (i < leftMargin || i > peaksSize - rightMargin)
                    {
                        distributedPeaks.Add(0.0);
                    }
                    else 
                    { 
                      distributedPeaks.Add(DistributionFunction(peaks[i],(Double)(((Double)(i) ) / ((Double) peaksSize) )));
                    }
                }

                return distributedPeaks;
            }

            private Double DistributionFunction(Double value, Double positionPercentage)
            {
                Double res = 0.0;

                res = value * (1.0 - power * System.Math.Abs(positionPercentage - center));

                return res;
            }
        }
    }
}
