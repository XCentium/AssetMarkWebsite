using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genworth.SitecoreExt.ScheduledTasks
{
    public interface IImportable
    {
        bool GetFile(string path, out string fileName);

        bool ImportFile(string path, string fileName);

        string GenerateDocTitle(string fileName);
    }
}
