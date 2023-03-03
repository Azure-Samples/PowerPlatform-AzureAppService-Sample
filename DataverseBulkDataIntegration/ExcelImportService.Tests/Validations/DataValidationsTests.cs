using ExcelImportService.Models.Response;
using ExcelImportService.Validations;

namespace ExcelImportService.Tests.Validations
{
    public class DataValidationsTests
    {
        private readonly IDataValidations _dataValidations;

        public DataValidationsTests()
        {
            _dataValidations = new DataValidations();
        }

        [Fact]
        public void ValidateBudgetCategory_Success()
        {
            var validId = Guid.NewGuid();
            var masterData = new List<GetBudgetCategoryResponse>()
            {
                new GetBudgetCategoryResponse()
                {
                    BudgetCategoryName = "CAT1",
                    BudgetCategoryId = validId,
                },
                new GetBudgetCategoryResponse()
                {
                    BudgetCategoryName = "CAT2",
                    BudgetCategoryId = Guid.NewGuid(),
                }
            };
            var result = this._dataValidations.ValidateBudgetCategory(masterData, "CAT1");
            Assert.Equal(validId, result);
        }

        [Fact]
        public void ValidateBudgetCategory_Exception_WhenNotFound()
        {
            var validId = Guid.NewGuid();
            var masterData = new List<GetBudgetCategoryResponse>()
            {
                new GetBudgetCategoryResponse()
                {
                    BudgetCategoryName = "CAT1",
                    BudgetCategoryId = validId,
                },
                new GetBudgetCategoryResponse()
                {
                    BudgetCategoryName = "CAT2",
                    BudgetCategoryId = Guid.NewGuid(),
                }
            };
            Assert.Throws<Exception>(() => this._dataValidations.ValidateBudgetCategory(masterData, "INVALID"));
        }
    }
}
