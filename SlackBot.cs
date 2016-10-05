using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SlackBot
{
    public partial class SlackBot : ServiceBase
    {

        //OAuth token, generated elsewhere. Should not expire.
        public string token = "xoxp-55924763041-55924763121-76497785393-0bb09c1a3a";

        //public SlackSession slackSession()
        //{
        //    SlackSession slackSession = new SlackSession(this.token);
        //    return slackSession;
        //}

        public SlackBot()
        {
            InitializeComponent();
            eventLog1 = new System.Diagnostics.EventLog();
            if (!System.Diagnostics.EventLog.SourceExists("SlackBot"))
            {
                System.Diagnostics.EventLog.CreateEventSource(
                    "SlackBot", "SlackBot");
            }
            eventLog1.Source = "SlackBot";
            eventLog1.Log = "SlackBot";
            slackSession = new SlackSession(token);
        }

        protected override void OnStart(string[] args)
        {
            eventLog1.WriteEntry("Service has started.");
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 30000;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimer);
            timer.Start();
        }

        protected override void OnStop()
        {
            eventLog1.WriteEntry("Service has been asked to stop.");
        }

        public void OnTimer(object sender, System.Timers.ElapsedEventArgs args)
        {
            Console.WriteLine("Begin processing loop");

            var slackAPI = new SlackAPI();
            
            var oldestTime = DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            oldestTime = oldestTime - 30; //5min offset. Job needs to run more frequently than this offset.

            //Grab the messages from the general channel for the configured time range
            var response = slackAPI.channelsHistory(token, "C1MSVR0LQ", oldestTime.ToString());

            //Turn response into proper JSON Object
            var joObject = JObject.Parse(response);

            //CheckForLinks(joObject);
            //SlackSession slackSession = new SlackSession(token);

            SlackCommand slackCommands = new SlackCommand(slackSession);
            slackCommands.CheckForCommands(joObject);

        }
    }
}
