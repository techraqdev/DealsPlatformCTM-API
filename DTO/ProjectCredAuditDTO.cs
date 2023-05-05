using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DTO
{
    public class ProjectCredAuditDTO
    {
        [JsonPropertyName("Client Email")]
        public string ClientEmail { get; set; } = default!;

        [JsonPropertyName("Client Contact Name")]
        public string ClientContactName { get; set; } = default!;

        [JsonPropertyName("Engagement Type")]
        public string EngagementType { get; set; } = default!;

        [JsonPropertyName("Nature of Engagement / Deal")]
        public string NatureofEngagement { get; set; } = default!;

        [JsonPropertyName("Nature of Transaction (Deal) / Nature of Work (Non Deal)")]
        public string NatureofTransaction { get; set; } = default!;

        [JsonPropertyName("Deal Value")]
        public string DealValue { get; set; } = default!;

        [JsonPropertyName("Sector")]
        public string Sector { get;set; } = default!;

        [JsonPropertyName("Business Description")]
        public string BusinessDescription { get; set; } = default!;

        [JsonPropertyName("Services")]
        public string Services { get; set; } = default!;

        [JsonPropertyName("Transaction Status")]
        public string TransactionStatus { get; set; } = default!;

        [JsonPropertyName("Public Website")]
        public string PublicWebsite { get; set; } = default!;
        [JsonPropertyName("Pwc Name Quoted in Public Webite")]
        public string QuotedinAnnouncements { get; set; } = default!;        

        [JsonPropertyName("Entity Name Disclosed")]
        public string EntityNameDisclosed { get; set; } = default!;

        [JsonPropertyName("Client Entity Type")]
        public string ClientEntityType { get; set; } = default!;

        [JsonPropertyName("Domicile Country")]
        public string DomicileCountry { get; set; } = default!;

        [JsonPropertyName("Domicile Work Country")]
        public string DomicileWorkCountry { get; set; } = default!;

        [JsonPropertyName("Target Entity Type")]
        public string TargetEntityType { get; set; } = default!;

        [JsonPropertyName("Short Description")]
        public string ShortDescription { get; set; } = default!;
        [JsonPropertyName("Confirmation Date")]
        public string CompletedOn { get; set; } = default!;
        [JsonPropertyName("Target Entity Name")]
        public string TargetEntityName { get; set; } = default!;
    }
}
