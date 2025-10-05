﻿using TaskTracker.Core.Entities;

namespace TaskTracker.Core.Interfaces;
public interface IAuthService
{
    string GenerateJwtToken(User user);
    string HashPassword(string password);
    bool VerifyPassword(string password, string passwordHash);
}