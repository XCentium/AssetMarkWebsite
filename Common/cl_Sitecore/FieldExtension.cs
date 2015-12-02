using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ServerLogic.Linq;
using Sitecore.Data.Items;
using Sitecore.Data.Fields;
using Sitecore;
using Sitecore.Resources.Media;
using Sitecore.Data;
using System.Collections.Specialized;

namespace ServerLogic.SitecoreExt
{
	public static class FieldExtension
	{
		/// <summary>
		/// Returns the value of a field for use as a CSS style. Essentially, returns the value in lower case.
		/// </summary>
		/// <param name="oField"></param>
		/// <returns>string</returns>
		public static string GetStyleValue(this Field oField)
		{
			return oField.GetStyleValue(string.Empty);
		}

		/// <summary>
		/// Returns the value of a field or default value for use as a CSS style. Essentially, returns the value in lower case.
		/// </summary>
		/// <param name="oField"></param>
		/// <param name="sDefault"></param>
		/// <returns>string</returns>
		public static string GetStyleValue(this Field oField, string sDefault)
		{
			return oField.GetText(sDefault).ToLower();
		}

        public static void ConfigImage(this Field oField, System.Web.UI.WebControls.Image oImage)
        {
            var imgItem = oField.GetImageItem();
            oImage.Visible = imgItem != null;
            if (oImage.Visible)
            {
                var width = imgItem.GetText("Image", "Width");
                var height = imgItem.GetText("Image", "Height");
                oImage.ImageUrl = string.Format("~/{0}?mw={1}&mh={2}", imgItem.GetMediaURL(string.Empty), width, height);
                oImage.Attributes["width"] = width;
                oImage.Attributes["height"] = height;
                oImage.AlternateText = imgItem.GetText("Image", "Alt");
                oImage.ToolTip = oImage.AlternateText;
            }
        }

		/// <summary>
		/// Returns the string value of a field as an HTML paragraph. Essentially, returns a string where carriage returns and new lines are replaced with HTML paragraph tags. (<p></p>)
		/// </summary>
		/// <param name="oField"></param>
		/// <returns>string</returns>
		public static string GetMultiLineText(this Field oField)
		{
			return oField.GetMultiLineText(string.Empty);
		}

