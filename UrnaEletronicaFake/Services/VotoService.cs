using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UrnaEletronicaFake.Data;
using UrnaEletronicaFake.Models;
using System.Security.Cryptography;
using System.Text;

namespace UrnaEletronicaFake.Services;

public class VotoService : IVotoService
{
    private readonly UrnaDbContext _context;
    private readonly IAuditoriaService _auditoriaService;
    private readonly IEleicaoService _eleicaoService;

    public VotoService(UrnaDbContext context, IAuditoriaService auditoriaService, IEleicaoService eleicaoService)
    {
        _context = context;
        _auditoriaService = auditoriaService;
        _eleicaoService = eleicaoService;
    }

    public async Task<Voto> RegistrarVotoAsync(string cpfEleitor, int eleicaoId, int cargoEleitoralId, int? candidatoId, 
        bool votoNulo = false, bool votoBranco = false, string? sessaoId = null)
    {
        // Verificar se a eleição está ativa
        if (!await _eleicaoService.EleicaoEstaAtivaAsync(eleicaoId))
            throw new InvalidOperationException("Eleição não está ativa ou fora do período de votação");

        // Verificar se já votou neste cargo específico
        if (await VerificarSeJaVotouAsync(cpfEleitor, eleicaoId, cargoEleitoralId))
            throw new InvalidOperationException("Eleitor já votou neste cargo");

        // Verificar se o candidato existe (se não for voto nulo/branco)
        if (candidatoId.HasValue && !votoNulo && !votoBranco)
        {
            var candidato = await _context.Candidatos
                .FirstOrDefaultAsync(c => c.Id == candidatoId.Value && c.EleicaoId == eleicaoId && 
                                         c.CargoEleitoralId == cargoEleitoralId && c.Ativo);
            if (candidato == null)
                throw new ArgumentException("Candidato não encontrado ou inativo");
        }

        var dataVoto = DateTime.Now;
        var hashVoto = await GerarHashVotoAsync(cpfEleitor, eleicaoId, cargoEleitoralId, candidatoId, dataVoto);

        var voto = new Voto
        {
            CpfEleitor = cpfEleitor,
            EleicaoId = eleicaoId,
            CargoEleitoralId = cargoEleitoralId,
            CandidatoId = candidatoId,
            VotoNulo = votoNulo,
            VotoBranco = votoBranco,
            DataVoto = dataVoto,
            HashVoto = hashVoto,
            SessaoId = sessaoId
        };

        _context.Votos.Add(voto);
        await _context.SaveChangesAsync();

        // Registrar auditoria
        var tipoVoto = votoNulo ? "NULO" : votoBranco ? "BRANCO" : "VÁLIDO";
        await _auditoriaService.RegistrarAcaoAsync("VOTAR", "Voto", voto.Id,
            $"Voto {tipoVoto} registrado para CPF {cpfEleitor} no cargo {cargoEleitoralId} da eleição {eleicaoId}");

        return voto;
    }

    public async Task<bool> VerificarSeJaVotouAsync(string cpfEleitor, int eleicaoId)
    {
        return await _context.Votos
            .AnyAsync(v => v.CpfEleitor == cpfEleitor && v.EleicaoId == eleicaoId);
    }

    private async Task<bool> VerificarSeJaVotouAsync(string cpfEleitor, int eleicaoId, int cargoEleitoralId)
    {
        return await _context.Votos
            .AnyAsync(v => v.CpfEleitor == cpfEleitor && v.EleicaoId == eleicaoId && v.CargoEleitoralId == cargoEleitoralId);
    }

    public async Task<IEnumerable<Voto>> ObterVotosPorEleicaoAsync(int eleicaoId)
    {
        return await _context.Votos
            .Include(v => v.Candidato)
            .Include(v => v.CargoEleitoral)
            .Where(v => v.EleicaoId == eleicaoId)
            .OrderBy(v => v.CargoEleitoral.Ordem)
            .ThenBy(v => v.DataVoto)
            .ToListAsync();
    }

