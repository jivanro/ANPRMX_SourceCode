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
using System.Xml;
using System.Xml.Linq;

using ANPR_MX.Globals;

namespace ANPR_MX.PlateDataDetectionLogic
{
    public class Parser
    {
        List<PlateForm> plateForms = null;

        public class PlateForm
        {
            private List<Position> positions = null;
            private String name = null;

            public Boolean flagged = false;

            public PlateForm(String pName)
            {
                name = pName;
                positions = new List<Position>();
            }

            public void AddPosition(Position p)
            {
                positions.Add(p);
            }

            public Position GetPosition(Int32 index)
            {
                return (positions[index]);
            }

            public Int32 Length
            {
                get { return (positions.Count); }
            }

           

            public class Position
            {
                private System.Char[] allowedChars;

                public Position(String data)
                {
                    allowedChars = data.ToCharArray();
                }

                public Boolean IsAllowed(System.Char chr)
                {
                    Boolean ret = false;
                    for (Int32 i = 0; i < allowedChars.Count(); i++)
                    {
                        if (allowedChars[i] == chr)
                        {
                            ret = true;
                            break;
                        }
                    }
                    return ret;
                }

           


            }


           
        }

        public class FinalPlate
        {
            private StringBuilder plate = null;
            private Double requiredChanges = 0.0;

            public Double RequiredChanges { get { return requiredChanges; } }


            public void AddRequiredChanges(Double value)
            {
                requiredChanges+=value;
            }

            public void IncrementRequiredChanges()
            {
              requiredChanges++;
            }

            public String GetPlate()
            {
                return (plate.ToString());
            }

            public FinalPlate()
            {
                plate = new StringBuilder();
            }

            public void AddChar(System.Char chr)
            {
                plate.Append(chr);
            }
        }

        public Parser()
        {
            plateForms = new List<PlateForm>();
            plateForms = LoadFromXml(Constants.INTELLIGENCE_SYNTAX_DESCRIPTION_FILE);
        }

   

        public List<PlateForm> LoadFromXml(String fileName)
        {
            List<PlateForm> plateForms = new List<PlateForm>();

            XDocument doc = XDocument.Load(fileName);
            foreach (XElement element in doc.Element("structure").Elements())
            {
                PlateForm form = new PlateForm((String)(element.Attribute("name")));
                foreach (XElement insideElement in element.Elements())
                {
                    String content = (String)insideElement.Attribute("content");
                    form.AddPosition(new PlateForm.Position(content.ToUpper()));
                }
                plateForms.Add(form);
            }

            return plateForms;
        }


    public void UnFlagAll() {
        foreach (PlateForm form in plateForms)
            form.flagged = false;
    }


        public String Parse(RecognizedPlate recognizedPlate, Int32 syntaxAnalysisMode)
        {

            Int32 length = recognizedPlate.GetChars.Count;
            UnFlagAll();
            FlagEqualOrShorterLength(length);

            List<FinalPlate> finalPlates = new List<FinalPlate>();

            foreach (PlateForm form in plateForms)
            {
                if (!form.flagged)
                    continue;

                for (Int32 i = 0; i <= length - form.Length; i++)
                {
                    FinalPlate finalPlate = new FinalPlate();
                    for (Int32 ii = 0; ii < form.Length; ii++)
                    {

                        CharacterRecognizer.RecognizedChar rc = recognizedPlate.GetChar(ii + i);

                        if (form.GetPosition(ii).IsAllowed(rc.GetPattern(0).GetChar))
                        {
                            finalPlate.AddChar(rc.GetPattern(0).GetChar);
                        }
                        else
                        {
                            finalPlate.IncrementRequiredChanges();
                            for (Int32 x = 0; x < rc.GetPatterns().Count; x++)
                            {
                                if (form.GetPosition(ii).IsAllowed(rc.GetPattern(x).GetChar))
                                {
                                    CharacterRecognizer.RecognizedChar.RecognizedPattern rp = rc.GetPattern(x);
                                    finalPlate.AddRequiredChanges(rp.GetCost / 100.0);
                                    finalPlate.AddChar(rp.GetChar);
                                    break;
                                }
                            }
                        }
                    }

                    finalPlates.Add(finalPlate);
                }
            }

            if (finalPlates.Count == 0)
                return recognizedPlate.GetString();

            Double minimalChanges = Double.PositiveInfinity;
            Int32 minimalIndex = 0;

            for (Int32 i = 0; i < finalPlates.Count; i++)
            {

                if (finalPlates[i].RequiredChanges <= minimalChanges)
                {
                    minimalChanges = finalPlates[i].RequiredChanges;
                    minimalIndex = i;
                }
            }

            String toReturn = recognizedPlate.GetString();
            if (finalPlates[minimalIndex].RequiredChanges <= 2)
                toReturn = finalPlates[minimalIndex].GetPlate();

            return toReturn;

        }

        public void FlagEqualOrShorterLength(Int32 length)
        {
            Boolean found = false;
            for (Int32 i = length; i >= 1 && !found; i--)
            {
                foreach (PlateForm form in plateForms)
                {
                    if (form.Length == i)
                    {
                        form.flagged = true;
                        found = true;
                    }
                }
            }
        }

    }
}
