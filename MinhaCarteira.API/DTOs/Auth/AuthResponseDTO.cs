using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MinhaCarteira.API.DTOs;

namespace MinhaCarteira.API.DTOs.Auth
{
    public class AuthResponseDTO
    {
        public string Token { get; set; } = string.Empty;
        public UserDTO User { get; set; } = null!;
    }
}