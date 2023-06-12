
namespace GusboyGbsPlayer
{
    partial class Player
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.PrevTrackBtn = new System.Windows.Forms.Button();
            this.NextTrackBtn = new System.Windows.Forms.Button();
            this.TitleLbl = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SongNumbersLbl = new System.Windows.Forms.Label();
            this.waveformPainter1 = new NAudio.Gui.WaveformPainter();
            this.SuspendLayout();
            // 
            // PrevTrackBtn
            // 
            this.PrevTrackBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.PrevTrackBtn.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.PrevTrackBtn.Location = new System.Drawing.Point(12, 166);
            this.PrevTrackBtn.Name = "PrevTrackBtn";
            this.PrevTrackBtn.Size = new System.Drawing.Size(75, 75);
            this.PrevTrackBtn.TabIndex = 0;
            this.PrevTrackBtn.TabStop = false;
            this.PrevTrackBtn.Text = "⏮️";
            this.PrevTrackBtn.UseVisualStyleBackColor = true;
            this.PrevTrackBtn.Click += new System.EventHandler(this.PrevTrackBtn_Click);
            // 
            // NextTrackBtn
            // 
            this.NextTrackBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.NextTrackBtn.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.NextTrackBtn.Location = new System.Drawing.Point(289, 166);
            this.NextTrackBtn.Name = "NextTrackBtn";
            this.NextTrackBtn.Size = new System.Drawing.Size(75, 75);
            this.NextTrackBtn.TabIndex = 1;
            this.NextTrackBtn.TabStop = false;
            this.NextTrackBtn.Text = "⏭️";
            this.NextTrackBtn.UseVisualStyleBackColor = true;
            this.NextTrackBtn.Click += new System.EventHandler(this.NextTrackBtn_Click);
            // 
            // TitleLbl
            // 
            this.TitleLbl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TitleLbl.BackColor = System.Drawing.SystemColors.Control;
            this.TitleLbl.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.TitleLbl.Location = new System.Drawing.Point(12, 9);
            this.TitleLbl.Name = "TitleLbl";
            this.TitleLbl.Size = new System.Drawing.Size(352, 73);
            this.TitleLbl.TabIndex = 2;
            this.TitleLbl.Text = "\r\nDrag and drop file here";
            this.TitleLbl.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.BackColor = System.Drawing.SystemColors.Control;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(93, 166);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(190, 23);
            this.label1.TabIndex = 3;
            this.label1.Text = "Now Playing";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // SongNumbersLbl
            // 
            this.SongNumbersLbl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SongNumbersLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.SongNumbersLbl.Location = new System.Drawing.Point(93, 178);
            this.SongNumbersLbl.Name = "SongNumbersLbl";
            this.SongNumbersLbl.Size = new System.Drawing.Size(190, 63);
            this.SongNumbersLbl.TabIndex = 4;
            this.SongNumbersLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // waveformPainter1
            // 
            this.waveformPainter1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.waveformPainter1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(198)))), ((int)(((byte)(203)))), ((int)(((byte)(165)))));
            this.waveformPainter1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(74)))), ((int)(((byte)(81)))), ((int)(((byte)(57)))));
            this.waveformPainter1.Location = new System.Drawing.Point(0, 85);
            this.waveformPainter1.Name = "waveformPainter1";
            this.waveformPainter1.Size = new System.Drawing.Size(376, 75);
            this.waveformPainter1.TabIndex = 5;
            this.waveformPainter1.Text = "waveformPainter1";
            // 
            // Player
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(376, 253);
            this.Controls.Add(this.waveformPainter1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.SongNumbersLbl);
            this.Controls.Add(this.TitleLbl);
            this.Controls.Add(this.NextTrackBtn);
            this.Controls.Add(this.PrevTrackBtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Player";
            this.Text = "Gusboy GBS Player";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Player_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Player_DragEnter);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button PrevTrackBtn;
        private System.Windows.Forms.Button NextTrackBtn;
        private System.Windows.Forms.Label TitleLbl;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label SongNumbersLbl;
        private NAudio.Gui.WaveformPainter waveformPainter1;
    }
}

