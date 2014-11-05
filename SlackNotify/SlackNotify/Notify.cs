using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
        public static bool Send(string message)
        {
            return Send("", message, "");
        }


        /// <summary>
        /// send the message to slack, to the specified channel
        /// </summary>
        /// <param name="channel">the channel to send the message to; if empty string it will default to #general</param>
        /// <param name="message">the message to send</param>
        /// <returns>true if successful; otherwise false</returns>
        public static bool Send(string channel, string message, string user)
        {
            bool result = false;

            if (string.IsNullOrEmpty(channel))
            {
                channel = "#general";
            }

            var client = new WebClient();

            try
            {
                var payload = new payload();
                payload.channel = channel;
                payload.text = message;
                if (string.IsNullOrEmpty(user) == false)
                {
                    payload.username = user;
                }

                string p = string.Format("{{\"channel\": \"{0}\", \"text\": \"{1}\", \"username\": \"{2}\"}}", payload.channel, payload.text, payload.username);

                NameValueCollection form = new NameValueCollection();
                form.Add("payload", p);

                byte[] responsedata = client.UploadValues(WEBHOOK_URL, form);

                if(responsedata.Length >= 2){
                    if (responsedata[0] == 0x6f && responsedata[1] == 0x6b)
                    {
                        result = true;
                    }
                }
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
