using DiscordApp.Domain.Common;

namespace DiscordApp.Domain.Constants;

public static class AuthenticationErrors
{
    public static readonly Error EmailAlreadyRegistered = new(
        "Auth.EmailAlreadyRegistered",
        "Email is already registered.");

    public static readonly Error PendingRegistrationNotFoundForEmail = new(
        "Auth.PendingRegistrationNotFound",
        "No pending registration found for this email.");

    public static readonly Error VerificationCodeExpired = new(
        "Auth.VerificationCodeExpired",
        "Code has expired.");

    public static readonly Error IncorrectVerificationCode = new(
        "Auth.IncorrectVerificationCode",
        "Incorrect code.");

    public static readonly Error PendingRegistrationNotFound = new(
        "Auth.PendingRegistrationNotFound",
        "No pending registration found.");

    public static readonly Error EmailNotVerified = new(
        "Auth.EmailNotVerified",
        "Email not verified yet.");

    public static readonly Error RegistrationSessionExpired = new(
        "Auth.RegistrationSessionExpired",
        "Registration session expired. Please start again.");

    public static readonly Error UsernameTagsExhausted = new(
        "Auth.UsernameTagsExhausted",
        "All tags for this username are taken.");

    public static readonly Error InvalidCredentials = new(
        "Auth.InvalidCredentials",
        "Invalid credentials.");

    public static Error DatabaseUserCreationFailed(string details) => new(
        "Auth.DatabaseUserCreationFailed",
        $"Database user creation failed: {details}");

    public static Error UserCreationFailed(string details) => new(
        "Auth.UserCreationFailed",
        details);
}
