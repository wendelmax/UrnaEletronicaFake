using UrnaEletronicaFake.Shared.Models;
using UrnaEletronicaFake.Shared.Enums;

namespace UrnaEletronicaFake.Administration.Managers;

public interface IElectionManager
{
    Task<bool> StartElectionAsync(int eleicaoId);
    Task<bool> StopElectionAsync(int eleicaoId);
    Task<bool> PauseElectionAsync(int eleicaoId);
    Task<bool> ResumeElectionAsync(int eleicaoId);
    Task<ElectionStatus> GetElectionStatusAsync(int eleicaoId);
    Task<bool> CanStartElectionAsync(int eleicaoId);
    Task<bool> CanStopElectionAsync(int eleicaoId);
    Task<Dictionary<string, object>> GetElectionStatisticsAsync(int eleicaoId);
    Task<bool> ValidateElectionConfigurationAsync(int eleicaoId);
    Task<bool> ScheduleElectionAsync(int eleicaoId, DateTime startTime, DateTime endTime);
}

public enum ElectionStatus
{
    NotStarted = 0,
    Scheduled = 1,
    InProgress = 2,
    Paused = 3,
    Completed = 4,
    Cancelled = 5,
    Error = 6
}

