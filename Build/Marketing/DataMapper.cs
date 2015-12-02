using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using Genworth.SitecoreExt.Marketing.Request;
using GFWM.Common.AUM.Entities.Data;
using Genworth.SitecoreExt.Security;
using GFWM.Common.Preference.Entities.Data;
//using GFWM.Common.State.Entities.Data;
using GFWM.App.Shared.Entity.Registration.Data;

namespace Genworth.SitecoreExt.Marketing
{
    public class DataMapper
    {

        public StandardRegisterLoginData MapToStandardRegisterData(AgentDetails agentData, IEnumerable<SupportTeamDetails> supportTeamContactDetails)
        {
            StandardRegisterLoginData data = new StandardRegisterLoginData();

            data.CostCenter = CostCenterParam;
            data.Email = agentData.Email;
            data.FailureMessage = FailureMessageParam;
            data.FirstName = agentData.FirstName;
            data.LastName = agentData.LastName;
            data.Phone = FormatPhoneNumber(agentData.Phone);
            data.Role = GetMappingRole(agentData.APLId, agentData.Channel, agentData.PC_Status);
            data.ReturnUrl = ReturnUrlParam;
            data.SFDCID = agentData.PrimaryAdvisorID;
            data.SourceSystemName = SourceSystemNameParam;
            data.UpdateProfile = "true";
            data.UserIdentity = agentData.APLId;

            data.Address = new PhysicalAddressData()
            {
                Line1 = agentData.Address1,
                City = agentData.City,
                PostalCode = agentData.Zip,
                State = agentData.State
            };

            var regionalConsultant = supportTeamContactDetails != null ? supportTeamContactDetails.Where(t => t.JobTitle == "Regional Consultant").FirstOrDefault() : null;
            var internalConsultant = supportTeamContactDetails != null ? supportTeamContactDetails.Where(t => t.JobTitle == "Internal Consultant").FirstOrDefault() : null;


            data.AuxFields = new AuxField[5] {
                    new AuxField() { Id = "1", Value = agentData.DivisionManagerWebUserId }, // Manager Identity
                    new AuxField() { Id = "2", Value = regionalConsultant != null ? regionalConsultant.FirstName + " " + regionalConsultant.LastName : null }, // Regional Consultant Name
                    new AuxField() { Id = "3", Value = internalConsultant != null ? internalConsultant.FirstName + " " + internalConsultant.LastName : null }, // Internal Consultant Name
                    new AuxField() { Id = "4", Value = agentData.Channel }, // Channel
                    new AuxField() { Id = "5", Value = agentData.PC_Status } // PC Status
            };

            return data;
        }

        public void MapImpersonatingUserData(StandardRegisterLoginData data, User userData)
        {
            data.ImpersonatingUser = new ImpersonatingUserData()
            {
                FirstName = userData != null ? userData.FirstName ?? string.Empty : string.Empty,
                LastName = userData != null ? userData.LastName ?? string.Empty : string.Empty,
                UserIdentity = userData != null ? userData.UserName ?? string.Empty : string.Empty,
                AuxFields = new AuxField[1] { 
                new AuxField() { 
                    Id = "6", Value = "Yes" } // Is internal user
                }
            };
        }

        public StandardRegisterLoginData MapToStandardRegisterData(string aplId, IEnumerable<MyProfileInformation> profiles)
        {
            StandardRegisterLoginData data = new StandardRegisterLoginData();
            var profile = profiles.FirstOrDefault();

            data.Address = new PhysicalAddressData()
            {
                Line1 = profile.Address1,
                Line2 = profile.Address2,
                State = profile.State,
                City = profile.city,
                PostalCode = profile.zipcode
            };

            if (!string.IsNullOrWhiteSpace(profile.Email))
            {
                data.Email = profile.Email;
            }
            else if (!string.IsNullOrWhiteSpace(profile.PortalEmail))
            {
                data.Email = profile.PortalEmail;
            }

            data.CostCenter = CostCenterParam;
            data.FailureMessage = FailureMessageParam;
            data.FirstName = profile.Firstname;
            data.LastName = profile.Lastname;
            data.Phone = FormatPhoneNumber(profile.Phone);
            data.Role = GetMappingRole(aplId, string.Empty, null);
            data.SourceSystemName = SourceSystemNameParam;
            data.ReturnUrl = ReturnUrlParam;
            data.UpdateProfile = "true";
            data.UserIdentity = aplId;

            return data;
        }

        private string FormatPhoneNumber(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
            {
                return string.Empty;
            }

            // apply this format to phone numbers: (999)999-9999 x999 
            string format = "({0}){1}-{2}";
            string formatExtension = "({0}){1}-{2} x{3}";
            string formattedPhone = string.Empty;

            StringBuilder digits = new StringBuilder();
            foreach (var digit in phone.ToCharArray())
            {
                if (Char.IsDigit(digit))
                {
                    digits.Append(digit);
                }
            }

            int totalDigits = digits.ToString().Count();
            string phoneNumber = digits.ToString();

            // check if valid phone number
            if (totalDigits < 10)
            {
                // return empty phone number
            }
            else if (totalDigits == 10)
            {
                formattedPhone = string.Format(format, phoneNumber.Substring(0, 3), phoneNumber.Substring(3, 3), phoneNumber.Substring(6));
            }
            else
            {
                formattedPhone = string.Format(formatExtension, phoneNumber.Substring(0, 3), phoneNumber.Substring(3, 3), phoneNumber.Substring(6, 4), phoneNumber.Substring(10));
            }

            return formattedPhone;
        }

