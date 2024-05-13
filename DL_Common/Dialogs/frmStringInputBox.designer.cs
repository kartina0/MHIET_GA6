﻿namespace DL_CommonLibrary
{
    partial class frmStringInputBox
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
            this.components = new System.ComponentModel.Container();
            this.lblMessage = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.tmrUpdateWindow = new System.Windows.Forms.Timer(this.components);
            this.btnNo = new System.Windows.Forms.Button();
            this.txtInput = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.BackColor = System.Drawing.Color.White;
            this.lblMessage.Font = new System.Drawing.Font("メイリオ", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblMessage.Location = new System.Drawing.Point(1, 6);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(30, 36);
            this.lblMessage.TabIndex = 0;
            this.lblMessage.Text = "1";
            this.lblMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOK.BackColor = System.Drawing.Color.Lime;
            this.btnOK.Location = new System.Drawing.Point(7, 116);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(104, 43);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "決定";
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.BackColor = System.Drawing.Color.White;
            this.pictureBox1.Location = new System.Drawing.Point(-2, -3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(338, 113);
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // tmrUpdateWindow
            // 
            this.tmrUpdateWindow.Interval = 300;
            this.tmrUpdateWindow.Tick += new System.EventHandler(this.tmrUpdateWindow_Tick);
            // 
            // btnNo
            // 
            this.btnNo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNo.BackColor = System.Drawing.Color.White;
            this.btnNo.Location = new System.Drawing.Point(232, 116);
            this.btnNo.Name = "btnNo";
            this.btnNo.Size = new System.Drawing.Size(104, 43);
            this.btnNo.TabIndex = 3;
            this.btnNo.Text = "戻る";
            this.btnNo.UseVisualStyleBackColor = false;
            this.btnNo.Click += new System.EventHandler(this.btnNo_Click);
            // 
            // txtInput
            // 
            this.txtInput.Location = new System.Drawing.Point(10, 53);
            this.txtInput.Name = "txtInput";
            this.txtInput.Size = new System.Drawing.Size(314, 43);
            this.txtInput.TabIndex = 4;
            // 
            // frmStringInputBox
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(338, 164);
            this.ControlBox = false;
            this.Controls.Add(this.txtInput);
            this.Controls.Add(this.btnNo);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.pictureBox1);
            this.Font = new System.Drawing.Font("メイリオ", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "frmStringInputBox";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmMessageBox";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmStringInputBox_FormClosing);
            this.Load += new System.EventHandler(this.frmMessageBox_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Timer tmrUpdateWindow;
        private System.Windows.Forms.Button btnNo;
        private System.Windows.Forms.TextBox txtInput;
    }
}