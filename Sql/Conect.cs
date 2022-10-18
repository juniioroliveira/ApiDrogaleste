using System.Data.SqlClient;

namespace ApiDrogaleste.Sql
{
    internal class Conect
    {
        public static SqlConnection con = new();
        public static SqlConnection? Conectar(Servidor servidor, Database database)
        {

            string servidorlinq = servidor == Servidor.LOCALHOST ? servidor + @"\SQLEXPRESS" : Convert.ToString(servidor.ToString().Replace("_", "."));
            string[] UsuarioLocal = { "SA", "873562" };
            string[] UsuarioDrogaleste = { "Drogaleste.Consulta", "Drogaleste@2021!@#" };
            string[] Login = database == Database.PBS_DROGALESTE_DADOS ? UsuarioDrogaleste : UsuarioLocal;

            con = new SqlConnection();

            #region METODO CONECTAR
            try
            {

                if (con.State == System.Data.ConnectionState.Closed)
                {
                    ///CONSTRUTOR
                    con.ConnectionString = @"DATA SOURCE=" + servidorlinq + @"; INITIAL CATALOG=" + database + @"; USER ID=" + Login[0] + "; PASSWORD=" + Login[1] + ";";
                    con.Open();
                }
            }
            catch (Exception ex)
            {
                //Global.Log.Capitute(tipoMensagem: Global.TipoMensagem.Erro, Acao: "Conectar ao banco de dados", MensagemEx: ex.Message, HelpLink: ex.HelpLink);
                //App.Current.MainWindow.Close();
            }
            #endregion

            return con;
        }
    }
}