        private string GetMappingRole(string agentId, string agentChannel, string pc_Status)
        {
            //ID	Name	    webIndividualPCTitle
            //1	    Non-PC	    GFWM Financial Advisor
            //2	    Standard	Premier Consultant
            //4	    Gold	    Gold Premier Consultant
            //5	    Platinum	Platinum Premier Consultant

            //User Group	                Channel 	PC Status	    Role
            //Internal users 	            n/a	        n/a	            GENMKT
            //ADV: Non-PC	                ADG	        Non-PC	        GENANP
            //ADV: PC	                    ADG	        PC	            GENAPC
            //ADV: Gold/Platinum 	        ADG	        Gold/Platinum	GENRGP
            //REF: Non-PC	                IPG	        Non-PC	        GENRNP
            //REF: PC	                    IPG	        PC	            GENRPC
            //REF: Gold/Platinum	        IPG	        Gold/Platinum	GENRGP
            //Private Client Group (PCG)	PCG 	    n/a	            GENPCG

            string role = string.Empty;
            string pcStatus = pc_Status ?? string.Empty;
            string channel = string.IsNullOrWhiteSpace(agentChannel) ? string.Empty : agentChannel;

            switch (channel.ToUpper())
            {
                case "":
                    switch (pcStatus.ToUpper())
                    {
                        case "":
                            role = "GENMKT";
                            break;
                    }
                    break;
                case "ADG":
                    switch (pcStatus.ToUpper())
                    {
                        case "NON-PC":
                            role = "GENANP";
                            break;
                        case "STANDARD":
                            role = "GENAPC";
                            break;
                        case "GOLD":
                        case "PLATINUM":
                            role = "GENAGP";
                            break;
                    }
                    break;
                case "IPG":
                    switch (pcStatus.ToUpper())
                    {
                        case "NON-PC":
                            role = "GENRNP";
                            break;
                        case "STANDARD":
                            role = "GENRPC";
                            break;
                        case "GOLD":
                        case "PLATINUM":
                            role = "GENRGP";
                            break;
                    }
                    break;
                case "PCG":
                    role = "GENPCG";
                    break;
            }

            if (string.IsNullOrWhiteSpace(role))
            {
                Sitecore.Diagnostics.Log.Error("Unable to map a role for agent " + agentId + " for Standard Register request.",
                    new Exception("Unable to map a role for agent " + agentId), typeof(Controller));
            }

            return role;
        }

        private static string sSourceSystemName;
        public static string SourceSystemNameParam
        {

            get
            {
                if (string.IsNullOrEmpty(sSourceSystemName))
                {
                    sSourceSystemName = Sitecore.Configuration.Settings.GetSetting(Genworth.SitecoreExt.Constants.Settings.Pages.Marketing.SourceSystemName);
                    if (string.IsNullOrEmpty(sSourceSystemName))
                    {
                        Sitecore.Diagnostics.Log.Error("Standard Register SourceSystemName has not been configured in sitecore\\settings", typeof(DataMapper));
                    }
                }

                return sSourceSystemName;
            }
        }

        private static string sCostCenter;
        public static string CostCenterParam
        {

            get
            {
                if (string.IsNullOrEmpty(sCostCenter))
                {
                    sCostCenter = Sitecore.Configuration.Settings.GetSetting(Genworth.SitecoreExt.Constants.Settings.Pages.Marketing.CostCenter);
                    if (string.IsNullOrEmpty(sCostCenter))
                    {
                        Sitecore.Diagnostics.Log.Error("Standard Register CostCenter has not been configured in sitecore\\settings", typeof(DataMapper));
                    }
                }

                return sCostCenter;
            }
        }

        private static string sFailureMessage;
        public static string FailureMessageParam
        {
            get
            {
                if (string.IsNullOrEmpty(sFailureMessage))
                {
                    sFailureMessage = Sitecore.Configuration.Settings.GetSetting(Genworth.SitecoreExt.Constants.Settings.Pages.Marketing.FailureMessage);
                    if (string.IsNullOrEmpty(sFailureMessage))
                    {
                        Sitecore.Diagnostics.Log.Error("Standard Register CostCenter has not been configured in sitecore\\settings", typeof(DataMapper));
                    }
                }

                return sFailureMessage;
            }
        }

        private static string sReturnURL;
        public static string ReturnUrlParam
        {
            get
            {
                if (string.IsNullOrEmpty(sReturnURL))
                {
                    sReturnURL = Sitecore.Configuration.Settings.GetSetting(Genworth.SitecoreExt.Constants.Settings.Pages.Marketing.ReturnURL);
                    if (string.IsNullOrEmpty(sReturnURL))
                    {
                        Sitecore.Diagnostics.Log.Error("Standard Register Return URL has not been configured in sitecore\\settings", typeof(DataMapper));
                    }
                }

                return sReturnURL;
            }
        }
    }
}
