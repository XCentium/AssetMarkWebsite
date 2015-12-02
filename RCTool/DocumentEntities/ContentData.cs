using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DocumentEntities
{
    [XmlRoot("content", ElementName = "content")]
    public class ContentData : RcToolsData
    {
        [XmlElement("update-notes")]
        public string UpdateNotes { get; set; }

        [XmlElement("categories")]
        public Category Category { get; set; }

        [XmlElement("resources")]
        public Resource Resource { get; set; }

        [XmlElement("views")]
        public View View { get; set; }

        [XmlElement("view-package")]
        public string Package { get; set; }

        [XmlArray("view-data")]
        [XmlArrayItem("data")]
        public List<ViewData> ViewData { get; set; }

        [XmlArray("view-media")]
        [XmlArrayItem("media")]
        public List<ViewMedia> ViewMedia { get; set; }

        [XmlArray("settings")]
        [XmlArrayItem("setting")]
        public List<Setting> Settings { get; set; }

        [XmlElement("menu")]
        public Menu Menu { get; set; }

        [XmlArray("scenarios")]
        [XmlArrayItem("scenario")]
        public List<Scenario> PredefinedScenarios { get; set; }
    }
}
