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
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;

namespace Shareable_data_generator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // odporuca sa spravit len jeden context pre databazu a pracovat snou, lebo takto sa spravi len jedno nacitanie z DB
        private readonly ShareableDataEntities TE = new ShareableDataEntities();

        public MainWindow()
        {
            InitializeComponent();
            loadgrid();
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


        private void dataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {

        }

        private void dataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            // ulozi zmeny v contexte do DB
            TE.SaveChanges();
        }
    }
}
