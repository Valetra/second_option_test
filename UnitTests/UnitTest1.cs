using DAL.Repositories;
using Services;
using DAL.Models;

namespace UnitTests;

public class UnitTest1
{
    public class InMemoryRepository : IBaseRepository<Event, Guid>
    {
        private readonly List<Event> _entities = [];

        public IQueryable<Event> GetAllQuery() => _entities.AsQueryable();
        public Task<Event> Create(Event model)
        {
            _entities.Add(model);

            return Task.FromResult(model);
        }
    }

    readonly EventService eventService = new(new InMemoryRepository());


    [Theory]
    [InlineData("14d10317-9f29-4cc8-bd76-4ba0806d3f11", "Event 1", 5, "2022/1/10 12:00:00")]
    public async void CreateTest(string id, string name, int value, string dateTime)
    {
        Guid passedGuid = new(id);
        DateTime passedDateTime = DateTime.Parse(dateTime);

        Event passedEvent = new() { Id = passedGuid, Name = name, Value = value, CreateDateTime = passedDateTime };
        Event createdEvent = await eventService.Create(passedEvent);

        Assert.True(createdEvent == passedEvent);
    }

    [Theory]
    [InlineData("14d10317-9f29-4cc8-bd76-4ba0806d3f11", "Event 1", 5, "2022/1/10 12:00:00")]
    public async void GetValuesAtMinutesTestWithNullArguments(string id, string name, int value, string dateTime)
    {

        Guid passedGuid = new(id);
        DateTime passedDateTime = DateTime.Parse(dateTime);

        Event passedEvent = new() { Id = passedGuid, Name = name, Value = value, CreateDateTime = passedDateTime };
        await eventService.Create(passedEvent);

        List<ValuesAtMinute> methodResult = await eventService.GetValuesAtMinutes(null, null);

        List<ValuesAtMinute> expectedResult =
        [
           new ValuesAtMinute() {ParticularMinute = new DateTime(2022, 1, 10, 12, 00, 00), TotalValue = 5}
        ];

        Assert.True
        (
            Equals(methodResult.First().ParticularMinute, expectedResult.First().ParticularMinute) &&
            Equals(methodResult.First().TotalValue, expectedResult.First().TotalValue)
        );
    }

}