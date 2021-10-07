using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Threading.Tasks;
using RestSharp;

namespace VisualV13.Libreria
{
    public class Ws_Documentos
    {
        private readonly Odbc _odbc = new Odbc();

        public Task Autorizar(string idDocument, Dictionary<string, string> token, string url)
        {
            Task task = Task.Run(async () =>
            {
                // Obtenemos el id de la empresa
                int num = _odbc.Select_empresaId(idDocument);

                // Hacemos el cambio de empresa
                await AutorizarDoc_CambioEmp(url, token, num);

                // Enviamos
                await AutorizarDoc_Enviar(idDocument, url, token);

                // Recibimos
                await AutorizarDoc_Recibir(idDocument, url, token);
            });
            return task;
        }

        public Task AutorizarDoc_CambioEmp(string url, Dictionary<string, string> token, int empresaId)
        {
            Task task = Task.Run(() =>
            {
                RestClient restClient = new RestClient($"{url}EmpresaAdministracion/CambiarEmpresa?empresa_id={empresaId}")
                {
                    Timeout = 90000
                };
                RestRequest restRequest = new RestRequest(Method.GET);
                foreach (KeyValuePair<string, string> row in token)
                {
                    restRequest.AddParameter(row.Key, row.Value, ParameterType.Cookie);
                }
                restClient.Execute(restRequest);
            });
            return task;
        }

        private Task AutorizarDoc_Enviar(string idDoc, string url, Dictionary<string, string> token)
        {
            Task task = Task.Run(() =>
            {
                RestClient restClient = new RestClient(string.Concat(url, "DocumentoEmitido/Enviar"))
                {
                    Timeout = 90000
                };
                RestRequest request = new RestRequest(Method.POST);
                request.AddParameter("fecha_desde", "01/01/2017");
                request.AddParameter("fecha_hasta", "01/01/2017");
                request.AddParameter("data_table_length", "10");
                request.AddParameter("doc_seleccionados", idDoc);
                foreach (KeyValuePair<string, string> row in token)
                {
                    request.AddParameter(row.Key, row.Value, ParameterType.Cookie);
                }
                restClient.Execute(request);
            });
            return task;
        }

        private Task AutorizarDoc_Recibir(string idDoc, string url, Dictionary<string, string> token)
        {
            Task task = Task.Run(() =>
            {
                RestClient restClient = new RestClient(string.Concat(url, "DocumentoEmitido/ActualizarEstado?id=", idDoc, "&fecha_desde=2017/01/01&fecha_hasta=2017/01/01"))
                {
                    Timeout = 90000
                };
                RestRequest restRequest = new RestRequest(Method.GET);
                foreach (KeyValuePair<string, string> row in token)
                {
                    restRequest.AddParameter(row.Key, row.Value, ParameterType.Cookie);
                }
                restClient.Execute(restRequest);
            });
            return task;
        }
    }
}
