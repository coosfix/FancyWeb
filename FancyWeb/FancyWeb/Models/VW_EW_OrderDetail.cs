//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace FancyWeb.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class VW_EW_OrderDetail
    {
        public int OrderID { get; set; }
        public string ProductName { get; set; }
        public string ColorName { get; set; }
        public string SizeName { get; set; }
        public Nullable<int> OrderQTY { get; set; }
        public int StockQTY { get; set; }
        public Nullable<int> Balance { get; set; }
        public int ProductID { get; set; }
        public int ProductColorID { get; set; }
        public int ProductSizeID { get; set; }
        public int UnitPrice { get; set; }
        public Nullable<int> Amount { get; set; }
    }
}
