//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace UP_02_Patrakov
{
    using System;
    using System.Collections.Generic;
    
    public partial class PartnerProducts
    {
        public int ID { get; set; }
        public Nullable<int> productID { get; set; }
        public Nullable<int> partnerID { get; set; }
        public Nullable<int> quantityProduct { get; set; }
        public Nullable<System.DateTime> dateSell { get; set; }
    
        public virtual Partners Partners { get; set; }
        public virtual Product Product { get; set; }
    }
}
