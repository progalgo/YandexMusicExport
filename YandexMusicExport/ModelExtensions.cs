namespace YandexMusicExport;

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

static class TrackExtensions
{
    public static string GetTextRecord(this Track track)
    {
        var artistsNames = string.Join(", ", track.Artists.Select(x => x.Name));
        return artistsNames + " - " + track.Title;
    }
}