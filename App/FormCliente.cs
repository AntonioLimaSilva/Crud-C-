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
    public partial class FormCliente : Form
    {
        Cliente cliente = new Cliente();

        public FormCliente()
        {
            InitializeComponent();
            initialize();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if(txtName.Text.Trim() == string.Empty)
            {
                MessageBox.Show("Nome é obrigatório");
                txtName.Focus();
                return;
            }

            if(txtAddress.Text.Trim() == string.Empty)
            {
                MessageBox.Show("Logradouro é obrigatório");
                txtAddress.Focus();
                return;
            }

            cliente.nome = txtName.Text;
            cliente.logradouro = txtAddress.Text;
            cliente.id_grupo_economico = Convert.ToInt32(cbBoxGroups.SelectedValue);

            using(var db = new DBEntities())
            {
                if (cliente.id == 0)
                    db.Clientes.Add(cliente);
                else
                    db.Entry(cliente).State = EntityState.Modified;
                db.SaveChanges();
            }

            initialize();
            clear();

            MessageBox.Show("Cliente salvo com sucesso!");
        }

        private void initialize()
        {
            using(var db = new DBEntities())
            {
                cbBoxGroups.DataSource = db.GrupoEconomicoes.ToList();
                cbBoxGroups.DisplayMember = "descricao";
                cbBoxGroups.ValueMember = "id";

                dataGridView.AutoGenerateColumns = false;
                dataGridView.DataSource = db.Clientes.ToList();
            }

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            clear();
        }

        private void clear()
        {
            txtName.Text = txtAddress.Text = "";
            btnSave.Text = "Salvar";
            btnRemove.Enabled = false;
        }

        private void dataGridView_DoubleClick(object sender, EventArgs e)
        {
            if(dataGridView.CurrentRow.Index != -1)
            {
                cliente.id = Convert.ToInt32(dataGridView.CurrentRow.Cells["id"].Value);

                using(var db = new DBEntities())
                {
                    cliente = db.Clientes.Where(c => c.id == cliente.id).FirstOrDefault();
                }

                txtName.Text = cliente.nome;
                txtAddress.Text = cliente.logradouro;
                cbBoxGroups.SelectedValue = cliente.id_grupo_economico;

                btnSave.Text = "Atualizar";
                btnRemove.Enabled = true;
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("Tem certeza que deseja excluir esse item?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                using(var db = new DBEntities())
                {
                    var entity = db.Entry(cliente).State;

                    if(entity == EntityState.Detached)
                    {
                        db.Clientes.Attach(cliente);
                    }

                    db.Clientes.Remove(cliente);
                    db.SaveChanges();
                }

                initialize();
                clear();

                MessageBox.Show("Cliente exxluído com sucesso!");
            }
        }
    }
}
