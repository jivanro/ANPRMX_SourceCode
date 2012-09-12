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


using OpenCvSharp;
using OpenCvSharp.Blob;
using OpenCvSharp.CPlusPlus;

namespace ANPR_MX.PlateAreaDetectionLogic
{
    public class PlateAreaFinder
    {

        #region PUBLIC_FUNCTIONS

        public IplImage ImageTreatment(IplImage img, out IplImage[] plateImages)
        {
            IplImage tgray = null;
            IplImage gray = null;
            IplImage mainSubImage = null;
            IplImage tmpImage = null;
            IplImage tmpImage2 = null;
            CvBlobs blobs1 = null;
            CvBlobs blobs2 = null;
            List<IplImage> plates = null;
            CvRect subImageRect;

            plateImages = null;

            try
            {
                plates = new List<IplImage>();
                mainSubImage = ExtractSubImage(img, out subImageRect);
                tgray = new IplImage(mainSubImage.Size, BitDepth.U8, 1);
                mainSubImage.CvtColor(tgray, ColorConversion.RgbaToGray);
                blobs1 = PreProcessImage1(mainSubImage, tgray);
                blobs2 = PreProcessImage2(mainSubImage, tgray);


                tmpImage = img.Clone();
                tmpImage2 = mainSubImage.Clone();
                tmpImage.SetROI(subImageRect);
                if (null != blobs1 && blobs1.Count > 0)
                {
                    IplImage[] plateImage = GetPlates(tmpImage, tmpImage2, blobs1, 2.4);
                    if (null != plateImage)
                        plates.AddRange(plateImage);
                }

                if (null != blobs2 && blobs2.Count > 0)
                {
                    IplImage[] plateImage = GetPlates(tmpImage, tmpImage2, blobs2, 3.5);
                    if (null != plateImage)
                        plates.AddRange(plateImage);
                }
                tmpImage.ResetROI();
               
                Cv.ReleaseImage(gray);
                gray = tmpImage;
            }
            finally
            {
                if (null != tmpImage2)
                    Cv.ReleaseImage(tmpImage2);

                if (null != tgray)
                    Cv.ReleaseImage(tgray);

                if (null != mainSubImage)
                    Cv.ReleaseImage(mainSubImage);
            }

            if (plates.Count > 0)
                plateImages = plates.ToArray();

            return gray;
        }

        #endregion

        #region PRIVATE_FUNCTIONS

        private IplImage ExtractSubImage(IplImage imgOrig, out CvRect pSubImageRect)
        {
            IplImage mainSubImage = null;

            CvSize subImageSize = new CvSize((Int32)((imgOrig.Size.Width / 9) * 7), (Int32)((imgOrig.Size.Height / 9) * 4));
            CvPoint point = new CvPoint(0, (Int32)((imgOrig.Size.Height / 9) * 3));
            CvRect subImageRect = new CvRect(point, subImageSize);

            pSubImageRect = subImageRect;
            mainSubImage = GetSubImage(imgOrig, subImageRect);

            return mainSubImage;

        }

        private CvBlobs PreProcessImage1(IplImage mainSubImage, IplImage imgGray)
        {
            CvBlobs blobs = null;
            IplImage tmpImage = null;
            IplImage gray = null;
            IplImage tgray = null;
            IplImage labelImg = null;
            IplImage temp = null;

            try
            {
                tgray = imgGray.Clone();
                gray = new IplImage(tgray.Size, tgray.Depth, 1);
                Cv.Smooth(tgray, tgray, SmoothType.Gaussian);
                Cv.Canny(tgray, gray, 500, 2, ApertureSize.Size5);
                temp = gray.Clone();
                //IplConvKernel element = Cv.CreateStructuringElementEx(5, 1, 3, 0, ElementShape.Rect, null);
                IplConvKernel element = Cv.CreateStructuringElementEx(7, 1, 3, 0, ElementShape.Rect, null);
                Cv.MorphologyEx(gray, gray, temp, element, MorphologyOperation.BlackHat, 1);
                Cv.Threshold(gray, gray, 100, 255, ThresholdType.Binary | ThresholdType.Otsu);
                Cv.Smooth(gray, gray, SmoothType.Gaussian);


                labelImg = new IplImage(mainSubImage.Size, CvBlobLib.DepthLabel, 1);
                blobs = new CvBlobs();
                CvBlobLib.Label(gray, labelImg, blobs);
                CvBlobLib.FilterByArea(blobs, 1550, 4850);

                tmpImage = mainSubImage.Clone();
                //CvTracks tracks = new CvTracks();
                //CvBlobLib.UpdateTracks(blobs, tracks, 200.0, 5);
                //CvBlobLib.RenderTracks(tracks, tmpImage, tmpImage, RenderTracksMode.ID);
                blobs.RenderBlobs(labelImg, mainSubImage, tmpImage, RenderBlobsMode.BoundingBox | RenderBlobsMode.Angle);
                /*
                img.SetROI(subImageRect);
                Cv.Copy(tmpImage, img);
                img.ResetROI();
                Cv.ReleaseImage(tmpImage);                
                
                */
            }
            finally
            {
                if (null != temp)
                    Cv.ReleaseImage(temp);

                if (null != tgray)
                    Cv.ReleaseImage(tgray);

                if (null != gray)
                    Cv.ReleaseImage(gray);

                if (null != labelImg)
                    Cv.ReleaseImage(labelImg);

                if (null != tmpImage)
                    Cv.ReleaseImage(tmpImage);
            }

            return blobs;
        }


