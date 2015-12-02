using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Security;
using System.Configuration;
using System.Collections;
using Sitecore;
using Genworth.SitecoreExt.Services.Providers;


namespace Genworth.SitecoreExt.Utilities
{
	/// <summary>
	/// Helpers to Serialize/Deserialize the profile provider information obtained from the Genworth custom profile provider
	/// </summary>
	public sealed class ProfileServiceSerialization
	{

		public static SettingsPropertyCollection DeserializeSettingsPropertyCollection(List<ProfilePropertyContract> oSettingsPropertyList)
		{

			#region VARIABLES

			System.Configuration.SettingsPropertyCollection oSettingsPropertyCollection;					
			string sCurrentPropertyName;
			
			SettingsProperty oCurrentSettingsProperty;						

			#endregion

			oSettingsPropertyCollection = null;

			if (oSettingsPropertyList != null)
			{
				oSettingsPropertyCollection = new SettingsPropertyCollection();

				foreach (var oGenProperty in oSettingsPropertyList)
				{
					if (oGenProperty != null)
					{
						sCurrentPropertyName = oGenProperty.Name;

						if (!string.IsNullOrEmpty(sCurrentPropertyName))
						{
							oCurrentSettingsProperty = new SettingsProperty(sCurrentPropertyName);

							if (!string.IsNullOrEmpty(oGenProperty.Type))
							{																	
								oCurrentSettingsProperty.PropertyType = Type.GetType(oGenProperty.Type);								
							}
						
							oCurrentSettingsProperty.IsReadOnly = oGenProperty.IsReadOnly;
													
							if (!string.IsNullOrEmpty(oGenProperty.DefaultValue))
							{
								oCurrentSettingsProperty.DefaultValue = oGenProperty.DefaultValue;
							}
							

							if (oGenProperty.Attributes != null)
							{
								foreach (var oGenAttribute in oGenProperty.Attributes)
								{
									oCurrentSettingsProperty.Attributes.Add(oGenAttribute.Key, oGenAttribute.Value);
								}
								
							}							
							oSettingsPropertyCollection.Add(oCurrentSettingsProperty);
						}
					}


				}
			}

			return oSettingsPropertyCollection;
		
		}

		public static List<ProfilePropertyContract> SerializeSettingsPropertyCollection(SettingsPropertyCollection oSettingsPropertyCollection)
		{
			#region VARIABLES

			IEnumerable<SettingsProperty> oSettingsPropertyCollectionToRead;
			List<ProfilePropertyContract> oPropertyList;
			ProfilePropertyContract oGenProperty;
			ProfilePropertyAttributeContract oGenAttribute;
			ICollection oPropertyAttributesValues;
			ICollection oPropertyAttributesKeys;									
			int iAttributeIndex;
			#endregion

			oPropertyList = new List<ProfilePropertyContract>();

			if (oSettingsPropertyCollection != null)
			{
				oSettingsPropertyCollectionToRead = oSettingsPropertyCollection.Cast<SettingsProperty>();

				if (oSettingsPropertyCollectionToRead != null)
				{
					foreach (var property in oSettingsPropertyCollectionToRead)
					{

						oGenProperty = new ProfilePropertyContract()
						{
							Name = (!string.IsNullOrEmpty(property.Name) ? (property.Name) : (string.Empty)),
							Type = ((property.PropertyType != null) ? (property.PropertyType.ToString()) : (typeof(string).ToString())),
							IsReadOnly = property.IsReadOnly,
							DefaultValue = (property.DefaultValue != null) ? (property.DefaultValue.ToString()) : (string.Empty)
						};						

						if (property.Attributes != null)
						{
							oGenProperty.Attributes = new List<ProfilePropertyAttributeContract>();
							oPropertyAttributesValues = property.Attributes.Values;
							oPropertyAttributesKeys = property.Attributes.Keys;							
							
							foreach (var oPropertyattributeKey  in oPropertyAttributesKeys)
							{
								oGenAttribute = new ProfilePropertyAttributeContract()
								{
									Key = oPropertyattributeKey.ToString()
								};
								oGenProperty.Attributes.Add(oGenAttribute);								
							}

							iAttributeIndex = 0;
							foreach (var oPropertyAttributeValue in oPropertyAttributesValues)
							{
								if (oGenProperty.Attributes[iAttributeIndex] != null)
								{
									oGenProperty.Attributes[iAttributeIndex].Value = oPropertyAttributeValue.ToString();
								}
								iAttributeIndex++;
							}
						
						}

						oPropertyList.Add(oGenProperty);
					}
				}

			}

			return oPropertyList;
		}

