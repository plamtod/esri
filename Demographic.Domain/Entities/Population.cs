namespace Demographic.Domain.Entities
{
    public sealed class Population
    {
        public int Id { get; set; }
        public required string StateName { get; init; }
        public required int PopulationCount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
