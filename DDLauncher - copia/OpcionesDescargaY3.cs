using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Dragones_de_Dojima
{
    public partial class OpcionesDescargaY3 : Form
    {
        public List<string> OpcionesSeleccionadas { get; private set; }

        public OpcionesDescargaY3()
        {
            InitializeComponent();
            OpcionesSeleccionadas = new List<string>();
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (canciones.Checked)
            {
                OpcionesSeleccionadas.Add("omoiga_afuretara");
                OpcionesSeleccionadas.Add("cinematica11");
                OpcionesSeleccionadas.Add("amb_donki");
                OpcionesSeleccionadas.Add("amb_donki_c");
                OpcionesSeleccionadas.Add("amb_donki_e");
                OpcionesSeleccionadas.Add("amb_donki_k");
                OpcionesSeleccionadas.Add("haichi");
            }
            if (intro.Checked)
            {
                OpcionesSeleccionadas.Add("intro");
            }
            if (subtitulado.Checked)
            {
                OpcionesSeleccionadas.Add("intro_sub");
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void intro_CheckedChanged(object sender, EventArgs e)
        {
            if (intro.Checked)
            {
                subtitulado.Checked = false;
                subtitulado.Enabled = false;
            }
            else
            {
                subtitulado.Enabled = true;
            }
        }

        private void subtitulado_CheckedChanged(object sender, EventArgs e)
        {
            if (subtitulado.Checked)
            {
                intro.Checked = false;
                intro.Enabled = false;
            }
            else
            {
                intro.Enabled = true;
            }
        }
    }
}
