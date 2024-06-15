using System.Diagnostics;
using System.Net;
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

        var uri = $"https://music.yandex.ru/handlers/playlist.jsx?owner={owner}&kinds={kinds}";

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Обработка...");
        Console.ResetColor();

        var client = new WebClient();
        var response = JsonSerializer.Deserialize<PlaylistResponse>(client.OpenRead(uri), new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

        Playlist playlist = response.Playlist;

        using (TextWriter writer = new StreamWriter($"{playlist.Title}.txt"))
        {
            foreach (string line in playlist.GetPlaylistLines())
            {
                writer.WriteLine(line);
            }
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Готово!");
        Console.ResetColor();

        Console.WriteLine($"Название плейлиста: {playlist.Title}\n");
        Console.WriteLine($"Список треков распечатан ниже и сохранен рядом с файлом программы (файл {playlist.Title}.txt).");

        Process.Start(new ProcessStartInfo($"{playlist.Title}.txt") { UseShellExecute = true });
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