using System.Reflection;
using System.Diagnostics;

using PLC;
using DL_CommonLibrary;
using DL_Logger;
using SystemConfig;
using ErrorCodeDefine;
using System;


namespace Simulator
{
    /// <summary>
    /// PLCシミュレーター画面
    /// </summary>
    public partial class frmPlcSimulator : Form
    {
        /// <summary>
        /// 自クラス名
        /// </summary>
        private const string THIS_NAME = "frmMain";

        /// <summary>
        /// PLCシモジュール
        /// </summary>
        private PlcManager _plc = null;

        /// <summary>
        /// PLCサイクルスレッド
        /// </summary>
        private ThreadInfo _thread = new ThreadInfo(THREAD_SEQUENCE_TYPE.CONTINUOUS);
        /// <summary>
        /// スレッド シャットダウン
        /// </summary>
        private bool _shutDown = false;
        /// <summary>
        /// スレッド シャットダウン完了
        /// </summary>
        private bool _shutDownComp = false;

        /// <summary>
        /// 作業IDインデックス
        /// </summary>
        private int _workIdIndex = 0;
        /// <summary>
        /// 角度インデックス
        /// </summary>
        private int _angleIndex = 0;

        /// <summary>
        /// ロングランモード
        /// </summary>
        private bool _longRunMode = false;



        /// <summary>
        /// コンストラクタ
        /// </summary>
        public frmPlcSimulator(PlcManager plc)
        {
            InitializeComponent();

            // PLCモジュール
            _plc = plc;
        }
        /// <summary>
        /// フォームロード
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmPlcSimulator_Load(object sender, EventArgs e)
        {
            UInt32 rc = 0;
            try
            {
                // 各作業IDユーザーコントロールを初期化
                for (int i = 0; i < 3; i++)
                {
                    string ctrlName = $"ucWorkInfo{i + 1}";
                    object ctrl = WindowFunc.FindControl(this, ctrlName);
                    if (ctrl != null)
                    {
                        ((ucWorkInfo)ctrl).SetIndex(i);
                        ((ucWorkInfo)ctrl).SetPlc(_plc);
                    }
                }

                // コンボ 作業IDインデックス
                for (int i = 0; i < IniFile.MaxWorkIdCount; i++)
                    comboWorkId.Items.Add(i);
                comboWorkId.SelectedIndex = 0;
                // コンボ 角度インデックス
                for (int i = 0; i < IniFile.MaxWorkOrderCount; i++)
                    comboAngle.Items.Add(i);
                comboAngle.SelectedIndex = 0;
                //// コンボ 作業Item完了通知応答動作
                //comboWorkItemResponse.Items.Add("通常動作");
                //comboWorkItemResponse.Items.Add("再実施");
                //comboWorkItemResponse.Items.Add("強制終了");
                //comboWorkItemResponse.SelectedIndex = 0;


                // スレッド開始
                ThreadStart();

            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
            }
        }
        /// <summary>
        /// フォームクローズ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmPlcSimulator_FormClosing(object sender, FormClosingEventArgs e)
        {
            UInt32 rc = 0;
            try
            {
                // PLCクローズ
                _plc = null;

                ThreadClose();
                _thread = null;

            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
            }
        }
        /// <summary>
        /// ボタンクリック イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Click(object sender, EventArgs e)
        {
            UInt32 rc = 0;
            Button ctrl = (Button)sender;
            try
            {
                if (ctrl == btnStart)
                {// 検査要求

                    // 作業IDインデクス
                    _workIdIndex = comboWorkId.SelectedIndex;
                    rc = _plc.Debug_SetSelectedIdIndex(comboWorkId.SelectedIndex);
                    // 角度インデックス
                    _angleIndex = comboAngle.SelectedIndex;
                    rc = _plc.Debug_SetSelectedAngleIndex(comboAngle.SelectedIndex);
                    // 検査項目設定要求 ON
                    rc = _plc.Debug_SetItemReq(true);

                    // 検査要求 ON
                    rc = _plc.Debug_SetExamReq(true);
                }
                else if (ctrl == btnCancel)
                {// 検査中断要求
                    rc = _plc.Debug_SetCencelReq(true);
                }
                else if (ctrl == btnUpdate)
                {// 機種情報取得要求
                    rc = _plc.Debug_SetUpdateReq(true);
                }
                else if (ctrl == btnStartLong)
                {// 検査要求(ロングラン)

                    _longRunMode = true;


                    // 作業IDインデクス
                    _workIdIndex = comboWorkId.SelectedIndex;
                    rc = _plc.Debug_SetSelectedIdIndex(comboWorkId.SelectedIndex);
                    // 角度インデックス
                    _angleIndex = comboAngle.SelectedIndex;
                    rc = _plc.Debug_SetSelectedAngleIndex(comboAngle.SelectedIndex);
                    // 検査項目設定要求 ON
                    rc = _plc.Debug_SetItemReq(true);

                    // 検査要求 ON
                    rc = _plc.Debug_SetExamReq(true);

                }
                else if (ctrl == btnErrorClear)
                {// エラークリア

                    // エラーコード クリア
                    rc = _plc.SetErrorCode(0);
                }
                //else if (ctrl == btnResponse)
                //{// 作業Item完了通知応答動作
                //    rc = _plc.Debug_SetResponse();
                //}
                else if (ctrl == btnClose)
                {// 閉じる
                    this.Close();
                }


            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
            }
        }

