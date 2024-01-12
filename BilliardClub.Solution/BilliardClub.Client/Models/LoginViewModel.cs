﻿using System.ComponentModel.DataAnnotations;

namespace BilliardClub.Client.Models
{
    public class LoginViewModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
