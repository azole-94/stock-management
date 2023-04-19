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
using System.Data.SqlClient;
using System.Data;
using Azure.Core;

namespace StockManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public const string ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=""C:\Users\Azole sipika\source\repos\Stock-Management-System\StockManager\Database1.mdf"";Integrated Security=True";

        public string _name {  get; set; } 
        public string _category { get; set; }
        public string _costPrice { get; set; }
        public string _sellingPrice { get; set; }
        public string _quantity { get; set; }
        public string _barcode { get; set; }


        public MainWindow()
        {
            InitializeComponent();
            HandleLoadServer();
        }

        public void HandleLoadServer()
        {
        
        string connString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=""C:\Users\Azole sipika\source\repos\Stock-Management-System\StockManager\Database1.mdf"";Integrated Security=True";

        SqlConnection conn = new SqlConnection(connString);
            DataTable dt = new DataTable();

            conn.Open();

            SqlCommand cmd = new SqlCommand(" Select * from stock ", conn);

            SqlDataReader sdr = cmd.ExecuteReader();

            dt.Load(sdr);
            conn.Close();
            StockGrid.ItemsSource = dt.DefaultView;
        }

        public void configureBarcode ()
        {
            var random = new Random();
            var numerals = random.Next().ToString();
            var name = productName.Text.Trim().ToUpper();
            var price = productCostPrice.Text.ToString();

            var barcode = price + numerals ;
            Console.WriteLine("barcode :" + barcode);

            if (name != String.Empty)
            {
                _barcode = barcode;
            }   else
            {
                MessageBox.Show("Please Fill In All Boxes", "Incomplete", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /**
         * Validates if the form is filled
         * @return true || false
         */
        public bool formValid()
        {
            if (_name == String.Empty) {
                return false;
            }

            if(_category == String.Empty)
            {
                return false;  
            }

            if (_costPrice == String.Empty)
            {
                return false;
            }

            if (_sellingPrice == String.Empty)
            {
                return false;
            }


            return true;
        }

        public void clearInputs()
        {
            productQuantity.Text = "";
            productName.Text = "";
            productCostPrice.Text = "";
            productCategory.Text = "";
            deleteProductBarcode.Text = "";
            productSellingPrice.Text = "";

        }

        public void insert (object sender, RoutedEventArgs e)
        {
            try
            {
                if(formValid())
                {
                    configureBarcode();

                    SqlConnection conn = new(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=""C:\Users\Azole sipika\source\repos\Stock-Management-System\StockManager\Database1.mdf"";Integrated Security=True");

                    SqlCommand command = new("INSERT INTO stock VALUES (@Category, @Name, @Cost, @Selling, @Quantity ,@Barcode)", conn)
                    {
                        CommandType = CommandType.Text
                    };

                    command.Parameters.AddWithValue("@Category", productCategory.Text);
                    command.Parameters.AddWithValue("@Name", productName.Text);
                    command.Parameters.AddWithValue("@Cost", productCostPrice.Text);
                    command.Parameters.AddWithValue("@Selling", productSellingPrice.Text);
                    command.Parameters.AddWithValue("@Quantity", productQuantity.Text);
                    command.Parameters.AddWithValue("@Barcode", _barcode);

                    conn.Open();
                    command.ExecuteNonQuery();
                    conn.Close();
                    HandleLoadServer();
                    MessageBox.Show("Successfully Added With Barcode '"+ _barcode +"'", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
                    clearInputs();
                }
            } catch(SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void productCostPrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            double parsedValue;

            if (!double.TryParse(productCostPrice.Text, out parsedValue))
            {
                productCostPrice.Text = "";
            }
        }

        private void productSellingPrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            double parsedValue;

            if (!double.TryParse(productSellingPrice.Text, out parsedValue))
            {
                productSellingPrice.Text = "";
            }
        }

        private void productQuantity_TextChanged(object sender, TextChangedEventArgs e)
        {
            double parsedValue;

            if (!double.TryParse(productQuantity.Text, out parsedValue))
            {
                productQuantity.Text = "";
                
            }
        }

        public void delete(object sender, RoutedEventArgs e)
        {
            SqlConnection conn = new(ConnectionString);

            try
            {
                conn.Open();
                SqlCommand command = new SqlCommand("delete from stock where Barcode='" + this.deleteProductBarcode.Text + "'", conn);

                command.ExecuteNonQuery();
                MessageBox.Show("Deleted Product With Barcode '" + this.deleteProductBarcode.Text + "'", "Deleted", MessageBoxButton.OK, MessageBoxImage.Information);
                conn.Close();
                HandleLoadServer();
                clearInputs();
            } catch (SqlException ex) {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
