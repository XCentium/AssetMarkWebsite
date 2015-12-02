using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sitecore.Data;

namespace Genworth.SitecoreExt.Constants
{
	/// <summary>
	/// Constants related to the security framework.
	/// </summary>
	public static class Security
	{
		/// <summary>
		/// Constants for the authorization object
		/// </summary>
		public static class Authorization
		{
			public const string IsMeetingMode = "IsMeetingMode";

			public const string MeetingModeKey = "MEETING_MODE";

		}

		/// <summary>
		/// Constants for the SWT integration & authorization object
		/// </summary>
		public static class SWT
		{	
			public const string SSOGuid = "SSOGuid";

			public const string HTTPEncodingServerVariable = "HTTP_ACCEPT_ENCODING";

			public const string BrowserDetailsServerVariable = "ALL_RAW";

			public const string SWTCookieName = "SWT";

			public const string GFWMSessionId = "GFWMSessionId";
		}

		public enum RoleType
		{
			ADMIN = 81000,
			BROKERDEALER = 81001,
			ADVISOR = 81002,
			AGENT = 81003,
			CLIENT = 81004,
			THIRDPARTY = 81005,
			OSJ = 81006,
			RC = 81007,
			SHAREDACCESS = 81008,
			OPS = 81009,
			ADMINASSISTANT = 81010,
			RM = 81012
		}

		public static class Templates
		{
			/// <summary>
			/// Security Base template constants
			/// </summary>
			public static class SecurityBase
			{			
				/// <summary>
				/// Security Base template name (Item Name)
				/// </summary>
				public const string Name = "Security Base";

				/// <summary>
				/// Sections associated to the security base template
				/// </summary>
				public static class Sections
				{					

					/// <summary>
					/// Security Section associated to the Security Base template
					/// </summary>
					public static class Security
					{					
						/// <summary>
						/// Security Section Name
						/// </summary>
						public const string Name = "Security";

						/// <summary>
						/// Fields associated to the security base templates
						/// </summary>
						public static class Fields
						{					
							/// <summary>
							/// User levels field name
							/// </summary>
							public const string UserLevelsFieldName = "User Levels";
							
							public const string UserLevelsID = "{93E24FBB-9038-4EC8-9EB7-51342AB704F9}";

							public static ID UserLevels = new ID(UserLevelsID);

							/// <summary>
							/// Custodians field name
							/// </summary>
							public const string CustodiansFieldName = "Custodians";

							public const string CustodiansID = "{F4AF56F9-FD23-425C-91DC-441615CF771A}";

							public static ID Custodians = new ID(CustodiansID);

							/// <summary>
							/// Channels field name
							/// </summary>
							public const string ChannelsFieldName = "Channels";

							public const string ChannelsID = "{FEB1B145-B2EF-432A-8350-025D0853E827}";

							public static ID Channels = new ID(ChannelsID);


							/// <summary>
							/// Manager Strategist Privileges field name
							/// </summary>
							public const string ManagerStrategistPrivilegesFieldName = "Manager Strategist Privileges";

							public const string ManagerStrategistPrivilegesID = "{DFB3F2DA-355D-4890-9CE0-DC3F7A83187A}";

							public static ID ManagerStrategistPrivileges = new ID(ManagerStrategistPrivilegesID);


							/// <summary>
							/// Products field name
							/// </summary>
							public const string ProductsFieldName = "Products";

							public const string ProductsID = "{FACD5074-FC15-4720-B6F0-D42059FF849D}";

							public static ID Products = new ID(ProductsID);

							/// <summary>
							/// Client Approved field name
							/// </summary>
							public const string ClientApprovedFieldName = "Client Approved";

							public const string ClientApprovedID = "{C61E4CBF-AC8C-45CE-86D9-C6FE492B4855}";

							public static ID ClientApproved = new ID(ClientApprovedID);

							

						}
					}
				}
				


			}

			/// <summary>
			/// Constants related to the User Level template
			/// </summary>
			public static class UserLevel
			{
				/// <summary>
				/// User Level template name (Item Name)
				/// </summary>
				public const string Name = "User Level";

				/// <summary>
				/// Sections associated to the Channel template
				/// </summary>
				public static class Sections
				{
					/// <summary>
					/// Channel Section associated to the Channel template
					/// </summary>
					public static class UserLevel
					{
						/// <summary>
						/// Channel Section Name
						/// </summary>
						public const string Name = "User Level";

