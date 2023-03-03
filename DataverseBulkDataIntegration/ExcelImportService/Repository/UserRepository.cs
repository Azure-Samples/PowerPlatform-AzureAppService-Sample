// <copyright file="UserRepository.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace ExcelImportService.Repository
{
    using System.Text.Json;
    using Dataverse.RestClient;
    using ExcelImportService.Constants;
    using ExcelImportService.Models.Response;

    /// <summary>
    /// Repository implementation for User entity.
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private IDataverseClient dataverseClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRepository"/> class.
        /// </summary>
        /// <param name="dataverseClient">IDataverseClient.</param>
        public UserRepository(IDataverseClient dataverseClient)
        {
            this.dataverseClient = dataverseClient;
        }

        /// <inheritdoc/>
        public virtual async Task<GetUserResponse> GetByNameAsync(string? fullname)
        {
            var users = await this.dataverseClient.ListAsync<GetUserResponse>(
                EntityNames.UserEntityName,
                filter: $"contains(fullname, '{fullname}')",
                withAnnotations: true,
                convert: (jsonElement, _) => jsonElement.Deserialize<GetUserResponse>() ?? new GetUserResponse());
            return users.FirstOrDefault() ?? new();
        }
    }
}
