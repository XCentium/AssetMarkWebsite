using Sitecore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Genworth.SitecoreExt.ScheduledTasks
{
    public abstract class DailyTimeRangeTaskBase
    {
        public string StartUpWindow { get; set; }
        public string EndingWindow { get; set; }

        private bool isValidDateRange;
        protected bool IsValidDateRange
        {
            get
            {
                ValidateTimeWindow();
                return isValidDateRange;
            }
        }

        protected DateTime startupWindow;
        protected DateTime endingWindow;
        private bool isWindowValid;

        private string className = "Genworth.SitecoreExt.ScheduledTasks.DailyTimeRangeTaskBase";

        public DailyTimeRangeTaskBase(string className)
        {
            this.className = className;
        }

        private void ValidateTimeWindow()
        {
            if (isWindowValid = ValidPropertyValues())
            {
                isValidDateRange = CheckTime(DateTime.Now, startupWindow, endingWindow);
            }
        }

        private bool ValidPropertyValues()
        {
            if (string.IsNullOrWhiteSpace(StartUpWindow))
            {
                Log.Info(string.Format("{0}: StartUpWindow setting is not set in config file.", className), this);
                return false;
            }

            if (string.IsNullOrWhiteSpace(EndingWindow))
            {
                Log.Info(string.Format("{0}: EndingWindow setting is not set in config file.", className), this);
                return false;
            }

            if (!(DateTime.TryParse(StartUpWindow, out startupWindow) && DateTime.TryParse(EndingWindow, out endingWindow)))
            {
                Log.Info(string.Format("Genworth.SitecoreExt.ScheduledTasks.TimeRangeTaskBase: StartUpWindow '{0}' and EndingWindow '{1}' settings must be in the following format 00:00:00.", StartUpWindow, EndingWindow), this);
                return false;
            }

            return true;
        }

        private bool CheckTime(DateTime time, DateTime after, DateTime before)
        {
            return ((time >= after) && (time <= before));
        }
    }
}
