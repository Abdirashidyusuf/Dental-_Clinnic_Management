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
    public partial class CustomerPrescriptionForm : Form
    {
        public CustomerPrescriptionForm()
        {
            InitializeComponent();
        }

        private void CustomerPrescriptionForm_Load(object sender, EventArgs e)
        {
            GenerateCustomerPrescriptionNo(); // Automatically generate Customer Prescription No on load
            LoadCustomerPrescriptions();     // Load existing records
        }


        // Generate Customer Prescription No automatically
        public void GenerateCustomerPrescriptionNo()
        {
            SqlConnection conn = DatabaseConnection.GetConnection();
            try
            {
                SqlCommand cmd = new SqlCommand("SELECT MAX(cp_no) FROM customer_prescription", conn);
                conn.Open();
                var result = cmd.ExecuteScalar();
                int newId = (result != DBNull.Value) ? Convert.ToInt32(result) + 1 : 1; // Start at 1 if no records exist
                txtCustomerPrescriptionNo.Text = newId.ToString();  // Set the generated ID in the textbox
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

        // Insert a new customer prescription into the database
        private void btnAddCustomerPrescription_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtPrescriptionNo.Text) &&
                !string.IsNullOrWhiteSpace(txtPeopleNo.Text) &&
                !string.IsNullOrWhiteSpace(txtDate.Text) &&
                !string.IsNullOrWhiteSpace(txtUsageDescription.Text) &&
                !string.IsNullOrWhiteSpace(txtDiscription.Text))
            {
                AddCustomerPrescription(txtCustomerPrescriptionNo.Text, txtPrescriptionNo.Text, txtPeopleNo.Text,
                    DateTime.Parse(txtDate.Text), txtUsageDescription.Text, txtDiscription.Text);
                ClearTextboxes();
                GenerateCustomerPrescriptionNo();
            }
            else
            {
                MessageBox.Show("Please enter valid data for all fields.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Insert customer prescription into the database
        public void AddCustomerPrescription(string cp_no, string pr_no, string p_no, DateTime cp_date, string usageDescription, string description)
        {
            SqlConnection conn = DatabaseConnection.GetConnection();
            try
            {
                SqlCommand cmd = new SqlCommand(
                    "INSERT INTO customer_prescription (cp_no, pr_no, p_no, cp_date, usage_description, description) VALUES (@cp_no, @pr_no, @p_no, @cp_date, @usage_description, @description)",
                    conn);
                cmd.Parameters.AddWithValue("@cp_no", cp_no);
                cmd.Parameters.AddWithValue("@pr_no", pr_no);
                cmd.Parameters.AddWithValue("@p_no", p_no);
                cmd.Parameters.AddWithValue("@cp_date", cp_date);
                cmd.Parameters.AddWithValue("@usage_description", usageDescription);
                cmd.Parameters.AddWithValue("@description", description);
                conn.Open();
                cmd.ExecuteNonQuery();
                MessageBox.Show("Customer Prescription added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadCustomerPrescriptions();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                MessageBox.Show("An error occurred while adding the customer prescription. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (conn.State == System.Data.ConnectionState.Open)
                    conn.Close();
            }
        }

        // Refresh the DataGridView with the latest data
        private void LoadCustomerPrescriptions()
        {
            SqlConnection conn = DatabaseConnection.GetConnection();
            try
            {
                SqlCommand cmd = new SqlCommand("SELECT cp_no, pr_no, p_no, cp_date, usage_description, description FROM customer_prescription", conn);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);

                dataGridViewCustomerPrescription.DataSource = dataTable;
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

        // Update customer prescription
        private void btnEditCustomerPrescription_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtCustomerPrescriptionNo.Text, out int cp_no))
            {
                UpdateCustomerPrescription(cp_no);
            }
            else
            {
                MessageBox.Show("Please enter a valid Customer Prescription No to update.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Update customer prescription in the database
        public void UpdateCustomerPrescription(int cp_no)
        {
            SqlConnection conn = DatabaseConnection.GetConnection();
            try
            {
                SqlCommand cmd = new SqlCommand(
                    "UPDATE customer_prescription SET pr_no=@pr_no, p_no=@p_no, cp_date=@cp_date, usage_description=@usage_description, description=@description WHERE cp_no=@cp_no",
                    conn);
                cmd.Parameters.AddWithValue("@cp_no", cp_no);
                cmd.Parameters.AddWithValue("@pr_no", txtPrescriptionNo.Text);
                cmd.Parameters.AddWithValue("@p_no", txtPeopleNo.Text);
                cmd.Parameters.AddWithValue("@cp_date", DateTime.Parse(txtDate.Text));
                cmd.Parameters.AddWithValue("@usage_description", txtUsageDescription.Text);
                cmd.Parameters.AddWithValue("@description", txtDiscription.Text);
                conn.Open();
                cmd.ExecuteNonQuery();

                LoadCustomerPrescriptions();
                MessageBox.Show("Customer Prescription updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        // Delete customer prescription
        private void btnDeleteCustomerPrescription_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtCustomerPrescriptionNo.Text, out int cp_no))
            {
                DeleteCustomerPrescription(cp_no);
            }
            else
            {
                MessageBox.Show("Please enter a valid Customer Prescription No to delete.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Delete customer prescription from the database
        public void DeleteCustomerPrescription(int cp_no)
        {
            SqlConnection conn = DatabaseConnection.GetConnection();
            try
            {
                SqlCommand cmd = new SqlCommand("DELETE FROM customer_prescription WHERE cp_no=@cp_no", conn);
                cmd.Parameters.AddWithValue("@cp_no", cp_no);
                conn.Open();
                cmd.ExecuteNonQuery();

                LoadCustomerPrescriptions();
                MessageBox.Show("Customer Prescription deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearTextboxes();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                MessageBox.Show("An error occurred while deleting the customer prescription. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (conn.State == System.Data.ConnectionState.Open)
                    conn.Close();
            }
        }

        // Clear all textboxes
        public void ClearTextboxes()
        {
            txtCustomerPrescriptionNo.Clear();
            txtPrescriptionNo.Clear();
            txtPeopleNo.Clear();
            txtDate.Clear();
            txtUsageDescription.Clear();
            txtDiscription.Clear();
        }

        // Handle DataGridView cell click
        private void dataGridViewCustomerPrescription_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridViewCustomerPrescription.Rows[e.RowIndex];
                txtCustomerPrescriptionNo.Text = row.Cells["cp_no"].Value.ToString();
                txtPrescriptionNo.Text = row.Cells["pr_no"].Value.ToString();
                txtPeopleNo.Text = row.Cells["p_no"].Value.ToString();
                txtDate.Text = row.Cells["cp_date"].Value.ToString();
                txtUsageDescription.Text = row.Cells["usage_description"].Value.ToString();
                txtDiscription.Text = row.Cells["description"].Value.ToString();
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
