using System;
using System.Threading.Tasks;
using InnomateApp.Application.DTOs;
using InnomateApp.Application.Interfaces;
using InnomateApp.Domain.Entities;
using BCrypt.Net;

namespace InnomateApp.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly ILoginUpdateQueue _loginUpdateQueue;


        public AuthService(IUserRepository userRepository, IJwtTokenGenerator jwtTokenGenerator, ILoginUpdateQueue loginUpdateQueue)
        {
            _userRepository = userRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
            _loginUpdateQueue = loginUpdateQueue;
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto loginDto)
        {
            var user = await _userRepository.GetUserByEmailAsync(loginDto.Email);
            if (user == null) return null;

            bool isValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash);
            if (!isValid) return null;

            // Update last login WITHOUT blocking the login flow
            //await _userRepository.UpdateLastLoginAsync(user.Id, DateTime.UtcNow);

            //  Non - blocking login timestamp update
            _ = _loginUpdateQueue.EnqueueAsync(user.Id);

            var token = _jwtTokenGenerator.GenerateToken(user);

            

            return new AuthResponseDto
            {
                Token = token,
                User = new UserDto
                {
                    Id = user.Id.ToString(),   // adjust if Id is int/Guid
                    UserName = user.Username,
                    Email = user.Email
                }
            };
        }

        public async Task<AuthResponseDto?> RegisterAsync(RegisterDto registerDto)
        {
            var existingUser = await _userRepository.GetUserByUsernameAsync(registerDto.Username);
            if (existingUser != null) return null;

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

            var user = new User
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = hashedPassword
            };

            await _userRepository.CreateUserAsync(user);

            var token = _jwtTokenGenerator.GenerateToken(user);

            return new AuthResponseDto
            {
                Token = token,
                User = new UserDto
                {
                    Id = user.Id.ToString(),   // adjust if Id is int/Guid
                    UserName = user.Username,
                    Email = user.Email
                }
            };
        }
    }
}
