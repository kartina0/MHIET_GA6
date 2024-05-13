using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using PLC;
using SystemConfig;
using DL_CommonLibrary;


namespace Simulator
{
    /// <summary>
    /// ユーザーコントロール 
    /// 作業情報デバイスマップ (作業ID 1個分)
    /// </summary>
    public partial class ucWorkInfo : UserControl
    {
        /// <summary>
        /// 自クラス名
        /// </summary>
        private const string THIS_NAME = "frmMain";

        /// <summary>
        /// PLCモジュール
        /// </summary>
        private PlcManager _plc = null;
        /// <summary>
        /// 作業IDインデックス
        /// </summary>
        private int _index = -1;

        /// <summary>
        /// 初期化フラグ
        /// </summary>
        public bool _init = true;
        /// <summary>
        /// 画面更新タイマー　重複防止
        /// </summary>
        private bool _updating = false;


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="plc"></param>
        public ucWorkInfo()
        {
            InitializeComponent();
        }


        /// <summary>
        /// PLCモジュールを渡す
        /// </summary>
        /// <param name="plc"></param>
        /// <returns></returns>
        public UInt32 SetIndex(int index)
        {
            UInt32 rc = 0;
            try
            {
                _index = index;
            }
            catch (Exception ex)
            {

            }
            return rc;
        }
        /// <summary>
        /// PLCモジュールを渡す
        /// </summary>
        /// <param name="plc"></param>
        /// <returns></returns>
        public UInt32 SetPlc(PlcManager plc)
        {
            UInt32 rc = 0;
            try
            {
                // PLC
                _plc = plc;
            }
            catch (Exception ex)
            {

            }
            return rc;
        }
        /// <summary>
        /// 作業情報を更新
        /// </summary>
        /// <returns></returns>
        public UInt32 UpdateWorkInfo()
        {
            UInt32 rc = 0;
            try
            {
                // -----------------------------------------
                // 作業情報読み出し
                // -----------------------------------------
                // 作業IDデータ取得
                rc = _plc.Debug_GetWorkInfo(_index, out WorkInfo info);


                // -----------------------------------------
                // 画面表示
                // -----------------------------------------
                // 作業ID名
                txtWorkIdName.Text = info.workIdName;
                // 角度
                for (int i = 0; i < IniFile.MaxWorkOrderCount; i++)
                {
                    string ctrlName = $"txtAngle{i + 1}";
                    object ctrl = WindowFunc.FindControl(this, ctrlName);
                    ((TextBox)ctrl).Text = info.AngleList[i].angle.ToString();

                    for (int j = 0; j < IniFile.CameraCount; j++)
                    {
                        ctrlName = $"txtCamera0{j + 1}_Part{i + 1}";
                        ctrl = WindowFunc.FindControl(this, ctrlName);
                        ((TextBox)ctrl).Text = info.AngleList[i].camera[j].partName;
                    }
                }

            }
            catch (Exception ex)
            {

            }
            return rc;
        }



        /// <summary>
        /// 画面更新タイマー イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tmrUpdateDisplay_Tick(object sender, EventArgs e)
        {
            if (DesignMode)
                return;

            UpdateDisplay();
        }
        /// <summary>
        /// 画面更新
        /// </summary>
        private void UpdateDisplay()
        {
            UInt32 rc = 0;
            try
            {
                // 重複防止
                if (_updating)
                    return;
                _updating = true;

                // 初期化処理
                if (_init)
                {
                    // 作業情報を更新表示
                    rc = UpdateWorkInfo();

                    //// 作業IDデータ取得
                    //rc = _plc.Debug_GetWorkInfo(_index, out WorkInfo info);
                    //// 画面表示
                    //SetWorkInfo(info);
                }


            }
            catch (Exception ex)
            {

            }
            finally
            {
                _updating = false;
            }
        }




    }
}
