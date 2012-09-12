// ANPR_MX_Sample is a simple Automatic Plate Recognition System
// for North American type plates.
// Developed by Jivan Miranda Rodriguez
// For suggestions & feedback contact me at
// jivanro@hotmail.com

// IMPORTANT: READ BEFORE DOWNLOADING, COPYING, INSTALLING OR USING. 

// By downloading, copying, installing or using the software you agree to this and
// the third party library licenses. (See the "NOTES" section for details.)
// If you do not agree to any of the licenses, do not download, install,
// copy or use the software.

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
// ANPR_MX_Sample is an intellectual property of Jivan Miranda Rodriguez
// ----  label end  ----


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;



namespace ANPR_MX_Sample
{
    public partial class Form1 : Form
    {
        private ANPR_MX.MainControl.ImageProcessing tratImg = null;

        public Form1()
        {
            InitializeComponent();
            tratImg = new ANPR_MX.MainControl.ImageProcessing();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            ProcessSelection();
        
            


        }

        private void btnCargar_Click(object sender, EventArgs e)
        {
            String selectedPath = null;

            
            FolderBrowserDialog openFileDialog = new FolderBrowserDialog();
            openFileDialog.ShowNewFolderButton = false;
            String path = Directory.GetCurrentDirectory() + System.IO.Path.DirectorySeparatorChar + "carpictures";
            if (Directory.Exists(path))
                openFileDialog.SelectedPath = path;
            if (DialogResult.OK == openFileDialog.ShowDialog())
            {
                lstImages.Items.Clear();
                selectedPath = openFileDialog.SelectedPath;
                DirectoryInfo di = new DirectoryInfo(selectedPath);
                FileInfo[] rgFiles = di.GetFiles("*.jpg");
                FileInfo[] rgFiles2 = di.GetFiles("*.png");
                foreach (FileInfo fi in rgFiles.Concat(rgFiles2))
                {
                    lstImages.Items.Add(fi);
                    
                }
            }

        }

        private void lstImages_SelectedIndexChanged(object sender, EventArgs e)
        {
            FileInfo fi = (FileInfo)lstImages.SelectedItem;
            Bitmap preview = tratImg.GetPreview(fi.FullName, imgPreview.Width, imgPreview.Height);
            imgPreview.Image = preview;
            imgPreview.Refresh();
        }

        private void lstImages_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ProcessSelection();
        }

        private void ProcessSelection()
        {
            String[] plates;

            this.btnProcesar.Text = "Processing";
            this.btnProcesar.Enabled = false;

            FileInfo fi = (FileInfo)lstImages.SelectedItem;
            if (null != fi)
            {
                lstPlacas.Items.Clear();
                lstPlacas.Refresh();
                Bitmap fullImage = tratImg.GetFullImage(fi.FullName);
                imgCoche.Image = fullImage;
                imgCoche.Refresh();
                Bitmap preview = tratImg.ProcessImage(fi.FullName, out plates);
                imgCoche.Image = preview;
                imgCoche.Refresh();
                if (plates != null)
                { 
                    foreach(String plate in plates)
                    {
                        if (plate != null && plate.Length > ANPR_MX.Globals.Constants.LIST_PLATE_MIN_CHARS)
                            lstPlacas.Items.Add(plate);
                    }
                }
            }

            this.btnProcesar.Text = "3) Process Image";
            this.btnProcesar.Enabled = true;
        }
    }
}
