//----------------------------------------------------------
// Copyright © 2020 DATALINK
//----------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace DL_CommonLibrary
{
    /// <summary>
    /// USBドングル ROCKEY I/F
    /// </summary>
    public static class RockeyIF
    {
        const ushort RY_FIND = 1;
        const ushort RY_FIND_NEXT = 2;
        const ushort RY_OPEN = 3;
        const ushort RY_CLOSE = 4;
        const ushort RY_READ = 5;
        const ushort RY_WRITE = 6;
        const ushort RY_RANDOM = 7;
        const ushort RY_SEED = 8;
        const ushort RY_WRITE_USERID = 9;
        const ushort RY_READ_USERID = 10;
        const ushort RY_SET_MOUDLE = 11;
        const ushort RY_CHECK_MOUDLE = 12;
        const ushort RY_WRITE_ARITHMETIC = 13;
        const ushort RY_CALCULATE1 = 14;
        const ushort RY_CALCULATE2 = 15;
        const ushort RY_CALCULATE3 = 16;
        const ushort RY_DECREASE = 17;

        [DllImport("Rockey4ND.dll")]
        static extern ushort Rockey(ushort function, out ushort handle, out uint lp1, out uint lp2,
                                    out ushort p1, out ushort p2, out ushort p3, out ushort p4, ref byte buffer);

        // ドングルID
        //private static ushort[] _ID = new ushort[4] { 0xC44C, 0xC8F8, 0x0799, 0xC43B }; // デモ用パスワード
        private static ushort[] _ID = new ushort[4] { 0x4435, 0x744b, 0xb82e, 0x7778 };

        /// <summary>
        /// ハンドル
        /// </summary>
        private static ushort[] _Handle = new ushort[32];

        /// <summary>
        /// 初期化完了フラグ
        /// </summary>
        private static bool _initialized = false;
        /// <summary>
        /// エンコーディング
        /// </summary>
        public static Encoding _fileEncoding = System.Text.Encoding.GetEncoding("shift_jis");

        /// <summary>
        /// USBドングルが挿入されているのか確認する
        /// </summary>
        public static bool IsConnected
        {
            get
            {
                byte[] buffer = new byte[1024];
                uint lp1, lp2;
                int ret = Rockey(RY_FIND, out _Handle[0], out lp1, out lp2, out _ID[0], out _ID[1], out _ID[2], out _ID[3], ref buffer[0]);

                return ret == 0;
            }
        }

        /// <summary>
        /// USBドングル パスワード確認
        /// </summary>
        /// <returns></returns>
        public static bool Open()
        {

            byte[] buffer = new byte[1024];
            uint lp1, lp2;

            // IDを取得する
            int ret = Rockey(RY_FIND, out _Handle[0], out lp1, out lp2, out _ID[0], out _ID[1], out _ID[2], out _ID[3], ref buffer[0]);

            if (STATUS_SUCCESS(ret))
                ret = Rockey(RY_OPEN, out _Handle[0], out lp1, out lp2, out _ID[0], out _ID[1], out _ID[2], out _ID[3], ref buffer[0]);

            _initialized = STATUS_SUCCESS(ret);
            return ret == 0;
        }

        /// <summary>
        /// USBドングル クローズ
        /// </summary>
        /// <returns></returns>
        public static bool Close()
        {

            byte[] buffer = new byte[1024];
            uint lp1, lp2;
            int ret = Rockey(RY_CLOSE, out _Handle[0], out lp1, out lp2, out _ID[0], out _ID[1], out _ID[2], out _ID[3], ref buffer[0]);

            _initialized = false;
            return ret == 0;
        }

        /// <summary>
        /// メモリ書き込み
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool WriteMemory(string data)
        {
            byte[] buffer = new byte[1024];
            uint lp1, lp2;
            ushort p1, p2, p3, p4;
            p1 = 0;         //offset
            p2 = 100;       //length


            byte[] buf = Misc.ConvertToByteArray(data, _fileEncoding);
            Array.Resize<byte>(ref buf, p2);
            int ret = Rockey(RY_WRITE, out _Handle[0], out lp1, out lp2, out p1, out p2, out p3, out p4, ref buf[0]);

            buffer = new byte[1024];
            ret = Rockey(RY_READ, out _Handle[0], out lp1, out lp2, out p1, out p2, out p3, out p4, ref buffer[0]);
            string ss = Misc.ConvertToString(buffer, 0, 100, _fileEncoding);
            return STATUS_SUCCESS(ret);
        }

        /// <summary>
        /// メモリ読込(ASCII)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool ReadMemory(ref string data)
        {
            byte[] buffer = new byte[1024];
            uint lp1, lp2;
            ushort p1, p2, p3, p4;

            p1 = 0;         //offset
            p2 = 100;       //length

            int ret = Rockey(RY_READ, out _Handle[0], out lp1, out lp2, out p1, out p2, out p3, out p4, ref buffer[0]);
            data = Misc.ConvertToString(buffer, 0, 100, _fileEncoding);

            return STATUS_SUCCESS(ret);
        }


        /// <summary>
        /// 異常コード確認
        /// </summary>
        /// <param name="ret"></param>
        /// <returns></returns>
        private static bool STATUS_SUCCESS(int ret)
        {
            return ret == 0;
        }

    }
}
