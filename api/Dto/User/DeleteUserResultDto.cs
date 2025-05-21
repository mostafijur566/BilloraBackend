using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;


public class DeleteUserResultDto
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public User? DeletedUser { get; set; }
}
