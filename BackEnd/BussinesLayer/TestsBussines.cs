using ContactsDataAccessLayer;
using DataLayer;
using System;
using System.Data;
using DtoLayer.Application;
using DtoLayer.Test;

namespace BussinesLayer
{
    public class TestsBussines
    {
        public static List<testAppointmentsDTO> GetAllTestAppointmentRecords(int LocalDrivingLicenseApplicationID, int TestTypeID)
        {
            return TestsData.GetAllTestAppointmentRecordsData(LocalDrivingLicenseApplicationID, TestTypeID);
        }



        public static ApplicantInformationTestDTO GetApplicantInformation(int LocalDrivingLicenseApplicationID)
        {
            return TestsData.GetApplicantInformation(LocalDrivingLicenseApplicationID);
        }
        public static int AddNewTestAppointmentRecord(AddApplicationDTO dTO)
        {
            return TestsData.AddNewTestAppointmentData(dTO);
        }



        public static TakerTestInfoDTO GetTakerTestInfoInformation(int TestAppointmentID)
        {
            return TestsData.GetTakerTestInfoInformation(TestAppointmentID);
        }

        public static int TakeTest(TakeTestDTO dTO)
        {
            return TestsData.TakeTestData(dTO);
        }


        public static bool EditTestAppointment(UpdateTestAppointmentDto dto)
        {
            return TestsData.EditTestAppointmentData(dto);
        }

















        public static bool HasTestAppointment(LocalAndTestTypeidDTO dTO)
        {
            return TestsData.HasTestAppointmentData(dTO);
        }


     

  

    }
}
