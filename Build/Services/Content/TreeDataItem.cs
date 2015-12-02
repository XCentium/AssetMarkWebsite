using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Genworth.SitecoreExt.Services.Content
{
    [DataContract]
    public class TreeDataItem
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "text")]
        public string DisplayName { get; set; }

        [DataMember(Name = "icon")]
        public string IconUrl { get; set; }

        [DataMember(Name = "children")]
        public List<TreeDataItem> Children { get; set; }

        [DataMember(Name = "state")]
        public State State { get; set; }

        [DataMember(Name = "path")]
        public string Path { get; set; }

        
    }

    [DataContract]
    public class State
    {
        [DataMember(Name = "disabled")]
        public bool Disabled { get; set; }

        [DataMember(Name = "selected")]
        public bool Selected { get; set; }

        public State(bool disabled, bool selected)
        {
            this.Disabled = disabled;
            this.Selected = selected;
        }
    }
}
