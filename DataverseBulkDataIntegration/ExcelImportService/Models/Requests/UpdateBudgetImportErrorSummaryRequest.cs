// <copyright file="UpdateBudgetImportErrorSummaryRequest.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace ExcelImportService.Models.Requests
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// Model to update error summary for BudgetIport entity.
    /// </summary>
    public class UpdateBudgetImportErrorSummaryRequest
    {
        /// <summary>
        /// Gets or sets ErrorSummary.
        /// </summary>
        [JsonPropertyName("contoso_errorsummary")]
        public string? ErrorSummary { get; set; }
    }
}
