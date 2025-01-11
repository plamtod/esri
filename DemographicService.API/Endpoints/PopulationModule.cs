using Demographic.Infrastructure.Servieces.Contracts;
using DemographicService.API.Abstractions;
using DemographicService.API.Responses.Population;

namespace DemographicService.API.Endpoints
{
    public class PopulationModule : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("/population", async (IPopulationService populationService, string? stateName = null) =>
            {
                var population = await populationService.GetStatePopulationAsync(stateName);
                return Results.Ok(population.Select(PopulationResponse.FromPopulation).ToList());
            })
            .MapToApiVersion(1)
            .Produces<List<PopulationResponse>>(StatusCodes.Status200OK)
            .WithName("GetStatesPopulation")
            .WithOpenApi();
        }
    }
}
