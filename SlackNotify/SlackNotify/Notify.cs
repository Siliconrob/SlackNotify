using System;
using System.Collections.Generic;
//using System.Collections.Specialized;
using System.IO;
using System.Net;
//using System.Net.Http;
//using System.Runtime.Serialization.Json;
using System.Text;
//using System.Threading.Tasks;

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
        public Notify()
        {
            FileInfo fileInfo = new FileInfo(CONFIG_FILE);

            if (!fileInfo.Exists) throw new FileNotFoundException("Config file not found", CONFIG_FILE);

            StreamReader reader = new StreamReader(fileInfo.FullName);
            string configString = reader.ReadToEnd();
            config = JsonConvert.DeserializeObject<Config>(configString);

            if (IsValidUri(config.WebHookURL) == false) throw new UriFormatException("Invalid WebHookURL");
            
        }


        /// <summary>
        /// Initialises an instance of the Notify class. Doesn't require a json config file to exist.
        /// </summary>
        /// <param name="WebHookURL">URL provided by SlackHQ when configuring an 'Incoming WebHook' integration</param>
        public Notify(string WebHookURL){

            if (string.IsNullOrEmpty(WebHookURL)) throw new ArgumentException("Argument is null or empty");
            if (string.IsNullOrWhiteSpace(WebHookURL)) throw new ArgumentException("Argument is null or whitespace");

            config.WebHookURL = WebHookURL;

            if (IsValidUri(config.WebHookURL) == false) throw new UriFormatException("Invalid WebHookURL");
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


        public bool Send(string channel, string message)
        {
            return Send(channel, message, "", "");
        }


        public bool Send(string channel, string message, string user)
        {
            return Send(channel, message, user, "");
        }


        /// <summary>
        /// send the message to slack, to the specified channel
        /// </summary>
        /// <param name="channel">the channel to send the message to; if empty string it will default to #general</param>
        /// <param name="message">the message to send</param>
        /// <returns>true if successful; otherwise false</returns>
        public bool Send(string channel, string message, string user, string avatar)
        {
            bool result = false;

            if (IsValidUri(config.WebHookURL) == false)  throw new UriFormatException("Invalid WebHookURL");
            if (string.IsNullOrEmpty(message.Trim())) throw new ArgumentException("Message is empty");

            try
            {
                string ch = string.IsNullOrEmpty(channel) ? config.Channel : channel;
                string us = string.IsNullOrEmpty(user) ? config.Username : user;
                string av = string.IsNullOrEmpty(avatar) ? config.Avatar : avatar;

                StringBuilder sb = new StringBuilder();
                sb.Append("{");
                sb.AppendFormat("\"text\": \"{0}\"", message);
                if (string.IsNullOrEmpty(ch) == false) sb.AppendFormat(",\"channel\": \"{0}\"", ch);
                if (string.IsNullOrEmpty(us) == false) sb.AppendFormat(",\"username\": \"{0}\"", us);
                if (string.IsNullOrEmpty(av) == false) sb.AppendFormat(",\"icon_emoji\": \"{0}\"", av);
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
        /// Checks for a valid URI & URI Scheme, which needs to be Https
        /// </summary>
        /// <param name="uriName">The Uri to test</param>
        /// <returns>true if valid; otherwise false</returns>
        bool IsValidUri(string uriName)
        {
            Uri uriResult;
            return Uri.TryCreate(uriName, UriKind.Absolute, out uriResult) && uriResult.Scheme == Uri.UriSchemeHttps;
        }
    }



    
}
