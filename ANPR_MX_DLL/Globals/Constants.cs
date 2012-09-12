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

// If you want to alter upon this work, you MUST attribute it in 
// a) all source files
// b) on every place, where is the copyright of derivated work
// exactly by the following label :

// ---- label begin ----
// This work is a derivate of ANPR_MX. 
// ANPR_MX is an intellectual property of Jivan Miranda Rodriguez
// ----  label end  ----


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ANPR_MX.Globals
{
    public  class Constants
    {
        public const Int32 CAPTURE_HEIGHT_PX = 600;//480;//720;//480;//720;//600;
        public const Int32 CAPTURE_WIDTH_PX = 800;//640;//960;//640;//960;//800;

        public const Int32 HEIGHT_480PX = 480;
        public const Int32 WIDTH_640PX = 640;
        public const Int32 WIDTH_800PX = 800;
        public const Int32 HEIGHT_600PX = 600;
        
        public const Double FPS_25 = 25.0;

        public const Int32 MAX_NUMBER_OF_CHARACTERS_EXTRACTED_FROM_PLATE = 15;
        public const Int32 PLATEGRAPH_DETECTION_MODE = 1; //1 = Edge Detection
        public const Int32 ADAPTATIVE_THRESHOLDING_RADIUS = 7;

        public const Double PLATE_VERTICAL_GRAPH_PEAK_FOOT_CONSTANT = 0.42;
        public const Double PLATE_HORIZONTAL_GRAPH_PEAK_FOOT_CONSTANT = 0.05;

        public const Int32 PLATE_HORIZONTAL_GRAPH_DETECTION_TYPE = 1; //Edge Detection

        public const Double MIN_PLATE_WIDTH_HEIGHT_RATIO = 0.5;
        public const Double MAX_PLATE_WIDTH_HEIGHT_RATIO = 20.0;//15.0;

        public const Double PLATEGRAPH_REL_MINPEAKSIZE = 1.0;//0.86;
        public const Double PLATEGRAPH_PEAK_FOOT_CONSTANT = 1.0;//0.7;

        public const Int32 INTELLIGENCE_MIN_CHARS = 3;
        public const Int32 INTELLIGENCE_MAX_CHARS = 12;

        public const Int32 LIST_PLATE_MIN_CHARS = 4;

        public const Double INTELLIGENCE_MAX_CHAR_WIDTH_DISP = 0.9;//0.67;// 0.55;

        public const Int32  INTELLIGENCE_CHAR_NORMALIZED_DIMENSIONS_X= 7;
        public const Int32  INTELLIGENCE_CHAR_NORMALIZED_DIMENSIONS_Y = 17;

        public const Double INTELLIGENCE_MIN_CHAR_WIDTH_HEIGHT_RATIO = 0.1;
        public const Double INTELLIGENCE_MAX_CHAR_WIDTH_HEIGHT_RATIO = 1.0;

        public const Double INTELLIGENCE_MAX_BRIGHTNESS_COST_DISPERSION = 0.32;
        public const Double INTELLIGENCE_MAX_CONTRAST_COST_DISPERSION = 0.19;
        public const Double INTELLIGENCE_MAX_HUE_COST_DISPERSION = 0.145;
        public const Double INTELLIGENCE_MAX_SATURATION_COST_DISPERSION = 0.24;
        public const Double INTELLIGENCE_MAX_HIGHT_COST_DISPERSION = 0.2;
        public const Double INTELLIGENCE_MAX_SIMILARITY_COST_DISPERSION = 100.0;

        public const Int32 INTELLIGENCE_SYNTAX_ANALYSIS_MODE = 2;

        public const String INTELLIGENCE_SYNTAX_DESCRIPTION_FILE = @"Resources\syntax.xml";
        public const String INTELLIGENCE_CHAR_LEARN_ALPHABET_FILE = @"Resources\alphabets\alphabet_7x17";


        public const Int32 PEAK_AVERAGE_METHOD_NORMAL = 0;
        public const Int32 PEAK_AVERAGE_METHOD_MODAL = 1;

        public const Int32 PREPROCESS_IMAGE_TYPE_NORMAL = 0;
        public const Int32 PREPROCESS_IMAGE_TYPE_EXTENDED = 1;

        public const Int32 OK = 0;
        public const Int32 ERROR = -1; //Generic error
        
        public const Int32 ALL_METHODS = 0;
        public const Int32 EXCLUDE_METHOD = 1;
        

        /******************************************
         * Error Codes start from 5000
         * ****************************************/

        public const Int32 CAMERA_NOT_FOUND_OR_ERROR_WHILE_ACTIVATING_CAMERA = 5000;

    }
}
