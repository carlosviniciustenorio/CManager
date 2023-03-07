﻿using CManager.Domain.Enums;

namespace CManager.Domain.Models
{
    public class Anuncio
    {
        public Anuncio()
        {

        }
        public Anuncio(string placa, Modelo modelo, Versao versao, List<TipoCombustivel> tiposCombustiveis, int portas, ECambio cambio, ECor cor, List<Opcional> opcionais, List<Caracteristica> caracteristicas, string km, string estado, decimal preco, string usuarioId, bool exibirTelefone, bool exibirEmail)
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
            Preco = preco;
            UsuarioId = usuarioId;
            ExibirTelefone = exibirTelefone;
            ExibirEmail = exibirEmail;
        }

        public Guid Id { get; init; }
        public string Placa { get; private set; }
        public Marca Marca { get; private set; }
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
        public decimal Preco { get; private set; }
        public string UsuarioId { get; private set; }
        public bool ExibirTelefone { get; private set; } = false;
        public bool ExibirEmail { get; private set; } = false;
    }
}
