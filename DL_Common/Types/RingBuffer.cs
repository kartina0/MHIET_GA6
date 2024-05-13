using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DL_CommonLibrary
{
    /// <summary>
    /// リングバッファ
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RingBuffer<T>
    {
        /// <summary>
        /// バッファサイズ
        /// </summary>
        private int _size;
        /// <summary>
        /// バッファ
        /// </summary>
        private T[] _buffer;
        /// <summary>
        /// 書込位置
        /// </summary>
        private int _writeIndex = -1;
        /// <summary>
        /// 読込位置
        /// </summary>
        private int _readIndex = -1;
        /// <summary>
        /// バッファフル
        /// </summary>
        private bool _isFull = false;

        /// <summary>
        /// バッファ初期状態
        /// 未使用時はtrue
        /// </summary>
        private bool _isNotWrite = true;

        /// <summary>
        /// バッファ初期状態
        /// 未使用時はtrue
        /// </summary>
        private bool _isNotRead = true;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="size"></param>
        public RingBuffer(int size)
        {
            _size = size;
            _buffer = new T[size];
        }

        /// <summary>
        /// バッファ古状態か取得する
        /// </summary>
        public bool IsFull
        { get { return _isFull; } }

        /// <summary>
        /// バッファ残数
        /// </summary>
        public int Remain
        {
            get
            {
                int remain = 0;

                if (!_isFull)
                {
                    // 空の状態
                    if (_readIndex == _writeIndex)
                        remain = _size;
                    else if (_writeIndex > _readIndex)
                        remain = _size - _writeIndex + _readIndex;
                    else if (_writeIndex < _readIndex)
                        remain = _readIndex - _writeIndex;
                }
                return remain;
            }

        }

        /// <summary>
        /// データ数
        /// </summary>
        public int Count()
        {
            int cnt = 0;
            if (_isNotWrite)
                cnt = 0;
            else if (_writeIndex > _readIndex)
                cnt = _writeIndex - _readIndex;
            else if (_writeIndex < _readIndex)
                cnt = _size - _readIndex + _writeIndex;
            else
                cnt = 0;

            return cnt;
        }
        /// <summary>
        /// クリア
        /// </summary>
        /// <returns></returns>
        public void Clear()
        {
            _isFull = false;
            _readIndex = 0;
            _writeIndex = 0;
        }

        public void Add(T value)

        {
            if (_isFull) return;


            int curIndex = 0;

            if (!_isNotWrite)
                curIndex = _writeIndex + 1;

            if (curIndex >= _size)
                curIndex = 0;

            if (!_isNotWrite && curIndex == _readIndex)
                _isFull = true;

            // 書込み
            if (!_isFull)
            {
                _buffer[curIndex] = value;
                _writeIndex = curIndex;
            }
            _isNotWrite = false;
        }

        public bool Get(ref T value)
        {
            if (_isNotWrite)
                return false;

            int curIndex = 0;
            if (!_isNotRead)
                curIndex = _readIndex + 1;

            if (curIndex >= _size) curIndex = 0;
            value = _buffer[curIndex];

            if (_isFull) _isFull = false;
            _readIndex = curIndex;
            _isNotRead = false;

            return true;
        }

    }
}
