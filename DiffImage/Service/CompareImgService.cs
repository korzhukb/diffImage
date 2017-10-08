using DiffImage.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace DiffImage.Service
{
    public class CompareImgService
    {
        public static string imgSrc = AppDomain.CurrentDomain.BaseDirectory + @"\Image\diffimg.png";
        
        /// <summary>
        /// Determines whether is valid content type.
        /// </summary>
        /// <param name="contentType">Type of the content.</param>
        /// <returns></returns>
        public static bool isValidContentType(string contentType)
        {
            return contentType.Equals("image/png") || contentType.Equals("image/gif") ||
                contentType.Equals("image/jpeg") || contentType.Equals("image/jpg");
        }

        /// <summary>
        /// Gets the different.
        /// </summary>
        /// <param name="pic1">The pic1.</param>
        /// <param name="pic2">The pic2.</param>
        /// <returns></returns>
        public static string GetDifferent(Bitmap pic1, Bitmap pic2)
        {
            List<PointCoordinateList> pointCoordinateList = new List<PointCoordinateList>();
            try
            {
                List<PointCoordinateList> pointsList = new List<PointCoordinateList>();
                Bitmap diffPic = new Bitmap(pic1.Width, pic1.Height);

                int maxDiffPercent = 10;

                for (int i = 0; i < pic1.Width; i++)
                {
                    for (int j = 0; j < pic1.Height; j++)
                    {
                        if (DifferentRGB(pic1.GetPixel(i, j), pic2.GetPixel(i, j)) > maxDiffPercent)
                        {
                            AddNewPoint(pointsList, i, j);
                            diffPic.SetPixel(i, j, pic2.GetPixel(i, j));
                        }
                        else
                        {
                            diffPic.SetPixel(i, j, pic1.GetPixel(i, j));
                        }
                    }
                }

                CreateRectangle(diffPic, pointsList);

                diffPic.Save(imgSrc);
            }
            catch (Exception ex)
            {
                return "Error";
            }

            return "Success";
        }

        /// <summary>
        /// Calculating of percentage difference
        /// </summary>
        /// <param name="picColor1">The pic color1.</param>
        /// <param name="picColor2">The pic color2.</param>
        /// <returns></returns>
        public static Double DifferentRGB(Color picColor1, Color picColor2)
        {
            double diff = 0.0;

            diff += (Double)Math.Abs(picColor1.R - picColor2.R) / 255;
            diff += (Double)Math.Abs(picColor1.G - picColor2.G) / 255;
            diff += (Double)Math.Abs(picColor1.B - picColor2.B) / 255;

            return diff / 3 * 100;
        }

        /// <summary>
        /// Drawing the rectangle.
        /// </summary>
        /// <param name="img">The img.</param>
        /// <param name="diffList">The difference list.</param>
        /// <returns></returns>
        public static Bitmap CreateRectangle(Bitmap img, List<PointCoordinateList> diffList)
        {
            foreach (var points in diffList)
            {
                int widthRectangleBorder = 2;
                PointModal minPoint = new PointModal() { X = points.CoordinateList.OrderBy(item => item.X).FirstOrDefault().X, Y = points.CoordinateList.OrderBy(item => item.Y).FirstOrDefault().Y }; //points.CoordinateList.OrderBy(item => item.X).ThenByDescending(item => item.Y).First();
                PointModal maxPoint = new PointModal() { X = points.CoordinateList.OrderBy(item => item.X).LastOrDefault().X, Y = points.CoordinateList.OrderBy(item => item.Y).LastOrDefault().Y };// points.CoordinateList.OrderBy(item => item.X).ThenByDescending(item => item.Y).Last();

                int lengthWidth = maxPoint.X - minPoint.X;
                int lengthHeight = maxPoint.Y - minPoint.Y;

                lengthWidth = CalculationMaxValue(minPoint.X, lengthWidth, img.Width);
                lengthHeight = CalculationMaxValue(minPoint.Y, lengthHeight, img.Height);

                Graphics gr = Graphics.FromImage(img);
                Rectangle rectangle = new Rectangle(minPoint.X, minPoint.Y, lengthWidth, lengthHeight);
                gr.DrawRectangle(new Pen(Color.Red, widthRectangleBorder), rectangle);

            }
            return img;
        }

        /// <summary>
        /// Calculations the maximum value.
        /// </summary>
        /// <param name="minPoint">The minimum point.</param>
        /// <param name="length">The length.</param>
        /// <param name="maxLength">The maximum length.</param>
        /// <returns></returns>
        public static int CalculationMaxValue(int minPoint, int length, int maxLength)
        {
            int result = 0;

            result = (minPoint + length) > maxLength ? maxLength : length;

            return result;
        }

        /// <summary>
        /// Adds the new difference point.
        /// </summary>
        /// <param name="pointsList">The points list.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        private static void AddNewPoint(List<PointCoordinateList> pointsList, int x, int y)
        {
            int maxPixelRange = 100;
            int index = -1;
            index = pointsList.FindIndex(elem => elem.CoordinateList.Where(point => Math.Abs(point.X - x) < maxPixelRange && Math.Abs(point.Y - y) < maxPixelRange).FirstOrDefault() != null);

            if (index == -1)
            {
                pointsList.Add(new PointCoordinateList() { CoordinateList = new List<PointModal>() { new PointModal() { X = x, Y = y } } });
            }
            else
            {
                pointsList[index].CoordinateList.Add(new PointModal() { X = x, Y = y });
            }
        }

        /// <summary>
        /// generate url with data 
        /// </summary>
        /// <returns></returns>
        public static string ImageDataURL()
        {
            string path = CompareImgService.imgSrc;
            byte[] imageByteData = System.IO.File.ReadAllBytes(path);
            string imageBase64Data = Convert.ToBase64String(imageByteData);
            string imageDataURL = string.Format("data:image/png;base64,{0}", imageBase64Data);

            return imageDataURL;
        }
    }
}