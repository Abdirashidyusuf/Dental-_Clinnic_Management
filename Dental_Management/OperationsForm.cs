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
    public partial class OperationsForm : Form
    {
        public OperationsForm()
        {
            InitializeComponent();
        }

        private void OperationsForm_Load(object sender, EventArgs e)
        {
            GenerateOperationNo();  // Automatically generate Operation No on load
            LoadOperations();  // Load existing operation records
        }

        // Generate Operation No automatically
        public void GenerateOperationNo()
        {
            SqlConnection conn = DatabaseConnection.GetConnection();
            try
            {
                SqlCommand cmd = new SqlCommand("SELECT MAX(oper_no) FROM operations", conn);
                conn.Open();
                var result = cmd.ExecuteScalar();
                int newId = (result != DBNull.Value) ? Convert.ToInt32(result) + 1 : 1; // Start at 1 if no records exist
                txtOperationNo.Text = newId.ToString();  // Set the generated ID in the textbox
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

        // Insert a new operation into the database
        private void btnAddOperation_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtVisitNo.Text) &&
                !string.IsNullOrWhiteSpace(txtServiceNo.Text) &&
                !string.IsNullOrWhiteSpace(txtOperationDate.Text) &&
                !string.IsNullOrWhiteSpace(txtAppointmentDate.Text) &&
                !string.IsNullOrWhiteSpace(txtOperationDiscription.Text))
            {
                AddOperation(txtOperationNo.Text, txtVisitNo.Text, txtServiceNo.Text,
                             DateTime.Parse(txtOperationDate.Text), DateTime.Parse(txtAppointmentDate.Text),
                             txtOperationDiscription.Text);
                ClearTextboxes();  // Clear textboxes after adding the operation
                GenerateOperationNo();  // Automatically generate a new ID for the next operation
            }
            else
            {
                MessageBox.Show("Please enter valid data for all fields.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Insert operation into the database
        public void AddOperation(string oper_no, string v_no, string ser_no, DateTime oper_date, DateTime app_date, string description)
        {
            SqlConnection conn = DatabaseConnection.GetConnection();
            try
            {
                SqlCommand cmd = new SqlCommand("INSERT INTO operations (oper_no, v_no, ser_no, oper_date, app_date, description) VALUES (@oper_no, @v_no, @ser_no, @oper_date, @app_date, @description)", conn);
                cmd.Parameters.AddWithValue("@oper_no", oper_no);
                cmd.Parameters.AddWithValue("@v_no", v_no);
                cmd.Parameters.AddWithValue("@ser_no", ser_no);
                cmd.Parameters.AddWithValue("@oper_date", oper_date);
                cmd.Parameters.AddWithValue("@app_date", app_date);
                cmd.Parameters.AddWithValue("@description", description);
                conn.Open();
                cmd.ExecuteNonQuery();
                MessageBox.Show("Operation added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Refresh the DataGridView
                LoadOperations();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                MessageBox.Show("An error occurred while adding the operation. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (conn.State == System.Data.ConnectionState.Open)
                    conn.Close();
            }
        }

        // Refresh the DataGridView with the latest data from the database
        private void LoadOperations()
        {
            SqlConnection conn = DatabaseConnection.GetConnection();
            try
            {
                SqlCommand cmd = new SqlCommand("SELECT oper_no, v_no, ser_no, oper_date, app_date, description FROM operations", conn);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);

                dataGridViewOperations.DataSource = dataTable;
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

        // Update operation information
        private void btnEditOperation_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtOperationNo.Text, out int oper_no))
            {
                UpdateOperation(oper_no);
            }
            else
            {
                MessageBox.Show("Please enter a valid Operation Number to update.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Update operation details in the database
        public void UpdateOperation(int oper_no)
        {
            string v_no = txtVisitNo.Text;
            string ser_no = txtServiceNo.Text;
            DateTime oper_date = DateTime.Parse(txtOperationDate.Text);
            DateTime app_date = DateTime.Parse(txtAppointmentDate.Text);
            string description = txtOperationDiscription.Text;

            SqlConnection conn = DatabaseConnection.GetConnection();
            try
            {
                SqlCommand cmd = new SqlCommand("UPDATE operations SET v_no=@v_no, ser_no=@ser_no, oper_date=@oper_date, app_date=@app_date, description=@description WHERE oper_no=@oper_no", conn);
                cmd.Parameters.AddWithValue("@oper_no", oper_no);
                cmd.Parameters.AddWithValue("@v_no", v_no);
                cmd.Parameters.AddWithValue("@ser_no", ser_no);
                cmd.Parameters.AddWithValue("@oper_date", oper_date);
                cmd.Parameters.AddWithValue("@app_date", app_date);
                cmd.Parameters.AddWithValue("@description", description);
                conn.Open();
                cmd.ExecuteNonQuery();

                // Refresh the DataGridView to show updated data
                LoadOperations();
                MessageBox.Show("Operation updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        // Delete an operation
        private void btnDeleteOperation_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtOperationNo.Text, out int oper_no))
            {
                DeleteOperation(oper_no);
            }
            else
            {
                MessageBox.Show("Please enter a valid Operation Number to delete.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Delete operation from the database
        public void DeleteOperation(int oper_no)
        {
            SqlConnection conn = DatabaseConnection.GetConnection();
            try
            {
                SqlCommand cmd = new SqlCommand("DELETE FROM operations WHERE oper_no=@oper_no", conn);
                cmd.Parameters.AddWithValue("@oper_no", oper_no);
                conn.Open();
                cmd.ExecuteNonQuery();
                MessageBox.Show("Operation deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Refresh the DataGridView after deleting
                LoadOperations();
                ClearTextboxes();  // Clear textboxes after deleting
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                MessageBox.Show("An error occurred while deleting the operation. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            txtOperationNo.Clear();
            txtVisitNo.Clear();
            txtServiceNo.Clear();
            txtOperationDate.Clear();
            txtAppointmentDate.Clear();
            txtOperationDiscription.Clear();
        }

        // Handle cell click in DataGridView
        private void dataGridViewOperations_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridViewOperations.Rows[e.RowIndex];
                txtOperationNo.Text = row.Cells["oper_no"].Value.ToString();
                txtVisitNo.Text = row.Cells["v_no"].Value.ToString();
                txtServiceNo.Text = row.Cells["ser_no"].Value.ToString();
                txtOperationDate.Text = row.Cells["oper_date"].Value.ToString();
                txtAppointmentDate.Text = row.Cells["app_date"].Value.ToString();
                txtOperationDiscription.Text = row.Cells["description"].Value.ToString();
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
