using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface ISelectListService
{
    Task<IEnumerable<SelectListItem>> GetDepartamentosSelectListAsync();
    Task<IEnumerable<SelectListItem>> GetEspecialidadesSelectListAsync();
    Task<IEnumerable<SelectListItem>> GetRolesSelectListAsync();
}
