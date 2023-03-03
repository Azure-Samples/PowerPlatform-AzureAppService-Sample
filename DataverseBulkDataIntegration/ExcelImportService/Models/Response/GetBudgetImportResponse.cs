// <copyright file="GetBudgetImportResponse.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace ExcelImportService.Models.Response
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// Model for Budget Import entity.
    /// </summary>
    public class GetBudgetImportResponse
    {
        /*
        /// <summary>
        /// Initializes a new instance of the <see cref="GetBudgetImportResponse"/> class.
        /// </summary>
        /// <param name="id">Guid.</param>
        public GetBudgetImportResponse(Guid id)
        {
        }
        */

        /// <summary>
        /// gets or sets Id field.
        /// </summary>
        [JsonPropertyName("contoso_budgetimportid")]
        public string? Id { get; set; }

        /// <summary>
        /// gets or sets Name field.
        /// </summary>
        [JsonPropertyName("contoso_budgetimportname")]
        public string? Name { get; set; }

        /// <summary>
        /// gets or sets FileAttachment field.
        /// </summary>
        public byte[]? FileAttachment { get; set; }
    }
}
