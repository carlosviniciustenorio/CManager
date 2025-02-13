﻿using BusinessMotors.Domain.Enums;

namespace BusinessMotors.Domain.Models
{
    public class Anuncio
    {
        public Guid Id { get; init; }
        public string Placa { get; private set; }
        public Modelo Modelo { get; private set; }
        public Versao Versao { get; private set; }
        public List<TipoCombustivel> TiposCombustiveis { get; private set; }
        public int Portas { get; private set; }
        public ECambio Cambio { get; private set; }
        public ECor Cor { get; private set; }
        public List<Opcional> Opcionais { get; private set; }
        public List<Caracteristica> Caracteristicas {get; private set;}
        public string Km { get; private set; }
        public string Estado { get; private set; }
        public string Cidade { get; private set; }
        public decimal Preco { get; private set; }
        public string UserId { get; private set; }
        public Usuario User { get; private set; }
        public int AnoFabricacao { get; private set; }
        public int AnoVeiculo { get; private set; }
        public bool ExibirTelefone { get; private set; }
        public bool ExibirEmail { get; private set; }
        public List<Imagem> ImagensS3 { get; private set; }
        public DateTime? DataCriacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        
        public Anuncio(){}
        public Anuncio(string placa,
                       Modelo modelo,
                       Versao versao,
                       List<TipoCombustivel> tiposCombustiveis,
                       int portas,
                       ECambio cambio,
                       ECor cor,
                       List<Opcional> opcionais,
                       List<Caracteristica> caracteristicas,
                       string km,
                       string estado,
                       string cidade,
                       decimal preco,
                       string usuarioId,
                       bool exibirTelefone,
                       bool exibirEmail,
                       List<Imagem> imagens,
                       int anoFabricacao,
                       int anoVeiculo,
                       DateTime? dataAtualizacao)
        {
            Placa = placa;
            Modelo = modelo;
            Versao = versao;
            TiposCombustiveis = tiposCombustiveis;
            Portas = portas;
            Cambio = cambio;
            Cor = cor;
            Opcionais = opcionais;
            Caracteristicas = caracteristicas;
            Km = km;
            Estado = estado;
            Cidade = cidade;
            Preco = preco;
            UserId = usuarioId;
            ExibirTelefone = exibirTelefone;
            ExibirEmail = exibirEmail;
            ImagensS3 = imagens;
            AnoFabricacao = anoFabricacao;
            AnoVeiculo = anoVeiculo;
            DataCriacao = DataCriacao ?? DateTime.UtcNow;
            DataAtualizacao = dataAtualizacao;
        }

    }
}
