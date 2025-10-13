using InnomateApp.Domain.Entities;

namespace InnomateApp.Application.Interfaces;

public interface IPermissionRepository
{
    Task<IEnumerable<Permission>> GetByIdsAsync(IEnumerable<int> ids);
}
