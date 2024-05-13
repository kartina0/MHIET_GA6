// ----------------------------------------------
// Copyright © 2021 DATALINK
// ----------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using DL_CommonLibrary;
using ErrorCodeDefine;
using DL_Socket;
using ActUtlTypeLib;
using ActProgTypeLib;
namespace DL_PlcInterfce
{
    class Mitsubishi_PLC : IPlc
    {

        #region Private Variable

        /// <summary>
        /// 自クラス名
        /// </summary>
        private const string THIS_NAME = "PLC_IF";

        /// <summary>
        /// PLCレスポンス受信イベント
        /// </summary>
        private ManualResetEvent _respDetect = new ManualResetEvent(false);
        /// <summary>
        /// PLCレスポンスバッファ
        /// </summary>
        private volatile string _response = "";
        /// <summary>
        /// Sokcet Lockオブジェクト
        /// </summary>
        private object _sockLock = new object();

        /// <summary>
        /// 接続パラメータ
        /// </summary>
        private string _connectionParam = "";

        /// <summary>
        /// 接続状態
        /// </summary>
        private bool _connected = false;

        /// <summary>
        /// PLC IPアドレス
        /// </summary>
        private string _ipAddress = "127.0.0.1";
        /// <summary>
        /// PLCﾎﾟｰﾄ番号
        /// </summary>
        private int _port = 8501;

        /// <summary>
        /// 通信ﾀｲﾑｱｳﾄ
        /// </summary>
        private int _timeout = 10000;

        /// <summary>
        /// 毎回接続・切断しないフラグ
        /// </summary>
        private bool _keepConnection = true;

        /// <summary>
        /// PLC
        /// </summary>
        private ActUtlTypeClass _plc = new ActUtlTypeClass();

        #endregion

        /// <summary>
        /// ダミーモードか確認
        /// </summary>
        /// <returns></returns>
        public bool IsDummy()
        {
            return false;
        }

        /// <summary>
        /// 接続
        /// </summary>
        /// <param name="connectionParam">接続パラメータ</param>
        /// <returns></returns>
        public UInt32 Open(string connectionParam)
        {
            UInt32 rc = 0;

            _connectionParam = connectionParam;
            string[] param = connectionParam.Split(';');
            try
            {
                if (_plc == null) _plc = new ActUtlTypeClass();
                int ret = _plc.Open();

                if (ret != 0)
                    rc = (UInt32)ErrorCodeList.PLC_CONNECT_ERROR;

                _connected = STATUS_SUCCESS(rc);
            }
            catch { rc = (UInt32)ErrorCodeList.PLC_INIT_ERROR; }

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
                if (_plc != null)
                {
                    _plc.Close();
                    _plc = null;
                }
                _connected = false;
            }
            catch { rc = (UInt32)ErrorCodeList.EXCEPTION; }

            return rc;
        }

        /// <summary>
        /// 接続確認
        /// </summary>
        /// <returns></returns>
        public bool IsConnected()
        {
            return _connected ;
        }

