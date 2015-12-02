using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Data.Items;
using System.IO;
using org.apache.pdfbox.pdmodel;
using org.apache.pdfbox.util;
using java.io;


namespace Genworth.SitecoreExt.Helpers
{
    class PDFExtractorHelper
    {
        public static string GetContent(Item oItem)
        {
            if (oItem.Paths.IsMediaItem)
            {
                MediaItem oMediaItem = (MediaItem)oItem;
                return GetContent(oMediaItem);
            }
            return string.Empty;
        }
        public static string GetContent(MediaItem oMediaItem)
        {
            string sResult = string.Empty;
            PDDocument doc = null;
            ikvm.io.InputStreamWrapper wrapper = null;
            if (oMediaItem != null && oMediaItem.Extension == "pdf")
            {
                try
                {

                    //Stream stream = oMediaItem.GetMediaStream();
                    //MemoryStream ms = new MemoryStream();
                    //stream.CopyTo(ms);
                    //ByteArrayInputStream oByteArrayStream = new ByteArrayInputStream(ms.GetBuffer());
                    //ms.Close();
                    //stream.Close();

                    //doc = PDDocument.load(oByteArrayStream);
                    //PDFTextStripper stripper = new PDFTextStripper();
                    //sResult = stripper.getText(doc);

                    /*
                     * Using above code, will generate several exceptions on content retrieval, all with the same error message, example:
                     * 23864 18:28:32 ERROR PDF GetContent: Could not extract from mediaItem: {57F2C94F-1204-4975-BABE-287B167F07AF}, media item path: [/sitecore/media library/Files/Investments/AssetMark/Monthly Performance/09-2013/GFWM_3301_MonthlyPerformanceAR_2013_09_C11582pdf?sc_database=web]
                        Exception: java.io.IOException
                        Message: Push back buffer is full
                        Source: pdfbox-1.6.0
                           at org.apache.pdfbox.pdfparser.PDFParser.parse()
                           at org.apache.pdfbox.pdmodel.PDDocument.load(InputStream input, RandomAccess scratchFile)
                           at org.apache.pdfbox.pdmodel.PDDocument.load(InputStream input)
                           at Genworth.SitecoreExt.Helpers.PDFExtractorHelper.GetContent(MediaItem oMediaItem)
                     * 
                     * According to this web reference (https://issues.apache.org/jira/browse/PDFBOX-1818), the issue has been fixed on 1.8.4 and 2.0.0 versions. Current PDFbox version 1.6.0.
                     * 
                     * However, code below does not bring the issues as in above's code.
                     */

                    wrapper = new ikvm.io.InputStreamWrapper(oMediaItem.GetMediaStream());
                    doc = PDDocument.load(wrapper);
                    sResult = new PDFTextStripper().getText(doc);
                    int contentLength = sResult != null ? sResult.Length : 0;
                    Sitecore.Diagnostics.Log.Info("PDF Content extraction successful, media item ID: " + oMediaItem.ID.ToString() + ", content length: " + contentLength, typeof(PDFExtractorHelper));
                }
                catch (Exception Ex)
                {
                    Sitecore.Diagnostics.Log.Error("PDF GetContent: Could not extract from mediaItem: " + oMediaItem.ID.ToString() + ", media item path: [" + oMediaItem.Path + "]", Ex, typeof(PDFExtractorHelper));
                }
                finally
                {
                    if (doc != null)
                        doc.close();

                    if (wrapper != null)
                        wrapper.close();
                }
            }
            return sResult;
        }




    }
}
