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
    public partial class VisitsForm : Form
    {
        public VisitsForm()
        {
            InitializeComponent();
        }

        private void VisitsForm_Load(object sender, EventArgs e)
        {
            GenerateVisitNo();  // Automatically generate Visit No on load
            LoadVisits();  // Load existing visit records
        }

        // Generate Visit No automatically
        public void GenerateVisitNo()
        {
            SqlConnection conn = DatabaseConnection.GetConnection();
            try
            {
                SqlCommand cmd = new SqlCommand("SELECT MAX(v_no) FROM visits", conn);
                conn.Open();
                var result = cmd.ExecuteScalar();
                int newId = (result != DBNull.Value) ? Convert.ToInt32(result) + 1 : 1; // Start at 1 if no records exist
                txtVisitNo.Text = newId.ToString();  // Set the generated ID in the textbox
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

        // Insert a new visit into the database
        private void btnAddVisit_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtPatient.Text) &&
                !string.IsNullOrWhiteSpace(txtDoctor.Text) &&
                !string.IsNullOrWhiteSpace(txtVisitDate.Text))
            {
                AddVisit(txtVisitNo.Text, txtPatient.Text, txtDoctor.Text, DateTime.Parse(txtVisitDate.Text));
                ClearTextboxes();  // Clear textboxes after adding the visit
                GenerateVisitNo();  // Automatically generate a new ID for the next visit
            }
            else
            {
                MessageBox.Show("Please enter valid data for all fields.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Insert visit into the database
        public void AddVisit(string v_no, string patient, string doctor, DateTime v_date)
        {
            SqlConnection conn = DatabaseConnection.GetConnection();
            try
            {
                SqlCommand cmd = new SqlCommand("INSERT INTO visits (v_no, patient, doctor, v_date) VALUES (@v_no, @patient, @doctor, @v_date)", conn);
                cmd.Parameters.AddWithValue("@v_no", v_no);
                cmd.Parameters.AddWithValue("@patient", patient);
                cmd.Parameters.AddWithValue("@doctor", doctor);
                cmd.Parameters.AddWithValue("@v_date", v_date);
                conn.Open();
                cmd.ExecuteNonQuery();
                MessageBox.Show("Visit added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Refresh the DataGridView
                LoadVisits();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                MessageBox.Show("An error occurred while adding the visit. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (conn.State == System.Data.ConnectionState.Open)
                    conn.Close();
            }
        }

        // Refresh the DataGridView with the latest data from the database
        private void LoadVisits()
        {
            SqlConnection conn = DatabaseConnection.GetConnection();
            try
            {
                SqlCommand cmd = new SqlCommand("SELECT v_no, patient, doctor, v_date FROM visits", conn);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);

                dataGridViewVisit.DataSource = dataTable;
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

        // Update visit information
        private void btnEditVisit_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtVisitNo.Text, out int v_no))
            {
                UpdateVisit(v_no);
            }
            else
            {
                MessageBox.Show("Please enter a valid Visit Number to update.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Update visit details in the database
        public void UpdateVisit(int v_no)
        {
            string patient = txtPatient.Text;
            string doctor = txtDoctor.Text;
            string v_date = txtVisitDate.Text;

            SqlConnection conn = DatabaseConnection.GetConnection();
            try
            {
                SqlCommand cmd = new SqlCommand("UPDATE visits SET patient=@patient, doctor=@doctor, v_date=@v_date WHERE v_no=@v_no", conn);
                cmd.Parameters.AddWithValue("@v_no", v_no);
                cmd.Parameters.AddWithValue("@patient", patient);
                cmd.Parameters.AddWithValue("@doctor", doctor);
                cmd.Parameters.AddWithValue("@v_date", v_date);
                conn.Open();
                cmd.ExecuteNonQuery();

                // Refresh the DataGridView to show updated data
                LoadVisits();
                MessageBox.Show("Visit updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        // Delete a visit
        private void btnDeleteVisit_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtVisitNo.Text, out int v_no))
            {
                DeleteVisit(v_no);
            }
            else
            {
                MessageBox.Show("Please enter a valid Visit Number to delete.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Delete visit from the database
        public void DeleteVisit(int v_no)
        {
            SqlConnection conn = DatabaseConnection.GetConnection();
            try
            {
                SqlCommand cmd = new SqlCommand("DELETE FROM visits WHERE v_no=@v_no", conn);
                cmd.Parameters.AddWithValue("@v_no", v_no);
                conn.Open();
                cmd.ExecuteNonQuery();
                MessageBox.Show("Visit deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Refresh the DataGridView after deleting
                LoadVisits();
                ClearTextboxes();  // Clear textboxes after deleting
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                MessageBox.Show("An error occurred while deleting the visit. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            txtVisitNo.Clear();
            txtPatient.Clear();
            txtDoctor.Clear();
            txtVisitDate.Clear();
        }

        // Handle cell click in DataGridView to populate textboxes

        private void dataGridViewVisit_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridViewVisit.Rows[e.RowIndex];
                txtVisitNo.Text = row.Cells["v_no"].Value.ToString();
                txtPatient.Text = row.Cells["patient"].Value.ToString();
                txtDoctor.Text = row.Cells["doctor"].Value.ToString();
                txtVisitDate.Text = row.Cells["v_date"].Value.ToString();
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
