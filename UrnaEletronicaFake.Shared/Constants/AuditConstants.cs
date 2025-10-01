namespace UrnaEletronicaFake.Shared.Constants;

public static class AuditConstants
{
    public const int MAX_AUDIT_RETENTION_DAYS = 2555; // 7 anos
    public const int AUDIT_BATCH_SIZE = 1000;
    public const int MAX_AUDIT_MESSAGE_LENGTH = 500;
    
    public const string AUDIT_SUCCESS_MESSAGE = "Operação realizada com sucesso";
    public const string AUDIT_FAILURE_MESSAGE = "Falha na operação";
    public const string AUDIT_SECURITY_VIOLATION_MESSAGE = "Violação de segurança detectada";
    
    public const string SYSTEM_USER = "SYSTEM";
    public const string ANONYMOUS_USER = "ANONYMOUS";
}

