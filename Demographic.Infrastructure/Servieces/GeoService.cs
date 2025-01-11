using Demographic.Domain.Models;
using Demographic.Infrastructure.Servieces.Contracts;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Demographic.Infrastructure.Servieces
{
    internal sealed class GeoService(ILogger<GeoService> logger): IGeoService
    {

        public async Task<Dictionary<string, int>> GetPopulationsAsync(HttpClient client)
        {
            Dictionary<string, int> statesPopulation = [];
            GeoResponse response;
            int recordCount = 100;

            try
            {
                int i = 0;
                do
                {
                    var url = $"https://services.arcgis.com/P3ePLMYs2RVChkJx/ArcGIS/rest/services/USA_Census_Counties/FeatureServer/0/query?where=1%3D1&outFields=population%2C+state_name&returnGeometry=false&f=json&&resultRecordCount&resultOffset={i * recordCount}&resultRecordCount={recordCount}";
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
