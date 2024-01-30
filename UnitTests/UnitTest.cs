using DAL.Repositories;
using Services;
using DAL.Models;
using System.Data;

namespace UnitTests;

public class UnitTest
{
    bool EqualsByProperties(ValuesAtMinute lhs, ValuesAtMinute rhs)
    {
        return lhs.ParticularMinute == rhs.ParticularMinute && lhs.TotalValue == rhs.TotalValue;
    }

    bool EqualsByItemsProperties(List<ValuesAtMinute> lhs, List<ValuesAtMinute> rhs)
    {
        for (int i = 0; i < lhs.Count; i++)
        {
            if (!EqualsByProperties(lhs[i], rhs[i]))
            {
                return false;
            }
        }
        return true;
    }


    public class InMemoryRepository : IBaseRepository<Event, Guid>
    {
        public readonly List<Event> entities = [];

        public IQueryable<Event> GetAllQuery() => entities.AsQueryable();
        public Task<Event> Create(Event model)
        {
            entities.Add(model);

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
        InMemoryRepository inMemoryRepository = new();
        EventService eventService = new(inMemoryRepository);

        Guid passedGuid = new(id);
        DateTime passedDateTime = DateTime.Parse(dateTime);

        Event passedEvent = new() { Id = passedGuid, Name = name, Value = value, CreateDateTime = passedDateTime };
        await eventService.Create(passedEvent);

        List<Event> events = inMemoryRepository.entities;

        Assert.True(events.FirstOrDefault(passedEvent) == passedEvent);
    }

    [Fact]
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

        Assert.True(EqualsByItemsProperties(methodResult, expectedResult));
    }

    [Fact]
    public async void GetValuesAtMinutesTestWithEventsAtDifferentMinutesRanges()
    {
        Event[] testEvents =
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

        foreach (var @event in testEvents)
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

        Assert.True(EqualsByItemsProperties(methodResult, expectedResult));
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

        Assert.True(EqualsByItemsProperties(methodResult, expectedResult));
    }

    [Fact]
    public async void GetValuesAtMinutesTestWithFromArgument()
    {
        Event[] testEvents =
        [
            new() { Id = new Guid("14d10317-9f29-4cc8-bd76-4ba0806d3f11"), Name = "Event 1", Value = 5, CreateDateTime = new DateTime(2022, 6, 5, 14, 14, 00) },
            new() { Id = new Guid("14d10317-9f29-4cc8-bd76-4ba0806d3f11"), Name = "Event 2", Value = 5, CreateDateTime = new DateTime(2022, 8, 10, 14, 14, 45) },
            new() { Id = new Guid("14d10317-9f29-4cc8-bd76-4ba0806d3f12"), Name = "Event 3", Value = 5, CreateDateTime = new DateTime(2022, 10, 23, 14, 15, 13) },
            //from (2022, 11, 10, 14, 15, 15)
            //(2022, 11, 10, 14, 15, 00)
            new() { Id = new Guid("14d10317-9f29-4cc8-bd76-4ba0806d3f13"), Name = "Event 4", Value = 5, CreateDateTime = new DateTime(2022, 11, 10, 14, 15, 22) },
            new() { Id = new Guid("14d10317-9f29-4cc8-bd76-4ba0806d3f14"), Name = "Event 5", Value = 5, CreateDateTime = new DateTime(2022, 11, 10, 14, 15, 47) },
            //(2034, 12, 2, 14, 16, 00)
            new() { Id = new Guid("14d10317-9f29-4cc8-bd76-4ba0806d3f15"), Name = "Event 6", Value = 5, CreateDateTime = new DateTime(2034, 12, 2, 14, 16, 48) },
            new() { Id = new Guid("14d10317-9f29-4cc8-bd76-4ba0806d3f16"), Name = "Event 7", Value = 5, CreateDateTime = new DateTime(2034, 12, 2, 14, 16, 08) },
            new() { Id = new Guid("14d10317-9f29-4cc8-bd76-4ba0806d3f16"), Name = "Event 7", Value = 5, CreateDateTime = new DateTime(2034, 12, 2, 14, 16, 25) },
            //(2323, 12, 2, 14, 15, 00)
            new() { Id = new Guid("14d10317-9f29-4cc8-bd76-4ba0806d3f16"), Name = "Event 8", Value = 5, CreateDateTime = new DateTime(2323, 12, 2, 14, 15, 59) }
        ];

        EventService eventService = new(new InMemoryRepository());

        foreach (var @event in testEvents)
        {
            await eventService.Create(@event);
        }

        DateTime from = new(2022, 11, 10, 14, 15, 15);

        List<ValuesAtMinute> methodResult = await eventService.GetValuesAtMinutes(from, null);

        List<ValuesAtMinute> expectedResult =
        [
           new ValuesAtMinute() {ParticularMinute = new DateTime(2022, 11, 10, 14, 15, 00) , TotalValue = 10},
           new ValuesAtMinute() {ParticularMinute = new DateTime(2034, 12, 2, 14, 16, 00), TotalValue = 15},
           new ValuesAtMinute() {ParticularMinute = new DateTime(2323, 12, 2, 14, 15, 00) , TotalValue = 5}
        ];

        Assert.True(EqualsByItemsProperties(methodResult, expectedResult));
    }

