namespace UrnaEletronicaFake.Shared.Constants;

public static class SecurityConstants
{
    public const int MAX_LOGIN_ATTEMPTS = 3;
    public const int LOGIN_LOCKOUT_MINUTES = 15;
    public const int SESSION_TIMEOUT_MINUTES = 30;
    public const int PASSWORD_MIN_LENGTH = 8;
    public const int PASSWORD_MAX_LENGTH = 50;
    
    public const string ADMIN_ROLE = "Administrator";
    public const string MESARIO_ROLE = "Mesario";
    public const string AUDITOR_ROLE = "Auditor";
    
    public const string SECURITY_VIOLATION_MESSAGE = "Tentativa de acesso não autorizada";
    public const string SESSION_EXPIRED_MESSAGE = "Sessão expirada";
    public const string INVALID_CREDENTIALS_MESSAGE = "Credenciais inválidas";
}