						/// <summary>
						/// Fields associated to the Channel templates
						/// </summary>
						public static class Fields
						{
							public const string NameFieldName = "Name";

							public const string NameFieldID = "{47CB44CD-CCFE-4946-BE09-D8784601E804}";

							public static ID NameField = new ID(NameFieldID);

							public const string CodeFieldName = "Code";

							public const string CodeFielID = "{2C14C5D2-4762-4ACC-8875-FC49B3CE8D83}";

							public static ID CodeField = new ID(CodeFielID);

						}
					}
				}
			}

			/// <summary>
			/// Constants related to the Custodian template
			/// </summary>
			public static class Custodian
			{
				/// <summary>
				/// Channel Custodian name (Item Name)
				/// </summary>
				public const string Name = "Custodian";

				/// <summary>
				/// Sections associated to the Custodian template
				/// </summary>
				public static class Sections
				{
					/// <summary>
					/// Custodian Section associated to the Custodian template
					/// </summary>
					public static class Custodian
					{
						/// <summary>
						/// Custodian Section Name
						/// </summary>
						public const string Name = "Custodian";

						/// <summary>
						/// Fields associated to the Custodian section
						/// </summary>
						public static class Fields
						{
							public const string NameFieldName = "Name";

							public const string NameFieldID = "{C3A2A715-D025-4CA7-84BF-EE3E64EFC1EE}";

							public static ID NameField = new ID(NameFieldID);

							public const string CodeFieldName = "Code";

							public const string CodeFielID = "{C59F7BFF-B9A4-4085-BF9A-CC93A4E1E815}";

							public static ID CodeField = new ID(CodeFielID);

						}
					}
				}
			}

			/// <summary>
			/// Constants related to the Channel template
			/// </summary>
			public static class Channel 
			{
				/// <summary>
				/// Channel template name (Item Name)
				/// </summary>
				public const string Name = "Channel";

				/// <summary>
				/// Sections associated to the Channel template
				/// </summary>
				public static class Sections
				{
					/// <summary>
					/// Channel Section associated to the Channel template
					/// </summary>
					public static class Channel
					{
						/// <summary>
						/// Channel Section Name
						/// </summary>
						public const string Name = "Channel";

						/// <summary>
						/// Fields associated to the Channel templates
						/// </summary>
						public static class Fields
						{
							public const string NameFieldName = "Name";

							public const string NameFieldID = "{C3A2A715-D025-4CA7-84BF-EE3E64EFC1EE}";

							public static ID NameField = new ID(NameFieldID);

							public const string CodeFieldName = "Code";

							public const string CodeFielID = "{C59F7BFF-B9A4-4085-BF9A-CC93A4E1E815}";

							public static ID CodeField = new ID(CodeFielID);
							
						}
					}
				}
			}

			/// <summary>
			/// Constants related to the Product template
			/// </summary>
			public static class Product
			{
				/// <summary>
				/// Product template name (Item Name)
				/// </summary>
				public const string Name = "Product";

				/// <summary>
				/// Sections associated to the Product template
				/// </summary>
				public static class Sections
				{
					/// <summary>
					/// Product Section associated to the Product template
					/// </summary>
					public static class Product
					{
						/// <summary>
						/// Product Section Name
						/// </summary>
						public const string Name = "Product";

						/// <summary>
						/// Fields associated to the Product templates
						/// </summary>
						public static class Fields
						{
							public const string NameFieldName = "Name";

							public const string NameFieldID = "{F51027FA-A29E-43BD-852E-EB78DBC8EDD6}";

							public static ID NameField = new ID(NameFieldID);

							public const string CodeFieldName = "Code";

							public const string CodeFielID = "{24257658-97E7-45F2-84DB-42BD64571979}";

							public static ID CodeField = new ID(CodeFielID);

						}
					}
				}
			}

			/// <summary>
			/// Constants related to the User Type template
			/// </summary>
			public static class UserType
			{
				/// <summary>
				/// User Type template name (Item Name)
				/// </summary>
				public const string Name = "User Type";

