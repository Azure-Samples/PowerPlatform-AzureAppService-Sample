// <copyright file="DataValidations.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace ExcelImportService.Validations
{
    using ExcelImportService.Models.Common;
    using ExcelImportService.Models.Response;

    /// <summary>
    /// Class for data validations.
    /// </summary>
    public class DataValidations : IDataValidations
    {
        /// <inheritdoc/>
        public virtual Guid ValidateBudgetCategory(
            List<GetBudgetCategoryResponse> budgetCategoriesMasterData,
            string? budgetCategoryFromExcel)
        {
            var budgetCategory = budgetCategoriesMasterData.FirstOrDefault(b => b.BudgetCategoryName == budgetCategoryFromExcel);
            if (budgetCategory == null)
            {
                throw new Exception($"Invalid budget category: {budgetCategoryFromExcel}");
            }

            return budgetCategory.BudgetCategoryId;
        }

        /// <inheritdoc/>
        public virtual Guid ValidateCorporation(
            List<GetCorporationResponse> corporationMasterData,
            string? corporationFromExcel,
            string? parentCorporationFromExcel)
        {
            var corporation = corporationMasterData.FirstOrDefault(b => b.CorporationName == corporationFromExcel);
            if (corporation == null)
            {
                throw new Exception($"Invalid corporation: {corporationFromExcel}");
            }

            var parentCorporation = corporationMasterData.FirstOrDefault(b => b.CorporationName == parentCorporationFromExcel);
            if (parentCorporation == null)
            {
                throw new Exception($"Invalid parent corporation: {parentCorporationFromExcel}");
            }

            if (corporation.ParentCorporationId != parentCorporation.CorporationId)
            {
                throw new Exception($"Invalid hierarchy of corporarion {parentCorporation.CorporationName} -> {corporation.CorporationName}");
            }

            return corporation.CorporationId;
        }

        /// <inheritdoc/>
        public virtual Guid ValidateDepartment(
            List<GetDepartmentResponse> departmentMasterData,
            string? departmentFromExcel)
        {
            var department = departmentMasterData.FirstOrDefault(b => b.DepartmentName == departmentFromExcel);
            if (department == null)
            {
                throw new Exception($"Invalid department: {departmentFromExcel}");
            }

            return department.DepartmentId;
        }

        /// <inheritdoc/>
        public virtual bool TotalValidations(BudgetExcelRow row)
        {
            if (row.TotalBudgetedAmount != (row.BudgetedAmountQ1 + row.BudgetedAmountQ2 + row.BudgetedAmountQ3 + row.BudgetedAmountQ4))
            {
                throw new Exception("Total budget amount is not matching with sum of all Q's budget amount");
            }

            return true;
        }
    }
}
