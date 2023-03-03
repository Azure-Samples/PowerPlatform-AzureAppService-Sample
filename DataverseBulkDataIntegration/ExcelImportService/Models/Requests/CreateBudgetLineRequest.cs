// <copyright file="CreateBudgetLineRequest.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace ExcelImportService.Models.Requests
{
    using System.Text.Json.Serialization;
    using ExcelImportService.Constants;

    /// <summary>
    /// Model to create Budget Line entity record.
    /// </summary>
    public class CreateBudgetLineRequest
    {
        private string? budgetHolderId;
        private string? budgetCategoryId;
        private string? budgetHeaderId;

        /// <summary>
        /// Gets or sets BudgetLineId field.
        /// </summary>
        [JsonIgnore]
        public Guid? BudgetLineId { get; set; }

        /// <summary>
        /// Gets or sets BudgetLineName field.
        /// </summary>
        [JsonPropertyName("contoso_budgetlinename")]
        public string? BudgetLineName { get; set; }

        /// <summary>
        /// Gets or sets BudgetHolderId field.
        /// </summary>
        [JsonPropertyName("contoso_budgetholder@odata.bind")]
        public string BudgetHolderId
        {
            get
            {
                return string.IsNullOrEmpty(this.budgetHolderId) ? string.Empty : $"/{EntityNames.UserEntityName}({this.budgetHolderId})";
            }

            set
            {
                this.budgetHolderId = value;
            }
        }

        /// <summary>
        /// Gets or sets BudgetCategoryId field.
        /// </summary>
        [JsonPropertyName("contoso_budgetcategory@odata.bind")]
        public string BudgetCategoryId
        {
            get
            {
                return string.IsNullOrEmpty(this.budgetCategoryId) ? string.Empty : $"/{EntityNames.BudgetCategoryEntityName}({this.budgetCategoryId})";
            }

            set
            {
                this.budgetCategoryId = value;
            }
        }

        /// <summary>
        /// Gets the BudgetCategoryIdGuid field.
        /// </summary>
        [JsonIgnore]
        public Guid BudgetCategoryIdGuid
        {
            get
            {
                return string.IsNullOrEmpty(this.budgetCategoryId) ? Guid.Empty : Guid.Parse(this.budgetCategoryId);
            }
        }

        /// <summary>
        /// Gets or sets BudgetHeaderId field.
        /// </summary>
        [JsonPropertyName("contoso_budgetheader@odata.bind")]
        public string BudgetHeaderId
        {
            get
            {
                return string.IsNullOrEmpty(this.budgetHeaderId) ? string.Empty : $"/{EntityNames.BudgetHeaderEntityName}({this.budgetHeaderId})";
            }

            set
            {
                this.budgetHeaderId = value;
            }
        }

        /// <summary>
        /// Gets the BudgetHeaderIdGuid field.
        /// </summary>
        [JsonIgnore]
        public Guid BudgetHeaderIdGuid
        {
            get
            {
                return string.IsNullOrEmpty(this.budgetHeaderId) ? Guid.Empty : Guid.Parse(this.budgetHeaderId);
            }
        }

        /// <summary>
        /// Gets or sets BudgetedAmountQ1 field.
        /// </summary>
        [JsonPropertyName("contoso_budgetedamountq1")]
        public float? BudgetedAmountQ1 { get; set; }

        /// <summary>
        /// Gets or sets BudgetedAmountQ2 field.
        /// </summary>
        [JsonPropertyName("contoso_budgetedamountq2")]
        public float? BudgetedAmountQ2 { get; set; }

        /// <summary>
        /// Gets or sets BudgetedAmountQ3 field.
        /// </summary>
        [JsonPropertyName("contoso_budgetedamountq3")]
        public float? BudgetedAmountQ3 { get; set; }

        /// <summary>
        /// Gets or sets BudgetedAmountQ4 field.
        /// </summary>
        [JsonPropertyName("contoso_budgetedamountq4")]
        public float? BudgetedAmountQ4 { get; set; }

        /// <summary>
        /// Gets or sets TotalBudgetedAmount field.
        /// </summary>
        [JsonPropertyName("contoso_totalbudgetedamount")]
        public float? TotalBudgetedAmount { get; set; }
    }
}
