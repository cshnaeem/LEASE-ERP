using System;
namespace AGEERP.CrReports
{
    
    public class InstallmentRVM 
    {
        public string Location { get; set; }
        public long AccNo { get; set; }
        public DateTime DeliveryDate { get; set; }
        public long ReceiptNo { get; set; }
        public DateTime InstDate { get; set; }
        public string Customer { get; set; }
        public string FName { get; set; }
        public string RecoveryOff { get; set; }
        public decimal Fine { get; set; }
        public string FineType { get; set; }
        public int Duration { get; set; }
        public decimal RecvInst { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal Advance { get; set; }
        public decimal RecvInstAmt { get; set; }
        public decimal InstCharges { get; set; }
        public decimal Discount { get; set; }
        public string Username { get; set; }
        public decimal PreBalance { get; set; }
        public decimal Balance { get; set; }

    }
    public class CustomerDetailsRVM
    {
        public long AccNo { get; set; }
        public string AccDate { get; set; }
        public string CustCode { get; set; }
        public string CurrentDate { get; set; }

        public string ProcessNo { get; set; }
        public string ProcessDate { get; set; }
        public string Location { get; set; }
        public string CustName { get; set; }
        public string SO { get; set; }
        public string Occupation { get; set; }
        public string ResAddress { get; set; }
        public string OffAddress { get; set; }
        public string ImgCustomer { get; set; }
        public string SignCustomer { get; set; }
        public string PreACNo { get; set; }
        public string Mobile { get; set; }
        public string ResTel { get; set; }
        public string OffTel { get; set; }
        public string NIC { get; set; }
        public string CompName { get; set; }
        public string ItemName { get; set; }
        public string Model { get; set; }
        public string jointItems { get; set; }
        public string jointOfficers { get; set; }
        public string jointMobile { get; set; }
        public string jointAddress { get; set; }
        public string SerialNo { get; set; }
        public string MangName { get; set; }
        public string InqOfficerName { get; set; }
        public string InspectorName { get; set; }
        public string ProcessAt { get; set; }
        public string ProcessFee { get; set; }
        public string TPrice { get; set; }
        public string Duration { get; set; }
        public string Advance { get; set; }
        public string ActualAdvance { get; set; }
        public string MonthlyInstallment { get; set; }
        public string InstCharges { get; set; }
        public string InstReceive { get; set; }
        public string InstRemaining { get; set; }
        public string TotalReceive { get; set; }
        public string DuePayment { get; set; }
        public string Balance { get; set; }
        public string Status { get; set; }
        public string CloseDate { get; set; }
        public string FineTime { get; set; }
        public string Cheque { get; set; }
        public string NoOfCheques { get; set; }
        public string ChequeType { get; set; }
        public string ChequeNo { get; set; }
        public string ChequeAmount { get; set; }
        public string Pending { get; set; }
        public string Gender { get; set; }
        public string WrantyCard { get; set; }
        public string Affidavit { get; set; }
        public string Worth { get; set; }
        public Nullable<decimal> Salary { get; set; }
        public string Residential { get; set; }
        public string Defaulter { get; set; }
        public string VPNStatus { get; set; }
        public string RepAsCust { get; set; }
        public string RepAsGrun { get; set; }
        public string Remarks { get; set; }
        public string G1Name { get; set; }
        public string G1SO { get; set; }
        public string G1TelRes { get; set; }
        public string G1TelOff { get; set; }
        public string G1NIC { get; set; }
        public string G1ResAddress { get; set; }
        public string G1OffAddress { get; set; }

        public string G1Occupation { get; set; }
        public string G2Name { get; set; }
        public string G2SO { get; set; }
        public string G2TelRes { get; set; }
        public string G2TelOff { get; set; }
        public string G2NIC { get; set; }
        public string G2ResAddress { get; set; }
        public string G2OffAddress { get; set; }
        public string G2Occupation { get; set; }

        public string G3Name { get; set; }
        public string G3SO { get; set; }
        public string G3TelRes { get; set; }
        public string G3TelOff { get; set; }
        public string G3NIC { get; set; }
        public string G3ResAddress { get; set; }
        public string G3OffAddress { get; set; }
        public string G3Occupation { get; set; }

        public string G4Name { get; set; }
        public string G4SO { get; set; }
        public string G4TelRes { get; set; }
        public string G4TelOff { get; set; }
        public string G4NIC { get; set; }
        public string G4ResAddress { get; set; }
        public string G4OffAddress { get; set; }
        public string G4Occupation { get; set; }

        public string G1Relation { get; set; }
        public string G2Relation { get; set; }
        public string G3Relation { get; set; }
        public string G4Relation { get; set; }
        public string GroupID { get; set; }
        public string GroupName { get; set; }
        public string Category { get; set; }
        public string ReportSelect { get; set; }
        public string UName { get; set; }
        public string TName { get; set; }
        public string TIP { get; set; }
        public string IndexNo { get; set; }


    }
    public class CustomerDetailRVM
    {
        public long AccNo { get; set; }
        public int OldAccNo { get; set; }
        public string thumbImg { get; set; }
        public string picImg { get; set; }
        public DateTime TransDate { get; set; }
        public decimal ProcessFee { get; set; }
        public long CustId { get; set; }
        public string Customer { get; set; }
        public string FName { get; set; }
      
