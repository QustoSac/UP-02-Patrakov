using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    public partial class AddPartner : Page
    {

        private static readonly Regex FIOregex = new Regex(@"^[А-ЯЁ][а-яё]+$"); // Для ФИО директора
        private static readonly Regex EmailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$"); // Для электронной почты
        private static readonly Regex INNRegex = new Regex(@"^\d{10}$"); // Для ИНН (10 цифр)
        private static readonly Regex QuantityRegex = new Regex(@"^\d+$");
        private static readonly Regex PhoneNumberRegex = new Regex(@"^\+?[1-9]\d{9,14}$"); // Для номера телефона с международным кодом

        private readonly Master_PolEntities db; // Контекст базы данных
        private readonly Partners currentPartner;

        public AddPartner(Partners selectedPartner = null)
        {
            InitializeComponent();

            db = new Master_PolEntities();
            LoadTypes(); // Загрузка типов партнеров в комбобокс
            LoadProducts();
            LoadProductTypes();
            currentPartner = selectedPartner;

            if (currentPartner != null)
            {
                Name.Text = currentPartner.partnerName;
                Email.Text = currentPartner.partnerEmail;
                Phone.Text = currentPartner.partnerPhone;
                Director.Text = currentPartner.Director;
                Adress.Text = currentPartner.partnerAdress;
                INN.Text = currentPartner.partnerINN;  // Здесь могут быть проблемы с форматированием ИНН
                Rating.Text = currentPartner.partnerRating.ToString();

                if (currentPartner.partnerTypeID != null)
                {
                    Type.SelectedValue = currentPartner.partnerTypeID; // Выбираем тип партнера
                }

                var partnerProduct = db.PartnerProducts
                               .FirstOrDefault(pp => pp.partnerID == currentPartner.ID);

                if (partnerProduct != null)
                {
                    // Выбираем соответствующий товар и тип товара, если такой есть
                    Product.SelectedValue = partnerProduct.productID;
                    Quantity.Text = partnerProduct.quantityProduct.ToString();

                    // Найдем тип товара и установим его в комбобокс
                    var product = db.Product.FirstOrDefault(p => p.ID == partnerProduct.productID);
                    if (product != null)
                    {
                        var productType = db.ProductType.FirstOrDefault(pt => pt.ID == product.productTypeID);
                        if (productType != null)
                        {
                            TypeProduct.SelectedValue = productType.ID; // Устанавливаем тип товара
                        }
                    }
                }
            }
        }

        private void LoadTypes()
        {
            var types = db.PartnerType.ToList(); // Получаем все типы из базы
            Type.ItemsSource = types; // Привязываем типы к источнику данных
            Type.DisplayMemberPath = "typeName"; // Отображаем название типа
            Type.SelectedValuePath = "ID"; // ID будет значением, которое хранится в базе
        }

        private void LoadProducts()
        {
            var products = db.Product.ToList();
            Product.ItemsSource = products;
            Product.DisplayMemberPath = "productName";
            Product.SelectedValuePath = "ID";
        }

        private void LoadProductTypes()
        {
            var productTypes = db.ProductType.ToList();
            TypeProduct.ItemsSource = productTypes;
            TypeProduct.DisplayMemberPath = "productsType";
            TypeProduct.SelectedValuePath = "ID";
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new DiscountPage());
            ClearFields();
        }
        private void ClearFields()
        {
            Type.SelectedItem = null;
            Name.Text = string.Empty;
            Director.Text = string.Empty;
            Adress.Text = string.Empty;
            Phone.Text = string.Empty;
            INN.Text = string.Empty;
            Rating.Text = string.Empty;
            Email.Text = string.Empty;
            TypeProduct.SelectedItem = null;
            Product.SelectedItem = null;
            Quantity.Text = string.Empty;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            string NameOrganization = Name.Text;
            string DirectorOrganization = Director.Text;
            string EmailOrganization = Email.Text;
            string NumberOrganization = Phone.Text;
            string INNOrganization = INN.Text;
            string RatingOrganization = Rating.Text;
            string AddressOrganization = Adress.Text;

            int? selectedType = (int?)Type.SelectedValue;

            string quantity = Quantity.Text;
            int? selectedProductTypeID = (int?)TypeProduct.SelectedValue;
            int? selectedProductID = (int?)Product.SelectedValue;

            try
            {
                // Валидация данных
                if (!FIOValidation(DirectorOrganization))
                {
                    MessageBox.Show(
                        "ФИО директора должно содержать от 2 до 5 слов, начинаться с заглавной буквы и содержать только буквы кириллицы.",
                        "Ошибка: Некорректное ФИО",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                if (!PhoneNumberRegex.IsMatch(NumberOrganization))
                {
                    MessageBox.Show(
                        "Номер телефона должен быть в международном формате, например: +1234567890.",
                        "Ошибка: Некорректный номер телефона",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                if (!EmailRegex.IsMatch(EmailOrganization))
                {
                    MessageBox.Show(
                        "Электронная почта должна быть в формате example@domain.com.",
                        "Ошибка: Некорректная электронная почта",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                if (!INNRegex.IsMatch(INNOrganization))
                {
                    MessageBox.Show(
                        "ИНН должен содержать ровно 10 цифр.",
                        "Ошибка: Некорректный ИНН",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(RatingOrganization, out int ratingValue) || ratingValue < 1 || ratingValue > 10)
                {
                    MessageBox.Show(
                        "Рейтинг должен быть числом от 1 до 10.",
                        "Ошибка: Некорректный рейтинг",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                if (!QuantityRegex.IsMatch(quantity))
                {
                    MessageBox.Show(
                        "Количество должно быть числовым значением.",
                        "Ошибка: Некорректное количество",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                // Работа с партнером
                if (currentPartner != null) // Редактирование
                {
                    if (MessageBox.Show(
                        "Вы уверены, что хотите сохранить изменения?",
                        "Подтверждение",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question) != MessageBoxResult.Yes)
                    {
                        return;
                    }

                    // Обновление данных партнера
                    currentPartner.partnerName = NameOrganization;
                    currentPartner.Director = DirectorOrganization;
                    currentPartner.partnerEmail = EmailOrganization;
                    currentPartner.partnerPhone = NumberOrganization;
                    currentPartner.partnerINN = INNOrganization;
                    currentPartner.partnerAdress = AddressOrganization;
                    currentPartner.partnerRating = ratingValue;
                    currentPartner.partnerTypeID = (int)selectedType;

                    db.SaveChanges();

                    int partnerId = currentPartner.ID;

                    var partnerProduct = db.PartnerProducts
                                           .FirstOrDefault(pp => pp.partnerID == partnerId);

                    if (partnerProduct != null)
                    {
                        partnerProduct.productID = selectedProductID ?? partnerProduct.productID;
                        partnerProduct.quantityProduct = int.TryParse(quantity, out int quantityValue)
                            ? quantityValue
                            : partnerProduct.quantityProduct;
                    }
                    else if (selectedProductID.HasValue && int.TryParse(quantity, out int quantityValue))
                    {
                        db.PartnerProducts.Add(new PartnerProducts
                        {
                            partnerID = partnerId,
                            productID = selectedProductID.Value,
                            quantityProduct = quantityValue,
                            dateSell = DateTime.Now
                        });
                    }

                    db.SaveChanges();
                    MessageBox.Show(
                        "Данные партнера успешно обновлены.",
                        "Успех",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                else // Создание нового партнера
                {
                    var newPartner = new Partners
                    {
                        partnerTypeID = (int)selectedType,
                        partnerName = NameOrganization,
                        Director = DirectorOrganization,
                        partnerEmail = EmailOrganization,
                        partnerPhone = NumberOrganization,
                        partnerINN = INNOrganization,
                        partnerAdress = AddressOrganization,
                        partnerRating = ratingValue
                    };

                    db.Partners.Add(newPartner);
                    db.SaveChanges();

                    int partnerId = newPartner.ID;

                    if (int.TryParse(quantity, out int quantityValue) && selectedProductID.HasValue)
                    {
                        db.PartnerProducts.Add(new PartnerProducts
                        {
                            partnerID = partnerId,
                            productID = selectedProductID.Value,
                            quantityProduct = quantityValue,
                            dateSell = DateTime.Now
                        });
                        db.SaveChanges();

                        MessageBox.Show(
                            "Партнер и продукция успешно добавлены.",
                            "Успех",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show(
                            "Некорректное количество продукции или продукт не выбран.",
                            "Предупреждение",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                    }
                }

                NavigationService?.Navigate(new DiscountPage());
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Произошла ошибка: {ex.Message}\nПожалуйста, обратитесь к администратору, если проблема повторяется.",
                    "Критическая ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private bool FIOValidation(string FIO)
        {
            var splitFIO = FIO.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries); // Разделяем ФИО по пробелам

            // Проверка, что ФИО состоит из 2-5 частей (имя, фамилия, и возможно отчество)
            if (splitFIO.Length < 2 || splitFIO.Length > 5)
            {
                return false;
            }

            // Проверка каждой части ФИО на соответствие регулярному выражению и длину
            foreach (var part in splitFIO)
            {
                if (string.IsNullOrWhiteSpace(part) || !FIOregex.IsMatch(part) || part.Length > 50)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
