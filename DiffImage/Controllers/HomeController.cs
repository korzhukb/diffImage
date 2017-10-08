using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using DiffImage.Models;

namespace DiffImage.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        int MaxHeight = 0;
        int MaxWidth = 0;
        string imgSrc = AppDomain.CurrentDomain.BaseDirectory + @"\Image\diffimg.png";

        /// <summary>
        /// Gets the different.
        /// </summary>
        /// <param name="pic1">The pic1.</param>
        /// <param name="pic2">The pic2.</param>
        /// <returns></returns>
        public string GetDifferent(Bitmap pic1, Bitmap pic2)
        {
            List<PointCoordinateList> pointCoordinateList = new List<PointCoordinateList>();
            try
            {
                List<PointCoordinateList> pointsList = new List<PointCoordinateList>();
                Bitmap diffPic = new Bitmap(pic1.Width, pic1.Height);
                MaxHeight = pic1.Height > pic2.Height ? pic1.Height : pic2.Height;
                MaxWidth = pic2.Width > pic2.Width ? pic1.Width : pic2.Width;
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
        /// Determines whether is valid content type.
        /// </summary>
        /// <param name="contentType">Type of the content.</param>
        /// <returns></returns>
        private bool isValidContentType(string contentType)
        {
            return contentType.Equals("image/png") || contentType.Equals("image/gif") ||
                contentType.Equals("image/jpeg") || contentType.Equals("image/jpg");
        }

        [HttpPost]
        public ActionResult btnShowDiff(HttpPostedFileBase File1, HttpPostedFileBase File2)
        {
            string resMessage = string.Empty;
            if (File1 == null || File2 == null)
            {
                ViewBag.Error = (File1 == null && File2 == null) ? "Please add the files !" : File1 == null ? "Please add the first file !" : "Please add the second file !";
                return View("index");
            }

            if (!isValidContentType(File1.ContentType) || !isValidContentType(File2.ContentType))
            {
                ViewBag.Error = "Only JPEG, PNG, JPG and GIF !";
                return View("index");
            }

            Bitmap pic1 = new Bitmap(File1.InputStream);
            Bitmap pic2 = new Bitmap(File2.InputStream);

            resMessage = GetDifferent(pic1, pic2);

            if (resMessage == "Error")
            {
                ViewBag.Error = resMessage + " !";
            }
            else
            {
                ViewBag.Success = resMessage + " !";
            }
            
            string path = imgSrc;
            byte[] imageByteData = System.IO.File.ReadAllBytes(path);
            string imageBase64Data = Convert.ToBase64String(imageByteData);
            string imageDataURL = string.Format("data:image/png;base64,{0}", imageBase64Data);
            ViewBag.ImgSrc = imageDataURL;


            return View("index");
        }

        /// <summary>
        /// Calculating of percentage difference
        /// </summary>
        /// <param name="picColor1">The pic color1.</param>
        /// <param name="picColor2">The pic color2.</param>
        /// <returns></returns>
        private Double DifferentRGB(Color picColor1, Color picColor2)
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
        public Bitmap CreateRectangle(Bitmap img, List<PointCoordinateList> diffList)
        {
            foreach (var points in diffList)
            {
                int widthRectangleBorder = 2;
                PointModal minPoint = new PointModal() { X = points.CoordinateList.OrderBy(item => item.X).FirstOrDefault().X, Y = points.CoordinateList.OrderBy(item => item.Y).FirstOrDefault().Y }; //points.CoordinateList.OrderBy(item => item.X).ThenByDescending(item => item.Y).First();
                PointModal maxPoint = new PointModal() { X = points.CoordinateList.OrderBy(item => item.X).LastOrDefault().X, Y = points.CoordinateList.OrderBy(item => item.Y).LastOrDefault().Y };// points.CoordinateList.OrderBy(item => item.X).ThenByDescending(item => item.Y).Last();

                int lengthWidth = maxPoint.X - minPoint.X;
                int lengthHeight = maxPoint.Y - minPoint.Y;

                lengthWidth = CalculationMaxValue(minPoint.X, lengthWidth, MaxWidth);
                lengthHeight = CalculationMaxValue(minPoint.Y, lengthHeight, MaxHeight);

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
        private int CalculationMaxValue(int minPoint, int length, int maxLength)
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
        private void AddNewPoint(List<PointCoordinateList> pointsList, int x, int y)
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
    }
}