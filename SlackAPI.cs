using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackBot
{
    class SlackAPI
    {
        public SlackAPI() { }

        public string channelsHistory(string token, string channel, string oldestTime = null)
        {
            string url = "https://slack.com/api/channels.history";
            url += "?token=" + token;
            url += "&channel=" + channel;
            if (oldestTime != null)
            {
                url += "&oldest=" + oldestTime;
            }

            var client = new RestClient(url);
            var json = client.MakeRequest();

            return json;
        }//method
    }
}