    [Fact]
    public async void GetValuesAtMinutesTestWithToArgument()
    {
        Event[] testEvents =
        [
            //(2022, 6, 5, 14, 14, 00) - 5
            new() { Id = new Guid("14d10317-9f29-4cc8-bd76-4ba0806d3f11"), Name = "Event 1", Value = 5, CreateDateTime = new DateTime(2022, 6, 5, 14, 14, 00) },
            //(2022, 8, 10, 14, 14, 45) - 5
            new() { Id = new Guid("14d10317-9f29-4cc8-bd76-4ba0806d3f11"), Name = "Event 2", Value = 5, CreateDateTime = new DateTime(2022, 8, 10, 14, 14, 45) },
            //(2022, 10, 23, 14, 15, 13) - 5
            new() { Id = new Guid("14d10317-9f29-4cc8-bd76-4ba0806d3f12"), Name = "Event 3", Value = 5, CreateDateTime = new DateTime(2022, 10, 23, 14, 15, 13) },
            //(2022, 11, 10, 14, 15, 00) - 10
            new() { Id = new Guid("14d10317-9f29-4cc8-bd76-4ba0806d3f13"), Name = "Event 4", Value = 5, CreateDateTime = new DateTime(2022, 11, 10, 14, 15, 22) },
            new() { Id = new Guid("14d10317-9f29-4cc8-bd76-4ba0806d3f14"), Name = "Event 5", Value = 5, CreateDateTime = new DateTime(2022, 11, 10, 14, 15, 47) },
            //(2034, 12, 2, 14, 16, 00) - 10
            new() { Id = new Guid("14d10317-9f29-4cc8-bd76-4ba0806d3f16"), Name = "Event 7", Value = 5, CreateDateTime = new DateTime(2034, 12, 2, 14, 16, 08) },
            new() { Id = new Guid("14d10317-9f29-4cc8-bd76-4ba0806d3f16"), Name = "Event 7", Value = 5, CreateDateTime = new DateTime(2034, 12, 2, 14, 16, 25) },
            //to (2034, 12, 2, 14, 16, 26)
            new() { Id = new Guid("14d10317-9f29-4cc8-bd76-4ba0806d3f15"), Name = "Event 6", Value = 5, CreateDateTime = new DateTime(2034, 12, 2, 14, 16, 48) },
            //(2323, 12, 2, 14, 15, 00)
            new() { Id = new Guid("14d10317-9f29-4cc8-bd76-4ba0806d3f16"), Name = "Event 8", Value = 5, CreateDateTime = new DateTime(2323, 12, 2, 14, 15, 59) }
        ];

        EventService eventService = new(new InMemoryRepository());

        foreach (var @event in testEvents)
        {
            await eventService.Create(@event);
        }

        DateTime to = new(2034, 12, 2, 14, 16, 26);

        List<ValuesAtMinute> methodResult = await eventService.GetValuesAtMinutes(null, to);

        List<ValuesAtMinute> expectedResult =
        [
           new ValuesAtMinute() {ParticularMinute = new DateTime(2022, 6, 5, 14, 14, 00)  , TotalValue = 5},
           new ValuesAtMinute() {ParticularMinute = new DateTime(2022, 8, 10, 14, 14, 00), TotalValue = 5},
           new ValuesAtMinute() {ParticularMinute = new DateTime(2022, 10, 23, 14, 15, 00) , TotalValue = 5},
           new ValuesAtMinute() {ParticularMinute = new DateTime(2022, 11, 10, 14, 15, 00) , TotalValue = 10},
           new ValuesAtMinute() {ParticularMinute = new DateTime(2034, 12, 2, 14, 16, 00)  , TotalValue = 10}
        ];

        Assert.True(EqualsByItemsProperties(methodResult, expectedResult));
    }

