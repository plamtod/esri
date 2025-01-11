using Demographic.Domain.Entities;

namespace Demographic.Infrastructure.Servieces.Contracts
{
    public interface IPopulationService
    {
        /// <summary>
        /// save state population
        /// </summary>
        /// <param name="locations"></param>
        /// <returns></returns>
        Task SaveStatePopulationsAsync(Dictionary<string, int> locations);

        /// <summary>
        /// Get state population
        /// </summary>
        /// <param name="stateNameFilter"></param>
        /// <returns></returns>
        Task<List<Population>> GetStatePopulationAsync(string? stateNameFilter = null);
    }
}