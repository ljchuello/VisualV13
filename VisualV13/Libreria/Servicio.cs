using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

namespace VisualV13.Libreria
{
    public class Servicio
    {
        public async Task CheckAsync(string id)
        {
            await Task.Run(() => {
                try
                {
                    ServiceController servicioSql = new ServiceController(id);
                    if ((servicioSql.Status != ServiceControllerStatus.Running && servicioSql.Status != ServiceControllerStatus.StartPending))
                    {
                        try
                        {
                            servicioSql.Stop();
                        }
                        catch (Exception exception)
                        {
                        }
                        servicioSql.Start();
                    }
                    Thread.Sleep(10000);
                }
                catch (Exception exception1)
                {
                }
            });
        }

        public int Count()
        {
            int num;
            try
            {
                num = Process.GetProcessesByName("VisualV13").Count<Process>();
            }
            catch (Exception exception)
            {
                num = 100;
            }
            return num;
        }

        public bool ExisteServ(string servicio)
        {
            bool flag;
            try
            {
                List<ServiceController> services = ServiceController.GetServices().ToList<ServiceController>();
                int cant = services.Count<ServiceController>((ServiceController x) => x.ServiceName.ToLower() == servicio.ToLower());
                flag = cant == 1;
            }
            catch (Exception exception)
            {
                flag = false;
            }
            return flag;
        }
    }
}
