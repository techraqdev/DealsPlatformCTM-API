using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Deals.Domain.Constants
{
    public class DomainConstants
    {
        public static string buySide = "Buy  Side";
        public static string sellSide = "Sell Side";
        public static string nonDeal = "Non Deal";
        /// <summary>
        /// Values are references to the taxonomy services, 
        /// </summary>
        public enum ItemStatus
        {
            Active = 2,
            Inactive = 3,
            Deleted = 4
        }

        public enum ProjectWfStausEnum
        {
            CredCreated = 1,
            CredNotResponded = 2,
            CredQuotable = 3,
            CredNotQuotable = 4,
            CredRestricted = 5,
            CredPartnerApprovalPending = 6,
            CredRejectedbyPartner = 7,
            CredClientApprovalPending = 8,
            CredRejectedbyClient = 9,
            CredClientSeekingMoreInfo = 10,
            CredApproved = 11,
            CredRestrictionConfirmed = 12,
            ValuationProjectCreated = 201,
            ValuationProjectNotResponded = 202,
            ValuationCanbeUsedforCTM = 203,
            ValuationCannotbeUsedforCTM = 204,
            ValuationEngagementOngoing = 205,
            ValuationEngagementCompleted = 206,
            ValuationCannotbeUSedforCTMConfirmed = 207,
            ValuationInformationUploaded = 208,

            CfibProjectCreated = 305,
            InformationUploaded = 301,
            MarkedasInformationnotAvailable = 302,
            InformationNotAvailableConfirmed = 303,
            InformationNotAvailableRejected = 304,
        }

        public enum ProjectWfActionsEnum
        {
            CredCreateProject = 1,
            CredEmailTriggered = 2,
            CredMarkasQuotable = 3,
            CredMarkasNonQuotable = 4,
            CredMarkasRestricted = 5,
            CredOverridesRestriction = 6,
            CredSubmitforPartnerAproval = 7,
            CredMarkasRejectedPartner = 8,
            CredMarkasApprovedPartner = 9,
            CredMarkasApprovedClient = 10,
            CredMarkasRejectedClient = 11,
            CredMarkasneedMoreInfo = 12,
            CredConfirmRestriction = 13,
            CredRemoveRestriction = 14,
            CredRemoveApproval = 15,
            ValuationCreateProject = 201,
            ValuationTriggerEmail = 202,
            ValuationMarkasCanbeUsed = 203,
            ValuationMarkasCannotbeUsed = 204,
            ValuationMarkasEngagementOngoing = 205,
            ValuationMarkasEngagementCompleted = 206,
            ValuationConfirmCannotbeUsed = 207,
            ValuationUploadInformaiton = 208,
            ValuationRejectCannotbeUsed = 209,

            CfibProjectCreated = 305,
            InformationUploaded = 301,
            MarkedasInformationnotAvailable = 302,
            InformationNotAvailableConfirmed = 303,
            InformationNotAvailableRejected = 304,
            NoInfoToUpload = 306,

        }

        public enum StatusTypes
        {
            Active = 1,
            Inactive = 2,
            Deleted = 3
        }

        public enum EngagementTypes
        {
            BuySide = 1,
            SellSide = 2,
            NonDeal = 3,
        }

        public enum CostCentersEnum
        {
            TransactionServices = 1,
            Valuations = 2,
            CFIB = 3,
            BRS = 4,
            DealsCorporate = 5,
            DDV = 6,
            DealsStrategy = 7
        }
        public class EnumString : Attribute
        {
            public EnumString(string stringValue)
            {
                this.StringValue = stringValue;
            }
            public string StringValue { get; set; }

        }

        public enum RolesEnum
        {
            [EnumString("c41121ed-b6fb-c9a6-bc9b-574c82929e7e")]
            Admin = 1,
            [EnumString("c41121ed-b6fb-c9a6-bc9b-574c82929e7b")]
            User = 2,
            [EnumString("c016fa95-9f0b-47d6-ad67-23720d3c8b19")]
            Client = 3
        }

        public static class StringEnum
        {
            public static string GetStringValue(Enum value)
            {
                string output = null;
                Type type = value.GetType();
                FieldInfo fi = type.GetField(value.ToString());
                EnumString[] attrs = fi.GetCustomAttributes(typeof(EnumString), false) as EnumString[];

                if (attrs.Length > 0)
                    output = attrs[0].StringValue;

                return output;
            }
        }

        public enum UserTypesEnum
        {
            Admin = 1,
            TaskManager = 2,
            ProjectPartner = 3
        }
        public enum ProjectTypes
        {
            CTMDuplicateData= 2101,
            CTMErrorData = 2201,

            CTMDuplicateResolved = 2102,
            CTMDuplicateNotanIssue = 2103,

            CTMErrorResolved = 2202,
            CTMErrorNotanIssue = 2203,
        }
        public enum MailTemplateEnum
        {
            [EnumString("ddf5a906-112e-45b9-9701-03adf806e602")]
            CtmDisputeEmail = 1,
        }
    }
}
