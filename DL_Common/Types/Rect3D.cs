using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DL_CommonLibrary
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class Rect_3D
    {
        public double Width = 0;
        public double Height = 0;
        public double Depth = 0;

        public double X = 0;
        public double Y = 0;
        public double Z = 0;
        public double T = 0;
        public Rect_3D()
        {
            this.Width      = 0;
            this.Height     = 0;
            this.Depth      = 0;
            this.X          = 0;
            this.Y          = 0;
            this.Z          = 0;
            this.T          = 0;
        }

        public Rect_3D(double w, double h, double depth)
        {
            this.Width  = w;
            this.Height = h;
            this.Depth  = depth;
            this.X          = 0;
            this.Y          = 0;
            this.Z          = 0;
            this.T          = 0;
        }
        public Rect_3D(double w, double h, double depth, double x, double y)
        {
            this.Width  = w;
            this.Height = h;
            this.Depth  = depth;
            this.X      = x;
            this.Y      = y;
            this.Z      = 0;
            this.T      = 0;
        }
        public Rect_3D(double w, double h, double depth, double x, double y, double z)
        {
            this.Width = w;
            this.Height = h;
            this.Depth = depth;
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.T = 0;
        }
        public Rect_3D(double w, double h, double depth, double x, double y, double z, double t)
        {
            this.Width  = w;
            this.Height = h;
            this.Depth  = depth;
            this.X      = x;
            this.Y      = y;
            this.Z      = z;
            this.T      = t;
        }
        public Rect_3D(string pos)
        {
            pos = pos.Replace("(", "");
            pos = pos.Replace(")", "");

            string[] col = pos.Split(',');
            if (col.Length >= 1) Width  = double.Parse(col[0]);
            if (col.Length >= 2) Height = double.Parse(col[1]);
            if (col.Length >= 3) Depth  = double.Parse(col[2]);
            if (col.Length >= 4) X      = double.Parse(col[3]);
            if (col.Length >= 5) Y      = double.Parse(col[4]);
            if (col.Length >= 6) Z      = double.Parse(col[5]);
            if (col.Length >= 7) T      = double.Parse(col[6]);
        }


        public static Rect_3D operator +(Rect_3D a, Rect_3D b)
        {
            return new Rect_3D(a.Width + b.Width, a.Height + b.Height, a.Depth + b.Depth, a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.T + b.T);
        }

        public static Rect_3D operator -(Rect_3D a, Rect_3D b)
        {
            return new Rect_3D(a.Width - b.Width, a.Height - b.Height, a.Depth - b.Depth, a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.T - b.T);
        }

        public override string ToString()
        {
            return "(" + Width + "," + Height + "," + Depth + "," + X + "," + Y +"," + Z + "," + T + ")";
        }


        public double Left
        {
            get { return X; }
            set { X = value; }

        }
        public double Right
        {
            get { return X + Width; }
            set
            {
                Width = value - X;
            }
        }
        public double Top
        {
            get { return Y; }
            set { Y = value; }
        }
        public double Bottom
        {
            get { return Y + Height; }
            set
            {
                Height = value - Y;
            }
        }


        /// <summary>
        /// コピーを返す
        /// </summary>
        /// <returns></returns>
        public Rect_3D GetCopy()
        {
            return new Rect_3D(Width, Height, Depth, X, Y, Z, T);
        }


        /// <summary>
        /// 2DのpointDbl配列へ変換  ※Zは含まれない
        /// </summary>
        /// <returns></returns>
        public PointDbl[] ToPointDbl()
        {
            PointDbl[] points = new PointDbl[4];
            for (int i = 0; i < points.Length; i++) points[i] = new PointDbl();
            points[0].X = this.X;
            points[0].Y = this.Y;
            points[1].X = this.X + Width;
            points[1].Y = this.Y;
            points[2].X = this.X + Width;
            points[2].Y = this.Y + Height;
            points[3].X = this.X;
            points[3].Y = this.Y + Height;
            return points;


        }

        /// <summary>
        /// Rect2Dに変換
        /// </summary>
        /// <returns></returns>
        public Rect2D ToRect2D()
        {
            Rect2D rect = new Rect2D();
            rect.Width = this.Width;
            rect.Height = this.Height;
            rect.X = this.X;
            rect.Y = this.Y;
            return rect;
        }
    }
}
