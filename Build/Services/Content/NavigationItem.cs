using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Genworth.SitecoreExt.Services.Content
{
    [DataContract]
    public class NavigationItem
    {
        [DataMember]
        public string Title;

        [DataMember]
        public string URL;

        [DataMember]
        public bool IsShownOnMeetingMode;

        [DataMember]
        public bool RemoteNotificationsEnabled;

        [DataMember]
        public ContentSecurity Security;
    }

    [DataContract]
    public class NotificationItem
    {
        [DataMember]
        public string Target;

        [DataMember]
        public string URL;

        [DataMember]
        public string Summary;

    }
}
