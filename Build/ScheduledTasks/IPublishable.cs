using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genworth.SitecoreExt.ScheduledTasks
{
    public interface IPublishable
    {
        bool PublishSite();
    }
}
