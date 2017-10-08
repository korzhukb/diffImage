using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using DiffImage.Models;
using DiffImage.Service;

namespace DiffImage.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
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

            if (!CompareImgService.isValidContentType(File1.ContentType) || !CompareImgService.isValidContentType(File2.ContentType))
            {
                ViewBag.Error = "Only JPEG, PNG, JPG and GIF !";
                return View("index");
            }

            Bitmap pic1 = new Bitmap(File1.InputStream);
            Bitmap pic2 = new Bitmap(File2.InputStream);

            resMessage = CompareImgService.GetDifferent(pic1, pic2);

            if (resMessage == "Error")
            {
                ViewBag.Error = resMessage + " !";
            }
            else
            {
                ViewBag.Success = resMessage + " !";
            }

            ViewBag.ImgSrc = CompareImgService.ImageDataURL();
            
            return View("index");
        }
    }
}