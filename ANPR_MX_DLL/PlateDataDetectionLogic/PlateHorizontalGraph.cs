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
    public class PlateHorizontalGraph : Graph
    {
        
        private Int32 horizontalDetectionType = Constants.PLATE_HORIZONTAL_GRAPH_DETECTION_TYPE;

        Plate handle;

        public PlateHorizontalGraph(Plate pHandle)
        {
            handle = pHandle;
        }

        public List<Peak> FindPeak(Int32 count, Int32 method)
        {
            if (horizontalDetectionType == 1)
                return FindPeak_edgedetection(count, method);
            return FindPeak_derivate(count);
        }

        private List<Peak> FindPeak_derivate(Int32 count)
        {  
            Int32 a, b;
            Double maxVal = GetMaxValue();

            for (a = 2; -Derivation(a, a + 4) < maxVal * 0.2 && a < yValues.Count - 2 - 2 - 4; a++) ;
            for (b = yValues.Count - 1 - 2; Derivation(b - 4, b) < maxVal * 0.2 && b > a + 2; b--) ;

            List<Peak> outPeaks = new List<Peak>();

            outPeaks.Add(new Peak(a, b));
            base.peaks = outPeaks;
            return outPeaks;
        }

        private List<Peak> FindPeak_edgedetection(Int32 count, Int32 method)
        {
            Double average = GetAverageValue(method);
            
            Int32 a, b, c;
            for (b = yValues.Count - 1; yValues[b] < average; b--) ;
            for (a = 0; (a < yValues.Count) && (yValues[a] == 0.0); a++) ;
            for (c=0; (a < yValues.Count) && (yValues[a] >= 1.0); a++, c++) ;
             if(a>=(yValues.Count - 1) || a>=b || c <41)
              for (a = 0; yValues[a] < average; a++) ;
                                   

            List<Peak> outPeaks = new List<Peak>();
            a = Math.Max(a, 0);
            b = Math.Min(b + 5, yValues.Count);

            outPeaks.Add(new Peak(a, b));
            base.peaks = outPeaks;
            return outPeaks;
        }

        private List<Peak> FindPeak_edgedetection_old(Int32 count, Int32 method)
        {
            Double average = GetAverageValue(method);
            Int32 a, b;
            for (a = 0; yValues[a] < average; a++) ;
            for (b = yValues.Count - 1; yValues[b] < average; b--) ;

            List<Peak> outPeaks = new List<Peak>();
            a = Math.Max(a - 5, 0);
            b = Math.Min(b + 5, yValues.Count);

            outPeaks.Add(new Peak(a, b));
            base.peaks = outPeaks;
            return outPeaks;
        }

        private Double Derivation(Int32 index1, Int32 index2)
        {
            return (yValues[index1] - yValues[index2]);
        }
    }
}
