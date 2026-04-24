namespace MediLink.Application.Services;

using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BCrypt.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MediLink.Application.DTOs;
using MediLink.Domain.Entities;
using MediLink.Domain.Enums;
using MediLink.Infrastructure.Repositories;

/// <summary>
/// Authentication service implementation
/// </summary>
public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
    Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request);
    Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
}

public class AuthService : IAuthService
{
    private static readonly ConcurrentDictionary<string, RefreshTokenInfo> RefreshTokens =
        new(StringComparer.OrdinalIgnoreCase);

    private readonly IRepository<User> _userRepository;
    private readonly IConfiguration _configuration;

    public AuthService(IRepository<User> userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = await _userRepository.FirstOrDefaultAsync(u => u.Email == request.Email && !u.IsDeleted);

        if (user == null || !BCrypt.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        if (!user.IsActive)
        {
            throw new UnauthorizedAccessException("Account is deactivated");
        }

        user.LastLoginAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);

        var token = GenerateJwtToken(user);
        var refreshToken = CreateRefreshToken(user.Id);

        return BuildAuthResponse(user, token, refreshToken);
    }

    public async Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        if (request.Password != request.ConfirmPassword)
        {
            throw new ArgumentException("Passwords do not match");
        }

        var existingUser = await _userRepository.FirstOrDefaultAsync(u => u.Email == request.Email && !u.IsDeleted);
        if (existingUser != null)
        {
            throw new InvalidOperationException("User with this email already exists");
        }

        User user;
        string role;
        switch (request.Role?.ToLower())
        {
            case "doctor":
            case "generalist":
                user = new Doctor
                {
                    Email = request.Email,
                    PasswordHash = BCrypt.HashPassword(request.Password),
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    PhoneNumber = request.PhoneNumber,
                    DateOfBirth = request.DateOfBirth,
                    Role = UserRole.Generalist,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    Specialization = "General Medicine", // Default, can be updated later
                    CabinetName = "To be defined", // Default, can be updated later
                    CabinetPhone = request.PhoneNumber,
                    CabinetAddress = "To be defined", // Default, can be updated later
                    IsVerified = false
                };
                role = "Doctor";
                break;

            case "specialist":
                user = new Specialist
                {
                    Email = request.Email,
                    PasswordHash = BCrypt.HashPassword(request.Password),
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    PhoneNumber = request.PhoneNumber,
                    DateOfBirth = request.DateOfBirth,
                    Role = UserRole.Specialist,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    Specialization = "Specialist", // Default, can be updated later
                    IsVerified = false
                };
                role = "Specialist";
                break;

            case "patient":
            default:
                user = new Patient
                {
                    Email = request.Email,
                    PasswordHash = BCrypt.HashPassword(request.Password),
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    PhoneNumber = request.PhoneNumber,
                    DateOfBirth = request.DateOfBirth,
                    Role = UserRole.Patient,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                role = "Patient";
                break;
        }

        await _userRepository.AddAsync(user);

        return new RegisterResponseDto
        {
            Success = true,
            Role = role,
            Message = $"User registered successfully as {role}"
        };
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken) || !RefreshTokens.TryGetValue(refreshToken, out var tokenInfo))
        {
            throw new UnauthorizedAccessException("Invalid refresh token");
        }

        if (tokenInfo.ExpiresAt < DateTime.UtcNow)
        {
            RefreshTokens.TryRemove(refreshToken, out _);
            throw new UnauthorizedAccessException("Refresh token expired");
        }

        var user = await _userRepository.GetByIdAsync(tokenInfo.UserId);
        if (user == null || user.IsDeleted)
        {
            throw new UnauthorizedAccessException("Invalid user for refresh token");
        }

        var jwt = GenerateJwtToken(user);
        var newRefreshToken = CreateRefreshToken(user.Id);

        return BuildAuthResponse(user, jwt, newRefreshToken);
    }

    private static AuthResponseDto BuildAuthResponse(User user, string token, string refreshToken)
    {
        return new AuthResponseDto
        {
            Token = token,
            RefreshToken = refreshToken,
            ExpiresIn = 60 * 60,
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role.ToString(),
                PhoneNumber = user.PhoneNumber,
                ProfileImageUrl = user.ProfileImageUrl
            }
        };
    }

    private string CreateRefreshToken(Guid userId)
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var expiresAt = DateTime.UtcNow.AddDays(int.Parse(_configuration["Jwt:RefreshTokenExpiryDays"] ?? "7"));

        var tokenInfo = new RefreshTokenInfo
        {
            UserId = userId,
            ExpiresAt = expiresAt
        };

        RefreshTokens[token] = tokenInfo;
        return token;
    }

    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(
            jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key not configured")));

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
        };

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(int.Parse(jwtSettings["ExpiryMinutes"] ?? "60")),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private sealed class RefreshTokenInfo
    {
        public Guid UserId { get; init; }
        public DateTime ExpiresAt { get; init; }
    }
}
