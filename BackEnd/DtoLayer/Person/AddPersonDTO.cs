using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DtoLayer.Person
{
    public class AddPersonDTO
    {
        public int PersonID { set; get; }

        [Required(ErrorMessage = "National Number is required")]
        [StringLength(20, ErrorMessage = "National Number cannot exceed 20 characters")]
        public string NationalNo { set; get; }

        [Required(ErrorMessage = "First Name is required")]
        [StringLength(50)]
        public string FirstName { set; get; }

        [Required(ErrorMessage = "Second Name is required")]
        [StringLength(50)]
        public string SecondName { set; get; }

        [StringLength(50)]
        public string ThirdName { set; get; }

        [Required(ErrorMessage = "Last Name is required")]
        [StringLength(50)]
        public string LastName { set; get; }

        [Required(ErrorMessage = "Date of Birth is required")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { set; get; }

        [Required(ErrorMessage = "Gender is required")]
        [RegularExpression("^(Male|Female)$", ErrorMessage = "Gender must be 'Male' or 'Female'")]
        public string Gender { set; get; }

        [Required(ErrorMessage = "Address is required")]
        [StringLength(500)]
        public string Address { set; get; }

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid Phone Number format")]
        public string Phone { set; get; }

        [EmailAddress(ErrorMessage = "Invalid Email Address format")]
        [StringLength(100)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Nationality is required")]
        public int Nationality { set; get; }

        // مسار الصورة غالباً يكون اختيارياً
        public string ImagePath { set; get; }

        // إضافة Constructor فارغ ضروري لعمل الـ Deserialization (تحويل JSON إلى Object)
        public AddPersonDTO() { }

        public AddPersonDTO(int PersonID, string NationalNo, string FirstName,
            string SecondName, string ThirdName, string LastName, DateTime DateOfBirth,
            string Gender, string Address, string Phone, string Email, int Nationality, string ImagePath)
        {
            this.PersonID = PersonID;
            this.NationalNo = NationalNo;
            this.FirstName = FirstName;
            this.SecondName = SecondName;
            this.ThirdName = ThirdName;
            this.LastName = LastName;
            this.DateOfBirth = DateOfBirth;
            this.Gender = Gender;
            this.Address = Address;
            this.Phone = Phone;
            this.Email = Email;
            this.Nationality = Nationality;
            this.ImagePath = ImagePath;
        }
    }
}
