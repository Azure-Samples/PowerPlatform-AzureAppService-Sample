// <copyright file="UpdateBudgetLineTotalAmountRequest.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace ExcelImportService.Models.Requests
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// Model for updating total amount in Budget Header.
    /// </summary>
    public class UpdateBudgetLineTotalAmountRequest
    {
        /// <summary>
        /// Gets or sets BudgetAmount field.
        /// </summary>
        [JsonPropertyName("contoso_budgetamount")]
        public float? BudgetAmount { get; set; }
    }
}
