using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.Areas.Admin.Models.Users
{
    public class ActivateUserModel
    {
        [Required]
        public string Id { get; set; }

        public bool IsActive { get; set; }
    }
}