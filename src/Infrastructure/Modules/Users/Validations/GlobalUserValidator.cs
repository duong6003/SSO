using Core.Common.Interfaces;
using Infrastructure.Modules.Users.Entities;
using Infrastructure.Modules.Users.Requests;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Modules.Users.Validations;

public interface IGlobalUserValidator : IScopedService
{
    public Task<bool> IsActive(string userName);
}

public class GlobalUserValidator : IGlobalUserValidator
{
    private readonly IRepositoryWrapper RepositoryWrapper;

    public GlobalUserValidator(IRepositoryWrapper repositoryWapper)
    {
        RepositoryWrapper = repositoryWapper;
    }
    public async Task<bool> IsActive(string userName)
    {
        User? user = await RepositoryWrapper.Users.Find(x=> x.UserName == userName || x.EmailAddress == userName).FirstOrDefaultAsync();
        if (user == null) return true;
        return user.Active;
    }
}

