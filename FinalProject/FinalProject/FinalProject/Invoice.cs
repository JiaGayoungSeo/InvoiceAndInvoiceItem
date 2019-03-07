using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProject
{
    class Invoice
    {
        private int _InvoiceID;
        private DateTime _InvoiceDateTime;
        private bool _Shipping;
        private String _CustomerName;
        private String _CustomerAddres;
        private String _CustomerEmail;
        

        public Invoice()
        {

        }

        public Invoice(int InvoiceID, DateTime InvoiceDateTime, String CustomerName, String CustomerEmail, String CustomerAddress, bool Shipping)
        {
            this.InvoiceID = InvoiceID;
            this.InvoiceDateTime = InvoiceDateTime;
            this.Shipping = Shipping;
            this.CustomerName = CustomerName;
            this.CustomerEmail = CustomerEmail;
            this.CustomerAddress = CustomerAddress;
        }

        //public Invoice(int InvoiceID, String CustomerName, String CustomerEmail, bool Shipping)
        //{
        //    this.InvoiceID = InvoiceID;
        //    this.CustomerName = CustomerName;
        //    this.CustomerEmail = CustomerEmail;
        //    this.Shipping = Shipping;
        //}

        public int InvoiceID
        {
            get
            {
                return _InvoiceID;
            }
            set
            {
                _InvoiceID = value;
            }
        }
        public DateTime InvoiceDateTime
        {
            get
            {
                return _InvoiceDateTime;
            }
            set
            {
                _InvoiceDateTime = value;
            }
        }

        public bool Shipping
        {
            get
            {
                return _Shipping;
            }
            set
            {
                _Shipping = value;
            }
        }

       public string CheckShipping(bool Shipping)
        {
            if (Shipping == true) return "yes";              
            else return "no";
        }
        

        public String CustomerName
        {
            get
            {
                return _CustomerName;
            }
            set
            {
                _CustomerName = value;
            }
        }

        public String CustomerAddress
        {
            get
            {
                return _CustomerAddres;
            }
            set
            {
                _CustomerAddres = value;
            }
        }

        public String CustomerEmail
        {
            get
            {
                return _CustomerEmail;
            }
            set
            {
                _CustomerEmail = value;
            }
        }

        public decimal _TotalPrice
        {
            get
            {
                decimal Total = 0;
                SqlConnection connection = new SqlConnection();
                connection.ConnectionString = @"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename =|DataDirectory|\Invoices.mdf; Integrated Security = True";
                connection.Open();
                string sql = $"SELECT * FROM InvoiceItems WHERE InvoiceID = {this.InvoiceID}";
                SqlCommand myCommand = new SqlCommand(sql, connection);
                SqlDataReader myReader = myCommand.ExecuteReader();
                while (myReader.Read())
                {
                    Total += (decimal)myReader["ItemPrice"] * (int)myReader["ItemQuantity"];
                }

                return Total;
                
            }
        }
        public override string ToString() => $"{_InvoiceID,6}{"",15}{_CustomerName,-25}{"",15}{_CustomerEmail,-25}{"",15}{_TotalPrice,-35:C2}{"",30}{CheckShipping(Shipping),5}";

       

    }
}
