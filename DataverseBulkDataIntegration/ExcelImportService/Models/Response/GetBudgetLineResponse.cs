// <copyright file="GetBudgetLineResponse.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace ExcelImportService.Models.Response
{
    using System.Text.Json.Serialization;
    using ExcelImportService.Constants;

    /// <summary>
    /// Model for Budget Line entity.
    /// </summary>
    public class GetBudgetLineResponse
    {
        /// <summary>
        /// Gets or sets BudgetLineId field.
        /// </summary>
        [JsonPropertyName("contoso_budgetlineid")]
        public Guid? BudgetLineId { get; set; }

        /// <summary>
        /// Gets or sets BudgetCategoryId field.
        /// </summary>
        [JsonPropertyName("_contoso_budgetcategory_value")]
        public Guid? BudgetCategoryId { get; set; }

        /// <summary>
        /// Gets or sets BudgetHeaderId field.
        /// </summary>
        [JsonPropertyName("_contoso_budgetheader_value")]
        public Guid? BudgetHeaderId { get; set; }

        /// <summary>
        /// Gets or sets TotalBudgetedAmount field.
        /// </summary>
        [JsonPropertyName("contoso_totalbudgetedamount")]
        public float? TotalBudgetedAmount { get; set; }
    }
}
