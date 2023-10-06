﻿using CapaVista.Componentes.Utilidades;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CapaVista
{
    public partial class Navegador : UserControl
    {
        private utilidadesConsultasI utilConsultasI;
        public string operacion = "";
        public string tabla = "";
        public int filaActual = 0;
        public bool gridExiste = false;

        public Form parent;
        public Navegador()
        {
            InitializeComponent();
            this.parent = new Form();
            this.utilConsultasI = new utilidadesConsultasI();
            this.cambiarEstado(false);
        }

        public void config(string tabla, Form parent)
        {
            this.tabla = tabla;
            this.parent = parent;
            this.utilConsultasI.setTabla(this.tabla);
            DataGridView gd = GetDGV(this.parent);

            if (gd == null) return;
            _config_grid(gd);
            gridExiste = true;
            gd.CellClick += this.data_Click;
            this.utilConsultasI.refrescar(this.parent);
            this.cambiarEstado(false);
        }

        void _config_grid(DataGridView gd)
        {
            gd.ReadOnly = true;
            gd.RowHeadersVisible = false;
            gd.RowHeadersWidth = 51;
            gd.RowTemplate.Height = 24;
            gd.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
        }

        void verificarDG()
        {
            if (!gridExiste)
            {

                return;
            }

        }

        private void data_Click(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dt = sender as DataGridView;
            if (dt.SelectedRows.Count > 0)
            {
                filaActual = dt.SelectedRows[0].Index;
                focusData(dt);
            }
        }



        public void identificarFormulario(Form child, string operacion)
        {
            DataGridView dgvname = GetDGV(child);

            if (operacion.Equals("g")) this.utilConsultasI.guardar(child);
            if (operacion.Equals("m")) this.utilConsultasI.modificar(child);
            if (operacion.Equals("r")) this.utilConsultasI.refrescar(child);
            if (operacion.Equals("e")) this.utilConsultasI.eliminar(child, dgvname);
        }



        public DataGridView GetDGV(Form child)
        {
            foreach (Control c in child.Controls)
            {
                if (c is DataGridView dgv)
                {
                    return dgv;
                }
            }
            return null;
            throw new Exception("No se encontró un DataGridView en el formulario.");
        }
        private void btn_guardar_Click(object sender, EventArgs e)
        {

            this.identificarFormulario(this.parent, this.operacion);
            this.cambiarEstado(false);
        }

        public void cambiarEstado(bool estado)
        {
            foreach (Control control in this.panel.Controls)
            {
                if (control is Button)
                {
                    Button btn = (Button)control;
                    if (btn.Name.Equals("btn_guardar") || btn.Name.Equals("btn_cancelar"))
                    {
                        btn.Enabled = estado;
                    }
                    else
                    {
                        btn.Enabled = !estado;
                    }
                }
            }

            foreach (Control c in this.parent.Controls)
            {
                if (c is TextBox)
                {
                    TextBox txt = (TextBox)c;
                    txt.ReadOnly = !estado;
                }
                else if (c is DateTimePicker)
                {
                    DateTimePicker dtp = (DateTimePicker)c;
                    dtp.Enabled = estado;
                }
            }

        }

        private void btn_agregar_Click(object sender, EventArgs e)
        {
            this.cambiarEstado(true);
            this.operacion = "g";
        }

        public void limpiarControles()
        {
            foreach (Control control in this.parent.Controls)
            {
                if (control is TextBox)
                {
                    ((TextBox)control).Clear();
                }
                else if (control is DateTimePicker)
                {
                    ((DateTimePicker)control).Value = DateTime.Now;
                }
                else if (control is ComboBox)
                {
                    ((ComboBox)control).SelectedIndex = 0;
                }
            }
        }

        private void btn_cancelar_Click(object sender, EventArgs e)
        {
            this.limpiarControles();
            this.cambiarEstado(false);
        }

        private void btn_ayuda_Click_1(object sender, EventArgs e)
        {
            Help.ShowHelp(this, "Ayudas/AyudaSO2.chm", "NavAyuda.html");
        }

        private void btn_refrescar_Click(object sender, EventArgs e)
        {
            this.identificarFormulario(this.parent, "r");
        }

        private void btn_modificar_Click(object sender, EventArgs e)
        {

            try
            {
                MessageBox.Show(" Modificar");
                this.utilConsultasI.cargarModificar(this.parent, GetDGV(this.parent));
                this.operacion = "m";
                this.cambiarEstado(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                MessageBox.Show("Error" + ex);
            }



        }



        public void focusData(DataGridView gd)
        {
            Dictionary<string, string> rowData = new Dictionary<string, string>();
            DataGridViewRow selectedRow = gd.SelectedRows[0];
            foreach (DataGridViewColumn column in gd.Columns)
            {
                string columnName = column.Name;
                object cellValue = selectedRow.Cells[columnName].Value;
                rowData[columnName] = cellValue.ToString();
            }
            foreach (Control c in this.parent.Controls)
            {
                if (c is TextBox)
                {
                    TextBox txt = (TextBox)c;
                    if (rowData.ContainsKey(txt.Tag.ToString()))
                    {
                        txt.Text = rowData[txt.Tag.ToString()];
                    }
                }
                else if (c is DateTimePicker)
                {
                    DateTimePicker dt = (DateTimePicker)c;
                    if (rowData.ContainsKey(dt.Tag.ToString()))
                    {
                        DateTime date;
                        string _date_str = rowData[dt.Tag.ToString()];
                        if (DateTime.TryParse(_date_str, out date))
                        {
                            dt.Value = date;
                        }
                    }
                }
            }
        }


        private void btn_anterior_Click(object sender, EventArgs e)
        {

            verificarDG();

            DataGridView gd = GetDGV(this.parent);
            gd.ClearSelection();
            if (filaActual > 0)
            {
                filaActual--;
                gd.Rows[filaActual].Selected = true;
                focusData(gd);
            }
            else if (filaActual <= 0)
            {
                MessageBox.Show("No hay filas anteriores para seleccionar la anterior.");

            }

        }

        private void btn_siguiente_Click(object sender, EventArgs e)
        {
            verificarDG();


            DataGridView gd = GetDGV(this.parent);
            gd.ClearSelection();
            if (filaActual < gd.Rows.Count - 1)
            {
                filaActual++;
                gd.Rows[filaActual].Selected = true;
                focusData(gd);
            }
            else
            {
                MessageBox.Show("No hay filas posteriores para seleccionar la siguiente.");
            }



        }

        private void btn_inicio_Click(object sender, EventArgs e)
        {
            verificarDG();

            filaActual = 0;
            DataGridView gd = GetDGV(this.parent);
            gd.ClearSelection();
            gd.Rows[0].Selected = true;
            gd.FirstDisplayedScrollingRowIndex = 0;
            focusData(gd);
        }

        private void btn_fin_Click(object sender, EventArgs e)
        {
            verificarDG();
            DataGridView gd = GetDGV(this.parent);
            gd.ClearSelection();
            gd.Rows[gd.Rows.Count - 1].Selected = true;
            gd.FirstDisplayedScrollingRowIndex = gd.Rows.Count - 1;
            filaActual = gd.Rows.Count - 1;
            focusData(gd);
        }

        //Carol Chuy
        private void btn_eliminar_Click(object sender, EventArgs e)
        {
            this.identificarFormulario(this.parent, "e");
        }
    }
}
