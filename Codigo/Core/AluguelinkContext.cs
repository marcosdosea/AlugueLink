using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Core;

public partial class AluguelinkContext : DbContext
{
    public AluguelinkContext()
    {
    }

    public AluguelinkContext(DbContextOptions<AluguelinkContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Aluguel> Aluguels { get; set; }

    public virtual DbSet<Imovel> Imovels { get; set; }

    public virtual DbSet<Locador> Locadors { get; set; }

    public virtual DbSet<Locatario> Locatarios { get; set; }

    public virtual DbSet<Manutencao> Manutencaos { get; set; }

    public virtual DbSet<Pagamento> Pagamentos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
       
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_unicode_ci");

        modelBuilder.Entity<Aluguel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("aluguel");

            entity.HasIndex(e => e.Idimovel, "fk_aluguel_imovel1_idx");

            entity.HasIndex(e => e.Idlocatario, "fk_aluguel_locatario1_idx");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DataAssinatura).HasColumnName("dataAssinatura");
            entity.Property(e => e.DataFim).HasColumnName("dataFim");
            entity.Property(e => e.DataInicio).HasColumnName("dataInicio");
            entity.Property(e => e.Idimovel).HasColumnName("idimovel");
            entity.Property(e => e.Idlocatario).HasColumnName("idlocatario");
            entity.Property(e => e.Status)
                .HasColumnType("enum('A','F','P')")
                .HasColumnName("status");

            entity.HasOne(d => d.IdimovelNavigation).WithMany(p => p.Aluguels)
                .HasForeignKey(d => d.Idimovel)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_aluguel_imovel1");

