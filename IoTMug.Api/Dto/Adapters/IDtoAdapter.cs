using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IoTMug.Api.Dto.Adapters
{
    public interface IDtoAdapter<TEntity, TDto>
    {
        TEntity ConvertToEntity(TDto dto);
        TDto ConvertToDto(TEntity entity);
        IEnumerable<TEntity> ConvertToEntities(IEnumerable<TDto> dto);
        IEnumerable<TDto> ConvertToDtos(IEnumerable<TEntity> entities);
    }
}