        /// <summary>
        /// コンボボックス作業IDインデックス 変更イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboWorkId_SelectedIndexChanged(object sender, EventArgs e)
        {
            UInt32 rc = 0;
            try
            {
                rc = _plc.Debug_SetSelectedIdIndex(comboWorkId.SelectedIndex);
            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
            }
        }
        /// <summary>
        /// コンボボックス角度インデックス 変更イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboAngle_SelectedIndexChanged(object sender, EventArgs e)
        {
            UInt32 rc = 0;
            try
            {
                rc = _plc.Debug_SetSelectedAngleIndex(comboAngle.SelectedIndex);
            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
            }
        }
        /// <summary>
        /// コンボボックス作業Item完了通知応答インデックス 変更イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboWorkItemResponse_SelectedIndexChanged(object sender, EventArgs e)
        {
            UInt32 rc = 0;
            try
            {
                //rc = _plc.Debug_SetWorkItemResponseIndex(comboWorkItemResponse.SelectedIndex);
            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
            }
        }

        /// <summary>
        /// 画面更新タイマ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tmrUpdateDisplay_Tick(object sender, EventArgs e)
        {
            UpdateDisplay();
        }
        /// <summary>
        /// 画面更新
        /// </summary>
        private void UpdateDisplay()
        {
            UInt32 rc = 0;

            string ctrlName = "";
            object ctrl = null;

            try
            {
                // ------------------------------------------
                // PLCステータス 読み出し
                // ------------------------------------------

                // PC -> PLC
                rc = _plc.Debug_GetStatus(out Out status_out);

                if (status_out.InitComp)
                    lblInitComp.BackColor = Color.Lime;
                else
                    lblInitComp.BackColor = Color.Silver;

                if (status_out.StartComp)
                    lblStartComp.BackColor = Color.Lime;
                else
                    lblStartComp.BackColor = Color.Silver;

                if (status_out.UpdateComp)
                    lblUpdateComp.BackColor = Color.Lime;
                else
                    lblUpdateComp.BackColor = Color.Silver;

                if (status_out.ItemInputComp)
                    lblSetItemComp.BackColor = Color.Lime;
                else
                    lblSetItemComp.BackColor = Color.Silver;

                for (int i = 0; i < IniFile.CameraCount; i++)
                {
                    if (status_out.OutputResultComp[i])
                    {
                        ctrlName = $"lblOutputResult0{i + 1}";
                        ctrl = WindowFunc.FindControl(this, ctrlName);
                        ((Label)ctrl).BackColor = Color.Lime;

                        ctrlName = $"lblResult0{i + 1}";
                        ctrl = WindowFunc.FindControl(this, ctrlName);
                        ((Label)ctrl).BackColor = Color.Lime;

                        if (status_out.Result[i]) ((Label)ctrl).Text = "OK";
                        else ((Label)ctrl).Text = "NG";
                    }
                    else
                    {
                        ctrlName = $"lblOutputResult0{i + 1}";
                        ctrl = WindowFunc.FindControl(this, ctrlName);
                        ((Label)ctrl).BackColor = Color.Silver;

                        ctrlName = $"lblResult0{i + 1}";
                        ctrl = WindowFunc.FindControl(this, ctrlName);
                        ((Label)ctrl).BackColor = Color.Silver;
                    }
                }


                // PLC -> PC
                rc = _plc.GetStatus(out In status_in);

                if (status_in.ExamReq)
                    lblStartReq.BackColor = Color.Lime;
                else
                    lblStartReq.BackColor = Color.Silver;

                if (status_in.CancelReq)
                    lblCancelReq.BackColor = Color.Lime;
                else
                    lblCancelReq.BackColor = Color.Silver;

                if (status_in.UpdateReq)
                    lblUpdateReq.BackColor = Color.Lime;
                else
                    lblUpdateReq.BackColor = Color.Silver;

                if (status_in.ItemInputReq)
                {
                    lblSetItemReq.BackColor = Color.Lime;
                    lblWorkIdIndex.BackColor = Color.Lime;
                    lblAngleIndex.BackColor = Color.Lime;
                }
                else
                {
                    lblSetItemReq.BackColor = Color.Silver;
                    lblWorkIdIndex.BackColor = Color.Silver;
                    lblAngleIndex.BackColor = Color.Silver;
                }

                if (status_in.ItemInputReq) lblWorkIdIndex.Text = status_in.WorkIdIndex.ToString();
                else lblAngleIndex.Text = status_in.AngleIndex.ToString();




                //comboWorkId.SelectedIndex = _workIdIndex;
                //comboAngle.SelectedIndex = _angleIndex;

            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
            }
        }





