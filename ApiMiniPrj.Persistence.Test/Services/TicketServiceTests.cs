using ApiMiniPrj.Application.DTOs.Tickets;
using ApiMiniPrj.Domain.Enums;
using ApiMiniPrj.Domain.Models.Events;
using ApiMiniPrj.Domain.Models.Tickets;
using ApiMiniPrj.Persistence.Services;
using Microsoft.EntityFrameworkCore;

namespace ApiMiniPrj.Persistence.Test.Services;

public class TicketServiceTests
{
    [Fact]
    public async Task CreateTicketAsync_WhenEventExists_ShouldPersistTicket()
    {
        await using var context = ServiceTestFactory.CreateContext();
        context.Events.Add(new Event { Id = 1, Title = "Event", Description = "Description", Location = "Baku", Date = DateTime.UtcNow.AddDays(1) });
        await context.SaveChangesAsync();
        var service = new TicketService(context, ServiceTestFactory.CreateMapper());

        await service.CreateTicketAsync(new TicketCreateDto
        {
            EventId = 1,
            Type = TicketTypeEnum.VIP,
            Quantity = 3,
            Price = 50,
            IsAvaiable = true
        });

        var ticket = await context.Tickets.SingleAsync();
        Assert.Equal(1, ticket.EventId);
        Assert.Equal(TicketTypeEnum.VIP, ticket.Type);
        Assert.Equal(3, ticket.Quantity);
        Assert.Equal(50, ticket.Price);
    }

    [Fact]
    public async Task CreateTicketAsync_WhenEventDoesNotExist_ShouldThrow()
    {
        await using var context = ServiceTestFactory.CreateContext();
        var service = new TicketService(context, ServiceTestFactory.CreateMapper());

        var exception = await Assert.ThrowsAsync<Exception>(() => service.CreateTicketAsync(new TicketCreateDto { EventId = 99 }));

        Assert.Equal("Event not found.", exception.Message);
    }

    [Fact]
    public async Task UpdateTicketAsync_ShouldOnlyApplyProvidedValues()
    {
        await using var context = ServiceTestFactory.CreateContext();
        context.Events.Add(new Event { Id = 1, Title = "Event", Description = "Description", Location = "Baku", Date = DateTime.UtcNow.AddDays(1) });
        context.Tickets.Add(new Ticket { Id = 1, EventId = 1, Type = TicketTypeEnum.Standard, Quantity = 5, Price = 10, IsAvaiable = true });
        await context.SaveChangesAsync();
        var service = new TicketService(context, ServiceTestFactory.CreateMapper());

        await service.UpdateTicketAsync(1, new TicketUpdateDto { Quantity = 8, Price = 20, IsAvaiable = true });

        var ticket = await context.Tickets.SingleAsync();
        Assert.Equal(TicketTypeEnum.Standard, ticket.Type);
        Assert.Equal(8, ticket.Quantity);
        Assert.Equal(20, ticket.Price);
        Assert.True(ticket.IsAvaiable);
    }

    [Fact]
    public async Task DeleteTicketAsync_ShouldSoftDeleteTicket()
    {
        await using var context = ServiceTestFactory.CreateContext();
        context.Events.Add(new Event { Id = 1, Title = "Event", Description = "Description", Location = "Baku", Date = DateTime.UtcNow.AddDays(1) });
        context.Tickets.Add(new Ticket { Id = 1, EventId = 1, Quantity = 5, Price = 10 });
        await context.SaveChangesAsync();
        var service = new TicketService(context, ServiceTestFactory.CreateMapper());

        await service.DeleteTicketAsync(1);

        var ticket = await context.Tickets.IgnoreQueryFilters().SingleAsync();
        Assert.True(ticket.IsDeleted);
        Assert.Equal("System", ticket.DeletableBy);
    }

    [Fact]
    public async Task GetAllTicketsAsync_ShouldExcludeDeletedTickets()
    {
        await using var context = ServiceTestFactory.CreateContext();
        context.Events.Add(new Event { Id = 1, Title = "Event", Description = "Description", Location = "Baku", Date = DateTime.UtcNow.AddDays(1) });
        context.Tickets.AddRange(
            new Ticket { Id = 1, EventId = 1, Quantity = 5, Price = 10 },
            new Ticket { Id = 2, EventId = 1, Quantity = 2, Price = 15, IsDeleted = true });
        await context.SaveChangesAsync();
        var service = new TicketService(context, ServiceTestFactory.CreateMapper());

        var result = await service.GetAllTicketsAsync();

        var ticket = Assert.Single(result);
        Assert.Equal(1, ticket.Id);
    }

    [Fact]
    public async Task GetTicketByIdAsync_WhenTicketIsMissing_ShouldReturnMappedNullResult()
    {
        await using var context = ServiceTestFactory.CreateContext();
        var service = new TicketService(context, ServiceTestFactory.CreateMapper());

        var result = await service.GetTicketByIdAsync(99);

        Assert.Null(result);
    }
}
