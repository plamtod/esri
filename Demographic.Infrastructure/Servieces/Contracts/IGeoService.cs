namespace Demographic.Infrastructure.Servieces.Contracts
{
    public interface IGeoService
    {
        /// <summary>
        /// get populations of all states
        /// </summary>
        /// <param name="client">Http client</param>
        /// <returns></returns>
        Task<Dictionary<string, int>> GetPopulationsAsync(HttpClient client);
    }
}