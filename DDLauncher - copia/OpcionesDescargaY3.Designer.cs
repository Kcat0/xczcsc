namespace Dragones_de_Dojima
{
    partial class OpcionesDescargaY3
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tableLayoutPanel1 = new TableLayoutPanel();
            tableLayoutPanel2 = new TableLayoutPanel();
            subtitulado = new CheckBox();
            label2 = new Label();
            canciones = new CheckBox();
            intro = new CheckBox();
            label3 = new Label();
            panel1 = new Panel();
            cancelar = new Button();
            Aceptar = new Button();
            label4 = new Label();
            label1 = new Label();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 0, 1);
            tableLayoutPanel1.Controls.Add(label1, 0, 0);
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 17.560976F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 82.4390259F));
            tableLayoutPanel1.Size = new Size(387, 202);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 2;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 44.2455254F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55.7544746F));
            tableLayoutPanel2.Controls.Add(subtitulado, 0, 2);
            tableLayoutPanel2.Controls.Add(label2, 1, 0);
            tableLayoutPanel2.Controls.Add(canciones, 0, 0);
            tableLayoutPanel2.Controls.Add(intro, 0, 1);
            tableLayoutPanel2.Controls.Add(label3, 1, 1);
            tableLayoutPanel2.Controls.Add(panel1, 1, 3);
            tableLayoutPanel2.Controls.Add(label4, 1, 2);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(3, 38);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 4;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 56.47059F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 43.52941F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));
            tableLayoutPanel2.Size = new Size(381, 161);
            tableLayoutPanel2.TabIndex = 0;
            // 
            // subtitulado
            // 
            subtitulado.AutoSize = true;
            subtitulado.Dock = DockStyle.Fill;
            subtitulado.Location = new Point(3, 75);
            subtitulado.Name = "subtitulado";
            subtitulado.Size = new Size(162, 38);
            subtitulado.TabIndex = 5;
            subtitulado.Text = "Intro original (subtitulada)";
            subtitulado.UseVisualStyleBackColor = true;
            subtitulado.CheckedChanged += subtitulado_CheckedChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Dock = DockStyle.Fill;
            label2.Location = new Point(171, 0);
            label2.Name = "label2";
            label2.Size = new Size(207, 41);
            label2.TabIndex = 0;
            label2.Text = "Restauración de las canciones de Eikichi Yazawa";
            label2.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // canciones
            // 
            canciones.AutoSize = true;
            canciones.Dock = DockStyle.Fill;
            canciones.Location = new Point(3, 3);
            canciones.Name = "canciones";
            canciones.Size = new Size(162, 35);
            canciones.TabIndex = 1;
            canciones.Text = "Restauración de canciones";
            canciones.UseVisualStyleBackColor = true;
            // 
            // intro
            // 
            intro.AutoSize = true;
            intro.Dock = DockStyle.Fill;
            intro.Location = new Point(3, 44);
            intro.Name = "intro";
            intro.Size = new Size(162, 25);
            intro.TabIndex = 2;
            intro.Text = "Intro original";
            intro.UseVisualStyleBackColor = true;
            intro.CheckedChanged += intro_CheckedChanged;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Dock = DockStyle.Fill;
            label3.Location = new Point(171, 41);
            label3.Name = "label3";
            label3.Size = new Size(207, 31);
            label3.TabIndex = 3;
            label3.Text = "Canción original para la intro de Yakuza 3";
            label3.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // panel1
            // 
            panel1.Controls.Add(cancelar);
            panel1.Controls.Add(Aceptar);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(171, 119);
            panel1.Name = "panel1";
            panel1.Size = new Size(207, 39);
            panel1.TabIndex = 7;
            // 
            // cancelar
            // 
            cancelar.Location = new Point(50, 10);
            cancelar.Name = "cancelar";
            cancelar.Size = new Size(75, 23);
            cancelar.TabIndex = 7;
            cancelar.Text = "Cancelar";
            cancelar.UseVisualStyleBackColor = true;
            cancelar.Click += btnCancelar_Click;
            // 
            // Aceptar
            // 
            Aceptar.Location = new Point(131, 10);
            Aceptar.Name = "Aceptar";
            Aceptar.Size = new Size(75, 23);
            Aceptar.TabIndex = 6;
            Aceptar.Text = "Aceptar";
            Aceptar.UseVisualStyleBackColor = true;
            Aceptar.Click += btnAceptar_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Dock = DockStyle.Fill;
            label4.Location = new Point(171, 72);
            label4.Name = "label4";
            label4.Size = new Size(207, 44);
            label4.TabIndex = 8;
            label4.Text = "Subtítulado de \"Loser\" de Eikichi Yazawa en la intro";
            label4.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Dock = DockStyle.Fill;
            label1.Font = new Font("Bahnschrift", 14.25F, FontStyle.Regular, GraphicsUnit.Point);
            label1.Location = new Point(3, 0);
            label1.Name = "label1";
            label1.Size = new Size(381, 35);
            label1.TabIndex = 1;
            label1.Text = "Opciones";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // OpcionesDescargaY3
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(385, 203);
            Controls.Add(tableLayoutPanel1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Name = "OpcionesDescargaY3";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Descargas adicionales";
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            panel1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private TableLayoutPanel tableLayoutPanel2;
        private Label label1;
        private Label label2;
        private CheckBox canciones;
        private CheckBox intro;
        private Label label3;
        private CheckBox subtitulado;
        private Panel panel1;
        private Button cancelar;
        private Button Aceptar;
        private Label label4;
    }
}