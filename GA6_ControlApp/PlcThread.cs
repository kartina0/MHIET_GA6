using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using PLC;
using Camera;
using DL_CommonLibrary;
using DL_Logger;
using SystemConfig;
using ErrorCodeDefine;
using System.Collections;


namespace GA6_ControlApp
{
    public class PlcThread
    {
        /// <summary>
        /// 自クラス名
        /// </summary>
        private const string THIS_NAME = "PlcThread";

        /// <summary>
        /// PLCモジュール
        /// </summary>
        private PlcManager _PLC = null;
        /// <summary>
        /// カメラモジュール
        /// </summary>
        private CameraManager[] _Camera = null;
        /// <summary>
        /// 作業情報リスト
        /// 作業IDx100個 作業指示リスト30個 からなるクラスリスト
        /// </summary>
        public WorkInfo[] _workInfoList = null;

        /// <summary>
        /// サイクル管理スレッド
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
        /// コンストラクタ
        /// </summary>
        public PlcThread(PlcManager plc, CameraManager[] camera, WorkInfo[] workInfo) 
        {
            // PLC
            _PLC = plc;
            // カメラ
            _Camera = camera;
            // 作業情報リスト
            _workInfoList = workInfo;
        }


        /// <summary>
        /// 起動
        /// </summary>
        /// <returns></returns>
        public UInt32 Start()
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
        public UInt32 Close()
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
        /// サイクルスレッド
        /// </summary>
        private void Thread_Cycle(object arg)
        {
            UInt32 rc = 0;
            ThreadInfo info = (ThreadInfo)arg;
            Logger.WriteLog(LogType.METHOD_IN, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}()");
            try
            {
                bool exit = false;


                bool preCancelReq = false;


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
                        rc =  _PLC.GetStatus(out In status);

                        // -----------------------------------------
                        // カメラ通信初期化完了処理
                        // -----------------------------------------
                        if (_Camera.All(x => x.InitSeq == Sequence.COMP))
                        {
                            // カメラ通信初期化完了
                            rc = _PLC.SetInitComp(true);
                            for (int i = 0; i < IniFile.CameraCount; i++)
                                _Camera[i].InitSeq = Sequence.NONE;

                            if (STATUS_SUCCESS(rc))
                                Logger.WriteLog(LogType.INFO, "カメラ通信初期化 完了");
                        }


                        // -----------------------------------------
                        // 検査処理
                        // -----------------------------------------
                        if (!status.ExamReq)
                        {// 検査要求 : OFF


                            for (int i = 0; i < IniFile.CameraCount; i++)
                                _Camera[i].ExamSeq = EXAM_SEQUENCE.NONE;
                        }
                        else
                        {// 検査要求 : ON

                            for (int i = 0; i < IniFile.CameraCount; i++) 
                            {
                                if (_Camera[i].ExamSeq == EXAM_SEQUENCE.NONE)
                                {// NONE
                                    Logger.WriteLog(LogType.INFO, " [NONE]");

                                    _Camera[i].ExamSeq = EXAM_SEQUENCE.WAITING;
                                }
                                else if (_Camera[i].ExamSeq == EXAM_SEQUENCE.WAITING)
                                {// 待機中
                                    Logger.WriteLog(LogType.INFO, " [WAITING]");

                                    if (status.ItemInputReq)
                                    {// 検査項目設定要求 : ON

                                        // 選択中の機種情報インデックス 読み出し
                                        _Camera[i].currrentWorkIdIndex = status.WorkIdIndex;
                                        _Camera[i].currrentAngleIndex = status.AngleIndex;
                                        // 検査項目設定完了 書込み
                                        rc = _PLC.SetItemComp(true);

                                        if (STATUS_SUCCESS(rc))
                                            _Camera[i].ExamSeq = EXAM_SEQUENCE.ITEM_INPUT_COMP;
                                    }
                                }
                                else if (_Camera[i].ExamSeq == EXAM_SEQUENCE.ITEM_INPUT_COMP)
                                {// 検査項目設定完了
                                    Logger.WriteLog(LogType.INFO, " [ITEM_INPUT_COMP]");

                                    if (!status.ItemInputReq)
                                    {// 検査項目設定要求 : OFF
                                        _Camera[i].ExamSeq = EXAM_SEQUENCE.EXAM_WAITING;
                                    }
                                }
                                else if (_Camera[i].ExamSeq == EXAM_SEQUENCE.EXAM_WAITING)
                                {// 検査開始待ち
                                    Logger.WriteLog(LogType.INFO, " [EXAM_WAITING]");

                                    if (status.OutputResultReq)
                                    {// 検査項目設定要求 : ON

                                        _Camera[i].ExamSeq = EXAM_SEQUENCE.EXAM_WORK_ID_SEND;
                                    }
                                }
                                else if (_Camera[i].ExamSeq == EXAM_SEQUENCE.EXAM_WORK_ID_SEND)
                                {// 作業ID開始要求 送信
                                    Logger.WriteLog(LogType.INFO, " [EXAM_WORK_ID_SEND]");

                                    // 作業ID開始要求 書込み
                                    rc = _Camera[i].Send_WorkIdStart(_workInfoList[_Camera[i].currrentWorkIdIndex].workIdName);
                                    if (STATUS_SUCCESS(rc))
                                        _Camera[i].ExamSeq = EXAM_SEQUENCE.EXAM_WORK_ID_RECV;
                                }
                                else if (_Camera[i].ExamSeq == EXAM_SEQUENCE.EXAM_START_SEND)
                                {// START要求 送信
                                    Logger.WriteLog(LogType.INFO, " [EXAM_START_SEND]");

                                    // START要求 書込み
                                    //if(i == 0)
                                    //    Thread.Sleep(500);
                                    Thread.Sleep(500);

                                    Logger.WriteLog(LogType.SYSTEM, $"作業IDインデックス : {status.WorkIdIndex}   角度インデックス : {status.AngleIndex}");
                                    rc = _Camera[i].Send_Start(_workInfoList[status.WorkIdIndex].workIdName,
                                                               _workInfoList[status.WorkIdIndex].AngleList[status.AngleIndex].camera[i].workOrderName,
                                                               "1");
                                    if (STATUS_SUCCESS(rc))
                                        _Camera[i].ExamSeq = EXAM_SEQUENCE.EXAM_START_RECV;
                                }
                                else if (_Camera[i].ExamSeq == EXAM_SEQUENCE.RESULT_OUTPUT)
                                {// 検査結果出力
                                    Logger.WriteLog(LogType.INFO, " [RESULT_OUTPUT]");

                                    // 検査結果 書込み
                                    rc = _PLC.SetResult(i, _Camera[i].result);

                                    // 検査結果出力完了 書込み
                                    if (STATUS_SUCCESS(rc))
                                        rc = _PLC.SetOutputResultComp(i, true);

                                    if (STATUS_SUCCESS(rc))
                                        if (!status.OutputResultReq)
                                            _Camera[i].ExamSeq = EXAM_SEQUENCE.WAITING;
                                }
                                //else if (_Camera[i].ExamSeq == EXAM_SEQUENCE.ERROR) 
                                //{// エラー

                                //    // 検査結果 書込み
                                //    rc = _PLC.SetErrorCode(i);

                                //}
                            }

                        }



                        // -----------------------------------------
                        // 機種情報取得要求
                        // -----------------------------------------
                        if (!status.UpdateReq)
                        {// 機種情報取得要求 : OFF

                            for (int i = 0; i < IniFile.CameraCount; i++)
                                _Camera[i].WorkInfoSeq = WORK_INFO_SEQUENCE.NONE;
                        }
                        else
                        {// 機種情報取得要求 : ON

                            if (!_Camera.All(x => x.WorkInfoSeq == WORK_INFO_SEQUENCE.WORK_ITEM_OUTPUT))
                            {// 全カメラの作業Itemリスト 未取得
                                for (int i = 0; i < IniFile.CameraCount; i++)
                                {
                                    if (_Camera[i].WorkInfoSeq == WORK_INFO_SEQUENCE.NONE)
                                    {
                                        _Camera[i].WorkInfoSeq = WORK_INFO_SEQUENCE.WORK_ITEM_SEND;
                                    }
                                    else if (_Camera[i].WorkInfoSeq == WORK_INFO_SEQUENCE.WORK_ITEM_SEND)
                                    {
                                        // 作業Itemリスト取得要求
                                        rc = _Camera[i].Send_WorkItemListGet();

                                        if (STATUS_SUCCESS(rc))
                                            _Camera[i].WorkInfoSeq = WORK_INFO_SEQUENCE.WORK_ITEM_RECV;
                                    }
                                }
                            }
                            else 
                            {// 全カメラの作業Itemリスト 取得済み

                                // 作業Itemリスト => PLC向けの作業情報リストに変換
                                rc = CreateWorkInfo();
                                for (int i = 0; i < IniFile.MaxWorkIdCount; i++)
                                {
                                    // PLCへ書込み
                                    rc = _PLC.SetWorkInfo(i, _workInfoList[i]);
                                }

                                // 機種情報取得要求 完了
                                rc = _PLC.SetUpdateComp(true);

                                if (STATUS_SUCCESS(rc))
                                    Logger.WriteLog(LogType.INFO, "機種情報取得 完了");
                            }
                        }



                        if (!preCancelReq && status.CancelReq)
                        {// 機種情報取得要求 : OFF

                            for (int i = 0; i < IniFile.CameraCount; i++) 
                            {
                                // 作業Itemリスト取得要求
                                rc = _Camera[i].Send_Stop();
                            }
                        }
                        if (status.CancelReq)
                            preCancelReq = true;
                        else
                            preCancelReq = false;




                        // -----------------------------------------
                        // エラーコード書込み
                        // -----------------------------------------
                        for (int i = 0; i < IniFile.CameraCount; i++)
                        {
                            if (_Camera[i].ErrorType == ERROR_TYPE.EXAM_START_ERROR)
                            {// 検査 START要求エラー
                                _PLC.SetErrorCode(1);
                                _Camera[i].ErrorType = ERROR_TYPE.NONE;
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


        /// <summary>
        /// 作業Itemリストを作業情報に変換
        /// </summary>
        /// <returns></returns>
        public UInt32 CreateWorkInfo()
        {
            UInt32 rc = 0;
            try
            {
                if (_Camera.All(x => x.workIdList.Count <= 0))
                {
                    Logger.WriteLog(LogType.ERROR, "作業IDがありません");
                    rc = (UInt32)ErrorCodeList.EXCEPTION;
                    return rc;
                }


                for (int id = 0; id < IniFile.MaxWorkIdCount; id++)
                {


                    if (id >= 2)
                        break;




                    // ----------------------------------------------
                    // 作業ID
                    string name = _Camera[0].workIdList[id].name;
                    for (int i = 0; i < IniFile.CameraCount; i++)
                    {
                        if (name != _Camera[i].workIdList[id].name) 
                        {
                            Logger.WriteLog(LogType.ERROR, "作業ID名 不一致");
                            rc = (UInt32)ErrorCodeList.EXCEPTION;
                            return rc;
                        }
                    }
                    _workInfoList[id].workIdName = name;
                    //WorkID id02 = _Camera[1].workIdList[id];
                    //WorkID id01 = _Camera[0].workIdList[id];
                    //if (id01.name == id02.name)
                    //{
                    //    _workInfoList[id].workIdName = id01.name;
                    //}


                    // まず全ての作業指示リストを1つのリストにまとめる
                    List<workOrderAlignment> list = new List<workOrderAlignment>();
                    for (int i = 0; i < IniFile.CameraCount; i++)
                    {
                        for (int order = 0; order < IniFile.MaxWorkOrderCount; order++)
                        {
                            if (_Camera[i].workIdList[id].workOrderList.Count <= order)
                                break;

                            string[] s = _Camera[i].workIdList[id].workOrderList[order].name.Split('-');
                            int angle = int.Parse(s[0]);
                            string partName = s[1];
                            workOrderAlignment w = new workOrderAlignment
                            {
                                angle = angle,
                                name = partName,
                                fullName = _Camera[i].workIdList[id].workOrderList[order].name,
                                cameraIndex = i,
                            };
                            list.Add(w);
                        }
                    }
                    list.Sort((x, y) => x.angle.CompareTo(y.angle));

                    int angleIndex = 0;
                    while (true) 
                    {
                        if (list.Count <= 0)
                            break;

                        _workInfoList[id].AngleList[angleIndex].angle = list[0].angle;
                        _workInfoList[id].AngleList[angleIndex].camera[list[0].cameraIndex].partName = list[0].name;
                        _workInfoList[id].AngleList[angleIndex].camera[list[0].cameraIndex].workOrderName = list[0].fullName;

                        var w = list.Where(x => x.angle == list[0].angle && x.cameraIndex != list[0].cameraIndex).FirstOrDefault();
                        if (w != null) 
                        {
                            _workInfoList[id].AngleList[angleIndex].angle = w.angle;
                            _workInfoList[id].AngleList[angleIndex].camera[w.cameraIndex].partName = w.name;
                            _workInfoList[id].AngleList[angleIndex].camera[w.cameraIndex].workOrderName = w.fullName;
                            list.Remove(w);
                        }
                        list.RemoveAt(0);

                        angleIndex++;
                    }

                }

            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }

        private class workOrderAlignment 
        {
            public int angle = -1;
            public string name = "";
            public string fullName = "";
            public int cameraIndex = -1;
        }




        /// <summary>
        /// Check Error State
        /// </summary>
        private static bool STATUS_SUCCESS(UInt32 err) { return err == (int)ErrorCodeList.STATUS_SUCCESS; }

    }
}
