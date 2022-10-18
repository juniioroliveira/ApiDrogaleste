using System.Data.SqlClient;
using System.Data;
using Microsoft.SqlServer.Server;
using System.Collections.ObjectModel;

namespace ApiDrogaleste.Sql
{

    internal class Execute
    {

        private static SqlConnection con = new();
        public static SqlTransaction transaction;
        private static SqlConnection? Conectar(Servidor servidor, Database database)
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

        public static bool Update(string[] Campos, string[] Parametros, string Tabela, Servidor DataSource, Database DataBase)
        {
            try
            {
                string[] retorno = new string[Campos.Count()];

                string setCampos = null;
                int index = 0;
                foreach (var item in Campos)
                {
                    string[] Parametro = item.Split(";");
                    string Variavel = "@" + Parametro[0];
                    string Campo = Parametro[0];
                    string Valor = Parametro[1];


                    var condicao = Campo + " = " + Variavel;

                    var Linha = index > 0 ? ", " + condicao : condicao;

                    setCampos = setCampos + Linha;
                    index++;
                }
                index = 0;

                string setParametros = null;
                foreach (var item in Parametros)
                {
                    string[] Parametro = item.Split(";");
                    string Variavel = Parametro[0];
                    string Campo = Parametro[0].Replace("@", "");
                    string Valor = Parametro[1];

                    var condicao = Campo + " = '" + Valor + "' ";

                    var linha = index > 0 ? " AND " + condicao : condicao;

                    setParametros = setParametros + linha;
                    index++;
                }

                string Query = "UPDATE " + Tabela.ToUpper() + " SET " + setCampos + " WHERE " + setParametros;

                using (SqlCommand cmd = new SqlCommand(Query, Conectar(DataSource, DataBase)))
                {
                    ////transaction = con.BeginTransaction();

                    foreach (var item in Campos)
                    {
                        string[] Parametro = item.Split(";");
                        string Variavel = Parametro[0];
                        string Campo = Parametro[0].Replace("@", "");
                        string Valor = Parametro[1];

                        //cmd.Transaction = transaction;

                        cmd.Parameters.AddWithValue(Variavel, Valor);
                    }

                    cmd.ExecuteNonQuery();
                    //MessageBox.Show("Dados alterados com sucesso!");

                }

                return true;
            }
            catch (Exception EX)
            {
                //transaction.Rollback();

                //MessageBox.Show(EX.Message);
                return false;
            }
        }

        public static int Insert(string Query, string[] Variaveis, Servidor DataSource, Database DataBase)
        {
            int Scope = 0;
            try
            {
                using (SqlCommand cmd = new SqlCommand(Query, Conectar(DataSource, DataBase)))
                {
                    foreach (var declaracao in Variaveis)
                    {
                        string[] Variavel = declaracao.Split("=");

                        cmd.Parameters.AddWithValue(Variavel[0], Variavel[1]);
                    }

                    //cmd.Transaction = transaction;
                    Scope = Convert.ToInt32(cmd.ExecuteScalar());

                    //MessageBox.Show("Dados inseridos com sucesso!");
                }

                return Scope;
            }
            catch (Exception)
            {
                //transaction.Rollback();
                return Scope;
            }
        }

        public static bool Delete(string Query, string[] Variaveis, Servidor DataSource, Database DataBase)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand(Query, Conectar(DataSource, DataBase)))
                {
                    foreach (var declaracao in Variaveis)
                    {
                        string[] Variavel = declaracao.Split("=");

                        cmd.Parameters.AddWithValue(Variavel[0], Variavel[1]);
                    }

                    cmd.ExecuteNonQuery();
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static SqlDataReader Reader(string Query, string[] Parametros, Servidor DataSource, Database DataBase)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand(Query, Conectar(DataSource, DataBase)))
                {
                    if (Parametros != null)
                    {
                        foreach (var declaracao in Parametros)
                        {
                            string[] Variavel = declaracao.Split("=");

                            cmd.Parameters.AddWithValue(Variavel[0], Variavel[1]);
                        }
                    }
                    return cmd.ExecuteReader();
                    con.Close();
                }

            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                return null;
            }
        }

        public static ObservableCollection<string> GetList(string Tabela, string Coluna, Servidor DataSource, Database DataBase)
        {
            try
            {
                ObservableCollection<string> list = new ObservableCollection<string>();
                string Query = "SELECT " + Coluna.ToUpper() + " FROM " + Tabela.ToUpper();

                var dados = Reader(Query, null, DataSource, DataBase);
                if (dados != null)
                {
                    while (dados.Read())
                    {
                        list.Add(dados[0].ToString());
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                return null;
            }
        }       

        public static string[] GetText(string[] Campos, string[] Parametros, string Tabela, Servidor DataSource, Database DataBase)
        {
            string[] retorno = new string[Campos.Count()];

            string setCampos = null;
            int index = 0;
            foreach (var campo in Campos)
            {
                var Linha = index > 0 ? ", " + campo + "" : "" + campo + "";

                setCampos = setCampos + Linha;
                index++;
            }
            index = 0;

            string setParametros = null;
            foreach (var parametro in Parametros)
            {
                string[] Variavel = parametro.Split("=");
                var condicao = Variavel[0] + " = '" + Variavel[1] + "'";

                var linha = index > 0 ? " AND " + condicao : condicao;

                setParametros = setParametros + condicao;
                index++;
            }

            string Query = "SELECT " + setCampos.ToUpper() + " FROM " + Tabela.ToUpper() + " WHERE " + setParametros;

            var dados = Reader(Query, null, DataSource, DataBase);
            if (dados.Read())
            {
                for (int i = 0; i < dados.FieldCount; i++)
                {
                    if (!string.IsNullOrEmpty(dados.GetValue(i).ToString()))
                    {
                        var objectColun = (string)dados.GetValue(i);

                        retorno[i] = objectColun;
                    }

                }
            }

            return retorno;
        }


    }
}
