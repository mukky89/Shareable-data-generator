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
using System.Data.Entity;
using System.Reflection;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.IO;
using System.Data.SqlClient;
using Syncfusion.XlsIO;
using System.Data;

namespace Shareable_data_generator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // odporuca sa spravit len jeden context pre databazu a pracovat snou, lebo takto sa spravi len jedno nacitanie z DB
        private readonly ShareableDataEntities TE = new ShareableDataEntities();
        string FolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\OneDrive";
        string conString = @"Data Source=SYXDBX02\ISYS;Initial Catalog=ISYS;User ID=peaklogger;Password=peaklogger123";

        public MainWindow()
        {

            InitializeComponent();
            loadgrid();
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MTc2NDI5QDMxMzcyZTMzMmUzMGtnZUVNR0xlQ0xUNW5sUEhCSjJKbW1UNFF6bVhjR1hqSENyQkRsbTdkQTg9");

        }

        private void loadgrid()
        {
            // Najprv treba nacitat vsetko z DB do cache a nasledne sa bude pracovat s Local ObservableCollection
            // ObservableCollection je prave taky list, ktory sa automaticky aktualizuje
            // na to aby to fungovalo musi kazdy mat atribut spraveny PropertyChanged - na to som ale moc lenivy takze som pridal
            // plugin ktory to robi za nas + som upravil template Model1.tt na DB, ktory to bude sam pridavat do kazdej tabulky
            TE.MainTable.Load();
            dataGrid.ItemsSource = TE.MainTable.Local;
        }

        private IEnumerable<DataGridRow> GetDataGridRowsForButtons(DataGrid grid)
        {
            grid.UpdateLayout();
            {
                var itemsSource = grid.ItemsSource as IEnumerable<MainTable>;
                if (null == itemsSource) yield return null;

                foreach (var item in itemsSource)
                {

                    var row1 = grid.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
                    if (null != row1 & row1.IsSelected)
                        yield return row1;
                }
            }
        }

        public DataTable GetData()
        {
            SqlConnection conn = new SqlConnection(conString);
            conn.Open();
            string query = (dataGrid.SelectedItem as MainTable).SQLstring;
            SqlCommand cmd = new SqlCommand(query, conn);

            DataTable dt = new DataTable();
            dt.Load(cmd.ExecuteReader());
            conn.Close();
            return dt;
        }


        void Button_Click_delete_row(object sender, RoutedEventArgs e)

        {
            try
            {
                int data_id = (dataGrid.SelectedItem as MainTable).Id;
                MainTable tbl = (from r in TE.MainTable where r.Id == data_id select r).SingleOrDefault();
                TE.MainTable.Remove(tbl);
                TE.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }



        public void dataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            var currentRow = e.Row;
            var currentItem = currentRow?.DataContext;
            var currentHeader = e.Column?.Header;
            var currentValue = (e.Column?.GetCellContent(currentRow) as TextBox).Text;
            string CustomerName = (dataGrid.SelectedItem as MainTable).CustomerName;
            string ExcelName = (dataGrid.SelectedItem as MainTable).ExcelName;
            string ExcelPath = FolderPath + @"\ShareableData\" + CustomerName + @"\" + ExcelName + ".xlsx";
            string ColumnsName = (dataGrid.SelectedItem as MainTable).ColumnsName;
            string FolderPathSX = FolderPath + @"\ShareableData\" + CustomerName;


            switch (currentHeader)
            {
                case "CustomerName":
                    System.IO.Directory.CreateDirectory(FolderPathSX);
                    (dataGrid.SelectedItem as MainTable).FolderPath = FolderPathSX;
                    (dataGrid.SelectedItem as MainTable).LastQuery = DateTime.Now.ToString();
                    break;
                case "ExcelName":
                    if (ExcelName != null)
                    {
                        ExcelEngine excelEngine = new ExcelEngine();
                        IApplication application = excelEngine.Excel;
                        IWorkbook workbook = application.Workbooks.Create(1);
                        workbook.Worksheets[0].Name = "Data from SYLEX";
                        workbook.SaveAs(ExcelPath);
                        workbook.Close();
                        excelEngine.Dispose();
                        (dataGrid.SelectedItem as MainTable).FilePath = ExcelPath;
                    }
                    break;
                case "ColumnsName":
                    if (ColumnsName != null)
                    {
                    }
                    break;

            }

            TE.SaveChanges();
        }

        private void dataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {

        }

        private void dataGrid_CurrentCellChanged(object sender, EventArgs e)
        {
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {

            var data = TE.MainTable.ToList();
            foreach (var dbdata in data)
            {
                string name = dbdata.ColumnsName;
                string CustomerName = dbdata.CustomerName;
                string ExcelName = dbdata.ExcelName;
                string ExcelPath = dbdata.FilePath;
                string ColumnsName = dbdata.ColumnsName;

                string FolderPathSX = FolderPath + @"\ShareableData\" + CustomerName;
                string[] columns = ColumnsName.Split(';').Where(x => !string.IsNullOrEmpty(x)).ToArray();
                ExcelEngine excelEngine = new ExcelEngine();
                IApplication application = excelEngine.Excel;
                IWorkbook workbook = excelEngine.Excel.Workbooks.Open(ExcelPath);


                workbook.Worksheets[0].ClearData();
                var worksheet = workbook.Worksheets["Data from SYLEX"];
                for (int i = 0; i < columns.Count(); i++)
                {
                    worksheet.Range[1, i + 1].Value = columns[i];
                    worksheet.Range[1, i + 1].CellStyle.Font.Bold = true;
                }

                int ColumnsCount = columns.Count();
                string sqlString = (dataGrid.SelectedItem as MainTable).SQLstring;

                DataTable SqlData = GetData();
                worksheet.ImportDataTable(SqlData, true, 1, 1);
                worksheet.UsedRange.AutofitColumns();
                worksheet.InsertRow(1, 1, ExcelInsertOptions.FormatAsBefore);
                worksheet.Range[1, 1].Value = "Last update: " + DateTime.Now.ToString();
                (dataGrid.SelectedItem as MainTable).LastQuery = DateTime.Now.ToString();

                workbook.SaveAs(ExcelPath);
                workbook.Close();
                excelEngine.Dispose();
                TE.SaveChanges();

            }

             //string CustomerName = (dataGrid.SelectedItem as MainTable).CustomerName;
             //string ExcelName = (dataGrid.SelectedItem as MainTable).ExcelName;
             //string ExcelPath = (dataGrid.SelectedItem as MainTable).FilePath;
             //string ColumnsName = (dataGrid.SelectedItem as MainTable).ColumnsName;

           
        }

        private void btnStartTimer_Click(object sender, RoutedEventArgs e)
        {
            double hrtoms = Convert.ToDouble(this.textBoxInterval.Text);
            TimeSpan result = TimeSpan.FromHours(hrtoms);
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = result;
            dispatcherTimer.Start();
            btnStartTimer.IsEnabled = false;
            MessageBox.Show("Timer bol zapnutý, interval je: " + hrtoms + " [h]");
        }

        private void btnStopTimer_Click(object sender, RoutedEventArgs e)
        {
            btnStartTimer.IsEnabled = true;
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Stop();
            MessageBox.Show("Timer bol zastavený");
        }
        private void Button_Click_Test(object sender, RoutedEventArgs e)
        {
            btnStartTimer.IsEnabled = true;
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Stop();
            MessageBox.Show("Timer bol zastavený");
        }
    }

}
