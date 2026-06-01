using System;
using System.Collections.Generic;
using System.Text;

namespace DtoLayer.TestTypes
{
    public class TestTypesDTO
    {
        public TestTypesDTO(int ID, string Title, string Description, decimal Fees)
        {
            this.ID = ID;
            this.Title = Title;
            this.Description = Description;
            this.Fees = Fees;
        }
        public int ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Fees { get; set; }
    }
}
