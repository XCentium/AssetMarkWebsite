using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Linq;
using System.Net;
using System.IO;

namespace Genworth.SitecoreExt.Utilities
{
	class Geography
	{
		
		public const double EarthRadiusInMiles = 3956.0;

		public const double EarthRadiusInKilometers = 6367.0;

		#region GOOGLE GEOLOCATION RESPONSE CONSTANTS

		private const string sCoordinatesXMLResponseNodeName = "location";

		private const string sLatitudeXMLResponseNodeName = "lat";

		private const string sLongitudeXMLResponseNodeName = "lng";

		private const string sXPathSelectorForLocationNode = "./result/geometry/location";

		private const int iLatitude = 0;

		private const int iLongitude = 1;

		public const string sLatitudeKey = "Latitude";

		public const string sLongitudeKey = "Longitude";

		#endregion


		#region PROPERTIES

		private static string GeocodingBaseURL
		{
			get {
					return Sitecore.Configuration.Settings.GetSetting(Genworth.SitecoreExt.Constants.Settings.Integrations.Google.GeocodingBaseURL, string.Empty);
				}
		}

		private static bool IsUsingGoogleAPIKey
		{
			get
			{
				bool bUsingAPKey;				

				bool.TryParse(Sitecore.Configuration.Settings.GetSetting(Genworth.SitecoreExt.Constants.Settings.Integrations.Google.UseAPIKey, string.Empty), out bUsingAPKey);

				return bUsingAPKey;
			}
		}

		private static string GoogleAPIKey
		{
			get
			{
				return Sitecore.Configuration.Settings.GetSetting(Genworth.SitecoreExt.Constants.Settings.Integrations.Google.APIKey, string.Empty);
			}
		}

		#endregion

		public static double ToRadian(double val) { return val * (Math.PI / 180); }

		public static double DiffRadian(double val1, double val2) { return ToRadian(val2) - ToRadian(val1); }

		/// <summary> 
		/// Calculate the distance between two geocodes. Defaults to using Miles. 
		/// </summary> 
		public static double CalcDistance(double lat1, double lng1, double lat2, double lng2)
		{
			return CalcDistance(lat1, lng1, lat2, lng2, GeographyMeasurement.Miles);
		}
		/// <summary> 
		/// Calculate the distance between two geocodes. 
		/// </summary> 
		public static double CalcDistance(double lat1, double lng1, double lat2, double lng2, GeographyMeasurement m)
		{
			double radius = Geography.EarthRadiusInMiles;
			if (m == GeographyMeasurement.Kilometers) { radius = Geography.EarthRadiusInKilometers; }
			return radius * 2 * Math.Asin(Math.Min(1, Math.Sqrt((Math.Pow(Math.Sin((DiffRadian(lat1, lat2)) / 2.0), 2.0) + Math.Cos(ToRadian(lat1)) * Math.Cos(ToRadian(lat2)) * Math.Pow(Math.Sin((DiffRadian(lng1, lng2)) / 2.0), 2.0)))));
		}

		/// <summary>
		/// Determines whether a given zip code is valid
		/// </summary>
		/// <param name="sZipCode"></param>
		/// <returns></returns>
		public static bool IsValidUSZip(string sZipCode)
		{
			// Allows 5 digit, 5+4 digit and 9 digit zip codes
			string pattern = @"^(\d{5}-\d{4}|\d{5}|\d{9})$";

			Regex match = new Regex(pattern);
			return match.IsMatch(sZipCode);
		}

		public static Dictionary<string, string> LookupCoordinates(string sZipCode)
		{			
			WebRequest oWebRequest;
			string sGeocodingURL;
			bool bUseAPIKey;			
			string sRequestURL;
			WebResponse oWebResponse;
			StreamReader oStreamReader;
			string sRequestResult;
			XElement oRequestResult;
			IEnumerable<XElement> oCoordinatesNodes;
			XElement oXMLCoordinates;
			XElement oXMLLatitude;
			XElement oXMLLongitude;			
			Dictionary<string, string> oCoordinatesToReturn;


			oCoordinatesToReturn = new Dictionary<string, string>();

			if (IsValidUSZip(sZipCode))
			{

				sGeocodingURL = GeocodingBaseURL;
				bUseAPIKey = IsUsingGoogleAPIKey;			

				if (Uri.IsWellFormedUriString(sGeocodingURL, UriKind.RelativeOrAbsolute))
				{
					sRequestURL = string.Format("{0}?address={1}&sensor=false", sGeocodingURL, sZipCode);
					
					oWebRequest = WebRequest.Create(sRequestURL);
					oWebRequest.Timeout = 1000 * 1000;
					oWebResponse = oWebRequest.GetResponse();

					if (oWebResponse != null)
					{
						oStreamReader = new StreamReader(oWebResponse.GetResponseStream());
						sRequestResult = oStreamReader.ReadToEnd().Trim();
						if (!string.IsNullOrEmpty(sRequestResult))
						{
							oRequestResult = XElement.Parse(sRequestResult);

							if (oRequestResult != null)
							{
								oCoordinatesNodes = oRequestResult.XPathSelectElements(sXPathSelectorForLocationNode);

								if (oCoordinatesNodes != null)
								{
									oXMLCoordinates = oCoordinatesNodes.FirstOrDefault();

									if (oXMLCoordinates != null)
									{
										oXMLLatitude = oXMLCoordinates.Element(sLatitudeXMLResponseNodeName);
										oXMLLongitude = oXMLCoordinates.Element(sLongitudeXMLResponseNodeName);

										if (oXMLLatitude != null && oXMLLongitude != null)
										{											
											oCoordinatesToReturn.Add(sLatitudeKey, oXMLLatitude.Value);
											oCoordinatesToReturn.Add(sLongitudeKey, oXMLLongitude.Value);
										}
									}
								}
							}
						}
						else
						{
							Sitecore.Diagnostics.Log.Error("LookupCoordinates - No valid response was obtained for zip code {0}", typeof(Geography));
						}
					}
					else
					{
						Sitecore.Diagnostics.Log.Error("LookupCoordinates - Unable to reach Google Geocoding{0}", typeof(Geography));
					}
				}
				else
				{
					Sitecore.Diagnostics.Log.Error(string.Format("LookupCoordinates - Unable to process zip code {0}", sZipCode), typeof(Geography));
				}
			}

			return oCoordinatesToReturn;
		}
	}
	

	public enum GeographyMeasurement : int
	{
		Miles = 0,
		Kilometers = 1
	}

}
