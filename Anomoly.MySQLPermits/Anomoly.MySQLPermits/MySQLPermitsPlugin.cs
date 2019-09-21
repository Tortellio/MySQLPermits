using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rocket.Core.Plugins;
using Steamworks;
using UnityEngine;
using Rocket.Unturned.Player;
using Rocket.Unturned.Chat;
using Rocket.API.Collections;
using System.Timers;
using Logger = Rocket.Core.Logging.Logger;

namespace Anomoly.MySQLPermits
{
    public class MySQLPermitsPlugin : RocketPlugin<PermitsConfiguration>
    {
        public static MySQLPermitsPlugin Instance { get; private set; }
        public DatabaseManager DatabaseManager { get; private set; }
        protected override void Load()
        {
            Instance = this;
            try
            {
                DatabaseManager = new DatabaseManager();
            }
            catch(Exception ex) { Logger.LogException(ex); }
            
            Logger.Log("MySQLPermits by Anomoly has loaded!");
        }

        protected override void Unload()
        {
            DatabaseManager = null;
            Instance = null;
            Logger.Log("MySQLPermits by Anomoly has unloaded!");
        }

        #region Utils

        public bool IsValidPermitName(string name)
        {
            foreach(PermitModel p in Configuration.Instance.Permits)
            {
                if(p.PermitName.ToLower() == name.ToLower())
                {
                    return true;
                }
            }
            return false;
        }

        public string GetPermit(string name)
        {
            PermitModel p = Configuration.Instance.Permits.Find(x => x.PermitName.ToLower() == name.ToLower());
            if(p == null)
            {
                return string.Empty;
            }
            return p.PermitName;
        }

        #endregion

        public override TranslationList DefaultTranslations => 
            new TranslationList() {
                    {"command_permits_list","You have the following permits: {0}!"},
                    {"comamnd_permit_add_alreadyHasPermit","{0} already has the {1} permit!" },
                    {"command_permit_add","You have given {0} a {1} permit!" },
                    {"command_permit_remove_DoesNotHavePermit","{0} does not have the {1} permit!" },
                    {"command_permit_remove","You have removed the {0} permit from {1}!" },
                    {"command_permit_add_remove_permitNotFound","Permit not found!" },
                    {"comamnd_permit_playerNotFound","Player not found!" },
                    {"command_permit_check_hasNotPermtis","{0} does not have any permits" },
                    {"command_permit_check_List","{0} has the following permits: {1}!" },
                    {"command_permit_NoPermissions","You do not have permission to use this part of the command!" },
                    {"command_permit_admin_usage","Please do /permit [add|remove|check] <player> [permit]" },
                    {"command_permits_usage","Please do /permit to check your current permits!" }
                };
    }
}
