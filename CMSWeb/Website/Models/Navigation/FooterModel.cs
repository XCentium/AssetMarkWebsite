using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcAssetmark.Models.Navigation
{
    public class FooterModel
    {
        public string Name { get; set; }

        public string Url { get; set; }

        public bool Selected { get; set; }
    }
}