		public static SettingsPropertyValueCollection DeserializeSettingsPropertyValueCollection(List<ProfilePropertyValueContract> oSerializedSettingsPropertyValueCollection)
		{
		
			#region VARIABLES

			System.Configuration.SettingsPropertyValueCollection oSettingsPropertyValueCollection;
		
			
			string sCurrentPropertyName;			
			Type oCurrentPropertyType;			
			string sCurrentPropertyDefaultValue;			
			SettingsPropertyValue oSettingsCurrentPropertyValue;
			SettingsProperty oCurrentSettingsProperty;			
			

			#endregion

			oSettingsPropertyValueCollection = null;


			if (oSerializedSettingsPropertyValueCollection != null)
			{
				oSettingsPropertyValueCollection = new SettingsPropertyValueCollection();

				foreach (var oPropertySerialized in oSerializedSettingsPropertyValueCollection)
				{

					if (oPropertySerialized.ProfileProperty != null)
					{
						sCurrentPropertyName = oPropertySerialized.ProfileProperty.Name;

						if (!string.IsNullOrEmpty(sCurrentPropertyName))
						{ 
							oCurrentSettingsProperty = new SettingsProperty(sCurrentPropertyName);

							if (oPropertySerialized.ProfileProperty.Type != null)
							{

								oCurrentPropertyType = !string.IsNullOrEmpty(oPropertySerialized.ProfileProperty.Type)? Type.GetType(oPropertySerialized.ProfileProperty.Type) : typeof(string);
								oCurrentSettingsProperty.PropertyType = oCurrentPropertyType;


								oCurrentSettingsProperty.IsReadOnly = oPropertySerialized.ProfileProperty.IsReadOnly;


								sCurrentPropertyDefaultValue = oPropertySerialized.ProfileProperty.DefaultValue;
								if (!string.IsNullOrEmpty(sCurrentPropertyDefaultValue))
								{
									oCurrentSettingsProperty.DefaultValue = sCurrentPropertyDefaultValue;
								}




								if (oPropertySerialized.ProfileProperty.Attributes != null)
									{
										foreach (var oPropertyAttributeSerialized in oPropertySerialized.ProfileProperty.Attributes)
										{
											oCurrentSettingsProperty.Attributes.Add(oPropertyAttributeSerialized.Key, oPropertyAttributeSerialized.Value);											
										}


									}

									//Must set the type to use the PropertyValue  otherwise you will get " NullReferenceException when trying to access the PropertyValue"
									oSettingsCurrentPropertyValue = new SettingsPropertyValue(oCurrentSettingsProperty);
									
									if (!string.IsNullOrEmpty(oPropertySerialized.Value))
									{
										oSettingsCurrentPropertyValue.PropertyValue = oPropertySerialized.Value;;
									}									
									else
									{
										oSettingsCurrentPropertyValue.PropertyValue = null;
									}

									oSettingsPropertyValueCollection.Add(oSettingsCurrentPropertyValue);
															
							}																					
							
						}
					}

					
				}
			}

			return oSettingsPropertyValueCollection;
				
		}


		public static List<ProfilePropertyValueContract> SerializeSettingsPropertyValueCollection(SettingsPropertyValueCollection oSettingsPropertyValueCollection)
		{

			#region VARIABLES

			List<ProfilePropertyValueContract> oSettingsPropertyValueCollectionSerialized;			
			ProfilePropertyAttributeContract oAttributeSerialized;
			ProfilePropertyValueContract oSettingsPropertyValueSerialized;
			ProfilePropertyContract oSettingsPropertySerialized;
			IEnumerable<SettingsPropertyValue> oSettingsCastedOut;						
			ICollection oPropertyAttributesValues;
			ICollection oPropertyAttributesKeys;			
			int iAttributeIndex;

			#endregion

			oSettingsPropertyValueCollectionSerialized = new List<ProfilePropertyValueContract>();		

			if (oSettingsPropertyValueCollection != null && oSettingsPropertyValueCollection.Count > 0)
			{				
				oSettingsCastedOut = oSettingsPropertyValueCollection.Cast<SettingsPropertyValue>();

				foreach (var oSetting in oSettingsCastedOut)
				{
					oSetting.Property.PropertyType = typeof(string);

					oSettingsPropertySerialized = new ProfilePropertyContract()
					{
																Name = (!string.IsNullOrEmpty(oSetting.Name) ? (oSetting.Name) : (string.Empty)),
																Type = ((oSetting.Property.PropertyType != null) ? (oSetting.Property.PropertyType.ToString()) : (typeof(string).ToString())),
																IsReadOnly = oSetting.Property.IsReadOnly,
																DefaultValue = (oSetting.Property.DefaultValue != null) ? (oSetting.Property.DefaultValue.ToString()) : (string.Empty)
					};

					oSettingsPropertyValueSerialized = new ProfilePropertyValueContract()
					{
						ProfileProperty = oSettingsPropertySerialized
					};

					if (oSetting.PropertyValue != null)
					{
						oSettingsPropertyValueSerialized.Value  = oSetting.PropertyValue.ToString();
					}
					

					if (oSetting.Property.Attributes != null)
					{
						oSettingsPropertySerialized.Attributes = new List<ProfilePropertyAttributeContract>();

						oPropertyAttributesValues = oSetting.Property.Attributes.Values;
						oPropertyAttributesKeys = oSetting.Property.Attributes.Keys;						

						foreach (var oPropertyattributeKey in oPropertyAttributesKeys)
						{
							oAttributeSerialized = new ProfilePropertyAttributeContract()
							{
								Key = oPropertyattributeKey.ToString()
							};
							oSettingsPropertySerialized.Attributes.Add(oAttributeSerialized);
						}

						iAttributeIndex = 0;
						foreach (var oPropertyAttributeValue in oPropertyAttributesValues)
						{
							if (oSettingsPropertySerialized.Attributes[iAttributeIndex] != null)
							{
								oSettingsPropertySerialized.Attributes[iAttributeIndex].Value = oPropertyAttributeValue.ToString();
							}
							iAttributeIndex++;
						}												
					}					
				}
			}

			return oSettingsPropertyValueCollectionSerialized;
		}
	}
}
