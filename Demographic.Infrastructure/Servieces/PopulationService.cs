using Demographic.Domain.Entities;
using Demographic.Infrastructure.Servieces.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Demographic.Infrastructure.Servieces
{
    public class PopulationService(PopulationContext context, IMemoryCache memoryCache, ILogger<PopulationService> logger) : IPopulationService
    {
        public async Task SaveStatePopulationsAsync(Dictionary<string, int> locations, CancellationToken cancellationToken = default)
        {
            //cache in memory
            var populations = await context.Populations.ToListAsync(cancellationToken);

            // Convert to list of named tuples
            var statesPopulations = locations.Select(kvp => (Name: kvp.Key, Count: kvp.Value)).ToList();

            foreach (var (Name, Count) in statesPopulations)
            {
                var result = populations.FirstOrDefault(x => x.StateName == Name);
                if (result == null)
                {
                    context.Populations.Add(new Population
                    {
                        StateName = Name,
                        PopulationCount = Count
                    });
                }
                else
                {
                    result.PopulationCount = Count;
                    result.CreatedAt = DateTime.UtcNow;
                }
            }

            await context.SaveChangesAsync(cancellationToken);
        }

        private readonly TimeSpan expiration = TimeSpan.FromMinutes(3);
        public async Task<List<Population>> GetStatePopulationAsync(string ? stateNameFilter = null, CancellationToken cancellationToken = default)
        {
            var cacheKey = $"States_{stateNameFilter??"all"}"; 
            
            if (memoryCache.TryGetValue(cacheKey, out List<Population> states))
            {
                return states; 
            }

            states = await GetStatePopulationFromDbAsync(stateNameFilter, cancellationToken); 

            var serializedStates = JsonConvert.SerializeObject(states);
            memoryCache.Set(cacheKey, serializedStates, expiration);

            return states;
        }

        private async Task<List<Population>> GetStatePopulationFromDbAsync(string? stateNameFilter = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var query = context.Populations.AsNoTracking();

                if (!string.IsNullOrEmpty(stateNameFilter))
                {
                    query = query.Where(x => EF.Functions.Like(x.StateName, $"%{stateNameFilter}%"));
                }

                return await query.ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while fetching state population");

                return [];
            }
            
        }
    }
}
