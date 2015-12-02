using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace ServerLogic.SitecoreExt.Data.Remote.SqlServer.Server.Implementations
{
    public class GenericTimeableObject<T>
    {
        private Stopwatch oStopWatch;
        private T oObject;
        public GenericTimeableObject(T oData)
        {
            oObject = oData;
            oStopWatch = Stopwatch.StartNew();

        }
        public long LifeTime
        {
            get
            {
                return oStopWatch.ElapsedMilliseconds;
            }
        }
        public T Data
        {
            get
            {
                oStopWatch.Reset();
                oStopWatch.Start();
                return oObject;
            }
        }
        public void Reset()
        {
            oStopWatch.Reset();
            oStopWatch.Start();
        }
        public bool IsExpired(long lMilliseconds)
        {
            return oStopWatch.ElapsedMilliseconds > lMilliseconds;
        }
        public static implicit operator T(GenericTimeableObject<T> oData)
        {
            return oData.Data;
        }
        public override string ToString()
        {
            return oStopWatch.ElapsedMilliseconds.ToString();
        }

    }
}
