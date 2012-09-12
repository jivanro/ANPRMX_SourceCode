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

namespace ANPR_MX.PlateDataDetectionLogic
{
    public abstract class CharacterRecognizer
    {

        public System.Char[] alphabet = {
        '0','1','2','3','4','5','6','7','8','9','A','B','C',
        'D','E','F','G','H','I','J','K','L','M','N','P',
        'R','S','T','U','V','W','X','Y','Z'};

       

        public class RecognizedChar
        {

            private List<RecognizedPattern> patterns= null;
            private Boolean isSorted;

            public Boolean IsSorted { get { return isSorted; } }

            public RecognizedChar()
            {
                patterns = new List<RecognizedPattern>();
                isSorted = false;
            }
            
            public void AddPattern(RecognizedPattern pattern)
            {
                patterns.Add(pattern);
            }



            public void Sort(Int32 direction)
            {
                if (isSorted) 
                    return;
                patterns.Sort(new PatternComparer(direction));
                isSorted = true;

            }

            public List<RecognizedPattern> GetPatterns()
            {
                if (isSorted)
                    return patterns;
                return null; 
            }

            public RecognizedPattern GetPattern(Int32 i)
            {
                if (isSorted) 
                    return patterns[i];
                return null;
            }

            public class RecognizedPattern
            {
                private System.Char chr;
                private Double cost;

                public RecognizedPattern(System.Char pChr, Double pValue)
                {
                    chr = pChr;
                    cost = pValue;
                }
                public System.Char GetChar  { get { return chr; }  }
                public Double GetCost   { get { return cost; }  }
            }

            public class PatternComparer : IComparer<RecognizedPattern>
            {
                Int32 direction;
                public PatternComparer(Int32 pDirection)
                {
                    direction = pDirection;
                }
                public Int32 Compare(RecognizedPattern o1, RecognizedPattern o2)
                {
                    Double cost1 = o1.GetCost;
                    Double cost2 = o2.GetCost;

                    Int32 ret = 0;

                    if (cost1 < cost2) ret = -1;
                    if (cost1 > cost2) ret = 1;
                    if (direction == 1) ret *= -1;
                    
                    return ret;
                }
            }

        }

        public CharacterRecognizer()
        {

        }

        public abstract RecognizedChar Recognize(Char chr);
    }
}