        /// <summary>
        /// 単一デバイス読み出し
        /// </summary>
        /// <param name="devType">デバイス種類</param>
        /// <param name="addr">デバイス番号</param>
        /// <param name="value">値</param>
        /// <returns></returns>
        public UInt32 Read(DeviceType deviceType, UInt32 addr, ref int value)
        {
            UInt32 rc = 0;
            try
            {
                string typeName = GetDeviceTypeName(deviceType);
                string deviceName = $"{typeName}{addr}";

                int[] buf = new int[1];
                if (!IsConnected()) rc = (UInt32)ErrorCodeList.PLC_CONNECT_ERROR;


                if (STATUS_SUCCESS(rc))
                {
                    int ret = _plc.ReadDeviceBlock(deviceName, buf.Length, out buf[0]);

                    if (ret == 0)
                    {
                        value = (int)buf[0];
                    }
                    else
                    {
                        rc = (UInt32)ErrorCodeList.PLC_READ_ERROR;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }
        /// <summary>
        /// 単一デバイス読み出し
        /// </summary>
        /// <param name="devType">デバイス種類</param>
        /// <param name="addr">デバイス番号</param>
        /// <param name="value">値</param>
        /// <returns></returns>
        public UInt32 Read32(DeviceType deviceType, UInt32 addr, ref Int32 value)
        {
            UInt32 rc = 0;
            try
            {
                string typeName = GetDeviceTypeName(deviceType);
                string deviceName = $"{typeName}{addr}";

                int[] buf = new int[2];

                if (!IsConnected()) rc = (UInt32)ErrorCodeList.PLC_CONNECT_ERROR;

                if (STATUS_SUCCESS(rc))
                {
                    int ret = _plc.ReadDeviceBlock(deviceName, buf.Length, out buf[0]);

                    if (ret == 0)
                    {
                        value = (int)((ushort)buf[0]) | ((ushort)buf[1] << 16);
                    }
                    else
                    {
                        rc = (UInt32)ErrorCodeList.PLC_READ_ERROR;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }

        /// <summary>
        /// 文字読み出し
        /// </summary>
        /// <param name="devType">デバイス種類</param>
        /// <param name="addr">デバイス番号</param>
        /// <param name="len">最大文字数</param>
        /// <param name="buf">文字列</param>
        /// <returns></returns>
        public UInt32 Read(DeviceType deviceType, UInt32 addr, Int32 len, ref string sBuf)
        {
            UInt32 rc = 0;
            try
            {
                string typeName = GetDeviceTypeName(deviceType);
                string deviceName = $"{typeName}{addr}";
                int count = (int)((float)len / 2 + 0.5);
                byte[] tempBuf = new byte[len];


                int[] buf = new int[len];
                if (!IsConnected()) rc = (UInt32)ErrorCodeList.PLC_CONNECT_ERROR;


                if (STATUS_SUCCESS(rc))
                {
                    int ret = _plc.ReadDeviceBlock(deviceName, buf.Length, out buf[0]);

                    int bIndex = 0;
                    for (int i = 0; i < count; i++)
                    {
                        byte b1 = (byte)(buf[i] & 0xFF);
                        byte b2 = (byte)(buf[i] >> 8 & 0xFF);

                        if (b1 == 0) break;
                        tempBuf[bIndex++] = (byte)(buf[i] & 0xFF);
                        if (b2 == 0) break;
                        tempBuf[bIndex++] = (byte)(buf[i] >> 8 & 0xFF);
                    }
                    //sBuf = ASCIIEncoding.ASCII.GetString(tempBuf).Trim();
                    sBuf = Encoding.GetEncoding("Shift-JIS").GetString(tempBuf).Trim();
                    

                    sBuf = FileIo.DeleteAfter(sBuf, "\0");

                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rc;

        }

        /// <summary>
        /// 連続デバイス読み出し(16bit)
        /// </summary>
        /// <param name="devType">デバイス種類</param>
        /// <param name="addr">デバイス番号</param>
        /// <param name="count">個数</param>
        /// <param name="value">値</param>
        /// <returns></returns>
        public UInt32 Read(DeviceType deviceType, UInt32 addr, Int32 count, ref Int32[] value)
        {
            UInt32 rc = 0;
            try
            {
                string typeName = GetDeviceTypeName(deviceType);
                string deviceName = $"{typeName}{addr}";

                int[] buf = new int[count];
                value = new int[count];

                if (!IsConnected()) rc = (UInt32)ErrorCodeList.PLC_CONNECT_ERROR;

                if (STATUS_SUCCESS(rc))
                {
                    int ret = _plc.ReadDeviceBlock(deviceName, buf.Length, out buf[0]);

                    if (ret == 0)
                    {
                        for (int i = 0; i < buf.Length; i++)
                        {
                            value[i] = buf[i];
                        }
                    }
                    else
                    {
                        rc = (UInt32)ErrorCodeList.PLC_READ_ERROR;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }

        /// <summary>
        /// 単一デバイス書込み(16Bit)
        /// </summary>
        /// <param name="devType">デバイス種類</param>
        /// <param name="addr">デバイス番号</param>
        /// <param name="value">値</param>
        /// <returns></returns>
        public UInt32 Write(DeviceType deviceType, UInt32 addr, int value)
        {
            UInt32 rc = 0;
            try
            {
                string typeName = GetDeviceTypeName(deviceType);
                string deviceName = $"{typeName}{addr}";

                int[] buf = new int[1];
                if (!IsConnected()) rc = (UInt32)ErrorCodeList.PLC_CONNECT_ERROR;


                if (STATUS_SUCCESS(rc))
                {
                    buf[0] = value;
                    int ret = _plc.WriteDeviceBlock(deviceName, buf.Length, ref buf[0]);
                    if(ret != 0) rc = (UInt32)ErrorCodeList.PLC_READ_ERROR;
                }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }
        /// <summary>
        /// 単一デバイス書込み
        /// </summary>
        /// <param name="devType">デバイス種類</param>
        /// <param name="addr">デバイス番号</param>
        /// <param name="value">値</param>
        /// <returns></returns>
        public UInt32 Write32(DeviceType deviceType, UInt32 addr, Int32 value)
        {
            UInt32 rc = 0;
            try
            {
                string typeName = GetDeviceTypeName(deviceType);
                string deviceName = $"{typeName}{addr}";

                int[] buf = new int[2];
                if (!IsConnected()) rc = (UInt32)ErrorCodeList.PLC_CONNECT_ERROR;

                if (STATUS_SUCCESS(rc))
                {
                    value = (int)((ushort)buf[0]) | ((ushort)buf[1] << 16);

                    buf[0] = (value & 0xFFFF);
                    buf[1] = (value >> 16 & 0xFFFF);

                    int ret = _plc.WriteDeviceBlock(deviceName, buf.Length, ref buf[0]);
                    if (ret != 0) rc = (UInt32)ErrorCodeList.PLC_READ_ERROR;
                }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }


        /// <summary>
        /// 連続デバイス書込み
        /// </summary>
        /// <param name="devType">デバイス種類</param>
        /// <param name="addr">デバイス番号</param>
        /// <param name="count">個数</param>
        /// <param name="value">値</param>
        /// <returns></returns>
        public UInt32 Write(DeviceType deviceType, UInt32 addr, Int32 count, Int32[] value)
        {
            UInt32 rc = 0;
            try
            {
                string typeName = GetDeviceTypeName(deviceType);
                string deviceName = $"{typeName}{addr}";

                int[] buf = new int[1];
                if (!IsConnected()) rc = (UInt32)ErrorCodeList.PLC_CONNECT_ERROR;

                if (STATUS_SUCCESS(rc))
                {
                    for (int i = 0; i < value.Length; i++)
                        buf[i] = value[i];

                    int ret = _plc.WriteDeviceBlock(deviceName, buf.Length, ref buf[0]);
                    if (ret != 0) rc = (UInt32)ErrorCodeList.PLC_READ_ERROR;
                }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }

        /// <summary>
        /// 文字書込み
        /// </summary>
        /// <param name="devType">デバイス種類</param>
        /// <param name="addr">デバイス番号</param>
        /// <param name="buf">文字列</param>
        /// <returns></returns>
        public UInt32 Write(DeviceType deviceType, UInt32 addr, string sBuf)
        {
            UInt32 rc = 0;
            try
            {
                string typeName = GetDeviceTypeName(deviceType);
                string deviceName = $"{typeName}{addr}";
                //int count = (int)((float)sBuf.Length / 2 + 0.5);


                //byte[] byteBuf = ASCIIEncoding.ASCII.GetBytes(sBuf);
                byte[] byteBuf = Encoding.GetEncoding("Shift-JIS").GetBytes(sBuf);
                int count = (int)((float)byteBuf.Length / 2 + 0.5);


                int[] buf = new int[count + 1]; // 最後に0を入れるため1ワード余分に作成する
 
                if (!IsConnected()) rc = (UInt32)ErrorCodeList.PLC_CONNECT_ERROR;

                if (STATUS_SUCCESS(rc))
                {
                    int bIndex = 0;

                    for (int i = 0; i < count; i++)
                    {
                        if (byteBuf.Length > bIndex)
                            buf[i] = (int)(byteBuf[bIndex++] | buf[i]);
                        if (byteBuf.Length > bIndex)
                            buf[i] = (int)(byteBuf[bIndex++] << 8) | buf[i];
                    }

                    int ret = _plc.WriteDeviceBlock(deviceName, buf.Length, ref buf[0]);
                    if (ret != 0) rc = (UInt32)ErrorCodeList.PLC_READ_ERROR;

                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }

        /// <summary>
        /// デバイスタイプをコマンド用の文字列に変更
        /// </summary>
        /// <param name="deviceType"></param>
        /// <returns></returns>
        private string GetDeviceTypeName(DeviceType deviceType)
        {
            string name = "";
            if (deviceType == DeviceType.DataMemory) name = "D";
            else if (deviceType == DeviceType.ExtendDataMemory) name = "E";
            else if (deviceType == DeviceType.FileRegister) name = "ZR";
            else if (deviceType == DeviceType.InternalRelay) name = "M";
            else if (deviceType == DeviceType.LinkRegister) name = "W";
            else if (deviceType == DeviceType.LinkRelay) name = "B";
            else if (deviceType == DeviceType.Relay) name = "R";
            else if (deviceType == DeviceType.Timer) name = "T";
            else name = "";

            return name;
        }

        /// <summary>
        /// エラー有無確認
        /// </summary>
        /// <param name="rc"></param>
        /// <returns></returns>
        private bool STATUS_SUCCESS(UInt32 rc)
        {
            return rc == 0;
        }
    }
}
