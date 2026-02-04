using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookTracker
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public DateTime DOB { get; set; }
        public DateTime AccountCreationDate { get; set; }

        public override string ToString()
        {
            return $"UserId: {UserId}, Username: {Username}, Email: {Email}, DOB: {DOB.ToShortDateString()}, Created: {AccountCreationDate.ToShortDateString()}";
        }
    }
}