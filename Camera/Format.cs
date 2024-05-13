using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Camera
{
    public static class Format
    {

        #region 受信コマンド メッセージID
        /// <summary> 起動通知 </summary>
        public static byte[] RECV_COMMAND_BOOT = new byte[4] { 0x01, 0x00, 0x01, 0x10 };
        /// <summary> 作業Item完了通知(マッチング) </summary>
        public static byte[] RECV_COMMAND_WORK_ITEM_COMP_MATCHING = new byte[4] { 0x02, 0x00, 0x01, 0x10 };
        /// <summary> 作業Item完了通知(シリアルナンバー) </summary>
        public static byte[] RECV_COMMAND_WORK_ITEM_COMP_SERIAL_NUMBER = new byte[4] { 0x03, 0x00, 0x01, 0x10 };
        /// <summary> 作業Item完了通知(チェックモード) </summary>
        public static byte[] RECV_COMMAND_WORK_ITEM_COMP_CHECK_MODE = new byte[4] { 0x04, 0x00, 0x01, 0x10 };
        /// <summary> 作業Item完了通知(Stop) </summary>
        public static byte[] RECV_COMMAND_WORK_ITEM_COMP_STOP = new byte[4] { 0x05, 0x00, 0x01, 0x10 };
        /// <summary> 作業ID完了通知 </summary>
        public static byte[] RECV_COMMAND_WORK_ID_COMP = new byte[4] { 0x08, 0x00, 0x01, 0x10 };
        /// <summary> 作業Itemリストデータ通知 </summary>
        public static byte[] RECV_COMMAND_WORK_ITEM_LIST_DATA = new byte[4] { 0x09, 0x00, 0x01, 0x10 };
        /// <summary> 作業Itemリスト取得完了通知 </summary>
        public static byte[] RECV_COMMAND_WORK_ITEM_LIST_GET_COMP = new byte[4] { 0x0B, 0x00, 0x01, 0x10 };
        /// <summary> ログイン通知 </summary>
        public static byte[] RECV_COMMAND_LOGIN = new byte[4] { 0x0C, 0x00, 0x01, 0x10 };
        /// <summary> ログアウト通知 </summary>
        public static byte[] RECV_COMMAND_LOGOUT = new byte[4] { 0x0D, 0x00, 0x01, 0x10 };
        /// <summary> システム停止通知 </summary>
        public static byte[] RECV_COMMAND_SYSTEM_STOP = new byte[4] { 0x0E, 0x00, 0x01, 0x10 };
        /// <summary> タイムアウト通知 </summary>
        public static byte[] RECV_COMMAND_TIMEOUT = new byte[4] { 0x0F, 0x00, 0x01, 0x10 };
        /// <summary> データ取得完了通知 </summary>
        public static byte[] RECV_COMMAND_DATA_GET_COMP = new byte[4] { 0x10, 0x00, 0x03, 0x10 };
        /// <summary> データ設定準備完了通知 </summary>
        public static byte[] RECV_COMMAND_DATA_SET_READY_COMP = new byte[4] { 0x11, 0x00, 0x03, 0x10 };
        /// <summary> データ設定完了通知 </summary>
        public static byte[] RECV_COMMAND_DATA_SET_COMP = new byte[4] { 0x12, 0x00, 0x03, 0x10 };
        /// <summary> ファイルパスデータ通知 </summary>
        public static byte[] RECV_COMMAND_FILE_PATH_DATA = new byte[4] { 0x13, 0x00, 0x03, 0x10 };
        /// <summary> ファイルパスデータ取得完了通知 </summary>
        public static byte[] RECV_COMMAND_FILE_PATH_DATA_GET_COMP = new byte[4] { 0x14, 0x00, 0x03, 0x10 };

        /// <summary> 作業ID開始要求応答 </summary>
        public static byte[] RECV_COMMAND_WORK_ID_START = new byte[4] { 0x01, 0x00, 0x00, 0x10 };
        /// <summary> Start要求応答 </summary>
        public static byte[] RECV_COMMAND_START = new byte[4] { 0x02, 0x00, 0x00, 0x10 };
        /// <summary> Stop要求応答 </summary>
        public static byte[] RECV_COMMAND_STOP = new byte[4] { 0x03, 0x00, 0x00, 0x10 };
        /// <summary> 作業Itemリスト取得要求応答 </summary>
        public static byte[] RECV_COMMAND_WORK_ITEM_LIST_GET = new byte[4] { 0x04, 0x00, 0x00, 0x10 };
        /// <summary> 作業ID実行要求応答 </summary>
        public static byte[] RECV_COMMAND_WORK_ID_EXECUTE = new byte[4] { 0x05, 0x00, 0x00, 0x10 };
        /// <summary> 作業ID変更要求応答 </summary>
        public static byte[] RECV_COMMAND_WORK_ID_CHANGE = new byte[4] { 0x06, 0x00, 0x00, 0x10 };
        /// <summary> 外部IO入力要求応答 </summary>
        public static byte[] RECV_COMMAND_IO_INPUT = new byte[4] { 0x07, 0x00, 0x00, 0x10 };
        /// <summary> 状態確認要求応答 </summary>
        public static byte[] RECV_COMMAND_STATUS_CHECK = new byte[4] { 0x08, 0x00, 0x00, 0x10 };
        /// <summary> シャットダウン実行要求応答 </summary>
        public static byte[] RECV_COMMAND_SHUTDOWN_EXECUTE = new byte[4] { 0x09, 0x00, 0x00, 0x10 };
        /// <summary> 再起動実行要求応答 </summary>
        public static byte[] RECV_COMMAND_REBOOT = new byte[4] { 0x0A, 0x00, 0x00, 0x10 };
        /// <summary> ファイル保存先変更要求応答 </summary>
        public static byte[] RECV_COMMAND_FILE_SAVE_PATH_CHANGE = new byte[4] { 0x0B, 0x00, 0x02, 0x10 };
        /// <summary> データ取得要求応答 </summary>
        public static byte[] RECV_COMMAND_DATA_GET = new byte[4] { 0x0C, 0x00, 0x02, 0x10 };
        /// <summary> データ設定要求応答 </summary>
        public static byte[] RECV_COMMAND_DATA_SET = new byte[4] { 0x0D, 0x00, 0x02, 0x10 };
        /// <summary> ファイルパス取得要求応答 </summary>
        public static byte[] RECV_COMMAND_FILE_PATH_GET = new byte[4] { 0x0E, 0x00, 0x02, 0x10 };
        #endregion



        //#region エラーコード
        ///// <summary> 起動通知 </summary>
        //private static byte[] ERROR_COMMAND_BOOT = new byte[2] { 0x01, 0x00 };
        ///// <summary> 作業Item完了通知(マッチング) </summary>
        //private static byte[] RECV_COMMAND_WORK_ITEM_COMP_MATCHING = new byte[4] { 0x02, 0x00, 0x01, 0x10 };
        ///// <summary> 作業Item完了通知(シリアルナンバー) </summary>
        //private static byte[] RECV_COMMAND_WORK_ITEM_COMP_SERIAL_NUMBER = new byte[4] { 0x03, 0x00, 0x01, 0x10 };
        ///// <summary> 作業Item完了通知(チェックモード) </summary>
        //private static byte[] RECV_COMMAND_WORK_ITEM_COMP_CHECK_MODE = new byte[4] { 0x04, 0x00, 0x01, 0x10 };
        ///// <summary> 作業Item完了通知(Stop) </summary>
        //private static byte[] RECV_COMMAND_WORK_ITEM_COMP_STOP = new byte[4] { 0x05, 0x00, 0x01, 0x10 };
        ///// <summary> 作業ID完了通知 </summary>
        //private static byte[] RECV_COMMAND_WORK_ID_COMP = new byte[4] { 0x08, 0x00, 0x01, 0x10 };
        ///// <summary> 作業Itemリストデータ通知 </summary>
        //private static byte[] RECV_COMMAND_WORK_ITEM_LIST_DATA = new byte[4] { 0x09, 0x00, 0x01, 0x10 };
        ///// <summary> 作業Itemリスト取得完了通知 </summary>
        //private static byte[] RECV_COMMAND_WORK_ITEM_LIST_GET_COMP = new byte[4] { 0x0B, 0x00, 0x01, 0x10 };
        ///// <summary> ログイン通知 </summary>
        //private static byte[] RECV_COMMAND_LOGIN = new byte[4] { 0x0C, 0x00, 0x01, 0x10 };
        
        //#endregion


    }

    /// <summary>
    /// SC10A 受信コマンド
    /// </summary>
    public enum SC10A_RECV_COMMAND
    {
        /// <summary> NONE </summary>
        NONE = 0,

        // ------------------------------
        // 通知
        // ------------------------------
        /// <summary> 起動通知 </summary>
        BOOT,
        /// <summary> 作業Item完了(マッチング)通知 </summary>
        WORK_ITEM_COMP_MATCHING,
        /// <summary> 作業Item完了(シリアルナンバー)通知 </summary>
        WORK_ITEM_COMP_SERIAL_NUMBER,
        /// <summary> 作業Item完了(チェックモード)通知 </summary>
        WORK_ITEM_COMP_CHECK_MODE,
        /// <summary> 作業Item完了(Stop)通知 </summary>
        WORK_ITEM_COMP_STOP,
        /// <summary> 作業ID完了通知 </summary>
        WORK_ID_COMP,
        /// <summary> 作業Itemリストデータ通知 </summary>
        WORK_ITEM_LIST_DATA,
        /// <summary> 作業Itemリスト取得完了通知 </summary>
        WORK_ITEM_LIST_GET_COMP,
        /// <summary> ログイン通知 </summary>
        LOGIN,
        /// <summary> ログアウト通知 </summary>
        LOGOUT,
        /// <summary> システム停止通知 </summary>
        SYSTEM_STOP,
        /// <summary> タイムアウト通知 </summary>
        TIMEOUT,
        /// <summary> データ取得完了通知 </summary>
        DATA_GET_COMP,
        /// <summary> データ設定準備完了通知 </summary>
        DATA_SET_READY_COMP,
        /// <summary> データ設定完了通知 </summary>
        DATA_SET_COMP,
        /// <summary> ファイルパスデータ通知 </summary>
        FILE_PATH_DATA,
        /// <summary> ファイルパスデータ取得完了通知 </summary>
        FILE_PATH_DATA_GET_COMP,

        // ------------------------------
        // 要求応答
        // ------------------------------
        /// <summary> 作業ID開始要求応答 </summary>
        WORK_ID_START,
        /// <summary> Start要求応答 </summary>
        START,
        /// <summary> Stop要求応答 </summary>
        STOP,
        /// <summary> 作業Itemリスト取得要求応答 </summary>
        WORK_ITEM_LIST_GET,
        /// <summary> 作業ID実行要求応答 </summary>
        WORK_ID_EXECUTE,
        /// <summary> 作業ID変更要求応答 </summary>
        WORK_ID_CHANGE,
        /// <summary> 外部IO入力要求応答 </summary>
        IO_INPUT,
        /// <summary> 状態確認要求応答 </summary>
        STATUS_CHECK,
        /// <summary> シャットダウン実行要求応答 </summary>
        SHUTDOWN_EXECUTE,
        /// <summary> 再起動実行要求応答 </summary>
        REBOOT,
        /// <summary> ファイル保存先変更要求応答 </summary>
        FILE_SAVE_PATH_CHANGE,
        /// <summary> データ取得要求応答 </summary>
        DATA_GET,
        /// <summary> データ設定要求応答 </summary>
        DATA_SET,
        /// <summary> ファイルパス取得要求応答 </summary>
        FILE_PATH_GET,
    }
    /// <summary>
    /// SC10A 送信コマンド
    /// </summary>
    public enum SC10A_SEND_COMMAND
    {
        /// <summary> NONE </summary>
        NONE = 0,

        // ------------------------------
        // 通知応答
        // ------------------------------
        /// <summary> 起動通知応答 </summary>
        BOOT,
        /// <summary> 作業Item完了通知応答 </summary>
        WORK_ITEM_COMP,
        /// <summary> 作業ID完了通知応答 </summary>
        WORK_ID_COMP,
        /// <summary> 作業Itemリストデータ通知応答 </summary>
        WORK_ITEM_LIST_DATA,
        /// <summary> 作業Itemリスト取得完了通知応答 </summary>
        WORK_ITEM_LIST_GET_COMP,
        /// <summary> ログイン通知応答 </summary>
        LOGIN,
        /// <summary> ログアウト通知応答 </summary>
        LOGOUT,
        /// <summary> システム停止通知応答 </summary>
        SYSTEM_STOP,
        /// <summary> タイムアウト通知応答 </summary>
        TIMEOUT,
        /// <summary> データ取得完了通知応答 </summary>
        DATA_GET_COMP,
        /// <summary> データ設定準備完了通知応答 </summary>
        DATA_SET_READY_COMP,
        /// <summary> データ設定完了通知応答 </summary>
        DATA_SET_COMP,
        /// <summary> ファイルパスデータ通知応答 </summary>
        FILE_PATH_DATA,
        /// <summary> ファイルパスデータ取得完了通知応答 </summary>
        FILE_PATH_DATA_GET_COMP,

        // ------------------------------
        // 要求
        // ------------------------------
        /// <summary> 作業ID開始要求 </summary>
        WORK_ID_START,
        /// <summary> Start要求 </summary>
        START,
        /// <summary> Stop要求 </summary>
        STOP,
        /// <summary> 作業Itemリスト取得要求 </summary>
        WORK_ITEM_LIST_GET,
        /// <summary> 作業ID実行要求 </summary>
        WORK_ID_EXECUTE,
        /// <summary> 作業ID変更要求 </summary>
        WORK_ID_CHANGE,
        /// <summary> 外部IO入力要求 </summary>
        IO_INPUT,
        /// <summary> 状態確認要求 </summary>
        STATUS_CHECK,
        /// <summary> シャットダウン実行要求 </summary>
        SHUTDOWN_EXECUTE,
        /// <summary> 再起動実行要求 </summary>
        REBOOT,
        /// <summary> ファイル保存先変更要求 </summary>
        FILE_SAVE_PATH_CHANGE,
        /// <summary> データ取得要求 </summary>
        DATA_GET,
        /// <summary> データ設定要求 </summary>
        DATA_SET,
        /// <summary> ファイルパス取得要求 </summary>
        FILE_PATH_GET,
    }


    /// <summary>
    /// SC10A 結果コマンド
    /// </summary>
    public enum SC10A_RESULT
    {
        /// <summary> 通常動作 </summary>
        NORMAL = 0,
        /// <summary> 再実施 </summary>
        RETRY = 1,
        /// <summary> 強制作業ID終了 </summary>
        CANCEL = 2,
    }

}
