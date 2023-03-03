using Dataverse.RestClient;
using ExcelImportService.Models.Requests;
using ExcelImportService.Models.Response;
using ExcelImportService.Repository;
using FakeItEasy;
using System.Net.Http.Json;
using System.Text.Json;

namespace ExcelImportService.Tests.Repository
{
    public class BudgetHeaderRepositoryTests
    {
        [Fact]
        public async void InsertAsync_Success()
        {
            var dataverseClient = A.Fake<IDataverseClient>();
            var budgetHeaderRepository = new BudgetHeaderRepository(dataverseClient);
            var toBeCreatedRecordId = Guid.NewGuid();
            var httpResponse = new HttpResponseMessage(System.Net.HttpStatusCode.Created)
            {
                Content = JsonContent.Create(JsonSerializer.Deserialize<GetBudgetHeaderResponse>($"{{\"contoso_budgetheaderid\": \"{toBeCreatedRecordId}\"}}"))
            };
            A.CallTo(() => dataverseClient.PostAsync(A<string>._,
                                                          A<string>._,
                                                          A<string>._,
                                                          A<Guid>._,
                                                          A<string>._,
                                                          A<string>._,
                                                          A<bool>._,
                                                          A<bool>._,
                                                          A<CancellationToken>._))
                .WithAnyArguments()
                .Returns(Task.FromResult(httpResponse));
            var result = await budgetHeaderRepository.InsertAsync(new CreateBudgetHeaderRequest());
            Assert.Equal(toBeCreatedRecordId, result.BudgetHeaderId);
        }
    }
}
