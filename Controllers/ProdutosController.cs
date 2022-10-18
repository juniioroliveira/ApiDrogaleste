using ApiDrogaleste.Objects;
using ApiDrogaleste.Sql;
using Microsoft.AspNetCore.Mvc;

namespace ApiDrogaleste.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProdutosController : ControllerBase
    {
        [HttpGet(Name = "Getprodutos")]
        public List<Produtos> Get()
        {
            List<Produtos> produtos = new List<Produtos>();
            var dados = Execute.Reader("SELECT TOP 5 PRODUTO, DESCRICAO FROM PRODUTOS", null, Servidor.DROGALESTE_PROCFIT_COM_BR, Database.PBS_DROGALESTE_DADOS);
            while(dados.Read())
            {
                produtos.Add(new Produtos()
                {
                    produto = dados[0].ToString(),
                    descricao = dados[1].ToString()
                });
            }

            return produtos;
        }
    }
}