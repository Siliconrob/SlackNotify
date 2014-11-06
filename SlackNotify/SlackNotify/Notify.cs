using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

using Newtonsoft.Json;

namespace SlackNotify
{
    public class Notify
    {
        const string CONFIG_FILE = "notifyconfig.json";
        Config config = new Config();

        /// <summary>
        /// Initialises an instance of the Notify class. Requires that a json config file exists containing the webhook URL.
        /// </summary>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="UriFormatException"></exception>
        public Notify()
        {
            FileInfo fileInfo = new FileInfo(CONFIG_FILE);

            if (!fileInfo.Exists) throw new FileNotFoundException("Config file not found", CONFIG_FILE);

            StreamReader reader = new StreamReader(fileInfo.FullName);
            string configString = reader.ReadToEnd();
            config = JsonConvert.DeserializeObject<Config>(configString);

            if (IsValidUri(config.WebHookURL, true) == false) throw new UriFormatException("Invalid WebHookURL");
            
        }


        /// <summary>
        /// Initialises an instance of the Notify class. Doesn't require a json config file to exist.
        /// </summary>
        /// <param name="WebHookURL">URL provided by SlackHQ when configuring an 'Incoming WebHook' integration</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="UriFormatException"></exception>
        public Notify(string WebHookURL){

            if (string.IsNullOrEmpty(WebHookURL)) throw new ArgumentException("Argument is null or empty");
            if (string.IsNullOrWhiteSpace(WebHookURL)) throw new ArgumentException("Argument is null or whitespace");

            config.WebHookURL = WebHookURL;

            if (IsValidUri(config.WebHookURL, true) == false) throw new UriFormatException("Invalid WebHookURL");
        }


        /// <summary>
        /// sends the message to slack, to the #general channel
        /// </summary>
        /// <param name="message">the message to send</param>
        /// <returns>true if successful; otherwise false</returns>
        public bool Send(string message)
        {
            return Send("", message, "", "");
        }


        /// <summary>
        /// send the message to slack, to the specified channel
        /// </summary>
        /// <param name="channel">the channel to send the message to. if blank the message will be posted to the channel specified in the slack integration screen</param>
        /// <param name="message">the message to send</param>
        /// <returns>true if successful; otherwise false</returns>
        public bool Send(string channel, string message)
        {
            return Send(channel, message, "", "");
        }


        /// <summary>
        /// sends the message to slack, to the specified channel, as the specified username
        /// </summary>
        /// <param name="channel">the channle to send the message to. if blank the message will be posted to the channel specified in the slack integration screen</param>
        /// <param name="message">the message to send</param>
        /// <param name="username">the username the message will apear to come from. if blank the username will be the username specified in the slack integration screen</param>
        /// <returns>true if successful; otherwise false</returns>
        public bool Send(string channel, string message, string username)
        {
            return Send(channel, message, username, "");
        }


        /// <summary>
        /// send the message to slack, to the specified channel
        /// </summary>
        /// <param name="channel">the channel to send the message to. if blank the message will be posted to the channel specified in the slack integration screen</param>
        /// <param name="message">the message to send</param>
        /// <param name="username">the username the message will apear to come from. if blank the username will be the username specified in the slack integration screen</param>
        /// <param name="avatar">the icon to use. can be a fully formed absolute URI, or an emoji identifier (eg ":alien:"). See slack integration screens for info on changing the icon.</param>
        /// <exception cref="UriFormatException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <returns>true if successful; otherwise false</returns>
        public bool Send(string channel, string message, string username, string avatar)
        {
            bool result = false;

            if (IsValidUri(config.WebHookURL, true) == false)  throw new UriFormatException("Invalid Slack WebHook URL");
            if (string.IsNullOrEmpty(message.Trim())) throw new ArgumentException("Message is empty");

            try
            {
                string ch = string.IsNullOrEmpty(channel) ? config.Channel : channel;
                string us = string.IsNullOrEmpty(username) ? config.Username : username;
                string av = string.IsNullOrEmpty(avatar) ? config.Avatar : avatar;

                StringBuilder sb = new StringBuilder();
                sb.Append("{");
                sb.AppendFormat("\"text\": \"{0}\"", message);
                if (string.IsNullOrEmpty(ch) == false) sb.AppendFormat(",\"channel\": \"{0}\"", ch);
                if (string.IsNullOrEmpty(us) == false) sb.AppendFormat(",\"username\": \"{0}\"", us);
                if (string.IsNullOrEmpty(av) == false)
                {
                    if (IsValidUri(av))
                    {
                        sb.AppendFormat(",\"icon_url\": \"{0}\"", av);
                    }
                    else
                    {
                        sb.AppendFormat(",\"icon_emoji\": \"{0}\"", av);
                    }
                }
                sb.Append("}");

                var client = new WebClient();
                string responsedata = client.UploadString(config.WebHookURL, sb.ToString());
                result = responsedata.Equals("ok") ? true : false;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error : " + e.Message);
            }

            return result;
        }


        /// <summary>
        /// Send a preformatted json payload to slack. Useful if you need to send message attachments as specified in the slack integration screens
        /// </summary>
        /// <param name="payload">the json payload to send</param>
        /// <exception cref="UriFormatException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <returns>true if successful; otherwise false</returns>
        public bool SendPreformattedJson(string payload)
        {
            bool result = false;

            if (IsValidUri(config.WebHookURL, true) == false) throw new UriFormatException("Invalid Slack WebHook URL");
            if (string.IsNullOrEmpty(payload)) throw new ArgumentException("Payload is empty");

            try
            {
                var client = new WebClient();
                string responsedata = client.UploadString(config.WebHookURL, payload);
                result = responsedata.Equals("ok") ? true : false;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error : " + e.Message);
            }

            return result;
        }


        /// <summary>
        /// Checks for a valid URI and URI Scheme, which needs to be Https
        /// </summary>
        /// <param name="uriName">The Uri to test</param>
        /// <returns>true if valid; otherwise false</returns>
        bool IsValidUri(string uriName)
        {
            return IsValidUri(uriName, false);
        }


        /// <summary>
        /// Checks for a valid URI and URI Scheme
        /// </summary>
        /// <param name="uriName">The Uri to test</param>
        /// <returns>true if valid; otherwise false</returns>
        bool IsValidUri(string uriName, bool Https)
        {
            Uri uriResult;
            bool result = Uri.TryCreate(uriName, UriKind.Absolute, out uriResult);
            if (result && Https)
            {
                result = uriResult.Scheme == Uri.UriSchemeHttps;
            }
            return result;
        }
    }
  
}