		/// <summary>
		/// Returns a date value of a field. Essentially, attempts to parse a date field and returns a nullable date.
		/// </summary>
		/// <param name="oField"></param>
		/// <returns>DateTime?</returns>
		public static DateTime? GetDate(this Field oField)
		{
			string sText;
			DateTime dTempDate;
			DateTime? dDate;

			//set hte date to null
			dDate = null;

			if (!string.IsNullOrEmpty(sText = oField.GetText()))
			{
				//can we parse the date?
				if (DateTime.TryParseExact(sText, "yyyyMMddTHHmmss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dTempDate))
				{
					//set hte date to dTempDate
					dDate = dTempDate;
				}
			}

			//return the date
			return dDate;
		}

        public static NameValueCollection GetNameValueCollection(this Field oField)
        {
            return Sitecore.Web.WebUtil.ParseUrlParameters(oField.GetText(), '&');
        }


		/// <summary>
		/// Returns the string-formatted-value for a date. Since dates can be null, a null value can be specified. The null value will be returned if the date is null.
		/// </summary>
		/// <param name="oField">The field containing the date.</param>
		/// <param name="sFormat">The format string to use.</param>
		/// <param name="sNullValue">The value to return if the field does not contain a date.</param>
		/// <returns></returns>
		public static string GetDateString(this Field oField, string sFormat, string sNullValue)
		{
			DateTime? dDate;
			string sDate;

			if ((dDate = oField.GetDate()).HasValue)
			{
				sDate = dDate.Value.ToString(sFormat);
			}
			else
			{
				sDate = sNullValue;
			}

			//return the formatted date string
			return sDate;
		}

		/// <summary>
		/// Returns the string value of a field as an HTML paragraph. Essentially, returns a string where carriage returns and new lines are replaced with HTML paragraph tags. (<p></p>)
		/// </summary>
		/// <param name="oField"></param>
		/// <param name="sDefault"></param>
		/// <returns>string</returns>
		public static string GetMultiLineText(this Field oField, string sDefault)
		{
			return oField.GetText(sDefault).Replace("\r\n", "</p><p>");
		}

		/// <summary>
		/// Returns the string value of a field. Essentially, gets the text value of a field.
		/// </summary>
		/// <param name="oField"></param>
		/// <returns>string</returns>
		public static string GetText(this Field oField)
		{
			return oField.GetText(string.Empty);
		}

		/// <summary>
		/// Returns the string value of a field. Essentially, gets the text value of a field or returns a default value.
		/// </summary>
		/// <param name="oField"></param>
		/// <param name="sDefault"></param>
		/// <returns>string</returns>
		public static string GetText(this Field oField, string sDefault)
		{
			return oField != null ? oField.Value : sDefault;
		}

		/// <summary>
		/// Returns the decimal value of a field. Essentially, tries to parse the decimal value and returns it.
		/// </summary>
		/// <param name="oField"></param>
		/// <returns>decimal</returns>
		public static decimal GetDecimal(this Field oField)
		{
			return oField.GetDecimal(0M);
		}

		/// <summary>
		/// Returns the decimal value of a field. Essentially, tries to parse the decimal value and returns it or a default value.
		/// </summary>
		/// <param name="oField"></param>
		/// <param name="dDefault"></param>
		/// <returns>decimal</returns>
		public static decimal GetDecimal(this Field oField, decimal dDefault)
		{
			decimal dValue;
			return oField != null && decimal.TryParse(oField.Value, out dValue) ? dValue : dDefault;
		}

		/// <summary>
		/// Returns a Sitecore Item when a field is provided. Essentially, returns an Item based on the current Sitecore context.
		/// </summary>
		/// <param name="oField"></param>
		/// <returns>Item</returns>
		public static Item GetItem(this Field oField)
		{
			return GetItem(oField, ContextExtension.CurrentDatabase);
		}

		/// <summary>
		/// Returns a Sitecore Item when a field is provided. Essentially, returns an Item based on the current Sitecore context.
		/// </summary>
		/// <param name="oField"></param>
		/// <param name="oDatabase"></param>
		/// <returns>Item</returns>
		public static Item GetItem(this Field oField, Database oDatabase)
		{
			//Sitecore.Diagnostics.Log.Debug("Field [" + oField.Section + "." + oField.Name + "] Value [" + oField.Value + "]");
			if (oField == null) return null;
			return oField.Value != null ? oDatabase.GetItem(oField.Value) : null;
		}

		/// <summary>
		/// Returns a string for use as a URL for an image. Essentially, returns the path of an image for display
		/// </summary>
		/// <param name="oField"></param>
		/// <returns>string</returns>
		public static string GetImageURL(this Field oField)
		{
			return oField.GetImageURL(string.Empty);
		}

		/// <summary>
		/// Returns a string for use as a URL for an image. Essentially, returns the path of an image for display
		/// </summary>
		/// <param name="oField"></param>
		/// <returns>Item</returns>
		public static Item GetImageItem(this Field oField)
		{
			ImageField oImageField;
			return (oImageField = oField) != null && oImageField.MediaItem != null ? oImageField.MediaItem : null;
		}

		/// <summary>
		/// Returns a string for use as a URL for an image. Essentially, returns the path of an image for display
		/// </summary>
		/// <param name="oField"></param>
		/// <param name="sDefault"></param>
		/// <returns>string</returns>
		public static string GetImageURL(this Field oField, string sDefault)
		{
			return oField.GetImageItem().GetMediaURL(sDefault);
		}

        public static void ConfigureImageItem(this Field oField, System.Web.UI.WebControls.Image imageControl)
        {
            Item oImage;
            int iWidth;
            int iHeight;

            var oImageField = (Sitecore.Data.Fields.ImageField)oField;
            imageControl.Visible = (oImage = oField.GetImageItem()) != null;
            if (imageControl.Visible)
            {
                imageControl.ImageUrl = "~/" + oImage.GetMediaURL();
                imageControl.AlternateText = oImage.GetText("Image", "Alt");

                if (int.TryParse(oImageField.Width, out iWidth))
                {
                    imageControl.Width = iWidth;
                }

                if (int.TryParse(oImageField.Height, out iHeight))
                {
                    imageControl.Height = iHeight;
                }
            }
        }

		/// <summary>
		/// Returns the size of a file as a double. Essentially, returns the size of a Sitecore media item
		/// </summary>
		/// <param name="oField"></param>
		/// <param name="dDefault"></param>
		/// <returns>double</returns>
		public static double GetFileSize(this Field oField, double dDefault)
		{
			FileField oFileField;

			return (oFileField = oField) != null && oFileField.MediaItem != null ? ((double)new MediaItem(oFileField.MediaItem).Size) / 1024D / 1024D : dDefault;
		}
	}
}