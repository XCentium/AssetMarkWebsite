using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace ServerLogic.WCF
{
    public class ResilientChannelFactory<T> : IDisposable where T : class
    {
        private ChannelFactory<T> oChannelFactory;
        private T oService;
        private Action<T> oInitializeAction;
        private string endpointConfiguration;
        private bool disposed;

        public T Service
        {
            get
            {
                if (oService == null)
                {
                    //create the service
                    oService = oChannelFactory.CreateChannel();

                    //if the channel faults, kill the service
                    ((IClientChannel)oService).Faulted += oChannel_Faulted;

                    //do we have an initialization action?
                    if (oInitializeAction != null)
                    {
                        //perform initialization
                        oInitializeAction(oService);
                    }
                }

                //return the service
                return oService;
            }
        }

        public ResilientChannelFactory(string sEndpointConfigurationName) : this(sEndpointConfigurationName, null) { }

        public ResilientChannelFactory(string sEndpointConfigurationName, Action<T> oInitializeAction)
        {
            //set the initialization action
            this.oInitializeAction = oInitializeAction;
            this.endpointConfiguration = sEndpointConfigurationName;
            //create a channel factory
            InitializeFactory();
        }

        private void InitializeFactory()
        {
            oChannelFactory = new ChannelFactory<T>(endpointConfiguration);
            oChannelFactory.Faulted += oChannelFactory_Faulted;
        }

        private void oChannelFactory_Faulted(object sender, EventArgs e)
        {
            Sitecore.Diagnostics.Log.Error("ResilientChannelFactory.oChannelFactory_Faulted: Channel factory object faulted", this);
            Dispose();
            InitializeFactory();
        }

        private void oChannel_Faulted(object sender, EventArgs e)
        {
            Sitecore.Diagnostics.Log.Error("ResilientChannelFactory.oChannel_Faulted: Service channel object faulted", this);
            AbortService();
            //service is faulted, kill the service
            oService = null;
        }

        private void AbortService()
        {
            if (oService != null)
            {
                ((IClientChannel)oService).Abort();
            }
        }

        private void CloseService()
        {
            if (oService != null)
            {
                ((IClientChannel)oService).Close();
            }
        }

        private void DisposeService()
        {
            try
            {
                CloseService();
            }
            catch (Exception ex)
            {
                Sitecore.Diagnostics.Log.Error("ResilientChannelFactory.DisposeService: Calling CloseService method threw exception", ex, this);
                AbortService();
            }
        }

        private void Close()
        {
            if (oChannelFactory != null)
            {
                oChannelFactory.Faulted -= oChannelFactory_Faulted;
                oChannelFactory.Close();
            }
        }

        private void Abort()
        {
            if (oChannelFactory != null)
            {
                oChannelFactory.Abort();
            }
        }

        public void Dispose()
        {
            if (!this.disposed)
            {
                Dispose(true);
                GC.SuppressFinalize(this);
                this.disposed = true;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                DisposeService();
                try
                {
                    Close();
                }
                catch (Exception ex)
                {
                    Sitecore.Diagnostics.Log.Error("ResilientChannelFactory.Dispose(bool): Calling Close method threw exception", ex, this);
                    Abort();
                }
            }
        }
    }
}
