using FluentAssertions;
using Jellyfin.Plugin.SmartLists.Core.Models;
using Xunit;

namespace Jellyfin.Plugin.SmartLists.Tests.Core.Models;

public class IgnoredTrackTests
{
    [Fact]
    public void Create_WithDuration_SetsExpiryDate()
    {
        // Arrange
        var trackId = Guid.NewGuid().ToString();
        var playlistId = Guid.NewGuid().ToString();
        var userId = Guid.NewGuid().ToString();
        var durationDays = 7;

        // Act
        var ignore = IgnoredTrack.Create(trackId, playlistId, userId, durationDays);

        // Assert
        ignore.TrackId.Should().Be(trackId);
        ignore.SmartPlaylistId.Should().Be(playlistId);
        ignore.UserId.Should().Be(userId);
        ignore.DurationDays.Should().Be(durationDays);
        ignore.ExpiresAt.Should().NotBeNull();
        ignore.ExpiresAt.Should().BeCloseTo(DateTime.UtcNow.AddDays(durationDays), TimeSpan.FromSeconds(5));
        ignore.Id.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Create_WithNullDuration_HasNoExpiryDate()
    {
        // Arrange
        var trackId = Guid.NewGuid().ToString();
        var playlistId = Guid.NewGuid().ToString();
        var userId = Guid.NewGuid().ToString();

        // Act
        var ignore = IgnoredTrack.Create(trackId, playlistId, userId, null);

        // Assert
        ignore.DurationDays.Should().BeNull();
        ignore.ExpiresAt.Should().BeNull();
    }

    [Fact]
    public void IsExpired_WithFutureExpiry_ReturnsFalse()
    {
        // Arrange
        var ignore = IgnoredTrack.Create(
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            7);

        // Act & Assert
        ignore.IsExpired().Should().BeFalse();
        ignore.IsActive().Should().BeTrue();
    }

    [Fact]
    public void IsExpired_WithPastExpiry_ReturnsTrue()
    {
        // Arrange
        var ignore = new IgnoredTrack
        {
            Id = Guid.NewGuid().ToString(),
            TrackId = Guid.NewGuid().ToString(),
            SmartPlaylistId = Guid.NewGuid().ToString(),
            UserId = Guid.NewGuid().ToString(),
            IgnoredAt = DateTime.UtcNow.AddDays(-10),
            DurationDays = 7,
            ExpiresAt = DateTime.UtcNow.AddDays(-3) // Expired 3 days ago
        };

        // Act & Assert
        ignore.IsExpired().Should().BeTrue();
        ignore.IsActive().Should().BeFalse();
    }

    [Fact]
    public void IsExpired_WithNullExpiry_ReturnsFalse()
    {
        // Arrange - permanent ignore
        var ignore = IgnoredTrack.Create(
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            null);

        // Act & Assert
        ignore.IsExpired().Should().BeFalse();
        ignore.IsActive().Should().BeTrue();
    }

    [Fact]
    public void UpdateDuration_ChangesExpiryDate()
    {
        // Arrange
        var ignore = IgnoredTrack.Create(
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            7);

        var originalExpiry = ignore.ExpiresAt;

        // Act
        ignore.UpdateDuration(14);

        // Assert
        ignore.DurationDays.Should().Be(14);
        ignore.ExpiresAt.Should().BeAfter(originalExpiry!.Value);
    }

    [Fact]
    public void UpdateDuration_ToNull_RemovesExpiryDate()
    {
        // Arrange
        var ignore = IgnoredTrack.Create(
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            7);

        // Act
        ignore.UpdateDuration(null);

        // Assert
        ignore.DurationDays.Should().BeNull();
        ignore.ExpiresAt.Should().BeNull();
    }

    [Fact]
    public void Create_WithMetadata_PreservesMetadata()
    {
        // Arrange & Act
        var ignore = IgnoredTrack.Create(
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString(),
            30,
            trackName: "Test Track",
            artistName: "Test Artist",
            albumName: "Test Album",
            reason: "Tired of it");

        // Assert
        ignore.TrackName.Should().Be("Test Track");
        ignore.ArtistName.Should().Be("Test Artist");
        ignore.AlbumName.Should().Be("Test Album");
        ignore.Reason.Should().Be("Tired of it");
    }
}
