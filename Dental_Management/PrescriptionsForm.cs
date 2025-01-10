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
    public partial class PrescriptionsForm : Form
    {
        public PrescriptionsForm()
        {
            InitializeComponent();
        }

        private void PrescriptionsForm_Load(object sender, EventArgs e)
        {
            GeneratePrescriptionNo(); // Automatically generate Prescription No on load
            LoadPrescriptions(); // Load existing prescription records
        }

        // Generate Prescription No automatically
        public void GeneratePrescriptionNo()
        {
            SqlConnection conn = DatabaseConnection.GetConnection();
            try
            {
                SqlCommand cmd = new SqlCommand("SELECT MAX(pr_no) FROM prescriptions", conn);
                conn.Open();
                var result = cmd.ExecuteScalar();
                int newId = (result != DBNull.Value) ? Convert.ToInt32(result) + 1 : 1; // Start at 1 if no records exist
                txtPrescriptionNo.Text = newId.ToString(); // Set the generated ID in the textbox
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

        // Add a new prescription
        private void btnAddPrescription_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtPrescriptionName.Text) &&
                !string.IsNullOrWhiteSpace(txtPrescriptionDescription.Text) &&
                !string.IsNullOrWhiteSpace(txtPrescriptionMadeIn.Text) &&
                DateTime.TryParse(txtPrescriptionExpireDate.Text, out DateTime expireDate))
            {
                AddPrescription(txtPrescriptionNo.Text, txtPrescriptionName.Text, txtPrescriptionDescription.Text, txtPrescriptionMadeIn.Text, expireDate);
                ClearPrescriptionTextboxes(); // Clear textboxes after adding
                GeneratePrescriptionNo(); // Automatically generate a new ID for the next prescription
            }
            else
            {
                MessageBox.Show("Please enter valid data for all fields.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Add prescription to the database
        public void AddPrescription(string pr_no, string name, string description, string madeIn, DateTime expireDate)
        {
            SqlConnection conn = DatabaseConnection.GetConnection();
            try
            {
                SqlCommand cmd = new SqlCommand("INSERT INTO prescriptions (pr_no, pres_name, description, madeIn, expire_date) VALUES (@pr_no, @pres_name, @description, @madeIn, @expire_date)", conn);
                cmd.Parameters.AddWithValue("@pr_no", pr_no);
                cmd.Parameters.AddWithValue("@pres_name", name);
                cmd.Parameters.AddWithValue("@description", description);
                cmd.Parameters.AddWithValue("@madeIn", madeIn);
                cmd.Parameters.AddWithValue("@expire_date", expireDate);
                conn.Open();
                cmd.ExecuteNonQuery();
                MessageBox.Show("Prescription added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Refresh the DataGridView
                LoadPrescriptions();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                MessageBox.Show("An error occurred while adding the prescription. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (conn.State == System.Data.ConnectionState.Open)
                    conn.Close();
            }
        }

        // Refresh DataGridView with the latest data
        private void LoadPrescriptions()
        {
            SqlConnection conn = DatabaseConnection.GetConnection();
            try
            {
                SqlCommand cmd = new SqlCommand("SELECT pr_no, pres_name, description, madeIn, expire_date FROM prescriptions", conn);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);

                dataGridViewPrescription.DataSource = dataTable; // Assuming your DataGridView name
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

        // Edit prescription details
        private void btnEditPrescription_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtPrescriptionNo.Text, out int pr_no))
            {
                UpdatePrescription(pr_no);
            }
            else
            {
                MessageBox.Show("Please enter a valid Prescription No to update.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Update prescription in the database
        public void UpdatePrescription(int pr_no)
        {
            string name = txtPrescriptionName.Text;
            string description = txtPrescriptionDescription.Text;
            string madeIn = txtPrescriptionMadeIn.Text;
            DateTime expireDate = DateTime.Parse(txtPrescriptionExpireDate.Text);

            SqlConnection conn = DatabaseConnection.GetConnection();
            try
            {
                SqlCommand cmd = new SqlCommand("UPDATE prescriptions SET pres_name=@pres_name, description=@description, madeIn=@madeIn, expire_date=@expire_date WHERE pr_no=@pr_no", conn);
                cmd.Parameters.AddWithValue("@pr_no", pr_no);
                cmd.Parameters.AddWithValue("@pres_name", name);
                cmd.Parameters.AddWithValue("@description", description);
                cmd.Parameters.AddWithValue("@madeIn", madeIn);
                cmd.Parameters.AddWithValue("@expire_date", expireDate);
                conn.Open();
                cmd.ExecuteNonQuery();

                // Refresh DataGridView
                LoadPrescriptions();
                MessageBox.Show("Prescription updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearPrescriptionTextboxes();
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

        // Delete a prescription
        private void btnDeletePrescription_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtPrescriptionNo.Text, out int pr_no))
            {
                DeletePrescription(pr_no);
            }
            else
            {
                MessageBox.Show("Please enter a valid Prescription No to delete.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Delete prescription from the database
        public void DeletePrescription(int pr_no)
        {
            SqlConnection conn = DatabaseConnection.GetConnection();
            try
            {
                SqlCommand cmd = new SqlCommand("DELETE FROM prescriptions WHERE pr_no=@pr_no", conn);
                cmd.Parameters.AddWithValue("@pr_no", pr_no);
                conn.Open();
                cmd.ExecuteNonQuery();
                MessageBox.Show("Prescription deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Refresh DataGridView
                LoadPrescriptions();
                ClearPrescriptionTextboxes();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                MessageBox.Show("An error occurred while deleting the prescription. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (conn.State == System.Data.ConnectionState.Open)
                    conn.Close();
            }
        }

        // Clear all textboxes
        public void ClearPrescriptionTextboxes()
        {
            txtPrescriptionNo.Clear();
            txtPrescriptionName.Clear();
            txtPrescriptionDescription.Clear();
            txtPrescriptionMadeIn.Clear();
            txtPrescriptionExpireDate.Clear();
        }

        // Handle DataGridView cell click
        private void dataGridViewPrescription_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridViewPrescription.Rows[e.RowIndex];
                txtPrescriptionNo.Text = row.Cells["pr_no"].Value.ToString();
                txtPrescriptionName.Text = row.Cells["pres_name"].Value.ToString();
                txtPrescriptionDescription.Text = row.Cells["description"].Value.ToString();
                txtPrescriptionMadeIn.Text = row.Cells["madeIn"].Value.ToString();
                txtPrescriptionExpireDate.Text = row.Cells["expire_date"].Value.ToString();
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
