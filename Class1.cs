using System;
using System.Collections.Generic;

using System.Linq;
using System.Net.Security;
using System.Text;
using System.Timers;

using Fougerite;
using Fougerite.Events;

namespace TimeVoter
{
    public class TimeVoter : Fougerite.Module
    {
        public override string Name { get { return "TimeVoter"; } }
        public override string Author { get { return "Salva/juli & samvds"; } }
        public override string Description { get { return "TimeVoter"; } }
        public override Version Version { get { return new Version("1.2"); } }

        private string red = "[color #FF0000]";
        private string blue = "[color #00ffff]";
        private string green = "[color #00ff00]";
        private string yellow = "[color #ffff00]";
        private string white = "[color #ffffff]";

        private float actualtime = World.GetWorld().Time;
        private bool activevote = false;
        private int yes = 0;
        private int no = 0;

        public List<ulong> ids;


        public override void Initialize()
        {
            ids = new List<ulong>();
            Hooks.OnServerInit += OnServerInit;
            Hooks.OnCommand += new Hooks.CommandHandlerDelegate(this.On_Command);
        }
        public override void DeInitialize()
        {
            Hooks.OnServerInit -= OnServerInit;
            Hooks.OnCommand -= new Hooks.CommandHandlerDelegate(this.On_Command);
        }

        public void OnServerInit()
        {
            Timer reloj = new Timer();
            reloj.Interval = 10000;
            reloj.AutoReset = true;
            reloj.Elapsed += (x, y) =>
            {
                actualtime = World.GetWorld().Time;
                if (actualtime >= 17 && actualtime <= 17.5)
                {
                    if (!activevote)
                    {
                        Server.GetServer().BroadcastNotice("Time to vote!");
                        Server.GetServer().BroadcastFrom("Legacy Lives", "Use" + this.blue + " /day " + this.white + "- to vote for daytime.");
                        Server.GetServer().BroadcastFrom("Legacy Lives", "Use" + this.blue + " /night " + this.white + "- to vote for nighttime.");
                        activevote = true;
                    }
                }
                if (actualtime >= 17.6 && activevote)
                {
                    activevote = false;
                    ComprobarVotos();
                }
            };
            reloj.Start();
        }
        public void ComprobarVotos()
        {
            if (yes >= no)
            {
                Server.GetServer().BroadcastFrom("Legacy Lives", "☢ The results are in: " + this.blue + yes + this.white + " votes for daytime and " + this.blue + no + this.white + " votes for nighttime.");
                Server.GetServer().BroadcastFrom("Legacy Lives", "☢ " + this.green + "Daytime Wins!");
                World.GetWorld().Time = 7;
                BorrarVariables();
            }
            else if (yes <= no)
            {
                Server.GetServer().BroadcastFrom("Legacy Lives", "☢ The results are in: " + this.blue + yes + this.white + " votes for daytime and " + this.blue + no + this.white + " votes for nighttime.");
                Server.GetServer().BroadcastFrom("Legacy Lives", "☢ " + this.green + "Nighttime Wins!");
                BorrarVariables();
            }
        }
        public void BorrarVariables()
        {
            ids.Clear();
            yes = 0;
            no = 0;
        }
        public void On_Command(Fougerite.Player player, string cmd, string[] args)
        {
            if (cmd == "day")
            {
                if (!activevote)
                {
                    player.MessageFrom("Legacy Lives", this.yellow + "☢ " + this.red + "There is no vote running!");
                    return;
                }

                if (ids.Contains<ulong>(player.UID))
                {
                    player.MessageFrom("Legacy Lives", this.yellow + "☢ " + this.green + "You have voted succesfully!");
                    return;
                }

                player.MessageFrom("Legacy Lives", this.yellow + "☢ " + this.green + "You voted for daytime!");
                yes += 1;
                ids.Add(player.UID);

                Server.GetServer().BroadcastFrom("Legacy Lives", "As it currently stands, there are:");
                Server.GetServer().BroadcastFrom("Legacy Lives", this.blue + yes + this.white + " vote(s) for daytime and " + this.blue + no + this.white + " vote(s) for nighttime.");
                Server.GetServer().BroadcastFrom("Legacy Lives", this.blue + player.Name + this.white + " has voted for daytime!");
            }
            else if (cmd == "night")
            {
                if (!activevote)
                {
                    player.MessageFrom("Legacy Lives", this.yellow + "☢ " + this.red + "There is no vote running!");
                    return;
                }

                if (ids.Contains<ulong>(player.UID))
                {
                    player.MessageFrom("Legacy Lives", this.yellow + "☢ " + this.green + "You have voted succesfully!");
                    return;
                }

                player.MessageFrom("Legacy Lives", this.yellow + "☢ " + this.green + "You voted for nighttime!");
                no += 1;
                ids.Add(player.UID);

                Server.GetServer().BroadcastFrom("Legacy Lives", "As it currently stands, there are:");
                Server.GetServer().BroadcastFrom("Legacy Lives", this.blue + "☢ " + yes + this.white + " vote(s) for daytime and " + this.blue + no + this.white + " vote(s) for nighttime.");
                Server.GetServer().BroadcastFrom("Legacy Lives", this.blue + "☢ " + player.Name + this.white + " has voted for daytime!");
            }
            else if (cmd == "timeday")
            {
                if (player.Admin)
                {
                    World.GetWorld().Time = 7;
                }
            }
            else if (cmd == "time")
            {
                player.MessageFrom("Legacy Lives", "---------------------- [" + this.green + " TimeVoter " + this.white + "] ----------------------");
                player.MessageFrom("Legacy Lives", "Use" + this.blue + " /day " + this.white + "- to vote for day.");
                player.MessageFrom("Legacy Lives", "Use" + this.blue + " /night " + this.white + "- to vote for night.");
                player.MessageFrom("Legacy Lives", "The current time is: " + this.blue + actualtime + this.white + ", and the vote will start at " + this.blue + "17:00" + this.white + ".");
            }
        }
    }
}
