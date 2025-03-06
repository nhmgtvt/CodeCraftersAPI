﻿using CodeCrafters.Application.Entities.Auth;
using CodeCrafters.Application.Services.Auth;
using Microsoft.AspNetCore.Mvc;

namespace CodeCrafters.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var response = await _authService.LoginAsync(request);
                return Ok(response);
            }
            catch (ArgumentException)
            {
                return BadRequest("Invalid input");
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized("Invalid credentials");
            }
        }

        // Register a new user (password-based or via OAuth)
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var response = await _authService.RegisterAsync(request);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);  // Handle invalid input or email already exists
            }
        }

        // OAuth Login initiation (redirect to external provider login page)
        [HttpGet("{provider}/login")]
        public IActionResult OAuthLogin(string provider)
        {
            var url = _authService.GetOAuthLoginUrl(provider);
            return Redirect(url);
        }

        // OAuth Callback after the user is authenticated with the external provider
        [HttpGet("{provider}/callback")]
        public async Task<IActionResult> OAuthCallback(string provider, [FromQuery] string code)
        {
            try
            {
                var response = await _authService.HandleOAuthCallbackAsync(provider, code);
                return Ok(response);
            }
            catch (Exception)
            {
                return Unauthorized("OAuth login failed");
            }
        }
    }
}
