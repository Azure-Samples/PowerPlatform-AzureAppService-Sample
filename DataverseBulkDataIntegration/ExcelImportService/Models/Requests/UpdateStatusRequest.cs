// <copyright file="UpdateStatusRequest.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace ExcelImportService.Models.Requests
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// Model to update status of a record.
    /// </summary>
    public class UpdateStatusRequest
    {
        /// <summary>
        /// Gets or sets set statecode.
        /// </summary>
        [JsonPropertyName("statecode")]
        public int? StateCode { get; set; } = null;

        /// <summary>
        /// Gets or sets set statuscode.
        /// </summary>
        [JsonPropertyName("statuscode")]
        public int? StatusCode { get; set; } = null;
    }
}
