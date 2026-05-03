using ShiftCore.Dtos;
using ShiftCore.Entity;
using System.Runtime.CompilerServices;

namespace ShiftCore.Mapping;

public static class CreateWorkerMap
{
    public static Worker ToEntity(this CreateWorkerDto dto) 
    {
        return new Worker
        {
            Id = Guid.NewGuid(),
            FullName = dto.FullName,
            Role = dto.Role,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }
}
