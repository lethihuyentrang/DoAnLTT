using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QLBQA.Models
{
    public class ImageDetail
    {
        public int ImageID { get; set; }


        public string Path { get; set; }

        public string Description { get; set; }

        public int ProductID { get; set; }

        public byte[] ImageData { get; set; }
    }
}