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
    public partial class BillsForm : Form
    {
        public BillsForm()
        {
            InitializeComponent();
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void BillsForm_Load(object sender, EventArgs e)
        {
            GenerateBillNo(); // Automatically generate Bill No on load
            LoadBills();      // Load existing bill records
        }

        // Generate Bill No automatically
        public void GenerateBillNo()
        {
            SqlConnection conn = DatabaseConnection.GetConnection();
            try
            {
                SqlCommand cmd = new SqlCommand("SELECT MAX(bil_no) FROM bills", conn);
                conn.Open();
                var result = cmd.ExecuteScalar();
                int newId = (result != DBNull.Value) ? Convert.ToInt32(result) + 1 : 1; // Start at 1 if no records exist
                txtBillNo.Text = newId.ToString(); // Set the generated ID in the textbox
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

        // Add a new bill
        private void btnAddBill_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtAmountNo.Text) &&
                !string.IsNullOrWhiteSpace(txtAmount.Text) &&
                DateTime.TryParse(txtBillDate.Text, out DateTime billDate) &&
                !string.IsNullOrWhiteSpace(txtDescriptionBill.Text))
            {
                AddBill(txtBillNo.Text, txtAmountNo.Text, float.Parse(txtAmount.Text), billDate, txtDescriptionBill.Text);
                ClearTextboxes();  // Clear textboxes after adding the bill
                GenerateBillNo();  // Automatically generate a new ID for the next bill
            }
            else
            {
                MessageBox.Show("Please enter valid data for all fields.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Insert bill into the database
        public void AddBill(string bil_no, string am_no, float amount, DateTime bil_date, string description)
        {
            SqlConnection conn = DatabaseConnection.GetConnection();
            try
            {
                SqlCommand cmd = new SqlCommand("INSERT INTO bills (bil_no, am_no, amount, bil_date, description) VALUES (@bil_no, @am_no, @amount, @bil_date, @description)", conn);
                cmd.Parameters.AddWithValue("@bil_no", bil_no);
                cmd.Parameters.AddWithValue("@am_no", am_no);
                cmd.Parameters.AddWithValue("@amount", amount);
                cmd.Parameters.AddWithValue("@bil_date", bil_date);
                cmd.Parameters.AddWithValue("@description", description);
                conn.Open();
                cmd.ExecuteNonQuery();
                MessageBox.Show("Bill added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Refresh the DataGridView
                LoadBills();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                MessageBox.Show("An error occurred while adding the bill. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (conn.State == System.Data.ConnectionState.Open)
                    conn.Close();
            }
        }

        // Refresh the DataGridView with the latest data from the database
        private void LoadBills()
        {
            SqlConnection conn = DatabaseConnection.GetConnection();
            try
            {
                SqlCommand cmd = new SqlCommand("SELECT bil_no, am_no, amount, bil_date, description FROM bills", conn);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);

                // Assuming your DataGridView is named "dataGridViewBill"
                dataGridViewBill.DataSource = dataTable;
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

        // Update bill information
        private void btnEditBill_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtBillNo.Text, out int bil_no))
            {
                UpdateBill(bil_no);
            }
            else
            {
                MessageBox.Show("Please enter a valid Bill Number to update.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Update bill details in the database
        public void UpdateBill(int bil_no)
        {
            string am_no = txtAmountNo.Text;
            float amount = float.Parse(txtAmount.Text);
            DateTime bil_date = DateTime.Parse(txtBillDate.Text);
            string description = txtDescriptionBill.Text;

            SqlConnection conn = DatabaseConnection.GetConnection();
            try
            {
                SqlCommand cmd = new SqlCommand("UPDATE bills SET am_no=@am_no, amount=@amount, bil_date=@bil_date, description=@description WHERE bil_no=@bil_no", conn);
                cmd.Parameters.AddWithValue("@bil_no", bil_no);
                cmd.Parameters.AddWithValue("@am_no", am_no);
                cmd.Parameters.AddWithValue("@amount", amount);
                cmd.Parameters.AddWithValue("@bil_date", bil_date);
                cmd.Parameters.AddWithValue("@description", description);
                conn.Open();
                cmd.ExecuteNonQuery();

                // Refresh the DataGridView to show updated data
                LoadBills();
                MessageBox.Show("Bill updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        // Delete a bill
        private void btnDeleteBill_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtBillNo.Text, out int bil_no))
            {
                DeleteBill(bil_no);
            }
            else
            {
                MessageBox.Show("Please enter a valid Bill Number to delete.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Delete bill from the database
        public void DeleteBill(int bil_no)
        {
            SqlConnection conn = DatabaseConnection.GetConnection();
            try
            {
                SqlCommand cmd = new SqlCommand("DELETE FROM bills WHERE bil_no=@bil_no", conn);
                cmd.Parameters.AddWithValue("@bil_no", bil_no);
                conn.Open();
                cmd.ExecuteNonQuery();
                MessageBox.Show("Bill deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Refresh the DataGridView after deleting
                LoadBills();
                ClearTextboxes();  // Clear textboxes after deleting
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                MessageBox.Show("An error occurred while deleting the bill. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            txtBillNo.Clear();
            txtAmountNo.Clear();
            txtAmount.Clear();
            txtBillDate.Clear();
            txtDescriptionBill.Clear();
        }

        // Handle DataGridView cell click to populate textboxes
        private void dataGridViewBill_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridViewBill.Rows[e.RowIndex];
                txtBillNo.Text = row.Cells["bil_no"].Value.ToString();
                txtAmountNo.Text = row.Cells["am_no"].Value.ToString();
                txtAmount.Text = row.Cells["amount"].Value.ToString();
                txtBillDate.Text = row.Cells["bil_date"].Value.ToString();
                txtDescriptionBill.Text = row.Cells["description"].Value.ToString();
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
