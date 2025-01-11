namespace DemographicService.API.Responses.Population
{
    using Demographic.Domain.Entities;
    public record PopulationResponse(string StateName, int Population, string DateUpdated) 
    {
        public static PopulationResponse FromPopulation(Population population) => 
            new(population.StateName, population.PopulationCount, population.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ss"));

    }
}
