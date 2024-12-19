using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WoltersKluwer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Archivos Excel (*.xlsx)|*.xlsx";
                openFileDialog.Title = "Seleccionar archivo Excel";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    inputFilePath.Text = openFileDialog.FileName;
                    LoadDataFromExcelFile(openFileDialog.FileName);
                }
            }
        }

        private void LoadDataFromExcelFile(string inputFilePath)
        {
            try
            {
                var dataTable = new DataTable();
                using (var workbook = new XLWorkbook(inputFilePath))
                {
                    var worksheet = workbook.Worksheets.First();
                    bool firstRow = true;
                    foreach (var row in worksheet.RowsUsed())
                    {
                        if (firstRow)
                        {
                            foreach (var cell in row.Cells())
                            {
                                dataTable.Columns.Add(cell.Value.ToString());
                            }
                            firstRow = false;
                        }
                        else
                        {
                            dataTable.Rows.Add(row.Cells().Select(c => c.Value.ToString()).ToArray());
                        }
                    }
                }
                gridViewClients.DataSource = dataTable;

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar el archivo Excel: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            if(gridViewClients.SelectedRows.Count == 0)
            {
                MessageBox.Show("Por favor, seleccione una fila para procesar.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var selectedRow = gridViewClients.SelectedRows[0];


            if (selectedRow.Cells["Nif"].Value == null ||
                selectedRow.Cells["Nombre comercial de la organización"].Value == null ||
                selectedRow.Cells["Fecha alta"].Value == null ||
                selectedRow.Cells["Código postal"].Value == null ||
                selectedRow.Cells["Cód. provincia fiscal"].Value == null)
            {
                MessageBox.Show("La fila seleccionada contiene datos incompletos. Por favor, revise antes de procesar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var client = new Client
                {
                    Nif = selectedRow?.Cells["Nif"].Value.ToString(),
                    Nombre = selectedRow?.Cells["Nombre comercial de la organización"].Value.ToString(),
                    FechaNacimiento = DateTime.Parse(selectedRow?.Cells["Fecha alta"].Value.ToString()),
                    CodigoPostal = selectedRow?.Cells["Código postal"].Value.ToString(),
                    Provincia = selectedRow?.Cells["Cód. provincia fiscal"].Value.ToString(),
                };
                InsertarCliente(client);

                MessageBox.Show($"Cliente {client.Nombre} procesado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al procesar el cliente: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InsertarCliente(Client cliente)
        {
            return;
        }


    }

    public class Client
    {
        public string Nif { get; set; }
        public string Nombre { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string CodigoPostal { get; set; }
        public string Provincia { get; set; }
    }

}
