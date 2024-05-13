using DL_Logger;
using ErrorCodeDefine;
using ShareResource;
using System.Reflection;
using System.Text;


namespace Camera
{
    public class CameraManager
    {
        /// <summary>
        /// 自クラス名
        /// </summary>
        private const string THIS_NAME = "Command";


        /// <summary>
        /// ソケットオブジェクト
        /// </summary>
        private Socket _socket = null;
        /// <summary>
        /// サーバー受信処理 ロックオブジェクト
        /// </summary>
        private object _serverRecvLock = new object();

        /// <summary>
        /// 作業Itemリスト取得要求 受信データ
        /// </summary>
        public List<byte[]> recvData = new List<byte[]>();
        /// <summary>
        /// 作業Itemリスト
        /// </summary>
        public List<WorkID> workIdList = new List<WorkID>();

        /// <summary>
        /// カメラ IPアドレス
        /// </summary>
        private string _ipAddress = "127.0.0.1";
        /// <summary>
        /// カメラ ポート番号
        /// </summary>
        private int _port = 55555;
        /// <summary>
        /// 端末ID
        /// </summary>
        private byte[] _id = new byte[4];
        /// <summary>
        /// 端末名
        /// </summary>
        private byte[] _name = new byte[64];

        /// <summary>
        /// 現在作業IDインデックス
        /// </summary>
        public int currrentWorkIdIndex = -1;
        /// <summary>
        /// 現在角度インデックス
        /// </summary>
        public int currrentAngleIndex = -1;
        /// <summary>
        /// 検査結果
        /// </summary>
        public bool result = false;


        /// <summary>
        /// 初期化 処理プロセス
        /// </summary>
        public Sequence InitSeq = Sequence.NONE;
        /// <summary>
        /// 検査 処理プロセス
        /// </summary>
        public EXAM_SEQUENCE ExamSeq = EXAM_SEQUENCE.NONE;
        /// <summary>
        /// 機種情報取得 処理プロセス
        /// </summary>
        public WORK_INFO_SEQUENCE WorkInfoSeq = WORK_INFO_SEQUENCE.NONE;
        /// <summary>
        /// エラー種別
        /// </summary>
        public ERROR_TYPE ErrorType = ERROR_TYPE.NONE;


        ///// <summary>
        ///// 検査モード 処理プロセス
        ///// </summary>
        //public Sequence ExamModeSeq = Sequence.NONE;
        ///// <summary>
        ///// Start要求 処理プロセス
        ///// </summary>
        //public Sequence StartSeq = Sequence.NONE;
        ///// <summary>
        ///// Stop要求 処理プロセス
        ///// </summary>
        //public Sequence StopSeq = Sequence.NONE;
        ///// <summary>
        ///// Close要求 処理プロセス
        ///// </summary>
        //public Sequence CloseSeq = Sequence.NONE;
        /// <summary>
        /// 作業Itemリスト取得要求 処理プロセス
        /// </summary>
        public Sequence UpdateSeq = Sequence.NONE;

        /// <summary>
        /// 起動完了フラグ
        /// </summary>
        public bool initComp = false;
        ///// <summary>
        ///// 作業Item受信完了フラグ
        ///// </summary>
        //public bool workItem_RecvComp = false;
        ///// <summary>
        ///// 検査モード中フラグ
        ///// </summary>
        //public bool examMode = false;







        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CameraManager(string ipAddress, int port) 
        {
            // ソケット生成
            _socket = new Socket(RecvEventHandler);
            // カメラ IPアドレス
            _ipAddress = ipAddress;
            // カメラ ポート番号
            _port = port;

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }


