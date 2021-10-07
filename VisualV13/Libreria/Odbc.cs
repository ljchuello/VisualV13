using System;
using System.Data.Odbc;
using System.Text;

namespace VisualV13.Libreria
{
    public  class Odbc
    {
        private const string ConnectionString = "DSN=odbc_sqlfac;Uid=sa;Pwd=Sermatick3000;";

        public int Select_Count_NoMail()
        {
            int cantidad = 0;
            try
            {
                using (OdbcConnection odbcConnection = new OdbcConnection("DSN=odbc_sqlfac;Uid=sa;Pwd=Sermatick3000;"))
                {
                    OdbcCommand odbcCommand = new OdbcCommand();
                    odbcConnection.Open();
                    odbcCommand.Connection = odbcConnection;
                    odbcCommand.CommandText = "SET DATEFORMAT YMD" +
                                              "\nSELECT COUNT(db.id) AS 'Cantidad' FROM dbo.DocumentosBase db" +
                                              "\nWHERE id NOT IN(SELECT he.docBaseId FROM dbo.HistorialesEmail he) AND" +
                                              "\nEstadoId = 17 AND" +
                                              $"\ndb.fechaEmision > '{GetStringDateTime(DateTime.Now.AddDays(-30))}' AND" +
                                              $"\ndb.fechaAutorizacion < '{GetStringDateTime(DateTime.Now.AddMinutes(-15))}'; ";
                    OdbcDataReader dbReader = odbcCommand.ExecuteReader();
                    while (dbReader.Read())
                    {
                        cantidad = Convert.ToInt32(dbReader["Cantidad"]);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
            return cantidad;
        }

        /// <summary>
        /// LIstade comprobantes
        /// </summary>
        /// <returns></returns>
        public string Select_List_Comprobante()
        {
            StringBuilder stringBuilder = new StringBuilder();
            try
            {

                using (OdbcConnection odbcConnection = new OdbcConnection("DSN=odbc_sqlfac;Uid=sa;Pwd=Sermatick3000;"))
                {
                    OdbcCommand odbcCommand = new OdbcCommand();
                    odbcConnection.Open();
                    odbcCommand.Connection = odbcConnection;
                    odbcCommand.CommandText = "SET DATEFORMAT YMD" +
                                              "\nSELECT Estados.id, Estados.titulo, COUNT(Estados.titulo) AS 'Count' FROM DocumentosBase" +
                                              "\nLEFT JOIN Estados ON" +
                                              "\nDocumentosBase.estadoId = Estados.id" +
                                              "\nGROUP BY Estados.id, Estados.titulo" +
                                              "\nORDER BY Estados.id; ";
                    OdbcDataReader dbReader = odbcCommand.ExecuteReader();
                    while (dbReader.Read())
                    {
                        string titulo = Convert.ToString(dbReader["titulo"]);
                        int Count = Convert.ToInt32(dbReader["Count"]);
                        if (titulo == "ERROR AL ENVIAR DESDE MILENIUM")
                        {
                            titulo = "ERROR MILENIUM";
                        }
                        stringBuilder.AppendLine($"{titulo}: {Count:n0}");
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
            return stringBuilder.ToString();
        }


        public string Select_Get_NoAutoriado()
        {
            string id = string.Empty;
            try
            {

                using (OdbcConnection odbcConnection = new OdbcConnection("DSN=odbc_sqlfac;Uid=sa;Pwd=Sermatick3000;"))
                {
                    OdbcCommand odbcCommand = new OdbcCommand();
                    odbcConnection.Open();
                    odbcCommand.Connection = odbcConnection;
                    odbcCommand.CommandText = "SET DATEFORMAT YMD" +
                                              "\nSELECT TOP 1 db.id FROM dbo.DocumentosBase db" +
                                              "\nWHERE db.estadoId IN(11, 12, 14, 15, 18)" +
                                              "\nORDER BY db.fechaEmision";
                    OdbcDataReader dbReader = odbcCommand.ExecuteReader();
                    while (dbReader.Read())
                    {
                        id = Convert.ToString(dbReader["id"]);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
            return id;
        }

        private string GetStringDateTime(DateTime dateTime)
        {
            return $"{dateTime:yyyy-MM-dd HH:mm:ss}";
        }

        public int Select_empresaId(string idDoc)
        {
            int empresaId = 0;
            try
            {
                using (OdbcConnection sqlConnection = new OdbcConnection("DSN=odbc_sqlfac;Uid=sa;Pwd=Sermatick3000;"))
                {
                    OdbcCommand odbcCommand = new OdbcCommand();
                    sqlConnection.Open();
                    odbcCommand.Connection = sqlConnection;
                    odbcCommand.CommandText = string.Concat("SELECT TOP 1 db.empresaId FROM dbo.DocumentosBase db WHERE db.id = '", idDoc, "';");
                    OdbcDataReader dbReader = odbcCommand.ExecuteReader();
                    while (dbReader.Read())
                    {
                        empresaId = Convert.ToInt32(dbReader["empresaId"]);
                    }
                }
            }
            catch (Exception exception)
            {
            }
            return empresaId;
        }
    }
}