        /// <summary>
        /// 起動
        /// </summary>
        /// <returns></returns>
        public UInt32 ThreadStart()
        {
            UInt32 rc = 0;
            Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
            try
            {
                // スレッド起動
                _thread.CreateThread(Thread_Cycle, _thread, ThreadPriority.Lowest);
                _thread.Interval = 30;
                _thread.Release();
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, ex.ToString());
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
            return rc;
        }
        /// <summary>
        /// 終了
        /// </summary>
        public UInt32 ThreadClose()
        {
            UInt32 rc = 0;
            Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
            try
            {
                // ----------------------------------
                // スレッド終了
                // ----------------------------------
                if (_thread != null)
                {
                    _shutDown = true;
                    while (!_shutDownComp)
                        Thread.Sleep(100);
                }
                _thread = null;
                Logger.WriteLog(LogType.INFO, string.Format("サイクル管理スレッド 終了"));
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, ex.ToString());
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
            return rc;
        }
        /// <summary>
        /// PLCサイクルスレッド
        /// </summary>
        private void Thread_Cycle(object arg)
        {
            UInt32 rc = 0;
            ThreadInfo info = (ThreadInfo)arg;
            Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
            try
            {
                bool exit = false;
                Stopwatch sw = Stopwatch.StartNew();

                // 前回の検査項目設定要求
                bool pre_ItemInputReq = false;
                // 前回の検査結果出力要求
                bool pre_OutputResultReq = false;

                while (true)
                {
                    rc = 0;
                    if (_shutDown)
                        break;

                    // Waits Any Event
                    THREAD_WAIT_RESULT index = info.WaitAnyEvent(100);

                    if (index == THREAD_WAIT_RESULT.SHUTDOWN)
                    {
                        exit = true;
                    }
                    if (index == THREAD_WAIT_RESULT.REQUEST)
                    {

                        // PLCステータス 読み出し
                        rc = _plc.Debug_GetStatus(out Out status_Out);
                        rc = _plc.GetStatus(out In status_In);

                        // PLC 機種情報 読み出し
                        rc = _plc.Debug_GetWorkInfo(_workIdIndex, out WorkInfo workInfo);



                        // -----------------------------------------
                        // エラーコード処理
                        // -----------------------------------------
                        if (status_Out.ErrorCode == 1)
                        {// 検査 START要求エラー
                            // 検査要求 OFF
                            rc = _plc.Debug_SetExamReq(false);
                        }


                        // -----------------------------------------
                        // カメラ通信初期化完了処理
                        // -----------------------------------------
                        if (status_Out.InitComp)
                        {// カメラ通信初期化完了 ON

                            // 機種情報取得要求 ON
                            rc = _plc.Debug_SetUpdateReq(true);
                            // カメラ通信初期化完了 OFF
                            rc = _plc.SetInitComp(false);
                        }

                        // -----------------------------------------
                        // 機種情報取得完了処理
                        // -----------------------------------------
                        if (status_Out.UpdateComp)
                        {// 機種情報取得完了 ON

                            // 機種情報取得要求 OFF
                            rc = _plc.Debug_SetUpdateReq(false);
                            // 機種情報取得完了 OFF
                            rc = _plc.SetUpdateComp(false);
                        }


                        // -----------------------------------------
                        // 検査処理
                        // -----------------------------------------
                        if (!_longRunMode)
                        {// 通常モード



                            if (status_In.ExamReq)
                            {// 検査要求 ON

                                // ----------------------------------------------
                                // 検査項目設定
                                if (status_Out.ItemInputComp)
                                {// 検査項目設定完了 ON

                                    // 検査項目設定要求 OFF
                                    rc = _plc.Debug_SetItemReq(false);
                                    // 検査項目設定完了 OFF
                                    rc = _plc.SetItemComp(false);
                                }
                                if (!pre_ItemInputReq && status_In.ItemInputReq)
                                {// 検査項目設定要求がONになった

                                    // 角度インデックス入力
                                    rc = _plc.Debug_SetSelectedAngleIndex(_angleIndex);

                                }
                                if (pre_ItemInputReq && !status_In.ItemInputReq)
                                {// 検査項目設定要求がOFFになった

                                    // 検査結果出力要求 ON
                                    rc = _plc.Debug_SetOutPutResultReq(true);
                                }

                                if (status_In.ItemInputReq)
                                    pre_ItemInputReq = true;
                                else
                                    pre_ItemInputReq = false;


                                // ----------------------------------------------
                                // 検査結果出力
                                if (status_Out.OutputResultComp.All(x => x))
                                {// 検査結果出力完了 ON

                                    // 検査結果出力要求 OFF
                                    rc = _plc.Debug_SetOutPutResultReq(false);

                                    for (int i = 0; i < IniFile.CameraCount; i++)
                                    {// クリア
                                        // 検査結果出力完了 OFF
                                        rc = _plc.SetOutputResultComp(i, false);
                                    }

                                    if (!status_Out.Result.All(x => x))
                                    {// 検査結果NG

                                        // 検査要求 OFF
                                        rc = _plc.Debug_SetExamReq(false);
                                    }
                                }
                                if (pre_OutputResultReq && !status_In.OutputResultReq)
                                {// 検査結果出力要求がOFFになった

                                    if (_angleIndex >= 30 || workInfo.AngleList[_angleIndex + 1].angle == -1)
                                    {// 検査終了

                                        // 検査要求 OFF
                                        rc = _plc.Debug_SetExamReq(false);
                                    }
                                    else
                                    {
                                        // 次の角度
                                        _angleIndex++;
                                        Logger.WriteLog(LogType.INFO, "++");

                                        // 検査項目設定要求 ON
                                        rc = _plc.Debug_SetItemReq(true);
                                    }
                                }

                                if (status_In.OutputResultReq)
                                    pre_OutputResultReq = true;
                                else
                                    pre_OutputResultReq = false;
                            }
                            else
                            {// 検査要求 OFF

                                // 検査中断要求 OFF
                                rc = _plc.Debug_SetCencelReq(false);
                                // 検査項目設定要求 OFF
                                rc = _plc.Debug_SetItemReq(false);
                                // 検査結果出力要求 OFF
                                rc = _plc.Debug_SetOutPutResultReq(false);


                            }
                        }
                        else
                        {// ロングランモード

                            if (status_In.ExamReq)
                            {// 検査要求 ON

                                // ----------------------------------------------
                                // 検査項目設定
                                if (status_Out.ItemInputComp)
                                {// 検査項目設定完了 ON

                                    // 検査項目設定要求 OFF
                                    rc = _plc.Debug_SetItemReq(false);
                                    // 検査項目設定完了 OFF
                                    rc = _plc.SetItemComp(false);
                                }
                                if (!pre_ItemInputReq && status_In.ItemInputReq)
                                {// 検査項目設定要求がONになった

                                    // 角度インデックス入力
                                    rc = _plc.Debug_SetSelectedAngleIndex(_angleIndex);

                                }
                                if (pre_ItemInputReq && !status_In.ItemInputReq)
                                {// 検査項目設定要求がOFFになった

                                    // 検査結果出力要求 ON
                                    rc = _plc.Debug_SetOutPutResultReq(true);
                                }

                                if (status_In.ItemInputReq)
                                    pre_ItemInputReq = true;
                                else
                                    pre_ItemInputReq = false;


                                // ----------------------------------------------
                                // 検査結果出力
                                if (status_Out.OutputResultComp.All(x => x))
                                {// 検査結果出力完了 ON

                                    // 検査結果出力要求 OFF
                                    rc = _plc.Debug_SetOutPutResultReq(false);

                                    for (int i = 0; i < IniFile.CameraCount; i++)
                                    {// クリア

                                        // 検査結果出力完了 OFF
                                        rc = _plc.SetOutputResultComp(i, false);
                                    }
                                }
                                if (pre_OutputResultReq && !status_In.OutputResultReq)
                                {// 検査結果出力要求がOFFになった

                                    if (_angleIndex >= 30 || workInfo.AngleList[_angleIndex + 1].angle == -1)
                                    {// 検査終了
                                        rc = _plc.Debug_SetExamReq(false);
                                    }
                                    else
                                    {
                                        // 次の角度
                                        _angleIndex++;
                                        Logger.WriteLog(LogType.INFO, "++");

                                        // 検査項目設定要求 ON
                                        rc = _plc.Debug_SetItemReq(true);
                                    }
                                }

                                if (status_In.OutputResultReq)
                                    pre_OutputResultReq = true;
                                else
                                    pre_OutputResultReq = false;
                            }
                            else
                            {// 検査要求 OFF

                                // 作業IDインデクス
                                _workIdIndex++;
                                rc = _plc.Debug_SetSelectedIdIndex(comboWorkId.SelectedIndex);
                                // 角度インデックス
                                _angleIndex = 0;
                                rc = _plc.Debug_SetSelectedAngleIndex(comboAngle.SelectedIndex);
                                // 検査項目設定要求 ON
                                rc = _plc.Debug_SetItemReq(true);

                                // 検査要求 ON
                                rc = _plc.Debug_SetExamReq(true);
                            }


                        }




                    }
                    if (exit) break;
                    Thread.Sleep(info.Interval);
                }

            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, ex.ToString());
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            Logger.WriteLog(LogType.METHOD_OUT, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name} : {(ErrorCodeList)rc}");
            _shutDownComp = true;
        }
    }

}