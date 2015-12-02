using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcAssetmark.Models.Navigation
{
    public class MenuItem
    {
        public string Name { get; set; }

        public string Url { get; set; }

        public bool Selected { get; set; }
    }
}