            entity.HasOne(d => d.IdlocatarioNavigation).WithMany(p => p.Aluguels)
                .HasForeignKey(d => d.Idlocatario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_aluguel_locatario1");
        });

        modelBuilder.Entity<Imovel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("imovel");

            entity.HasIndex(e => e.IdLocador, "fk_imovel_locador_idx");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Area)
                .HasPrecision(10, 2)
                .HasColumnName("area");
            entity.Property(e => e.Bairro)
                .HasMaxLength(50)
                .HasColumnName("bairro");
            entity.Property(e => e.Banheiros).HasColumnName("banheiros");
            entity.Property(e => e.Cep)
                .HasMaxLength(8)
                .HasColumnName("cep");
            entity.Property(e => e.Cidade)
                .HasMaxLength(45)
                .HasColumnName("cidade");
            entity.Property(e => e.Complemento)
                .HasMaxLength(50)
                .HasColumnName("complemento");
            entity.Property(e => e.Descricao)
                .HasMaxLength(200)
                .HasColumnName("descricao");
            entity.Property(e => e.Estado)
                .HasMaxLength(2)
                .IsFixedLength()
                .HasColumnName("estado");
            entity.Property(e => e.IdLocador).HasColumnName("idLocador");
            entity.Property(e => e.Logradouro)
                .HasMaxLength(100)
                .HasColumnName("logradouro");
            entity.Property(e => e.Numero)
                .HasMaxLength(5)
                .HasColumnName("numero");
            entity.Property(e => e.Quartos).HasColumnName("quartos");
            entity.Property(e => e.Tipo)
                .HasColumnType("enum('C','A','PC')")
                .HasColumnName("tipo");
            entity.Property(e => e.VagasGaragem).HasColumnName("vagasGaragem");
            entity.Property(e => e.Valor)
                .HasPrecision(10, 2)
                .HasColumnName("valor");

            entity.HasOne(d => d.IdLocadorNavigation).WithMany(p => p.Imovels)
                .HasForeignKey(d => d.IdLocador)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_imovel_locador");
        });

        modelBuilder.Entity<Locador>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("locador");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Cpf)
                .HasMaxLength(11)
                .IsFixedLength()
                .HasColumnName("cpf");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Nome)
                .HasMaxLength(100)
                .HasColumnName("nome");
            entity.Property(e => e.Telefone)
                .HasMaxLength(20)
                .HasColumnName("telefone");
        });

        modelBuilder.Entity<Locatario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("locatario");

            entity.HasIndex(e => e.Cpf, "cpf_UNIQUE").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Bairro)
                .HasMaxLength(50)
                .HasColumnName("bairro");
            entity.Property(e => e.Cep)
                .HasMaxLength(8)
                .HasColumnName("cep");
            entity.Property(e => e.Cidade)
                .HasMaxLength(45)
                .HasColumnName("cidade");
            entity.Property(e => e.Complemento)
                .HasMaxLength(50)
                .HasColumnName("complemento");
            entity.Property(e => e.Cpf)
                .HasMaxLength(11)
                .IsFixedLength()
                .HasColumnName("cpf");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Estado)
                .HasMaxLength(2)
                .IsFixedLength()
                .HasColumnName("estado");
            entity.Property(e => e.Logradouro)
                .HasMaxLength(100)
                .HasColumnName("logradouro");
            entity.Property(e => e.Nome)
                .HasMaxLength(100)
                .HasColumnName("nome");
            entity.Property(e => e.Numero)
                .HasMaxLength(5)
                .HasColumnName("numero");
            entity.Property(e => e.Profissao)
                .HasMaxLength(100)
                .HasColumnName("profissao");
            entity.Property(e => e.Renda)
                .HasPrecision(10, 2)
                .HasColumnName("renda");
            entity.Property(e => e.Telefone1)
                .HasMaxLength(11)
                .IsFixedLength()
                .HasColumnName("telefone1");
            entity.Property(e => e.Telefone2)
                .HasMaxLength(11)
                .IsFixedLength()
                .HasColumnName("telefone2");

            entity.HasMany(d => d.Idlocadors).WithMany(p => p.Idlocatarios)
                .UsingEntity<Dictionary<string, object>>(
                    "Locatariolocador",
                    r => r.HasOne<Locador>().WithMany()
                        .HasForeignKey("Idlocador")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_locatariolocador_locador1"),
                    l => l.HasOne<Locatario>().WithMany()
                        .HasForeignKey("Idlocatario")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("fk_locatariolocador_locatario1"),
                    j =>
                    {
                        j.HasKey("Idlocatario", "Idlocador")
                            .HasName("PRIMARY")
                            .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                        j.ToTable("locatariolocador");
                        j.HasIndex(new[] { "Idlocador" }, "fk_locatariolocador_locador1_idx");
                        j.HasIndex(new[] { "Idlocatario" }, "fk_locatariolocador_locatario1_idx");
                        j.IndexerProperty<int>("Idlocatario").HasColumnName("idlocatario");
                        j.IndexerProperty<int>("Idlocador").HasColumnName("idlocador");
                    });
        });

        modelBuilder.Entity<Manutencao>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("manutencao");

            entity.HasIndex(e => e.Idimovel, "fk_manutencao_imovel1_idx");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DataSolicitacao)
                .HasColumnType("datetime")
                .HasColumnName("dataSolicitacao");
            entity.Property(e => e.Descricao)
                .HasMaxLength(200)
                .HasColumnName("descricao");
            entity.Property(e => e.Idimovel).HasColumnName("idimovel");
            entity.Property(e => e.Status)
                .HasComment("P - PEDIDO REALIZADO\nA - ATENDIDA\nC - CANCELADA")
                .HasColumnType("enum('P','A','C')")
                .HasColumnName("status");
            entity.Property(e => e.Valor)
                .HasPrecision(10, 2)
                .HasColumnName("valor");

            entity.HasOne(d => d.IdimovelNavigation).WithMany(p => p.Manutencaos)
                .HasForeignKey(d => d.Idimovel)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_manutencao_imovel1");
        });

        modelBuilder.Entity<Pagamento>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("pagamento");

            entity.HasIndex(e => e.Idaluguel, "fk_pagamento_aluguel1_idx");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DataPagamento)
                .HasColumnType("datetime")
                .HasColumnName("dataPagamento");
            entity.Property(e => e.Idaluguel).HasColumnName("idaluguel");
            entity.Property(e => e.TipoPagamento)
                .HasColumnType("enum('CD','CC','P','B')")
                .HasColumnName("tipoPagamento");
            entity.Property(e => e.Valor)
                .HasPrecision(10, 2)
                .HasColumnName("valor");

            entity.HasOne(d => d.IdaluguelNavigation).WithMany(p => p.Pagamentos)
                .HasForeignKey(d => d.Idaluguel)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_pagamento_aluguel1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
