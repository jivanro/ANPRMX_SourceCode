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
using System.IO;

using ANPR_MX.Globals;

namespace ANPR_MX.PlateDataDetectionLogic
{
    public class KnnPatternClassificator : CharacterRecognizer
    {
        private List<List<Double>> learnVectors= null;

        public KnnPatternClassificator()
        {
            String path = Constants.INTELLIGENCE_CHAR_LEARN_ALPHABET_FILE;
            String alphaString = "0123456789abcdefghijklmnprstuvwxyz";

            learnVectors = new List<List<Double>>();
            for (Int32 i = 0; i < alphaString.Length; i++)
                learnVectors.Add(null);


            DirectoryInfo di = new DirectoryInfo(path);
            FileInfo[] rgFiles = di.GetFiles("*.jpg");
            FileInfo[] rgFiles2 = di.GetFiles("*.png");
            
            foreach (FileInfo fi in rgFiles.Concat(rgFiles2))
            {
                Int32 alphaPosition = alphaString.IndexOf(fi.Name.ToLower().ElementAt(0));
                if (-1 == alphaPosition)
                    continue;

                Char imgChar = new Char(path + Path.DirectorySeparatorChar + fi.Name);
                imgChar.Normalize();
                learnVectors[alphaPosition] = imgChar.ExtractFeatures();
                imgChar.ClearImage();
            }

            for (Int32 i = 0; i < alphaString.Length; i++)
                if (learnVectors[i] == null)
                    throw new IOException("Warning : alphabet in " + path + " is not complete");

        }

        public override RecognizedChar Recognize(Char chr)
        {
            List<Double> tested = chr.ExtractFeatures();
            

            RecognizedChar recognized = new RecognizedChar();

            for (int x = 0; x < learnVectors.Count; x++)
            {

                Double fx = SimplifiedEuclideanDistance(tested, learnVectors[x]);

                recognized.AddPattern(new CharacterRecognizer.RecognizedChar.RecognizedPattern(alphabet[x], fx));

            }

            recognized.Sort(0);
            return recognized;
        }


        private Double SimplifiedEuclideanDistance(List<Double> vectorA, List<Double> vectorB)
        {
            Double diff = 0.0;
            Double partialDiff;
            for (Int32 x = 0; x < vectorA.Count; x++)
            {
                partialDiff = Math.Abs(vectorA[x] - vectorB[x]);
                diff += partialDiff * partialDiff;
            }
            return diff;
        }
    }
}
