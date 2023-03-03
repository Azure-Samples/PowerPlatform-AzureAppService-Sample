// <copyright file="GetBudgetCategoryResponse.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace ExcelImportService.Models.Response
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// Model for Budget Category entity.
    /// </summary>
    public class GetBudgetCategoryResponse
    {
        /// <summary>
        /// gets or sets BudgetCategoryId field.
        /// </summary>
        [JsonPropertyName("contoso_budgetcategoryid")]
        public Guid BudgetCategoryId { get; set; }

        /// <summary>
        /// gets or sets BudgetCategoryCode field.
        /// </summary>
        [JsonPropertyName("contoso_budgetcategorycode")]
        public string? BudgetCategoryCode { get; set; }

        /// <summary>
        /// gets or sets BudgetCategoryName field.
        /// </summary>
        [JsonPropertyName("contoso_budgetcategoryname")]
        public string? BudgetCategoryName { get; set; }
    }
}
