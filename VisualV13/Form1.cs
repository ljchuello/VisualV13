using System;
using MetroFramework.Forms;

namespace VisualV13
{
    public partial class Form1 : MetroForm
    {
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
        }

        #region Configuración

        private void btnGuardar_Click(object sender, EventArgs e)
        {

        }

        private void btnRestaurar_Click(object sender, EventArgs e)
        {

        }

        #endregion
    }
}
