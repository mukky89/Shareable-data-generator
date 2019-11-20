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

namespace Shareable_data_generator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            loadgrid();
        }

        private void loadgrid()
        {
            ShareableDataEntities TE = new ShareableDataEntities();
            var data = from d in TE.MainTable select d;
            dataGrid.ItemsSource = data.ToList();
        }

        private IEnumerable<DataGridRow> GetDataGridRowsForButtons(DataGrid grid)
        { //IQueryable 
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

        void Button_Click_Test(object sender, RoutedEventArgs e)
        {

            for (var vis = sender as Visual; vis != null; vis = VisualTreeHelper.GetParent(vis) as Visual)
                if (vis is DataGridRow)
                {
                    var rows = GetDataGridRowsForButtons(dataGrid);
                    string id;
                    foreach (DataGridRow dr in rows)
                    {
                        id = (dr.Item as MainTable).CustomerName;
                        MessageBox.Show(id);
                        break;
                    }
                    break;
                }
        }



        void Button_Click_delete_row(object sender, RoutedEventArgs e)
        {

            for (var vis = sender as Visual; vis != null; vis = VisualTreeHelper.GetParent(vis) as Visual)
                if (vis is DataGridRow)
                {
                    var rows = GetDataGridRowsForButtons(dataGrid);
                    string id;
                    foreach (DataGridRow dr in rows)
                    {
                        id = (dr.Item as MainTable).CustomerName;
                        MessageBox.Show(id);
                        break;
                    }
                    break;
                }
        }
        ShareableDataEntities _db = new ShareableDataEntities();

        private void dataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            _db.SaveChanges();
        }


    }

}
