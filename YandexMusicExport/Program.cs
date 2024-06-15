using System.Diagnostics;
using System.Net;
using System.Text;
using System.Text.Json;

namespace YandexMusicExport;

internal static class Program
{
    private static void Main()
    {
        Greet();

        Console.Write("Введите ссылку на плейлист Яндекс Музыки >>> ");
        var uriRaw = Console.ReadLine();
        if (uriRaw == null)
        {
            Console.WriteLine("No input. Terminating.");
            return;
        }

        Uri inputUri = new(uriRaw);
        if (inputUri.Segments.Length != 5)
        {
            Console.WriteLine("Wrong URL. Terminating.");
            return;
        }

        var owner = inputUri.Segments[2].Trim('/');
        var kinds = inputUri.Segments[4].Trim('/');

        var uri = "https://music.yandex.ru/handlers/playlist.jsx?owner=" + owner + "&kinds=" + kinds;

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Обработка...\n");
        Console.ResetColor();

        try
        {
            var client = new WebClient();
            var response = JsonSerializer.Deserialize<PlaylistResponse>(client.OpenRead(uri), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var playlistTitle = response?.Playlist.Title;
            var tracks = response?.Playlist.Tracks;

            StringBuilder allFile = new();

            foreach (var track in tracks!)
            {
                var artistsNames = "";

                foreach (var artist in track.Artists) artistsNames += artist.Name + ", ";

                artistsNames = artistsNames.TrimEnd(',', ' ');

                var fullTrack = artistsNames + " - " + track.Title + "\n";

                allFile.Append(fullTrack);
            }

            File.WriteAllText($"{playlistTitle}.txt", allFile.ToString());

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Готово!\n");
            Console.ResetColor();

            Console.WriteLine($"Название плейлиста: {playlistTitle}\n" +
                              $"Список треков распечатан ниже и сохранен рядом с файлом программы (файл {playlistTitle}.txt).\n");

            Process.Start(new ProcessStartInfo($"{playlistTitle}.txt") { UseShellExecute = true });

        }
        catch (JsonException e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Ошибка! Несуществующий плейлист или временный бан от Яндекса. Проверьте ссылку " +
                              "и попробуйте еще раз через некоторое время или на другом устройстве. Также есть сайт " +
                              "https://files.u-pov.ru/programs/YandexMusicExport" +
                              ", но велика вероятность, что там " +
                              "вообще ничего не будет работать :) \n" +
                              "Если вообще ничего не помогает, напишите мне https://t.me/aleqsanbr.");
            Console.ResetColor();
            Console.WriteLine("\nДополнительная информация:");
            Console.WriteLine(e);
        }

        catch (IndexOutOfRangeException e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Ошибка! Вероятно, некорректная ссылка. Проверьте, чтобы она была вида " +
                              "https://music.yandex.ru/users/USERNAME/playlists/PLAYLIST_ID, т. е. без лишних " +
                              "параметров. Попробуйте еще раз. Если ничего не работает, напишите мне https://t.me/aleqsanbr.");
            Console.ResetColor();
            Console.WriteLine("\nДополнительная информация:");
            Console.WriteLine(e);
        }

        catch (Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Ошибка! Проверьте правильность ссылки и попробуйте еще раз. " +
                              "Также учтите, что из-за большого количества запросов может последовать временный " +
                              "бан от Яндекса. В таком случае попробуйте с другого устройства или на сайте " +
                              "https://files.u-pov.ru/programs/YandexMusicExport" + ". " +
                              "Если ничего не работает, напишите мне https://t.me/aleqsanbr.");
            Console.ResetColor();
            Console.WriteLine("\nДополнительная информация:");
            Console.WriteLine(e);

        }

        Console.WriteLine("\nДля закрытия нажмите любую клавишу...");
        Console.ReadKey();
    }

    private static void Greet()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.BackgroundColor = ConsoleColor.DarkMagenta;
        Console.Write("=== Экспорт Яндекс Музыки ===");
        Console.ResetColor();

        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine(" by https://t.me/aleqsanbr\n");
        Console.ResetColor();

        Console.WriteLine("!! ИНФОРМАЦИЯ !!");
        Console.WriteLine("Данная программа позволяет экспортировать любой плейлист Яндекс Музыки в текстовое " +
                          "представление ИМЯ ИСПОЛНИТЕЛЯ - НАЗВАНИЕ ТРЕКА.\n" +
                          "1. Скопируйте и вставьте ниже ссылку на плейлист. Обязательно проверьте, чтобы она была " +
                          "вида https://music.yandex.ru/users/USERNAME/playlists/PLAYLIST_ID.\n" +
                          "2. Если плейлист большой, может потребоваться некоторое время для обработки.\n" +
                          "3. Если ссылка корректная, но возникает ошибка, то, вероятно, сработал \"бан\" со " +
                          "стороны Яндекса. В таком случае попробуйте еще раз через некоторое время или на " +
                          "другом устройстве. Также есть сайт https://files.u-pov.ru/programs/YandexMusicExport, " +
                          "но там обычно вообще ничего не работает, так как все запросы пользователей посылаются с " +
                          "одного адреса.\n" +
                          "4. Вам необязательно вручную копировать весь вывод. Каждый раз автоматически создается " +
                          "файл НАЗВАНИЕ_ПЛЕЙЛИСТА.txt рядом с программой.\n" +
                          "5. Предложения, критика и прочее принимаются тута: https://t.me/aleqsanbr. В описании " +
                          "ссылка, подпишитесь на канал :)" +
                          "\n");
    }
}

internal class PlaylistResponse
{
    public required PlaylistData Playlist { get; set; }
}

internal class PlaylistData
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