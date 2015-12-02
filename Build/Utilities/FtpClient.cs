using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Genworth.SitecoreExt.Utilities
{
    public class FtpClient
    {
        public FtpWebRequest CreateRequest(string address, string username, string password, bool enableSSL, bool usePassive, bool useBinary, string methodName)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(address);
            request.Method = methodName;
            request.Credentials = new NetworkCredential(username, password);
            request.EnableSsl = enableSSL;
            request.UsePassive = usePassive;
            request.UseBinary = useBinary;

            return request;
        }

        public void UploadFile(FtpWebRequest request, Stream stream)
        {
            Stream requestStream = null;
            try
            {
                // Copy the contents of the file to the request stream.
                requestStream = request.GetRequestStream();
                byte[] fileContents = null;
                using (var br = new BinaryReader(stream))
                {
                    fileContents = br.ReadBytes((int)stream.Length);
                }
                requestStream.Write(fileContents, 0, fileContents.Length);

                request.GetResponseAsync().ContinueWith(t =>
                {
                    var response = t.Result as FtpWebResponse;
                    Sitecore.Diagnostics.Log.Info(string.Format("Sitecore FTP request sent to: {0}, Upload File Complete, status {1}", request.RequestUri.ToString(), response.StatusDescription), this);

                    requestStream.Close();
                    response.Close();
                });
            }
            catch (Exception ex)
            {
                if (requestStream != null)
                {
                    requestStream.Close();
                }

                Sitecore.Diagnostics.Log.Error(string.Format("Sitecore FtpClient Utility threw an error while uploading a file. Request: {0}", request.RequestUri.ToString()), ex, this);
                throw ex;
            }
        }
    }
}
