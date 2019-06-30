using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services
{
    public static class PhysiognomyService
    {
        private enum Direction
        {
            Top,
            Left,
            Right,
            Bottom
        }

        private enum HeadForms
        {
            QuiteRound,
            LittleBitRound,
            LittleBitLong,
            QuiteLong
        }
        
        public static string GetDescription(Bitmap bmp)
        {
            var points = FindCoordinates(bmp);
            var height = GetLength(points[Direction.Top], points[Direction.Bottom]);
            var width = GetLength(points[Direction.Left], points[Direction.Right]);
            var relation = width / height;

            if (relation > 0.95)
            {
                return "You have good intuition and strong sense of justice.";
            }
            else if (relation > 0.87)
            {
                return "You are very bright and lovely person.";
            }
            else if (relation > 0.74)
            {
                return "You have nice intellect and health.";
            }
            else
            {
                return "You are aristocratic and energetic person.";
            }
        }

        private static double GetLength(Point p1, Point p2)
        {
            return Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y));
        }
        
        private static Dictionary<Direction, Point> FindCoordinates(Bitmap bmp)
        {
            var headBorders = new Dictionary<Direction, Point>();
            const int minVal = 200;
            headBorders[Direction.Top] = new Point(0, 0);
            headBorders[Direction.Bottom] = new Point(0, 0);
            headBorders[Direction.Left] = new Point(0, 0);
            headBorders[Direction.Right] = new Point(0, 0);
            
            for (var i = 0; i < bmp.Height; i++)
            {
                for (var j = 0; j < bmp.Width; j++)
                {
                    var rgb = bmp.GetPixel(j, i);
                    var rgbVal = rgb.R + rgb.G + rgb.B;

                    if (rgbVal > minVal)
                    {
                        headBorders[Direction.Top] = new Point(j, i);
                        break;
                    }
                }

                if (headBorders[Direction.Top].X != 0 && headBorders[Direction.Top].Y != 0)
                {
                    break;
                }
            }
            
            for (var i = bmp.Height-1; i >= 0; i--)
            {
                for (var j = 0; j < bmp.Width; j++)
                {
                    var rgb = bmp.GetPixel(j, i);
                    var rgbVal = rgb.R + rgb.G + rgb.B;

                    if (rgbVal > minVal)
                    {
                        headBorders[Direction.Bottom] = new Point(j, i);
                        break;
                    }
                }

                if (headBorders[Direction.Bottom].X != 0 && headBorders[Direction.Bottom].Y != 0)
                {
                    break;
                }
            }
            
            for (var i = bmp.Width-1; i >= 0; i--)
            {
                for (var j = bmp.Height-1; j >= 0; j--)
                {
                    var rgb = bmp.GetPixel(i, j);
                    var rgbVal = rgb.R + rgb.G + rgb.B;

                    if (rgbVal > minVal)
                    {
                        headBorders[Direction.Right] = new Point(i, j);
                        break;
                    }
                }

                if (headBorders[Direction.Right].X != 0 && headBorders[Direction.Right].Y != 0)
                {
                    break;
                }
            }
            
            for (var i = 0; i < bmp.Width; i++)
            {
                for (var j = bmp.Height-1; j >= 0; j--)
                {
                    var rgb = bmp.GetPixel(i, j);
                    var rgbVal = rgb.R + rgb.G + rgb.B;

                    if (rgbVal > minVal)
                    {
                        headBorders[Direction.Left] = new Point(i, j);
                        break;
                    }
                }

                if (headBorders[Direction.Left].X != 0 && headBorders[Direction.Left].Y != 0)
                {
                    break;
                }
            }
            
            return headBorders;
        }
    }
}