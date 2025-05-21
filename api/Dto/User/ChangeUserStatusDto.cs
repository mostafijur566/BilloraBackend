using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dto.User
{
    public class ChangeUserStatusDto
    {
        [Required]
        public bool IsActive { get; set; }
    }
}