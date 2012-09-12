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
using System.Drawing;


using OpenCvSharp;
using OpenCvSharp.Blob;
using OpenCvSharp.CPlusPlus;

using ANPR_MX.PlateAreaDetectionLogic;
using ANPR_MX.PlateDataDetectionLogic;
using ANPR_MX.Globals;

namespace ANPR_MX.MainControl
{
    public class ImageProcessing
    {

        private PlateAreaFinder plateAreaFinder = null;
        private MainLogic mainLogic = null;

        #region PUBLIC_FUNCTIONS

        public ImageProcessing()
        {
            plateAreaFinder = new PlateAreaFinder();
            mainLogic = new MainLogic();
        }

        public Bitmap ProcessImage(String fullPath, out String[] platesList)
        {
            Bitmap result = null;

            IplImage img = null;
            IplImage tempImg = null;
            IplImage[] plates = null;
            platesList = null;

            try
            {
                img = new IplImage(fullPath);

                if (img.Width > Constants.CAPTURE_WIDTH_PX || img.Height > Constants.CAPTURE_HEIGHT_PX)
                {
                    IplImage tempImage2 = new IplImage(new CvSize(Constants.CAPTURE_WIDTH_PX, Constants.CAPTURE_HEIGHT_PX), img.Depth, img.NChannels);
                    img.Resize(tempImage2, Interpolation.Area);
                    Cv.ReleaseImage(img);
                    img = tempImage2;
                }

                tempImg = plateAreaFinder.ImageTreatment(img,out plates);
                platesList = ExtractPlateData(plates);
                result = tempImg.ToBitmap();
            }
            finally
            {
                if (null != tempImg)
                {
                    Cv.ReleaseImage(tempImg);
                    tempImg = null;
                }

                if (null != img)
                    Cv.ReleaseImage(img);

                if (null != plates)
                {
                    foreach (IplImage plate in plates)
                    {
                        Cv.ReleaseImage(plate);
                    }
                }
            }


            return result;

        }

        public Bitmap ProcessImage(IplImage sourceImg)
        {
            Bitmap result = null;

            IplImage img = null;
            IplImage tempImg = null;
            IplImage[] plates = null;

            try
            {
                img = sourceImg.Clone();
                tempImg = plateAreaFinder.ImageTreatment(img, out plates);
                result = tempImg.ToBitmap();
            }
            finally
            {
                if (null != tempImg)
                {
                    Cv.ReleaseImage(tempImg);
                    tempImg = null;
                }

                if (null != img)
                    Cv.ReleaseImage(img);

                if (null != plates)
                {
                    foreach (IplImage plate in plates)
                    {
                        Cv.ReleaseImage(plate);
                    }
                }
            }


            return result;

        }


        public Bitmap GetPreview(String fullPath, Int32 width, Int32 height)
        {
            Bitmap result = null;
            IplImage img = null;
            IplImage tempImg = null;

            try
            {
                img = new IplImage(fullPath);
                tempImg = new IplImage(new CvSize(width, height), img.Depth, img.NChannels);
                Cv.Resize(img, tempImg, Interpolation.Area);
                result = tempImg.ToBitmap();
            }
            finally
            {

                if (null != tempImg)
                    Cv.ReleaseImage(tempImg);

                if (null != img)
                    Cv.ReleaseImage(img);

            }

            return result;
        }