				/// <summary>
				/// Sections associated to the User Type template
				/// </summary>
				public static class Sections
				{
					/// <summary>
					/// User Type Section associated to the User Type sections
					/// </summary>
					public static class UserLevel
					{
						/// <summary>
						/// User Type Section Name
						/// </summary>
						public const string Name = "User Type";

						/// <summary>
						/// Fields associated to the User Type section
						/// </summary>
						public static class Fields
						{
							public const string NameFieldName = "Name";

							public const string NameFieldID = "{ABF62ACD-8207-4184-94F5-9C26066809A5}";

							public static ID NameField = new ID(NameFieldID);

							public const string CodeFieldName = "Code";

							public const string CodeFielID = "{6997A768-AC39-486E-8CB9-1E1EAF4B09DF}";

							public static ID CodeField = new ID(CodeFielID);

						}
					}
				}
			}


			/// <summary>
			/// Constants related to the Manager Strategist Privilege template
			/// </summary>
			public static class ManagerStrategistPrivilege
			{
				/// <summary>
				/// Manager Strategist Privilege template name (Item Name)
				/// </summary>
				public const string Name = "Manager Strategist Privilege";

				/// <summary>
				/// Sections associated to the Manager Strategist Privilege template
				/// </summary>
				public static class Sections
				{
					/// <summary>
					/// Manager Strategist Privilege Section associated to the User Type sections
					/// </summary>
					public static class ManagerStrategistPrivilege
					{
						/// <summary>
						/// Manager Strategist Privilege Section Name
						/// </summary>
						public const string Name = "Manager Strategist Privilege";

						/// <summary>
						/// Fields associated to the Manager Strategist Privilege section
						/// </summary>
						public static class Fields
						{
							public const string NameFieldName = "Name";

							public const string NameFieldID = "{3B65F7BF-1E2A-4543-B8BC-3E14A65C0B9D}";

							public static ID NameField = new ID(NameFieldID);

							public const string CodeFieldName = "Code";

							public const string CodeFielID = "{9E09B4A5-34E2-4698-804C-55E244F07351}";

							public static ID CodeField = new ID(CodeFielID);

						}
					}
				}
			}


			/// <summary>
			/// Security Base template constants
			/// </summary>
			public static class PC_StatusSecurity
			{
				/// <summary>
				/// PC_Status Security Base template name (Item Name)
				/// </summary>
				public const string Name = "PC-Status-Security";

				/// <summary>
				/// Sections associated to the PC_Status Security Base base template
				/// </summary>
				public static class Sections
				{

					/// <summary>
					/// Security By PC_Status Section associated to the Security Base template
					/// </summary>
					public static class Security
					{
						/// <summary>
						/// Security By PC_Status Section Name
						/// </summary>
						public const string Name = "Security";

						/// <summary>
						/// Fields associated to the security base templates
						/// </summary>
						public static class Fields
						{
							/// <summary>
							/// PC-Status field name
							/// </summary>
							public const string PC_StatusFieldName = "PC-Status";

							public const string PC_StatusID = "{EED41CA2-D2C6-4C82-83F6-20C070FD5791}";

							public static ID PC_Status = new ID(PC_StatusID);
						}
					}
				}
				
			}

			/// <summary>
			/// Constants related to the PC_Status template
			/// </summary>
			public static class PC_Status
			{
				/// <summary>
				/// User Level template name (Item Name)
				/// </summary>
				public const string Name = "PC-Status";

				/// <summary>
				/// Sections associated to the Channel template
				/// </summary>
				public static class Sections
				{
					/// <summary>
					/// Channel Section associated to the Channel template
					/// </summary>
					public static class PC_Status
					{
						/// <summary>
						/// Channel Section Name
						/// </summary>
						public const string Name = "PC-Status";

						/// <summary>
						/// Fields associated to the Channel templates
						/// </summary>
						public static class Fields
						{
							public const string NameFieldName = "Name";

							public const string NameFieldID = "{ED006BB3-7AA2-4B36-8B53-7D1F9D6B24F3}";

							public static ID NameField = new ID(NameFieldID);

							public const string CodeFieldName = "Code";

							public const string CodeFielID = "{2E17B838-0239-49ED-B19E-E371171E104D}";

							public static ID CodeField = new ID(CodeFielID);

						}
					}
				}
			}
		}
						
	}
}
