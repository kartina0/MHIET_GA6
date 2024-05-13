namespace Simulator
{
    partial class frmPlcSimulator
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
            btnStart = new Button();
            btnCancel = new Button();
            btnUpdate = new Button();
            comboWorkId = new ComboBox();
            label1 = new Label();
            label2 = new Label();
            comboAngle = new ComboBox();
            btnClose = new Button();
            btnResponse = new Button();
            btnSetItem = new Button();
            label3 = new Label();
            lblStartReq = new Label();
            lblCancelReq = new Label();
            lblCloseReq = new Label();
            lblUpdateReq = new Label();
            lblSetItemReq = new Label();
            label9 = new Label();
            lblInitComp = new Label();
            lblStartComp = new Label();
            lblUpdateComp = new Label();
            lblSetItemComp = new Label();
            lblOutputResult01 = new Label();
            lblOutputResult02 = new Label();
            lblResult01 = new Label();
            lblResult02 = new Label();
            lblWorkIdIndex = new Label();
            lblAngleIndex = new Label();
            tmrUpdateDisplay = new System.Windows.Forms.Timer(components);
            ucWorkInfo1 = new ucWorkInfo();
            ucWorkInfo2 = new ucWorkInfo();
            ucWorkInfo3 = new ucWorkInfo();
            btnStartLong = new Button();
            btnErrorClear = new Button();
            SuspendLayout();
            // 
            // btnStart
            // 
            btnStart.BackColor = Color.Gold;
            btnStart.Font = new Font("メイリオ", 9F, FontStyle.Regular, GraphicsUnit.Point);
            btnStart.Location = new Point(5, 11);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(112, 37);
            btnStart.TabIndex = 1;
            btnStart.Text = "検査要求";
            btnStart.UseVisualStyleBackColor = false;
            btnStart.Click += button_Click;
            // 
            // btnCancel
            // 
            btnCancel.BackColor = Color.Gold;
            btnCancel.Font = new Font("メイリオ", 9F, FontStyle.Regular, GraphicsUnit.Point);
            btnCancel.Location = new Point(123, 11);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(112, 37);
            btnCancel.TabIndex = 1;
            btnCancel.Text = "検査中断要求";
            btnCancel.UseVisualStyleBackColor = false;
            btnCancel.Click += button_Click;
            // 
            // btnUpdate
            // 
            btnUpdate.BackColor = Color.Gold;
            btnUpdate.Font = new Font("メイリオ", 9F, FontStyle.Regular, GraphicsUnit.Point);
            btnUpdate.Location = new Point(274, 11);
            btnUpdate.Name = "btnUpdate";
            btnUpdate.Size = new Size(100, 37);
            btnUpdate.TabIndex = 1;
            btnUpdate.Text = "機種情報取得";
            btnUpdate.UseVisualStyleBackColor = false;
            btnUpdate.Click += button_Click;
            // 
            // comboWorkId
            // 
            comboWorkId.BackColor = Color.Wheat;
            comboWorkId.Font = new Font("メイリオ", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            comboWorkId.FormattingEnabled = true;
            comboWorkId.Location = new Point(1031, 16);
            comboWorkId.Name = "comboWorkId";
            comboWorkId.Size = new Size(77, 25);
            comboWorkId.TabIndex = 2;
            comboWorkId.SelectedIndexChanged += comboWorkId_SelectedIndexChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("メイリオ", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            label1.Location = new Point(951, 21);
            label1.Name = "label1";
            label1.Size = new Size(87, 17);
            label1.TabIndex = 3;
            label1.Text = "作業ID Index：";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("メイリオ", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            label2.Location = new Point(1114, 21);
            label2.Name = "label2";
            label2.Size = new Size(75, 17);
            label2.TabIndex = 3;
            label2.Text = "角度 Index：";
            // 
            // comboAngle
            // 
            comboAngle.BackColor = Color.Wheat;
            comboAngle.Font = new Font("メイリオ", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            comboAngle.FormattingEnabled = true;
            comboAngle.Location = new Point(1184, 16);
            comboAngle.Name = "comboAngle";
            comboAngle.Size = new Size(77, 25);
            comboAngle.TabIndex = 2;
            comboAngle.SelectedIndexChanged += comboAngle_SelectedIndexChanged;
            // 
            // btnClose
            // 
            btnClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClose.BackColor = Color.Yellow;
            btnClose.Font = new Font("メイリオ", 9F, FontStyle.Regular, GraphicsUnit.Point);
            btnClose.Location = new Point(1771, 13);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(80, 37);
            btnClose.TabIndex = 1;
            btnClose.Text = "閉じる";
            btnClose.UseVisualStyleBackColor = false;
            btnClose.Click += button_Click;
            // 
            // btnResponse
            // 
            btnResponse.BackColor = Color.Gold;
            btnResponse.Font = new Font("メイリオ", 9F, FontStyle.Regular, GraphicsUnit.Point);
            btnResponse.Location = new Point(382, 11);
            btnResponse.Name = "btnResponse";
            btnResponse.Size = new Size(148, 37);
            btnResponse.TabIndex = 1;
            btnResponse.Text = "作業Item完了通知応答";
            btnResponse.UseVisualStyleBackColor = false;
            btnResponse.Click += button_Click;
            // 
            // btnSetItem
            // 
            btnSetItem.BackColor = Color.Gold;
            btnSetItem.Font = new Font("メイリオ", 9F, FontStyle.Regular, GraphicsUnit.Point);
            btnSetItem.Location = new Point(835, 11);
            btnSetItem.Name = "btnSetItem";
            btnSetItem.Size = new Size(100, 37);
            btnSetItem.TabIndex = 1;
            btnSetItem.Text = "項目設定要求";
            btnSetItem.UseVisualStyleBackColor = false;
            btnSetItem.Click += button_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("メイリオ", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            label3.Location = new Point(21, 71);
            label3.Name = "label3";
            label3.Size = new Size(64, 17);
            label3.TabIndex = 4;
            label3.Text = "PLC -> PC";
            // 
            // lblStartReq
            // 
            lblStartReq.AutoSize = true;
            lblStartReq.BackColor = Color.Silver;
            lblStartReq.Font = new Font("メイリオ", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            lblStartReq.Location = new Point(21, 119);
            lblStartReq.Name = "lblStartReq";
            lblStartReq.Size = new Size(52, 17);
            lblStartReq.TabIndex = 4;
            lblStartReq.Text = "検査要求";
            // 
            // lblCancelReq
            // 
            lblCancelReq.AutoSize = true;
            lblCancelReq.BackColor = Color.Silver;
            lblCancelReq.Font = new Font("メイリオ", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            lblCancelReq.Location = new Point(21, 142);
            lblCancelReq.Name = "lblCancelReq";
            lblCancelReq.Size = new Size(74, 17);
            lblCancelReq.TabIndex = 4;
            lblCancelReq.Text = "検査中断要求";
            // 
            // lblCloseReq
            // 
            lblCloseReq.AutoSize = true;
            lblCloseReq.BackColor = Color.Silver;
            lblCloseReq.Font = new Font("メイリオ", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            lblCloseReq.Location = new Point(21, 165);
            lblCloseReq.Name = "lblCloseReq";
            lblCloseReq.Size = new Size(74, 17);
            lblCloseReq.TabIndex = 4;
            lblCloseReq.Text = "検査終了要求";
            // 
            // lblUpdateReq
            // 
            lblUpdateReq.AutoSize = true;
            lblUpdateReq.BackColor = Color.Silver;
            lblUpdateReq.Font = new Font("メイリオ", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            lblUpdateReq.Location = new Point(21, 188);
            lblUpdateReq.Name = "lblUpdateReq";
            lblUpdateReq.Size = new Size(96, 17);
            lblUpdateReq.TabIndex = 4;
            lblUpdateReq.Text = "機種情報取得要求";
            // 
            // lblSetItemReq
            // 
            lblSetItemReq.AutoSize = true;
            lblSetItemReq.BackColor = Color.Silver;
            lblSetItemReq.Font = new Font("メイリオ", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            lblSetItemReq.Location = new Point(21, 211);
            lblSetItemReq.Name = "lblSetItemReq";
            lblSetItemReq.Size = new Size(96, 17);
            lblSetItemReq.TabIndex = 4;
            lblSetItemReq.Text = "検査項目設定要求";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new Font("メイリオ", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            label9.Location = new Point(265, 71);
            label9.Name = "label9";
            label9.Size = new Size(64, 17);
            label9.TabIndex = 4;
            label9.Text = "PC -> PLC";
            // 
            // lblInitComp
            // 
            lblInitComp.AutoSize = true;
            lblInitComp.BackColor = Color.Silver;
            lblInitComp.Font = new Font("メイリオ", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            lblInitComp.Location = new Point(264, 97);
            lblInitComp.Name = "lblInitComp";
            lblInitComp.Size = new Size(118, 17);
            lblInitComp.TabIndex = 4;
            lblInitComp.Text = "カメラ通信初期化完了";
            // 
            // lblStartComp
            // 
            lblStartComp.AutoSize = true;
            lblStartComp.BackColor = Color.Silver;
            lblStartComp.Font = new Font("メイリオ", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            lblStartComp.Location = new Point(265, 119);
            lblStartComp.Name = "lblStartComp";
            lblStartComp.Size = new Size(52, 17);
            lblStartComp.TabIndex = 4;
            lblStartComp.Text = "検査完了";
            // 
            // lblUpdateComp
            // 
            lblUpdateComp.AutoSize = true;
            lblUpdateComp.BackColor = Color.Silver;
            lblUpdateComp.Font = new Font("メイリオ", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            lblUpdateComp.Location = new Point(264, 188);
            lblUpdateComp.Name = "lblUpdateComp";
            lblUpdateComp.Size = new Size(96, 17);
            lblUpdateComp.TabIndex = 4;
            lblUpdateComp.Text = "機種情報取得完了";
            // 
            // lblSetItemComp
            // 
            lblSetItemComp.AutoSize = true;
            lblSetItemComp.BackColor = Color.Silver;
            lblSetItemComp.Font = new Font("メイリオ", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            lblSetItemComp.Location = new Point(264, 211);
            lblSetItemComp.Name = "lblSetItemComp";
            lblSetItemComp.Size = new Size(107, 17);
            lblSetItemComp.TabIndex = 4;
            lblSetItemComp.Text = "検査項目設定要完了";
            // 
            // lblOutputResult01
            // 
            lblOutputResult01.AutoSize = true;
            lblOutputResult01.BackColor = Color.Silver;
            lblOutputResult01.Font = new Font("メイリオ", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            lblOutputResult01.Location = new Point(264, 234);
            lblOutputResult01.Name = "lblOutputResult01";
            lblOutputResult01.Size = new Size(88, 17);
            lblOutputResult01.TabIndex = 4;
            lblOutputResult01.Text = "検査結果出力01";
            // 
            // lblOutputResult02
            // 
            lblOutputResult02.AutoSize = true;
            lblOutputResult02.BackColor = Color.Silver;
            lblOutputResult02.Font = new Font("メイリオ", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            lblOutputResult02.Location = new Point(265, 257);
            lblOutputResult02.Name = "lblOutputResult02";
            lblOutputResult02.Size = new Size(88, 17);
            lblOutputResult02.TabIndex = 4;
            lblOutputResult02.Text = "検査結果出力02";
            // 
            // lblResult01
            // 
            lblResult01.AutoSize = true;
            lblResult01.BackColor = Color.Silver;
            lblResult01.Font = new Font("メイリオ", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            lblResult01.Location = new Point(265, 280);
            lblResult01.Name = "lblResult01";
            lblResult01.Size = new Size(13, 17);
            lblResult01.TabIndex = 4;
            lblResult01.Text = "-";
            // 
            // lblResult02
            // 
            lblResult02.AutoSize = true;
            lblResult02.BackColor = Color.Silver;
            lblResult02.Font = new Font("メイリオ", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            lblResult02.Location = new Point(266, 303);
            lblResult02.Name = "lblResult02";
            lblResult02.Size = new Size(13, 17);
            lblResult02.TabIndex = 4;
            lblResult02.Text = "-";
            // 
            // lblWorkIdIndex
            // 
            lblWorkIdIndex.AutoSize = true;
            lblWorkIdIndex.BackColor = Color.Lime;
            lblWorkIdIndex.Font = new Font("メイリオ", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            lblWorkIdIndex.Location = new Point(21, 234);
            lblWorkIdIndex.Name = "lblWorkIdIndex";
            lblWorkIdIndex.Size = new Size(15, 17);
            lblWorkIdIndex.TabIndex = 4;
            lblWorkIdIndex.Text = "0";
            // 
            // lblAngleIndex
            // 
            lblAngleIndex.AutoSize = true;
            lblAngleIndex.BackColor = Color.Lime;
            lblAngleIndex.Font = new Font("メイリオ", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
            lblAngleIndex.Location = new Point(21, 257);
            lblAngleIndex.Name = "lblAngleIndex";
            lblAngleIndex.Size = new Size(15, 17);
            lblAngleIndex.TabIndex = 4;
            lblAngleIndex.Text = "0";
            // 
            // tmrUpdateDisplay
            // 
            tmrUpdateDisplay.Enabled = true;
            tmrUpdateDisplay.Tick += tmrUpdateDisplay_Tick;
            // 
            // ucWorkInfo1
            // 
            ucWorkInfo1.AutoScroll = true;
            ucWorkInfo1.BackColor = Color.Wheat;
            ucWorkInfo1.Font = new Font("メイリオ", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            ucWorkInfo1.Location = new Point(5, 340);
            ucWorkInfo1.Margin = new Padding(4, 5, 4, 5);
            ucWorkInfo1.Name = "ucWorkInfo1";
            ucWorkInfo1.Size = new Size(369, 304);
            ucWorkInfo1.TabIndex = 5;
            // 
            // ucWorkInfo2
            // 
            ucWorkInfo2.AutoScroll = true;
            ucWorkInfo2.BackColor = Color.Wheat;
            ucWorkInfo2.Font = new Font("メイリオ", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            ucWorkInfo2.Location = new Point(382, 340);
            ucWorkInfo2.Margin = new Padding(4, 5, 4, 5);
            ucWorkInfo2.Name = "ucWorkInfo2";
            ucWorkInfo2.Size = new Size(369, 304);
            ucWorkInfo2.TabIndex = 5;
            // 
            // ucWorkInfo3
            // 
            ucWorkInfo3.AutoScroll = true;
            ucWorkInfo3.BackColor = Color.Wheat;
            ucWorkInfo3.Font = new Font("メイリオ", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            ucWorkInfo3.Location = new Point(759, 340);
            ucWorkInfo3.Margin = new Padding(4, 5, 4, 5);
            ucWorkInfo3.Name = "ucWorkInfo3";
            ucWorkInfo3.Size = new Size(369, 304);
            ucWorkInfo3.TabIndex = 5;
            // 
            // btnStartLong
            // 
            btnStartLong.BackColor = Color.Gold;
            btnStartLong.Font = new Font("メイリオ", 9F, FontStyle.Regular, GraphicsUnit.Point);
            btnStartLong.Location = new Point(561, 11);
            btnStartLong.Name = "btnStartLong";
            btnStartLong.Size = new Size(141, 37);
            btnStartLong.TabIndex = 6;
            btnStartLong.Text = "検査要求(ロングラン)";
            btnStartLong.UseVisualStyleBackColor = false;
            btnStartLong.Click += button_Click;
            // 
            // btnErrorClear
            // 
            btnErrorClear.BackColor = Color.Gold;
            btnErrorClear.Font = new Font("メイリオ", 9F, FontStyle.Regular, GraphicsUnit.Point);
            btnErrorClear.Location = new Point(720, 11);
            btnErrorClear.Name = "btnErrorClear";
            btnErrorClear.Size = new Size(100, 37);
            btnErrorClear.TabIndex = 7;
            btnErrorClear.Text = "エラークリア";
            btnErrorClear.UseVisualStyleBackColor = false;
            btnErrorClear.Click += button_Click;
            // 
            // frmPlcSimulator
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(1858, 673);
            Controls.Add(btnErrorClear);
            Controls.Add(btnStartLong);
            Controls.Add(ucWorkInfo3);
            Controls.Add(ucWorkInfo2);
            Controls.Add(ucWorkInfo1);
            Controls.Add(lblResult02);
            Controls.Add(lblOutputResult02);
            Controls.Add(lblResult01);
            Controls.Add(lblOutputResult01);
            Controls.Add(lblSetItemComp);
            Controls.Add(lblSetItemReq);
            Controls.Add(lblUpdateComp);
            Controls.Add(lblAngleIndex);
            Controls.Add(lblWorkIdIndex);
            Controls.Add(lblUpdateReq);
            Controls.Add(lblCloseReq);
            Controls.Add(lblStartComp);
            Controls.Add(lblCancelReq);
            Controls.Add(lblInitComp);
            Controls.Add(label9);
            Controls.Add(lblStartReq);
            Controls.Add(label3);
            Controls.Add(comboAngle);
            Controls.Add(label2);
            Controls.Add(comboWorkId);
            Controls.Add(label1);
            Controls.Add(btnResponse);
            Controls.Add(btnUpdate);
            Controls.Add(btnSetItem);
            Controls.Add(btnCancel);
            Controls.Add(btnClose);
            Controls.Add(btnStart);
            Font = new Font("メイリオ", 11.25F, FontStyle.Regular, GraphicsUnit.Point);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Name = "frmPlcSimulator";
            Text = "PLC Simulator";
            FormClosing += frmPlcSimulator_FormClosing;
            Load += frmPlcSimulator_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnStart;
        private Button btnCancel;
        private Button btnUpdate;
        private ComboBox comboWorkId;
        private Label label1;
        private Label label2;
        private ComboBox comboAngle;
        private Button btnClose;
        private Button btnResponse;
        private Button btnSetItem;
        private Label label3;
        private Label lblStartReq;
        private Label lblCancelReq;
        private Label lblCloseReq;
        private Label lblUpdateReq;
        private Label lblSetItemReq;
        private Label label9;
        private Label lblInitComp;
        private Label lblStartComp;
        private Label lblUpdateComp;
        private Label lblSetItemComp;
        private Label lblOutputResult01;
        private Label lblOutputResult02;
        private Label lblResult01;
        private Label lblResult02;
        private Label lblWorkIdIndex;
        private Label lblAngleIndex;
        private System.Windows.Forms.Timer tmrUpdateDisplay;
        private ucWorkInfo ucWorkInfo1;
        private ucWorkInfo ucWorkInfo2;
        private ucWorkInfo ucWorkInfo3;
        private Button btnStartLong;
        private Button btnErrorClear;
    }
}