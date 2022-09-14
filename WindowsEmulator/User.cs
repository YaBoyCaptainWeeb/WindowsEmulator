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
        public string _username { get; set; }
        public string _password { get; set; }
        public bool _OpenFolders { get; set; }
        public bool _OpenPersonalFolder { get; set; }
        public bool _Journal { get; set; }
        public bool _AccountsAdministrating { get; set; }



        public User(string username, string password, bool OpenFolders, bool OpenPersonalFolder, bool Journal, bool AccountsAdminitrating)
        {
            this._username = username;
            this._password = password;
            this._OpenFolders = OpenFolders;
            this._OpenPersonalFolder = OpenPersonalFolder;
            this._Journal = Journal;
            this._AccountsAdministrating = AccountsAdminitrating;
        }
        public User()
        {
            
        }
    } 
}
