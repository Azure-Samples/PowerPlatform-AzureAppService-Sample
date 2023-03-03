// <copyright file="GetBudgetHeaderResponse.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace ExcelImportService.Models.Response
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// Model for Budget Header entity.
    /// </summary>
    public class GetBudgetHeaderResponse
    {
        /// <summary>
        /// Gets or sets BudgetHeaderId field.
        /// </summary>
        [JsonPropertyName("contoso_budgetheaderid")]
        public Guid BudgetHeaderId { get; set; }

        /// <summary>
        /// Gets or sets FiscalYear field.
        /// </summary>
        [JsonPropertyName("contoso_fiscalyear")]
        public int? FiscalYear { get; set; }

        /// <summary>
        /// Gets or sets CorporationId field.
        /// </summary>
        [JsonPropertyName("_contoso_corporation_value")]
        public Guid? CorporationId { get; set; }

        /// <summary>
        /// Gets or sets DepartmentId field.
        /// </summary>
        [JsonPropertyName("_contoso_department_value")]
        public Guid? DepartmentId { get; set; }
    }
}
