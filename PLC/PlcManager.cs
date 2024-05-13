using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using DL_Logger;
using DL_PlcInterfce;
using SystemConfig;
using ErrorCodeDefine;


namespace PLC
{
    public class PlcManager
    {
        /// <summary>
        /// 自クラス名
        /// </summary>
        private const string THIS_NAME = "PlcManager";

        /// <summary>
        /// PLC IF オブジェクト
        /// </summary>
        private PLC_IF _plcIf = new PLC_IF();
        /// <summary>
        /// 接続文字列
        /// </summary>
        private string _connectionString = "";


        #region DM アドレス
        /// <summary> 
        /// PLC読み出し デバイス種別
        /// </summary>
        private DeviceType _deviceType_PlcRead = DeviceType.DataMemory;
        /// <summary> 
        /// PLC読み出し 先頭アドレス 
        /// </summary>
        private int _startAddr_PlcRead = 0;
        /// <summary>
        /// PLC読み出し メモリ割り当て
        /// </summary>
        private enum PLC_READ_ADDR
        {
            /// <summary> 検査要求 </summary>
            EXAM_REQ = 10,
            /// <summary> 検査中断要求 </summary>
            CANCEL_REQ = 11,

            /// <summary> 機種情報取得要求 </summary>
            UPDATE_REQ = 15,

            /// <summary> 検査項目設定要求 </summary>
            ITEM_INPUT_REQ = 20,
            /// <summary> 選択中の作業IDインデックス </summary>
            WORK_ID_INDEX = 21,
            /// <summary> 選択中の角度インデックス </summary>
            ANGLE_INDEX = 22,

            /// <summary> 検査結果出力要求 (1角度分) </summary>
            OUTPUT_RESULT_REQ = 30,
        }
        /// <summary> データ長 操作情報 </summary>
        private int _dataSize_PlcRead = 50;

        /// <summary> 
        /// PLC書込み デバイス種別
        /// </summary>
        private DeviceType _deviceType_PlcWrite = DeviceType.DataMemory;
        /// <summary> 
        /// PLC書込み 先頭アドレス 
        /// </summary>
        private int _startAddr_PlcWrite = 50;
        /// <summary>
        /// PLC書込み メモリ割り当て
        /// </summary>
        private enum PLC_WRITE_ADDR
        {
            /// <summary> エラーコード </summary>
            ERROR_CODE = 0,
            /// <summary> カメラ通信初期化完了 </summary>
            INIT_COMP = 5,
            /// <summary> 機種情報取得完了 </summary>
            UPDATE_COMP = 15,
            /// <summary> 検査項目設定完了 </summary>
            ITEM_INPUT_COMP = 20,
            /// <summary> 検査結果出力完了 (1角度分) </summary>
            OUTPUT_RESULT_COMP = 30,
            /// <summary> 検査結果 0:NG 1:OK </summary>
            RESULT = 32,
        }
        /// <summary> データ長 操作情報 </summary>
        private int _dataSize_PlcWrite = 50;
        #endregion

        #region ZR アドレス
        /// <summary> 
        /// 作業情報 デバイス種別
        /// </summary>
        private DeviceType _deviceType_WorkInfo = DeviceType.FileRegister;
        /// <summary> 
        /// 作業情報 先頭アドレス 
        /// </summary>
        private int _startAddr_WorkInfo = 0;
        /// <summary>
        /// 作業情報 メモリ割り当て
        /// </summary>
        private enum PLC_ADDR_WORK_INFO
        {
            /// <summary> 作業ID </summary>
            WORK_ID = 0,
            /// <summary> 角度 </summary>
            ANGLE = 20,
            /// <summary> 作業指示リスト </summary>
            WORK_ORDER = 100,
        }
        /// <summary> データ長 作業情報(1ワーク分) </summary>
        private int _dataSize_WorkInfo_Total = 800;
        /// <summary> データ長 作業ID </summary>
        private int _dataSize_WorkId = 15;
        /// <summary> データ長 角度</summary>
        private int _dataSize_Angle = 2;
        /// <summary> データ長 作業指示リスト </summary>
        private int _dataSize_WorkOrder = 10;
        #endregion



