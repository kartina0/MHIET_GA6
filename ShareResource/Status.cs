namespace ShareResource
{
    public static class Resource
    {

        /// <summary>
        /// カメラ通信ログ最大件数
        /// </summary>
        public static int MaxCameraLogCount = 100;
        /// <summary>
        /// カメラ通信ログ件数
        /// </summary>
        public static int CameraLogCount = 0;
        /// <summary>
        /// カメラ通信ログリスト
        /// </summary>
        public static List<CameraLog> CameraLogList = new List<CameraLog>();


    }


    /// <summary>
    /// カメラ通信ログ
    /// </summary>
    public class CameraLog
    {
        /// <summary>
        /// 日付
        /// </summary>
        public string Dt { get; set; }
        /// <summary>
        /// IPアドレス
        /// </summary>
        public string Ip { get; set; }
        /// <summary>
        /// ポート番号
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// 種別 (送信/受信)
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 処理内容
        /// </summary>
        public string Command { get; set; }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="type"></param>
        /// <param name="command"></param>
        public CameraLog(string dt, string ip, int port, string type, string command)
        {
            Dt = dt;
            Ip = ip;
            Port = port;
            Type = type;
            Command = command;
        }
    }



}