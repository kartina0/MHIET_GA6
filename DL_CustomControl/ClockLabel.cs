using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DL_CustomControl
{
    public partial class ClockLabel : Label
    {
        /// <summary>
        /// タイマ
        /// </summary>
        private System.Threading.Timer _autoLimitCheckTimer = null;
        /// <summary>
        /// 一定周期で時刻を更新する
        /// </summary>
        private int _autoLimitCheckTime = 1000;

        /// <summary>
        /// 表示フォーマット
        /// </summary>
        private string _format = "yyyy年MM月dd日 HH時mm分";
        /// <summary>
        /// @@20190703
        /// </summary>
        [Category("フォーマット")]
        [Description("時刻表示フォーマット")]
        public string DisplayFormat
        {
            get { return _format; }
            set { _format = value; }
        }


        public ClockLabel()
        {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }

        /// <summary>
        /// ハンドル作成
        /// </summary>
        protected override void CreateHandle()
        {
            base.CreateHandle();
            if (!DesignMode && _autoLimitCheckTime > 0)
            {
                _autoLimitCheckTimer = new System.Threading.Timer(_TimerCallback, null, 100, _autoLimitCheckTime);
            }
        }

        /// <summary>
        /// タイマ破棄
        /// </summary>
        protected override void DestroyHandle()
        {
            try
            {
                if (!DesignMode && _autoLimitCheckTimer != null)
                    _autoLimitCheckTimer.Dispose();
                _autoLimitCheckTimer = null;
            }
            catch { }
            base.DestroyHandle();
        }

        /// <summary>
        /// タイマイベント
        /// 定期的にリミット値を確認し背景色を変更する
        /// </summary>
        /// <param name="state"></param>
        public void _TimerCallback(object state)
        {
            try
            {
                this.Invoke((MethodInvoker)delegate
                {
                    this.Text = DateTime.Now.ToString(_format);
                });
            }
            catch { }
        }
    }
}
