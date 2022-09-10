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
        public string _access;

        public User(string username, string password, string access)
        {
            this._username = username;
            this._password = password;
            this._access = access;
        }
    } 
}
