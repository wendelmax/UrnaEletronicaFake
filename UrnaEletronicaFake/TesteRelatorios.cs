using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using UrnaEletronicaFake.Data;
using UrnaEletronicaFake.Services;
using UrnaEletronicaFake.Models;
using Microsoft.EntityFrameworkCore;

namespace UrnaEletronicaFake;

public class TesteRelatorios
{
    private readonly IServiceProvider _serviceProvider;
    private readonly UrnaDbContext _context;

    public TesteRelatorios(IServiceProvider serviceProvider, UrnaDbContext context)
    {
        _serviceProvider = serviceProvider;
        _context = context;
    }

    public async Task ExecutarTesteCompleto()
    {
        Console.WriteLine("=== INICIANDO TESTE DE RELATÓRIOS E FINALIZAÇÃO ===\n");

        try
        {
            // 1. Verificar eleições existentes
            await VerificarEleicoesExistentes();

            // 2. Simular votos para uma eleição
            await SimularVotos();

            // 3. Testar geração de resultados
            await TestarResultados();

            // 4. Testar auditoria
            await TestarAuditoria();

            // 5. Finalizar eleição
            await FinalizarEleicao();

            Console.WriteLine("\n=== TESTE CONCLUÍDO COM SUCESSO ===");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nERRO NO TESTE: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
        }
    }

    private async Task VerificarEleicoesExistentes()
    {
        Console.WriteLine("1. VERIFICANDO ELEIÇÕES EXISTENTES...");
        
        var eleicoes = await _context.Eleicoes
            .Include(e => e.CargosEleitorais.Where(c => c.Ativo))
            .Include(e => e.Candidatos.Where(c => c.Ativo))
            .ToListAsync();

        Console.WriteLine($"   Total de eleições: {eleicoes.Count}");
        
        foreach (var eleicao in eleicoes)
        {
            Console.WriteLine($"   - {eleicao.Titulo} (ID: {eleicao.Id})");
            Console.WriteLine($"     Ativa: {eleicao.Ativa}");
        Console.WriteLine($"     Cargos: {eleicao.CargosEleitorais.Count()}");
        Console.WriteLine($"     Candidatos: {eleicao.Candidatos.Count()}");
            Console.WriteLine($"     Período: {eleicao.DataInicio:dd/MM/yyyy} a {eleicao.DataFim:dd/MM/yyyy}");
        }
        Console.WriteLine();
    }

    private async Task SimularVotos()
    {
        Console.WriteLine("2. SIMULANDO VOTOS...");
        
        var eleicaoAtiva = await _context.Eleicoes
            .Include(e => e.CargosEleitorais.Where(c => c.Ativo))
            .Include(e => e.Candidatos.Where(c => c.Ativo))
            .FirstOrDefaultAsync(e => e.Ativa);

        if (eleicaoAtiva == null)
        {
            Console.WriteLine("   Nenhuma eleição ativa encontrada. Criando eleição de teste...");
            await CriarEleicaoTeste();
            eleicaoAtiva = await _context.Eleicoes
                .Include(e => e.CargosEleitorais.Where(c => c.Ativo))
                .Include(e => e.Candidatos.Where(c => c.Ativo))
                .FirstOrDefaultAsync(e => e.Ativa);
        }

        if (eleicaoAtiva == null)
        {
            Console.WriteLine("   Erro: Não foi possível criar eleição de teste");
            return;
        }

        var votoService = _serviceProvider.GetRequiredService<IVotoService>();
        var auditoriaService = _serviceProvider.GetRequiredService<IAuditoriaService>();

        // Simular alguns votos
        var cpfs = new[] { "11111111111", "22222222222", "33333333333", "44444444444", "55555555555" };
        var candidatos = eleicaoAtiva.Candidatos.ToList();
        var cargos = eleicaoAtiva.CargosEleitorais.ToList();

        Console.WriteLine($"   Simulando votos para: {eleicaoAtiva.Titulo}");
        Console.WriteLine($"   Cargos disponíveis: {cargos.Count()}");
        Console.WriteLine($"   Candidatos disponíveis: {candidatos.Count()}");

        for (int i = 0; i < cpfs.Length; i++)
        {
            var cpf = cpfs[i];
            Console.WriteLine($"   Votando eleitor {i + 1}: {cpf}");

            foreach (var cargo in cargos)
            {
                var candidatosDoCargo = candidatos.Where(c => c.CargoEleitoralId == cargo.Id).ToList();
                
                if (candidatosDoCargo.Any())
                {
                    var candidatoEscolhido = candidatosDoCargo[i % candidatosDoCargo.Count()];
                    await votoService.RegistrarVotoAsync(cpf, eleicaoAtiva.Id, cargo.Id, candidatoEscolhido.Id);
                    Console.WriteLine($"     Cargo {cargo.Nome}: {candidatoEscolhido.Nome} ({candidatoEscolhido.Numero})");
                }
                else
                {
                    // Voto branco se não há candidatos
                    await votoService.RegistrarVotoAsync(cpf, eleicaoAtiva.Id, cargo.Id, null, votoBranco: true);
                    Console.WriteLine($"     Cargo {cargo.Nome}: VOTO BRANCO");
                }
            }

            await auditoriaService.RegistrarAcaoAsync("VOTAR", "Voto", null, 
                $"Eleitor {cpf} votou na eleição {eleicaoAtiva.Titulo}");
        }

        Console.WriteLine("   Votos simulados com sucesso!\n");
    }