        private CvBlobs PreProcessImage2(IplImage img, IplImage imgGray)
        {
            CvBlobs blobs = null;
            IplConvKernel element = null;
            IplImage temp = null;
            IplImage dest = null;
            IplImage tmpImage = null;
            IplImage tmpImage2 = null;
            IplImage labelImg = null;

            try
            {
                //element = Cv.CreateStructuringElementEx(76, 65, 35, 22, ElementShape.Rect, null);
                element = Cv.CreateStructuringElementEx(111,86,49,22, ElementShape.Rect, null);
                tmpImage = imgGray.Clone();//new IplImage(img.Size, BitDepth.U8, 1);
                temp = tmpImage.Clone();
                dest = tmpImage.Clone();
                /*img.CvtColor(tmpImage, ColorConversion.RgbaToGray);
                tmpImage.Rectangle(new CvPoint(0, 0), new CvPoint((Int32)(tmpImage.Size.Width), (Int32)((tmpImage.Size.Height / 9) * 3)), new CvScalar(255, 255, 255), -1);
                tmpImage.Rectangle(new CvPoint(0, (Int32)((tmpImage.Size.Height / 5) * 4)), new CvPoint((Int32)(tmpImage.Size.Width), (Int32)(tmpImage.Size.Height)), new CvScalar(255, 255, 255), -1);
                tmpImage.Rectangle(new CvPoint((Int32)((tmpImage.Size.Width / 9) * 7), 0), new CvPoint((Int32)((tmpImage.Size.Width)), (Int32)(tmpImage.Size.Height)), new CvScalar(255, 255, 255), -1);
                 * */
                Cv.Smooth(tmpImage, tmpImage, SmoothType.Gaussian);
                Cv.MorphologyEx(tmpImage, dest, temp, element, MorphologyOperation.TopHat, 1);
                Cv.Threshold(dest, tmpImage, 128, 255, ThresholdType.Binary | ThresholdType.Otsu);
                Cv.Smooth(tmpImage, dest, SmoothType.Median);


                labelImg = new IplImage(img.Size, CvBlobLib.DepthLabel, 1);
                blobs = new CvBlobs();
                tmpImage2 = tmpImage.Clone();
                CvBlobLib.Label(tmpImage2, labelImg, blobs);

                /*
                Cv.ReleaseImage(tmpImage);
                tmpImage = img.Clone();
                blobs.RenderBlobs(labelImg, img, tmpImage, RenderBlobsMode.BoundingBox | RenderBlobsMode.Angle);
                tmpImage.SaveImage(@"c:\temp\newImages\RenderBlobsNOFiltered.png");
                 */
                


                CvBlobLib.FilterByArea(blobs, 850, 8850);
                Cv.ReleaseImage(tmpImage);
                tmpImage = img.Clone();
                  //CvTracks tracks = new CvTracks();
                  //CvBlobLib.UpdateTracks(blobs, tracks, 200.0, 5);
                  //CvBlobLib.RenderTracks(tracks, tmpImage, tmpImage, RenderTracksMode.ID);
                blobs.RenderBlobs(labelImg, img, tmpImage, RenderBlobsMode.BoundingBox | RenderBlobsMode.Angle);
                  //tmpImage.SaveImage(@"c:\temp\newImages\RenderBlobsFiltered.png");
            }
            finally
            {

                if (null != temp)
                    Cv.ReleaseImage(temp);

                if (null != dest)
                    Cv.ReleaseImage(dest);

                if (null != tmpImage)
                    Cv.ReleaseImage(tmpImage);

                if (null != tmpImage2)
                    Cv.ReleaseImage(tmpImage2);

                if (null != labelImg)
                    Cv.ReleaseImage(labelImg);
            }


            return blobs;
        }

