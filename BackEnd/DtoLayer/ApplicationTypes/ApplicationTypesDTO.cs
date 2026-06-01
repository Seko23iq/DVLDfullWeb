using System;
using System.Collections.Generic;
using System.Text;

namespace DtoLayer.ApplicationTypes
{
    public class ApplicationTypesDTO
    {
        public ApplicationTypesDTO() { }
        public ApplicationTypesDTO(int ID, string Title, decimal Fees)
        {
            this.ID = ID;
            this.Title = Title;
            this.Fees = Fees;
        }
        public int ID { get; set; }
        public string Title { get; set; }
        public decimal Fees { get; set; }
    }
}
