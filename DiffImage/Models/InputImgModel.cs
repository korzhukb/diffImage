using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace DiffImage.Models
{
    public class InputImgModel
    {
        public FileInfo File1 { get; set; }
        public FileInfo File2 { get; set; }
    }
}