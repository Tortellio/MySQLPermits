using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rocket.API;
using System.Xml.Serialization;
using UnityEngine;
using Steamworks;

namespace Anomoly.MySQLPermits
{
    public class PermitsConfiguration : IRocketPluginConfiguration
    {
        public string DatabaseAddress;
        public string DatabaseUsername;
        public string DatabasePassword;
        public string DatabaseName;
        public string DatabaseTableName;
        public int DatabasePort;
        public List<PermitModel> Permits;
        public string MessageColor;
        public void LoadDefaults()
        {
            DatabaseAddress = "localhost";
            DatabaseUsername = "root";
            DatabasePassword = "";
            DatabaseName = "unturned";
            DatabaseTableName = "permits";
            DatabasePort = 3306;
            Permits = new List<PermitModel>()
            {
                new PermitModel{PermitName = "BusinessPermit", PermitDays = 1}
            };
            MessageColor = "EE82EE";
        }
    }
}
