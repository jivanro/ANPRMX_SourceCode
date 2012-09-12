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

using ANPR_MX.Globals;

namespace ANPR_MX.PlateDataDetectionLogic
{
    public class MainLogic
    {
        private CharacterRecognizer chrRecog = null;
        private Parser parser = null;

        public MainLogic() {

            chrRecog = new KnnPatternClassificator();
            parser = new Parser();
        }

        public String ImageAnalysis(IplImage image, Int32 method)
        {
            String plateData = null;
            Double plateWHratio;
            Plate plate = null;
            List<Char> chars = null;
            

            try
            {
                plate = new Plate(image);
                // JIVAN
                //plate.SavePlateCopyImage(@"C:\temp\newImages\newImageAdaptativeThreshold1.png");
                plate.Normalize(method);

                plateWHratio = (Double)((Double)plate.GetWidth / (Double)plate.GetHeight);
                if (plateWHratio < Constants.MIN_PLATE_WIDTH_HEIGHT_RATIO ||
                    plateWHratio > Constants.MAX_PLATE_WIDTH_HEIGHT_RATIO)
                    return plateData;

                // JIVAN
                //plate.SavePlateCopyImage(@"C:\temp\newImages\newImageAdaptativeThreshold2.png");
                chars = plate.GetChars();
                
                //  JIVAN
                /*Int32 count2 =0;
                foreach(Char ch in chars)
                {
                    ch.GetImage.SaveImage(String.Format(@"C:\temp\newImages\newBeforeNormImageCh{0}.png", count2));
                    ch.GetThresholdedImage.SaveImage(String.Format(@"C:\temp\newImages\newImageThresholdedCh{0}.png", count2++));
                }
                */

                if (chars.Count < Constants.INTELLIGENCE_MIN_CHARS ||
                   chars.Count > Constants.INTELLIGENCE_MAX_CHARS)
                    return plateData;

                if (plate.GetCharsWidthDispertion(chars) > Constants.INTELLIGENCE_MAX_CHAR_WIDTH_DISP)
                    return plateData;

                RecognizedPlate recognizedPlate = new RecognizedPlate();
                //Int32 count =0;
                foreach (Char ch in chars)
                {
                    ch.Normalize();
                    // JIVAN
                    //ch.GetImage.SaveImage(String.Format(@"C:\temp\newImages\newNormImageCh{0}.png",count++));
                    
                }

                Double averageHeight = plate.GetAveragePieceHeight(chars);
                Double averageContrast = plate.GetAveragePieceContrast(chars);
                Double averageBrightness = plate.GetAveragePieceBrightness(chars);
                Double averageHue = plate.GetAveragePieceHue(chars);
                Double averageSaturation = plate.GetAveragePieceSaturation(chars);

                foreach (Char chr in chars)
                {

                    
                    Double widthHeightRatio = (Double)(chr.PieceWidth);
                    widthHeightRatio /= (Double)(chr.PieceHeight);

                    if (widthHeightRatio < Constants.INTELLIGENCE_MIN_CHAR_WIDTH_HEIGHT_RATIO ||
                        widthHeightRatio > Constants.INTELLIGENCE_MAX_CHAR_WIDTH_HEIGHT_RATIO)
                        continue;


                    if ((chr.PositionInPlate.X1 < 2 ||
                            chr.PositionInPlate.X2 > plate.GetWidth - 1)
                            && widthHeightRatio < 0.12)
                        continue;

                    Double contrastCost = Math.Abs(chr.StatisticContrast - averageContrast);
                    Double brightnessCost = Math.Abs(chr.StatisticAverageBrightness - averageBrightness);
                    Double hueCost = Math.Abs(chr.StatisticAverageHue - averageHue);
                    Double saturationCost = Math.Abs(chr.StatisticAverageSaturation - averageSaturation);
                    Double heightCost = (chr.PieceHeight - averageHeight) / averageHeight;

                    if (brightnessCost > Constants.INTELLIGENCE_MAX_BRIGHTNESS_COST_DISPERSION)
                        continue;

                    if (contrastCost > Constants.INTELLIGENCE_MAX_CONTRAST_COST_DISPERSION)
                        continue;

                    if (hueCost > Constants.INTELLIGENCE_MAX_HUE_COST_DISPERSION)
                        continue;

                    if (saturationCost > Constants.INTELLIGENCE_MAX_SATURATION_COST_DISPERSION)
                        continue;

                    if (heightCost < -(Constants.INTELLIGENCE_MAX_HIGHT_COST_DISPERSION))
                        continue;

                    Double similarityCost = 0.0;
                    CharacterRecognizer.RecognizedChar rc = null;

                    rc = chrRecog.Recognize(chr);
                    similarityCost = rc.GetPatterns()[0].GetCost;
                    if (similarityCost <= Constants.INTELLIGENCE_MAX_SIMILARITY_COST_DISPERSION)
                        recognizedPlate.AddChar(rc);

                }

                if (recognizedPlate.GetChars.Count < Constants.INTELLIGENCE_MIN_CHARS)
                    return plateData;

                plateData = parser.Parse(recognizedPlate, Constants.INTELLIGENCE_SYNTAX_ANALYSIS_MODE);

                

            }
            finally
            {
               
                if (null != chars)
                {
                    foreach (Char ch in chars)
                    {
                        ch.ClearImage();
                    }
                }

                if (null != plate)
                    plate.ClearImage();
            }

            return plateData;
        }

    }
}
