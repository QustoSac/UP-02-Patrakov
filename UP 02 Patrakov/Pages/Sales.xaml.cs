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

namespace UP_02_Patrakov.Pages
{
    public partial class Sales : Page
    {
        private readonly Master_PolEntities db;
        public Sales(int partnerID)
        {
            InitializeComponent();

            db = new Master_PolEntities();

            var partnerSales = db.PartnerProducts.Where(p => p.partnerID == partnerID).ToList();

            DataGridSales.ItemsSource = partnerSales;
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new DiscountPage());
        }
    }
}
