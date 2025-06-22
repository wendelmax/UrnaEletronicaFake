using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UrnaEletronicaFake.Models;

namespace UrnaEletronicaFake.Services;

public interface IVotoService
{
    Task<Voto> RegistrarVotoAsync(string cpfEleitor, int eleicaoId, int? candidatoId, 
        bool votoNulo = false, bool votoBranco = false, string? sessaoId = null);
    Task<bool> VerificarSeJaVotouAsync(string cpfEleitor, int eleicaoId);
    Task<IEnumerable<Voto>> ObterVotosPorEleicaoAsync(int eleicaoId);
    Task<object> ObterResultadoEleicaoAsync(int eleicaoId);
    Task<string> GerarHashVotoAsync(string cpfEleitor, int eleicaoId, int? candidatoId, DateTime dataVoto);
    Task<bool> VerificarIntegridadeVotoAsync(int votoId);
} 