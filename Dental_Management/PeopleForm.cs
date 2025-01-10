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
    public partial class PeopleForm : Form
    {
        public PeopleForm()
        {
            InitializeComponent();
        }

        

        private void PeopleForm_Load(object sender, EventArgs e)
        {
            GeneratePeopleNo();  // Automatically generate People No on load
            LoadPeople();  // Load existing people records

        }

        // Generate People No automatically
        public void GeneratePeopleNo()
        {
            SqlConnection conn = DatabaseConnection.GetConnection();
            try
            {
                SqlCommand cmd = new SqlCommand("SELECT MAX(p_no) FROM people", conn);
                conn.Open();
                var result = cmd.ExecuteScalar();
                int newId = (result != DBNull.Value) ? Convert.ToInt32(result) + 1 : 1; // Start at 1 if no records exist
                txtPeopleNo.Text = newId.ToString();  // Set the generated ID in the textbox
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

        // Insert a new person into the database
        private void btnAddPeople_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtPeopleName.Text) &&
                !string.IsNullOrWhiteSpace(txtPeopleNumber.Text) &&
                !string.IsNullOrWhiteSpace(txtPeopleBirthDate.Text) &&
                comBoxPeopleSex.SelectedIndex != -1)
            {
                AddPeople(txtPeopleNo.Text, txtPeopleName.Text, txtPeopleNumber.Text, comBoxPeopleSex.SelectedItem.ToString(), DateTime.Parse(txtPeopleBirthDate.Text));
                ClearTextboxes();  // Clear textboxes after adding the person
                GeneratePeopleNo();  // Automatically generate a new ID for the next person
            }
            else
            {
                MessageBox.Show("Please enter valid data for all fields.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Insert person into the database
        public void AddPeople(string p_no, string name, string tell, string sex, DateTime birthDate)
        {
            SqlConnection conn = DatabaseConnection.GetConnection();
            try
            {
                SqlCommand cmd = new SqlCommand("INSERT INTO people (p_no, name, tell, sex, birth_date) VALUES (@p_no, @name, @tell, @sex, @birth_date)", conn);
                cmd.Parameters.AddWithValue("@p_no", p_no);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@tell", tell);
                cmd.Parameters.AddWithValue("@sex", sex);
                cmd.Parameters.AddWithValue("@birth_date", birthDate);
                conn.Open();
                cmd.ExecuteNonQuery();
                MessageBox.Show("Person added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Refresh the DataGridView
                LoadPeople();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                MessageBox.Show("An error occurred while adding the person. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (conn.State == System.Data.ConnectionState.Open)
                    conn.Close();
            }
        }

        // Refresh the DataGridView with the latest data from the database
        private void LoadPeople()
        {
            SqlConnection conn = DatabaseConnection.GetConnection();
            try
            {
                SqlCommand cmd = new SqlCommand("SELECT p_no, name, tell, sex, birth_date FROM people", conn);
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();
                dataAdapter.Fill(dataTable);

                // Assuming your DataGridView is named "dataGridViewPeople"
                dataGridViewPeople.DataSource = dataTable;
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

        // Update person information
        private void btnEditPeople_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtPeopleNo.Text, out int p_no))
            {
                UpdatePeople(p_no);
            }
            else
            {
                MessageBox.Show("Please enter a valid People Number to update.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Update person details in the database
        public void UpdatePeople(int p_no)
        {
            string name = txtPeopleName.Text;
            string tell = txtPeopleNumber.Text;
            string sex = comBoxPeopleSex.SelectedItem?.ToString();
            DateTime birthDate = DateTime.Parse(txtPeopleBirthDate.Text);

            SqlConnection conn = DatabaseConnection.GetConnection();
            try
            {
                SqlCommand cmd = new SqlCommand("UPDATE people SET name=@name, tell=@tell, sex=@sex, birth_date=@birth_date WHERE p_no=@p_no", conn);
                cmd.Parameters.AddWithValue("@p_no", p_no);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@tell", tell);
                cmd.Parameters.AddWithValue("@sex", sex);
                cmd.Parameters.AddWithValue("@birth_date", birthDate);
                conn.Open();
                cmd.ExecuteNonQuery();

                // Refresh the DataGridView to show updated data
                LoadPeople();
                MessageBox.Show("Person updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        // Delete a person
        private void btnDeletePeople_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtPeopleNo.Text, out int p_no))
            {
                DeletePeople(p_no);
            }
            else
            {
                MessageBox.Show("Please enter a valid People Number to delete.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Delete person from the database
        public void DeletePeople(int p_no)
        {
            SqlConnection conn = DatabaseConnection.GetConnection();
            try
            {
                SqlCommand cmd = new SqlCommand("DELETE FROM people WHERE p_no=@p_no", conn);
                cmd.Parameters.AddWithValue("@p_no", p_no);
                conn.Open();
                cmd.ExecuteNonQuery();
                MessageBox.Show("Person deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Refresh the DataGridView after deleting
                LoadPeople();
                ClearTextboxes();  // Clear textboxes after deleting
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                MessageBox.Show("An error occurred while deleting the person. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            txtPeopleNo.Clear();
            txtPeopleName.Clear();
            txtPeopleNumber.Clear();
            txtPeopleBirthDate.Clear();
            comBoxPeopleSex.SelectedIndex = -1;  // Reset the combo box selection
        }

        // Method to handle the cell click in DataGridView
        private void dataGridViewPeople_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridViewPeople.Rows[e.RowIndex];
                txtPeopleNo.Text = row.Cells["p_no"].Value.ToString();
                txtPeopleName.Text = row.Cells["name"].Value.ToString();
                txtPeopleNumber.Text = row.Cells["tell"].Value.ToString();
                comBoxPeopleSex.SelectedItem = row.Cells["sex"].Value.ToString();
                txtPeopleBirthDate.Text = row.Cells["birth_date"].Value.ToString();
            }
        }

        // Method to handle the cell click in DataGridView
        private void dataGridViewPeople_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridViewPeople.Rows[e.RowIndex];
                txtPeopleNo.Text = row.Cells["p_no"].Value.ToString();
                txtPeopleName.Text = row.Cells["name"].Value.ToString();
                txtPeopleNumber.Text = row.Cells["tell"].Value.ToString();
                comBoxPeopleSex.SelectedItem = row.Cells["sex"].Value.ToString();
                txtPeopleBirthDate.Text = row.Cells["birth_date"].Value.ToString();
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