    [Fact]
    public async void GetValuesAtMinutesTestWithFromAndToArguments()
    {
        Event[] testEvents =
        [
            new() { Id = new Guid("14d10317-9f29-4cc8-bd76-4ba0806d3f11"), Name = "Event 1", Value = 5, CreateDateTime = new DateTime(2022, 6, 5, 14, 14, 00) },
            new() { Id = new Guid("14d10317-9f29-4cc8-bd76-4ba0806d3f11"), Name = "Event 2", Value = 5, CreateDateTime = new DateTime(2022, 8, 10, 14, 14, 45) },
            //from (2022, 10, 23, 14, 15, 9)
            //(2022, 10, 23, 14, 15, 13) - 5
            new() { Id = new Guid("14d10317-9f29-4cc8-bd76-4ba0806d3f12"), Name = "Event 3", Value = 5, CreateDateTime = new DateTime(2022, 10, 23, 14, 15, 13) },
            //(2022, 11, 10, 14, 15, 00) - 10
            new() { Id = new Guid("14d10317-9f29-4cc8-bd76-4ba0806d3f13"), Name = "Event 4", Value = 5, CreateDateTime = new DateTime(2022, 11, 10, 14, 15, 22) },
            new() { Id = new Guid("14d10317-9f29-4cc8-bd76-4ba0806d3f14"), Name = "Event 5", Value = 5, CreateDateTime = new DateTime(2022, 11, 10, 14, 15, 47) },
            //(2034, 12, 2, 14, 16, 00) - 10
            new() { Id = new Guid("14d10317-9f29-4cc8-bd76-4ba0806d3f16"), Name = "Event 7", Value = 5, CreateDateTime = new DateTime(2034, 12, 2, 14, 16, 08) },
            new() { Id = new Guid("14d10317-9f29-4cc8-bd76-4ba0806d3f16"), Name = "Event 7", Value = 5, CreateDateTime = new DateTime(2034, 12, 2, 14, 16, 25) },
            //to (2034, 12, 2, 14, 16, 26)
            new() { Id = new Guid("14d10317-9f29-4cc8-bd76-4ba0806d3f15"), Name = "Event 6", Value = 5, CreateDateTime = new DateTime(2034, 12, 2, 14, 16, 48) },
            new() { Id = new Guid("14d10317-9f29-4cc8-bd76-4ba0806d3f16"), Name = "Event 8", Value = 5, CreateDateTime = new DateTime(2323, 12, 2, 14, 15, 59) }
        ];

        EventService eventService = new(new InMemoryRepository());

        foreach (var @event in testEvents)
        {
            await eventService.Create(@event);
        }

        DateTime from = new(2022, 10, 23, 14, 15, 9);
        DateTime to = new(2034, 12, 2, 14, 16, 26);

        List<ValuesAtMinute> methodResult = await eventService.GetValuesAtMinutes(from, to);

        List<ValuesAtMinute> expectedResult =
        [
           new ValuesAtMinute() {ParticularMinute = new DateTime(2022, 10, 23, 14, 15, 00)  , TotalValue = 5},
           new ValuesAtMinute() {ParticularMinute = new DateTime(2022, 11, 10, 14, 15, 00), TotalValue = 10},
           new ValuesAtMinute() {ParticularMinute = new DateTime(2034, 12, 2, 14, 16, 00) , TotalValue = 10}
        ];

        Assert.True(EqualsByItemsProperties(methodResult, expectedResult));
    }
}