using System;
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
    }
}