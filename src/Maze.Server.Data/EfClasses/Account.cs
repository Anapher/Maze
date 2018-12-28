using System;
using System.ComponentModel.DataAnnotations;
using Maze.Server.Connection;

namespace Maze.Server.Data.EfClasses
{
    public class Account
    {
        public int AccountId { get; set; }

        [Required]
        [MaxLength(LengthConsts.AccountUsernameMaxLength)]
        public string Username { get; set; }

        [Required] public string Password { get; set; }

        public bool IsEnabled { get; set; } = true;

        [Required] public DateTime CreatedOn { get; private set; }

        [Required] public DateTime TokenValidityPeriod { get; set; }
    }
}