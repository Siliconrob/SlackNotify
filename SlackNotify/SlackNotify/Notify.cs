using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace SlackNotify
{
    public class Notify
    {
        const string WEBHOOK_URL = "https://hooks.slack.com/services/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";

        /// <summary>
        /// sends the message to slack, to the #general channel
        /// </summary>
        /// <param name="message">the message to send</param>
        /// <returns>true if successful; otherwise false</returns>
        public static async Task<bool> Send(string message)
        {
            return await Send("", message, "");
        }


        /// <summary>
        /// send the message to slack, to the specified channel
        /// </summary>
        /// <param name="channel">the channel to send the message to; if empty string it will default to #general</param>
        /// <param name="message">the message to send</param>
        /// <returns>true if successful; otherwise false</returns>
        public static async Task<bool> Send(string channel, string message, string user)
        {
            bool result = false;

            if (string.IsNullOrEmpty(channel))
            {
                channel = "#general";
            }

            var client = new HttpClient();

            try
            {
                var payload = new payload();
                payload.channel = channel;
                payload.text = message;
                if (string.IsNullOrEmpty(user) == false)
                {
                    payload.username = user;
                }

                // Get the response.
                HttpResponseMessage response = await client.PostAsJsonAsync(WEBHOOK_URL, payload);

                // Get the response content.
                result = response.IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error : " + e.Message);
            }

            

            return result;
        }
    }



    class payload
    {
        public string channel="";
        public string text = "";
        public string username = "airmax-bot";
    }
}
