using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DiffImage.Models
{
    public class PointCoordinateList
    {
        public List<PointModal> CoordinateList { get; set; }

        public PointCoordinateList()
        {
            CoordinateList = new List<PointModal>();
        }
    }
}