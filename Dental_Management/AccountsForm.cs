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
    public partial class AccountsForm : Form
    {
        public AccountsForm()
        {
            InitializeComponent();
        }

        private void AccountsForm_Load(object sender, EventArgs e)
        {
            GenerateAccountNo(); // Automatically generate Account No on load
            LoadAccounts(); // Load existing account records
        }

        // Generate Account No automatically
        public void GenerateAccountNo()
        {
            SqlConnection conn = DatabaseConnection.GetConnection();
            try
            {
                SqlCommand cmd = new SqlCommand("SELECT MAX(acc_no) FROM accounts", conn);
                conn.Open();
                var result = cmd.ExecuteScalar();
                int newId = (result != DBNull.Value) ? Convert.ToInt32(result) + 1 : 1; // Start at 1 if no records exist
                txtAccountNo.Text = newId.ToString(); // Set the generated ID in the textbox
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

        // Insert a new account into the database
        private void btnAddAccount_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtAccountName.Text) &&
                !string.IsNullOrWhiteSpace(txtAccountInstitution.Text) &&
                float.TryParse(txtAccountBalance.Text, out float balance))
            {
                AddAccount(txtAccountNo.Text, txtAccountName.Text, txtAccountInstitution.Text, balance);
                ClearTextboxes(); // Clear textboxes after adding the account
                GenerateAccountNo(); // Automatically generate a new ID for the next account
            }
            else
            {
                MessageBox.Show("Please enter valid data for all fields.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Insert account into the database
        public void AddAccount(string acc_no, string acc_name, string institution, float balance)
        {
            SqlConnection conn = DatabaseConnection.GetConnection();
            try
            {
                SqlCommand cmd = new SqlCommand("INSERT INTO accounts (acc_no, acc_name, institution, balance) VALUES (@acc_no, @acc_name, @institution, @balance)", conn);
                cmd.Parameters.AddWithValue("@acc_no", acc_no);
                cmd.Parameters.AddWithValue("@acc_name", acc_name);
                cmd.Parameters.AddWithValue("@institution", institution);
                cmd.Parameters.AddWithValue("@balance", balance);
                conn.Open();
                cmd.ExecuteNonQuery();
                MessageBox.Show("Account added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Refresh the DataGridView
                LoadAccounts();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                MessageBox.Show("An error occurred while adding the account. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (conn.State == System.Data.ConnectionState.Open)
                    conn.Close();
            }
        }

        // Refresh the DataGridView with the latest data from the database
        private void LoadAccounts()
        {
            SqlConnection conn = DatabaseConnection.GetConnection();
            try
            {
                SqlCommand cmd = new SqlCommand("SELECT acc_no, acc_name, institution, balance FROM accounts", conn);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);

                dataGridViewAccount.DataSource = dataTable;
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

        // Update account information
        private void btnEditAccount_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtAccountNo.Text, out int acc_no))
            {
                UpdateAccount(acc_no);
            }
            else
            {
                MessageBox.Show("Please enter a valid Account Number to update.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Update account details in the database
        public void UpdateAccount(int acc_no)
        {
            string acc_name = txtAccountName.Text;
            string institution = txtAccountInstitution.Text;
            float balance = float.Parse(txtAccountBalance.Text);

            SqlConnection conn = DatabaseConnection.GetConnection();
            try
            {
                SqlCommand cmd = new SqlCommand("UPDATE accounts SET acc_name=@acc_name, institution=@institution, balance=@balance WHERE acc_no=@acc_no", conn);
                cmd.Parameters.AddWithValue("@acc_no", acc_no);
                cmd.Parameters.AddWithValue("@acc_name", acc_name);
                cmd.Parameters.AddWithValue("@institution", institution);
                cmd.Parameters.AddWithValue("@balance", balance);
                conn.Open();
                cmd.ExecuteNonQuery();

                // Refresh the DataGridView to show updated data
                LoadAccounts();
                MessageBox.Show("Account updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        // Delete an account
        private void btnDeleteAccount_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtAccountNo.Text, out int acc_no))
            {
                DeleteAccount(acc_no);
            }
            else
            {
                MessageBox.Show("Please enter a valid Account Number to delete.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Delete account from the database
        public void DeleteAccount(int acc_no)
        {
            SqlConnection conn = DatabaseConnection.GetConnection();
            try
            {
                SqlCommand cmd = new SqlCommand("DELETE FROM accounts WHERE acc_no=@acc_no", conn);
                cmd.Parameters.AddWithValue("@acc_no", acc_no);
                conn.Open();
                cmd.ExecuteNonQuery();
                MessageBox.Show("Account deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Refresh the DataGridView after deleting
                LoadAccounts();
                ClearTextboxes(); // Clear textboxes after deleting
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                MessageBox.Show("An error occurred while deleting the account. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            txtAccountNo.Clear();
            txtAccountName.Clear();
            txtAccountInstitution.Clear();
            txtAccountBalance.Clear();
        }

        // Method to handle the cell click in DataGridView
        private void dataGridViewAccount_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridViewAccount.Rows[e.RowIndex];
                txtAccountNo.Text = row.Cells["acc_no"].Value.ToString();
                txtAccountName.Text = row.Cells["acc_name"].Value.ToString();
                txtAccountInstitution.Text = row.Cells["institution"].Value.ToString();
                txtAccountBalance.Text = row.Cells["balance"].Value.ToString();
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
