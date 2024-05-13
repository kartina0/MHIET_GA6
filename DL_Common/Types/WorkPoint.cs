using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DL_CommonLibrary
{
    public class WorkPoint : RobotPos
    {
        /// <summary>
        /// ロボット位置を返す
        /// </summary>
        public RobotPos Pos
        {
            get { return this; }
            set
            {
                base.X = value.X;
                base.Y = value.Y;
                base.Z = value.Z;
                base.T = value.T;
            }
        }

    }
}
