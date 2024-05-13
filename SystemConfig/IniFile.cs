using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DL_CommonLibrary;
using ErrorCodeDefine;


namespace SystemConfig
{

    /// <summary>
    /// INIファイル クラス
    /// </summary>
    public static class IniFile
    {
        /// <summary>
        /// 自クラス名
        /// </summary>
        private const string THIS_NAME = "IniFile";

        /// <summary>
        /// this file path
        /// </summary>
        private static string _filePath = "";


        // --------------------------------------
        // SYSTEM
        // --------------------------------------
        /// <summary>
        /// バージョン
        /// </summary>
        public static int Version = 1;
        /// <summary>
        /// メンテナンスモード
        /// </summary>
        public static bool MaintenanceMode = false;
        /// <summary>
        /// PLCシミュレーターモード
        /// </summary>
        public static bool PlcSimulatorMode = false;

        // --------------------------------------
        // PATH
        // --------------------------------------
        #region "PATH"
        /// <summary>
        /// システムログフォルダ
        /// </summary>
        public static string LogDir = ".\\Log";
        /// <summary>
        /// メッセージフォルダ
        /// </summary>
        public static string MessageDir = ".\\Message";
        #endregion

        // --------------------------------------
        // PLC
        // --------------------------------------
        /// <summary>
        /// PLC IPアドレス
        /// </summary>
        public static string PlcIpAddress = "127.0.0.1";
        /// <summary>
        /// PLC ポート番号
        /// </summary>
        public static int PlcPort = 5000;
        /// <summary>
        /// PLC タイムアウト
        /// </summary>
        public static int PlcTimeout = 5000;
        /// <summary>
        /// PLC 接続文字列
        /// </summary>
        public static string PlcConnectionString = "";

        // --------------------------------------
        // SC-10A カメラ
        // --------------------------------------
        /// <summary>
        /// カメラ台数
        /// </summary>
        public static int CameraCount = 2;
        /// <summary>
        /// 作業ID最大数
        /// </summary>
        public static int MaxWorkIdCount = 100;
        /// <summary>
        /// 作業指示リスト最大数
        /// </summary>
        public static int MaxWorkOrderCount = 30;

        /// <summary>
        /// PC IPアドレス
        /// </summary>
        public static string PCIpAddress = "127.0.0.1";
        /// <summary>
        /// PC ポート番号
        /// </summary>
        public static int[] PCPort = null;
        /// <summary>
        /// カメラ IPアドレス
        /// </summary>
        public static string[] CameraIpAddress = null;

        /// <summary>
        /// カメラ通信ログ最大件数
        /// </summary>
        public static int MaxCameraLogCount = 100;