        #region 接続/切断
        /// <summary>
        /// 接続
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public UInt32 Open(string PCIpAddress, int PCPort)
        {
            UInt32 rc = 0;
            try
            {
                rc = _socket.Server_Open(PCIpAddress, PCPort, _ipAddress);
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
                _socket.Server_Close();
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }
        #endregion


        #region 送信コマンド
        /// <summary>
        /// 起動通知応答
        /// </summary>
        /// <returns></returns>
        public UInt32 Send_Boot() 
        {
            UInt32 rc = 0;
            byte[] b = null;
            try
            {
                b = new byte[72];

                // --------------------------------------------------
                // メッセージID
                b[0] = 0x01;
                b[1] = 0x00;
                b[2] = 0x01;
                b[3] = 0x00;
                // --------------------------------------------------
                // 端末ID
                Array.Copy(_id, 0, b, 4, 4);
                // --------------------------------------------------
                // 端末名
                Array.Copy(_name, 0, b, 8, 64);


                // 送信
                rc = _socket.Server_Send(b);

                string hexString = "0x" + BitConverter.ToString(_id).Replace("-", "");
                
                // ログ
                DateTime dt = DateTime.Now;
                Resource.CameraLogList.Add(new CameraLog(dt.ToString("yyyy/MM/dd HH:mm:ss"), _ipAddress, _port, "送信", "起動通知応答"));
                Logger.WriteLog(LogType.INFO, $"{_ipAddress} {_port} 送信 起動通知応答 {hexString}");

            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }
        /// <summary>
        /// ログイン通知応答
        /// </summary>
        /// <returns></returns>
        public UInt32 Send_Login()
        {
            UInt32 rc = 0;
            byte[] b = null;
            try
            {
                b = new byte[72];

                // --------------------------------------------------
                // メッセージID
                b[0] = 0x0C;
                b[1] = 0x00;
                b[2] = 0x01;
                b[3] = 0x00;
                // --------------------------------------------------
                // 端末ID
                Array.Copy(_id, 0, b, 4, 4);
                // --------------------------------------------------
                // 端末名
                Array.Copy(_name, 0, b, 8, 64);


                // 送信
                rc = _socket.Server_Send(b);
                // ログ
                DateTime dt = DateTime.Now;
                Resource.CameraLogList.Add(new CameraLog(dt.ToString("yyyy/MM/dd HH:mm:ss"), _ipAddress, _port, "送信", "ログイン通知応答"));
                Logger.WriteLog(LogType.INFO, $"{_ipAddress} {_port} 送信 ログイン通知応答");
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }
        /// <summary>
        /// ログアウト通知応答
        /// </summary>
        /// <returns></returns>
        public UInt32 Send_Logout()
        {
            UInt32 rc = 0;
            byte[] b = null;
            try
            {
                b = new byte[72];

                // --------------------------------------------------
                // メッセージID
                b[0] = 0x0D;
                b[1] = 0x00;
                b[2] = 0x01;
                b[3] = 0x00;
                // --------------------------------------------------
                // 端末ID
                Array.Copy(_id, 0, b, 4, 4);
                // --------------------------------------------------
                // 端末名
                Array.Copy(_name, 0, b, 8, 64);


                // 送信
                rc = _socket.Server_Send(b);
                // ログ
                DateTime dt = DateTime.Now;
                Resource.CameraLogList.Add(new CameraLog(dt.ToString("yyyy/MM/dd HH:mm:ss"), _ipAddress, _port, "送信", "ログアウト通知応答"));
                Logger.WriteLog(LogType.INFO, $"{_ipAddress} {_port} 送信 ログアウト通知応答");
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }
        /// <summary>
        /// 作業ID開始要求
        /// </summary>
        /// <returns></returns>
        public UInt32 Send_WorkIdStart(string workId)
        {
            UInt32 rc = 0;
            byte[] b = null;
            try
            {
                b = new byte[136];

                // --------------------------------------------------
                // メッセージID
                b[0] = 0x01;
                b[1] = 0x00;
                b[2] = 0x00;
                b[3] = 0x00;
                // --------------------------------------------------
                // 端末ID
                Array.Copy(_id, 0, b, 4, 4);
                // --------------------------------------------------
                // 端末名
                Array.Copy(_name, 0, b, 8, 64);
                // --------------------------------------------------
                // 作業ID
                byte[] bWorkId = new byte[64];
                int i = 0;
                foreach (char c in workId)
                {
                    bWorkId[i] = Convert.ToByte(c);
                    i++;
                }
                Array.Copy(bWorkId, 0, b, 72, 64);


                // 送信
                rc = _socket.Server_Send(b);
                // ログ
                DateTime dt = DateTime.Now;
                Resource.CameraLogList.Add(new CameraLog(dt.ToString("yyyy/MM/dd HH:mm:ss"), _ipAddress, _port, "送信", "作業ID開始要求"));
                Logger.WriteLog(LogType.INFO, $"{_ipAddress} {_port} 送信 作業ID開始要求");
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }
        /// <summary>
        /// 作業ID完了通知応答
        /// </summary>
        /// <returns></returns>
        public UInt32 Send_WorkIdComp()
        {
            UInt32 rc = 0;
            byte[] b = null;
            try
            {
                b = new byte[76];

                // --------------------------------------------------
                // メッセージID
                b[0] = 0x08;
                b[1] = 0x00;
                b[2] = 0x01;
                b[3] = 0x00;
                // --------------------------------------------------
                // 端末ID
                Array.Copy(_id, 0, b, 4, 4);
                // --------------------------------------------------
                // 端末名
                Array.Copy(_name, 0, b, 8, 64);

                // 送信
                rc = _socket.Server_Send(b);

                // ログ
                DateTime dt = DateTime.Now;
                Resource.CameraLogList.Add(new CameraLog(dt.ToString("yyyy/MM/dd HH:mm:ss"), _ipAddress, _port, "送信", "作業ID完了通知応答"));
                Logger.WriteLog(LogType.INFO, $"{_ipAddress} {_port} 送信 作業ID完了通知応答");
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }
        /// <summary>
        /// Start要求
        /// </summary>
        /// <returns></returns>
        public UInt32 Send_Start(string workId, string workOrder, string workItem)
        {
            UInt32 rc = 0;
            byte[] b = null;
            try
            {
                b = new byte[396];

                // --------------------------------------------------
                // メッセージID
                b[0] = 0x02;
                b[1] = 0x00;
                b[2] = 0x00;
                b[3] = 0x00;
                // --------------------------------------------------
                // 端末ID
                Array.Copy(_id, 0, b, 4, 4);
                // --------------------------------------------------
                // 端末名
                Array.Copy(_name, 0, b, 8, 64);
                // --------------------------------------------------
                // 作業ID
                byte[] bWorkId = new byte[64];
                int i = 0;
                foreach (char c in workId)
                {
                    bWorkId[i] = Convert.ToByte(c);
                    i++;
                }
                Array.Copy(bWorkId, 0, b, 72, 64);
                // --------------------------------------------------
                // 作業List
                byte[] bWorkList = new byte[64];
                int j = 0;
                foreach (char c in workOrder)
                {
                    bWorkList[j] = Convert.ToByte(c);
                    j++;
                }
                Array.Copy(bWorkList, 0, b, 136, 64);
                // --------------------------------------------------
                // 作業Item
                byte[] bWorkItem = new byte[64];
                int k = 0;
                foreach (char c in workItem)
                {
                    bWorkList[j] = Convert.ToByte(c);
                    k++;
                }
                Array.Copy(bWorkItem, 0, b, 200, 64);
                // --------------------------------------------------
                // CheckSum値
                int sum = 0;
                for (int index = 0; index < b.Length - 4; index++)
                    sum += b[index];
                byte[] sumByteArray = BitConverter.GetBytes(sum);
                Array.Copy(sumByteArray, 0, b, 392, 2);


                // 送信
                rc = _socket.Server_Send(b);
                // ログ
                DateTime dt = DateTime.Now;
                Resource.CameraLogList.Add(new CameraLog(dt.ToString("yyyy/MM/dd HH:mm:ss"), _ipAddress, _port, "送信", "Start要求"));
                Logger.WriteLog(LogType.INFO, $"{_ipAddress} {_port} 送信 Start要求");
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }
        /// <summary>
        /// Stop要求
        /// </summary>
        /// <returns></returns>
        public UInt32 Send_Stop()
        {
            UInt32 rc = 0;
            byte[] b = null;
            try
            {
                b = new byte[72];

                // --------------------------------------------------
                // メッセージID
                b[0] = 0x03;
                b[1] = 0x00;
                b[2] = 0x00;
                b[3] = 0x00;
                // --------------------------------------------------
                // 端末ID
                Array.Copy(_id, 0, b, 4, 4);
                // --------------------------------------------------
                // 端末名
                Array.Copy(_name, 0, b, 8, 64);


                // 送信
                rc = _socket.Server_Send(b);
                // ログ
                DateTime dt = DateTime.Now;
                Resource.CameraLogList.Add(new CameraLog(dt.ToString("yyyy/MM/dd HH:mm:ss"), _ipAddress, _port, "送信", "Stop要求"));
                Logger.WriteLog(LogType.INFO, $"{_ipAddress} {_port} 送信 Stop要求");
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }
        /// <summary>
        /// 作業Item完了通知応答
        /// </summary>
        /// <returns></returns>
        public UInt32 Send_WorkItemComp(SC10A_RESULT result)
        {
            UInt32 rc = 0;
            byte[] b = null;
            try
            {
                b = new byte[76];

                // --------------------------------------------------
                // メッセージID
                b[0] = 0x07;
                b[1] = 0x00;
                b[2] = 0x01;
                b[3] = 0x00;
                // --------------------------------------------------
                // 端末ID
                Array.Copy(_id, 0, b, 4, 4);
                // --------------------------------------------------
                // 端末名
                Array.Copy(_name, 0, b, 8, 64);
                // --------------------------------------------------
                // 結果
                if(result == SC10A_RESULT.NORMAL)
                    b[72] = 0x00;
                else if(result == SC10A_RESULT.RETRY)
                    b[72] = 0x01;
                else if (result == SC10A_RESULT.CANCEL)
                    b[72] = 0x02;
                b[73] = 0x00;
                b[74] = 0x00;
                b[75] = 0x00;


                // 送信
                rc = _socket.Server_Send(b);
                // ログ
                DateTime dt = DateTime.Now;
                Resource.CameraLogList.Add(new CameraLog(dt.ToString("yyyy/MM/dd HH:mm:ss"), _ipAddress, _port, "送信", "作業Item完了通知応答"));
                Logger.WriteLog(LogType.INFO, $"{_ipAddress} {_port} 送信 作業Item完了通知応答");
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }

        /// <summary>
        /// 作業Itemリスト取得要求
        /// </summary>
        /// <returns></returns>
        public UInt32 Send_WorkItemListGet()
        {
            UInt32 rc = 0;
            byte[] b = null;
            try
            {
                b = new byte[72];

                // --------------------------------------------------
                // メッセージID
                b[0] = 0x04;
                b[1] = 0x00;
                b[2] = 0x00;
                b[3] = 0x00;
                // --------------------------------------------------
                // 端末ID
                Array.Copy(_id, 0, b, 4, 4);
                // --------------------------------------------------
                // 端末名
                Array.Copy(_name, 0, b, 8, 64);


                // 送信
                rc = _socket.Server_Send(b);
                // ログ
                DateTime dt = DateTime.Now;
                Resource.CameraLogList.Add(new CameraLog(dt.ToString("yyyy/MM/dd HH:mm:ss"), _ipAddress, _port, "送信", "作業Itemリスト取得要求"));
                Logger.WriteLog(LogType.INFO, $"{_ipAddress} {_port} 送信 作業Itemリスト取得要求");
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }
        /// <summary>
        /// 作業Itemリスト取得完了通知応答
        /// </summary>
        /// <returns></returns>
        public UInt32 Send_WorkItemListGetComp()
        {
            UInt32 rc = 0;
            byte[] b = null;
            try
            {
                b = new byte[72];

                // --------------------------------------------------
                // メッセージID
                b[0] = 0x0B;
                b[1] = 0x00;
                b[2] = 0x01;
                b[3] = 0x00;
                // --------------------------------------------------
                // 端末ID
                Array.Copy(_id, 0, b, 4, 4);
                // --------------------------------------------------
                // 端末名
                Array.Copy(_name, 0, b, 8, 64);


                // 送信
                rc = _socket.Server_Send(b);
                // ログ
                DateTime dt = DateTime.Now;
                Resource.CameraLogList.Add(new CameraLog(dt.ToString("yyyy/MM/dd HH:mm:ss"), _ipAddress, _port, "送信", "作業Itemリスト取得完了通知応答"));
                Logger.WriteLog(LogType.INFO, $"{_ipAddress} {_port} 送信 作業Itemリスト取得完了通知応答");
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }
        #endregion


        private List<byte[]> b = new List<byte[]>();


        #region 受信コマンド

        
        
        /// <summary>
        /// ソケットサーバー受信イベント
        /// </summary>
        /// <param name="recv"></param>
        private void RecvEventHandler(byte[] byteArray)
        {
            UInt32 rc = 0;
            try
            {
                b.Add(byteArray);


                DateTime dt = DateTime.Now;

                byte[] messageId = new byte[4];
                Array.Copy(byteArray, 0, messageId, 0, 4);

                if (messageId.SequenceEqual(Format.RECV_COMMAND_BOOT))
                {// 起動通知

                    Array.Copy(byteArray, 4, _id, 0, 4);        // 端末ID
                    Array.Copy(byteArray, 8, _name, 0, 64);     // 端末名

                    string hexString = "0x" + BitConverter.ToString(_id).Replace("-", "");

                    // ログ
                    Resource.CameraLogList.Add(new CameraLog(dt.ToString("yyyy/MM/dd HH:mm:ss"), _ipAddress, _port, "受信", "起動通知"));
                    Logger.WriteLog(LogType.INFO, $"{_ipAddress} {_port} 受信 起動通知 {hexString}");

                    InitSeq = Sequence.PROCESSING;

                    // 起動通知応答
                    rc = Send_Boot();
                }
                else if (messageId.SequenceEqual(Format.RECV_COMMAND_WORK_ITEM_COMP_MATCHING))
                {// 作業Item完了通知(マッチング)

                    // OKorNG 確認
                    byte[] b = new byte[2];
                    Array.Copy(byteArray, 672, b, 0, 2);
                    int result_int = BitConverter.ToInt16(b, 0);

                    //Resource.CameraLogList.Add(new CameraLog(dt.ToString("yyyy/MM/dd HH:mm:ss"), "受信", "作業Item完了通知(マッチング)"));
                    //Logger.WriteLog(LogType.INFO, "受信 : 作業Item完了通知(マッチング)   (結果 -> OK)");

                    if (result_int == 0)
                    {// OK
                     // ログ
                        Resource.CameraLogList.Add(new CameraLog(dt.ToString("yyyy/MM/dd HH:mm:ss"), _ipAddress, _port, "受信", "作業Item完了通知(マッチング) (結果->OK)"));
                        Logger.WriteLog(LogType.INFO, $"{_ipAddress} {_port} 受信 作業Item完了通知(マッチング) (結果->OK)");

                        result = true;
                    }
                    else if (result_int == 1)
                    {// N/A
                     // ログ
                        Resource.CameraLogList.Add(new CameraLog(dt.ToString("yyyy/MM/dd HH:mm:ss"), _ipAddress, _port, "受信", "作業Item完了通知(マッチング) (結果->N/A)"));
                        Logger.WriteLog(LogType.INFO, $"{_ipAddress} {_port} 受信 作業Item完了通知(マッチング) (結果->N/A)");
                    }
                    else if (result_int == -1)
                    {// NG

                        //// エラーコード取得
                        //byte[] errorCode = new byte[2];
                        //Array.Copy(byteArray, 58, errorCode, 0, 2);
                        //string errorCode_hex = "0x" + BitConverter.ToString(errorCode).Replace("-", "");

                        // ログ
                        Resource.CameraLogList.Add(new CameraLog(dt.ToString("yyyy/MM/dd HH:mm:ss"), _ipAddress, _port, "受信", "作業Item完了通知(マッチング) (結果->NG)"));
                        Logger.WriteLog(LogType.INFO, $"{_ipAddress} {_port} 受信 作業Item完了通知(マッチング) (結果->NG)");

                        result = false;
                    }

                    // 起動通知応答
                    rc = Send_WorkItemComp(SC10A_RESULT.CANCEL);
                }
                else if (messageId.SequenceEqual(Format.RECV_COMMAND_WORK_ITEM_COMP_SERIAL_NUMBER))
                {// 作業Item完了通知(シリアルナンバー)

                    // ログ
                    Resource.CameraLogList.Add(new CameraLog(dt.ToString("yyyy/MM/dd HH:mm:ss"), _ipAddress, _port, "受信", "作業Item完了通知(シリアルナンバー)"));
                    Logger.WriteLog(LogType.INFO, $"{_ipAddress} {_port} 受信 作業Item完了通知(シリアルナンバー)");
                }
                else if (messageId.SequenceEqual(Format.RECV_COMMAND_WORK_ITEM_COMP_CHECK_MODE))
                {// 作業Item完了通知(チェックモード)

                    // ログ
                    Resource.CameraLogList.Add(new CameraLog(dt.ToString("yyyy/MM/dd HH:mm:ss"), _ipAddress, _port, "受信", "作業Item完了通知(チェックモード)"));
                    Logger.WriteLog(LogType.INFO, $"{_ipAddress} {_port} 受信 作業Item完了通知(チェックモード)");
                }
                else if (messageId.SequenceEqual(Format.RECV_COMMAND_WORK_ITEM_COMP_STOP))
                {// 作業Item完了通知(Stop)

                    // ログ
                    Resource.CameraLogList.Add(new CameraLog(dt.ToString("yyyy/MM/dd HH:mm:ss"), _ipAddress, _port, "受信", "作業Item完了通知(Stop)"));
                    Logger.WriteLog(LogType.INFO, $"{_ipAddress} {_port} 受信 作業Item完了通知(Stop)");
                }
                else if (messageId.SequenceEqual(Format.RECV_COMMAND_WORK_ID_COMP))
                {// 作業ID完了通知

                    // ログ
                    Resource.CameraLogList.Add(new CameraLog(dt.ToString("yyyy/MM/dd HH:mm:ss"), _ipAddress, _port, "受信", "作業ID完了通知"));
                    Logger.WriteLog(LogType.INFO, $"{_ipAddress} {_port} 受信 作業ID完了通知");

                    // 作業ID完了通知応答
                    rc = Send_WorkIdComp();

                    if (ExamSeq == EXAM_SEQUENCE.EXAM_COMP_WAITING)
                        ExamSeq = EXAM_SEQUENCE.RESULT_OUTPUT;
                }
                else if (messageId.SequenceEqual(Format.RECV_COMMAND_WORK_ITEM_LIST_DATA)
                      || messageId.SequenceEqual(Format.RECV_COMMAND_WORK_ITEM_LIST_GET_COMP))
                {// 作業Itemリストデータ通知
                 // 作業Itemリスト取得完了通知

                    // ログ
                    if (messageId.SequenceEqual(Format.RECV_COMMAND_WORK_ITEM_LIST_DATA))
                    {
                        Resource.CameraLogList.Add(new CameraLog(dt.ToString("yyyy/MM/dd HH:mm:ss"), _ipAddress, _port, "受信", "作業Itemリストデータ通知"));
                        Logger.WriteLog(LogType.INFO, $"{_ipAddress} {_port} 受信 作業Itemリストデータ通知");
                    }
                    else if (messageId.SequenceEqual(Format.RECV_COMMAND_WORK_ITEM_LIST_GET_COMP))
                    {
                        Resource.CameraLogList.Add(new CameraLog(dt.ToString("yyyy/MM/dd HH:mm:ss"), _ipAddress, _port, "受信", "作業Itemリスト取得完了通知"));
                        Logger.WriteLog(LogType.INFO, $"{_ipAddress} {_port} 受信 作業Itemリスト取得完了通知");
                    }

                    // 受信データから作業Itemを解読
                    rc = DecodeWorkItem(byteArray, out bool recvComp, out int count);

                    // 作業Itemリスト取得完了
                    if (STATUS_SUCCESS(rc) && recvComp)
                    {
                        UpdateSeq = Sequence.COMP;

                        // 作業Itemリスト取得完了通知応答
                        rc = Send_WorkItemListGetComp();

                        if (WorkInfoSeq == WORK_INFO_SEQUENCE.WORK_ITEM_COMP_WAITING)
                            WorkInfoSeq = WORK_INFO_SEQUENCE.WORK_ITEM_OUTPUT;
                    }
                }
                else if (messageId.SequenceEqual(Format.RECV_COMMAND_LOGIN))
                {// ログイン通知

                    // ログ
                    Resource.CameraLogList.Add(new CameraLog(dt.ToString("yyyy/MM/dd HH:mm:ss"), _ipAddress, _port, "受信", "ログイン通知"));
                    Logger.WriteLog(LogType.INFO, $"{_ipAddress} {_port} 受信 ログイン通知");

                    // ログイン通知応答
                    rc = Send_Login();

                    // カメラ通信初期化完了
                    if (STATUS_SUCCESS(rc))
                        InitSeq = Sequence.COMP;
                }
                else if (messageId.SequenceEqual(Format.RECV_COMMAND_LOGOUT))
                {// ログアウト通知

                    // ログ
                    Resource.CameraLogList.Add(new CameraLog(dt.ToString("yyyy/MM/dd HH:mm:ss"), _ipAddress, _port, "受信", "ログアウト通知"));
                    Logger.WriteLog(LogType.INFO, $"{_ipAddress} {_port} 受信 ログアウト通知");

                    // ログアウト通知応答
                    rc = Send_Logout();
                }
                else if (messageId.SequenceEqual(Format.RECV_COMMAND_SYSTEM_STOP))
                {// システム停止通知

                    // ログ
                    Resource.CameraLogList.Add(new CameraLog(dt.ToString("yyyy/MM/dd HH:mm:ss"), _ipAddress, _port, "受信", "システム停止通知"));
                    Logger.WriteLog(LogType.INFO, $"{_ipAddress} {_port} 受信 システム停止通知");

                }
                else if (messageId.SequenceEqual(Format.RECV_COMMAND_TIMEOUT))
                {// タイムアウト通知

                    // ログ
                    Resource.CameraLogList.Add(new CameraLog(dt.ToString("yyyy/MM/dd HH:mm:ss"), _ipAddress, _port, "受信", "タイムアウト通知"));
                    Logger.WriteLog(LogType.INFO, $"{_ipAddress} {_port} 受信 タイムアウト通知");
                }
                else if (messageId.SequenceEqual(Format.RECV_COMMAND_DATA_GET_COMP))
                {// データ取得完了通知

                    // ログ
                    Resource.CameraLogList.Add(new CameraLog(dt.ToString("yyyy/MM/dd HH:mm:ss"), _ipAddress, _port, "受信", "データ取得完了通知"));
                    Logger.WriteLog(LogType.INFO, $"{_ipAddress} {_port} 受信 データ取得完了通知");
                }
                else if (messageId.SequenceEqual(Format.RECV_COMMAND_DATA_SET_READY_COMP))
                {// データ設定準備完了通知

                    // ログ
                    Resource.CameraLogList.Add(new CameraLog(dt.ToString("yyyy/MM/dd HH:mm:ss"), _ipAddress, _port, "受信", "データ設定準備完了通知"));
                    Logger.WriteLog(LogType.INFO, $"{_ipAddress} {_port} 受信 データ設定準備完了通知");
                }
                else if (messageId.SequenceEqual(Format.RECV_COMMAND_DATA_SET_COMP))
                {// データ設定完了通知

                    // ログ
                    Resource.CameraLogList.Add(new CameraLog(dt.ToString("yyyy/MM/dd HH:mm:ss"), _ipAddress, _port, "受信", "データ設定完了通知"));
                    Logger.WriteLog(LogType.INFO, $"{_ipAddress} {_port} 受信 データ設定完了通知");
                }
                else if (messageId.SequenceEqual(Format.RECV_COMMAND_FILE_PATH_DATA))
                {// ファイルパスデータ通知

                    // ログ
                    Resource.CameraLogList.Add(new CameraLog(dt.ToString("yyyy/MM/dd HH:mm:ss"), _ipAddress, _port, "受信", "ファイルパスデータ通知"));
                    Logger.WriteLog(LogType.INFO, $"{_ipAddress} {_port} 受信 ファイルパスデータ通知");
                }
                else if (messageId.SequenceEqual(Format.RECV_COMMAND_FILE_PATH_DATA_GET_COMP))
                {// ファイルパスデータ取得完了通知

                    // ログ
                    Resource.CameraLogList.Add(new CameraLog(dt.ToString("yyyy/MM/dd HH:mm:ss"), _ipAddress, _port, "受信", "ファイルパスデータ取得完了通知"));
                    Logger.WriteLog(LogType.INFO, $"{_ipAddress} {_port} 受信 ファイルパスデータ取得完了通知");
                }
                else if (messageId.SequenceEqual(Format.RECV_COMMAND_WORK_ID_START))
                {// 作業ID開始要求応答

                    // OKorNG 確認
                    byte[] result = new byte[2];
                    Array.Copy(byteArray, 80, result, 0, 2);
                    int result_int = BitConverter.ToInt16(result, 0);

                    // ログ
                    if (result_int == 0)
                    {// OK
                        Resource.CameraLogList.Add(new CameraLog(dt.ToString("yyyy/MM/dd HH:mm:ss"), _ipAddress, _port, "受信", "作業ID開始要求応答 (結果 -> OK)"));
                        Logger.WriteLog(LogType.INFO, $"{_ipAddress} {_port} 受信 作業ID開始要求応答 (結果 -> OK)");
                    }
                    else if (result_int == -1)
                    {// NG
                        // エラーコード取得
                        byte[] errorCode = new byte[2];
                        Array.Copy(byteArray, 82, errorCode, 0, 2);
                        string errorCode_hex = "0x" + BitConverter.ToString(errorCode).Replace("-", "");

                        Resource.CameraLogList.Add(new CameraLog(dt.ToString("yyyy/MM/dd HH:mm:ss"), _ipAddress, _port, "受信", $"作業ID開始要求応答 (結果 -> NG) (エラーコード -> {errorCode_hex})"));
                        Logger.WriteLog(LogType.INFO, $"{_ipAddress} {_port} 受信 作業ID開始要求応答 (結果 -> NG) (エラーコード -> {errorCode_hex})");
                    }


                    //// ログ
                    //Resource.CameraLogList.Add(new CameraLog(dt.ToString("yyyy/MM/dd HH:mm:ss"), _ipAddress, _port, "受信", "作業ID開始要求応答"));
                    //Logger.WriteLog(LogType.INFO, $"{_ipAddress} {_port} 受信 作業ID開始要求応答");

                    if (ExamSeq == EXAM_SEQUENCE.EXAM_WORK_ID_RECV)
                        ExamSeq = EXAM_SEQUENCE.EXAM_START_SEND;
                }
                else if (messageId.SequenceEqual(Format.RECV_COMMAND_START))
                {// Start要求応答

                    // OKorNG 確認
                    byte[] result = new byte[2];
                    Array.Copy(byteArray, 80, result, 0, 2);
                    int result_int = BitConverter.ToInt16(result, 0);

                    // ログ
                    if (result_int == 0)
                    {// OK
                        Resource.CameraLogList.Add(new CameraLog(dt.ToString("yyyy/MM/dd HH:mm:ss"), _ipAddress, _port, "受信", "Start要求応答 (結果 -> OK)"));
                        Logger.WriteLog(LogType.INFO, $"{_ipAddress} {_port} 受信 Start要求応答 (結果 -> OK)");

                        if (ExamSeq == EXAM_SEQUENCE.EXAM_START_RECV)
                            ExamSeq = EXAM_SEQUENCE.EXAM_COMP_WAITING;
                    }
                    else if (result_int == -1)
                    {// NG
                        // エラーコード取得
                        byte[] errorCode = new byte[2];
                        Array.Copy(byteArray, 82, errorCode, 0, 2);
                        string errorCode_hex = "0x" + BitConverter.ToString(errorCode).Replace("-", "");

                        Resource.CameraLogList.Add(new CameraLog(dt.ToString("yyyy/MM/dd HH:mm:ss"), _ipAddress, _port, "受信", $"Start要求応答 (結果 -> NG) (エラーコード -> {errorCode_hex})"));
                        Logger.WriteLog(LogType.INFO, $"{_ipAddress} {_port} 受信 Start要求応答 (結果 -> NG) (エラーコード -> {errorCode_hex})");


                        ErrorType = ERROR_TYPE.EXAM_START_ERROR;

                        //if (ExamSeq == EXAM_SEQUENCE.EXAM_START_RECV)
                        //    ExamSeq = EXAM_SEQUENCE.EXAM_COMP_WAITING;
                    }


                }
                else if (messageId.SequenceEqual(Format.RECV_COMMAND_STOP))
                {// Stop要求応答


                    // OKorNG 確認
                    byte[] result = new byte[2];
                    Array.Copy(byteArray, 80, result, 0, 2);
                    int result_int = BitConverter.ToInt16(result, 0);

                    // ログ
                    if (result_int == 0)
                    {// OK

                        Resource.CameraLogList.Add(new CameraLog(dt.ToString("yyyy/MM/dd HH:mm:ss"), _ipAddress, _port, "受信", "Stop要求応答 (結果 -> OK)"));
                        Logger.WriteLog(LogType.INFO, $"{_ipAddress} {_port} 受信 Stop要求応答 (結果 -> OK)");
                    }
                    else if (result_int == -1)
                    {// NG
                        // エラーコード取得
                        byte[] errorCode = new byte[2];
                        Array.Copy(byteArray, 82, errorCode, 0, 2);
                        string errorCode_hex = "0x" + BitConverter.ToString(errorCode).Replace("-", "");

                        Resource.CameraLogList.Add(new CameraLog(dt.ToString("yyyy/MM/dd HH:mm:ss"), _ipAddress, _port, "受信", $"Stop要求応答 (結果 -> NG) (エラーコード -> {errorCode_hex})"));
                        Logger.WriteLog(LogType.INFO, $"{_ipAddress} {_port} 受信 Stop要求応答 (結果 -> NG) (エラーコード -> {errorCode_hex})");
                    }
                }
                else if (messageId.SequenceEqual(Format.RECV_COMMAND_WORK_ITEM_LIST_GET))
                {// 作業Itemリスト取得要求応答

                    // ログ
                    Resource.CameraLogList.Add(new CameraLog(dt.ToString("yyyy/MM/dd HH:mm:ss"), _ipAddress, _port, "受信", "作業Itemリスト取得要求応答"));
                    Logger.WriteLog(LogType.INFO, $"{_ipAddress} {_port} 受信 作業Itemリスト取得要求応答");

                    // もしエラーだったら
                    WorkInfoSeq = WORK_INFO_SEQUENCE.WORK_ITEM_COMP_WAITING;
                }
                else if (messageId.SequenceEqual(Format.RECV_COMMAND_WORK_ID_EXECUTE))
                {// 作業ID実行要求応答

                    // ログ
                    Resource.CameraLogList.Add(new CameraLog(dt.ToString("yyyy/MM/dd HH:mm:ss"), _ipAddress, _port, "受信", "作業ID実行要求応答"));
                    Logger.WriteLog(LogType.INFO, $"{_ipAddress} {_port} 受信 作業ID実行要求応答");
                }
                else if (messageId.SequenceEqual(Format.RECV_COMMAND_WORK_ID_CHANGE))
                {// 作業ID変更要求応答

                    // ログ
                    Resource.CameraLogList.Add(new CameraLog(dt.ToString("yyyy/MM/dd HH:mm:ss"), _ipAddress, _port, "受信", "作業ID変更要求応答"));
                    Logger.WriteLog(LogType.INFO, $"{_ipAddress} {_port} 受信 作業ID変更要求応答");
                }
                else if (messageId.SequenceEqual(Format.RECV_COMMAND_IO_INPUT))
                {// 外部IO入力要求応答

                    // ログ
                    Resource.CameraLogList.Add(new CameraLog(dt.ToString("yyyy/MM/dd HH:mm:ss"), _ipAddress, _port, "受信", "外部IO入力要求応答"));
                    Logger.WriteLog(LogType.INFO, $"{_ipAddress} {_port} 受信 外部IO入力要求応答");
                }
                else if (messageId.SequenceEqual(Format.RECV_COMMAND_STATUS_CHECK))
                {// 状態確認要求応答

                    // ログ
                    Resource.CameraLogList.Add(new CameraLog(dt.ToString("yyyy/MM/dd HH:mm:ss"), _ipAddress, _port, "受信", "状態確認要求応答"));
                    Logger.WriteLog(LogType.INFO, $"{_ipAddress} {_port} 受信 状態確認要求応答");
                }
                else if (messageId.SequenceEqual(Format.RECV_COMMAND_SHUTDOWN_EXECUTE))
                {// シャットダウン実行要求応答

                    // ログ
                    Resource.CameraLogList.Add(new CameraLog(dt.ToString("yyyy/MM/dd HH:mm:ss"), _ipAddress, _port, "受信", "シャットダウン実行要求応答"));
                    Logger.WriteLog(LogType.INFO, $"{_ipAddress} {_port} 受信 シャットダウン実行要求応答");
                }
                else if (messageId.SequenceEqual(Format.RECV_COMMAND_REBOOT))
                {// 再起動実行要求応答

                    // ログ
                    Resource.CameraLogList.Add(new CameraLog(dt.ToString("yyyy/MM/dd HH:mm:ss"), _ipAddress, _port, "受信", "再起動実行要求応答"));
                    Logger.WriteLog(LogType.INFO, $"{_ipAddress} {_port} 受信 再起動実行要求応答");
                }
                else if (messageId.SequenceEqual(Format.RECV_COMMAND_FILE_SAVE_PATH_CHANGE))
                {// ファイル保存先変更要求応答

                    // ログ
                    Resource.CameraLogList.Add(new CameraLog(dt.ToString("yyyy/MM/dd HH:mm:ss"), _ipAddress, _port, "受信", "ファイル保存先変更要求応答"));
                    Logger.WriteLog(LogType.INFO, $"{_ipAddress} {_port} 受信 ファイル保存先変更要求応答");
                }
                else if (messageId.SequenceEqual(Format.RECV_COMMAND_DATA_GET))
                {// データ取得要求応答

                    // ログ
                    Resource.CameraLogList.Add(new CameraLog(dt.ToString("yyyy/MM/dd HH:mm:ss"), _ipAddress, _port, "受信", "データ取得要求応答"));
                    Logger.WriteLog(LogType.INFO, $"{_ipAddress} {_port} 受信 データ取得要求応答");
                }
                else if (messageId.SequenceEqual(Format.RECV_COMMAND_DATA_SET))
                {// データ設定要求応答

                    // ログ
                    Resource.CameraLogList.Add(new CameraLog(dt.ToString("yyyy/MM/dd HH:mm:ss"), _ipAddress, _port, "受信", "データ設定要求応答"));
                    Logger.WriteLog(LogType.INFO, $"{_ipAddress} {_port} 受信 データ設定要求応答");
                }
                else if (messageId.SequenceEqual(Format.RECV_COMMAND_FILE_PATH_GET))
                {// ファイルパス取得要求応答

                    // ログ
                    Resource.CameraLogList.Add(new CameraLog(dt.ToString("yyyy/MM/dd HH:mm:ss"), _ipAddress, _port, "受信", "ファイルパス取得要求応答"));
                    Logger.WriteLog(LogType.INFO, $"{_ipAddress} {_port} 受信 ファイルパス取得要求応答");
                }
                else 
                {
                    Resource.CameraLogList.Add(new CameraLog(dt.ToString("yyyy/MM/dd HH:mm:ss"), _ipAddress, _port, "受信", "(不明)"));
                    Logger.WriteLog(LogType.INFO, $"{_ipAddress} {_port} 受信 (不明)");



                    // 受信データから作業Itemを解読
                    rc = DecodeWorkItem(byteArray, out bool recvComp, out int count);

                    // 作業Itemリスト取得完了
                    if (STATUS_SUCCESS(rc) && recvComp)
                    {
                        UpdateSeq = Sequence.COMP;

                        // 作業Itemリスト取得完了通知応答
                        rc = Send_WorkItemListGetComp();

                        if (WorkInfoSeq == WORK_INFO_SEQUENCE.WORK_ITEM_COMP_WAITING)
                            WorkInfoSeq = WORK_INFO_SEQUENCE.WORK_ITEM_OUTPUT;
                    }

                }

            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, $"{THIS_NAME} {MethodBase.GetCurrentMethod().Name}() : {ex.ToString()}");
                rc = (UInt32)ErrorCodeList.EXCEPTION;
            }
        }

        /// <summary>
        /// 受信データから作業Itemを解読
        /// </summary>
        /// <returns></returns>
        private UInt32 DecodeWorkItem(byte[] byteArray, out bool recvComp, out int count)
        {
            UInt32 rc = 0;
            recvComp = false;
            count = 0;
            try
            {
                // 各コマンドの基本情報を定義
                int workItemListData_count = 272;
                byte[] workItemListData_byte = new byte[4] { 0x09, 0x00, 0x01, 0x10 };
                int workItemListGetComp_count = 84;
                byte[] workItemListGetComp_byte = new byte[4] { 0x0B, 0x00, 0x01, 0x10 };


                // ------------------------------
                // 作業Itemリスト取得完了通知　を探す
                // ------------------------------
                bool found = true;
                for (int i = 0; i <= byteArray.Length - workItemListGetComp_byte.Length; i++)
                {
                    found = true;
                    for (int j = 0; j < workItemListGetComp_byte.Length; j++)
                    {
                        if (byteArray[i + j] != workItemListGetComp_byte[j])
                        {
                            found = false;
                            break;
                        }
                    }
                    if (found)
                    {// 作業Itemリスト取得要求応答
                        break;
                    }
                }



                if (!found)
                {// 作業Itemリスト取得完了通知 ナシ

                    // 受信データに追加
                    recvData.Add(byteArray);
                }
                else
                {// 作業Itemリスト取得完了通知 発見

                    //int a = 0;
                    //if (_port == 50020)
                    //    a = 1;


                    // 作業Itemリスト取得完了
                    recvComp = true;

                    // 転送件数を取得
                    //count = 
                    // 末尾の作業Itemリスト取得完了通知を除外する
                    byte[] byteArray_trim = new byte[byteArray.Length - workItemListGetComp_count];
                    Array.Copy(byteArray, 0, byteArray_trim, 0, byteArray.Length - workItemListGetComp_count);
                    // 受信データに追加
                    recvData.Add(byteArray_trim);


                    // 今まで受信したrecvDataを1つのbyte配列にまとめる
                    int byteCount = 0;
                    for (int i = 0; i < recvData.Count; i++)
                        byteCount += recvData[i].Length;
                    byte[] totalByteArray = new byte[byteCount];
                    int startIndex = 0;
                    for (int i = 0; i < recvData.Count; i++)
                    {
                        Array.Copy(recvData[i], 0, totalByteArray, startIndex, recvData[i].Length);
                        startIndex += recvData[i].Length;
                    }



                    // ------------------------------
                    // 受信データから作業Itemを解析
                    // ------------------------------
                    byte[] line = null;
                    byte[] lines = new byte[totalByteArray.Length];
                    Array.Copy(totalByteArray, 0, lines, 0, totalByteArray.Length);
                    while (true)
                    {
                        if (lines.Length <= 0)
                            break;


                        // ------------------------------
                        // 作業Itemリストデータ通知のメッセージIDを探す
                        found = true;
                        for (int i = 0; i < workItemListData_byte.Length; i++)
                        {
                            if (lines[i] != workItemListData_byte[i])
                            {
                                found = false;
                                break;
                            }
                        }
                        if (found)
                        {
                            // ------------------------------
                            // byte配列を切り出し
                            line = new byte[workItemListData_count];
                            Array.Copy(lines, 0, line, 0, workItemListData_count);
                            byte[] bufs = new byte[lines.Length];
                            Array.Copy(lines, 0, bufs, 0, lines.Length);
                            lines = new byte[bufs.Length - workItemListData_count];
                            if (bufs.Length - workItemListData_count > 0)
                                Array.Copy(bufs, workItemListData_count, lines, 0, bufs.Length - workItemListData_count);


                            // ------------------------------
                            // 作業ID、作業指示リスト、作業Itemを取得
                            string workIdName = "";
                            string workListName = "";
                            string workItemName = "";

                            byte[] b = new byte[64];
                            Array.Copy(line, 80, b, 0, 64);
                            string str = (Encoding.GetEncoding("shift_jis").GetString(b)).Replace("\0", string.Empty);
                            workIdName = str.Replace("\0", string.Empty);
                            if (workIdName == "Default")
                                continue;

                            b = new byte[64];
                            Array.Copy(line, 144, b, 0, 64);
                            str = Encoding.GetEncoding("shift_jis").GetString(b);
                            workListName = str.Replace("\0", string.Empty);

                            b = new byte[64];
                            Array.Copy(line, 208, b, 0, 64);
                            str = Encoding.GetEncoding("shift_jis").GetString(b);
                            workItemName = str.Replace("\0", string.Empty);

                            var workId = workIdList.FirstOrDefault(x => x.name == workIdName);
                            if (workId == null)
                            {// 重複防止
                                WorkID id = new WorkID();
                                id.name = workIdName;
                                workIdList.Add(id);
                            }

                            var workList = workIdList[workIdList.Count - 1].workOrderList.FirstOrDefault(x => x.name == workListName);
                            if (workList == null)
                            {// 重複防止
                                WorkOrder list = new WorkOrder();
                                list.name = workListName;
                                workIdList[workIdList.Count - 1].workOrderList.Add(list);
                            }

                            var workItem = workIdList[workIdList.Count - 1].workOrderList[workIdList[workIdList.Count - 1].workOrderList.Count - 1].workItemList.FirstOrDefault(x => x.name == workItemName);
                            if (workItem == null)
                            {// 重複防止
                                WorkItem item = new WorkItem();
                                item.name = workItemName;
                                workIdList[workIdList.Count - 1].workOrderList[workIdList[workIdList.Count - 1].workOrderList.Count - 1].workItemList.Add(item);
                            }

                            continue;
                        }
                        else
                        {
                            break;
                        }
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
        #endregion


        
        /// <summary>
        /// Check Error State
        /// </summary>
        private static bool STATUS_SUCCESS(UInt32 err) { return err == (int)ErrorCodeList.STATUS_SUCCESS; }

    }




    /// <summary>
    /// 作業ID
    /// </summary>
    public class WorkID
    {
        /// <summary>
        /// 作業ID名
        /// </summary>
        public string name = "";
        /// <summary>
        /// 作業指示リスト リスト
        /// </summary>
        public List<WorkOrder> workOrderList = null;

        public WorkID()
        {
            workOrderList = new List<WorkOrder>();
        }

    }
    /// <summary>
    /// 作業指示リスト
    /// </summary>
    public class WorkOrder
    {
        /// <summary>
        /// 作業指示リスト名
        /// </summary>
        public string name = "";
        /// <summary>
        /// 作業Item リスト
        /// </summary>
        public List<WorkItem> workItemList = null;

        public WorkOrder()
        {
            workItemList = new List<WorkItem>();
        }
    }
    /// <summary>
    /// 作業Item
    /// </summary>
    public class WorkItem
    {
        /// <summary>
        /// 作業Item名
        /// </summary>
        public string name = "";
    }


    /// <summary>
    /// 処理プロセス
    /// </summary>
    public enum Sequence 
    {
        /// <summary> None </summary>
        NONE = 0,
        /// <summary> 処理中 </summary>
        PROCESSING,
        /// <summary> 完了 </summary>
        COMP,
        /// <summary> エラー </summary>
        ERROR,
    }

    /// <summary>
    /// 検査 処理プロセス
    /// </summary>
    public enum EXAM_SEQUENCE
    {
        /// <summary> None </summary>
        NONE = 0,
        /// <summary> 待機中 </summary>
        WAITING,
        /// <summary> 検査項目設定完了 </summary>
        ITEM_INPUT_COMP,

        /// <summary> 検査開始待ち </summary>
        EXAM_WAITING,
        /// <summary> 作業ID開始要求 送信 </summary>
        EXAM_WORK_ID_SEND,
        /// <summary> 作業ID開始要求応答 受信待ち </summary>
        EXAM_WORK_ID_RECV,
        /// <summary> START要求 送信 </summary>
        EXAM_START_SEND,
        /// <summary> START要求応答 受信待ち </summary>
        EXAM_START_RECV,
        /// <summary> 検査完了待ち </summary>
        EXAM_COMP_WAITING,
        /// <summary> 検査結果出力 </summary>
        RESULT_OUTPUT,
        ///// <summary> 検査結果出力完了 </summary>
        //RESULT_OUTPUT_COMP,

        /// <summary> エラー </summary>
        ERROR,
    }
    /// <summary>
    /// 機種情報取得 処理プロセス
    /// </summary>
    public enum WORK_INFO_SEQUENCE
    {
        /// <summary> None </summary>
        NONE = 0,
        /// <summary> 作業Itemリスト取得要求 送信 </summary>
        WORK_ITEM_SEND,
        /// <summary> 作業Itemリスト取得要求応答 受信待ち </summary>
        WORK_ITEM_RECV,
        /// <summary> 作業Itemリスト取得完了待ち </summary>
        WORK_ITEM_COMP_WAITING,
        /// <summary> 作業Itemリスト出力 </summary>
        WORK_ITEM_OUTPUT,

        /// <summary> エラー </summary>
        ERROR,
    }
    /// <summary>
    /// 機種情報取得 処理プロセス
    /// </summary>
    public enum ERROR_TYPE
    {
        /// <summary> None </summary>
        NONE = 0,
        /// <summary> 検査 START要求 エラー </summary>
        EXAM_START_ERROR,


    }

}