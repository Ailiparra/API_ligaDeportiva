using SportsLeague.DataAccess.Context;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsLeague.DataAccess.Repositories;

public class SponsorRepository : GenericRepository<Sponsor>, ISponsorRepository
{
    public SponsorRepository(LeagueDbContext context) : base(context)
    {
    }

    // Implementación del método específico
    public async Task<bool> NameExistsAsync(string name)
    {
        // Usamos _dbSet que es protected en el GenericRepository
        return await _dbSet.AnyAsync(s => s.Name.ToLower() == name.ToLower());
    }
}