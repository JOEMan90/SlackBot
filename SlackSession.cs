using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackBot
{
    class SlackSession
    {
        public string genChannel, linkChannel, testChannel;
        public JObject startResp;

        public Dictionary<string, SlackUser> userList; //This will allow the code to look up user properties based on short username

        public SlackSession() { }

        public SlackSession(string token)
        {
            //Generate the RTM.Start response using the provided OAuth token
            string startURL = "https://slack.com/api/rtm.start";
            var client = new RestClient(startURL);
            var json = client.MakeRequest("?token=" + token);

            this.startResp = JObject.Parse(json);

            //FOR loop to set the channel ID's via parsing the RTM.Start response
            //the continue statements skip to the next iteration if a condition is satisfied to prevent some unnecessary tests
            for (int i = 0; i < startResp["channels"].Count(); i++)
            {
                if (startResp["channels"][i]["name"].ToString() == "general")
                {
                    this.genChannel = (string)startResp["channels"][i]["id"];
                    continue;
                }
                else if (startResp["channels"][i]["name"].ToString() == "links")
                {
                    this.linkChannel = (string)startResp["channels"][i]["id"];
                    continue;
                }
                else if (startResp["channels"][i]["name"].ToString() == "testing")
                {
                    this.testChannel = (string)startResp["channels"][i]["id"];
                    continue;
                }
            }//for

            //Create the user list when the session is initialized
            this.userList = createUserList(startResp);

        }//constructor

        public Dictionary<string, SlackUser> createUserList(JObject startResp)
        {
            //This function creates the user list by parsing the RTM.Start response for every user
            //It returns a dictionary that will allow someone to look up user properties based on username

            Dictionary<string, SlackUser> userList = new Dictionary<string, SlackUser>();

            for (int i = 0; i < startResp["users"].Count(); i++)
            {
                SlackUser newUser = new SlackUser();
                newUser.shortName = (string)startResp["users"][i]["name"];
                newUser.uID = (string)startResp["users"][i]["id"];

                //FOR loop to get direct message channels
                for (int n = 0; n < startResp["ims"].Count(); n++)
                {
                    if (startResp["ims"][n]["user"].ToString() == newUser.uID)
                    {
                        newUser.dmChannel = (string)startResp["ims"][n]["id"];
                    }
                }//dmChannel FOR

                userList.Add(newUser.shortName, newUser);
            }//newUser add FOR

            return userList;
        }//method

        public void SendMessage(string channel, string text)
        {
            string urlWithAccessToken = "https://hooks.slack.com/services/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";

            SlackWebhook client = new SlackWebhook(urlWithAccessToken);

            client.PostMessage(username: "pmayhem",
                text: text,
                channel: channel);
        }//method
    }
}