    public async Task<object> ObterResultadoEleicaoAsync(int eleicaoId)
    {
        var eleicao = await _context.Eleicoes
            .Include(e => e.CargosEleitorais.Where(c => c.Ativo).OrderBy(c => c.Ordem))
            .Include(e => e.Candidatos.Where(c => c.Ativo))
            .FirstOrDefaultAsync(e => e.Id == eleicaoId);

        if (eleicao == null)
            throw new ArgumentException("Eleição não encontrada");

        var votos = await _context.Votos
            .Include(v => v.CargoEleitoral)
            .Where(v => v.EleicaoId == eleicaoId)
            .ToListAsync();

        var totalVotos = votos.Count;
        var votosValidos = votos.Count(v => !v.VotoNulo && !v.VotoBranco);
        var votosNulos = votos.Count(v => v.VotoNulo);
        var votosBrancos = votos.Count(v => v.VotoBranco);

        var resultadoPorCargo = eleicao.CargosEleitorais.Select(cargo => new
        {
            cargo.Id,
            cargo.Nome,
            cargo.QuantidadeDigitos,
            Votos = votos.Count(v => v.CargoEleitoralId == cargo.Id),
            VotosValidos = votos.Count(v => v.CargoEleitoralId == cargo.Id && !v.VotoNulo && !v.VotoBranco),
            VotosNulos = votos.Count(v => v.CargoEleitoralId == cargo.Id && v.VotoNulo),
            VotosBrancos = votos.Count(v => v.CargoEleitoralId == cargo.Id && v.VotoBranco),
            Candidatos = eleicao.Candidatos
                .Where(c => c.CargoEleitoralId == cargo.Id)
                .Select(candidato => new
                {
                    candidato.Id,
                    candidato.Nome,
                    candidato.Partido,
                    candidato.Numero,
                    candidato.Foto,
                    Votos = votos.Count(v => v.CandidatoId == candidato.Id),
                    Percentual = votos.Count(v => v.CargoEleitoralId == cargo.Id) > 0 
                        ? (double)votos.Count(v => v.CandidatoId == candidato.Id) / votos.Count(v => v.CargoEleitoralId == cargo.Id) * 100 
                        : 0
                }).OrderByDescending(r => r.Votos).ToList()
        }).ToList();

        return new
        {
            Eleicao = new
            {
                eleicao.Id,
                eleicao.Titulo,
                eleicao.Descricao,
                eleicao.DataInicio,
                eleicao.DataFim,
                eleicao.Ativa
            },
            Estatisticas = new
            {
                TotalVotos = totalVotos,
                VotosValidos = votosValidos,
                VotosNulos = votosNulos,
                VotosBrancos = votosBrancos,
                PercentualValidos = totalVotos > 0 ? (double)votosValidos / totalVotos * 100 : 0,
                PercentualNulos = totalVotos > 0 ? (double)votosNulos / totalVotos * 100 : 0,
                PercentualBrancos = totalVotos > 0 ? (double)votosBrancos / totalVotos * 100 : 0
            },
            ResultadosPorCargo = resultadoPorCargo
        };
    }

    public async Task<string> GerarHashVotoAsync(string cpfEleitor, int eleicaoId, int cargoEleitoralId, int? candidatoId, DateTime dataVoto)
    {
        var dadosVoto = $"{cpfEleitor}|{eleicaoId}|{cargoEleitoralId}|{candidatoId}|{dataVoto:yyyy-MM-dd HH:mm:ss}";
        
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(dadosVoto);
        var hash = sha256.ComputeHash(bytes);
        
        return Convert.ToBase64String(hash);
    }

    public async Task<bool> VerificarIntegridadeVotoAsync(int votoId)
    {
        var voto = await _context.Votos.FindAsync(votoId);
        if (voto == null)
            return false;

        var hashCalculado = await GerarHashVotoAsync(voto.CpfEleitor, voto.EleicaoId, voto.CargoEleitoralId, voto.CandidatoId, voto.DataVoto);
        return voto.HashVoto == hashCalculado;
    }
} 