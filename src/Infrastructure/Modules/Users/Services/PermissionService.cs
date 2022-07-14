using AutoMapper;
using Core.Common.Interfaces;
using Infrastructure.Definitions;
using Infrastructure.Modules.Users.Entities;
using Infrastructure.Modules.Users.Requests;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Core.Utilities;

namespace Infrastructure.Modules.Users.Services
{
    public interface IPermissionService : IScopedService
    {
        Task<Permission?> GetByIdAsync(string permissionCode);

        Task<PaginationResponse<Permission>?> GetAllAsync(PaginationRequest request);
        Task<PaginationResponse<GetPermissionResponse>?> GetAllGroupByNameAsync(PaginationRequest request);

        Task CreateAsync(CreatePermissionRequest request);

        Task UpdateAsync(string permissionCode, UpdatePermissionRequest request);

        Task<string?> DeleteAsync(string permissionCode);
    }

    public class PermissionService : IPermissionService
    {
        private readonly IRepositoryWrapper RepositoryWrapper;
        private readonly IMapper Mapper;

        public PermissionService(IRepositoryWrapper repositoryWrapper, IMapper mapper)
        {
            RepositoryWrapper = repositoryWrapper;
            Mapper = mapper;
        }

        public async Task CreateAsync(CreatePermissionRequest request)
        {
            Permission? permission = Mapper.Map<Permission>(request);
            await RepositoryWrapper.Permissions.AddAsync(permission);
        }

        public async Task<string?> DeleteAsync(string permissionCode)
        {
            Permission? permission = await GetByIdAsync(permissionCode);
            if (permission == null)
            {
                return Messages.Permissions.CodeNotFound;
            }
            await RepositoryWrapper.Permissions.DeleteAsync(permission);
            return null;
        }

        public async Task<PaginationResponse<Permission>?> GetAllAsync(PaginationRequest request)
        {
            IQueryable<Permission>? permissions = RepositoryWrapper.Permissions.Find(x =>
                (
                    string.IsNullOrEmpty(request.Search)
                    || x.Name!.ToLower().Contains(request.Search!.ToLower())
                    || x.Code!.ToLower().Contains(request.Search!.ToLower())
                ));
            permissions = SortUtility<Permission>.ApplySort(permissions, request.OrderByQuery!);
            PaginationUtility<Permission>? data = await PaginationUtility<Permission>.ToPagedListAsync(permissions, request.Current, request.PageSize);
            return PaginationResponse<Permission>.PaginationInfo(data, data.PageInfo);
        }

        public async Task<PaginationResponse<GetPermissionResponse>?> GetAllGroupByNameAsync(PaginationRequest request)
        {
            IQueryable<Permission>? permissions = RepositoryWrapper.Permissions.Find();
            PaginationUtility<Permission>? data = await PaginationUtility<Permission>.ToPagedListAsync(permissions, request.Current, request.PageSize);
            return PaginationResponse<GetPermissionResponse>.PaginationInfo(
                data.GroupBy(x => x.Name!.Split(" ")[0])
                .Select( x=> new GetPermissionResponse(){ Group = x.Key, Permissions = x.ToList()})
                .ToList(), data.PageInfo);
        }

        public async Task<Permission?> GetByIdAsync(string permissionCode)
        {
            return await RepositoryWrapper.Permissions.GetByIdAsync(permissionCode);
        }

        public async Task UpdateAsync(string permissionCode, UpdatePermissionRequest request)
        {
            Permission? permission = await GetByIdAsync(permissionCode);
            Mapper.Map(request, permission);
            await RepositoryWrapper.Permissions.UpdateAsync(permission!);
        }
    }
}