        public int RepeatCus { get; set; }
        public int RepeatGuar { get; set; }
        public string DBMRemarks { get; set; }
        public string ResidentialStatus { get; set; }
        public string Occupation { get; set; }
        public string ResAddress { get; set; }
        public string OffAddress { get; set; }
        public string Mobile1 { get; set; }
        public string Mobile2 { get; set; }
        public string NIC { get; set; }
        public string Gender { get; set; }
        public decimal InstPrice { get; set; }
        public decimal MonthlyInst { get; set; }
        public decimal Advance { get; set; }
        public int ActualAdvance { get; set; }
        public int TotalRecv { get; set; }
        public int Discount { get; set; }
        public int Balance { get; set; }
        public int FineRecv { get; set; }
        public int NoOfFine { get; set; }
        public int NoOfFineExempt { get; set; }
        public string Company { get; set; }
        public string Product { get; set; }
        public string Model { get; set; }
        public string SerialNo { get; set; }
        public int Duration { get; set; }
        public int NoOfInstRec { get; set; }
        public int NoOfInstRem { get; set; }
        public string Status { get; set; }
        public string Manager { get; set; }
        public string SManager { get; set; }
        public string Marketing { get; set; }
        public string Inquiry { get; set; }
        public string SRM { get; set; }
        public string RM { get; set; }
        public string CRC { get; set; }
        public string ProcessAt { get; set; }
        public string Affidavit { get; set; }
        public string Worth { get; set; }
        public decimal Salary { get; set; }
        public string Defaulter { get; set; }
        public string Remarks { get; set; }
        public string Username { get; set; }
        public string SKUCode { get; set; }
        public decimal ProductPrice { get; set; }
        public string PTO { get; set; }
        public string SearchStatus { get; set; }
        public DateTime OutstandDate { get; set; }
        public string Category { get; set; }
        public string RecOfficer { get; set; }
        public int OutstandAmt { get; set; }
        public int RecvAmt { get; set; }
        public int DueAmt { get; set; }
        public string MonthStatus { get; set; }
        public string PrevAcc { get; set; }
        public string LocName { get; set; }
        public string LocCode { get; set; }
        public string Column1 { get; set; }
        public string CRCRemarks { get; set; }
        public string EmployeeStatus { get; set; }
        public string G1Name { get; set; }
        public string G1SO { get; set; }
        public string G1TelRes { get; set; }
        public string G1TelOff { get; set; }
        public string G1NIC { get; set; }
        public string G1ResAddress { get; set; }
        public string G1OffAddress { get; set; }

        public string G1Occupation { get; set; }
        public string G2Name { get; set; }
        public string G2SO { get; set; }
        public string G2TelRes { get; set; }
        public string G2TelOff { get; set; }
        public string G2NIC { get; set; }
        public string G2ResAddress { get; set; }
        public string G2OffAddress { get; set; }
        public string G2Occupation { get; set; }

        public string G3Name { get; set; }
        public string G3SO { get; set; }
        public string G3TelRes { get; set; }
        public string G3TelOff { get; set; }
        public string G3NIC { get; set; }
        public string G3ResAddress { get; set; }
        public string G3OffAddress { get; set; }
        public string G3Occupation { get; set; }

        public string G4Name { get; set; }
        public string G4SO { get; set; }
        public string G4TelRes { get; set; }
        public string G4TelOff { get; set; }
        public string G4NIC { get; set; }
        public string G4ResAddress { get; set; }
        public string G4OffAddress { get; set; }
        public string G4Occupation { get; set; }

        public string G1Relation { get; set; }
        public string G2Relation { get; set; }
        public string G3Relation { get; set; }
        public string G4Relation { get; set; }
    }

    public class IssueRVM
    {
        public string LocCode { get; set; }
    }
    public class SalesReportRVM
    {
        public string EasyName { get; set; }
        public string CompName { get; set; }
        public string ItemName { get; set; }
        public string Model { get; set; }
        public decimal SQty { get; set; }
        public decimal RQty { get; set; }
        public decimal AQty { get; set; }
        public decimal SPrice { get; set; }
    }
    public class PurchaseReportRVM
    {
        public string Company { get; set; }
        public string ItemName { get; set; }
        public string Model { get; set; }
        public string SRCode { get; set; }
        public string City { get; set; }
        public decimal Qty { get; set; }
        public decimal Amount { get; set; }

    }

    public class StockReportRVM
    {
        public string EasyName { get; set; }
        public string SuppName { get; set; }
        public string CompName { get; set; }
        public string ItemName { get; set; }
        public string TypeName { get; set; }
        public string Model { get; set; }
        public string SrNo { get; set; }
        public int Qty { get; set; }
        public decimal PPrice { get; set; }
        public DateTime AgeDate { get; set; }
        public int AgeDays { get; set; }
        public string City { get; set; }
    }
}