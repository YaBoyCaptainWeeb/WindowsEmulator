using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WindowsEmulator
{
    public class User
    {
        public string _username;
        public string _password;
        public bool _OpenFolders;
        public bool _OpenPersonalFolder;
        public bool _Journal;
        public bool _Settings;
        public bool _AccountsAdministrating;

        public User(string username, string password, bool OpenFolders, bool OpenPersonalFolder, bool Journal, bool Settings, bool AccountsAdminitrating)
        {
            this._username = username;
            this._password = password;
            this._OpenFolders = OpenFolders;
            this._OpenPersonalFolder = OpenPersonalFolder;
            this._Journal = Journal;
            this._Settings = Settings;
            this._AccountsAdministrating = AccountsAdminitrating;
        }
    } 
}
