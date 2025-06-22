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
            .Include(e => e.CargosEleitorais.Where(c => c.Ativo))
            .Include(e => e.Candidatos.Where(c => c.Ativo))
            .OrderByDescending(e => e.DataCriacao)
            .ToListAsync();
    }

    public async Task<Eleicao?> ObterEleicaoPorIdAsync(int id)
    {
        return await _context.Eleicoes
            .Include(e => e.CargosEleitorais.Where(c => c.Ativo).OrderBy(c => c.Ordem))
            .Include(e => e.Candidatos.Where(c => c.Ativo))
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Eleicao?> ObterEleicaoAtivaAsync()
    {
        return await _context.Eleicoes
            .Include(e => e.CargosEleitorais.Where(c => c.Ativo).OrderBy(c => c.Ordem))
            .Include(e => e.Candidatos.Where(c => c.Ativo))
            .FirstOrDefaultAsync(e => e.Ativa);
    }

    public async Task<Eleicao> CriarEleicaoAsync(Eleicao eleicao)
    {
        eleicao.DataCriacao = DateTime.Now;
        _context.Eleicoes.Add(eleicao);
        await _context.SaveChangesAsync();

        await _auditoriaService.RegistrarAcaoAsync("CRIAR", "Eleicao", eleicao.Id, 
            $"Eleição '{eleicao.Titulo}' criada");

        return eleicao;
    }

    public async Task<Eleicao> AtualizarEleicaoAsync(Eleicao eleicao)
    {
        var eleicaoExistente = await _context.Eleicoes.FindAsync(eleicao.Id);
        if (eleicaoExistente == null)
            throw new ArgumentException("Eleição não encontrada");

        eleicaoExistente.Titulo = eleicao.Titulo;
        eleicaoExistente.Descricao = eleicao.Descricao;
        eleicaoExistente.DataInicio = eleicao.DataInicio;
        eleicaoExistente.DataFim = eleicao.DataFim;

        await _context.SaveChangesAsync();

        await _auditoriaService.RegistrarAcaoAsync("ATUALIZAR", "Eleicao", eleicao.Id, 
            $"Eleição '{eleicao.Titulo}' atualizada");

        return eleicaoExistente;
    }

    public async Task<bool> DeletarEleicaoAsync(int id)
    {
        var eleicao = await _context.Eleicoes.FindAsync(id);
        if (eleicao == null) return false;

        _context.Eleicoes.Remove(eleicao);
        await _context.SaveChangesAsync();

        await _auditoriaService.RegistrarAcaoAsync("DELETAR", "Eleicao", id, 
            $"Eleição '{eleicao.Titulo}' deletada");

        return true;
    }

    public async Task<bool> AtivarEleicaoAsync(int id)
    {
        var eleicao = await _context.Eleicoes.FindAsync(id);
        if (eleicao == null) return false;

        eleicao.Ativa = true;
        await _context.SaveChangesAsync();

        await _auditoriaService.RegistrarAcaoAsync("ATIVAR", "Eleicao", id, 
            $"Eleição '{eleicao.Titulo}' ativada");

        return true;
    }

    public async Task<bool> DesativarEleicaoAsync(int id)
    {
        var eleicao = await _context.Eleicoes.FindAsync(id);
        if (eleicao == null) return false;

        eleicao.Ativa = false;
        await _context.SaveChangesAsync();

        await _auditoriaService.RegistrarAcaoAsync("DESATIVAR", "Eleicao", id, 
            $"Eleição '{eleicao.Titulo}' desativada");

        return true;
    }

    public async Task<bool> EleicaoEstaAtivaAsync(int id)
    {
        var eleicao = await _context.Eleicoes.FindAsync(id);
        return eleicao?.Ativa == true;
    }

    // Métodos para gerenciar candidatos
    public async Task<IEnumerable<Candidato>> ObterCandidatosAsync(int eleicaoId)
    {
        return await _context.Candidatos
            .Include(c => c.CargoEleitoral)
            .Where(c => c.EleicaoId == eleicaoId && c.Ativo)
            .OrderBy(c => c.CargoEleitoral.Ordem)
            .ThenBy(c => c.Numero)
            .ToListAsync();
    }

    public async Task<Candidato> AdicionarCandidatoAsync(int eleicaoId, Candidato candidato)
    {
        candidato.EleicaoId = eleicaoId;
        _context.Candidatos.Add(candidato);
        await _context.SaveChangesAsync();

        await _auditoriaService.RegistrarAcaoAsync("ADICIONAR", "Candidato", candidato.Id, 
            $"Candidato '{candidato.Nome}' adicionado à eleição {eleicaoId}");

        return candidato;
    }

    public async Task<Candidato> AtualizarCandidatoAsync(int eleicaoId, Candidato candidato)
    {
        var candidatoExistente = await _context.Candidatos
            .FirstOrDefaultAsync(c => c.Id == candidato.Id && c.EleicaoId == eleicaoId);
        
        if (candidatoExistente == null)
            throw new ArgumentException("Candidato não encontrado");

        candidatoExistente.Nome = candidato.Nome;
        candidatoExistente.Partido = candidato.Partido;
        candidatoExistente.Numero = candidato.Numero;
        candidatoExistente.Foto = candidato.Foto;
        candidatoExistente.CargoEleitoralId = candidato.CargoEleitoralId;

        await _context.SaveChangesAsync();

        await _auditoriaService.RegistrarAcaoAsync("ATUALIZAR", "Candidato", candidato.Id, 
            $"Candidato '{candidato.Nome}' atualizado");

        return candidatoExistente;
    }

    public async Task<bool> RemoverCandidatoAsync(int eleicaoId, int candidatoId)
    {
        var candidato = await _context.Candidatos
            .FirstOrDefaultAsync(c => c.Id == candidatoId && c.EleicaoId == eleicaoId);
        
        if (candidato == null) return false;

        candidato.Ativo = false;
        await _context.SaveChangesAsync();

        await _auditoriaService.RegistrarAcaoAsync("REMOVER", "Candidato", candidatoId, 
            $"Candidato '{candidato.Nome}' removido");

        return true;
    }

    // Métodos para gerenciar cargos eleitorais
    public async Task<IEnumerable<CargoEleitoral>> ObterCargosAsync(int eleicaoId)
    {
        return await _context.CargosEleitorais
            .Where(c => c.EleicaoId == eleicaoId && c.Ativo)
            .OrderBy(c => c.Ordem)
            .ToListAsync();
    }

    public async Task<CargoEleitoral> AdicionarCargoAsync(int eleicaoId, CargoEleitoral cargo)
    {
        cargo.EleicaoId = eleicaoId;
        
        // Definir ordem automaticamente se não fornecida
        if (cargo.Ordem == 0)
        {
            var ultimaOrdem = await _context.CargosEleitorais
                .Where(c => c.EleicaoId == eleicaoId)
                .MaxAsync(c => (int?)c.Ordem) ?? 0;
            cargo.Ordem = ultimaOrdem + 1;
        }

        _context.CargosEleitorais.Add(cargo);
        await _context.SaveChangesAsync();

        await _auditoriaService.RegistrarAcaoAsync("ADICIONAR", "CargoEleitoral", cargo.Id, 
            $"Cargo '{cargo.Nome}' adicionado à eleição {eleicaoId}");

        return cargo;
    }

    public async Task<CargoEleitoral> AtualizarCargoAsync(int eleicaoId, CargoEleitoral cargo)
    {
        var cargoExistente = await _context.CargosEleitorais
            .FirstOrDefaultAsync(c => c.Id == cargo.Id && c.EleicaoId == eleicaoId);
        
        if (cargoExistente == null)
            throw new ArgumentException("Cargo não encontrado");

        cargoExistente.Nome = cargo.Nome;
        cargoExistente.QuantidadeDigitos = cargo.QuantidadeDigitos;
        cargoExistente.Ordem = cargo.Ordem;

        await _context.SaveChangesAsync();

        await _auditoriaService.RegistrarAcaoAsync("ATUALIZAR", "CargoEleitoral", cargo.Id, 
            $"Cargo '{cargo.Nome}' atualizado");

        return cargoExistente;
    }

    public async Task<bool> RemoverCargoAsync(int eleicaoId, int cargoId)
    {
        var cargo = await _context.CargosEleitorais
            .FirstOrDefaultAsync(c => c.Id == cargoId && c.EleicaoId == eleicaoId);
        
        if (cargo == null) return false;

        cargo.Ativo = false;
        await _context.SaveChangesAsync();

        await _auditoriaService.RegistrarAcaoAsync("REMOVER", "CargoEleitoral", cargoId, 
            $"Cargo '{cargo.Nome}' removido");

        return true;
    }
} 