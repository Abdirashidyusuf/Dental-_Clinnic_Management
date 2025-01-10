using Dental_Management.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dental_Management
{
    public partial class ReceiptsForm : Form
    {
        public ReceiptsForm()
        {
            InitializeComponent();
        }

        private void ReceiptsForm_Load(object sender, EventArgs e)
        {
            GenerateReceiptNo();  // Automatically generate Receipt No on load
            LoadReceipts();  // Load existing receipt records
        }

        // Generate Receipt No automatically
        public void GenerateReceiptNo()
        {
            SqlConnection conn = DatabaseConnection.GetConnection();
            try
            {
                SqlCommand cmd = new SqlCommand("SELECT MAX(rn_no) FROM receipts", conn);
                conn.Open();
                var result = cmd.ExecuteScalar();
                int newId = (result != DBNull.Value) ? Convert.ToInt32(result) + 1 : 1; // Start at 1 if no records exist
                txtReceiptNo.Text = newId.ToString();  // Set the generated Receipt No in the textbox
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                if (conn.State == System.Data.ConnectionState.Open)
                    conn.Close();
            }
        }

        // Insert a new receipt into the database
        private void btnAddReceipt_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtReceiptNo.Text) &&
                !string.IsNullOrWhiteSpace(txtPeopleNo.Text) &&
                !string.IsNullOrWhiteSpace(txtAccountNo.Text) &&
                !string.IsNullOrWhiteSpace(txtAmount.Text) &&
                !string.IsNullOrWhiteSpace(txtReceiptDate.Text) &&
                !string.IsNullOrWhiteSpace(txtReceiptDescription.Text))
            {
                AddReceipt(txtReceiptNo.Text, txtPeopleNo.Text, txtAccountNo.Text, Convert.ToDecimal(txtAmount.Text), DateTime.Parse(txtReceiptDate.Text), txtReceiptDescription.Text);
                ClearReceiptTextboxes();  // Clear textboxes after adding the receipt
                GenerateReceiptNo();  // Automatically generate a new Receipt No for the next receipt
            }
            else
            {
                MessageBox.Show("Please enter valid data for all fields.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Insert receipt into the database
        public void AddReceipt(string rn_no, string p_no, string acc_no, decimal amount, DateTime rec_date, string description)
        {
            SqlConnection conn = DatabaseConnection.GetConnection();
            try
            {
                SqlCommand cmd = new SqlCommand("INSERT INTO receipts (rn_no, p_no, acc_no, amount, rec_date, description) VALUES (@rn_no, @p_no, @acc_no, @amount, @rec_date, @description)", conn);
                cmd.Parameters.AddWithValue("@rn_no", rn_no);
                cmd.Parameters.AddWithValue("@p_no", p_no);
                cmd.Parameters.AddWithValue("@acc_no", acc_no);
                cmd.Parameters.AddWithValue("@amount", amount);
                cmd.Parameters.AddWithValue("@rec_date", rec_date);
                cmd.Parameters.AddWithValue("@description", description);
                conn.Open();
                cmd.ExecuteNonQuery();
                MessageBox.Show("Receipt added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Refresh the DataGridView
                LoadReceipts();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                MessageBox.Show("An error occurred while adding the receipt. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (conn.State == System.Data.ConnectionState.Open)
                    conn.Close();
            }
        }

        // Refresh the DataGridView with the latest data from the database
        private void LoadReceipts()
        {
            SqlConnection conn = DatabaseConnection.GetConnection();
            try
            {
                SqlCommand cmd = new SqlCommand("SELECT rn_no, p_no, acc_no, amount, rec_date, description FROM receipts", conn);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);

                // Assuming your DataGridView is named "dataGridViewReceipt"
                dataGridViewReceipt.DataSource = dataTable;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                if (conn.State == System.Data.ConnectionState.Open)
                    conn.Close();
            }
        }

        // Update receipt information
        private void btnEditReceipt_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtReceiptNo.Text, out int rn_no))
            {
                UpdateReceipt(rn_no);
            }
            else
            {
                MessageBox.Show("Please enter a valid Receipt Number to update.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Update receipt details in the database
        public void UpdateReceipt(int rn_no)
        {
            string p_no = txtPeopleNo.Text;
            string acc_no = txtAccountNo.Text;
            decimal amount = Convert.ToDecimal(txtAmount.Text);
            DateTime rec_date = DateTime.Parse(txtReceiptDate.Text);
            string description = txtReceiptDescription.Text;

            SqlConnection conn = DatabaseConnection.GetConnection();
            try
            {
                SqlCommand cmd = new SqlCommand("UPDATE receipts SET p_no=@p_no, acc_no=@acc_no, amount=@amount, rec_date=@rec_date, description=@description WHERE rn_no=@rn_no", conn);
                cmd.Parameters.AddWithValue("@rn_no", rn_no);
                cmd.Parameters.AddWithValue("@p_no", p_no);
                cmd.Parameters.AddWithValue("@acc_no", acc_no);
                cmd.Parameters.AddWithValue("@amount", amount);
                cmd.Parameters.AddWithValue("@rec_date", rec_date);
                cmd.Parameters.AddWithValue("@description", description);
                conn.Open();
                cmd.ExecuteNonQuery();

                // Refresh the DataGridView to show updated data
                LoadReceipts();
                MessageBox.Show("Receipt updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearReceiptTextboxes();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                if (conn.State == System.Data.ConnectionState.Open)
                    conn.Close();
            }
        }

        // Delete a receipt
        private void btnDeleteReceipt_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtReceiptNo.Text, out int rn_no))
            {
                DeleteReceipt(rn_no);
            }
            else
            {
                MessageBox.Show("Please enter a valid Receipt Number to delete.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Delete receipt from the database
        public void DeleteReceipt(int rn_no)
        {
            SqlConnection conn = DatabaseConnection.GetConnection();
            try
            {
                SqlCommand cmd = new SqlCommand("DELETE FROM receipts WHERE rn_no=@rn_no", conn);
                cmd.Parameters.AddWithValue("@rn_no", rn_no);
                conn.Open();
                cmd.ExecuteNonQuery();
                MessageBox.Show("Receipt deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Refresh the DataGridView after deleting
                LoadReceipts();
                ClearReceiptTextboxes();  // Clear textboxes after deleting
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                MessageBox.Show("An error occurred while deleting the receipt. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (conn.State == System.Data.ConnectionState.Open)
                    conn.Close();
            }
        }

        // Clear all receipt textboxes after adding, updating, or deleting
        public void ClearReceiptTextboxes()
        {
            txtReceiptNo.Clear();
            txtPeopleNo.Clear();
            txtAccountNo.Clear();
            txtAmount.Clear();
            txtReceiptDate.Clear();
            txtReceiptDescription.Clear();
        }

        // Method to handle the cell click in DataGridView for receipts
        private void dataGridViewReceipt_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridViewReceipt.Rows[e.RowIndex];
                txtReceiptNo.Text = row.Cells["rn_no"].Value.ToString();
                txtPeopleNo.Text = row.Cells["p_no"].Value.ToString();
                txtAccountNo.Text = row.Cells["acc_no"].Value.ToString();
                txtAmount.Text = row.Cells["amount"].Value.ToString();
                txtReceiptDate.Text = row.Cells["rec_date"].Value.ToString();
                txtReceiptDescription.Text = row.Cells["description"].Value.ToString();
            }
        }

        private void gunaPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void lbDashboard_Click(object sender, EventArgs e)
        {
            this.Hide();
            DahsboardForm dashboard = new DahsboardForm();
            dashboard.Show();
        }

        private void lbServices_Click(object sender, EventArgs e)
        {
            this.Hide(); // Hide the DashboardForm

            // Open the Service Form
            ServicesForm serviceForm = new ServicesForm();
            serviceForm.Show();
        }

        private void lbPeople_Click(object sender, EventArgs e)
        {
            this.Hide(); //hide dashboard

            //open the people form
            PeopleForm peopleForm = new PeopleForm();
            peopleForm.Show();
        }

        private void lbVisits_Click(object sender, EventArgs e)
        {
            this.Hide(); // Hide the DashboardForm

            // Open the vitis Form
            VisitsForm visitForm = new VisitsForm();
            visitForm.Show();
        }

        private void lbOperations_Click(object sender, EventArgs e)
        {
            this.Hide(); // Hide the DashboardForm

            // Open the operations Form
            OperationsForm operationForm = new OperationsForm();
            operationForm.Show();
        }

        private void lbPrescriptions_Click(object sender, EventArgs e)
        {
            this.Hide(); // Hide the DashboardForm

            // Open the prescrption Form
            PrescriptionsForm prescriptionForm = new PrescriptionsForm();
            prescriptionForm.Show();
        }

        private void lbCustomerPrescription_Click(object sender, EventArgs e)
        {
            this.Hide(); // Hide the DashboardForm

            // Open the customer prescription Form
            CustomerPrescriptionForm customerPrescriptionForm = new CustomerPrescriptionForm();
            customerPrescriptionForm.Show();
        }

        private void lbAmount_Click(object sender, EventArgs e)
        {
            this.Hide(); // Hide the DashboardForm

            // Open the amount Form
            AmountsForm amountForm = new AmountsForm();
            amountForm.Show();
        }

        private void lbBills_Click(object sender, EventArgs e)
        {
            this.Hide(); // Hide the DashboardForm

            // Open the bills Form
            BillsForm billForm = new BillsForm();
            billForm.Show();
        }

        private void lbAcounts_Click(object sender, EventArgs e)
        {
            this.Hide(); // Hide the DashboardForm

            // Open the account Form
            AccountsForm accountForm = new AccountsForm();
            accountForm.Show();
        }

        private void lbUsers_Click(object sender, EventArgs e)
        {
            this.Hide(); // Hide the DashboardForm

            // Open the users Form
            UsersForm userForm = new UsersForm();
            userForm.Show();
        }
    }
}
