using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;

using System.Threading.Tasks;
using System.Xml;
using TestEntities;

namespace TestWinClient
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        public void DownloadFile(string Url, string PostData, bool GZip)
        {
            // NOTE:
            // The following method will only work if the returning data from the service is a Stream object.

            HttpWebRequest Http = (HttpWebRequest)WebRequest.Create(Url);

            if (GZip)
                Http.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");

            if (!string.IsNullOrEmpty(PostData))
            {
                Http.Method = "POST";
                byte[] lbPostBuffer = Encoding.Default.GetBytes(PostData);

                Http.ContentLength = lbPostBuffer.Length;

                Stream PostStream = Http.GetRequestStream();
                PostStream.Write(lbPostBuffer, 0, lbPostBuffer.Length);
                PostStream.Close();
            }

            HttpWebResponse webResponse = (HttpWebResponse)Http.GetResponse();
            DisplayResponseSettings(webResponse);

            long contentLength = webResponse.ContentLength;
            Stream responseStream = webResponse.GetResponseStream();

            //Get XML response
            GetXml(webResponse, responseStream);
            using (MemoryStream gzippedStream = new MemoryStream())
            using (MemoryStream gzippedStream2 = new MemoryStream())
            {

                double totalGZIP = 0;
                double totalFLAT = 0;
                bool isGzip = false;

                if (webResponse.ContentEncoding.ToLower().Contains("gzip"))
                {
                    isGzip = true;
                    txtMessages.AppendText("Downloading compressed stream started at: " + DateTime.Now.ToString() + Environment.NewLine);
                    Application.DoEvents();
                    CopyStream(responseStream, gzippedStream, gzippedStream2);
                    txtMessages.AppendText("Downloading compressed stream ended at: " + DateTime.Now.ToString() + Environment.NewLine);

                    using (FileStream fs = File.OpenWrite(txtfileName.Text + ".zip"))
                    {
                        gzippedStream.Position = 0;
                        totalGZIP = CopyStream(gzippedStream, fs);
                        txtMessages.AppendText("Total bytes written from compressed stream: " + totalGZIP.ToString("N0") + Environment.NewLine);

                        DisplayHash(gzippedStream, "MD5", new MD5CryptoServiceProvider(), true);
                        //DisplayHash(gzippedStream, "SHA1", new SHA1CryptoServiceProvider(), true);
                        //DisplayHash(gzippedStream, "SHA256", new SHA256CryptoServiceProvider(), true);
                        //DisplayHash(gzippedStream, "SHA384", new SHA384CryptoServiceProvider(), true);
                        //DisplayHash(gzippedStream, "SHA512", new SHA512CryptoServiceProvider(), true);
                        //DisplayHash(gzippedStream, "HMACMD5", new HMACMD5(), true);
                        //DisplayHash(gzippedStream, "HMACSHA1", new HMACSHA1(), true);
                        //DisplayHash(gzippedStream, "HMACSHA256", new HMACSHA256(), true);
                        //DisplayHash(gzippedStream, "HMACSHA384", new HMACSHA384(), true);
                        //DisplayHash(gzippedStream, "HMACSHA512", new HMACSHA512(), true);
                    }

                    gzippedStream2.Position = 0;
                    responseStream = new GZipStream(gzippedStream2, CompressionMode.Decompress);
                }

                using (FileStream fs = File.OpenWrite(txtfileName.Text))
                {
                    if (!isGzip)
                    {
                        txtMessages.AppendText("Downloading uncompressed stream started at: " + DateTime.Now.ToString() + Environment.NewLine);
                    }

                    Application.DoEvents();
                    MemoryStream checkSumStream = new MemoryStream();
                    totalFLAT = CopyStream(responseStream, checkSumStream, fs);

                    if (!isGzip)
                    {
                        txtMessages.AppendText("Downloading uncompressed stream ended at: " + DateTime.Now.ToString() + Environment.NewLine);
                    }

                    DisplayHash(checkSumStream, "MD5", new MD5CryptoServiceProvider(), false);
                    DisplayHash(checkSumStream, "SHA1", new SHA1CryptoServiceProvider(), false);
                    DisplayHash(checkSumStream, "SHA256", new SHA256CryptoServiceProvider(), false);
                    DisplayHash(checkSumStream, "SHA384", new SHA384CryptoServiceProvider(), false);
                    DisplayHash(checkSumStream, "SHA512", new SHA512CryptoServiceProvider(), false);
                    DisplayHash(checkSumStream, "HMACMD5", new HMACMD5(), false);
                    DisplayHash(checkSumStream, "HMACSHA1", new HMACSHA1(), false);
                    DisplayHash(checkSumStream, "HMACSHA256", new HMACSHA256(), false);
                    DisplayHash(checkSumStream, "HMACSHA384", new HMACSHA384(), false);
                    DisplayHash(checkSumStream, "HMACSHA512", new HMACSHA512(), false);


                    txtMessages.AppendText("Total bytes written from flat stream: " + totalFLAT.ToString("N0") + Environment.NewLine);
                }

                if (isGzip && totalFLAT != 0)
                {
                    txtMessages.AppendText("Compression ratio %: " + (100 - ((totalGZIP / totalFLAT) * 100.0)).ToString("F") + Environment.NewLine);
                }
            }

            responseStream.Close();
            webResponse.Close();
        }

        private static void GetXml(HttpWebResponse webResponse, Stream responseStream)
        {
            if (webResponse.ContentType == "application/xml; charset=utf-8")
            {
                using (StreamReader responseReader = new StreamReader(responseStream))
                {
                    string responseString = responseReader.ReadToEnd();
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(responseString);
                }
            }
        }

        private int CopyStream(Stream input, Stream output, Stream output2)
        {
            byte[] buffer = new byte[8 * 1024];
            int len;
            int total = 0;
            while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                total += len;
                output.Write(buffer, 0, len);
                output2.Write(buffer, 0, len);
            }
            return total;
        }

        private int CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[8 * 1024];
            int len;
            int total = 0;
            while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                total += len;
                output.Write(buffer, 0, len);
            }
            return total;
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            DownloadFile(txtURL.Text, null, chkZip.Checked);
        }

        private void DisplayResponseSettings(HttpWebResponse response)
        {
            txtMessages.Text = string.Empty;
            txtMessages.AppendText("CharacterSet: " + response.CharacterSet + Environment.NewLine);

            string contentEncoding = response.ContentEncoding;
            txtMessages.AppendText("ContentEncoding: " + contentEncoding + Environment.NewLine);

            if (contentEncoding.ToLower().Contains("gzip"))
            {
                txtMessages.AppendText("* gzip encoding FOUND!!" + Environment.NewLine);
            }
            else
            {
                txtMessages.AppendText("* gzip encoding NOT FOUND: instead -> " + contentEncoding + Environment.NewLine);
            }

            txtMessages.AppendText("ContentLength: " + response.ContentLength.ToString() + Environment.NewLine);
            txtMessages.AppendText("ContentType: " + response.ContentType + Environment.NewLine);

            foreach (var headerKey in response.Headers.AllKeys)
            {
                txtMessages.AppendText("Header: " + headerKey + ", value: " + response.Headers[headerKey] + Environment.NewLine);
            }

            txtMessages.AppendText("IsFromCache: " + response.IsFromCache.ToString() + Environment.NewLine);
            txtMessages.AppendText("Method: " + response.Method + Environment.NewLine);
            txtMessages.AppendText("ProtocolVersion: " + response.ProtocolVersion.ToString() + Environment.NewLine);
            txtMessages.AppendText("ResponseUri.AbsolutePath: " + response.ResponseUri.AbsolutePath + Environment.NewLine);
            txtMessages.AppendText("StatusCode: " + response.StatusCode.ToString() + Environment.NewLine);
            txtMessages.AppendText("StatusDescription: " + response.StatusDescription + Environment.NewLine);
        }

        private void MakePostTest()
        {
            try
            {
                WebClient client = new WebClient();
                client.Headers["Content-type"] = "application/json";

                PostRequest request = new PostRequest();
                request.All = true;

                MemoryStream memStream = new MemoryStream();
                DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(PostRequest));

                jsonSerializer.WriteObject(memStream, request);

                byte[] res1 = client.UploadData(txtURL.Text, "POST", memStream.ToArray());

                Stream res2 = new MemoryStream(res1);
                jsonSerializer = new DataContractJsonSerializer(typeof(List<PostResponse>));

                List<PostResponse> responseList = (List<PostResponse>)jsonSerializer.ReadObject(res2);

                string format = "Response -> ID: {0}, Name: {1}, URL: {2}" + Environment.NewLine;
                txtMessages.Text = "POST Test to WCF service." + Environment.NewLine;
                foreach (var responseObj in responseList)
                {
                    txtMessages.AppendText(string.Format(format, responseObj.ID, responseObj.Name, responseObj.URL));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnDoPost_Click(object sender, EventArgs e)
        {
            MakePostTest();
        }

        private void btnGetCheckSum_Click(object sender, EventArgs e)
        {
            string Url = "http://localhost:88/WcfRESTService/Service1.svc/checksum/" + txtReportNumber.Text;
            bool GZip = chkZip.Checked;
            string PostData = string.Empty;
            try
            {

                HttpWebRequest Http = (HttpWebRequest)WebRequest.Create(Url);

                if (GZip)
                    Http.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");

                if (!string.IsNullOrEmpty(PostData))
                {
                    Http.Method = "POST";
                    byte[] lbPostBuffer = Encoding.Default.GetBytes(PostData);

                    Http.ContentLength = lbPostBuffer.Length;

                    Stream PostStream = Http.GetRequestStream();
                    PostStream.Write(lbPostBuffer, 0, lbPostBuffer.Length);
                    PostStream.Close();
                }

                HttpWebResponse webResponse = (HttpWebResponse)Http.GetResponse();

                string stringResponse = new StreamReader(webResponse.GetResponseStream()).ReadToEnd();
                MessageBox.Show("MD5 response: " + stringResponse);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        public string ComputeMD5(Stream stream)
        {
            return ComputeHash(stream, new System.Security.Cryptography.MD5CryptoServiceProvider());
        }

        public string ComputeHash(Stream stream, HashAlgorithm algorithm)
        {
            stream.Position = 0;

            byte[] hash = algorithm.ComputeHash(stream);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x2"));
            }

            return sb.ToString();
        }

        public void DisplayHash(Stream stream, string algorithmName, HashAlgorithm algorithm, bool compressed)
        {
            string streamType = compressed ? "compressed" : "flat";
            string checkSum = string.Empty;

            txtMessages.AppendText("Computing " + algorithmName + " from " + streamType +
                " stream started at: " + DateTime.Now.ToString() + Environment.NewLine);

            checkSum = ComputeHash(stream, algorithm);

            txtMessages.AppendText("Computing " + algorithmName + " from " + streamType +
                " stream finished at: " + DateTime.Now.ToString() + Environment.NewLine);

            txtMessages.AppendText(algorithmName + " from " + streamType + " stream: " +
                checkSum + Environment.NewLine);
        }

        private void btnStressChecksum_Click(object sender, EventArgs e)
        {
            string filename = GetFileNamePath();

            if (!string.IsNullOrWhiteSpace(filename))
            {
                ProcessChecksumStressMode(filename);
            }
        }

        private string GetFileNamePath()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                return dialog.FileName;
            }
            return string.Empty;
        }

        private void ProcessChecksumStressMode(string filename)
        {
            int cycles = Convert.ToInt32(txtStressCycles.Text);
            txtMessages.Text = string.Empty;

            List<string> list = new List<string>();
            list.Add("MD5");
            list.Add("SHA1");
            list.Add("SHA256");
            list.Add("SHA384");
            list.Add("SHA512");
            list.Add("HMACMD5");
            list.Add("HMACSHA1");
            list.Add("HMACSHA256");
            list.Add("HMACSHA384");
            list.Add("HMACSHA512");

            for (int j = 0; j < list.Count; j++)
            {
                txtMessages.AppendText("Computing checksums, " + cycles + " cycles, for algorithm " + list[j] + ", started at: " + DateTime.Now.ToString("o") + Environment.NewLine);
                DateTime start = DateTime.Now;
                Application.DoEvents();


                Parallel.For(0, cycles, delegate(int i)
                {
                    GetCheckSum(list[j], filename, GetHashAlgorithmInstance(list[j]));
                });

                DateTime stop = DateTime.Now;
                TimeSpan span = stop.Subtract(start);
                txtMessages.AppendText("Computing checksums, " + cycles + " cycles, for algorithm " + list[j] + ", finished at: " + DateTime.Now.ToString("o") + Environment.NewLine);
                txtMessages.AppendText("Computing checksums, " + cycles + " cycles, finished in: " + span.Minutes.ToString("00") + ":" + span.Seconds.ToString("00") + "." + span.Milliseconds + " (mm:ss.ffff)" + Environment.NewLine);
                Application.DoEvents();
            }
        }

        private HashAlgorithm GetHashAlgorithmInstance(string name)
        {
            switch (name)
            {
                case "MD5": return new MD5CryptoServiceProvider(); break;
                case "SHA1": return new SHA1CryptoServiceProvider(); break;
                case "SHA256": return new SHA256CryptoServiceProvider(); break;
                case "SHA384": return new SHA384CryptoServiceProvider(); break;
                case "SHA512": return new SHA512CryptoServiceProvider(); break;
                case "HMACMD5": return new HMACMD5(); break;
                case "HMACSHA1": return new HMACSHA1(); break;
                case "HMACSHA256": return new HMACSHA256(); break;
                case "HMACSHA384": return new HMACSHA384(); break;
                case "HMACSHA512": return new HMACSHA512(); break;
            }
            return new MD5CryptoServiceProvider();
        }

        public string GetCheckSum(string name, string filename, HashAlgorithm algorithm)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                using (FileStream fileStream = new System.IO.FileStream(filename, System.IO.FileMode.Open, FileAccess.Read, FileShare.Read, 512, true))
                {
                    byte[] retVal = algorithm.ComputeHash(fileStream);

                    for (int i = 0; i < retVal.Length; i++)
                    {
                        sb.Append(retVal[i].ToString("x2"));
                    }
                }
                return sb.ToString();
            }
            catch (Exception err)
            {
                throw;
            }
        }
    }
}
