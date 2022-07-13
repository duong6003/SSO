using Microsoft.EntityFrameworkCore;
using Infrastructure.App.DesignPatterns.Reponsitories;
using Infrastructure.Modules.Users.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Modules.Accounts.Services
{
    public interface IPermissionService
    {
        Task<List<Permission>> GetAll();
    }
    public class PermissionService : IPermissionService
    {
        private readonly IRepositoryWrapperMariaDB RepositoryWrapperMariaDb;

        public PermissionService(IRepositoryWrapperMariaDB repositoryWrapperMariaDb)
        {
            RepositoryWrapperMariaDb = repositoryWrapperMariaDb;
        }

        public async Task<List<Permission>> GetAll()
        {
            List<Permission> permissions = await RepositoryWrapperMariaDb.Permissions.FindAll().ToListAsync();
            return permissions;
        }
    }
}
