namespace YandexMusicExport;
internal class PlaylistResponse
{
    public required Playlist Playlist { get; set; }
}

internal class Playlist
{
    public required string Title { get; set; }
    public required Track[] Tracks { get; set; }
}

static class PlaylistExtensions
{
    public static IEnumerable<string> GetPlaylistLines(this Playlist playlist)
    {
        foreach (Track track in playlist.Tracks)
        {
            yield return track.GetTextRecord();
        }
    }
}

internal class Track
{
    public required string Title { get; set; }
    public required Artist[] Artists { get; set; }
}

static class TrackExtensions
{
    public static string GetTextRecord(this Track track)
    {
        var artistsNames = string.Join(", ", track.Artists.Select(x => x.Name));
        return artistsNames + " - " + track.Title;
    }
}

internal class Artist
{
    public required string Name { get; set; }
}