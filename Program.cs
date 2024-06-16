using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public class ExeLinkParser
{
    private static readonly string url = @"https://discord\.com/api/webhooks/\d+/[A-Za-z0-9_-]+";

    private static HttpClient httpClient = new HttpClient();

    private static void Main(string[] args)
    {
        if (args.Length == 0) return;

        string filePath = args[0];

        try
        {
            string foundWebhook = FindWebhookUrl(filePath);

            while (foundWebhook != null)
            {
                HttpResponseMessage responseMessage = FetchDiscordUrl(foundWebhook, "sexo").GetAwaiter().GetResult();

                if (!responseMessage.IsSuccessStatusCode)
                {
                    Console.WriteLine("Error sending message");
                }

                Console.WriteLine("Message sended");
            }

            Console.WriteLine("Webhook not found");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }

        Console.ReadKey();
    }

    public static async Task<HttpResponseMessage> FetchDiscordUrl(string webhookUrl, string message)
    {
        var payload = new
        {
            content = message
        };

        var json = JsonConvert.SerializeObject(payload);

        var content = new StringContent(json, Encoding.UTF8, "application/json");

        return await httpClient.PostAsync(webhookUrl, content); ;
    }

    public static string FindWebhookUrl(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("File not found.");
        }

        byte[] fileBytes = File.ReadAllBytes(filePath);
        string fileContent = Encoding.ASCII.GetString(fileBytes);

        StringBuilder filteredContent = new StringBuilder();
        foreach (char c in fileContent)
        {
            if (c >= 32 && c <= 126)
            {
                filteredContent.Append(c);
            }
        }

        Regex regex = new Regex(url, RegexOptions.IgnoreCase);
        Match match = regex.Match(filteredContent.ToString());

        return match.Success ? match.Value : null;
    }
}