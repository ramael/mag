using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MagApi.Models;
using Microsoft.Extensions.Logging;
using MagApi.Contracts;
using MagApi.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using MagApi.Identity.Helpers;

namespace MagApi.Controllers
{
    [Route("api/public/v1/identity")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly ILogger<CartsController> _logger;
        private readonly MagIdentityDbContext _context;
        private readonly RoleManager<MagApplicationRole> _roleManager;
        private readonly UserManager<MagApplicationUser> _userManager;
        private readonly ITokenHelper _tokenHelper;

        public IdentityController(ILogger<CartsController> logger, MagIdentityDbContext context,
                                UserManager<MagApplicationUser> userManager, RoleManager<MagApplicationRole> roleManager,
                                ITokenHelper tokenHelper)
        {
            _logger = logger;
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _tokenHelper = tokenHelper;
        }

        // POST: api/roles
        [HttpPost("roles")]
        public async Task<IActionResult> PostRoles(Role dto)
        {
            var role = new MagApplicationRole
            {
                Name = dto.Name,
                Description = dto.Description
            };

            var result = await _roleManager.CreateAsync(role);
            if (result.Succeeded)
            {
                _logger.LogInformation("Created a new role: " + dto.Name);
                return NoContent();
            }

            _logger.LogWarning("Failed to create a new role: " + dto.Name);
            foreach (var error in result.Errors)
            {
                _logger.LogWarning("-- Reason: " + error.Description);
            }
            return BadRequest("See log for details");

        }

        // POST: api/users
        [AllowAnonymous]
        [HttpPost("users")]
        public async Task<IActionResult> PostUsers(User dto)
        {
            var user = new MagApplicationUser { 
                UserName = dto.UserName, 
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email 
            };
            
            var result = await _userManager.CreateAsync(user, dto.Password);
            if (result.Succeeded)
            {
                _logger.LogInformation("Created a new account with password for user: " + dto.UserName);
                return NoContent();
            }

            _logger.LogWarning("Failed to create a new account with password for user: " + dto.UserName);
            foreach (var error in result.Errors)
            {
                _logger.LogWarning("-- Reason: " + error.Description);
            }
            return BadRequest("See log for details");
             
        }

        // POST: api/users/{username}/roles
        [AllowAnonymous]
        [HttpPost("users/{username}/roles")]
        public async Task<IActionResult> PostUsersRoles(string username, UserRoles dto)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                _logger.LogWarning("User unknown: " + username);
                return NotFound("Unknow username");
            }

            var result = await _userManager.AddToRolesAsync(user, dto.Roles);
            if (result.Succeeded)
            {
                _logger.LogInformation("Added new roles for user: " + username);
                return NoContent();
            }

            _logger.LogWarning("Failed to roles for user: " + username);
            foreach (var error in result.Errors)
            {
                _logger.LogWarning("-- Reason: " + error.Description);
            }
            return BadRequest("See log for details");

        }

        // POST: api/login
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> PostLogin(LoginRequest dto)
        {
            var user = await _userManager.FindByNameAsync(dto.UserName);
            if (user == null)
            {
                _logger.LogWarning("User unknown: " + dto.UserName);
                return NotFound("Unknow username");
            }

            var result = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!result)
            {
                _logger.LogWarning("User failed to login: " + dto.UserName);
                return Unauthorized("Invalid login attempt");
            }

            _logger.LogInformation("User logged in: " + dto.UserName);

            var roles = await _userManager.GetRolesAsync(user);

            var response = new LoginResponse()
            {
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = roles,
                Token = _tokenHelper.GenerateToken(dto.UserName, roles)
            };
                
            return Ok(response);
            
        }

    }
}