    private async Task CriarEleicaoTeste()
    {
        var eleicaoService = _serviceProvider.GetRequiredService<IEleicaoService>();
        
        var eleicao = new Eleicao
        {
            Titulo = "Eleição de Teste - Relatórios",
            Descricao = "Eleição criada para teste de relatórios e finalização",
            DataInicio = DateTime.Now.AddDays(-1),
            DataFim = DateTime.Now.AddDays(1),
            Ativa = true
        };

        await eleicaoService.CriarEleicaoAsync(eleicao);

        // Criar cargos de teste
        var cargos = new[]
        {
            new CargoEleitoral { Nome = "Prefeito", Ordem = 1, Ativo = true, EleicaoId = eleicao.Id },
            new CargoEleitoral { Nome = "Vereador", Ordem = 2, Ativo = true, EleicaoId = eleicao.Id }
        };

        _context.CargosEleitorais.AddRange(cargos);
        await _context.SaveChangesAsync();

        // Criar candidatos de teste
        var candidatos = new[]
        {
            new Candidato { Nome = "João Silva", Partido = "PT", Numero = "13", CargoEleitoralId = cargos[0].Id, EleicaoId = eleicao.Id, Ativo = true },
            new Candidato { Nome = "Maria Santos", Partido = "PSDB", Numero = "45", CargoEleitoralId = cargos[0].Id, EleicaoId = eleicao.Id, Ativo = true },
            new Candidato { Nome = "Pedro Costa", Partido = "PSOL", Numero = "50", CargoEleitoralId = cargos[1].Id, EleicaoId = eleicao.Id, Ativo = true },
            new Candidato { Nome = "Ana Lima", Partido = "DEM", Numero = "25", CargoEleitoralId = cargos[1].Id, EleicaoId = eleicao.Id, Ativo = true }
        };

        _context.Candidatos.AddRange(candidatos);
        await _context.SaveChangesAsync();

        Console.WriteLine("   Eleição de teste criada com sucesso!");
    }

    private async Task TestarResultados()
    {
        Console.WriteLine("3. TESTANDO GERAÇÃO DE RESULTADOS...");
        
        var votoService = _serviceProvider.GetRequiredService<IVotoService>();
        var eleicaoAtiva = await _context.Eleicoes.FirstOrDefaultAsync(e => e.Ativa);

        if (eleicaoAtiva == null)
        {
            Console.WriteLine("   Nenhuma eleição ativa para testar resultados");
            return;
        }

        var resultado = await votoService.ObterResultadoEleicaoAsync(eleicaoAtiva.Id);
        
        Console.WriteLine($"   Resultados para: {eleicaoAtiva.Titulo}");
        Console.WriteLine($"   Resultado gerado com sucesso! Tipo: {resultado.GetType().Name}");
        
        // Verificar votos diretamente no banco
        var votos = await _context.Votos.Where(v => v.EleicaoId == eleicaoAtiva.Id).ToListAsync();
        Console.WriteLine($"   Total de votos no banco: {votos.Count()}");
        Console.WriteLine($"   Votos válidos: {votos.Count(v => !v.VotoNulo && !v.VotoBranco)}");
        Console.WriteLine($"   Votos nulos: {votos.Count(v => v.VotoNulo)}");
        Console.WriteLine($"   Votos brancos: {votos.Count(v => v.VotoBranco)}");
        Console.WriteLine();
    }

    private async Task TestarAuditoria()
    {
        Console.WriteLine("4. TESTANDO AUDITORIA...");
        
        var auditoriaService = _serviceProvider.GetRequiredService<IAuditoriaService>();
        var auditorias = await auditoriaService.ObterAuditoriasAsync();

        Console.WriteLine($"   Total de ações registradas: {auditorias.Count()}");
        
        var ultimasAuditorias = auditorias.Take(5);
        Console.WriteLine("   Últimas 5 ações:");
        foreach (var auditoria in ultimasAuditorias)
        {
            Console.WriteLine($"     {auditoria.DataAcao:dd/MM/yyyy HH:mm:ss} - {auditoria.TipoAcao} {auditoria.Entidade}: {auditoria.Descricao}");
        }
        Console.WriteLine();
    }

    private async Task FinalizarEleicao()
    {
        Console.WriteLine("5. FINALIZANDO ELEIÇÃO...");
        
        var eleicaoService = _serviceProvider.GetRequiredService<IEleicaoService>();
        var auditoriaService = _serviceProvider.GetRequiredService<IAuditoriaService>();
        
        var eleicaoAtiva = await _context.Eleicoes.FirstOrDefaultAsync(e => e.Ativa);

        if (eleicaoAtiva == null)
        {
            Console.WriteLine("   Nenhuma eleição ativa para finalizar");
            return;
        }

        // Desativar a eleição
        await eleicaoService.DesativarEleicaoAsync(eleicaoAtiva.Id);
        
        // Registrar finalização na auditoria
        await auditoriaService.RegistrarAcaoAsync("FINALIZAR", "Eleicao", eleicaoAtiva.Id, 
            $"Eleição '{eleicaoAtiva.Titulo}' finalizada");

        Console.WriteLine($"   Eleição '{eleicaoAtiva.Titulo}' finalizada com sucesso!");
        Console.WriteLine("   Status: INATIVA");
        Console.WriteLine();
    }
}
