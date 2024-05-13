//----------------------------------------------------------
// Copyright © 2020 DATALINK
//----------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SystemConfig;


namespace PLC
{
    /// <summary>
    /// PLC用定数
    /// </summary>
    internal static class PlcStatus
    {

        /// <summary>
        /// PLC接続ユニット種別
        /// </summary>
        public static string[] PlcUnitTypes = new string[5] { "Ether接続", "Ether接続(直接)", "シュミレータ", "RS-232C", "USB" };

        /// <summary>
        /// PLCメイン メモリ読込数
        /// </summary>
        public const int PlcReadDeviceCount = 50;

        /// <summary>
        /// PLCメイン メモリ書込数
        /// </summary>
        public const int PlcWriteDeviceCount = 50;

        /// <summary>
        /// モニタスレッド更新待ち時間
        /// </summary>
        public const int PlcMonitorUpdateTime = 3000;

    }


    /// <summary>
    /// PLC -> PC
    /// </summary>
    public class In
    {
        /// <summary>
        /// 検査要求 (1機種分)
        /// </summary>
        public bool ExamReq = false;
        /// <summary>
        /// 検査中断要求
        /// </summary>
        public bool CancelReq = false;
        /// <summary>
        /// 検査終了要求
        /// </summary>
        public bool CloseReq = false;
        /// <summary>
        /// 機種情報取得要求
        /// </summary>
        public bool UpdateReq = false;

        /// <summary>
        /// 検査項目設定要求
        /// </summary>
        public bool ItemInputReq = false;
        /// <summary>
        /// 作業IDインデックス
        /// </summary>
        public int WorkIdIndex = -1;
        /// <summary>
        /// 角度インデックス
        /// </summary>
        public int AngleIndex = -1;

        /// <summary>
        /// 検査結果出力要求 (1角度分)
        /// </summary>
        public bool OutputResultReq = false;
    }
    /// <summary>
    /// PC -> PLC
    /// </summary>
    public class Out
    {
        /// <summary>
        /// エラーコード
        /// </summary>
        public int ErrorCode = 0;
        /// <summary>
        /// カメラ通信初期化完了
        /// </summary>
        public bool InitComp = false;
        /// <summary>
        /// 検査完了
        /// </summary>
        public bool StartComp = false;
        /// <summary>
        /// 機種情報取得完了
        /// </summary>
        public bool UpdateComp = false;

        /// <summary>
        /// 検査項目設定完了
        /// </summary>
        public bool ItemInputComp = false;
        /// <summary>
        /// 検査結果出力完了 (1角度分)
        /// </summary>
        public bool[] OutputResultComp = new bool[IniFile.CameraCount];
        /// <summary>
        /// 検査結果 1:OK 2:NG
        /// </summary>
        public bool[] Result = new bool[IniFile.CameraCount];

    }



    /// <summary>
    /// 作業情報
    /// </summary>
    public class WorkInfo
    {
        /// <summary>
        /// 作業ID名
        /// (機種名)
        /// </summary>
        public string workIdName = "-";
        /// <summary>
        /// 角度リスト
        /// </summary>
        public AngleInfo[] AngleList = new AngleInfo[IniFile.MaxWorkOrderCount];

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public WorkInfo()
        {
            for (int i = 0; i < IniFile.MaxWorkOrderCount; i++)
            {
                AngleList[i] = new AngleInfo();
            }
        }
    }
    /// <summary>
    /// 角度
    /// </summary>
    public class AngleInfo
    {
        /// <summary>
        /// 角度
        /// </summary>
        public int angle = -1;
        /// <summary>
        /// カメラ
        /// </summary>
        public CameraInfo[] camera = new CameraInfo[IniFile.CameraCount];

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AngleInfo()
        {
            for (int i = 0; i < IniFile.CameraCount; i++)
            {
                camera[i] = new CameraInfo();
            }
        }
    }
    /// <summary>
    /// カメラ種別
    /// </summary>
    public class CameraInfo
    {
        /// <summary>
        /// IPアドレス
        /// </summary>
        public string ipAddress = "127.0.0.1";
        /// <summary>
        /// 部品名
        /// </summary>
        public string partName = "-";
        /// <summary>
        /// 作業指示リスト名(内部的に保持)
        /// </summary>
        public string workOrderName = "-";
    }




}
