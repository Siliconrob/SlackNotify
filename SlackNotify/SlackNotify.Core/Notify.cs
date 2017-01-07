using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SlackNotify.Core
{
  public static class Notify
  {
    /// <summary>
    /// Initialises an instance of the Notify class. Requires that a json config file exists containing the webhook URL.
    /// </summary>
    /// <exception cref="FileNotFoundException"></exception>
    /// <exception cref="UriFormatException"></exception>
    private static Settings ReadSettings(FileSystemInfo optionsFile)
    {
      if (!optionsFile.Exists)
      {
        throw new FileNotFoundException("Config file not found", optionsFile.Name);
      }
      var settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(optionsFile.FullName));
      if (!IsValidUri(settings.WebHookURL))
      {
        throw new UriFormatException("Invalid WebHookURL");
      }
      return settings;
    }

    /// <summary>
    /// send the message to slack, to the specified channel
    /// </summary>
    /// <exception cref="UriFormatException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <returns>true if successful; otherwise false</returns>
    public static async Task<bool> SendAsync(FileSystemInfo optionsFile)
    {
      var settings = ReadSettings(optionsFile);
      if (IsValidUri(settings.WebHookURL) == false) { throw new UriFormatException("Invalid Slack WebHook URL");}

      try
      {
        var ch = settings.Channel ?? "";
        var us = settings.Username ?? "";
        var av = settings.Avatar ?? "";
        var message = settings.Message ?? "";

        var sb = new StringBuilder();
        sb.Append("{");
        sb.AppendFormat("\"text\": \"{0}\"", message);
        if (string.IsNullOrEmpty(ch) == false) sb.AppendFormat(",\"channel\": \"{0}\"", ch);
        if (string.IsNullOrEmpty(us) == false) sb.AppendFormat(",\"username\": \"{0}\"", us);
        if (string.IsNullOrEmpty(av) == false)
        {
          sb.AppendFormat(IsValidUri(av, false) ? ",\"icon_url\": \"{0}\"" : ",\"icon_emoji\": \"{0}\"", av);
        }
        sb.Append("}");

        var client = new HttpClient();
        var responsedata = await client.PostAsync(settings.WebHookURL, new StringContent(sb.ToString()));
        return responsedata.StatusCode == HttpStatusCode.OK;
      }
      catch (Exception e)
      {
        Console.WriteLine("Error : " + e.Message);
      }
      return false;
    }

    /// <summary>
    /// Checks for a valid URI and URI Scheme
    /// </summary>
    /// <param name="uriName">The Uri to test</param>
    /// <param name="Https">Send requests https instead of http</param>
    /// <returns>true if valid; otherwise false</returns>
    private static bool IsValidUri(string uriName, bool Https = true)
    {
      Uri uriResult;
      var result = Uri.TryCreate(uriName, UriKind.Absolute, out uriResult);
      if (result && Https)
      {
        result = string.Compare(uriResult.Scheme, "https", StringComparison.OrdinalIgnoreCase) == 0;
      }
      return result;
    }
  }
}
