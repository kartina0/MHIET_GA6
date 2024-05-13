namespace SystemConfig
{
    public static class Const
    {
        /// <summary>
        /// Application Title
        /// </summary>
        public const string Title = "GA6_ControlApp";


        /// <summary>
        /// INIファイル格納フォルダ
        /// </summary>
        public const string IniDir = "Ini";
        /// <summary>
        /// INIファイル名
        /// </summary>
        public const string IniFileName = "GA6_ControlApp.INI";


        #region "ログ関連"
        /// <summary>
        /// システムログ ファイル名
        /// </summary>
        public const string FilePrefix_SystemLog = "System.log";

        /// <summary>
        /// アラーム履歴ログ ファイル名
        /// </summary>
        public const string FilePrefix_AlarmLog = "Alarm.log";

        /// <summary>
        /// 履歴ログ ファイル名
        /// </summary>
        public const string FilePrefix_HistoryLog = "History.log";
        /// <summary>
        /// 操作ログ ファイル名
        /// </summary>
        public const string FilePrefix_OperationLog = "Operation.log";

        /// <summary>
        /// ログファイルの最大行数
        /// </summary>
        public const int LogMaxLineCount = 100000;

        /// <summary>
        /// ログファイルの最大数
        /// </summary>
        public const int LogMaxFileCount = 30;
        #endregion

    }
}