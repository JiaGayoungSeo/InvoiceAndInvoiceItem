using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
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

namespace FinalProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Invoice> InvoiceList = new List<Invoice>();
        List<InvoiceItems> InvoiceItemsList = new List<InvoiceItems>();
        private Invoice CurrentSelectedInvoice = new Invoice();
        private InvoiceItems CurrentSeledtedItems = new InvoiceItems();
        private int CurrentSelectedInvoiceIndex;
        private int CurrentSelectedItemIndex;
        private bool isNewRecord;

        public MainWindow()
        {
            InitializeComponent();

            LoadInvoice();
            isNewRecord = false;
        }

        void LoadInvoice()
        {
            InvoiceList.Clear(); // empty out all display data
            InvoiceRecListbox.Items.Clear();
            InvoiceRecListbox.Items.Add(String.Format("{0,5}{1,15}{2,40}{3,80}{4,40}", "Invoice ID ", "Customer", "Email", "Address", "Shipped"));
            
            //Create and open a connection
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename =|DataDirectory|\Invoices.mdf; Integrated Security = True";
                connection.Open();
                string sqlCommand = $"Select * From Invoices";
                SqlCommand myCommand = new SqlCommand(sqlCommand, connection);
                using (SqlDataReader Reader = myCommand.ExecuteReader())
                {
                    while (Reader.Read())
                    {
                        Invoice newInvoice = new Invoice();
                        newInvoice.InvoiceID = (int)Reader[0];
                        newInvoice.InvoiceDateTime = (DateTime)Reader[1];
                        newInvoice.CustomerName = (String)Reader[3];
                        newInvoice.CustomerAddress = (string)Reader[4];
                        newInvoice.CustomerEmail = (String)Reader[5];
                        newInvoice.Shipping = (bool)Reader["Shipped"];


                        InvoiceList.Add(newInvoice);//add to list
                        InvoiceRecListbox.Items.Add(newInvoice);//add to listox
                    }

                }
            }
        }

        void LoadItems()
        {
            //emply all display data
            InvoiceItemsList.Clear();
            InvoiceItemListbox.Items.Clear();
            SubTotTxtbox.Clear();
            PstTxtbox.Clear();
            GstTxtbox.Clear();
            TotalTxtbox.Clear();
            InvoiceItemListbox.Items.Add(String.Format("{0,5}{1,20}{2,40}{3,80}{4,40}", "Item ID ", "Item Name", "Item Description", "Item Price", "Price"));

            decimal SubTotal = 0;//assign subtotal

            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename =|DataDirectory|\Invoices.mdf; Integrated Security = True";
                connection.Open();
                string sqlCommand = $"Select * From InvoiceItems WHERE InvoiceID = {CurrentSelectedInvoice.InvoiceID}";
                SqlCommand myCommand = new SqlCommand(sqlCommand, connection);
                using (SqlDataReader Reader = myCommand.ExecuteReader())
                {
                    while (Reader.Read())
                    {
                        InvoiceItems newInvoiceItems = new InvoiceItems((int)Reader[0], (int)Reader[1], (String)Reader[2], (string)Reader[3], (decimal)Reader[4], (int)Reader[5]);

                        InvoiceItemsList.Add(newInvoiceItems);
                        InvoiceItemListbox.Items.Add(newInvoiceItems);

                        //when items are loaded, multiply quantity and price and add up in subtoal 
                        SubTotal += newInvoiceItems.ItemPrice * newInvoiceItems.ItemQuantity;

                        //calculate based on subtotal
                        decimal PST = SubTotal * 6 / 100;
                        decimal GST = SubTotal * 5 / 100;
                        decimal Total = SubTotal + PST + GST;

                        //display in textboxes
                        SubTotTxtbox.Text = Convert.ToString(SubTotal);
                        PstTxtbox.Text = Convert.ToString(PST);
                        GstTxtbox.Text = Convert.ToString(GST);
                        TotalTxtbox.Text = Convert.ToString(Total);
                    }

                }
            }
        }

        private void DisplayInvoice()
        {
            if (CurrentSelectedInvoice != null)
            {
                InvoiceTxtbox.Text = CurrentSelectedInvoice.InvoiceID.ToString();
                v.Text = CurrentSelectedInvoice.InvoiceDateTime.ToString();

                CustNameTxbox.Text = CurrentSelectedInvoice.CustomerName;
                CustEmailTxtbox.Text = CurrentSelectedInvoice.CustomerEmail;
                CustAddressTxbox.Text = CurrentSelectedInvoice.CustomerAddress;

                //if items are shipped, checkbox is checked
                if ((bool)CurrentSelectedInvoice.Shipping == true)
                {
                    ShippedChckbox.IsChecked = true;
                }
                else
                {
                    ShippedChckbox.IsChecked = false;
                }

            }

            else
            {
                InvoiceTxtbox.Clear();
                CustNameTxbox.Clear();
                CustEmailTxtbox.Clear();
                CustAddressTxbox.Clear();
            }
        }
        private void DisplayItems()
        {
            if (CurrentSeledtedItems != null)
            {
                ItemIDTxtbox.Text = CurrentSeledtedItems.ItemID.ToString();
                ItemNameTxtbox.Text = CurrentSeledtedItems.ItemName;
                ItemDescTxtbox.Text = CurrentSeledtedItems.ItemDescription;
                ItemPriceTxtbox.Text = CurrentSeledtedItems.ItemPrice.ToString();
                ItemQuantTxtbox.Text = CurrentSeledtedItems.ItemQuantity.ToString();
            }

            else
            {
                ItemIDTxtbox.Clear();
                ItemNameTxtbox.Clear();
                ItemDescTxtbox.Clear();
                ItemPriceTxtbox.Clear();
                ItemQuantTxtbox.Clear();
            }
        }

        private void InvoiceRecListbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (InvoiceRecListbox.SelectedIndex == 0)
            {
                InvoiceRecListbox.SelectedIndex = 1;
            }
            CurrentSelectedInvoice = (Invoice)InvoiceRecListbox.SelectedItem;
            CurrentSelectedInvoiceIndex = InvoiceRecListbox.SelectedIndex;
            DisplayInvoice();
            LoadItems();
        }

        private void InvoiceItemListbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (InvoiceItemListbox.SelectedIndex == 0)
            {
                InvoiceItemListbox.SelectedIndex = 1;
            }

            CurrentSeledtedItems = (InvoiceItems)InvoiceItemListbox.SelectedItem;
            CurrentSelectedItemIndex = InvoiceItemListbox.SelectedIndex;
            DisplayItems();
            isNewRecord = false;
        }

        private void NewItemBtn_Click(object sender, RoutedEventArgs e)
        {
            //clear textboxes needed and be ready to save by boolean
            ItemIDTxtbox.Clear();
            ItemNameTxtbox.Clear();
            ItemDescTxtbox.Clear();
            ItemPriceTxtbox.Clear();
            ItemQuantTxtbox.Clear();
            isNewRecord = true;
        }

        private void SaveItemBtn_Click(object sender, RoutedEventArgs e)
        {
            if (isNewRecord)
            {
                //to check is the data users put is valid. If not, return
                if(IsItemDataValid() == false)
                {
                    ErrorMeassageBox.Text = "Invalid Information";
                    return;
                }

                SaveItems();
            }
            else if (isNewRecord == false) {
                UpdateItems();
            }
        }

        void UpdateInvoice()
        {
           

            CurrentSelectedInvoice.InvoiceID = Convert.ToInt32(InvoiceTxtbox.Text);
            CurrentSelectedInvoice.CustomerName = CustNameTxbox.Text;
            CurrentSelectedInvoice.InvoiceDateTime = Convert.ToDateTime(v.Text);
            CurrentSelectedInvoice.Shipping = Convert.ToBoolean(ShippedChckbox.IsChecked.Value);
            CurrentSelectedInvoice.CustomerEmail = CustEmailTxtbox.Text;
            CurrentSelectedInvoice.CustomerAddress = CustAddressTxbox.Text;

            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename =|DataDirectory|\Invoices.mdf; Integrated Security = True";
                connection.Open();
                string sqlCommand = "UPDATE Invoices SET " +
                                    
                                    $"InvoiceDate= '{CurrentSelectedInvoice.InvoiceDateTime.ToString()}'," +
                                    $"Shipped = '{CurrentSelectedInvoice.Shipping.ToString()}'," +
                                    $"CustomerName= '{CurrentSelectedInvoice.CustomerName}'" +
                                    $"CustomerEmail = '{CurrentSelectedInvoice.CustomerEmail}'"+
                                    $"CustomerAddress = '{CurrentSelectedInvoice.CustomerAddress}'"+
                                    $"WHERE InvoiceID = '{CurrentSelectedInvoice.InvoiceID}; ";


                using (SqlCommand UpdateCommand = new SqlCommand(sqlCommand, connection))
                {
                    try
                    {
                        UpdateCommand.ExecuteNonQuery();
                        MessageBox.Show("Information saved");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Update:  " + ex);
                    }
                }
            }

            
            LoadInvoice();
        }

        void UpdateItems()
        {
            CurrentSeledtedItems.ItemID = Convert.ToInt32(ItemIDTxtbox.Text);
            CurrentSeledtedItems.ItemName = ItemNameTxtbox.Text;
            CurrentSeledtedItems.ItemDescription = ItemDescTxtbox.Text;
            CurrentSeledtedItems.ItemPrice = Convert.ToDecimal(ItemPriceTxtbox.Text);
            CurrentSeledtedItems.ItemQuantity = Convert.ToInt32(ItemQuantTxtbox.Text);

            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename =|DataDirectory|\Invoices.mdf; Integrated Security = True";
                connection.Open();
                string sqlCommand = "UPDATE InvoiceItems SET " +
                                    $"ItemName= '{CurrentSeledtedItems.ItemName}', " +
                                    $"ItemDescription= '{CurrentSeledtedItems.ItemDescription}'," +
                                    $"ItemPrice = '{CurrentSeledtedItems.ItemPrice.ToString()}'," +
                                    $"ItemQuantity= '{CurrentSeledtedItems.ItemQuantity.ToString()}'" +
                                    $"WHERE ItemID = '{CurrentSeledtedItems.ItemID}; ";


                using (SqlCommand UpdateCommand = new SqlCommand(sqlCommand, connection))
                {
                    try
                    {
                        UpdateCommand.ExecuteNonQuery();
                        MessageBox.Show("Information saved");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Update:  " + ex);
                    }
                }
            }
            LoadItems();
        }

        //methods to check if the information users put is valid
        bool IsInvoiceDataValid()
        {
            if (CustNameTxbox.Text == "" || CustEmailTxtbox.Text == "" || v.Text == "") return false;
            else return true;    
        }

        bool IsItemDataValid()
        {
            if (ItemNameTxtbox.Text == "" || ItemPriceTxtbox.Text == "" || ItemQuantTxtbox.Text == "") return false;
            else if (Convert.ToDecimal(ItemPriceTxtbox.Text) <= 0 || (Convert.ToInt32(ItemQuantTxtbox.Text) <= 0)) return false;
            else return true;
        }

        
        
        void SaveInvoice()
        {
            string sql;

            //create new object from form data
            Invoice newInvoice = new Invoice();

            newInvoice.InvoiceDateTime = Convert.ToDateTime(v.Text);
            newInvoice.Shipping = ShippedChckbox.IsChecked.Value;
            newInvoice.CustomerName = CustNameTxbox.Text;
            newInvoice.CustomerEmail = CustEmailTxtbox.Text;
            newInvoice.CustomerAddress = CustAddressTxbox.Text;

            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename =|DataDirectory|\Invoices.mdf; Integrated Security = True";
                connection.Open();

                //to find the last primary key used (to know the maximum primary key)
                sql = $"Select MAX(InvoiceID) FROM Invoices;";

                int NewInvoiceID;


                //to create new primary key
                using (SqlCommand Selection = new SqlCommand(sql, connection))
                {
                    NewInvoiceID = Convert.ToInt32(Selection.ExecuteScalar()) + 1;
                    newInvoice.InvoiceID = NewInvoiceID;
                }

                sql = $"INSERT INTO Invoices " +
                      "(InvoiceID,InvoiceDate,Shipped,CustomerName, CustomerAddress, CustomerEmail) " +
                      "VALUES " +
                      $"('{newInvoice.InvoiceID}'," +
                      $"'{newInvoice.InvoiceDateTime}'," +
                      $"'{newInvoice.Shipping}'," +
                      $"'{newInvoice.CustomerName}'," +
                      $"'{newInvoice.CustomerAddress}'," +
                      $"'{newInvoice.CustomerEmail}')";

                using (SqlCommand InsertCommand = new SqlCommand(sql, connection))
                {
                    InsertCommand.ExecuteNonQuery();
                }

                //clear all data displayed and update data
                InvoiceRecListbox.Items.Clear();
                LoadInvoice();

                int NewInvoiceIndex = InvoiceRecListbox.Items.IndexOf(newInvoice);

                //select the new index
                InvoiceRecListbox.SelectedIndex = NewInvoiceIndex;
                InvoiceItemListbox.ScrollIntoView(newInvoice);
            }

            //not to save data users put unless clicking addnew buttons
            isNewRecord = false;

        }
        
    

        void SaveItems()
        {
            string sql;

            //create a new object from form data
            InvoiceItems newItems = new InvoiceItems();

            //get changed data from form
            newItems.ItemName = ItemNameTxtbox.Text;
            newItems.ItemDescription = ItemDescTxtbox.Text;
            newItems.ItemPrice = Convert.ToDecimal(ItemPriceTxtbox.Text);
            newItems.ItemQuantity = Convert.ToInt32(ItemQuantTxtbox.Text);

            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename =|DataDirectory|\Invoices.mdf; Integrated Security = True";
                //open connection
                connection.Open();
                //find the last primary key used
                sql = $"Select MAX(ItemID) FROM InvoiceItems;";

                int NewItemID;


                using (SqlCommand Selection = new SqlCommand(sql, connection))
                {
                    NewItemID = Convert.ToInt32(Selection.ExecuteScalar()) + 1;
                    newItems.ItemID = NewItemID;
                }

                sql = $"INSERT INTO InvoiceItems " +
                      "(ItemID,InvoiceID,ItemName,ItemDescription, ItemPrice, ItemQuantity) " +
                      "VALUES " +
                      $"('{NewItemID}'," +
                      $"'{CurrentSelectedInvoice.InvoiceID}'," + 
                      $"'{newItems.ItemName}',"+
                      $"'{newItems.ItemDescription}'," +
                      $"'{newItems.ItemPrice.ToString()}'," +
                      $"'{newItems.ItemQuantity.ToString()}')";

                using (SqlCommand InsertCommand = new SqlCommand(sql, connection))
                {
                    InsertCommand.ExecuteNonQuery();
                }

                LoadItems();

                int NewItemIndex = InvoiceItemListbox.Items.IndexOf(newItems);

                InvoiceItemListbox.SelectedIndex = NewItemIndex;
                InvoiceItemListbox.ScrollIntoView(newItems);
            }

            isNewRecord = false;
        }

        
        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {

            if (isNewRecord == true)
            {
                if (IsInvoiceDataValid() == false)
                {
                    MessageTextBox.Text = "Invalid Information";
                    return;
                }
                SaveInvoice();
            }
            
        }

        private void NewBtn_Click(object sender, RoutedEventArgs e)
        {
            InvoiceTxtbox.Clear();
            CustNameTxbox.Clear();
            CustEmailTxtbox.Clear();
            CustAddressTxbox.Clear();
            isNewRecord = true;
        }

        void DeleteItems()
        {
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString =
                 @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename =|DataDirectory|\Invoices.mdf; Integrated Security = True";
                connection.Open();

                string sql = $"DELETE From InvoiceItems WHERE ItemID = {CurrentSeledtedItems.ItemID}";

                using (SqlCommand myCommand = new SqlCommand(sql, connection))
                {
                    try
                    {
                        myCommand.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {
                        Exception error = new Exception("Error no matching record to delete", ex);
                        throw error;
                    }
                }
                LoadItems();
            }

            int IndexToDelete = CurrentSelectedItemIndex;

            //remove the current object from List<InvoiceItems>
            InvoiceItemsList.Remove(CurrentSeledtedItems);
            //remove the current object from listbox
            InvoiceItemListbox.Items.Remove(CurrentSeledtedItems);

            //if the last record deleted, reselect the next time
            if (IndexToDelete == InvoiceItemsList.Count)
            {
                CurrentSelectedItemIndex = InvoiceItemsList.Count - 1;
            }
            else
            {
                CurrentSelectedItemIndex = IndexToDelete;
            }

            InvoiceItemListbox.SelectedIndex = CurrentSelectedItemIndex;

        }
        private void DeleteItemBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentSeledtedItems == InvoiceItemListbox.SelectedItem)
            {
                DeleteItems();
                
            }
           
        }

        void DeleteInvoice()
        {
            //Delete all items Invoice's invoice item records.
            using(SqlConnection connection = new SqlConnection())
            {
                //open a connection
                connection.ConnectionString =
                  @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename =|DataDirectory|\Invoices.mdf; Integrated Security = True";
                connection.Open();
                string sql = $"DELETE From InvoiceItems WHERE InvoiceID = {CurrentSelectedInvoice.InvoiceID}";

                using (SqlCommand myCommand = new SqlCommand(sql, connection))
                {
                    try
                    {
                        myCommand.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {
                        Exception error = new Exception("Error no matching record to delete", ex);
                        throw error;
                    }
                }

            }

            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString =
                  @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename =|DataDirectory|\Invoices.mdf; Integrated Security = True";
                connection.Open();

                string sql = $"DELETE From Invoices WHERE InvoiceID = {CurrentSelectedInvoice.InvoiceID}";
                using (SqlCommand myCommand = new SqlCommand(sql, connection))
                {
                    try
                    {
                        myCommand.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {
                        Exception error = new Exception("Error no matching record to delete", ex);
                        throw error;
                    }
                }
                LoadInvoice();
            //remove the current object from the List<InvoiceList>  
            InvoiceList.Remove(CurrentSelectedInvoice);
            //remove the current object from listbox
            InvoiceRecListbox.Items.Remove(CurrentSelectedInvoice);
            }

            int IndexToDelete = CurrentSelectedInvoiceIndex;
            
            //if last record is deleted, select last record or if it's not reselct same position in the list
            if (IndexToDelete == InvoiceList.Count)
            {
                CurrentSelectedInvoiceIndex = InvoiceList.Count - 1;
            }
            else
            {
                CurrentSelectedInvoiceIndex = IndexToDelete;
            }

            //select the item
            InvoiceRecListbox.SelectedIndex = CurrentSelectedInvoiceIndex;
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if(CurrentSelectedInvoice == InvoiceRecListbox.SelectedItem)
            {
                DeleteInvoice();
            }
            
        }
    }
}
