using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SlackBot
{
    class SlackCommand : SlackSession
    {
        public Dictionary<string, bool> commandList = new Dictionary<string, bool>() //Holds all commands and whether they are true or not.
        {
            {"oprah", false},
        };

        private SlackSession slkSession;

        public SlackCommand(SlackSession currentSession)
        {
            this.slkSession = currentSession;
        }

        public void CheckForCommands(JObject latestMessages)
        {
            string searchText;

            for (int i = 0; i < latestMessages["messages"].Count(); i++)
            {
                //.oprah
                string pattern = @".oprah (.+)";
                Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                searchText = (string)latestMessages["messages"][i]["text"];
                Match match = rgx.Match(searchText);

                if (match.Success && match.Groups[1].ToString() != "dinosaur" && match.Groups[1].ToString() != "bread")
                {
                    cmd_Oprah(match.Groups[1].ToString());
                    continue;
                }

                //checklinks (pattern 1)
                pattern = @"<(.+)\|.+>"; //Slack almost always puts URLs into a "<fullURL|postedURL>" format
                rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                match = rgx.Match(searchText);

                if (match.Success)
                {
                    SendMessage("links", match.Groups[1].ToString()); //Post link to #links
                    continue;
                }

                //checklinks (pattern 2)
                pattern = @"<(.+)>"; //The second URL format Slack uses "<postedURL>" format
                rgx = new Regex(pattern, RegexOptions.IgnoreCase);
                match = rgx.Match(searchText);

                if (match.Success)
                {
                    SendMessage("links", match.Groups[1].ToString()); //Post link to #links
                    continue;
                }

            }

        }

        public void cmd_Oprah(string oprahText)
        {
            //.oprah <oprahText>
            //Publicly spams oprahText to all users in the current channel

            //Generate text for direct channels
            string userOText;
            userOText = "And _YOU_ get a " + oprahText + "!!!";

            //Loop through list of all users and send via direct message channel (To cause email spam too)
            foreach (var user in slkSession.userList.Keys)
            {
                SendMessage(slkSession.userList[user].dmChannel, userOText);
            }

            //Generate text for general channel
            string genOText = "*" + oprahText + "s for everyone!!!!!*";

            //Send to general channel
            SendMessage("general", genOText);
        }//method

    }
}
