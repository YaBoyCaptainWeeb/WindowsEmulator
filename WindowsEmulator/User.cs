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
        public int _level { get; set; }



        public User(string username, string password, int level)
        {
            this._username = username;
            this._password = password;
            this._level = level;
        }
        public User()
        {
            
        }
    } 
}
