namespace GA6_ControlApp
{
    partial class frmMain
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
            components = new System.ComponentModel.Container();
            btnExit = new Button();
            tmrUpdateDisplay = new System.Windows.Forms.Timer(components);
            listViewLog = new ListView();
            SuspendLayout();
            // 
            // btnExit
            // 
            btnExit.Location = new Point(861, 535);
            btnExit.Name = "btnExit";
            btnExit.Size = new Size(104, 49);
            btnExit.TabIndex = 0;
            btnExit.Text = "アプリ終了";
            btnExit.UseVisualStyleBackColor = true;
            btnExit.Click += btnExit_Click;
            // 
            // tmrUpdateDisplay
            // 
            tmrUpdateDisplay.Enabled = true;
            tmrUpdateDisplay.Interval = 500;
            tmrUpdateDisplay.Tick += tmrUpdateDisplay_Tick;
            // 
            // listViewLog
            // 
            listViewLog.Location = new Point(12, 24);
            listViewLog.Name = "listViewLog";
            listViewLog.Size = new Size(953, 505);
            listViewLog.TabIndex = 1;
            listViewLog.UseCompatibleStateImageBehavior = false;
            listViewLog.View = View.Details;
            // 
            // frmMain
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.DarkSeaGreen;
            ClientSize = new Size(978, 596);
            Controls.Add(listViewLog);
            Controls.Add(btnExit);
            Font = new Font("メイリオ", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Name = "frmMain";
            Text = "Form1";
            WindowState = FormWindowState.Minimized;
            FormClosing += Form1_FormClosing;
            Load += Form1_Load;
            ResumeLayout(false);
        }

        #endregion

        private Button btnExit;
        private System.Windows.Forms.Timer tmrUpdateDisplay;
        private ListView listViewLog;
    }
}