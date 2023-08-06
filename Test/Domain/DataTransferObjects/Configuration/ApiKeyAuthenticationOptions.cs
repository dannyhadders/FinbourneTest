using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authentication;

namespace FinbourneCache.Domain.DataTransferObjects.Configuration;

[ExcludeFromCodeCoverage]
public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    public const string DefaultScheme = "ClientKey";
}