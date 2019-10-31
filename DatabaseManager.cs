using MySql.Data.MySqlClient;
using Rocket.Core.Logging;
using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Anomoly.MySQLPermits
{
    public class DatabaseManager
    {
        public DatabaseManager()
        {
            new I18N.West.CP1250();
            CheckSchema();
        }

        private MySqlConnection CreateConnection()
        {
            MySqlConnection connection = null;
            try
            {
                if (MySQLPermitsPlugin.Instance.Configuration.Instance.DatabasePort == 0) MySQLPermitsPlugin.Instance.Configuration.Instance.DatabasePort = 3306;
                connection = new MySqlConnection(String.Format("SERVER={0};DATABASE={1};UID={2};PASSWORD={3};PORT={4};", MySQLPermitsPlugin.Instance.Configuration.Instance.DatabaseAddress, MySQLPermitsPlugin.Instance.Configuration.Instance.DatabaseName, MySQLPermitsPlugin.Instance.Configuration.Instance.DatabaseUsername, MySQLPermitsPlugin.Instance.Configuration.Instance.DatabasePassword, MySQLPermitsPlugin.Instance.Configuration.Instance.DatabasePort));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return connection;
        }

        public bool HasPermit(string steamId, string permit)
        {
            try
            {
                MySqlConnection connection = CreateConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select 1 from `" + MySQLPermitsPlugin.Instance.Configuration.Instance.DatabaseTableName + "` where `steamId` = '" + steamId + "' and `permit_name` = '" + permit + "';";
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null) return true;
                connection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return false;
        }

        public bool HasPermit(string steamid)
        {
            try
            {
                MySqlConnection c = CreateConnection();
                MySqlCommand command = c.CreateCommand();
                command.CommandText = "SELECT * FROM " + MySQLPermitsPlugin.Instance.Configuration.Instance.DatabaseTableName + " WHERE `steamId` = '" + steamid + "'";
                c.Open();
                object result = command.ExecuteScalar();
                if (result != null) return true;
                c.Clone();
            }
            catch(Exception ex)
            {
                Logger.LogException(ex);
            }
            return false;
        }

        public void AddPermit(UnturnedPlayer player, string Permit)
        {
            try
            {
                MySqlConnection c = CreateConnection();
                MySqlCommand command = c.CreateCommand();
                command.CommandText = "INSERT INTO " + MySQLPermitsPlugin.Instance.Configuration.Instance.DatabaseTableName + " (`steamId`,`permit_name`) VALUES('" + player.Id + "','" + Permit + "')";
                c.Open();
                command.ExecuteNonQuery();
                c.Close();
            }
            catch(Exception ex) { Logger.LogException(ex); }
        }

        public void RemovePermit(UnturnedPlayer player, string permit)
        {
            try
            {
                MySqlConnection c = CreateConnection();
                MySqlCommand command = c.CreateCommand();
                command.CommandText = "DELETE FROM " + MySQLPermitsPlugin.Instance.Configuration.Instance.DatabaseTableName + " WHERE `steamId` = '" + player.Id + "' AND `permit_name` = '" + permit + "'";
                c.Open();
                command.ExecuteNonQuery();
                c.Close();
            }
            catch (Exception ex) { Logger.LogException(ex); }
        }

        public void CheckSchema()
        {
            try
            {
                MySqlConnection connection = CreateConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "show tables like '" + MySQLPermitsPlugin.Instance.Configuration.Instance.DatabaseTableName + "'";
                connection.Open();
                object test = command.ExecuteScalar();

                if (test == null)
                {
                    command.CommandText = "CREATE TABLE `" + MySQLPermitsPlugin.Instance.Configuration.Instance.DatabaseTableName + "` (`id` INT NOT NULL AUTO_INCREMENT , `steamId` VARCHAR(35) NOT NULL , `permit_name` VARCHAR(55) NOT NULL , `permit_duration` INT NOT NULL DEFAULT '0' , `permit_time` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP() , PRIMARY KEY (`id`));";
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        public List<string> GetPlayerPermits(UnturnedPlayer player)
        {
            List<string> output = new List<string>();
            try
            {
                MySqlConnection c = CreateConnection();
                MySqlCommand command = c.CreateCommand();
                command.CommandText = "SELECT * FROM " + MySQLPermitsPlugin.Instance.Configuration.Instance.DatabaseTableName + " WHERE `steamId` = '" + player.CSteamID + "'";
                c.Open();
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    string permit = reader.GetString(2);
                    output.Add(permit);
                }
                c.Close();
            }
            catch(Exception ex)
            {
                Logger.LogException(ex);
            }
            return output;
        }
    }
}
