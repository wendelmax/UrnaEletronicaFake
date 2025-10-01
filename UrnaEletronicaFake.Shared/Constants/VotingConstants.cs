namespace UrnaEletronicaFake.Shared.Constants;

public static class VotingConstants
{
    public const int MIN_ELEITOR_ID_LENGTH = 4;
    public const int MAX_ELEITOR_ID_LENGTH = 12;
    public const int SESSION_TIMEOUT_MINUTES = 30;
    public const int MAX_VOTE_ATTEMPTS = 3;
    public const int VOTE_CONFIRMATION_TIMEOUT_SECONDS = 30;
    
    public const string BLANK_VOTE_CODE = "BRANCO";
    public const string NULL_VOTE_CODE = "NULO";
    
    public const string PRESIDENTIAL_CODE = "P";
    public const string SENATORIAL_CODE = "S";
    public const string FEDERAL_DEPUTY_CODE = "F";
    public const string STATE_DEPUTY_CODE = "E";
    public const string GOVERNOR_CODE = "G";
    public const string MAYOR_CODE = "M";
    public const string COUNCILOR_CODE = "C";
}

