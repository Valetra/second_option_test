using System.Collections;

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

    public static bool CompareToResults(List<ValuesAtMinute> first, List<ValuesAtMinute> second)
    {
        var firstNotSecond = first.Except(second).ToList();
        var secondNotFirst = second.Except(first).ToList();

        return firstNotSecond.Count == 0 && secondNotFirst.Count == 0;
    }


    [Theory]
    [InlineData("14d10317-9f29-4cc8-bd76-4ba0806d3f11", "Event 1", 5, "2022/1/10 12:00:00")]
    public async void CreateTest(string id, string name, int value, string dateTime)
    {
        EventService eventService = new(new InMemoryRepository());

        Guid passedGuid = new(id);
        DateTime passedDateTime = DateTime.Parse(dateTime);

        Event passedEvent = new() { Id = passedGuid, Name = name, Value = value, CreateDateTime = passedDateTime };
        Event createdEvent = await eventService.Create(passedEvent);

        Assert.True(createdEvent == passedEvent);
    }



    [Theory]
    [InlineData]
    public async void GetValuesAtMinutesTestWithEventsAtOneMinuteRange()
    {
        Event[] testEventsInOneMinuteRange =
    [
        new() { Id = new Guid("14d10317-9f29-4cc8-bd76-4ba0806d3f11"), Name = "Event 1", Value = 5, CreateDateTime = new DateTime(2022, 12, 10, 14, 15, 1) },
        new() { Id = new Guid("14d10317-9f29-4cc8-bd76-4ba0806d3f12"), Name = "Event 2", Value = 5, CreateDateTime = new DateTime(2022, 12, 10, 14, 15, 13) },
        new() { Id = new Guid("14d10317-9f29-4cc8-bd76-4ba0806d3f13"), Name = "Event 3", Value = 5, CreateDateTime = new DateTime(2022, 12, 10, 14, 15, 22) },
        new() { Id = new Guid("14d10317-9f29-4cc8-bd76-4ba0806d3f14"), Name = "Event 4", Value = 5, CreateDateTime = new DateTime(2022, 12, 10, 14, 15, 35) },
        new() { Id = new Guid("14d10317-9f29-4cc8-bd76-4ba0806d3f15"), Name = "Event 5", Value = 5, CreateDateTime = new DateTime(2022, 12, 10, 14, 15, 48) },
        new() { Id = new Guid("14d10317-9f29-4cc8-bd76-4ba0806d3f16"), Name = "Event 6", Value = 5, CreateDateTime = new DateTime(2022, 12, 10, 14, 15, 59) }
    ];

        EventService eventService = new(new InMemoryRepository());

        foreach (var @event in testEventsInOneMinuteRange)
        {
            await eventService.Create(@event);
        }
        List<ValuesAtMinute> methodResult = await eventService.GetValuesAtMinutes(null, null);

        List<ValuesAtMinute> expectedResult =
        [
           new ValuesAtMinute() {ParticularMinute = new DateTime(2022, 12, 10, 14, 15, 00), TotalValue = 30}
        ];

        Assert.True
        (
             Equals(methodResult.First().ParticularMinute, expectedResult.First().ParticularMinute) &&
             Equals(methodResult.First().TotalValue, expectedResult.First().TotalValue)
        );
    }

    [Theory]
    [InlineData]
    public async void GetValuesAtMinutesTestWithEventsAtDifferentMinutesRanges()
    {
        Event[] testEventsInOneMinuteRange =
    [
        new() { Id = new Guid("14d10317-9f29-4cc8-bd76-4ba0806d3f11"), Name = "Event 1", Value = 5, CreateDateTime = new DateTime(2022, 12, 10, 14, 14, 00) },
        new() { Id = new Guid("14d10317-9f29-4cc8-bd76-4ba0806d3f11"), Name = "Event 2", Value = 5, CreateDateTime = new DateTime(2022, 12, 10, 14, 14, 45) },
        new() { Id = new Guid("14d10317-9f29-4cc8-bd76-4ba0806d3f12"), Name = "Event 3", Value = 5, CreateDateTime = new DateTime(2022, 12, 10, 14, 15, 13) },
        new() { Id = new Guid("14d10317-9f29-4cc8-bd76-4ba0806d3f13"), Name = "Event 4", Value = 5, CreateDateTime = new DateTime(2022, 12, 10, 14, 15, 22) },
        new() { Id = new Guid("14d10317-9f29-4cc8-bd76-4ba0806d3f14"), Name = "Event 5", Value = 5, CreateDateTime = new DateTime(2022, 2, 10, 14, 16, 35) },
        new() { Id = new Guid("14d10317-9f29-4cc8-bd76-4ba0806d3f15"), Name = "Event 6", Value = 5, CreateDateTime = new DateTime(2022, 2, 10, 14, 16, 48) },
        new() { Id = new Guid("14d10317-9f29-4cc8-bd76-4ba0806d3f16"), Name = "Event 7", Value = 5, CreateDateTime = new DateTime(2034, 6, 2, 14, 15, 00) },
        new() { Id = new Guid("14d10317-9f29-4cc8-bd76-4ba0806d3f16"), Name = "Event 8", Value = 5, CreateDateTime = new DateTime(2034, 6, 2, 14, 15, 59) }
    ];

        EventService eventService = new(new InMemoryRepository());

        foreach (var @event in testEventsInOneMinuteRange)
        {
            await eventService.Create(@event);
        }
        List<ValuesAtMinute> methodResult = await eventService.GetValuesAtMinutes(null, null);

        List<ValuesAtMinute> expectedResult =
        [
           new ValuesAtMinute() {ParticularMinute = new DateTime(2022, 12, 10, 14, 14, 00), TotalValue = 10},
           new ValuesAtMinute() {ParticularMinute = new DateTime(2022, 12, 10, 14, 15, 00), TotalValue = 10},
           new ValuesAtMinute() {ParticularMinute = new DateTime(2022, 2, 10, 14, 16, 00), TotalValue = 10},
           new ValuesAtMinute() {ParticularMinute = new DateTime(2034, 6, 2, 14, 15, 00), TotalValue = 10},
        ];

        Assert.True(Equals(expectedResult, methodResult));
        Console.WriteLine();
    }

    [Theory]
    [InlineData("14d10317-9f29-4cc8-bd76-4ba0806d3f11", "Event 1", 5, "2022/1/10 12:00:00")]
    public async void GetValuesAtMinutesTestWithNullArguments(string id, string name, int value, string dateTime)
    {
        EventService eventService = new(new InMemoryRepository());

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