using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework.Forms;
using VisualV13.Libreria;

namespace VisualV13
{
    public partial class Form1 : MetroForm
    {
        private Configuracion _configuracion = new Configuracion();
        private Ws_InicioSesion _wsInicioSesion = new Ws_InicioSesion();
        private Ws_Documentos _wsDocumentos = new Ws_Documentos();
        private Odbc _odbc = new Odbc();

        private bool _trabajando = true;

        public Form1()
        {
            InitializeComponent();

            // Seteamos nos paramos en el tab 1
            metroTabControl1.TabPages.Clear();
            metroTabControl1.TabPages.Add(metroTabPage1);
            metroTabControl1.TabPages.Add(metroTabPage2);

            // Deshabilitamos el error multiTask
            CheckForIllegalCrossThreadCalls = false;

            // Ocultamos los botones
            MaximizeBox = false;

            // No ReSize
            Resizable = false;

            // Cargamos
            _configuracion = _configuracion.Leer();
            txtUrl.Text = _configuracion.Url;
            txtUsuario.Text = _configuracion.Usuario;
            txtContraseña.Text = _configuracion.Contrasenia;
            txtSql.Text = _configuracion.MsSqlServer;

            //Text
            Text = "Visual V13";
            metroButton1_Click(null, null);

            // Ejecutamos
            _ = Trabajo_Estadísticas_Email();
            _ = Trabajo_Estadisticas_Comprobantes();
            _ = Trabajo_Autorizar();
        }

        #region Trabajación

        private void metroButton1_Click(object sender, EventArgs e)
        {
            if (_trabajando == false)
            {
                _trabajando = true;
                lblEstado.Text = "Estado: Iniciado";
                lblFechaInicio.Text = $"Fecha inicio: {DateTime.Now:dd/MM/yyyy HH:mm:ss}";
                //lblEmailsSinEnviar.Text = $"Emails sin enviar: N/D";
            }
            else
            {
                lblEstado.Text = "Estado: Detenido";
                lblFechaInicio.Text = "Fecha inicio:";
                //lblEmailsSinEnviar.Text = "Emails sin enviar:";
                _trabajando = false;
            }
        }

        /// <summary>
        /// Muestra la estadistica de los correos
        /// </summary>
        /// <returns></returns>
        async Task Trabajo_Estadísticas_Email()
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        // Obtenemos los correos no enviados
                        int noEmail = _odbc.Select_Count_NoMail();

                        // Set
                        lblEmailsSinEnviar.Text = $"Emails sin enviar: {noEmail:n0}";
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                    finally
                    {
                        // Esperamos 1 seg
                        Thread.Sleep(1000);
                    }
                }
            });
        }

        /// <summary>
        ///  Muestra las estadisticas de los documentos
        /// </summary>
        /// <returns></returns>
        async Task Trabajo_Estadisticas_Comprobantes()
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        lblDocList.Text = _odbc.Select_List_Comprobante();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                    finally
                    {
                        // Esperamos 1 seg
                        Thread.Sleep(1000);
                    }
                }
            });
        }

        /// <summary>
        /// Autoriza los comprobantes
        /// </summary>
        /// <returns></returns>
        async Task Trabajo_Autorizar()
        {
            await Task.Run(async () =>
            {
                while (true)
                {
                    if (_trabajando)
                    {
                        try
                        {
                            // Validamos si hay documentos por autorizar
                            string id = _odbc.Select_Get_NoAutoriado();

                            // Validamos
                            if (!Cadena.Vacia(id))
                            {
                                // Iniciamos sesion
                                oResultado oResultado = _wsInicioSesion.GetToken(txtUrl.Text, txtUsuario.Text, txtContraseña.Text);

                                // Tokens
                                Dictionary<string, string> token = oResultado.Resultado;

                                // Mandamos a autorizar
                                await _wsDocumentos.Autorizar(id, token, txtUrl.Text);
                            }

                            // No hay nada que autorizar esperamos
                            Thread.Sleep(1000);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }
                    else
                    {
                        // Esperamos
                        Thread.Sleep(1000);
                    }
                }
            });
        }

        #endregion

        #region Configuración

        bool Validar()
        {
            try
            {
                // Validamos la url
                if (Cadena.Vacia(txtUrl.Text))
                {
                    MessageBox.Show("Debe ingresar la url del servidor");
                    return false;
                }

                // Validamos el usuario
                if (Cadena.Vacia(txtUsuario.Text))
                {
                    MessageBox.Show("Debe ingresar el usuario de la página web");
                    return false;
                }

                // Validamos el contraseña
                if (Cadena.Vacia(txtContraseña.Text))
                {
                    MessageBox.Show("Debe ingresar la contraseña del usuario de la página web");
                    return false;
                }

                // Validamos el sql
                if (Cadena.Vacia(txtSql.Text))
                {
                    MessageBox.Show("Debe ingresar el nombre de servicio de SQL SERVER");
                    return false;
                }

                // Validamosel inicio desion
                if (!_wsInicioSesion.GetToken(txtUrl.Text, txtUsuario.Text, txtContraseña.Text).Success)
                {
                    MessageBox.Show("No se ha podido verificar el inicio de sesión");
                    return false;
                }

                // Libre de pecados
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        private async void btnGuardar_Click(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                btnGuardar.Enabled = false;
                try
                {
                    // Validamos
                    if (!Validar())
                    {
                        return;
                    }

                    // Guardamos
                    _configuracion = new Configuracion();
                    _configuracion.Url = txtUrl.Text;
                    _configuracion.Usuario = txtUsuario.Text;
                    _configuracion.Contrasenia = txtContraseña.Text;
                    _configuracion.MsSqlServer = txtSql.Text;
                    if (!_configuracion.Guardar(_configuracion))
                    {
                        MessageBox.Show("No se ha podido guardar la configuración");
                        return;
                    }

                    // Libre de pecados
                    MessageBox.Show("Se ha guardado la configuración con éxito");
                    return;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ah ocurrido un error; {ex.Message}");
                }
                finally
                {
                    btnGuardar.Enabled = true;
                }
            });
        }

        private void btnRestaurar_Click(object sender, EventArgs e)
        {
            try
            {
                // Cargamos
                _configuracion = _configuracion.Leer();
                txtUrl.Text = _configuracion.Url;
                txtUsuario.Text = _configuracion.Usuario;
                txtContraseña.Text = _configuracion.Contrasenia;
                txtSql.Text = _configuracion.MsSqlServer;

                MessageBox.Show("Se ha restaurado la configuración");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                MessageBox.Show("No se ha podido restaurar la configuración");
            }
        }

        #endregion
    }
}
