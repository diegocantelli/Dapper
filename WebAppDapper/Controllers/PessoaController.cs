﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WebAppDapper.Entidades;

namespace WebAppDapper.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PessoaController : ControllerBase
    {
        private readonly IConfiguration _config;

        public PessoaController(IConfiguration configuration)
        {
            _config = configuration;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Pessoa>> GetPessoas()
        {
            using (SqlConnection connection = new SqlConnection(
                _config.GetConnectionString("ExemplosDapper")))
            {
                //connection.Query -> Query é o método de extensão do DAPPER
                var query = connection.Query<Pessoa>($" SELECT Id, Name, Email, GenderId " +
                    $"FROM tblPerson ");

                return Ok(query);
            }
        }

        [HttpGet("details/{id:int}")]
        public ActionResult<IEnumerable<Pessoa>> GetPessoaById(int id)
        {
            using (SqlConnection connection = new SqlConnection(
                _config.GetConnectionString("ExemplosDapper")))
            {
                //connection.Query -> Query é o método de extensão do DAPPER. Deve ser passado
                // o tipo que o resultado da query será mapeado, no caso será mapeado para um objeto 
                // do tipo Pessoa
                var query = connection.Query<Pessoa>($" SELECT Id, Name, Email, GenderId " +
                    $"FROM tblPerson " +
                    $" WHERE Id = @Id",
                    //Passage de parâmetro. Caso houvesse mais parâmetros, bastava passá-los separando
                    // com vírgulas
                    new { Id = id});

                return Ok(query);
            }
        }

        [HttpGet("dynamicQuery")]
        public ActionResult<IEnumerable<Pessoa>> GetPersonById([FromQuery]bool getPersonWithId1)
        {
            using (SqlConnection connection = new SqlConnection(
                _config.GetConnectionString("ExemplosDapper")))
            {
                var sql = $" SELECT Id, Name, Email, GenderId " +
                    $"FROM tblPerson ";

                //Possibilitando a passagem de parâmetros dinâmicos
                var dynamicParameter = new DynamicParameters();

                if (getPersonWithId1)
                {
                    sql += " WHERE Id = @Id";
                    dynamicParameter.Add("Id", 1);
                }


                //O parâmetro dinâmico é passado como parâmetro do método Query do Dapper
                var query = connection.Query<Pessoa>(sql, dynamicParameter);

                
                return Ok(query);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Post(Pessoa pessoa)
        {
            using (SqlConnection connection = new SqlConnection(
                _config.GetConnectionString("ExemplosDapper")))
            {
                //Cria a instrução necessária para o insert
                var sql = $" INSERT INTO dbo.tblPerson ( Id, Name, Email, GenderId ) " +
                    $" VALUES (@id, @name, @email, @genderId)";

                //Recebe os dados vindos da requisição e instancia um novo objeto
                var newPessoa = new Pessoa
                {
                    Id = pessoa.Id,
                    Name = pessoa.Name,
                    Email = pessoa.Email,
                    GenderId = pessoa.GenderId
                };

                //Para executar comando de Insert e update, usa-se o comando ExecuteAsync
                var query = await connection.ExecuteAsync(sql, newPessoa);

                return Ok(query);
            }
        }
    }
}