        private CvBlobs PreProcessImage2_old(IplImage img)
        {
            CvBlobs blobs = null;
            IplConvKernel element = null;
            IplImage temp = null;
            IplImage dest = null;
            IplImage tmpImage = null;
            IplImage tmpImage2 = null;
            IplImage labelImg = null;

            try
            {
                element = Cv.CreateStructuringElementEx(180, 5, 90, 1, ElementShape.Rect, null);
                tmpImage = new IplImage(img.Size, BitDepth.U8, 1); 
                temp = tmpImage.Clone();
                dest = tmpImage.Clone();
                img.CvtColor(tmpImage, ColorConversion.RgbaToGray);
                tmpImage.Rectangle(new CvPoint(0, 0), new CvPoint((Int32)(tmpImage.Size.Width), (Int32)((tmpImage.Size.Height / 9) * 3)), new CvScalar(255, 255, 255), -1);
                tmpImage.Rectangle(new CvPoint(0, (Int32)((tmpImage.Size.Height / 5) * 4)), new CvPoint((Int32)(tmpImage.Size.Width), (Int32)(tmpImage.Size.Height)), new CvScalar(255, 255, 255), -1);
                tmpImage.Rectangle(new CvPoint((Int32)((tmpImage.Size.Width / 9) * 7), 0), new CvPoint((Int32)((tmpImage.Size.Width)), (Int32)(tmpImage.Size.Height)), new CvScalar(255, 255, 255), -1);
                Cv.Smooth(tmpImage, tmpImage, SmoothType.Gaussian);
                Cv.MorphologyEx(tmpImage, dest, temp, element, MorphologyOperation.TopHat, 1);
                Cv.Threshold(dest, tmpImage, 128, 255, ThresholdType.Binary | ThresholdType.Otsu);
                Cv.Smooth(tmpImage, dest, SmoothType.Median);
                

                labelImg = new IplImage(img.Size, CvBlobLib.DepthLabel, 1);
                blobs = new CvBlobs();
                tmpImage2 = tmpImage.Clone();                
                CvBlobLib.Label(tmpImage2, labelImg, blobs);
                
                //Cv.ReleaseImage(tmpImage);
                //tmpImage = img.Clone();
                //blobs.RenderBlobs(labelImg, img, tmpImage);
                //tmpImage.SaveImage(@"c:\temp\newImages\RenderBlobsNOFiltered.png");
                
                
                CvBlobLib.FilterByArea(blobs, 850, 4850);
                Cv.ReleaseImage(tmpImage);
                tmpImage = img.Clone();
                //CvTracks tracks = new CvTracks();
                //CvBlobLib.UpdateTracks(blobs, tracks, 200.0, 5);
                //CvBlobLib.RenderTracks(tracks, tmpImage, tmpImage, RenderTracksMode.ID);
                blobs.RenderBlobs(labelImg, img, tmpImage, RenderBlobsMode.BoundingBox | RenderBlobsMode.Angle);
                //tmpImage.SaveImage(@"c:\temp\newImages\RenderBlobsFiltered.png");
            }
            finally
            {

                if (null != temp)
                    Cv.ReleaseImage(temp);

                if (null != dest)
                    Cv.ReleaseImage(dest);

                if (null != tmpImage)
                    Cv.ReleaseImage(tmpImage);

                if (null != tmpImage2)
                    Cv.ReleaseImage(tmpImage2);

                if (null != labelImg)
                    Cv.ReleaseImage(labelImg);
            }


            return blobs;
        }

        private IplImage[] GetPlates(IplImage tmpImage, IplImage tmpImage2, CvBlobs blobs, Double maxRatio)
        {
            IplImage[] plates = null;
            IplImage plakatemp = null;

            
            if (null != blobs)
            {

                List<IplImage> tempImages = new List<IplImage>();

                foreach (var item in blobs)
                {
                    item.Value.SetImageROItoBlob(tmpImage2);

                    Double ratio = (Double)item.Value.Rect.Width / item.Value.Rect.Height;
                    Double angle = (Double)item.Value.CalcAngle();
                    if (ratio > 1.2 && ratio < maxRatio && angle > -15 && angle < 15)
                    {
                        if (item.Value.Rect.Width < 90 || item.Value.Rect.Height < 50)
                        {
                            plakatemp = new IplImage(new CvSize(System.Convert.ToInt32(item.Value.Rect.Width * 1.3), System.Convert.ToInt32(item.Value.Rect.Height * 1.3)), tmpImage2.Depth, tmpImage2.NChannels);
                            Cv.Resize(tmpImage2, plakatemp, Interpolation.Area);
                        }
                        else
                        {
                            plakatemp = new IplImage(new CvSize(item.Value.Rect.Width, item.Value.Rect.Height), tmpImage2.Depth, tmpImage2.NChannels);
                            Cv.ConvertScale(tmpImage2, plakatemp);
                        }
                        tempImages.Add(plakatemp);
                        tmpImage.Rectangle(item.Value.Rect, new CvScalar(0, 0, 255), 2, LineType.Link4);
                    }
                }

                if (tempImages.Count > 0)
                    plates = tempImages.ToArray();
            }

            return plates;
        }

        

        private IplImage GetSubImage(IplImage image, CvRect rect)
        {
            IplImage imgDst = new IplImage(rect.Size, image.Depth, image.NChannels);

            image.SetROI(rect);
            Cv.Copy(image, imgDst);
            image.ResetROI();

            return imgDst;
        }

        #endregion
    }

    
}