        public Bitmap GetFullImage(String fullPath)
        {
            Bitmap result = null;
            IplImage img = null;
            IplImage tempImage = null;
            

            try
            {
                img = new IplImage(fullPath);
                if (img.Width > Constants.CAPTURE_WIDTH_PX || img.Height > Constants.CAPTURE_HEIGHT_PX)
                {
                    tempImage = new IplImage(new CvSize(Constants.CAPTURE_WIDTH_PX, Constants.CAPTURE_HEIGHT_PX), img.Depth, img.NChannels);
                    img.Resize(tempImage, Interpolation.Area);
                    Cv.ReleaseImage(img);
                    img = tempImage;
                }
                result = img.ToBitmap();
            }
            finally
            {

                if (null != img)
                    Cv.ReleaseImage(img);

            }

            return result;
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private String[] ExtractPlateData(IplImage[] plates)
        {
            String[] result = null;
            String value = null;
            List<String> tmpPlaca = null;
            List<String> platesList = null;
            IplImage deskewPlakatemp = null;
            IplImage sharpenedPlate = null;
            IplImage deskewSharpenedPlakatemp = null;

            if (null != plates)
            {
                platesList = new List<String>();
                try
                {
                    //Int32 cont = 0;
                    foreach (var plate in plates)
                    {


                        value = ProcessThresholdForPlate(SharpenImage(plate), 94, 255, Constants.PEAK_AVERAGE_METHOD_MODAL);
                        if (null == value || value.Length < 1)
                            value = mainLogic.ImageAnalysis(SharpenImage(plate), Constants.PEAK_AVERAGE_METHOD_MODAL);
                        if (null != value && value.Length > 0)
                        {
                            platesList.Add(value);
                            sharpenedPlate = SharpenImage(plate);
                            tmpPlaca = ProcessMultipleThresholdForPlate(sharpenedPlate, Constants.EXCLUDE_METHOD);
                            if (null != tmpPlaca && tmpPlaca.Count > 0)
                                platesList.AddRange(tmpPlaca);
                            deskewPlakatemp = RotateImage(plate, SkewAngle(plate));
                            if (null != deskewPlakatemp)
                            {

                                deskewSharpenedPlakatemp = SharpenImage(deskewPlakatemp);
                                if (null != deskewSharpenedPlakatemp)
                                {
                                    tmpPlaca = ProcessMultipleThresholdForPlate(deskewSharpenedPlakatemp, Constants.ALL_METHODS);
                                    if (null != tmpPlaca && tmpPlaca.Count > 0)
                                    {
                                        platesList.AddRange(tmpPlaca);
                                    }

                                    // JIVAN
                                    //deskewSharpenedPlakatemp.SaveImage(String.Format(@"c:\temp\newImages\deskewSharpenedPlakatempTmp{0}.png", counter));
                                }


                                // JIVAN
                                //deskewPlakatemp.SaveImage(String.Format(@"c:\temp\newImages\deskewPlakatempTmp{0}.png", counter));
                            }
                            if (null != deskewSharpenedPlakatemp)
                            {
                                Cv.ReleaseImage(deskewSharpenedPlakatemp);
                                deskewSharpenedPlakatemp = null;
                            }
                            if (null != deskewPlakatemp)
                            {
                                Cv.ReleaseImage(deskewPlakatemp);
                                deskewPlakatemp = null;
                            }
                            if (null != sharpenedPlate)
                            {
                                Cv.ReleaseImage(sharpenedPlate);
                                sharpenedPlate = null;
                            }

                        }
                    }
                }
                finally
                {
                    if (null != deskewSharpenedPlakatemp)
                    {
                        Cv.ReleaseImage(deskewSharpenedPlakatemp);
                        deskewSharpenedPlakatemp = null;
                    }
                    if (null != deskewPlakatemp)
                    {
                        Cv.ReleaseImage(deskewPlakatemp);
                        deskewPlakatemp = null;
                    }
                    if (null != sharpenedPlate)
                    {
                        Cv.ReleaseImage(sharpenedPlate);
                        sharpenedPlate = null;
                    }
                }
                //plate.SaveImage(String.Format(@"c:\temp\newImages\plaka{0}.jpg",cont++));
            }


            if (null != platesList && platesList.Count > 0)
            {
                result = platesList.ToArray();
            }

            return result;
        }

        private String ProcessThresholdForPlate(IplImage image, Int32 threshold1, Int32 threshold2, Int32 method)
        {
            String result = null;
            IplImage tmpImage = null;
            IplImage dest = null;
            IplImage destFinal = null;
            try
            {
                tmpImage = new IplImage(image.Size, BitDepth.U8, 1);
                dest = new IplImage(image.Size, BitDepth.U8, 1);
                destFinal = new IplImage(image.Size, BitDepth.U8, 3);
                image.CvtColor(tmpImage, ColorConversion.RgbaToGray);
                Cv.Threshold(tmpImage, dest, threshold1, threshold2, ThresholdType.Binary);
                dest.CvtColor(destFinal, ColorConversion.GrayToRgb);

                result = mainLogic.ImageAnalysis(destFinal, method);
            }
            finally
            {
                if(null!=dest)
                  Cv.ReleaseImage(dest);
                if(null!=tmpImage)
                  Cv.ReleaseImage(tmpImage);
                if(null!=destFinal)
                  Cv.ReleaseImage(destFinal);
            }

            return result;
        }

        private List<String> ProcessMultipleThresholdForPlate(IplImage image, Int32 method)
        {
            List<String> result = new List<String>();
            String resultado = null;

            
            resultado = mainLogic.ImageAnalysis(image, Constants.PEAK_AVERAGE_METHOD_MODAL);
            if (null != resultado && resultado.Length > 2)
                result.Add(resultado);
            
            

            if (Constants.ALL_METHODS == method)
            {
                resultado = ProcessThresholdForPlate(image, 94, 255, Constants.PEAK_AVERAGE_METHOD_MODAL);
                if (null != resultado && resultado.Length > 2)
                    result.Add(resultado);
            }

            
            resultado = ProcessThresholdForPlate(image, 111, 255, Constants.PEAK_AVERAGE_METHOD_MODAL);
            if (null != resultado && resultado.Length > 2)
                result.Add(resultado);
            

            /*
            resultado = ProcessThresholdForPlate(image, 135, 255, Constants.PEAK_AVERAGE_METHOD_MODAL);
            if (null != resultado && resultado.Length > 2)
                result.Add(resultado);
             */ 
            

            resultado = ProcessThresholdForPlate(image, 75, 255, Constants.PEAK_AVERAGE_METHOD_MODAL);
            if (null != resultado && resultado.Length > 2)
                result.Add(resultado);

            return result;
        }

        private IplImage RotateImage(IplImage img, Double angle)
        {



            Mat imgMat = null;
            IplImage tempImage = null;
            IplImage rotatedImg = null;
            CvSeq<CvPoint> contours = null;
            CvMemStorage storage = null;

            if (angle == 0.0 || Double.IsNaN(angle))
            {
                angle = -3.0;
            }

            if (angle != 0.0 && !Double.IsNaN(angle))
            {
                try
                {
                    rotatedImg = new IplImage(img.Size, img.Depth, img.NChannels);
                    tempImage = new IplImage(img.Size, img.Depth, 1);
                    Cv.CvtColor(img, tempImage, ColorConversion.RgbaToGray);
                    imgMat = new Mat(tempImage);
                    storage = new CvMemStorage();
                    Cv.FindContours(tempImage, storage, out contours, CvContour.SizeOf, ContourRetrieval.List, ContourChain.ApproxNone);
                    contours = Cv.ApproxPoly(contours, CvContour.SizeOf, storage, ApproxPolyMethod.DP, 3, true);
                    CvBox2D box = Cv.MinAreaRect2(contours);
                    CvMat rotMat = Cv.GetRotationMatrix2D(box.Center, angle, 1.0);
                    Cv.WarpAffine(img, rotatedImg, rotMat, Interpolation.Cubic);

                }
                catch (Exception ex)
                {
                    if (null != rotatedImg)
                    {
                        Cv.ReleaseImage(rotatedImg);
                        rotatedImg = null;
                    }
                    throw ex;
                }
                finally
                {
                    if (null != tempImage)
                        Cv.ReleaseImage(tempImage);
                }
            }

            return rotatedImg;
        }

        private Double SkewAngle(IplImage img)
        {
            Double result = 0.0;
            IplImage tempImage = null;

            try
            {

                tempImage = new IplImage(img.Size, img.Depth, 1);
                Cv.CvtColor(img, tempImage, ColorConversion.RgbaToGray);
                //Cv.Not(tempImage, tempImage);

                Mat imgMat = new Mat(tempImage);
                CvCpp.Canny(imgMat, imgMat, 50, 200, ApertureSize.Size3, false);
                CvLineSegmentPoint[] segProb = CvCpp.HoughLinesP(imgMat, 1, Math.PI / 180.0, 50, ((Double)img.Size.Width) / 2.0, 10.0);


                foreach (CvLineSegmentPoint point in segProb)
                {
                    result += Math.Atan2((Double)(point.P2.Y) - (Double)(point.P1.Y), (Double)(point.P2.X) - (Double)(point.P1.X));
                    //img.Line(point.P1,point.P2,new CvScalar(255,0,0));
                }

                if (segProb.Length > 0)
                {
                    result /= (Double)(segProb.Length);
                    result = result * 180 / Math.PI;
                }
            }
            finally
            {
                if (null != tempImage)
                    Cv.ReleaseImage(tempImage);
            }
            //img.SaveImage(@"C:\temp\newImages\res.jpg");
            return result;
        }

        private IplImage SharpenImage(IplImage img)
        {
            IplImage resImage = null;

            Double[] matrix = { 0.0,-1.0,0.0,
                                -1.0,5.0,-1.0,
                                0.0,-1.0,0.0 };



            resImage = new IplImage(new CvSize(img.Width, img.Height), img.Depth, img.NChannels);


            try
            {
                CvMat kernel = new CvMat(3, 3, MatrixType.F64C1, matrix);
                Cv.Filter2D(img, resImage, kernel);

            }
            catch (Exception)
            {
                if (null != resImage)
                {
                    Cv.ReleaseImage(resImage);
                    resImage = null;
                }
            }



            return resImage;
        }

        #endregion
    }
}
