﻿namespace FYP.API.Models.Dto
{
    public class SystemUserDto
    {
     
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Role { get; set; } = string.Empty;
    }
}
