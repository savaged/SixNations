using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace SixNations.Server.Models
{
    public class User : ModelBase
    {
        [HiddenInput(DisplayValue = false)]
        public int UserId { get; set; }

        [HiddenInput(DisplayValue = false)]
        public string Username { get; set; }

        public string Firstname { get; set; }

        public string Lastname { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
