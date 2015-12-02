using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using DocumentEntities;
using System.Security.Cryptography;
using TestEntities;
using System.Web.Services.Protocols;
using DocumentProcessor.Strategies;
using DocumentProcessor.Mapping;
using DocumentProcessor.Helpers;
using DocumentProcessor.Commands;
using System.IO;
using System.Configuration;

namespace TestRCToolService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    public class Service1 : IService1
    {
        public DocumentList GetFileMapDefaultFormat(string value)
        {
            return GetFileMap(value);
        }

        public DocumentList GetFileMapJSONFormat(string value)
        {
            return GetFileMap(value);
        }

        public DocumentList GetFileMapXMLFormat(string value)
        {
            return GetFileMap(value);
        }

        public DocumentList GetFileMap(string value)
        {
            return new DocumentList()
            {
                Documents = new List<Document>() 
                                    { 
                                          new Document() { Id = "jpm", Title = "JPM Quarterly Review", 
                                                            Path = "/InvestmentManagement/Step4PortfolioStrategistSelection/JPM/",
                                                            CanDistribute = false, ClientApproved = true, 
                                                            Category1 = "InvestmentManagement" , 
                                                            Category2 = "Step4PortfolioStrategistSelection", 
                                                            Category3 = "JPM",
                                                            URL = "http://www.ewealthmanager.com/en/~media/hbaldgbasjdnqy898.ashx",
                                                            Files = new List<RCFile>() 
                                                                {  
                                                                    new RCFile() { Name = "JP Morgan Overview", Size = "123456", Checksum= "14fde92e3629116bef9cb462e2301ba0ec90c3b9"},
                                                                    new RCFile() { Name = "JP Morgan Introduction", Size = "234567", Checksum= "14fde92e3629116bef9cb462e2301ba0ec90c3b9"} 
                                                                }
                                                            },
                                          new Document() { Id = "callan", Title = "Callan Quarterly Review", 
                                                            Path = "/InvestmentsReviews" }
                                    }
            };

            StrategyBase strategy = new IndexStrategy();
            Map file = new Map(strategy);
            return file.Get();
        }

        public System.IO.Stream Get(string value)
        {
            return GetReport(value);
        }

        public System.IO.Stream GetReport(string value)
        {
            //skipped some lines of code. 
            System.IO.MemoryStream memStream = null;
            try
            {
                //string myPDF = HttpContext.Current.Request.MapPath(".") + "\\" + "App_Data" + "\\" + "RMD_Selwyn_Miller_20120910.pdf";
                string myPDF = @"D:\Development Sandbox\Samples\Softtek\Rest4iOSApp\REST4iOSApp\WcfRESTService\App_Data\RMD_Selwyn_Miller_20120910.pdf";

                using (System.IO.FileStream fileStream = new System.IO.FileStream(myPDF, System.IO.FileMode.Open))
                {
                    memStream = new System.IO.MemoryStream();
                    memStream.SetLength(fileStream.Length);
                    fileStream.Read(memStream.GetBuffer(), 0, (int)fileStream.Length);
                }
            }
            catch (SoapException err)
            {
                throw;
            }

            memStream.Position = 0;

            WebOperationContext.Current.OutgoingResponse.ContentType = "application/pdf"; //application/x-zip-compressed
            //WebOperationContext.Current.OutgoingResponse.ContentType = "application/x-zip-compressed";
            //WebOperationContext.Current.OutgoingResponse.Headers.Add("Content-Disposition", string.Format("attachment; filename=RMD_Selwyn_Miller.pdf"));
            return memStream;
        }

        public System.IO.Stream GetPostReport(string value)
        {
            //skipped some lines of code. 
            System.IO.MemoryStream memStream = null;
            try
            {
                //string myPDF = HttpContext.Current.Request.MapPath(".") + "\\" + "App_Data" + "\\" + "RMD_Selwyn_Miller_20120910.pdf";
                string myPDF = @"D:\Development Sandbox\Samples\Softtek\Rest4iOSApp\REST4iOSApp\WcfRESTService\App_Data\SND-SlideNotes_v6.5.0.1_USLetter.pdf";

                using (System.IO.FileStream fileStream = new System.IO.FileStream(myPDF, System.IO.FileMode.Open))
                {
                    memStream = new System.IO.MemoryStream();
                    memStream.SetLength(fileStream.Length);
                    fileStream.Read(memStream.GetBuffer(), 0, (int)fileStream.Length);
                }
            }
            catch (SoapException err)
            {
                throw;
            }

            memStream.Position = 0;

            WebOperationContext.Current.OutgoingResponse.ContentType = "application/pdf";
            //WebOperationContext.Current.OutgoingResponse.Headers.Clear();
            //WebOperationContext.Current.OutgoingResponse.Headers.Add("Content-Disposition", string.Format("attachment; filename=SitecoreStudentSlides.pdf"));

            return memStream;
        }

        public List<PostResponse> GetDocumentList()
        {
            List<PostResponse> list = new List<PostResponse>();

            if (true)
            {
                list.Add(new PostResponse() { ID = "1", Name = "Google", URL = "http://www.google.com" });
                list.Add(new PostResponse() { ID = "2", Name = "Yahoo", URL = "http://www.yahoo.com" });
                list.Add(new PostResponse() { ID = "3", Name = "Microsoft", URL = "http://www.microsoft.com" });
                list.Add(new PostResponse() { ID = "4", Name = "Android", URL = "http://developer.android.com" });
            }
            else
            {
                list.Add(new PostResponse() { ID = "4", Name = "Android", URL = "http://developer.android.com" });
            }

            return list;
        }

        public string GetCheckSum(string value)
        {
            try
            {
                string myPDF = string.Empty;
                if (value == "1")
                    myPDF = @"D:\Development Sandbox\Samples\Softtek\Rest4iOSApp\REST4iOSApp\WcfRESTService\App_Data\RMD_Selwyn_Miller_20120910.pdf";
                else
                    myPDF = @"D:\Development Sandbox\Samples\Softtek\Rest4iOSApp\REST4iOSApp\WcfRESTService\App_Data\SND-SlideNotes_v6.5.0.1_USLetter.pdf";

                StringBuilder sb = new StringBuilder();
                using (System.IO.FileStream fileStream = new System.IO.FileStream(myPDF, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                {
                    MD5 md5 = new MD5CryptoServiceProvider();
                    byte[] retVal = md5.ComputeHash(fileStream);

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


        public System.IO.Stream GetPartialSync(System.IO.Stream csvStream)
        {
            throw new NotImplementedException();
        }

        //public Stream GetPartialSync(Stream csvStream, string format)
        //{
        //    throw new NotImplementedException();
        //}

        //public Stream GetFullSync()
        //{
        //    throw new NotImplementedException();
        //}

        //public Stream GetFullSync(string format)
        //{
        //    throw new NotImplementedException();
        //}





        IList<Document> IService1.GetFileMapDefaultFormat(string value)
        {
            throw new NotImplementedException();
        }

        IList<Document> IService1.GetFileMapJSONFormat(string value)
        {
            throw new NotImplementedException();
        }

        IList<Document> IService1.GetFileMapXMLFormat(string value)
        {
            throw new NotImplementedException();
        }


        public string WriteNonSecureXML()
        {
            DataStrategyBase strategy = new NonSecureMockDataStrategy();
            DataMap file = new DataMap(strategy);
            DocumentData data = file.Get();

            string path = ConfigurationManager.AppSettings["nonSecureFilePath"];            
            string fileName = Path.Combine(path, "nonSecure.xml");

            using (MemoryStream stream = (MemoryStream)RCServiceHelper.ConvertToStreamWithFormat<DocumentData>(data, "xml"))
            using (FileStream fStream = new FileStream(fileName, System.IO.FileMode.Create, System.IO.FileAccess.Write, FileShare.Write, 512, FileOptions.WriteThrough))
            {
                fStream.Write(stream.GetBuffer(), 0, (int)stream.Length);
            };

            return "ok";
        }

        public string WriteSecureXML()
        {
            DataStrategyBase strategy = new SecureMockDataStrategy();
            DataMap file = new DataMap(strategy);
            DocumentData data = file.Get();

            string path = ConfigurationManager.AppSettings["secureFilePath"];
            string fileName = Path.Combine(path, "secure.xml");

            using (MemoryStream stream = (MemoryStream)RCServiceHelper.ConvertToStreamWithFormat<DocumentData>(data, "xml"))
            using (FileStream fStream = new FileStream(fileName, System.IO.FileMode.Create, System.IO.FileAccess.Write, FileShare.Write, 512, FileOptions.WriteThrough))
            {
                fStream.Write(stream.GetBuffer(), 0, (int)stream.Length);
            };

            return "ok";
        }

        public string LoadRcLibrary()
        {
            using (new Sitecore.SecurityModel.SecurityDisabler())
            {
                RCToolFileGenerator generator = new RCToolFileGenerator();
                generator.Execute(null, null, null);
            }
            return "ok";
        }
    }
}
