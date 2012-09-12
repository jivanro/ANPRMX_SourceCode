namespace ANPR_MX_Sample
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.imgCoche = new System.Windows.Forms.PictureBox();
            this.btnProcesar = new System.Windows.Forms.Button();
            this.lstImages = new System.Windows.Forms.ListBox();
            this.imgPreview = new System.Windows.Forms.PictureBox();
            this.btnCargar = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lstPlacas = new System.Windows.Forms.ListBox();
            ((System.ComponentModel.ISupportInitialize)(this.imgCoche)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgPreview)).BeginInit();
            this.SuspendLayout();
            // 
            // imgCoche
            // 
            this.imgCoche.Location = new System.Drawing.Point(10, 7);
            this.imgCoche.Name = "imgCoche";
            this.imgCoche.Size = new System.Drawing.Size(800, 702);
            this.imgCoche.TabIndex = 0;
            this.imgCoche.TabStop = false;
            // 
            // btnProcesar
            // 
            this.btnProcesar.Location = new System.Drawing.Point(827, 443);
            this.btnProcesar.Name = "btnProcesar";
            this.btnProcesar.Size = new System.Drawing.Size(194, 38);
            this.btnProcesar.TabIndex = 1;
            this.btnProcesar.Text = "3) Process Image";
            this.btnProcesar.UseVisualStyleBackColor = true;
            this.btnProcesar.Click += new System.EventHandler(this.button1_Click);
            // 
            // lstImages
            // 
            this.lstImages.FormattingEnabled = true;
            this.lstImages.Location = new System.Drawing.Point(827, 94);
            this.lstImages.Name = "lstImages";
            this.lstImages.Size = new System.Drawing.Size(194, 329);
            this.lstImages.TabIndex = 2;
            this.lstImages.SelectedIndexChanged += new System.EventHandler(this.lstImages_SelectedIndexChanged);
            this.lstImages.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lstImages_MouseDoubleClick);
            // 
            // imgPreview
            // 
            this.imgPreview.Location = new System.Drawing.Point(1038, 94);
            this.imgPreview.Name = "imgPreview";
            this.imgPreview.Size = new System.Drawing.Size(120, 96);
            this.imgPreview.TabIndex = 3;
            this.imgPreview.TabStop = false;
            // 
            // btnCargar
            // 
            this.btnCargar.Location = new System.Drawing.Point(827, 7);
            this.btnCargar.Name = "btnCargar";
            this.btnCargar.Size = new System.Drawing.Size(194, 38);
            this.btnCargar.TabIndex = 4;
            this.btnCargar.Text = "1) Load Images";
            this.btnCargar.UseVisualStyleBackColor = true;
            this.btnCargar.Click += new System.EventHandler(this.btnCargar_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(824, 65);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "2) Select an Image";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(824, 507);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "4) Results:";
            // 
            // lstPlacas
            // 
            this.lstPlacas.FormattingEnabled = true;
            this.lstPlacas.Location = new System.Drawing.Point(827, 536);
            this.lstPlacas.Name = "lstPlacas";
            this.lstPlacas.Size = new System.Drawing.Size(194, 173);
            this.lstPlacas.TabIndex = 7;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1170, 739);
            this.Controls.Add(this.lstPlacas);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCargar);
            this.Controls.Add(this.imgPreview);
            this.Controls.Add(this.lstImages);
            this.Controls.Add(this.btnProcesar);
            this.Controls.Add(this.imgCoche);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.imgCoche)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imgPreview)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox imgCoche;
        private System.Windows.Forms.Button btnProcesar;
        private System.Windows.Forms.ListBox lstImages;
        private System.Windows.Forms.PictureBox imgPreview;
        private System.Windows.Forms.Button btnCargar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox lstPlacas;
    }
}

