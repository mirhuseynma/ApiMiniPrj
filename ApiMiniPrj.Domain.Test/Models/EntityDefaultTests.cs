using ApiMiniPrj.Domain.Models.Common;
using ApiMiniPrj.Domain.Models.Events;
using ApiMiniPrj.Domain.Models;
using ApiMiniPrj.Domain.Models.Organizers;
using ApiMiniPrj.Domain.Models.Tickets;
using ApiMiniPrj.Domain.Models.Users;

namespace ApiMiniPrj.Domain.Test.Models;

public class EntityDefaultTests
{
    [Fact]
    public void AuditableEntity_ShouldInitializeAuditFields()
    {
        var before = DateTime.Now.AddSeconds(-1);

        var entity = new AuditableEntity();

        Assert.Equal(string.Empty, entity.CreatedBy);
        Assert.Equal(string.Empty, entity.UpdatedBy);
        Assert.Equal(string.Empty, entity.DeletableBy);
        Assert.False(entity.IsDeleted);
        Assert.True(entity.CreatedDate >= before);
    }

    [Fact]
    public void Event_ShouldInitializeTicketsCollection()
    {
        var eventEntity = new Event();

        Assert.NotNull(eventEntity.Tickets);
        Assert.Empty(eventEntity.Tickets);
    }

    [Fact]
    public void Organizer_ShouldInitializeEventsCollection()
    {
        var organizer = new Organizer();

        Assert.NotNull(organizer.Events);
        Assert.Empty(organizer.Events);
    }

    [Fact]
    public void Ticket_ShouldBeAvailableByDefault()
    {
        var ticket = new Ticket();

        Assert.True(ticket.IsAvaiable);
        Assert.Equal(0, ticket.Quantity);
    }

    [Fact]
    public void AppUser_FullName_ShouldTrimMissingNameParts()
    {
        var fullUser = new AppUser { FirstName = "Test", LastName = "User" };
        var firstNameOnlyUser = new AppUser { FirstName = "Test" };

        Assert.Equal("Test User", fullUser.FullName);
        Assert.Equal("Test", firstNameOnlyUser.FullName);
    }

    [Fact]
    public void RefreshToken_ShouldKeepAssignedValues()
    {
        var id = Guid.NewGuid();
        var expires = DateTime.UtcNow.AddDays(15);
        var createdAt = DateTime.UtcNow;

        var refreshToken = new RefreshToken
        {
            Id = id,
            Token = "refresh-token",
            UserId = "user-id",
            Expires = expires,
            CreatedAt = createdAt,
            IsExpired = false
        };

        Assert.Equal(id, refreshToken.Id);
        Assert.Equal("refresh-token", refreshToken.Token);
        Assert.Equal("user-id", refreshToken.UserId);
        Assert.Equal(expires, refreshToken.Expires);
        Assert.Equal(createdAt, refreshToken.CreatedAt);
        Assert.False(refreshToken.IsExpired);
    }
}
