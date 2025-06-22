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

public class EleicaoService : IEleicaoService
{
    private readonly UrnaDbContext _context;
    private readonly IAuditoriaService _auditoriaService;

    public EleicaoService(UrnaDbContext context, IAuditoriaService auditoriaService)
    {
        _context = context;
        _auditoriaService = auditoriaService;
    }

    public async Task<IEnumerable<Eleicao>> ObterTodasEleicoesAsync()
    {
        return await _context.Eleicoes
            .Include(e => e.Candidatos)
            .OrderByDescending(e => e.DataCriacao)
            .ToListAsync();
    }

    public async Task<Eleicao?> ObterEleicaoPorIdAsync(int id)
    {
        return await _context.Eleicoes
            .Include(e => e.Candidatos)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Eleicao?> ObterEleicaoAtivaAsync()
    {
        return await _context.Eleicoes
            .Include(e => e.Candidatos.Where(c => c.Ativo))
            .FirstOrDefaultAsync(e => e.Ativa && e.DataInicio <= DateTime.Now && e.DataFim >= DateTime.Now);
    }

    public async Task<Eleicao> CriarEleicaoAsync(Eleicao eleicao)
    {
        eleicao.DataCriacao = DateTime.Now;
        eleicao.Ativa = false;
        
        _context.Eleicoes.Add(eleicao);
        await _context.SaveChangesAsync();

        await _auditoriaService.RegistrarAcaoAsync("CRIAR", "Eleicao", eleicao.Id, 
            $"Eleição '{eleicao.Titulo}' criada", null, SerializarEleicao(eleicao));

        return eleicao;
    }

    public async Task<Eleicao> AtualizarEleicaoAsync(Eleicao eleicao)
    {
        var eleicaoExistente = await _context.Eleicoes.FindAsync(eleicao.Id);
        if (eleicaoExistente == null)
            throw new ArgumentException("Eleição não encontrada");

        var dadosAnteriores = SerializarEleicao(eleicaoExistente);
        
        eleicaoExistente.Titulo = eleicao.Titulo;
        eleicaoExistente.Descricao = eleicao.Descricao;
        eleicaoExistente.DataInicio = eleicao.DataInicio;
        eleicaoExistente.DataFim = eleicao.DataFim;

        await _context.SaveChangesAsync();

        await _auditoriaService.RegistrarAcaoAsync("ATUALIZAR", "Eleicao", eleicao.Id,
            $"Eleição '{eleicao.Titulo}' atualizada", dadosAnteriores, SerializarEleicao(eleicaoExistente));

        return eleicaoExistente;
    }

    public async Task<bool> DeletarEleicaoAsync(int id)
    {
        var eleicao = await _context.Eleicoes.FindAsync(id);
        if (eleicao == null)
            return false;

        if (eleicao.Ativa)
            throw new InvalidOperationException("Não é possível deletar uma eleição ativa");

        var dadosAnteriores = SerializarEleicao(eleicao);
        
        _context.Eleicoes.Remove(eleicao);
        await _context.SaveChangesAsync();

        await _auditoriaService.RegistrarAcaoAsync("DELETAR", "Eleicao", id,
            $"Eleição '{eleicao.Titulo}' deletada", dadosAnteriores, null);

        return true;
    }

    public async Task<bool> AtivarEleicaoAsync(int id)
    {
        var eleicao = await _context.Eleicoes.FindAsync(id);
        if (eleicao == null)
            return false;

        // Desativar todas as outras eleições
        var eleicoesAtivas = await _context.Eleicoes.Where(e => e.Ativa).ToListAsync();
        foreach (var eleicaoAtiva in eleicoesAtivas)
        {
            eleicaoAtiva.Ativa = false;
        }

        eleicao.Ativa = true;
        await _context.SaveChangesAsync();

        await _auditoriaService.RegistrarAcaoAsync("ATIVAR", "Eleicao", id,
            $"Eleição '{eleicao.Titulo}' ativada", null, SerializarEleicao(eleicao));

        return true;
    }

    public async Task<bool> DesativarEleicaoAsync(int id)
    {
        var eleicao = await _context.Eleicoes.FindAsync(id);
        if (eleicao == null)
            return false;

        eleicao.Ativa = false;
        await _context.SaveChangesAsync();

        await _auditoriaService.RegistrarAcaoAsync("DESATIVAR", "Eleicao", id,
            $"Eleição '{eleicao.Titulo}' desativada", SerializarEleicao(eleicao), null);

        return true;
    }

    public async Task<bool> EleicaoEstaAtivaAsync(int id)
    {
        var eleicao = await _context.Eleicoes.FindAsync(id);
        return eleicao?.Ativa == true && 
               eleicao.DataInicio <= DateTime.Now && 
               eleicao.DataFim >= DateTime.Now;
    }

    private string SerializarEleicao(Eleicao eleicao)
    {
        return System.Text.Json.JsonSerializer.Serialize(new
        {
            eleicao.Id,
            eleicao.Titulo,
            eleicao.Descricao,
            eleicao.DataInicio,
            eleicao.DataFim,
            eleicao.Ativa,
            eleicao.DataCriacao
        });
    }
} 