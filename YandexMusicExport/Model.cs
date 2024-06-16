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

internal class Track
{
    public required string Title { get; set; }
    public required Artist[] Artists { get; set; }
}

internal class Artist
{
    public required string Name { get; set; }
}