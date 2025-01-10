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
    public partial class ServicesForm : Form
    {
        public ServicesForm()
        {
            InitializeComponent();
        }

        private void ServicesForm_Load(object sender, EventArgs e)
        {
            GenerateServiceId();  // Automatically generate Service ID on load
            LoadServices();  // Load existing services
        }

        // Generate Service ID automatically
        public void GenerateServiceId()
        {
            SqlConnection conn = DatabaseConnection.GetConnection();
            try
            {
                SqlCommand cmd = new SqlCommand("SELECT MAX(ser_no) FROM services", conn);
                conn.Open();
                var result = cmd.ExecuteScalar();
                int newId = (result != DBNull.Value) ? Convert.ToInt32(result) + 1 : 1; // Start at 1 if no records exist
                txtServiceNo.Text = newId.ToString();  // Set the generated ID in the textbox
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

        // Insert a new service into the database
        private void btnAddService_Click(object sender, EventArgs e)
        {
            if (float.TryParse(txtServicePrice.Text, out float price) &&
                !string.IsNullOrWhiteSpace(txtServiceName.Text) &&
                !string.IsNullOrWhiteSpace(txtServiceDescription.Text))
            {
                AddService(txtServiceNo.Text, txtServiceName.Text, txtServiceDescription.Text, price);
                ClearTextboxes();  // Clear textboxes after adding the service
                GenerateServiceId();  // Automatically generate a new ID for the next service
            }
            else
            {
                MessageBox.Show("Please enter valid data for all fields.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Insert service into the database
        public void AddService(string ser_no, string ser_name, string description, float price)
        {
            SqlConnection conn = DatabaseConnection.GetConnection();
            try
            {
                SqlCommand cmd = new SqlCommand("INSERT INTO services (ser_no, ser_name, description, price) VALUES (@ser_no, @ser_name, @description, @price)", conn);
                cmd.Parameters.AddWithValue("@ser_no", ser_no);
                cmd.Parameters.AddWithValue("@ser_name", ser_name);
                cmd.Parameters.AddWithValue("@description", description);
                cmd.Parameters.AddWithValue("@price", price);
                conn.Open();
                cmd.ExecuteNonQuery();
                MessageBox.Show("Service added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Refresh the DataGridView
                LoadServices();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                MessageBox.Show("An error occurred while adding the service. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (conn.State == System.Data.ConnectionState.Open)
                    conn.Close();
            }
        }

        // Refresh the DataGridView with the latest data from the database
        private void LoadServices()
        {
            SqlConnection conn = DatabaseConnection.GetConnection();
            try
            {
                SqlCommand cmd = new SqlCommand("SELECT ser_no, ser_name, description, price FROM services", conn);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);

                // Assuming your DataGridView is named "dataGridViewService"
                dataGridViewService.DataSource = dataTable;
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

        // Update service information
        private void btnEditService_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtServiceNo.Text, out int ser_no))
            {
                UpdateService(ser_no);
            }
            else
            {
                MessageBox.Show("Please enter a valid Service Number to update.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Update service details in the database
        public void UpdateService(int ser_no)
        {
            // After the user has made updates, get the updated data from the textboxes
            string ser_name = txtServiceName.Text;
            string description = txtServiceDescription.Text;
            float price = float.Parse(txtServicePrice.Text); // Ensure this is a valid float value

            SqlConnection conn = DatabaseConnection.GetConnection();
            try
            {
                SqlCommand cmd = new SqlCommand("UPDATE services SET ser_name=@ser_name, description=@description, price=@price WHERE ser_no=@ser_no", conn);
                cmd.Parameters.AddWithValue("@ser_no", ser_no);
                cmd.Parameters.AddWithValue("@ser_name", ser_name);
                cmd.Parameters.AddWithValue("@description", description);
                cmd.Parameters.AddWithValue("@price", price);
                conn.Open();
                cmd.ExecuteNonQuery();

                // Refresh the DataGridView to show updated data
                LoadServices();
                MessageBox.Show("Service updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearTextboxes();  // Clear textboxes after editing
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

        // Method to handle the cell click in DataGridView
        private void dataGridViewService_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridViewService.Rows[e.RowIndex];
                txtServiceNo.Text = row.Cells["ser_no"].Value.ToString();
                txtServiceName.Text = row.Cells["ser_name"].Value.ToString();
                txtServiceDescription.Text = row.Cells["description"].Value.ToString();
                txtServicePrice.Text = row.Cells["price"].Value.ToString();
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btnDeleteService_Click_1(object sender, EventArgs e)
        {
            if (int.TryParse(txtServiceNo.Text, out int ser_no))
            {
                DeleteService(ser_no);
            }
            else
            {
                MessageBox.Show("Please enter a valid service number.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        // Delete service from the database
        public void DeleteService(int ser_no)
        {
            SqlConnection conn = DatabaseConnection.GetConnection();
            try
            {
                SqlCommand cmd = new SqlCommand("DELETE FROM services WHERE ser_no=@ser_no", conn);
                cmd.Parameters.AddWithValue("@ser_no", ser_no);
                conn.Open();
                cmd.ExecuteNonQuery();
                MessageBox.Show("Service deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Refresh the DataGridView after deleting
                LoadServices();
                ClearTextboxes();  // Clear textboxes after deleting
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                MessageBox.Show("An error occurred while deleting the service. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            txtServiceNo.Clear();
            txtServiceName.Clear();
            txtServiceDescription.Clear();
            txtServicePrice.Clear();
        }

        private void lbDashboard_Click(object sender, EventArgs e)
        {
            this.Hide();
            DahsboardForm dashboard = new DahsboardForm();
            dashboard.Show();
        }

        private void lbServices_Click(object sender, EventArgs e)
        {
           
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
