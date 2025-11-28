using FluentAssertions;
using Jellyfin.Plugin.SmartLists.Core.Models;
using Xunit;

namespace Jellyfin.Plugin.SmartLists.Tests.Core.Models;

public class UserSmartPlaylistDtoTests
{
    [Fact]
    public void NewPlaylist_HasDefaultValues()
    {
        // Act
        var playlist = new UserSmartPlaylistDto { Name = "Test" };

        // Assert
        playlist.Public.Should().BeFalse();
        playlist.Enabled.Should().BeTrue();
        playlist.DefaultIgnoreDurationDays.Should().Be(30);
        playlist.MediaTypes.Should().BeEmpty();
    }

    [Fact]
    public void Playlist_CanSetAllProperties()
    {
        // Arrange
        var id = Guid.NewGuid().ToString();
        var userId = Guid.NewGuid().ToString();
        var sourcePlaylistId = Guid.NewGuid().ToString();
        var jellyfinPlaylistId = Guid.NewGuid().ToString();

        // Act
        var playlist = new UserSmartPlaylistDto
        {
            Id = id,
            UserId = userId,
            Name = "My Smart Playlist",
            SourcePlaylistId = sourcePlaylistId,
            Public = true,
            Enabled = true,
            MaxItems = 100,
            MaxPlayTimeMinutes = 60,
            JellyfinPlaylistId = jellyfinPlaylistId,
            DefaultIgnoreDurationDays = 14,
            MediaTypes = ["Audio"]
        };

        // Assert
        playlist.Id.Should().Be(id);
        playlist.UserId.Should().Be(userId);
        playlist.Name.Should().Be("My Smart Playlist");
        playlist.SourcePlaylistId.Should().Be(sourcePlaylistId);
        playlist.Public.Should().BeTrue();
        playlist.MaxItems.Should().Be(100);
        playlist.MaxPlayTimeMinutes.Should().Be(60);
        playlist.JellyfinPlaylistId.Should().Be(jellyfinPlaylistId);
        playlist.DefaultIgnoreDurationDays.Should().Be(14);
        playlist.MediaTypes.Should().Contain("Audio");
    }
}
