using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using DL_CommonLibrary;
using ErrorCodeDefine;


namespace DL_Logger
{
    /// <summary>
    /// ロガー
    /// ※Staticなのでアプリケーション単位で共有されるログクラス
    /// ※別アプリケーションの場合でもフォルダが同一であれば
    /// 　同一ファイルに出力されます
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// ベースフォルダパス
        /// </summary>
        private static string _baseDirPath = "";
        /// <summary>
        /// システムログ
        /// 全ログ集約
        /// </summary>
        private static LoggingModule _systemLog = null;
        /// <summary>
        /// 履歴用ログ
        /// </summary>
        private static LoggingModule _historyLog = null;
        /// <summary>
        /// アラーム履歴ログ
        /// </summary>
        private static LoggingModule _alarmLog = null;
        /// <summary>
        /// 操作ログ
        /// </summary>
        private static LoggingModule _opeLog = null;




        /// <summary>
        /// システムログ ファイル名
        /// </summary>
        public static string systemlogFileName = "System.log";
        /// <summary>
        /// 履歴用ログ ファイル名
        /// </summary>
        public static string historylogFileName = "History.log";
        /// <summary>
        /// アラーム履歴ログ ファイル名
        /// </summary>
        public static string alarmlogFileName = "Alarm.log";
        /// <summary>
        /// 操作ログ ファイル名
        /// </summary>
        public static string operationlogFileName = "Operation.log";

        /// <summary>
        /// Systemログフォルダパス
        /// </summary>
        public static string SystemLogPath
        {
            get { return System.IO.Path.Combine(_baseDirPath, "System"); }
        }
        /// <summary>
        /// Historyログフォルダパス
        /// </summary>
        public static string HistoryLogPath
        {
            get { return System.IO.Path.Combine(_baseDirPath, "History"); }
        }
        /// <summary>
        /// Alarmログフォルダパス
        /// </summary>
        public static string AlarmLogPath
        {
            get { return System.IO.Path.Combine(_baseDirPath, "Alarm"); }
        }
        /// <summary>
        /// Operationログフォルダパス
        /// </summary>
        public static string OperationLogPath
        {
            get { return System.IO.Path.Combine(_baseDirPath, "Operation"); }
        }

        /// <summary>
        /// オープン
        /// </summary>
        /// <returns></returns>
        public static UInt32 Open(string baseDir, string title, int LogMaxFileCount, int LogMaxLineCount)
        {
            UInt32 rc = 0;
            bool ret = false;
            try
            {
                _baseDirPath = System.IO.Path.GetFullPath(baseDir);

                // フォルダ作成
                //FileIo.CreateDir(_baseDirPath);
                //FileIo.CreateDir(SystemLogPath);
                //FileIo.CreateDir(AlarmLogPath);
                //FileIo.CreateDir(HistoryLogPath);
                //FileIo.CreateDir(OperationLogPath);

                // ------------------------------
                // ログオープン
                // ------------------------------
                if (_systemLog == null)
                    _systemLog = new LoggingModule();
                ret = _systemLog.Open(baseDir, systemlogFileName, LogMaxFileCount, LogMaxLineCount);
                //ret = _systemLog.Open(SystemLogPath, systemlogFileName, LogMaxFileCount, LogMaxLineCount);
                //FileIo.DeleteLogFile(SystemLogPath, systemlogFileName, DateTime.Now.AddDays(-10));   // 10日以上前のログは削除
                if (!ret)
                {
                    Dialogs.ShowInformationMessage("システムログをオープンできませんでした", title, System.Drawing.SystemIcons.Error);
                    rc = (UInt32)ErrorCodeList.FILE_CREATE_ERROR;
                }

                if (_alarmLog == null)
                    _alarmLog = new LoggingModule();
                ret = _alarmLog.Open(baseDir, alarmlogFileName, LogMaxFileCount, LogMaxLineCount);
                //ret = _alarmLog.Open(AlarmLogPath, alarmlogFileName, LogMaxFileCount, LogMaxLineCount);
                //FileIo.DeleteLogFile(AlarmLogPath, alarmlogFileName, DateTime.Now.AddDays(-10));    // 10日上前のログは削除
                if (!ret)
                {
                    Dialogs.ShowInformationMessage("アラームログをオープンできませんでした", title, System.Drawing.SystemIcons.Error);
                    rc = (UInt32)ErrorCodeList.FILE_CREATE_ERROR;
                }

                if (_historyLog == null)
                    _historyLog = new LoggingModule();
                ret = _historyLog.Open(baseDir, historylogFileName, LogMaxFileCount, LogMaxLineCount);
                //ret = _historyLog.Open(HistoryLogPath, historylogFileName, LogMaxFileCount, LogMaxLineCount);
                //FileIo.DeleteLogFile(HistoryLogPath, historylogFileName, DateTime.Now.AddMonths(-1));    // 1カ月以上前のログは削除
                if (!ret)
                {
                    Dialogs.ShowInformationMessage("履歴ログをオープンできませんでした", title, System.Drawing.SystemIcons.Error);
                    rc = (UInt32)ErrorCodeList.FILE_CREATE_ERROR;
                }

                if (_opeLog == null)
                    _opeLog = new LoggingModule();
                ret = _opeLog.Open(baseDir, operationlogFileName, LogMaxFileCount, LogMaxLineCount);
                //ret = _opeLog.Open(OperationLogPath, operationlogFileName, LogMaxFileCount, LogMaxLineCount);
                //FileIo.DeleteLogFile(OperationLogPath, operationlogFileName, DateTime.Now.AddMonths(-1));    // 1カ月以上前のログは削除
                if (!ret)
                {
                    Dialogs.ShowInformationMessage("操作ログをオープンできませんでした", title, System.Drawing.SystemIcons.Error);
                    rc = (UInt32)ErrorCodeList.FILE_CREATE_ERROR;
                }

            }
            catch (Exception ex)
            {
                rc = (Int32)ErrorCodeList.EXCEPTION;
            }
            return rc;
        }

        /// <summary>
        /// クローズ
        /// </summary>
        /// <returns></returns>
        public static UInt32 Close()
        {
            UInt32 rc = 0;
            try
            {
                if (_historyLog != null) _historyLog.Close();
                if (_alarmLog != null) _alarmLog.Close();
                if (_systemLog != null) _systemLog.Close();
                if (_opeLog != null) _opeLog.Close();

                _historyLog = null;
                _alarmLog = null;
                _systemLog = null;
                _opeLog = null;

            }
            catch (Exception ex)
            {
                ErrorManager.ErrorHandler(ex);
            }
            return rc;
        }


        /// <summary>
        /// ログ書き込み
        /// </summary>
        /// <param name="type"></param>
        /// <param name="msg"></param>
        public static void WriteLog(LogType type, string msg)
        {
            // システムログ
            if (_systemLog != null)
                _systemLog.LogWrite(type, msg);

            if (type == LogType.HISTORY)
            {   // 履歴ログ
                if (_historyLog != null)
                    _historyLog.LogWrite(msg);
            }
            else if (type == LogType.ALARM)
            {   // アラーム履歴ログ
                if (_alarmLog != null)
                    _alarmLog.LogWrite(msg);
            }
            else if (type == LogType.ERROR)
            {   // アラームログ
                if (_alarmLog != null)
                    _alarmLog.LogWrite(msg);
            }
            else if (type == LogType.CONTROL)
            {   // 操作ログ
                if (_opeLog != null)
                    _opeLog.LogWrite(msg);
            }

        }

    }


    


}