        #region 接続/切断
        /// <summary>
        /// 接続
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public UInt32 Open(string connectionString)
        {
            UInt32 rc = 0;
            try
            {
                rc = _plcIf.Open(connectionString);
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }
        /// <summary>
        /// 切断
        /// </summary>
        /// <returns></returns>
        public UInt32 Close()
        {
            UInt32 rc = 0;
            try
            {
                _plcIf.Close();
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }
        #endregion


        #region 読み出し
        /// <summary>
        /// PLC 読み出し
        /// </summary>
        /// <returns></returns>
        public UInt32 GetStatus(out In item)
        {
            UInt32 rc = 0;
            int addr = 0;
            int count = 0;
            item = new In();
            try
            {
                addr = _startAddr_PlcRead;
                count = _dataSize_PlcRead;
                int[] array = new int[count];
                rc = _plcIf.Read(_deviceType_PlcRead, (uint)addr, count, ref array);

                // 検査要求
                item.ExamReq = (array[(int)PLC_READ_ADDR.EXAM_REQ] == 1);
                // 検査中断要求
                item.CancelReq = (array[(int)PLC_READ_ADDR.CANCEL_REQ] == 1);
                // 機種情報取得要求
                item.UpdateReq = (array[(int)PLC_READ_ADDR.UPDATE_REQ] == 1);

                // 項目設定完了
                item.ItemInputReq = (array[(int)PLC_READ_ADDR.ITEM_INPUT_REQ] == 1);
                // 作業IDインデックス
                item.WorkIdIndex = array[(int)PLC_READ_ADDR.WORK_ID_INDEX];
                // 角度インデックス
                item.AngleIndex = array[(int)PLC_READ_ADDR.ANGLE_INDEX];

                // 検査結果出力完了
                item.OutputResultReq = (array[(int)PLC_READ_ADDR.OUTPUT_RESULT_REQ] == 1);

            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            finally
            {
                if (!STATUS_SUCCESS(rc))
                    Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name} 例外処理 : {(ErrorCodeList)rc}");
            }
            return rc;
        }
        #endregion


        #region 書込み
        /// <summary>
        /// エラーコード 書込み
        /// </summary>
        /// <returns></returns>
        public UInt32 SetErrorCode(int val)
        {
            UInt32 rc = 0;
            int count = 0;
            int addr = 0;
            try
            {
                addr = _startAddr_PlcWrite + (int)PLC_WRITE_ADDR.ERROR_CODE;
                rc = _plcIf.Write(_deviceType_PlcWrite, (uint)addr, val);
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }
        /// <summary>
        /// カメラ通信初期化完了 書込み
        /// </summary>
        /// <returns></returns>
        public UInt32 SetInitComp(bool onOff)
        {
            UInt32 rc = 0;
            int count = 0;
            int addr = 0;
            try
            {
                int val = 0;
                if (onOff) val = 1;

                addr = _startAddr_PlcWrite + (int)PLC_WRITE_ADDR.INIT_COMP;
                rc = _plcIf.Write(_deviceType_PlcWrite, (uint)addr, val);
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }
        /// <summary>
        /// 検査完了 書込み
        /// </summary>
        /// <returns></returns>
        public UInt32 SetExamComp(bool onOff)
        {
            UInt32 rc = 0;
            int count = 0;
            int addr = 0;
            try
            {
                int val = 0;
                if (onOff) val = 1;

                //addr = _startAddr_PlcWrite + (int)PLC_WRITE_ADDR.EXAM_COMP;
                //rc = _plcIf.Write(_deviceType_PlcWrite, (uint)addr, val);
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }
        /// <summary>
        /// 機種情報取得完了 書込み
        /// </summary>
        /// <returns></returns>
        public UInt32 SetUpdateComp(bool onOff)
        {
            UInt32 rc = 0;
            int count = 0;
            int addr = 0;
            try
            {
                int val = 0;
                if (onOff) val = 1;

                addr = _startAddr_PlcWrite + (int)PLC_WRITE_ADDR.UPDATE_COMP;
                rc = _plcIf.Write(_deviceType_PlcWrite, (uint)addr, val);
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }
        /// <summary>
        /// 項目設定完了 書込み
        /// </summary>
        /// <returns></returns>
        public UInt32 SetItemComp(bool onOff)
        {
            UInt32 rc = 0;
            int count = 0;
            int addr = 0;
            try
            {
                int val = 0;
                if (onOff) val = 1;

                addr = _startAddr_PlcWrite + (int)PLC_WRITE_ADDR.ITEM_INPUT_COMP;
                rc = _plcIf.Write(_deviceType_PlcWrite, (uint)addr, val);
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }
        /// <summary>
        /// 検査結果出力完了 書込み
        /// </summary>
        /// <returns></returns>
        public UInt32 SetOutputResultComp(int index, bool onOff)
        {
            UInt32 rc = 0;
            int count = 0;
            int addr = 0;
            try
            {
                int val = 0;
                if (onOff) val = 1;

                addr = _startAddr_PlcWrite + (int)PLC_WRITE_ADDR.OUTPUT_RESULT_COMP + index;
                rc = _plcIf.Write(_deviceType_PlcWrite, (uint)addr, val);
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }
        /// <summary>
        /// 検査結果 書込み
        /// </summary>
        /// <returns></returns>
        public UInt32 SetResult(int index, bool okNg)
        {
            UInt32 rc = 0;
            int count = 0;
            int addr = 0;
            try
            {
                int val = 0;
                if (okNg) val = 1;

                addr = _startAddr_PlcWrite + (int)PLC_WRITE_ADDR.RESULT + index;
                rc = _plcIf.Write(_deviceType_PlcWrite, (uint)addr, val);
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }

        /// <summary>
        /// 機種情報 書込み
        /// (作業ID 1個分)
        /// </summary>
        /// <param name="workIndex">1~10のどこか</param>
        /// <returns></returns>
        public UInt32 SetWorkInfo(int workIdIndex, WorkInfo workInfo)
        {
            UInt32 rc = 0;
            int count = 0;
            int addr = 0;
            try
            {
                // 作業ID
                addr = _startAddr_WorkInfo + (_dataSize_WorkInfo_Total * workIdIndex) + (int)PLC_ADDR_WORK_INFO.WORK_ID;
                count = _dataSize_WorkId;
                rc = _plcIf.Write(_deviceType_WorkInfo, (uint)addr, workInfo.workIdName);

                // 角度　30個
                addr = _startAddr_WorkInfo + (_dataSize_WorkInfo_Total * workIdIndex) + (int)PLC_ADDR_WORK_INFO.ANGLE;
                for (int i = 0; i < IniFile.MaxWorkOrderCount; i++)
                {
                    rc = _plcIf.Write(_deviceType_WorkInfo, (uint)addr, workInfo.AngleList[i].angle.ToString());
                    addr += _dataSize_Angle;
                }

                // 作業指示リスト(部品名) 30個 * カメラ2台
                addr = _startAddr_WorkInfo + (_dataSize_WorkInfo_Total * workIdIndex) + (int)PLC_ADDR_WORK_INFO.WORK_ORDER;
                for (int i = 0; i < IniFile.CameraCount; i++)
                {
                    for (int j = 0; j < IniFile.MaxWorkOrderCount; j++)
                    {
                        rc = _plcIf.Write(_deviceType_WorkInfo, (uint)addr, workInfo.AngleList[j].camera[i].partName);
                        addr += _dataSize_WorkOrder;
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            finally
            {
                if (!STATUS_SUCCESS(rc))
                    Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name} 例外処理 : {(ErrorCodeList)rc}");
            }
            return rc;
        }
        #endregion




        #region デバッグ
        /// <summary>
        /// デバッグ
        /// PLC 読み出し
        /// </summary>
        /// <returns></returns>
        public UInt32 Debug_GetStatus(out Out item)
        {
            UInt32 rc = 0;
            int addr = 0;
            int count = 0;
            item = new Out();
            try
            {
                addr = _startAddr_PlcWrite;
                count = _dataSize_PlcWrite;
                int[] array = new int[count];
                rc = _plcIf.Read(_deviceType_PlcWrite, (uint)addr, count, ref array);

                // カメラ通信初期化完了
                item.ErrorCode = array[(int)PLC_WRITE_ADDR.ERROR_CODE];
                // カメラ通信初期化完了
                item.InitComp = (array[(int)PLC_WRITE_ADDR.INIT_COMP] == 1);
                //// 検査完了
                //item.StartComp = (array[(int)PLC_WRITE_ADDR.EXAM_COMP] == 1);
                // 機種情報取得完了
                item.UpdateComp = (array[(int)PLC_WRITE_ADDR.UPDATE_COMP] == 1);

                // 項目設定完了
                item.ItemInputComp = (array[(int)PLC_WRITE_ADDR.ITEM_INPUT_COMP] == 1);

                for (int i = 0; i < IniFile.CameraCount; i++)
                {
                    // 検査結果出力要求
                    item.OutputResultComp[i] = (array[(int)(PLC_WRITE_ADDR.OUTPUT_RESULT_COMP + i)] == 1);
                    // 検査結果
                    item.Result[i] = (array[(int)(PLC_WRITE_ADDR.RESULT + i)] == 1);
                }

            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            finally
            {
                if (!STATUS_SUCCESS(rc))
                    Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name} 例外処理 : {(ErrorCodeList)rc}");
            }
            return rc;
        }

        /// <summary>
        /// デバッグ
        /// 検査要求 書込み
        /// </summary>
        /// <returns></returns>
        public UInt32 Debug_SetExamReq(bool onOff)
        {
            UInt32 rc = 0;
            int addr = 0;
            try
            {
                int val = 0;
                if (onOff) val = 1;

                addr = _startAddr_PlcRead + (int)PLC_READ_ADDR.EXAM_REQ;
                rc = _plcIf.Write(_deviceType_PlcRead, (uint)addr, val);
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }
        /// <summary>
        /// デバッグ
        /// 検査中断要求 書込み
        /// </summary>
        /// <returns></returns>
        public UInt32 Debug_SetCencelReq(bool onOff)
        {
            UInt32 rc = 0;
            int addr = 0;
            try
            {
                int val = 0;
                if (onOff) val = 1;

                addr = _startAddr_PlcRead + (int)PLC_READ_ADDR.CANCEL_REQ;
                rc = _plcIf.Write(_deviceType_PlcRead, (uint)addr, val);
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }
        /// <summary>
        /// デバッグ
        /// 検査終了要求 書込み
        /// </summary>
        /// <returns></returns>
        public UInt32 Debug_SetCloseReq(bool onOff)
        {
            UInt32 rc = 0;
            int addr = 0;
            try
            {
                int val = 0;
                if (onOff) val = 1;

                //addr = _startAddr_PlcRead + (int)PLC_READ_ADDR.CLOSE_REQ;
                //rc = _plcIf.Write(_deviceType_PlcRead, (uint)addr, val);
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }
        /// <summary>
        /// デバッグ
        /// 機種情報取得要求 書込み
        /// </summary>
        /// <returns></returns>
        public UInt32 Debug_SetUpdateReq(bool onOff)
        {
            UInt32 rc = 0;
            int addr = 0;
            try
            {
                int val = 0;
                if (onOff) val = 1;

                addr = _startAddr_PlcRead + (int)PLC_READ_ADDR.UPDATE_REQ;
                rc = _plcIf.Write(_deviceType_PlcRead, (uint)addr, val);
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }
        ///// <summary>
        ///// 作業Item完了通知応答 書込み (MR)
        ///// </summary>
        ///// <returns></returns>
        //public UInt32 Debug_SetResponse()
        //{
        //    UInt32 rc = 0;
        //    int addr = 0;
        //    try
        //    {
        //        addr = _startAddr_Operation + (int)PLC_ADDR_OPERATION.RESPONSE;
        //        rc = _plcIf.Write(_deviceType_Operation, (uint)addr, 1);
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
        //        rc = (UInt32)ErrorCodeList.EXCEPTION;
        //    }
        //    return rc;
        //}
        
        /// <summary>
        /// デバッグ
        /// 検査項目設定要求 書込み
        /// </summary>
        /// <returns></returns>
        public UInt32 Debug_SetItemReq(bool onOff)
        {
            UInt32 rc = 0;
            int addr = 0;
            try
            {
                int val = 0;
                if (onOff) val = 1;

                addr = _startAddr_PlcRead + (int)PLC_READ_ADDR.ITEM_INPUT_REQ;
                rc = _plcIf.Write(_deviceType_PlcRead, (uint)addr, val);
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }
        /// <summary>
        /// デバッグ
        /// 選択中の作業IDインデックス 書込み
        /// </summary>
        /// <returns></returns>
        public UInt32 Debug_SetSelectedIdIndex(int index)
        {
            UInt32 rc = 0;
            int addr = 0;
            try
            {
                addr = _startAddr_PlcRead + (int)PLC_READ_ADDR.WORK_ID_INDEX;
                rc = _plcIf.Write(_deviceType_PlcRead, (uint)addr, index);
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }
        /// <summary>
        /// デバッグ
        /// 選択中の角度インデックス 書込み
        /// </summary>
        /// <returns></returns>
        public UInt32 Debug_SetSelectedAngleIndex(int index)
        {
            UInt32 rc = 0;
            int addr = 0;
            try
            {
                addr = _startAddr_PlcRead + (int)PLC_READ_ADDR.ANGLE_INDEX;
                rc = _plcIf.Write(_deviceType_PlcRead, (uint)addr, index);
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }
        /// <summary>
        /// デバッグ
        /// 検査結果出力要求 書込み
        /// </summary>
        /// <returns></returns>
        public UInt32 Debug_SetOutPutResultReq(bool onOff)
        {
            UInt32 rc = 0;
            int addr = 0;
            try
            {
                int val = 0;
                if (onOff) val = 1;

                addr = _startAddr_PlcRead + (int)PLC_READ_ADDR.OUTPUT_RESULT_REQ;
                rc = _plcIf.Write(_deviceType_PlcRead, (uint)addr, val);
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }

        ///// <summary>
        ///// 作業Item完了通知応答 書込み (DM)
        ///// </summary>
        ///// <returns></returns>
        //public UInt32 Debug_SetWorkItemResponseIndex(int index)
        //{
        //    UInt32 rc = 0;
        //    int addr = 0;
        //    try
        //    {
        //        addr = _startAddr_SelectedWorkInfo + (int)PLC_ADDR_SELECTED_WORK_INFO.RESPONSE;
        //        rc = _plcIf.Write(_deviceType_SelectedWorkInfo, (uint)addr, index);
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
        //        rc = (UInt32)ErrorCodeList.EXCEPTION;
        //    }
        //    return rc;
        //}

        /// <summary>
        /// 作業情報 読み出し (ZR)
        /// (作業ID 1個分)
        /// </summary>
        /// <param name="workIndex">1~10のどこか</param>
        /// <returns></returns>
        public UInt32 Debug_GetWorkInfo(int workIdIndex, out WorkInfo workInfo)
        {
            UInt32 rc = 0;
            int count = 0;
            int addr = 0;
            string buf = "";
            workInfo = null;
            try
            {
                workInfo = new WorkInfo();


                // 作業ID
                buf = "";
                addr = _startAddr_WorkInfo + (_dataSize_WorkInfo_Total * workIdIndex) + (int)PLC_ADDR_WORK_INFO.WORK_ID;
                count = _dataSize_WorkId;
                rc = _plcIf.Read(_deviceType_WorkInfo, (uint)addr, count, ref buf);
                workInfo.workIdName = buf;

                // 角度　30個
                addr = _startAddr_WorkInfo + (_dataSize_WorkInfo_Total * workIdIndex) + (int)PLC_ADDR_WORK_INFO.ANGLE;
                count = _dataSize_Angle;
                for (int i = 0; i < IniFile.MaxWorkOrderCount; i++)
                {
                    buf = "";
                    rc = _plcIf.Read(_deviceType_WorkInfo, (uint)addr, count, ref buf);
                    if(buf != "")
                        workInfo.AngleList[i].angle = int.Parse(buf);
                    addr += _dataSize_Angle;
                }

                // 作業指示リスト(部品名) 30個 * カメラ2台
                addr = _startAddr_WorkInfo + (_dataSize_WorkInfo_Total * workIdIndex) + (int)PLC_ADDR_WORK_INFO.WORK_ORDER;
                count = _dataSize_WorkOrder;
                for (int i = 0; i < IniFile.CameraCount; i++)
                {
                    for (int j = 0; j < IniFile.MaxWorkOrderCount; j++)
                    {
                        buf = "";
                        rc = _plcIf.Read(_deviceType_WorkInfo, (uint)addr, count, ref buf);
                        addr += _dataSize_WorkOrder;
                        workInfo.AngleList[j].camera[i].partName = buf;
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            finally
            {
                if (!STATUS_SUCCESS(rc))
                    Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name} 例外処理 : {(ErrorCodeList)rc}");
            }
            return rc;
        }
        #endregion


        /// <summary>
        /// Check Error State
        /// </summary>
        private static bool STATUS_SUCCESS(UInt32 err) { return err == (int)ErrorCodeList.STATUS_SUCCESS; }

    }
}
