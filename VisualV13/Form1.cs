using System;
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
        }

        #region Configuración

        private async void btnGuardar_Click(object sender, EventArgs e)
        {
            await Task.Run(() =>
            {
                btnGuardar.Enabled = false;
                try
                {
                    // Validamos la url
                    if (Cadena.Vacia(txtUrl.Text))
                    {
                        MessageBox.Show("Debe ingresar la url del servidor");
                        return;
                    }

                    // Validamos el usuario
                    if (Cadena.Vacia(txtUsuario.Text))
                    {
                        MessageBox.Show("Debe ingresar el usuario de la página web");
                        return;
                    }

                    // Validamos el contraseña
                    if (Cadena.Vacia(txtContraseña.Text))
                    {
                        MessageBox.Show("Debe ingresar la contraseña del usuario de la página web");
                        return;
                    }

                    // Validamos el sql
                    if (Cadena.Vacia(txtSql.Text))
                    {
                        MessageBox.Show("Debe ingresar el nombre de servicio de SQL SERVER");
                        return;
                    }

                    // Validamosel inicio desion
                    if (!_wsInicioSesion.GetToken(txtUrl.Text, txtUsuario.Text, txtContraseña.Text).Success)
                    {
                        MessageBox.Show("No se ha podido verificar el inicio de sesión");
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
