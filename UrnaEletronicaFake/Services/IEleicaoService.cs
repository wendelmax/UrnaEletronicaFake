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
} 