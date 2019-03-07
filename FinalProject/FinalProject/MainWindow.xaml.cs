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
            //When application stars, load invoice records into Invoice listbox
            LoadInvoice();
            isNewRecord = false;
        }

        void LoadInvoice()
        {
            InvoiceList.Clear(); // empty out all display data
            InvoiceRecListbox.Items.Clear();
            //to create first list as a column heading
            InvoiceRecListbox.Items.Add(String.Format("{0,5}{1,20}{2,50}{3,60}{4,40}", "Invoice ID ", "Customer", "Email", "Total Price", "Shipped?"));
            
            //to create new object
            Invoice newInvoice = new Invoice();
            
            //Create and open a connection
            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename =|DataDirectory|\Invoices.mdf; Integrated Security = True";
                connection.Open();

                //Create a SQL command object
                string sqlCommand = $"Select * From Invoices";
                SqlCommand myCommand = new SqlCommand(sqlCommand, connection);

                using (SqlDataReader Reader = myCommand.ExecuteReader())
                {
                    while (Reader.Read())
                    {
                        newInvoice = new Invoice();
                        newInvoice.InvoiceID = (int)Reader[0];
                        newInvoice.InvoiceDateTime = (DateTime)Reader[1];
                        newInvoice.CustomerName = (String)Reader[3];
                        newInvoice.CustomerAddress = (string)Reader[4];
                        newInvoice.CustomerEmail = (String)Reader[5];
                       
                        newInvoice.Shipping = (bool)Reader["Shipped"];


                        InvoiceList.Add(newInvoice);//add to list
                        InvoiceRecListbox.Items.Add(newInvoice);//add to listbox
                    }

                }
            }
            
            
            
        }

        void LoadItems()
        {
            //empty all display data
            InvoiceItemsList.Clear();
            InvoiceItemListbox.Items.Clear();
            SubTotTxtbox.Clear();
            PstTxtbox.Clear();
            GstTxtbox.Clear();
            TotalTxtbox.Clear();
            //to create first list as a heading column
            InvoiceItemListbox.Items.Add(String.Format("{0,5}{1,30}{2,50}{3,60}{4,30}", "Item ID ","Item Name", "Item Description", "Item Price", "Quantity"));
                   

            decimal SubTotal = 0;//assign subtotal

            //Create and open a connection
            using (SqlConnection connection = new SqlConnection())
            {
                if (CurrentSelectedInvoice != null)
                {
                    // replace absolute file path with |DataDirectory| 
                    connection.ConnectionString = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename =|DataDirectory|\Invoices.mdf; Integrated Security = True";
                    connection.Open();

                    //Create SQL command object
                    string sqlCommand = $"Select * From InvoiceItems WHERE InvoiceID = {CurrentSelectedInvoice.InvoiceID}";   
                    SqlCommand myCommand = new SqlCommand(sqlCommand, connection);

                    using (SqlDataReader Reader = myCommand.ExecuteReader())
                    {
                        while (Reader.Read())
                        {
                            //Create new InvoiceItems Object from the record
                            InvoiceItems newInvoiceItems = new InvoiceItems((int)Reader[0], (int)Reader[1], (String)Reader[2], (string)Reader[3], (decimal)Reader[4], (int)Reader[5]);

                            //Add to list
                            InvoiceItemsList.Add(newInvoiceItems);

                            //Add to listbox
                            InvoiceItemListbox.Items.Add(newInvoiceItems);

                            //when items are loaded(loop), multiply quantity and price and add up in subtoal 
                            SubTotal += newInvoiceItems.ItemPrice * newInvoiceItems.ItemQuantity;

                            //calculate based on subtotal
                            decimal PST = SubTotal * 6 / 100;
                            decimal GST = SubTotal * 5 / 100;
                            decimal Total = SubTotal + PST + GST;

                            //display in textboxes
                            SubTotTxtbox.Text = SubTotal.ToString("C2");
                            PstTxtbox.Text = PST.ToString("C2");
                            GstTxtbox.Text = GST.ToString("C2");
                            TotalTxtbox.Text = Total.ToString("C2");
                        }

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
            //To fix first item as heading column in the invoice listbox
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
            //To fix first item as heading column int the item listbox
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
            ErrorMeassageBox.Clear();
            isNewRecord = true;
        }

        private void SaveItemBtn_Click(object sender, RoutedEventArgs e)
        {
            //User should click "add" button first to save record. 
            if (isNewRecord)
            {
                //to check if the data users put is valid. If not, return and show error message in the massage box
                if(IsItemDataValid() == false)
                {
                    ErrorMeassageBox.Text = "Invalid Information";
                    return;
                }

                SaveItems();
            }
            //If they dont' click add button, update items.
            else if (isNewRecord == false) {
                if (IsItemDataValid() == false)
                {
                    ErrorMeassageBox.Text = "Invalid Information";
                    return;
                }
                UpdateItems();
            }
            //to display updated total price, clear invoice list box and load again
            InvoiceRecListbox.Items.Clear();
            LoadInvoice();
        }

        void UpdateInvoice()
        {
            if (CurrentSelectedInvoice != null) { 
            //update the CurrentSelectedInvoice object
            CurrentSelectedInvoice.InvoiceID = Convert.ToInt32(InvoiceTxtbox.Text);
            CurrentSelectedInvoice.CustomerName = CustNameTxbox.Text;
            CurrentSelectedInvoice.InvoiceDateTime = Convert.ToDateTime(v.Text);
            CurrentSelectedInvoice.Shipping = Convert.ToBoolean(ShippedChckbox.IsChecked.Value);
            CurrentSelectedInvoice.CustomerEmail = CustEmailTxtbox.Text;
            CurrentSelectedInvoice.CustomerAddress = CustAddressTxbox.Text;

            using (SqlConnection connection = new SqlConnection())
            {
                //open connection
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
            //reload updated date and the lists
            LoadInvoice();
            }
        }

        void UpdateItems()
        {
            if (CurrentSeledtedItems != null)
            {

            
            //update the CurrentSelectedItems object
            CurrentSeledtedItems.ItemID = Convert.ToInt32(ItemIDTxtbox.Text);
            CurrentSeledtedItems.ItemName = ItemNameTxtbox.Text;
            CurrentSeledtedItems.ItemDescription = ItemDescTxtbox.Text;
            CurrentSeledtedItems.ItemPrice = Convert.ToDecimal(ItemPriceTxtbox.Text);
            CurrentSeledtedItems.ItemQuantity = Convert.ToInt32(ItemQuantTxtbox.Text);

            using (SqlConnection connection = new SqlConnection())
            {
                connection.ConnectionString = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename =|DataDirectory|\Invoices.mdf; Integrated Security = True";
                //open connection
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
            //reload updated the displayed data and the lists
            LoadItems();
            }
        }

        //to check if some information users put is empty.
        bool IsInvoiceDataValid()
        {
            if (CustNameTxbox.Text == "" || CustEmailTxtbox.Text == "" || v.Text == "")
            {
                //Place the focus on the first invalid textbox
                if (CustNameTxbox.Text == "") CustNameTxbox.Focus();
                if (CustEmailTxtbox.Text == "") CustEmailTxtbox.Focus();
                if (v.Text == "") v.Focus();
                return false;
            }

            else return true;    
        }

        //to check if some information user put is empty or invalid.
        bool IsItemDataValid()
        {
            //Check if an user input empty information
            if (ItemNameTxtbox.Text == "" || ItemPriceTxtbox.Text == "" || ItemQuantTxtbox.Text == "")
            {
                //Place the focus on the first invalid textbox
                if (ItemNameTxtbox.Text == "") ItemNameTxtbox.Focus();
                if (ItemPriceTxtbox.Text == "") ItemPriceTxtbox.Focus();
                if (ItemQuantTxtbox.Text == "") ItemQuantTxtbox.Focus();
                return false;
            }

            //Check if the number an user input is negative
            else if (Convert.ToDecimal(ItemPriceTxtbox.Text) <= 0 || (Convert.ToInt32(ItemQuantTxtbox.Text) <= 0))
            {
                //Place the focus on the first invalid textbox
                if (ItemPriceTxtbox.Text == "") ItemPriceTxtbox.Focus();
                if (ItemQuantTxtbox.Text == "") ItemQuantTxtbox.Focus();
                return false;
            }
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
                //connect database from local database and open it
                connection.ConnectionString = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename =|DataDirectory|\Invoices.mdf; Integrated Security = True";
                connection.Open();

                //to find the last primary key used (to know the maximum primary key)
                sql = $"Select MAX(InvoiceID) FROM Invoices;";

                //assign new invoice ID 
                int NewInvoiceID;
                
                //to create new primary key
                using (SqlCommand Selection = new SqlCommand(sql, connection))
                {
                    NewInvoiceID = Convert.ToInt32(Selection.ExecuteScalar()) + 1;
                    newInvoice.InvoiceID = NewInvoiceID;
                }

                //insert new data user put into database
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

                //InvoiceList.Add(newInvoice);
                InvoiceList.Add(newInvoice);
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

                //assing new item ID
                int NewItemID;

                //to create new primary key(item ID)
                using (SqlCommand Selection = new SqlCommand(sql, connection))
                {
                    NewItemID = Convert.ToInt32(Selection.ExecuteScalar()) + 1;
                    newItems.ItemID = NewItemID;
                }
                //insert new data into database
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
                //Update List<InvoiceItemsList>
                InvoiceItemsList.Add(newItems);

                //clear all data displayed and update data
                InvoiceItemListbox.Items.Clear();
                LoadItems();

                //finde index of new item
                int NewItemIndex = InvoiceItemListbox.Items.IndexOf(newItems);

                //selec the new item
                InvoiceItemListbox.SelectedIndex = NewItemIndex;
                InvoiceItemListbox.ScrollIntoView(newItems);
            }
            //close saving function until users click add button
            isNewRecord = false;
        }

        
        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            //to check if it is ready
            if (isNewRecord == true)
            {
                //if user put invalid information, return and display error message in the textbox
                if (IsInvoiceDataValid() == false)
                {
                    MessageTextBox.Text = "Invalid Information";
                    return;
                }
                SaveInvoice();
            }
            //if users don't click add button, update invoice
            else if (isNewRecord == false)
            {
                if (IsInvoiceDataValid() == false)
                {
                    MessageTextBox.Text = "Invalid Information";
                    return;
                }
                UpdateInvoice();
            }
            
        }

        private void NewBtn_Click(object sender, RoutedEventArgs e)
        {
            //to clear all textbox and be ready to save
            InvoiceTxtbox.Clear();
            CustNameTxbox.Clear();
            CustEmailTxtbox.Clear();
            CustAddressTxbox.Clear();
            MessageTextBox.Clear();
            isNewRecord = true;
        }

        void DeleteItems()
        {
            using (SqlConnection connection = new SqlConnection())
            {
                // create connection and open it
                connection.ConnectionString =
                 @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename =|DataDirectory|\Invoices.mdf; Integrated Security = True";
                connection.Open();

                //delete an item users click in listbox in data
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
            //select the item
            InvoiceItemListbox.SelectedIndex = CurrentSelectedItemIndex;
        }
        private void DeleteItemBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentSeledtedItems == InvoiceItemListbox.SelectedItem)
            {
                DeleteItems();
                //after deleting an item, reload invoice to display new total price
                InvoiceRecListbox.Items.Clear();
                LoadInvoice();

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

                //delete all items where invoiceID in invoice table equals to invoiceID in item table
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
            //Delete data in invoice user clicked
            using (SqlConnection connection = new SqlConnection())
            {
                //open a connection
                connection.ConnectionString =
                  @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename =|DataDirectory|\Invoices.mdf; Integrated Security = True";
                connection.Open();

                //delete invoice data from database
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
            
            //if last record is deleted, select remaining last record or if it's not, reselct same position in the list
            if (IndexToDelete == InvoiceList.Count)
            {
                CurrentSelectedInvoiceIndex = InvoiceList.Count - 1;
            }
            else
            {
                CurrentSelectedInvoiceIndex = IndexToDelete;
            }

            //select the invoice
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
