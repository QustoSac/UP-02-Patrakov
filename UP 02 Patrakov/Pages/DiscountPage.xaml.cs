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

        public partial class DiscountPage : Page
        {
            private readonly Master_PolEntities db;
            public DiscountPage()
            {
                InitializeComponent();

                db = new Master_PolEntities();
                LoadDiscounts();
            }

            private void LoadDiscounts()
            {
                var partnerSales = db.PartnerProducts
                    .GroupBy(p => p.Partners)
                    .Select(g => new
                    {
                        Partners = g.Key,
                        TotalQuantity = g.Sum(p => p.quantityProduct ?? 0)
                    })
                    .ToList();


                var partnerDiscounts = partnerSales.Select(ps => new
                {
                    Partners = db.Partners.FirstOrDefault(p => p.ID == ps.Partners.ID),
                    TotalQuantity = ps.TotalQuantity,
                    Discount = CalculateDiscount(ps.TotalQuantity)
                }).ToList();

                ListUser.ItemsSource = partnerDiscounts;
            }

            private decimal CalculateDiscount(int totalQuantity)
            {
                if (totalQuantity <= 10000)
                    return 0;
                else if (totalQuantity <= 50000)
                    return 5;
                else if (totalQuantity <= 300000)
                    return 10;
                else
                    return 15;
            }

            private void ListUser_SelectionChanged(object sender, MouseButtonEventArgs e)
            {
                if (ListUser.SelectedItem != null)
                {
                    var selectedPartner = (dynamic)ListUser.SelectedItem;
                    var partner = selectedPartner.Partners;

                    // Переход на страницу AddPartner с передачей выбранного партнера
                    NavigationService.Navigate(new AddPartner(partner));
                }
            }

            private void AddPartner_Click(object sender, RoutedEventArgs e)
            {
                NavigationService.Navigate(new AddPartner());
            }

            private void Sales_Click(object sender, RoutedEventArgs e)
            {
                if (ListUser.SelectedItem != null) // Проверяем, что элемент выбран
                {
                    // Получаем выбранного партнера
                    var selectedPartner = (dynamic)ListUser.SelectedItem;
                    int partnerId = selectedPartner.Partners.ID;

                    // Переход на страницу Sales с передачей ID партнёра
                    NavigationService?.Navigate(new Sales(partnerId));
                }
                else
                {
                    // Выводим сообщение, если элемент не выбран
                    MessageBox.Show("Пожалуйста, выберите партнёра из списка.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
    
}
