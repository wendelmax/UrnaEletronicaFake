using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UrnaEletronicaFake.Data;
using UrnaEletronicaFake.Models;

namespace UrnaEletronicaFake.Services;

public class AuditoriaService : IAuditoriaService
{
    private readonly UrnaDbContext _context;

    public AuditoriaService(UrnaDbContext context)
    {
        _context = context;
    }

    public async Task<Auditoria> RegistrarAcaoAsync(string tipoAcao, string entidade, int? entidadeId, 
        string descricao, string? dadosAnteriores = null, string? dadosNovos = null)
    {
        var auditoria = new Auditoria
        {
            TipoAcao = tipoAcao,
            Entidade = entidade,
            EntidadeId = entidadeId,
            Descricao = descricao,
            DadosAnteriores = dadosAnteriores,
            DadosNovos = dadosNovos,
            DataAcao = DateTime.Now,
            Usuario = "Sistema", // Em um sistema real, viria do contexto de autenticação
            IpAddress = "127.0.0.1" // Em um sistema real, viria do contexto da requisição
        };

        _context.Auditorias.Add(auditoria);
        await _context.SaveChangesAsync();

        return auditoria;
    }

    public async Task<IEnumerable<Auditoria>> ObterAuditoriasAsync(DateTime? dataInicio = null, DateTime? dataFim = null)
    {
        var query = _context.Auditorias.AsQueryable();

        if (dataInicio.HasValue)
            query = query.Where(a => a.DataAcao >= dataInicio.Value);

        if (dataFim.HasValue)
            query = query.Where(a => a.DataAcao <= dataFim.Value);

        return await query
            .OrderByDescending(a => a.DataAcao)
            .ToListAsync();
    }

    public async Task<IEnumerable<Auditoria>> ObterAuditoriasPorEleicaoAsync(int eleicaoId)
    {
        return await _context.Auditorias
            .Where(a => a.EleicaoId == eleicaoId)
            .OrderByDescending(a => a.DataAcao)
            .ToListAsync();
    }

    public async Task<IEnumerable<Auditoria>> ObterAuditoriasPorEntidadeAsync(string entidade, int? entidadeId = null)
    {
        var query = _context.Auditorias.Where(a => a.Entidade == entidade);

        if (entidadeId.HasValue)
            query = query.Where(a => a.EntidadeId == entidadeId.Value);

        return await query
            .OrderByDescending(a => a.DataAcao)
            .ToListAsync();
    }
} 