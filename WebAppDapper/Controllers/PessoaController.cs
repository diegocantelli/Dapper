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
    }
}