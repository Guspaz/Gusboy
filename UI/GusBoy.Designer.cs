namespace Gusboy
{
    partial class Gusboy
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
            this.txt_messages = new System.Windows.Forms.TextBox();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // txt_messages
            // 
            this.txt_messages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_messages.BackColor = System.Drawing.SystemColors.Control;
            this.txt_messages.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txt_messages.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txt_messages.Location = new System.Drawing.Point(0, 288);
            this.txt_messages.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txt_messages.Multiline = true;
            this.txt_messages.Name = "txt_messages";
            this.txt_messages.ReadOnly = true;
            this.txt_messages.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txt_messages.ShortcutsEnabled = false;
            this.txt_messages.Size = new System.Drawing.Size(346, 222);
            this.txt_messages.TabIndex = 99;
            this.txt_messages.TabStop = false;
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 510);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(346, 22);
            this.statusStrip.SizingGrip = false;
            this.statusStrip.TabIndex = 100;
            this.statusStrip.Text = "statusStrip1";
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // Gusboy
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(346, 532);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.txt_messages);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.Name = "Gusboy";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Gusboy";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Gusboy_FormClosed);
            this.Shown += new System.EventHandler(this.Gusboy_Shown);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Gusboy_Paint);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Gusboy_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Gusboy_KeyUp);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox txt_messages;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
    }
}

