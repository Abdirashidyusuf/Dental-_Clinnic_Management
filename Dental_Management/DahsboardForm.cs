using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dental_Management
{
    public partial class DahsboardForm : Form
    {
        public DahsboardForm()
        {
            InitializeComponent();
        }

        private void DahsboardForm_Load(object sender, EventArgs e)
        {

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
