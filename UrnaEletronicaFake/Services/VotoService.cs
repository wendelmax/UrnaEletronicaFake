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

    public async Task<Voto> RegistrarVotoAsync(string cpfEleitor, int eleicaoId, int? candidatoId, 
        bool votoNulo = false, bool votoBranco = false, string? sessaoId = null)
    {
        // Verificar se a eleição está ativa
        if (!await _eleicaoService.EleicaoEstaAtivaAsync(eleicaoId))
            throw new InvalidOperationException("Eleição não está ativa ou fora do período de votação");

        // Verificar se já votou
        if (await VerificarSeJaVotouAsync(cpfEleitor, eleicaoId))
            throw new InvalidOperationException("Eleitor já votou nesta eleição");

        // Verificar se o candidato existe (se não for voto nulo/branco)
        if (candidatoId.HasValue && !votoNulo && !votoBranco)
        {
            var candidato = await _context.Candidatos
                .FirstOrDefaultAsync(c => c.Id == candidatoId.Value && c.EleicaoId == eleicaoId && c.Ativo);
            if (candidato == null)
                throw new ArgumentException("Candidato não encontrado ou inativo");
        }

        var dataVoto = DateTime.Now;
        var hashVoto = await GerarHashVotoAsync(cpfEleitor, eleicaoId, candidatoId, dataVoto);

        var voto = new Voto
        {
            CpfEleitor = cpfEleitor,
            EleicaoId = eleicaoId,
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
            $"Voto {tipoVoto} registrado para CPF {cpfEleitor} na eleição {eleicaoId}");

        return voto;
    }

    public async Task<bool> VerificarSeJaVotouAsync(string cpfEleitor, int eleicaoId)
    {
        return await _context.Votos
            .AnyAsync(v => v.CpfEleitor == cpfEleitor && v.EleicaoId == eleicaoId);
    }

    public async Task<IEnumerable<Voto>> ObterVotosPorEleicaoAsync(int eleicaoId)
    {
        return await _context.Votos
            .Include(v => v.Candidato)
            .Where(v => v.EleicaoId == eleicaoId)
            .OrderBy(v => v.DataVoto)
            .ToListAsync();
    }

    public async Task<object> ObterResultadoEleicaoAsync(int eleicaoId)
    {
        var eleicao = await _context.Eleicoes
            .Include(e => e.Candidatos.Where(c => c.Ativo))
            .FirstOrDefaultAsync(e => e.Id == eleicaoId);

        if (eleicao == null)
            throw new ArgumentException("Eleição não encontrada");

        var votos = await _context.Votos
            .Where(v => v.EleicaoId == eleicaoId)
            .ToListAsync();

        var totalVotos = votos.Count;
        var votosValidos = votos.Count(v => !v.VotoNulo && !v.VotoBranco);
        var votosNulos = votos.Count(v => v.VotoNulo);
        var votosBrancos = votos.Count(v => v.VotoBranco);

        var resultadoCandidatos = eleicao.Candidatos.Select(candidato => new
        {
            candidato.Id,
            candidato.Nome,
            candidato.Partido,
            candidato.Numero,
            Votos = votos.Count(v => v.CandidatoId == candidato.Id),
            Percentual = totalVotos > 0 ? (double)votos.Count(v => v.CandidatoId == candidato.Id) / totalVotos * 100 : 0
        }).OrderByDescending(r => r.Votos).ToList();

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
            Candidatos = resultadoCandidatos
        };
    }

    public async Task<string> GerarHashVotoAsync(string cpfEleitor, int eleicaoId, int? candidatoId, DateTime dataVoto)
    {
        var dadosVoto = $"{cpfEleitor}|{eleicaoId}|{candidatoId}|{dataVoto:yyyy-MM-dd HH:mm:ss}";
        
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

        var hashCalculado = await GerarHashVotoAsync(voto.CpfEleitor, voto.EleicaoId, voto.CandidatoId, voto.DataVoto);
        return voto.HashVoto == hashCalculado;
    }
} 