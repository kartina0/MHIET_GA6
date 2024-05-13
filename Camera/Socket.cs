using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using DL_CommonLibrary;
using DL_Socket;
using DL_Logger;
using SystemConfig;


namespace Camera
{
    public class Socket
    {
        /// <summary>
        /// 自分 IPアドレス
        /// </summary>
        private string _myIpAddress = "192.168.0.10";
        /// <summary>
        /// 自分 ポート番号
        /// </summary>
        private int _myPortNo = 33333;
        /// <summary>
        /// 相手 IPアドレス
        /// </summary>
        private string _targetIpAddress = "192.168.0.2";

        /// <summary>
        /// サーバー
        /// </summary>
        private SckServer _server = null;
        /// <summary>
        /// クライアント
        /// </summary>
        private SckClient _client = null;

        public delegate void SocketRecieve(byte[] byteArray);
        /// <summary>
        /// デリゲート
        /// </summary>
        private SocketRecieve _callBack;

        /// <summary>
        /// サーバー受信処理 ロックオブジェクト
        /// </summary>
        private object _serverRecvLock = new object();


        private List<byte[]> _byteItemList = new List<byte[]>();

        /// <summary>
        /// 端末ID
        /// </summary>
        public byte[] id = new byte[4];
        /// <summary>
        /// 端末名
        /// </summary>
        public byte[] name = new byte[64];


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="dele"></param>
        public Socket(SocketRecieve dele) 
        {
            _callBack = dele;
        }


        #region SERVER
        /// <summary>
        /// サーバー オープン
        /// </summary>
        /// <returns></returns>
        public UInt32 Server_Open(string myIpAddress, int myPortNo, string targetIpAddress)
        {
            UInt32 rc = 0;
            int ret = 0;
            try
            {
                Server_Close();
                _myIpAddress = myIpAddress;
                _myPortNo = myPortNo;
                _targetIpAddress = targetIpAddress;

                //_server = new SckServer(_myIpAddress, _myPortNo, 1, "", Server_ReciveCallBack, out ret);
                _server = new SckServer(_targetIpAddress, _myIpAddress, _myPortNo, 1, "", Server_ReciveCallBack, out ret);

                //_sock = new SckClient(IniFile.Reader_IpAddress, IniFile.Reader_PortNo,"\r", ReciveCallBack, out ret);
                if (ret != 0) rc = 1;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                rc = 1;
            }
            return rc;
        }
        /// <summary>
        /// サーバー クローズ
        /// </summary>
        /// <returns></returns>
        public UInt32 Server_Close()
        {
            UInt32 rc = 0;
            try
            {
                if (_server != null)
                    _server.CloseServer();
                _server = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                rc = 1;
            }
            return rc;
        }
        /// <summary>
        /// サーバー 受信コールバック
        /// </summary>
        /// <param name="strData"></param>
        /// <returns></returns>
        private string Server_ReciveCallBack(String pSocketID, Byte[] byData)
        {
            UInt32 rc = 0;
            Thread.Sleep(2);
            lock (_serverRecvLock)
            {
                //Logger.WriteLog(LogType.INFO, $"{_count++}, {byData.Length}");

                try
                {
                    // コールバック
                    _callBack(byData);
                    //if (OnDataRecive != null)
                    //    OnDataRecive(byData);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            return "";
        }
        /// <summary>
        /// 送信
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public UInt32 Server_Send(byte[] msg)
        {
            UInt32 rc = 0;
            try
            {
                rc = (UInt32)_server.Send(msg);

            }
            catch (Exception ex)
            {
                rc = 1;
            }
            return rc;
        }
        #endregion




        #region CLIENT
#if false
        /// <summary>
        /// クライアント オープン
        /// </summary>
        /// <returns></returns>
        private UInt32 Client_Open()
        {
            UInt32 rc = 0;
            int ret = 0;
            try
            {
                Client_Close();

                //_server = new SckServer(_myIpAddress, 56109, 1, "", Server_ReciveCallBack, out ret);
                _client = new SckClient(_targetIpAddress, _targetPortNo, "", Client_ReciveCallBack, out ret);
                if (ret != 0) rc = 1;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                rc = 1;
            }
            return rc;
        }
        /// <summary>
        /// クライアント クローズ
        /// </summary>
        /// <returns></returns>
        private UInt32 Client_Close()
        {
            UInt32 rc = 0;
            try
            {
                if (_server != null)
                    _server.CloseServer();
                _server = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                rc = 1;
            }
            return rc;
        }

        /// <summary>
        /// 起動通知応答
        /// </summary>
        public UInt32 RespStart() 
        {
            UInt32 rc = 0;
            try
            {
                // 接続
                rc = Client_Open();

                if (rc == 0) 
                {
                    string send = "0x00010001";
                    int ret = _client.SendData(send);
                    rc = (UInt32)(ret * -1);
                }

                // 切断
                Client_Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                rc = 1;
            }
            return rc;
        }





        /// <summary>
        /// クライアント 受信コールバック
        /// </summary>
        /// <param name="strData"></param>
        /// <returns></returns>
        private void Client_ReciveCallBack(string strData)
        {
            strData = strData.Trim();

            Thread.Sleep(10);
            try
            {
                Console.WriteLine(strData);


                //// コールバック
                //// if (OnDataRecive != null)
                //OnDataRecive(strData);
                //if (OnDataRecive != null)
                //{
                //    foreach (string id in strData.Split('\r'))
                //        OnDataRecive(id);
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return;
        }
#endif
        #endregion



    }



}