        /// <summary>
        /// Load File
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static UInt32 Load(string fileName)
        {
            UInt32 rc = 0;
            try
            {

                // Load From File
                string section = "";
                bool exist = false;
                string sBuf = "";
                int iTemp = 0;
                string key = "";
                // Get Full Path.
                fileName = System.IO.Path.GetFullPath(fileName);

                if (!FileIo.ExistFile(fileName)) rc = (UInt32)ErrorCodeList.FILE_NOT_FOUND;

                if (STATUS_SUCCESS(rc))
                {
                    _filePath = fileName;
                    // --------------------------
                    // [SYSTEM]
                    // --------------------------
                    section = "SYSTEM";
                    exist = FileIo.ReadIniFile<int>(_filePath, section, "VERSION", ref Version);
                    exist = FileIo.ReadIniFile<bool>(_filePath, section, "MAINTENANCE_MODE", ref MaintenanceMode);
                    exist = FileIo.ReadIniFile<bool>(_filePath, section, "PLC_SIMULATOR_MODE", ref PlcSimulatorMode);

                    // --------------------------
                    // [PATH]
                    // --------------------------
                    section = "PATH";
                    sBuf = "";

                    exist = FileIo.ReadIniFile<string>(_filePath, section, "LOG_DIR", ref sBuf);
                    if (sBuf != "") LogDir = sBuf; // System.IO.Path.GetFullPath(sBuf);
                                                   //if (!exist) FileIo.WriteIniValue(_filePath, section, "SYSTEM_LOG_DIR", systemLogDir);

                    //exist = FileIo.ReadIniFile<string>(_filePath, section, "SYSTEM_LOG_DIR", ref sBuf);
                    //if (sBuf != "") systemLogDir = sBuf; // System.IO.Path.GetFullPath(sBuf);
                    ////if (!exist) FileIo.WriteIniValue(_filePath, section, "SYSTEM_LOG_DIR", systemLogDir);

                    //sBuf = "";
                    //exist = FileIo.ReadIniFile<string>(_filePath, section, "ALARM_LOG_DIR", ref sBuf);
                    //if (sBuf != "") alarmLogDir = sBuf; //System.IO.Path.GetFullPath(sBuf);
                    ////if (!exist) FileIo.WriteIniValue(_filePath, section, "ALARM_LOG_DIR", alarmLogDir);

                    //sBuf = "";
                    //exist = FileIo.ReadIniFile<string>(_filePath, section, "HISTORY_LOG_DIR", ref sBuf);
                    //if (sBuf != "") historyLogDir = sBuf; //System.IO.Path.GetFullPath(sBuf);
                    ////if (!exist) FileIo.WriteIniValue(_filePath, section, "HISTORY_LOG_DIR", historyLogDir);

                    //sBuf = "";
                    //exist = FileIo.ReadIniFile<string>(_filePath, section, "OPERATION_LOG_DIR", ref sBuf);
                    //if (sBuf != "") operationLogDir = sBuf; //System.IO.Path.GetFullPath(sBuf);
                    ////if (!exist) FileIo.WriteIniValue(_filePath, section, "OPERATION_LOG_DIR", operationLogDir);
                    //sBuf = "";


                    // フォルダ作成
                    if (LogDir != "") System.IO.Directory.CreateDirectory(LogDir);
                    //if (systemLogDir != "") System.IO.Directory.CreateDirectory(systemLogDir);
                    //if (alarmLogDir != "") System.IO.Directory.CreateDirectory(alarmLogDir);
                    //if (historyLogDir != "") System.IO.Directory.CreateDirectory(historyLogDir);
                    //if (operationLogDir != "") System.IO.Directory.CreateDirectory(operationLogDir);


                    // --------------------------
                    // [PLC]
                    // --------------------------
                    section = "PLC";
                    sBuf = "";
                    exist = FileIo.ReadIniFile<string>(_filePath, section, "PLC_IP_ADDRESS", ref PlcIpAddress);
                    exist = FileIo.ReadIniFile<int>(_filePath, section, "PLC_PORT", ref PlcPort);
                    exist = FileIo.ReadIniFile<int>(_filePath, section, "PLC_TIMEOUT", ref PlcTimeout);
                    exist = FileIo.ReadIniFile<string>(_filePath, section, "PLC_CONNECTION", new string[1] { "//" }, ref sBuf);
                    PlcConnectionString = string.Format(sBuf, PlcIpAddress, PlcPort);

                    // --------------------------
                    // [Camera]
                    // --------------------------
                    section = "Camera";
                    sBuf = "";
                    exist = FileIo.ReadIniFile<int>(_filePath, section, "CAMERA_COUNT", ref CameraCount);
                    exist = FileIo.ReadIniFile<int>(_filePath, section, "WORK_ID_MAX_COUNT", ref MaxWorkIdCount);
                    exist = FileIo.ReadIniFile<int>(_filePath, section, "WORK_ORDER_MAX_COUNT", ref MaxWorkOrderCount);
                    exist = FileIo.ReadIniFile<string>(_filePath, section, "PC_IP_ADDRESS", ref PCIpAddress);
                    PCPort = new int[IniFile.CameraCount];
                    for (int i = 0; i < IniFile.CameraCount; i++)
                        exist = FileIo.ReadIniFile<int>(_filePath, section, $"PC_PORT[{i}]", ref PCPort[i]);
                    CameraIpAddress = new string[IniFile.CameraCount];
                    for (int i = 0; i < IniFile.CameraCount; i++)
                        exist = FileIo.ReadIniFile<string>(_filePath, section, $"CAMERA_IP_ADDRESS[{i}]", ref CameraIpAddress[i]);

                    exist = FileIo.ReadIniFile<int>(_filePath, section, "MAX_CAMERA_LOG_COUNT", ref MaxCameraLogCount);

                }
            }
            catch (Exception ex)
            {
                //Resource.ErrorHandler(ex, true);
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }

        /// <summary>
        /// Save File
        /// </summary>
        /// <returns></returns>
        public static UInt32 Save()
        {
            UInt32 rc = 0;
            try
            {

                // Load From File
                string section = "";
                string key = "";
                string sBuf = "";
                if (STATUS_SUCCESS(rc))
                {
                    //// --------------------------
                    //// [PLC]
                    //// --------------------------
                    //section = "PLC";
                    //for (int i = 0; i < Const.MaxAisleCount; i++)
                    //{
                    //    if (IniFile.AisleEnable[i])
                    //        sBuf += "1,";
                    //    else
                    //        sBuf += "0,";
                    //}
                    //sBuf = sBuf.Remove(7, 1);
                    //FileIo.WriteIniValue(_filePath, section, "AISLE_ENABLE", sBuf);

                    //for (int i = 0; i < Const.MaxAisleCount; i++)
                    //{
                    //    sBuf = "";
                    //    for (int j = 0; j < Const.MaxUnitCount; j++)
                    //    {
                    //        if (IniFile.UnitEnable[i][j])
                    //            sBuf += "1,";
                    //        else
                    //            sBuf += "0,";
                    //    }
                    //    sBuf = sBuf.Remove(5, 1);
                    //    FileIo.WriteIniValue(_filePath, section, string.Format("UNIT_ENABLE[{0}]", i), sBuf);
                    //}


                    //// --------------------------
                    //// [SERVER]
                    //// --------------------------
                    //section = "SERVER";
                    //FileIo.WriteIniValue(_filePath, section, "IP_ADDRESS", DBIpAddress);
                    //FileIo.WriteIniValue(_filePath, section, "DB_PORT_NO", DBPortNo.ToString());

                }
            }
            catch
            {
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }



        /// <summary>
        /// Check Error State
        /// </summary>
        /// <param name="err"></param>
        /// <returns></returns>
        private static bool STATUS_SUCCESS(UInt32 err)
        {
            return err == (int)ErrorCodeList.STATUS_SUCCESS;
        }

    }



}
