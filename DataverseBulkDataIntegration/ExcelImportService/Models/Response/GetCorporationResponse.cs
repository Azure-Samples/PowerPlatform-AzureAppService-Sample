// <copyright file="GetCorporationResponse.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace ExcelImportService.Models.Response
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// Model for Corporation entity.
    /// </summary>
    public class GetCorporationResponse
    {
        /// <summary>
        /// gets or sets CorporationId field.
        /// </summary>
        [JsonPropertyName("contoso_corporationid")]
        public Guid CorporationId { get; set; }

        /// <summary>
        /// gets or sets CorporationCode field.
        /// </summary>
        [JsonPropertyName("contoso_corporationcode")]
        public string? CorporationCode { get; set; }

        /// <summary>
        /// gets or sets CorporationName field.
        /// </summary>
        [JsonPropertyName("contoso_corporationname")]
        public string? CorporationName { get; set; }

        /// <summary>
        /// gets or sets ParentCorporation field.
        /// </summary>
        [JsonPropertyName("_contoso_parentcorporation_value")]
        public Guid? ParentCorporationId { get; set; }
    }
}
