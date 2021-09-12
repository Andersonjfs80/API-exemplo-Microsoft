using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using System.Text;
using ContosoPizza.Models;

namespace ContosoPizza.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SegurancaController : ControllerBase
    {
        private IConfiguration _config;
        public SegurancaController(IConfiguration Configuration)
        {
            _config = Configuration;
        }

        [HttpPost]
        public IActionResult Login(Usuario usuario)
        {
            bool resultado = ValidarUsuario(usuario);
            if (resultado)
            {
                var tokenString = GerarTokenJWT();
                return Ok(new { token = tokenString });
            }
            else
            {
                return Unauthorized();
            }
        }

        private string GerarTokenJWT()
        {
            var issuer = _config["JwtTokenConfiguration:Issuer"];
            var audience = _config["JwtTokenConfiguration:Audience"];
            var expireSeconds = Convert.ToDouble(_config["JwtTokenConfiguration:ExpirationInSeconds"]);
            var expiry = DateTime.Now.AddMinutes(expireSeconds);
            var securityKey = new SymmetricSecurityKey(
                  Encoding.UTF8.GetBytes(_config["JwtTokenConfiguration:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                expires: expiry,
                signingCredentials: credentials);

            var tokenHandler = new JwtSecurityTokenHandler();
            var stringToken = tokenHandler.WriteToken(token);
            return stringToken;
        }

        private bool ValidarUsuario(Usuario usuario)
        {
            if (usuario.Nome == "Anderson" && usuario.Senha == "NS123456")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}