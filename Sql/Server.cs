namespace ApiDrogaleste.Sql
{
    internal class Server
    {
        public static string? Ip { get; set; }
        public static string? Hostname { get; set; }
        public static string? Port { get; set; }
        public static string Path { get; set; } = "LOCALHOST";
    }

    public enum Database
    {
        NONE,
        ADMINISTRATIVO,
        CONFIGURACAO,
        PARAMETROS,
        PDV,
        ACCESSCONTROL,
        BRSPBYTESOFTDC01,
        DICIONARIO,
        LOJA,
        PBS_DROGALESTE_DADOS,
        DADOS
    }

    public enum Servidor
    {
        LOCALHOST,
        SERVERSTORE,
        DROGALESTE_PROCFIT_COM_BR
    }
}
