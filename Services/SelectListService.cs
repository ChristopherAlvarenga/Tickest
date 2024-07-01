using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tickest.Data;
using Tickest.Models.Entities;

public class SelectListService : ISelectListService
{
    private readonly TickestContext _context;

    public SelectListService(TickestContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<SelectListItem>> GetDepartamentosSelectListAsync()
    {
        var departamentos = await _context.Departamentos
            .Select(d => new SelectListItem
            {
                Value = d.Id.ToString(),
                Text = d.Nome
            })
            .ToListAsync();

        return departamentos;
    }

    public async Task<IEnumerable<SelectListItem>> GetEspecialidadesSelectListAsync()
    {
        var especialidades = await _context.Especialidades
            .Select(e => new SelectListItem
            {
                Value = e.Id.ToString(),
                Text = e.Nome
            })
            .ToListAsync();

        return especialidades;
    }

    public async Task<IEnumerable<SelectListItem>> GetRolesSelectListAsync()
    {
        var roles = await _context.Roles
            .Select(r => new SelectListItem
            {
                Value = r.Id.ToString(),
                Text = r.Name
            })
            .ToListAsync();

        return roles;
    }
}
