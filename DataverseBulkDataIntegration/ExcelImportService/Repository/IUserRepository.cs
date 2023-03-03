// <copyright file="IUserRepository.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace ExcelImportService.Repository
{
    using ExcelImportService.Models.Response;

    /// <summary>
    /// User entity repository interface.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Get user id by name.
        /// </summary>
        /// <param name="fullName">User full name.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<GetUserResponse> GetByNameAsync(string? fullName);
    }
}
