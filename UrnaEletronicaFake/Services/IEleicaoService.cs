using System.Collections.Generic;
using System.Threading.Tasks;
using UrnaEletronicaFake.Models;

namespace UrnaEletronicaFake.Services;

public interface IEleicaoService
{
    Task<IEnumerable<Eleicao>> ObterTodasEleicoesAsync();
    Task<Eleicao?> ObterEleicaoPorIdAsync(int id);
    Task<Eleicao?> ObterEleicaoAtivaAsync();
    Task<Eleicao> CriarEleicaoAsync(Eleicao eleicao);
    Task<Eleicao> AtualizarEleicaoAsync(Eleicao eleicao);
    Task<bool> DeletarEleicaoAsync(int id);
    Task<bool> AtivarEleicaoAsync(int id);
    Task<bool> DesativarEleicaoAsync(int id);
    Task<bool> EleicaoEstaAtivaAsync(int id);
    
    // Métodos para gerenciar candidatos
    Task<IEnumerable<Candidato>> ObterCandidatosAsync(int eleicaoId);
    Task<Candidato> AdicionarCandidatoAsync(int eleicaoId, Candidato candidato);
    Task<Candidato> AtualizarCandidatoAsync(int eleicaoId, Candidato candidato);
    Task<bool> RemoverCandidatoAsync(int eleicaoId, int candidatoId);
    
    // Métodos para gerenciar cargos eleitorais
    Task<IEnumerable<CargoEleitoral>> ObterCargosAsync(int eleicaoId);
    Task<CargoEleitoral> AdicionarCargoAsync(int eleicaoId, CargoEleitoral cargo);
    Task<CargoEleitoral> AtualizarCargoAsync(int eleicaoId, CargoEleitoral cargo);
    Task<bool> RemoverCargoAsync(int eleicaoId, int cargoId);
} 