using Demographic.Domain.Models;
using Demographic.Infrastructure.Servieces.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Demographic.Infrastructure.Servieces
{
    internal sealed class GeoService(IConfiguration configuration, ILogger<GeoService> logger): IGeoService
    {

        public async Task<Dictionary<string, int>> GetPopulationsAsync(HttpClient client)
        {
            int recordCount = configuration.GetValue<int>("GeoService:recordCount");
            string baseUrl = configuration.GetValue<string>("GeoService:url")!;
            Dictionary<string, int> statesPopulation = [];
            GeoResponse response;

            try
            {
                int i = 0;
                do
                {
                    var url = string.Format(baseUrl, (i * recordCount), recordCount);
                    var httpResponse = await client.GetAsync(url);
                    httpResponse.EnsureSuccessStatusCode();
                    string responseContent = await httpResponse.Content.ReadAsStringAsync();
                    response = JsonConvert.DeserializeObject<GeoResponse>(responseContent)!;
                    
                    foreach (var item in response.StateCounties)
                    {
                        UpdateStatesPopulation(statesPopulation, item.StateCounty.StateName, item.StateCounty.Population);
                    }

                    i++;
                } while (response.ExceededTransferLimit);

                return statesPopulation;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error getting populations");
                return [];
            }
        }

        private void UpdateStatesPopulation(Dictionary<string, int> statesPopulation, string stateName, int? count)
        {
            if (statesPopulation.TryGetValue(stateName, out int currentCount))
            { 
                statesPopulation[stateName] = currentCount + (count ?? 0);
            }
            else 
            { 
                statesPopulation[stateName] = count ?? 0;
            }
        }
    }
}
