using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DocumentEntities
{
    [XmlRoot("asset", ElementName = "asset")]
    public class Asset
    {
        [XmlAttribute("id")]
        public string Id { get; set; }

        //[XmlAttribute("type")]
        //public AssetType Type { get; set; }

        [XmlElement("title")]
        public string Title { get; set; }

        [XmlElement("description")]
        public string Description { get; set; }

        [XmlElement("client-approved")]
        public bool ClientApproved { get; set; }

        [XmlElement("can-distribute")]
        public bool CanDistribute { get; set; }

        [XmlElement("available-orientation")]
        public AvailableOrientation AvailableOrientation { get; set; }

        [XmlElement("url")]
        public string URL { get; set; }

        [XmlArray("files")]
        [XmlArrayItem("file")]
        public List<RCFile> Files { get; set; }

        [XmlElement("image")]
        public Image Image { get; set; }
    }

    public enum Orientation
    {
        [XmlEnum(Name = "portrait")]
        Portrait,
        [XmlEnum(Name = "landscape")]
        Landscape
    }

    public enum AssetType
    {
        [XmlEnum(Name = "document")]
        Document,
        [XmlEnum(Name = "image")]
        Image,
        [XmlEnum(Name = "video")]
        Video,
        [XmlEnum(Name = "audio")]
        Audio,
        [XmlEnum(Name = "html")]
        HTML,
        [XmlEnum(Name = "resource")]
        ViewResource
    }
}

