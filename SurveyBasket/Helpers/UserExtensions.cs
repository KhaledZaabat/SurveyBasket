﻿using System.Security.Claims;

namespace SurveyBasket.Helpers;

public static class UserExtensions
{
    public static string? GetUserId(this ClaimsPrincipal user) =>
        user.FindFirstValue(ClaimTypes.NameIdentifier);
}