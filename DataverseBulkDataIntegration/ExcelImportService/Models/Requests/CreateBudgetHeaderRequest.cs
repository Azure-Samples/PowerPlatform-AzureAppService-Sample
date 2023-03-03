// <copyright file="CreateBudgetHeaderRequest.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace ExcelImportService.Models.Requests
{
    using System.Text.Json.Serialization;
    using ExcelImportService.Constants;

    /// <summary>
    /// Model to create Budget Header entity record.
    /// </summary>
    public class CreateBudgetHeaderRequest
    {
        private string? corporationId;
        private string? departmentId;

        /// <summary>
        /// Gets or sets BudgetHeaderName field.
        /// </summary>
        [JsonPropertyName("contoso_budgetheadername")]
        public string? BudgetHeaderName { get; set; }

        /// <summary>
        /// Gets or sets FiscalYear field.
        /// </summary>
        [JsonPropertyName("contoso_fiscalyear")]
        public int FiscalYear { get; set; }

        /// <summary>
        /// Gets or sets CorporationId field.
        /// </summary>
        [JsonPropertyName("contoso_corporation@odata.bind")]
        public string CorporationId
        {
            get
            {
                return string.IsNullOrEmpty(this.corporationId) ? string.Empty : $"/{EntityNames.CorporationEntityName}({this.corporationId})";
            }

            set
            {
                this.corporationId = value;
            }
        }

        /// <summary>
        /// Gets CorporationIdGuid field.
        /// </summary>
        [JsonIgnore]
        public Guid CorporationIdGuid
        {
            get
            {
                return string.IsNullOrEmpty(this.corporationId) ? Guid.Empty : Guid.Parse(this.corporationId);
            }
        }

        /// <summary>
        /// Gets or sets DepartmentId field.
        /// </summary>
        [JsonPropertyName("contoso_department@odata.bind")]
        public string DepartmentId
        {
            get
            {
                return string.IsNullOrEmpty(this.departmentId) ? string.Empty : $"/{EntityNames.DepartmentEntityName}({this.departmentId})";
            }

            set
            {
                this.departmentId = value;
            }
        }

        /// <summary>
        /// Gets DepartmentIdGuid field.
        /// </summary>
        [JsonIgnore]
        public Guid DepartmentIdGuid
        {
            get
            {
                return string.IsNullOrEmpty(this.departmentId) ? Guid.Empty : Guid.Parse(this.departmentId);
            }
        }

        /// <summary>
        /// Gets or sets BudgetAmount field.
        /// </summary>
        [JsonPropertyName("contoso_budgetamount")]
        public float? BudgetAmount { get; set; }
    }
}
