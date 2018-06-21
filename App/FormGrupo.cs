using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Entity;

namespace App
{
    public partial class FormGrupo : Form
    {
        GrupoEconomico grupo = new GrupoEconomico();

        public FormGrupo()
        {
            InitializeComponent();
            initialize();
        }

        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            if(dataGridView1.CurrentRow.Index != -1)
            {
                grupo.id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["id"].Value);

                using(var db = new DBEntities())
                {
                    grupo = db.GrupoEconomicoes.Where(g => g.id == grupo.id).FirstOrDefault();
                }

                txtDescription.Text = grupo.descricao;
                txtValue.Text = grupo.faixa_valor.ToString();

                btnSave.Text = "Atualizar";
                btnRemove.Enabled = true;
            }
        }


        private void btnSave_Click(object sender, EventArgs e)
        {

            if ((txtDescription.Text.Trim() == string.Empty) || (txtDescription.Text.Length > 100))
            {
                MessageBox.Show("Descrição é obrigatória");
                txtDescription.Focus();
                return;
            }

            if((txtValue.Text.Trim() == string.Empty) || (txtValue.Text.Length > 5))
            {
                MessageBox.Show("Valor é obrigatória");
                txtValue.Focus();
                return;
            }

            string str = txtValue.Text.Trim();
            double number;
            bool isNumber = double.TryParse(str, out number);

            if (!isNumber)
            {
                MessageBox.Show("Número inválido");
                return;
            }

            grupo.descricao = txtDescription.Text;
            grupo.faixa_valor = Convert.ToInt32(txtValue.Text);

            using(var db = new DBEntities())
            {
                if (grupo.id == 0)
                    db.GrupoEconomicoes.Add(grupo);
                else
                    db.Entry(grupo).State = EntityState.Modified;
                db.SaveChanges();
            }

            MessageBox.Show("Grupo salvo com sucesso!");

            initialize();
            clear();
        }

        private void txtDescription_Validating(object sender, CancelEventArgs e)
        {
          
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            clear();
        }

        private void initialize()
        {
            using(var db = new DBEntities())
            {
                dataGridView1.AutoGenerateColumns = false;
                dataGridView1.DataSource = db.GrupoEconomicoes.ToList();
            }
        }

        private void clear()
        {
            txtDescription.Text = txtValue.Text = "";
            btnSave.Text = "Salvar";
            btnRemove.Enabled = false;
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("Tem certeza que deseja excluir o item", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                using(var db = new DBEntities())
                {
                    var entity = db.Entry(grupo);

                    if(entity.State == EntityState.Detached)
                    {
                        db.GrupoEconomicoes.Attach(grupo);
                    }

                    db.GrupoEconomicoes.Remove(grupo);
                    db.SaveChanges();
                }

                initialize();
                clear();

                MessageBox.Show("Item excluído com sucesso");
            }
        }

        private void txtValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
        }
    }
}
