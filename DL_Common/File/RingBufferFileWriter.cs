using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace DL_CommonLibrary
{
    /// <summary>
    /// RINGバッファを使用したファイル書き込みクラス
    /// 
    /// </summary>
    public class RingBufferFileWriter : IDisposable
    {
        /// <summary>
        /// エンコーディング
        /// </summary>
        private static Encoding _fileEncoding = System.Text.Encoding.GetEncoding("shift_jis");

        /// <summary>
        /// ファイル書込みスレッド インターバル[ms]
        /// </summary>
        private const int ThreadPoolingInterval = 1000;

        /// <summary>
        /// 日時フォーマット
        /// </summary>
        private string _timeStanpFormat = "yyyyMMdd HH:mm:ss.fff";

        /// <summary>
        /// ファイルヘッダー
        /// ファイルが切り替わるときに最初の行に書き込むヘッダ
        /// </summary>
        private string _header = "";

        /// <summary>
        /// フォルダ パス
        /// </summary>
        private string _dir = @".\";

        /// <summary>
        /// ファイル名(拡張子無)
        /// </summary>
        private string _fileName = "TEMP";

        /// <summary>
        /// ファイル 拡張子
        /// </summary>
        private string _extention = "log";

        /// <summary>
        /// ログ格納バッファ
        /// </summary>
        private RingBuffer<string> _buffer;

        /// <summary>
        /// ログ書込みスレッド 解除イベント
        /// </summary>
        private ManualResetEvent _release = new ManualResetEvent(false);

        /// <summary>
        /// ログ書込みスレッド 終了イベント
        /// </summary>
        private ManualResetEvent _shutDown = new ManualResetEvent(false);
        /// <summary>
        /// ファイルハンドル
        /// </summary>
        private FileStream _fileHandle = null;

        /// <summary>
        /// 現在開いているファイルパス
        /// </summary>
        private string _curFilePath = "";

        /// <summary>
        /// 現在のファイルの書き込み行数
        /// </summary>
        private int _lineCount = 0;

        /// <summary>
        /// ファイル書込みスレッドハンドル
        /// </summary>
        private Thread _hThread = null;

        /// <summary>
        /// 最大ファイル数
        /// </summary>
        public int maxFileCount { get; set; } = 100;

        /// <summary>
        /// 1ファイルあたりの最大行数
        /// </summary>
        public int maxLine { get; set; } = 10000;
        /// <summary>
        /// ファイル名書式
        /// </summary>
        public string fileNameFormat { get; set; } = "yyyyMMddHHmm";

        /// <summary>
        /// ファイルFlush時にファイルをClose/Openする
        /// ※MultiHeaderGridではファイル変更が検知できないのでTRUEにする
        /// </summary>
        public bool FlushWithFileCloseAndOpen { get; set; } = false;

        /// <summary>
        /// ファイルヘッダー
        /// ファイルが切り替わるときに最初の行に書き込むヘッダ
        /// </summary>
        public string Header
        {
            get { return _header; }
            set { _header = value; }
        }
        /// <summary>
        /// 日時フォーマット
        /// </summary>
        public string TimeStampFormat
        {
            get { return _timeStanpFormat; }
            set
            {
                if (value.IndexOf("fff") >= 0)
                    _timeStanpFormat = "yyyyMMdd HH:mm:ss.fff";
            }
        }

        /// <summary>
        /// 格納フォルダパス
        /// </summary>
        public string DirPath
        {
            get { return _dir; }
            set { _dir = value; }
        }

        /// <summary>
        /// ログファイル パス
        /// </summary>
        public string FileName
        {
            get { return _dir + "\\" + _fileName + "." + _extention; }
            set
            {
                if (value == "") return;

                // 拡張子取得(.なし)
                string ext = Path.GetExtension(value);
                ext = ext.Replace(".", "");
                if (ext != "") _extention = ext;
                // ファイル名取得
                _fileName = Path.GetFileNameWithoutExtension(value);


                // フォルダパス取得
                string dir = Path.GetDirectoryName(value);
                if (dir != "") _dir = dir;

                string path = _dir + "\\" + _fileName + "." + _extention;

                //System.Diagnostics.Debug.Print("LogFileName:" + _dir + "\\" + _fileName +_extention);

                if (path != value)
                {
                    CloseFile();
                }
            }
        }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RingBufferFileWriter()
        {
            _buffer = new RingBuffer<string>(3000);
            // ファイル書込みスレッド起動
            _hThread = new Thread(new ParameterizedThreadStart(ThreadProc));
            _hThread.Priority = ThreadPriority.AboveNormal;
            _hThread.Start();

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        }
        /// <summary>
        /// 破棄
        /// </summary>
        public void Dispose()
        {
            _shutDown.Set();
            _hThread = null;
            _buffer.Clear();
        }
        /// <summary>
        /// ログメッセージ出力
        /// 備考      : 出力メッセージには、現在の日付時刻を付加します
        ///             ファイル名が未設定，メッセージがない場合は出力処理は行いません
        ///             GUIDを指定できる機能の追加(引数setGUIDには","を付けておく必要あり)
        /// </summary>
        /// <param name="strLogMessage">ログ出力文字列</param>
        /// <param name="setGUID"></param>
        /// <returns>0 = 正常</returns>    
        public int WriteFile(string msg)
        {
            int rc = 0;
            try
            {

                string buf = "";
                string sDate = "";  // 日時
                string sBuf = "";   // メッセージ

                // ------------------------
                // 現在時刻を取得
                //if (_timeStanpFormat.IndexOf("fff") >= 0)
                //    format = "yyyyMMdd HH:mm:ss.fff";

                //return 0;
                sDate = DateTime.Now.ToString(_timeStanpFormat);
                buf = sDate + "," + msg + "\r";
                
                // ------------------------------
                // バッファFull時は少し待つ
                // ------------------------------
                if (_buffer.IsFull)
                {
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    while (true)
                    {
                        if (!_buffer.IsFull) break;
                        if (sw.ElapsedMilliseconds > 100)
                        {
                            break;
                        }
                        System.Threading.Thread.Sleep(1);
                    }
                }

                // ------------------------------
                // ログ バッファにメッセージを格納
                // ------------------------------
                if (!_buffer.IsFull)
                {
                    // バッファに格納
                    _buffer.Add(buf);
                    // ログ書込みスレッド起動
                    _release.Set();
                }
                else
                {
                    Debug.WriteLine($"Buffer OverRun : {msg}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception {MethodBase.GetCurrentMethod().Name} {ex.Message} {ex.StackTrace}");
                //rc = -1;
                rc = (int)ErrorCodeDefine.ErrorCodeList.EXCEPTION;

            }
            return rc;
        }
        /// <summary>
        /// ログ書込みスレッド
        /// </summary>
        /// <param name="arg"></param>
        private void ThreadProc(object arg)
        {
            try
            {

                while (true)
                {
                    WaitHandle[] waits = new WaitHandle[2] { _shutDown, _release };
                    // 要求待ち
                    int waitIndex = WaitHandle.WaitAny(waits, ThreadPoolingInterval);
                    //bool release = _release.WaitOne(ThreadPoolingInterval);
                    if (waitIndex == 0) break;

                    if (_buffer.Count() > 0)
                    {
                        string msg = "";
                        bool exist = _buffer.Get(ref msg);
                        //_buffer.Clear();
                        // ------------------------------
                        //  ファイルが作成されていない場合は作成する
                        if (!IsFileHandle())
                        {
                            CreateFile();
                            // ファイル数確認
                            CheckFileCount();
                        }

                        if (IsFileHandle() && msg != null && msg != "")
                        {
       
                            
                            byte[] bytBuf = _fileEncoding.GetBytes(msg);
                            int lngLength = bytBuf.Length;
                            _fileHandle.Write(bytBuf, 0, lngLength);
                            _lineCount++;

                            // ライン数が最大値を超えた場合は次ファイル
                            if (_lineCount >= maxLine)
                            {
                                // ファイル閉じる
                                CloseFile();
                                // ファイル作成
                                CreateFile();

                                // ファイル数確認
                                CheckFileCount();
                            }

                            if (_buffer.Count() > 0)
                            {
                                _release.Set();
                                Thread.Sleep(0);
                            }
                            else
                            {
                                _fileHandle.Flush(true);

                                if (FlushWithFileCloseAndOpen)
                                {
                                    CloseAndOpen();
                                }

                                _release.Reset();
                            }
                        }
                        else
                        {
                            _release.Reset();
                        }
                    }
                    else
                    {
                        if (IsFileHandle())
                            _fileHandle.Flush();
                        _release.Reset();
                    }
                }
            }
            catch (Exception ex)
            {
            }
            // ファイル閉じる
            CloseFile();
        }


        /// <summary>
        /// 機能      : ファイル作成
        /// 戻り値    : 0 = 正常
        /// </summary>
        private int CreateFile()
        {
            int rc = 0;
            try
            {

                string filePath = _dir + "\\" + GetFileName();

                //ファイル作成
                _fileHandle = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                if (IsFileHandle())
                {
                    //ファイル後端までシーク
                    _fileHandle.Position = _fileHandle.Length;
                    if (_fileHandle.Position == 0 && !string.IsNullOrEmpty(_header))
                    {
                        byte[] bytBuf = _fileEncoding.GetBytes(_header + "\r");
                        int lngLength = bytBuf.Length;
                        _fileHandle.Write(bytBuf, 0, lngLength);
                    }
                    // 現在開いているファイルパスを保持
                    _curFilePath = filePath;
                }
                else
                {
                    _curFilePath = "";
                    _fileHandle = null;
                    rc = (int)ErrorCodeDefine.ErrorCodeList.FILE_CREATE_ERROR;
                }
                _lineCount = 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception {MethodBase.GetCurrentMethod().Name} {ex.Message} {ex.StackTrace}");
                rc = (int)ErrorCodeDefine.ErrorCodeList.EXCEPTION;
            }
            return rc;
        }

        /// <summary>
        /// 機能      : ファイル閉じる
        /// 戻り値    : 0 = 正常
        /// </summary>
        private int CloseFile()
        {
            int rc = 0;
            try
            {
                if (IsFileHandle())
                {
                    _fileHandle.Close();
                    _fileHandle = null;
                }
                _lineCount = 0;
            }
            catch
            {
                _fileHandle = null;
                _lineCount = 0;
            }
            return rc;
        }

        /// <summary>
        /// 現在開いているファイルを閉じて開き直す
        /// </summary>
        /// <returns></returns>
        private int CloseAndOpen()
        {
            int rc = 0;
            if (IsFileHandle() && _curFilePath!="")
            {
                _fileHandle.Close();
                _fileHandle = new FileStream(_curFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                //ファイル後端までシーク
                _fileHandle.Position = _fileHandle.Length;
            }
            return rc;
        }

        /// <summary>
        /// 機能      : ファイル名の取得
        /// 戻り値    : 指定された書式でファイル名を取得
        /// <summary>
        private string GetFileName()
        {
            string strFileName = "";
            System.DateTime dtNow = DateTime.Now;

            // 指定した書式で日付を文字列に変換する
            if (_fileName != "")
            {
                strFileName = _fileName + "_" + DateTime.Now.ToString(fileNameFormat) + "." + _extention;
            }
            else
            {
                strFileName = _fileName + "." + _extention;
            }
            return strFileName;
        }

        /// <summary>
        /// 指定したファイルの行数を取得する
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private int GetFileLineCount(string filePath)
        {
            int lineCount = 0;
            try
            {
                if (File.Exists(filePath))
                {
                    var s = File.ReadLines(filePath);
                    lineCount = s.Count();
                }
            }
            catch { lineCount = 0; }
            return lineCount;
        }

        /// <summary>
        /// 機能      : ファイル数のチェック
        /// 機能説明  : ファイル格納フォルダ内のログファイル数をチェックします
        ///             最大ファイル数を超えている場合には、タイムスタンプの
        ///             古いファイルを削除します
        /// </summary>
        private void CheckFileCount()
        {
            // ファイルを更新日の古いもの順で取得する
            string[] files = GetFileList(_dir, _fileName + "*", _extention);
            int deletFileCount = files.Length - maxFileCount;

            // 古いファイルを削除する
            if (deletFileCount > 0)
            {
                for (int i = 0; i < deletFileCount; i++)
                {
                    string filePath = _dir + "\\" + files[i];
                    File.Delete(filePath);
                }
            }
        }

        /// <summary>
        /// 指定フォルダ内のファイル名を取得
        /// </summary>
        /// <param name="dirName"></param>
        /// <param name="fileName"></param>
        /// <param name="extention"></param>
        /// <param name="sortType"></param>
        /// <returns></returns>
        private string[] GetFileList(string dirName, string fileName, string extention)
        {
            if (extention == "") extention = "*";
            if (fileName == "") fileName = "*";
            if (!System.IO.Directory.Exists(dirName)) return null;
            string[] list = System.IO.Directory.GetFiles(dirName, string.Format("{0}.{1}", fileName, extention));
            string[] result = new string[list.Length];
            List<System.IO.FileInfo> fileInfo = new List<System.IO.FileInfo>();
            int n = 0;

            foreach (string fname in list)
            {
                fileInfo.Add(new System.IO.FileInfo(fname));
            }

            System.IO.FileInfo[] infos = fileInfo.ToArray();

            Array.Sort(infos, delegate (System.IO.FileInfo f1, System.IO.FileInfo f2)
            {
                //return -f1.Name.CompareTo(f2.Name); // ファイル名を降順で取得
                //return f1.Name.CompareTo(f2.Name); // ファイル名を昇順で取得
                //return -f1.LastWriteTime.CompareTo(f2.LastWriteTime); // ファイル名を更新日時降順で取得
                return f1.LastWriteTime.CompareTo(f2.LastWriteTime); // ファイル名を更新日時昇順で取得
            }
            );

            for (int i = 0; i < infos.Length; i++)
                result[i] = infos[i].Name;
            return result;
        }

        /// <summary>
        /// 指定したFileStreamが正常か確認
        /// </summary>
        /// <param name="fs"></param>
        /// <returns></returns>
        private bool IsFileHandle()
        {
            if (_fileHandle == null) return false;
            if (!_fileHandle.CanWrite) return false;
            return true;
        }
    }
}
