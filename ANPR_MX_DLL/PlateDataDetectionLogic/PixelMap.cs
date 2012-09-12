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
    public class PixelMap
    {

        private Boolean[,] matrix;
        private Int32 width;
        private Int32 height;
        private Piece bestPiece = null;

        public PixelMap(IplImage image)
        {
            MatrixInit(image);
        }

        public Piece GetBestPiece()
        {
            ReduceOtherPieces();
            if (bestPiece == null)
                return new Piece();
            return bestPiece;
        }

       
        private PixelMap ReduceOtherPieces()
        {
            if (bestPiece != null)
                return this;

            PieceSet pieces = FindPieces();
            Int32 maxCost = 0;
            Int32 maxIndex = 0;

            for (Int32 i = 0; i < pieces.Count; i++)
            {
                if (pieces[i].Cost() > maxCost)
                {
                    maxCost = pieces[i].Cost();
                    maxIndex = i;
                }
            }


            for (Int32 i = 0; i < pieces.Count; i++)
            {
                if (i != maxIndex)
                    pieces[i].BleachPiece(ref matrix);
            }
            if (pieces.Count != 0)
                bestPiece = pieces[maxIndex];
            return this;
        }

        private PieceSet FindPieces()
        {
            
            PieceSet pieces = new PieceSet();

            
            PointSet unsorted = new PointSet();
            for (Int32 x = 0; x < width; x++)
                for (Int32 y = 0; y < height; y++)
                    if (matrix[x,y]) 
                        unsorted.Add(new Point(x, y));
            
            while (unsorted.Count>0)
            {
            
                pieces.Add(CreatePiece(unsorted));
            }
            
            return pieces;
        }

        private Piece CreatePiece(PointSet unsorted)
        {

            Piece piece = new Piece();

            PointSet stack = new PointSet();
            stack.Insert(0,unsorted.Last());

            while (stack.Count > 0)
            {
                Point p = stack.First();
                stack.RemoveAt(0);
                if (SeedShouldBeAdded(piece, p))
                {
                    piece.Add(p);
                    unsorted.RemovePoint(p);
                    stack.Insert(0,new Point(p.X + 1, p.Y));
                    stack.Insert(0,new Point(p.X - 1, p.Y));
                    stack.Insert(0,new Point(p.X, p.Y + 1));
                    stack.Insert(0,new Point(p.X, p.Y - 1));
                }
            }
            piece.CreateStatistics();
            return piece;
        }

       

        private Boolean SeedShouldBeAdded(Piece piece, Point p)
        {

            if (p.X < 0 || p.Y < 0 || p.X >= this.width || p.Y >= this.height) return false;

            if (!this.matrix[p.X, p.Y]) return false;

            foreach (Point piecePoint in piece)
                if (piecePoint.EqualsPoint(p))
                    return false;
            return true;
        }

        private void MatrixInit(IplImage image)
        {
            IplImage imageCopy = null;
            width = image.Width;
            height = image.Height;

            try
            {
                matrix = new Boolean[width, height];

                imageCopy = new IplImage(image.Size, image.Depth, image.NChannels);
                Cv.CvtColor(image, imageCopy, ColorConversion.RgbToHsv);

                for (Int32 x = 0; x < width; x++)
                {
                    for (Int32 y = 0; y < height; y++)
                    {
                        matrix[x, y] = (GetBrightness(imageCopy, x, y) < 0.5);
                    }
                }
            }
            finally
            {
                if (null != imageCopy)
                    Cv.ReleaseImage(imageCopy);
            }
        }

        private Double GetBrightness(IplImage image, Int32 x, Int32 y)
        {
            Double value = 0.0;

            CvScalar cvValues = image[y, x];
            value = (Double)(cvValues.Val2 / 255.0);

            return value;
        }


        private class PieceSet : List<Piece>
        {
            const Int64 serialVersionUID = 0;
        }


        public class PointSet : List<Point>
        {
            const Int64 serialVersionUID = 0;
            public void RemovePoint(Point p)
            {
                Point toRemove = null;
                foreach (Point px in this)
                {
                    if (px.EqualsPoint(p))
                        toRemove = px;
                }
                this.Remove(toRemove);
            }

        }


        public IplImage Render(Piece piece)
        { 
          return (piece.Render(matrix));
        }

        public class Piece : PointSet
        {
            const Int64 serialVersionUID = 0;
            public Int32 mostLeftPoint;
            public Int32 mostRightPoint;
            public Int32 mostTopPoint;
            public Int32 mostBottomPoint;
            public Int32 width;
            public Int32 height;
            public Int32 centerX;
            public Int32 centerY;
            public Double magnitude;
            public Int32 numberOfBlackPoints;
            public Int32 numberOfAllPoints;

            public IplImage Render(Boolean [,] matrix)
            {
                if (numberOfAllPoints == 0) return null;
                IplImage image = new IplImage(width, height, BitDepth.U8, 3 );
                for (int x = this.mostLeftPoint; x <= this.mostRightPoint; x++)
                {
                    for (int y = this.mostTopPoint; y <= this.mostBottomPoint; y++)
                    {
                        if (matrix[x,y])
                        {
                            image[(y - this.mostTopPoint), (x - this.mostLeftPoint)] = CvColor.Black;
                        }
                        else
                        {
                            image[(y - this.mostTopPoint), (x - this.mostLeftPoint)] = CvColor.White;
                        }
                    }
                }
                return image;
            }

            public void CreateStatistics()
            {
                mostLeftPoint = MostLeftPoint();
                mostRightPoint = MostRightPoint();
                mostTopPoint = MostTopPoint();
                mostBottomPoint = MostBottomPoint();
                width = mostRightPoint - mostLeftPoint + 1;
                height = mostBottomPoint - mostTopPoint + 1;
                centerX = (mostLeftPoint + mostRightPoint) / 2;
                centerY = (mostTopPoint + mostBottomPoint) / 2;
                numberOfBlackPoints = NumberOfBlackPoints();
                numberOfAllPoints = NumberOfAllPoints();
                magnitude = Magnitude();
            }

            public Int32 Cost()
            { 
                return (numberOfAllPoints - NumberOfBlackPoints());
            }

            public void BleachPiece(ref Boolean[,] matrix)
            {
                foreach (Point p in this)
                {
                    matrix[p.X, p.Y] = false;
                }
            }

            private Double Magnitude()
            {
                return (Double)(numberOfBlackPoints / numberOfAllPoints);
            }

            private Int32 NumberOfBlackPoints()
            {
                return Count;
            }
            private Int32 NumberOfAllPoints()
            {
                return (width * height);
            }

            private Int32 MostLeftPoint()
            {
                Int32 position = Int32.MaxValue;
                foreach (Point p in this)
                    position = Math.Min(position, p.X);
                return position;
            }

            private Int32 MostRightPoint()
            {
                Int32 position = 0;
                foreach (Point p in this)
                    position = Math.Max(position, p.X);
                return position;
            }

            private Int32 MostTopPoint()
            {
                Int32 position = Int32.MaxValue;
                foreach (Point p in this)
                    position = Math.Min(position, p.Y);
                return position;
            }

            private Int32 MostBottomPoint()
            {
                Int32 position = 0;
                foreach (Point p in this)
                    position = Math.Max(position, p.Y);
                return position;
            }

        }

        public class Point
        {
            private Int32 x;
            private Int32 y;

            public Int32 X {get{return x;}}
            public Int32 Y { get { return y; } }

            public Point(Int32 pX, Int32 pY)
            {
                x = pX;
                y = pY;
            }

            public Boolean EqualsPoint(Point p2)
            {
                if (p2.x == x && p2.y == y)
                    return true;
                return false;
            }

            public Boolean EqualsPoint(Int32 pX, Int32 pY)
            {
                if (pX == x && pY == y)
                    return true;
                return false;
            }
                        
        }

    }
}
