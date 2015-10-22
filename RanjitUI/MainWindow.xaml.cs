using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SeleniumWorker;

namespace RanjitUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _useCostCenterInsteadOfCostUnit = false;

        public MainWindow()
        {
            InitializeComponent();

            int defaultWorkType = Properties.Settings.Default.DefaultWorkType;

            this.txbUserName.Text = Properties.Settings.Default.DefaultUser;
            this.txbWorkType.Text = defaultWorkType.ToString();
            this.txbHours.Text = Properties.Settings.Default.DefaultHours.ToString();
            this.txbCostUnit.Text = Properties.Settings.Default.DefaultCostUnit.ToString();
            this.txbCostCenter.Text = Properties.Settings.Default.DefaultCostCenter.ToString();
            SetCostCategoriesVisibility(defaultWorkType);
        }
        
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            string userName = this.txbUserName.Text;
            Properties.Settings.Default.DefaultUser = userName;
            string password = this.pwbPassword.Password;
            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("The password field is mandatory", "Required field", MessageBoxButton.OK);
                return;
            }

            Properties.Settings.Default.DefaultWorkType = int.Parse(this.txbWorkType.Text);
            Properties.Settings.Default.DefaultHours = double.Parse(this.txbHours.Text);
            Properties.Settings.Default.DefaultCostUnit = long.Parse(this.txbCostUnit.Text);
            Properties.Settings.Default.DefaultCostCenter = long.Parse(this.txbCostCenter.Text);

            Properties.Settings.Default.Save();

            DateTime start = this.dtpStartDate.SelectedDate ?? DateTime.Now;
            DateTime end = this.dtpEndDate.SelectedDate ?? DateTime.Now;

            this.imgMain.Visibility = Visibility.Visible;

            //StartSeleniumWorker(userName, password, start, end);

            //this.imgMain.Visibility = Visibility.Hidden;
        }

        private void StartSeleniumWorker(string userName, string password, DateTime startDate, DateTime endDate)
        {
            WebPortalWorker worker = new WebPortalWorker();

            int workType = Properties.Settings.Default.DefaultWorkType;
            double hoursWorked = Properties.Settings.Default.DefaultHours;
            long costUnit = Properties.Settings.Default.DefaultCostUnit;
            long costCenter = Properties.Settings.Default.DefaultCostCenter;
            WorkItem workItem = new WorkItem()
            {
                WorkTypeCode = workType,
                WorkHours = hoursWorked,
                CostCenterCode = costCenter,
                CostUnitCode = costUnit
            };

            try
            {
                worker.FillInPeriod(userName, password, startDate, endDate, workItem);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void SetCostCategoriesVisibility(int defaultWorkType)
        {
            if (defaultWorkType < 200)
            {
                _useCostCenterInsteadOfCostUnit = false;
                
                txbCostUnit.IsEnabled = true;
                txbCostCenter.IsEnabled = false;
            }
            else
            {
                _useCostCenterInsteadOfCostUnit = true;
                
                txbCostCenter.IsEnabled = true;
                txbCostUnit.IsEnabled = false;
            }
        }
        
        private void TxbWorkType_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            int workType = int.Parse(this.txbWorkType.Text);
            SetCostCategoriesVisibility(workType);
        }
    }
}
