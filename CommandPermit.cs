using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using Rocket.Unturned.Commands;
using SDG;
using SDG.Unturned;
using Steamworks;
using System.Collections.Generic;
using Rocket.Core.Steam;
using System;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace Anomoly.MySQLPermits
{
    public class CommandPermit : IRocketCommand
    {
        public string Help
        {
            get { return "allows caller to give permits or check current permits"; }
        }

        public string Name
        {
            get { return "permit"; }
        }

        public string Syntax
        {
            get { return "add <player> <permit> | remove <player> <permit>| check <player> <permit>"; }
        }

        public List<string> Aliases
        {
            get { return new List<string>(); }
        }

        public AllowedCaller AllowedCaller
        {
            get { return AllowedCaller.Player; }
        }

        public List<string> Permissions
        {
            get
            {
                return new List<string>() { "permit" };
            }
        }

        public void Execute(IRocketPlayer caller, params string[] command)
        {
            try
            {
                UnturnedPlayer player = (UnturnedPlayer)caller;
                if(command.Length == 0)
                {
                    List<string> playerPermits = MySQLPermitsPlugin.Instance.DatabaseManager.GetPlayerPermits(player);
                    UnturnedChat.Say(caller, MySQLPermitsPlugin.Instance.Translate("command_permits_list", string.Join(", ", playerPermits.ToArray())), UnturnedChat.GetColorFromName(MySQLPermitsPlugin.Instance.Configuration.Instance.MessageColor, Color.blue));
                }
                else if(command.Length == 2)
                {
                    UnturnedPlayer target = UnturnedPlayer.FromName(command[1]);
                    switch (command[0])
                    {
                        case "check":
                            if (!caller.HasPermission("permits.check")) { UnturnedChat.Say(caller, MySQLPermitsPlugin.Instance.Translate("command_permit_NoPermissions")); return; }
                            if (!MySQLPermitsPlugin.Instance.DatabaseManager.HasPermit(target.Id))
                            {
                                UnturnedChat.Say(caller, MySQLPermitsPlugin.Instance.Translate("command_permit_check_hasNotPermtis", target.DisplayName), UnturnedChat.GetColorFromName(MySQLPermitsPlugin.Instance.Configuration.Instance.MessageColor, Color.blue));
                                return;
                            }
                            UnturnedChat.Say(caller, MySQLPermitsPlugin.Instance.Translate("command_permit_check_List", target.DisplayName, string.Join(", ", MySQLPermitsPlugin.Instance.DatabaseManager.GetPlayerPermits(target).ToArray())), UnturnedChat.GetColorFromName(MySQLPermitsPlugin.Instance.Configuration.Instance.MessageColor, Color.blue));
                            break;
                        default:
                            UnturnedChat.Say(caller, "Please /permit check <player>", UnturnedChat.GetColorFromName(MySQLPermitsPlugin.Instance.Configuration.Instance.MessageColor, Color.yellow));
                            break;
                    }
                }
                else if(command.Length == 3)
                {
                    UnturnedPlayer target = UnturnedPlayer.FromName(command[1]);
                    if (target == null)
                    {
                        UnturnedChat.Say(caller, MySQLPermitsPlugin.Instance.Translate("comamnd_permit_playerNotFound"), UnturnedChat.GetColorFromName(MySQLPermitsPlugin.Instance.Configuration.Instance.MessageColor, Color.blue));
                        return;
                    }
                    string permitName = command[2].ToLower();
                    if (!MySQLPermitsPlugin.Instance.IsValidPermitName(permitName))
                    {
                        UnturnedChat.Say(caller, MySQLPermitsPlugin.Instance.Translate("command_permit_add_remove_permitNotFound"), UnturnedChat.GetColorFromName(MySQLPermitsPlugin.Instance.Configuration.Instance.MessageColor, Color.blue));
                        return;
                    }

                    switch (command[0])
                    {
                        case "add":
                            if (!caller.HasPermission("permits.add")) { UnturnedChat.Say(caller, MySQLPermitsPlugin.Instance.Translate("command_permit_NoPermissions")); return; }
                            if (MySQLPermitsPlugin.Instance.DatabaseManager.HasPermit(target.Id, permitName))
                            {
                                UnturnedChat.Say(caller, MySQLPermitsPlugin.Instance.Translate("comamnd_permit_add_alreadyHasPermit", target.DisplayName, MySQLPermitsPlugin.Instance.GetPermit(permitName)), UnturnedChat.GetColorFromName(MySQLPermitsPlugin.Instance.Configuration.Instance.MessageColor, Color.blue));
                                return;
                            }
                            MySQLPermitsPlugin.Instance.DatabaseManager.AddPermit(target, MySQLPermitsPlugin.Instance.GetPermit(permitName));
                            UnturnedChat.Say(caller, MySQLPermitsPlugin.Instance.Translate("command_permit_add", target.DisplayName, MySQLPermitsPlugin.Instance.GetPermit(permitName)), UnturnedChat.GetColorFromName(MySQLPermitsPlugin.Instance.Configuration.Instance.MessageColor, Color.blue));
                            break;
                        case "remove":
                            if (!caller.HasPermission("permits.remove")) { UnturnedChat.Say(caller, MySQLPermitsPlugin.Instance.Translate("command_permit_NoPermissions")); return; }
                            if (!MySQLPermitsPlugin.Instance.DatabaseManager.HasPermit(target.Id, permitName))
                            {
                                UnturnedChat.Say(caller, MySQLPermitsPlugin.Instance.Translate("command_permit_remove_DoesNotHavePermit", target.DisplayName, MySQLPermitsPlugin.Instance.GetPermit(permitName)), UnturnedChat.GetColorFromName(MySQLPermitsPlugin.Instance.Configuration.Instance.MessageColor, Color.blue));
                                return;
                            }
                            MySQLPermitsPlugin.Instance.DatabaseManager.RemovePermit(target, permitName);
                            UnturnedChat.Say(caller, MySQLPermitsPlugin.Instance.Translate("command_permit_remove", MySQLPermitsPlugin.Instance.GetPermit(permitName), target.DisplayName), UnturnedChat.GetColorFromName(MySQLPermitsPlugin.Instance.Configuration.Instance.MessageColor, Color.blue));
                            break;
                        default:
                            UnturnedChat.Say(caller, MySQLPermitsPlugin.Instance.Translate("command_permit_admin_usage"), UnturnedChat.GetColorFromName(MySQLPermitsPlugin.Instance.Configuration.Instance.MessageColor, Color.blue));
                            break;
                    }
                }
                else
                {
                    UnturnedChat.Say(caller, MySQLPermitsPlugin.Instance.Translate("command_permits_usage"), UnturnedChat.GetColorFromName(MySQLPermitsPlugin.Instance.Configuration.Instance.MessageColor, Color.blue));
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }
    }
}
