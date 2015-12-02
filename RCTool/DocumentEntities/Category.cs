using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DocumentEntities
{
    [XmlRoot("category", ElementName = "category")]
    public class Category
    {
        public Category() { }

        public Category(Category category)
        {
            Id = category.Id;
            Title = category.Title;
            Description = category.Description;
        }

        public Category Clone()
        {
            return new Category()
            {
                Id = this.Id,
                Title = this.Title,
                Description = this.Description,
                Hidden = this.Hidden
            };
        }

        [XmlAttribute("id")]
        public string Id { get; set; }

        [XmlElement("title")]
        public string Title { get; set; }

        [XmlElement("description")]
        public string Description { get; set; }

        [XmlElement("hidden")]
        public bool Hidden { get; set; }

        [XmlElement("hide-from-supporting-material")]
        public bool HideFromSupportingMaterial { get; set; }

        [XmlElement("category")]
        public List<Category> SubCategories { get; set; }

        [XmlArray("assets")]
        [XmlArrayItem("asset")]
        public List<Asset> Assets { get; set; }

    }
}
