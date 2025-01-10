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
    public partial class AmountsForm : Form
    {
        public AmountsForm()
        {
            InitializeComponent();
        }

        private void AmountsForm_Load(object sender, EventArgs e)
        {
            GenerateAmountNo(); // Automatically generate Amount No on load
            LoadAmount();       // Load existing amount records
        }

        // Generate Amount No automatically
        public void GenerateAmountNo()
        {
            SqlConnection conn = DatabaseConnection.GetConnection();
            try
            {
                SqlCommand cmd = new SqlCommand("SELECT MAX(am_no) FROM amount", conn);
                conn.Open();
                var result = cmd.ExecuteScalar();
                int newId = (result != DBNull.Value) ? Convert.ToInt32(result) + 1 : 1; // Start at 1 if no records exist
                txtAmountNo.Text = newId.ToString(); // Set the generated ID in the textbox
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

        // Insert a new amount into the database
        private void btnAddAmount_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtAmountName.Text))
            {
                AddAmount(txtAmountNo.Text, txtAmountName.Text);
                ClearTextboxes();  // Clear textboxes after adding the amount
                GenerateAmountNo(); // Automatically generate a new ID for the next amount
            }
            else
            {
                MessageBox.Show("Please enter a valid amount name.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Insert amount into the database
        public void AddAmount(string am_no, string amount_name)
        {
            SqlConnection conn = DatabaseConnection.GetConnection();
            try
            {
                SqlCommand cmd = new SqlCommand("INSERT INTO amount (am_no, amount_name) VALUES (@am_no, @amount_name)", conn);
                cmd.Parameters.AddWithValue("@am_no", am_no);
                cmd.Parameters.AddWithValue("@amount_name", amount_name);
                conn.Open();
                cmd.ExecuteNonQuery();
                MessageBox.Show("Amount added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Refresh the DataGridView
                LoadAmount();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                MessageBox.Show("An error occurred while adding the amount. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (conn.State == System.Data.ConnectionState.Open)
                    conn.Close();
            }
        }

        // Refresh the DataGridView with the latest data from the database
        private void LoadAmount()
        {
            SqlConnection conn = DatabaseConnection.GetConnection();
            try
            {
                SqlCommand cmd = new SqlCommand("SELECT am_no, amount_name FROM amount", conn);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);

                dataGridViewAmount.DataSource = dataTable;
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

        // Update amount information
        private void btnEditAmount_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtAmountNo.Text, out int am_no))
            {
                UpdateAmount(am_no, txtAmountName.Text);
            }
            else
            {
                MessageBox.Show("Please enter a valid Amount Number to update.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Update amount details in the database
        public void UpdateAmount(int am_no, string amount_name)
        {
            SqlConnection conn = DatabaseConnection.GetConnection();
            try
            {
                SqlCommand cmd = new SqlCommand("UPDATE amount SET amount_name=@amount_name WHERE am_no=@am_no", conn);
                cmd.Parameters.AddWithValue("@am_no", am_no);
                cmd.Parameters.AddWithValue("@amount_name", amount_name);
                conn.Open();
                cmd.ExecuteNonQuery();

                // Refresh the DataGridView to show updated data
                LoadAmount();
                MessageBox.Show("Amount updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearTextboxes();
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

        // Delete an amount
        private void btnDeleteAmount_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtAmountNo.Text, out int am_no))
            {
                DeleteAmount(am_no);
            }
            else
            {
                MessageBox.Show("Please enter a valid Amount Number to delete.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Delete amount from the database
        public void DeleteAmount(int am_no)
        {
            SqlConnection conn = DatabaseConnection.GetConnection();
            try
            {
                SqlCommand cmd = new SqlCommand("DELETE FROM amount WHERE am_no=@am_no", conn);
                cmd.Parameters.AddWithValue("@am_no", am_no);
                conn.Open();
                cmd.ExecuteNonQuery();
                MessageBox.Show("Amount deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Refresh the DataGridView after deleting
                LoadAmount();
                ClearTextboxes();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                MessageBox.Show("An error occurred while deleting the amount. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (conn.State == System.Data.ConnectionState.Open)
                    conn.Close();
            }
        }

        // Clear all textboxes after adding, updating, or deleting
        public void ClearTextboxes()
        {
            txtAmountNo.Clear();
            txtAmountName.Clear();
        }

        // Handle DataGridView cell click
        private void dataGridViewAmount_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridViewAmount.Rows[e.RowIndex];
                txtAmountNo.Text = row.Cells["am_no"].Value.ToString();
                txtAmountName.Text = row.Cells["amount_name"].Value.ToString();
            }
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

        private void lbReceipts_Click(object sender, EventArgs e)
        {
            this.Hide(); // Hide the DashboardForm

            // Open the receipt Form
            ReceiptsForm receiptForm = new ReceiptsForm();
            receiptForm.Show();
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
