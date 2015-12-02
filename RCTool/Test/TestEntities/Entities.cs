using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace TestEntities
{
    [DataContract]
    public class PostResponse
    {
        [DataMember]
        public string ID { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string URL { get; set; }
    }

    [DataContract]
    public class PostRequest
    {
        [DataMember]
        public bool All { get; set; }
    }
}
