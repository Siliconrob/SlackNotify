using System;
using System.IO;
using System.Net;
using System.Text;

using Newtonsoft.Json;

namespace SlackNotify
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
    public static bool Send(FileSystemInfo optionsFile)
    {
      var settings = ReadSettings(optionsFile);
      var result = false;

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

        var client = new WebClient();
        var responsedata = client.UploadString(settings.WebHookURL, sb.ToString());
        result = responsedata.Equals("ok");
      }
      catch (Exception e)
      {
        Console.WriteLine("Error : " + e.Message);
      }

      return result;
    }

    /// <summary>
    /// Checks for a valid URI and URI Scheme
    /// </summary>
    /// <param name="uriName">The Uri to test</param>
    /// <returns>true if valid; otherwise false</returns>
    private static bool IsValidUri(string uriName, bool Https = true)
    {
      Uri uriResult;
      var result = Uri.TryCreate(uriName, UriKind.Absolute, out uriResult);
      if (result && Https)
      {
        result = uriResult.Scheme == Uri.UriSchemeHttps;
      }
      return result;
    }
  }
}
