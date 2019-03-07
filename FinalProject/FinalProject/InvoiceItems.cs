using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProject
{
    class InvoiceItems
    {
        private int _ItemID;
        private String _ItemName;
        private decimal _ItemPrice;
        private int _ItemQuantity;
        private String _ItemDescription;
        private int _InvoiceId;
        public InvoiceItems(int itemID, int InvoiceId, String itemName, String description, decimal itemPrice, int itemQuantity)
        {
            this._ItemID = itemID;
            this._ItemName = itemName;
            this._ItemDescription = description;
            this._ItemPrice = itemPrice;
            this._ItemQuantity = itemQuantity;
            this._InvoiceId = InvoiceId;
        }
        public InvoiceItems() { }

        public int ItemID
        {
            get
            {
                return _ItemID;
            }

            set
            {
                _ItemID = value;
            }
        }

        public String ItemName
        {
            get
            {
                return _ItemName;
            }
            set
            {
                _ItemName = value;
            }
        }

        public decimal ItemPrice
        {
            get
            {
                return _ItemPrice;
            }
            set
            {
                _ItemPrice = value;
            }
        }

        public int ItemQuantity
        {
            get
            {
                return _ItemQuantity;
            }
            set
            {
                _ItemQuantity = value;
            }
        }

        public String ItemDescription
        {
            get
            {
                return _ItemDescription;
            }
            set
            {
                _ItemDescription = value;
            }
        }

        public int InvoiceId
        {
            get
            {
                return _InvoiceId;
            }

            set
            {
                _InvoiceId = value;
            }
        }

        public override string ToString() => $"{_ItemID,-15}\t{_ItemName,-40}\t{_ItemDescription,-60}\t{_ItemPrice,-30:C2}\t{_ItemQuantity,-14}";
    
    }
}
