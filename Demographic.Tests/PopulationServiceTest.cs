using Demographic.Domain.Entities;
using Demographic.Infrastructure;
using Demographic.Infrastructure.Servieces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;

namespace Demographic.Tests
{
    public class PopulationServiceTest
    {
        private Mock<ILogger<PopulationService>> _logger;

        private readonly Mock<IMemoryCache> _cacheMock;
        private readonly PopulationContext _dbContext;
        PopulationService populationService;

        public PopulationServiceTest()
        {
            _logger = new Mock<ILogger<PopulationService>>();
            _cacheMock = new Mock<IMemoryCache>();

            var options = new DbContextOptionsBuilder<PopulationContext>()
               .UseInMemoryDatabase(databaseName: "TestDatabase").Options;

            _dbContext = new PopulationContext(options);

            var cacheEntryMock = new Mock<ICacheEntry>();

            // Set up TryGetValue
            _cacheMock.Setup(mc => mc.TryGetValue(It.IsAny<object>(), out It.Ref<object>.IsAny))
                .Returns((object key, out object value) =>
                {
                    value = null; // or set to a cached value if needed
                    return false; // or true if the value is found in the cache
                });

            // Set up Set
            _cacheMock.Setup(mc => mc.CreateEntry(It.IsAny<object>()))
                .Returns(cacheEntryMock.Object);

            populationService = new PopulationService(_dbContext, _cacheMock.Object, _logger.Object);
        }

        [Fact]
        public async Task Save_Populations_Should_Pass()
        {
            //Arrange
            await ClearContext();
            var locations = new Dictionary<string, int>
                    {
                        { "California", 1000 },
                        { "Texas", 2000 }
                    };

            //Act
            await populationService.SaveStatePopulationsAsync(locations);
            var states = await _dbContext.Populations.OrderBy(x=>x.StateName).ToListAsync();

            //Assert
            Assert.Equal(2, states.Count);
            Assert.Equal(1000, states.First().PopulationCount);
        }

        [Fact]
        public async Task Get_Populations_Should_Pass()
        {
            //Arrange
            await ClearContext();
            _dbContext.Populations.AddRange(new List<Population> 
            { 
                new() { StateName="California", PopulationCount=1000 },
                new() { StateName="Texas", PopulationCount=2000} 
            });
            await _dbContext.SaveChangesAsync();
           
            //Act
            var states =  await populationService.GetStatePopulationAsync(default);

            //Assert
            Assert.Equal(2, states.Count);
        }

        private async Task ClearContext()
        {
            var states = await _dbContext.Populations.ToListAsync();
            _dbContext.Populations.RemoveRange(states);
            await _dbContext.SaveChangesAsync();
        }
    }

}