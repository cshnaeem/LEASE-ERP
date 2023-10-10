using AGEERP.CrReports;
using AGEERP.Models;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AGEERP.Models
{
    public class ViewModel
    { }

    #region Salary
    public class ConditionVM
    {
        public string Sign { get; set; }
    }
    public class PaySalaryIncentiveVM
    {
        public long TIId { get; set; }
        public string DisbursementType { get; set; }
        public string LocCode { get; set; }
        public string Designation { get; set; }
        public string Employee { get; set; }
        public string Location { get; set; }
        public bool selectable { get; set; }

        public int EmpId { get; set; }
        public string EmpName { get; set; }
        public string DesgName { get; set; }
        public decimal IncAmount { get; set; }
        public decimal UsedInClosing { get; set; }
        public Nullable<decimal> IncPayable { get; set; }
        public Nullable<decimal> ApprovedPayable { get; set; }
        public Nullable<int> ApprovedBy { get; set; }
        public Nullable<System.DateTime> ApprovedDate { get; set; }
        public Nullable<bool> AllowDisbursement { get; set; }
    }
    public class SalaryDisbursementTypeVM
    {
        public int DisbursementTypeId { get; set; }
        public string DisbursementType { get; set; }
        public bool IsActive { get; set; }
    }


    public class SalaryControlPanelVM
    {
        public int PanelId { get; set; }
        public System.DateTime SalaryMonth { get; set; }
        public int HDeptId { get; set; }
        public int? CityId { get; set; }
        public int? DeptId { get; set; }
        public int? DesignationId { get; set; }
        public int? SectionId { get; set; }
        public int DisbursementTypeId { get; set; }
        public bool AllowDisbursement { get; set; }
        public bool AllowFinalized { get; set; }
        public System.DateTime DisbursementStartDate { get; set; }
        public Nullable<System.DateTime> DisbursementEndDate { get; set; }
        public int DefinedBy { get; set; }
        public System.DateTime DefinedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    }
    public class SalaryControlPanelListVM
    {
        public int PanelId { get; set; }
        public System.DateTime SalaryMonth { get; set; }
        public System.DateTime DisbursementStartDate { get; set; }
        public Nullable<System.DateTime> DisbursementEndDate { get; set; }
        public string HDeptName { get; set; }
        public string City { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public string Section { get; set; }
        public string AllowDisbursement { get; set; }
        public string AllowFinalized { get; set; }
    }
    public class TransStatus
    {
        public bool Status { get; set; }
        public string Msg { get; set; }
    }
    public class EmpSalaryDataVM
    {
        public int EmpId { get; set; }
        public string EmpName { get; set; }
        public string CNIC { get; set; }
        public string Msg { get; set; }
        public string Title { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        public string Location { get; set; }

        public List<EmpSalaryVM> EmpSal { get; set; }
    }

    public class EmpSalaryVM
    {
        public string Type { get; set; }
        public string Title { get; set; }
        public Nullable<decimal> Amount { get; set; }
    }
    #endregion

    #region Dashboard
    public class DashboardCloudVM
    {

        public string weight { get; set; }
        public string text { get; set; }
        public string color { get; set; }
    }
    public class DashboardVM
    {
        public string Title { get; set; }
        public string Link { get; set; }
        public string LinkCode { get; set; }
        public string ImgPath { get; set; }
    }
    #endregion



    public class CRCPolicyVM
    {
        public int PolicyId { get; set; }
        public string PolicyCode { get; set; }
        public string PolicyDetail { get; set; }
        public string ShortDesc { get; set; }
        public System.DateTime EffectiveFrom { get; set; }
        public int DefinedBy { get; set; }
        public System.DateTime DefinedDate { get; set; }
        public Nullable<int> ApprovedBy { get; set; }
        public Nullable<System.DateTime> ApprovedDate { get; set; }
        public bool Status { get; set; }
        public string Designation { get; set; }
        public decimal FineAmount { get; set; }
    }
    public class CRCPolicyDtlVM
    {
        public int PolicyId { get; set; }
        public int PolicyDtlId { get; set; }
        public int DesgId { get; set; }
        public decimal FineAmt { get; set; }
    }
    public class BirthdayVM
    {
        public bool IsBirthday = false;
        public bool IsWorkAnniversary = false;
    }
    public class ReasonVM
    {
        public int ReasonId { get; set; }
        public string Reason { get; set; }
    }
    public class LSECRCFinePolicy
    {
        public int PolicyId { get; set; }
        public string PolicyCode { get; set; }
        public string PolicyDetail { get; set; }
        public string DesgName { get; set; }
        public string ShortDesc { get; set; }
        public System.DateTime EffectiveFrom { get; set; }
        public int DefinedBy { get; set; }
        public System.DateTime DefinedDate { get; set; }
        public Nullable<int> ApprovedBy { get; set; }
        public Nullable<System.DateTime> ApprovedDate { get; set; }
        public bool Status { get; set; }
        public decimal Amount { get; set; }

    }
    public class BranchLeaveVM
    {
        public string DeptName { get; set; }
        public string DesgName { get; set; }
        public int EmpId { get; set; }
        public long LeaveId { get; set; }
        public string EmpName { get; set; }
        public string LeaveType { get; set; }
        public System.DateTime LeaveFromDate { get; set; }
        public System.DateTime LeaveToDate { get; set; }
        public string LeaveReason { get; set; }
    }
    public class ProductIncTime
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
    public class AttendanceDaysVM
    {
        public int EmpId { get; set; }
        public int Days { get; set; }
    }

    public class SalemanProductInfo
    {
        public string productname { get; set; }
        public decimal smprice { get; set; }
        public string company { get; set; }
        public string SKU { get; set; }
        public string image { get; set; }
        public string model { get; set; }
    }

    public class Poresturndata
    {
        public decimal PPrice { get; set; }
        public int? SuppId { get; set; }
        public long ItemId { get; set; }
        public int SkuId { get; set; }
        public string SerialNo { get; set; }

    }

    public class CategoryVM
    {
        [Key]
        public int CategoryId { get; set; }
        public string Category { get; set; }
        public bool Status { get; set; }
    }
    public class LocVM
    {
        public int LocId { get; set; }
        public string LocName { get; set; }
        public bool selectable { get; set; }
        public bool dirty { get; set; }
    }
    public class BranchClosingSearchVM
    {
        public DateTime ClosingMonth { get; set; }
        public int BranchId { get; set; }
    }

    public class BranchClosingPostVM
    {
        public int TransId { get; set; }
        public Nullable<System.DateTime> IsClosed { get; set; }
    }
    public class HolidayDetailVM
    {
        public int RowId { get; set; }
        public int HId { get; set; }
        public int LocId { get; set; }
    }
    public class HolidayMasterVM
    {
        public int RowId { get; set; }
        public int HolidayId { get; set; }
        public string Type { get; set; }
        public System.DateTime StartDate { get; set; }
        public System.DateTime EndDate { get; set; }
        public string Remarks { get; set; }
        public int UserId { get; set; }
        public System.DateTime TransDate { get; set; }
        public List<int> LocId { get; set; }
    }
    public class HolidayVM
    {
        public int RowId { get; set; }
        public string Holiday { get; set; }
        public bool Status { get; set; }
    }
    public class BranchMonthlyClosingVM
    {
        public string CityCode { get; set; }
        public int LocId { get; set; }
        public string LocCode { get; set; }
        public Nullable<decimal> Outstand_F { get; set; }
        public Nullable<decimal> Outstand_R { get; set; }
        public Nullable<decimal> AIC_CMonth { get; set; }
        public Nullable<decimal> AIC_Adj { get; set; }
        public Nullable<decimal> Sale_Target_BR { get; set; }
        public Nullable<decimal> INC_Adj { get; set; }
        public Nullable<decimal> Sale_Target_BM { get; set; }
        public Nullable<decimal> INC_Adj_BM { get; set; }
        public Nullable<decimal> Salary { get; set; }
        public Nullable<decimal> STAFF_Adj { get; set; }
        public Nullable<System.DateTime> IsClosed { get; set; }
    }
    public class IMEIUploaderVM
    {
        public int Id { get; set; }
        public string IMEI { get; set; }
    }

    public class ProductIncPeriod
    {
        public int RowId { get; set; }
        public string Month { get; set; }
        public int Week { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }


    public class ProductIncApprovalVM
    {
        public bool selectable { get; set; }
        public long? RowId { get; set; }
        public int Salesman { get; set; }
        public int LocId { get; set; }
        public int Period { get; set; }
        public string EmpName { get; set; }
        public string LocName { get; set; }
        public Nullable<decimal> CashInc { get; set; }
        public Nullable<decimal> InstInc { get; set; }
        public Nullable<decimal> TotalInc { get; set; }
        public Nullable<int> ApprovedBy { get; set; }
        public Nullable<System.DateTime> ApprovedDate { get; set; }
    }

    #region Security
    public class LoginVM
    {
        public string Username { get; set; }
        public string Password { get; set; }
        //public bool IsRemember { get; set; }
        public string ThemeSel { get; set; }
        public string MacAddress { get; set; }
        public string IpAddress { get; set; }

    }
    public class ForgotPasswordVM
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [StringLength(12, MinimumLength = 12)]
        public string MobileNo { get; set; }
        public string DeviceId { get; set; }
        //public bool IsRemember { get; set; }
        //public string ThemeSel { get; set; }
        //public string MacAddress { get; set; }
        //public string IpAddress { get; set; }
    }

    public class MenuObjectVM
    {
        public int ObjectId { get; set; }
        public int MenuId { get; set; }
        public string ObjectName { get; set; }
        public string ObjectAction { get; set; }
    }

    public class UsersGroupVM
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public List<int> ActivityIds { get; set; }
    }
    public class UserInfoVM
    {
        public int UserId { get; set; }
        public int GroupId { get; set; }
        public string FullName { get; set; }
        public string ThemeName { get; set; }
    }
    public class ChangePasswordVM
    {
        //public int UserId { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        public string CurrentPassword { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        public string NewPassword { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [Compare("NewPassword", ErrorMessage = "New Password does not match.")]
        public string ChangePassword { get; set; }
    }
    public class UserVM
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public int GroupId { get; set; }
        public System.DateTime TransDate { get; set; }
        public bool Status { get; set; }
        public string SessionId { get; set; }
        public int EmployeeId { get; set; }
        public bool IsFirstLogin { get; set; }
        public Nullable<System.DateTime> LastLoginDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }

        public bool SendSMS { get; set; }

        public string DeviceId { get; set; }

        public bool SendEmail { get; set; }
        public int LocId { get; set; }
        public string msg { get; set; }
    }


    public class UserMenuInfo
    {
        public int MenuId { get; set; }
        public string Name { get; set; }
        public string ActionLink { get; set; }
        public int ParentId { get; set; }
        public int MenuLevel { get; set; }
        public int SortOrder { get; set; }
        public string MenuIcon { get; set; }
        public int Menu2 { get; set; }
    }
    #endregion
    public class UserHubModels
    {
        public string UserName { get; set; }
        public dynamic Name { get; set; }
        public dynamic Branch { get; set; }
        public HashSet<string> ConnectionIds { get; set; }
    }
    public class LseMasterVM
    {
        public long AccNo { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string CustName { get; set; }
        public string NIC { get; set; }
        public string Mobile1 { get; set; }
        public string Remarks { get; set; }

    }
    public class PayEmpStatusLogVM
    {
        public int RowId { get; set; }
        public string StatusId { get; set; }
        public int EmpId { get; set; }
        public int LocId { get; set; }
        public int DesgId { get; set; }
        public System.DateTime TransDate { get; set; }
        public int UserId { get; set; }
        public bool FaceAllow { get; set; }
        public bool Finalized { get; set; }
        public bool BlockSalary { get; set; }
        public bool ManualSalary { get; set; }
        public bool AttendanceStatus { get; set; }
        public string Remarks { get; set; }
    }
    public class SalaryDisbursementLogVM
    {
        public int TransId { get; set; }
        public Nullable<System.DateTime> SalaryMonth { get; set; }
        public Nullable<int> EmpId { get; set; }
        public string Exception { get; set; }
        public Nullable<System.DateTime> TransDate { get; set; }
        public Nullable<int> UserId { get; set; }
        public Nullable<int> DisbursementTypeId { get; set; }
        public string DisbursementSource { get; set; }
    }

    public class ProductIncentiveCalcSearchVM
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int CityId { get; set; }
    }

    public class ProductIncDetailVM
    {
        public int LocId { get; set; }
        public string LocCode { get; set; }
        public int Salesman { get; set; }
        public string EmpName { get; set; }
        public string DesgName { get; set; }
        public string BillNo { get; set; }
        public System.DateTime BillDate { get; set; }
        public string ComName { get; set; }
        public string Model { get; set; }
        public string SKUCode { get; set; }
        public string SerialNo { get; set; }
        public decimal SPrice { get; set; }
        public decimal Incentive { get; set; }
        public string PolicyTitle { get; set; }
        public System.DateTime FromDate { get; set; }
        public System.DateTime ToDate { get; set; }
        public string SType { get; set; }
    }

    public class ProductIncVM
    {
        public int ProcessId { get; set; }
        public System.DateTime ProcessDate { get; set; }
        public System.DateTime FromDate { get; set; }
        public System.DateTime ToDate { get; set; }
        public int CityId { get; set; }
        public int ProcessBy { get; set; }
        public string City { get; set; }
        public string User { get; set; }
        public System.DateTime TransactionDate { get; set; }
    }

    public partial class PayProductIncInstVM
    {
        public int ProcessId { get; set; }
        public long RowId { get; set; }
        public int LocId { get; set; }
        public string BillNo { get; set; }
        public System.DateTime BillDate { get; set; }
        public int Salesman { get; set; }
        public int TypeId { get; set; }
        public int ModelId { get; set; }
        public int SkuId { get; set; }
        public int ItemId { get; set; }
        public string SerialNo { get; set; }
        public decimal SPrice { get; set; }
        public int Qty { get; set; }
        public decimal Incentive { get; set; }
        public int PolicyId { get; set; }
        public long AccNo { get; set; }
        public long DtlId { get; set; }

        public virtual Pay_ProductInc Pay_ProductInc { get; set; }
    }

    public class ProductIncPolicyVM
    {
        public int PolicyId { get; set; }
        public string PolicyTitle { get; set; }
        public System.DateTime PolicyStartDate { get; set; }
        public System.DateTime PolicyExpiryDate { get; set; }
        public bool PolicyStatus { get; set; }
        public string SaleType { get; set; }
        public string Remarks { get; set; }
        public List<int> Cities { get; set; }
        public List<int> Locations { get; set; }
        public int DefinedBy { get; set; }
        public System.DateTime DefinedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    }

    public partial class PayProductIncDetailVM
    {
        public string Location { get; set; }
        public string Saleman { get; set; }
        public string AccNo { get; set; }
        public DateTime DDate { get; set; }
        public string Company { get; set; }
        public string Model { get; set; }
        public string SerialNo { get; set; }
        public Decimal SPrice { get; set; }
        public Decimal Incentive { get; set; }
        public string Type { get; set; }
    }

    public partial class PayProductIncIMEIVM
    {
        public long TransId { get; set; }
        public string IMEI { get; set; }
        public bool isActive { get; set; }
        public int LoadBy { get; set; }
        public System.DateTime LoadDate { get; set; }
    }

    public class ProductIncPolicyLocationsVM
    {
        public long TransId { get; set; }
        public int PolicyId { get; set; }
        public int CityId { get; set; }
        public int LocId { get; set; }

    }
    public class ProductIncPolicyDetailVM
    {
        public long PolicyDtlId { get; set; }
        public int CompanyId { get; set; }
        public int ProductId { get; set; }
        public int PolicyId { get; set; }
        public int ProductClassId { get; set; }

        public int TypeId { get; set; }
        public int ModelId { get; set; }
        public Nullable<decimal> SalePrice { get; set; }
        public string Condition { get; set; }
        public Nullable<int> SkuId { get; set; }
        public string SerialNo { get; set; }
        public decimal IncAmount { get; set; }
        public int IncQty { get; set; }
    }


    public class LseAccountVM
    {
        public long AccNo { get; set; }
        public decimal InstPrice { get; set; }
        public bool IsLocked { get; set; }
    }
    public class CompLocationIPVM
    {
        public int LocId { get; set; }
        public string LocCode { get; set; }
        public int RowId { get; set; }
        public string LocName { get; set; }
        public string IP { get; set; }
        public bool Status { get; set; }
        public string ToIP { get; set; }
    }

    #region Setup
    public class ProductClassVM
    {
        public int ProductClassId { get; set; }
        public string ProductClass { get; set; }
        public int ProductId { get; set; }
        public bool Status { get; set; }
    }
    public class CompanyVM
    {
        public int ComId { get; set; }
        [Required]
        [MinLength(2, ErrorMessage = "Minimum Length 2")]
        [MaxLength(50)]
        public string ComName { get; set; }
        [Required]
        [MinLength(2)]
        [MaxLength(3)]
        public string ComCode { get; set; }
        public bool Status { get; set; }
    }
    public class ProductVM
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string ProductCode { get; set; }
        public string PCTCode { get; set; }
        public bool Status { get; set; }
    }
    public class TypeVM
    {
        public int TypeId { get; set; }
        public string Name { get; set; }
        public int ComId { get; set; }
        public int ProductId { get; set; }
        public bool Status { get; set; }
    }
    public class ModelVM
    {
        public int ModelId { get; set; }
        public string Model { get; set; }
        public int TypeId { get; set; }
        public string Company { get; set; }
        public string Product { get; set; }
        public int? ProductClassId { get; set; }
        public bool Status { get; set; }

    }
    public class SKUPairVM
    {
        public int SKUId { get; set; }
        //public int PairId { get; set; }
        public bool AvailableForSale { get; set; }
        public bool Type { get; set; }
    }
    public class SchSKUVM
    {
        public int SKUId { get; set; }
        public string SKUCode { get; set; }
        public string Company { get; set; }
        public string Product { get; set; }
        public long PODtlId { get; set; }
        public int OrderQty { get; set; }
        public long SchQty { get; set; }
        public long RecvQty { get; set; }
    }
    public class SKUVM
    {
        public int SKUId { get; set; }
        public string SKUCode { get; set; }
        public string SKUName { get; set; }
        public int ModelId { get; set; }
        public int ColorId { get; set; }
        public string Color { get; set; }
        public string Specs { get; set; }
        public string BarcodeValue { get; set; }
        public string Description { get; set; }
        public string Model { get; set; }
        public string Company { get; set; }
        public string Product { get; set; }
        public string InstPlanId { get; set; }
        public List<int> PairLst { get; set; }
        public bool IsPaired { get; set; }
        public bool AvailableForSale { get; set; }
        public bool AvailableForPurchase { get; set; }
        public int Capacity { get; set; }
        public bool Status { get; set; }
        public string Barcodes { get; set; }
    }
    public class ColorVM
    {
        public int ColorId { get; set; }
        public string ColorCode { get; set; }
        public string ColorName { get; set; }
    }
    public class SupplierVM
    {
        public int SuppId { get; set; }
        public string SuppName { get; set; }
        public int CategoryId { get; set; }
    }
    public class SupplierDetailVM
    {
        public int SuppId { get; set; }
        public string SuppName { get; set; }
        public string ContactPerson { get; set; }
        public string Address { get; set; }
        public string PhoneOff { get; set; }
        public string PhoneRes { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string NTN { get; set; }
        public string STRN { get; set; }
        public decimal TaxRate { get; set; }
        public int CategoryId { get; set; }
        public int PaymentTermId { get; set; }
        public string GLCode { get; set; }
        public decimal WHT { get; set; }
        public decimal GST { get; set; }
        public List<int> CompanyLst { get; set; }
        public List<int> CityLst { get; set; }
        public string TaxAppliedOn { get; set; }
        public bool IsReg { get; set; }
        public string RegisteredName { get; set; }
        public string RegisteredAddress { get; set; }
        public string CompanyName { get; set; }
    }
    public class GMVM
    {
        public int GMId { get; set; }
        public string GMName { get; set; }
    }
    public class CityVM
    {
        public int CityId { get; set; }
        public string City { get; set; }
        public string CityCode { get; set; }
    }
    public class RegionVM
    {
        public int RegionId { get; set; }
        public string Region { get; set; }
    }

    public class LocationDetailVM
    {
        public int LocId { get; set; }
        public string LocCode { get; set; }
        public string LocName { get; set; }
        public string Address { get; set; }
        public int CityId { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public int LocTypeId { get; set; }
        public bool Status { get; set; }
        public int PurchaseCenter { get; set; }
    }

    public class SupplierCatVM
    {
        public int CategoryId { get; set; }
        public string CategoryTitle { get; set; }
    }
    public class ChatVM
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Branch { get; set; }
        public string LastMsg { get; set; }
        public string LastMsgTime { get; set; }
        public int NoOfMsg { get; set; }
        public bool IsOnline { get; set; }
        public long MaxId { get; set; }
        public bool IsFile { get; set; }
    }
    public class PurchaseOrderVM
    {
        public int CityId { get; set; }
        public int LocId { get; set; }
        public int SupplierId { get; set; }
        public System.DateTime OrderDate { get; set; }
        public System.DateTime DueDate { get; set; }
        public int UserId { get; set; }
        public string SupName { get; set; }
        public string MobileNo { get; set; }
        public string Address { get; set; }

        public string Remarks { get; set; }
    }
    public class PODetailVM
    {
        public long TransId { get; set; }
        public long PlanId { get; set; }
        public long PODtlId { get; set; }
        public int ModelId { get; set; }
        public string Model { get; set; }
        public int CityId { get; set; }
        public string City { get; set; }
        public int SKUId { get; set; }
        public bool IsPair { get; set; }
        public string SKU { get; set; }
        public int Qty { get; set; }
        public string PolicyType { get; set; }
        public decimal PP { get; set; }
        public decimal TP { get; set; }
        public decimal SP { get; set; }
        public decimal MRP { get; set; }
        public decimal Discount { get; set; }
        public decimal GST { get; set; }
        public decimal WHT { get; set; }
        public decimal NetPrice { get; set; }
        public decimal Amount { get; set; }
        public int PolicyId { get; set; }
        public string Remarks { get; set; }
        public decimal MonthlyIncentive { get; set; }
        public decimal QuarterlyIncentive { get; set; }
        public decimal BiannuallyIncentive { get; set; }
        public decimal AnnuallyIncentive { get; set; }
    }
    public class POGiftVM
    {
        public long PODtlId { get; set; }
        public string Model { get; set; }
        public int CityId { get; set; }
        public string City { get; set; }
        public int SKUId { get; set; }
        public string SKU { get; set; }
        public int Qty { get; set; }
        public decimal ExtraCharges { get; set; }
        public decimal Amount { get; set; }
        public int PolicyId { get; set; }
    }
    public class POPlanVM
    {
        public long PlanId { get; set; }
        public string PlanNo { get; set; }
    }
    public class POPlanDetailVM
    {
        public int CityId { get; set; }
        public string City { get; set; }
        public int SkuId { get; set; }
        public string SKU { get; set; }
        public int Qty { get; set; }
        public int StockQty { get; set; }
        public int SaleQty { get; set; }
        public int PendingQty { get; set; }
    }

    public class InvOpeningStockMobileVM
    {
        public long RowId { get; set; }
        public int SKUId { get; set; }
        public int LocId { get; set; }
        public string SerialNo { get; set; }
        public decimal PPrice { get; set; }
        public int StatusID { get; set; }
        public int SuppId { get; set; }
        public decimal MRP { get; set; }
        public string CSerialNo { get; set; }
        public string Remarks { get; set; }
        //public System.DateTime TransDate { get; set; }
        public int UserId { get; set; }
        //    public decimal Tax { get; set; }
        //    public bool Exempted { get; set; }
    }
    public class BranchStockVM
    {
        public string LocCode { get; set; }
        public string LocName { get; set; }
        public string SKUCode { get; set; }
        public int Qty { get; set; }
        public decimal Distance { get; set; }
    }
    public class Inv_StoreVM
    {
        public int RowId { get; set; }
        public string CompanyId { get; set; }
        public string SKU { get; set; }
        public string Supplier { get; set; }
        public long ItemId { get; set; }
        public int SKUId { get; set; }
        public int LocId { get; set; }
        public int Qty { get; set; }
        public string SerialNo { get; set; }
        public decimal PPrice { get; set; }
        public int StatusID { get; set; }
        public Nullable<int> SuppId { get; set; }
        public string Remarks { get; set; }
        public Nullable<System.DateTime> TrxDate { get; set; }
        public decimal MRP { get; set; }
        public decimal Tax { get; set; }
        public bool Exempted { get; set; }
        public string CSerialNo { get; set; }
        public string Company { get; set; }
        public string Product { get; set; }
        public string Model { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
    }
    public class Inv_OpeningStockVM
    {
        public long RowId { get; set; }
        public int SKUId { get; set; }
        public int LocId { get; set; }
        public string SKU { get; set; }
        public string SerialNo { get; set; }
        public string CSerialNo { get; set; }
        public string Remarks { get; set; }
        public decimal PPrice { get; set; }
        public decimal MRP { get; set; }
        public decimal Tax { get; set; }
        public bool Exempted { get; set; }
        public int StatusID { get; set; }
        public int SuppId { get; set; }
        public long ItemId { get; set; }
        public System.DateTime TransDate { get; set; }
        public long UserId { get; set; }
    }
    public class FarOpeningVM
    {
        public long DocId { get; set; }
        public System.DateTime DocDate { get; set; }
        public long StoreId { get; set; }
        public int SuppId { get; set; }
        public int ItemId { get; set; }
        public string ItemType { get; set; }
        public string ItemName { get; set; }
        public string SerialNo { get; set; }
        public string CSerialNo { get; set; }
        public System.DateTime TransDate { get; set; }
        public int Status { get; set; }
        public int UserId { get; set; }
        public string Remarks { get; set; }
        public int LocId { get; set; }
        public int EmpId { get; set; }
        public string Condition { get; set; }
        public System.DateTime InstallationDate { get; set; }
        public decimal PPrice { get; set; }
        public System.DateTime PurchaseDate { get; set; }
        public decimal CurrentValue { get; set; }
        public int DepreciationMethod { get; set; }
        public decimal DepreciationPercent { get; set; }
        public int CCCode { get; set; }
        public int CostTypeId { get; set; }
    }
    public class FarOpeningAVM
    {
        public long DocId { get; set; }
        public int ItemId { get; set; }
        public int SuppId { get; set; }
        public string SerialNo { get; set; }
        public string CSerialNo { get; set; }
        public int UserId { get; set; }
        public string Remarks { get; set; }
        public int LocId { get; set; }
        public decimal PPrice { get; set; }
        public string ItemType { get; set; }
        public string UploadedFiles { get; set; }
    }
    public class FarDepreciationVM
    {
        public long RowId { get; set; }
        public long StoreId { get; set; }
        public string ItemName { get; set; }
        public string SerialNo { get; set; }
        public System.DateTime Month { get; set; }
        public int CCCode { get; set; }
        public int PCCode { get; set; }
        public string CCCodeDesc { get; set; }
        public int CostTypeId { get; set; }
        public string CostType { get; set; }

        public decimal DeprAmt { get; set; }
        public decimal PPrice { get; set; }
        public decimal CurrentValue { get; set; }
    }
    public class FarStoreVM
    {
        public long StoreId { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string SerialNo { get; set; }
        public string CSerialNo { get; set; }
        public System.DateTime TransDate { get; set; }
        public int Status { get; set; }
        public int UserId { get; set; }
        public string Remarks { get; set; }
        public int LocId { get; set; }
        public string LocName { get; set; }
        public string CCCodeDesc { get; set; }
        public int EmpId { get; set; }
        public int ToEmpId { get; set; }
        public string EmpName { get; set; }
        public string Condition { get; set; }
        public System.DateTime InstallationDate { get; set; }
        public decimal PPrice { get; set; }
        public System.DateTime PurchaseDate { get; set; }
        public decimal CurrentValue { get; set; }
        public int DepreciationMethod { get; set; }
        public decimal DepreciationPercent { get; set; }
        public int CCCode { get; set; }
        public int ToCCCode { get; set; }
        public int SuppId { get; set; }
        public string SuppName { get; set; }
        public string ItemType { get; set; }
    }
    public class POPlanCityDetailVM
    {
        public int CityId1 { get; set; }
        public int City1 { get; set; }
        public int CityId2 { get; set; }
        public int City2 { get; set; }
        public int CityId3 { get; set; }
        public int City3 { get; set; }
        public int CityId4 { get; set; }
        public int City4 { get; set; }
        public int CityId5 { get; set; }
        public int City5 { get; set; }
        public int CityId6 { get; set; }
        public int City6 { get; set; }
        public int CityId7 { get; set; }
        public int City7 { get; set; }
        public int CityId8 { get; set; }
        public int City8 { get; set; }
        public int CityId9 { get; set; }
        public int City9 { get; set; }
        public int CityId10 { get; set; }
        public int City10 { get; set; }
        public int CityId11 { get; set; }
        public int City11 { get; set; }
        public int CityId12 { get; set; }
        public int City12 { get; set; }
        public int CityId13 { get; set; }
        public int City13 { get; set; }
        public int CityId14 { get; set; }
        public int City14 { get; set; }
        public int CityId15 { get; set; }
        public int City15 { get; set; }
        public int CityId16 { get; set; }
        public int City16 { get; set; }
        public int CityId17 { get; set; }
        public int City17 { get; set; }
        public int CityId18 { get; set; }
        public int City18 { get; set; }
        public int CityId19 { get; set; }
        public int City19 { get; set; }
        public int CityId20 { get; set; }
        public int City20 { get; set; }
        public int SkuId { get; set; }
        public string SKU { get; set; }
        public int RowId { get; set; }
    }
    public class POStatusVM
    {
        public int StatusId { get; set; }
        public string Status { get; set; }
    }

    public class POTypeVM
    {
        public int POTypeId { get; set; }
        public string POType { get; set; }
    }
    public class ScheduleMasterVM
    {
        public long SchMasterId { get; set; }
    }
    public class OrderScheduleVM
    {
        public long POSchId { get; set; }
        public long PODtlId { get; set; }
        public int LocId { get; set; }
        public string LocName { get; set; }
        public int ModelId { get; set; }
        public string Model { get; set; }
        public int SKUId { get; set; }
        public string SKU { get; set; }
        public int Qty { get; set; }
        public int StockQty { get; set; }
        public int SaleQty { get; set; }
        public int PendingQty { get; set; }
    }
    public class OrderSearchVM
    {
        public long POId { get; set; }
        public string PONo { get; set; }
        public System.DateTime PODate { get; set; }
        public System.DateTime? DeliveryDate { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        public string FullName { get; set; }
        public string ValidateBy { get; set; }
        public string SuppName { get; set; }
        public int SuppId { get; set; }
        public long PInvId { get; set; }
        public int POTypeId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

    }
    public class PRVM
    {
        public long PrId { get; set; }
        public string PrNo { get; set; }
        public int LocId { get; set; }
        public string location { get; set; }
        public DateTime PRDate { get; set; }
        public DateTime TransDate { get; set; }
        public int UserId { get; set; }
        public bool Status { get; set; }
        public string LCSuppName { get; set; }
        public string LCSuppMob { get; set; }
        public string LCSuppAddress { get; set; }
        public List<long> files { get; set; }
        public List<PRVM_Detail> PRDetail { get; set; }
        public string PrStatus { get; set; }
        public int DocCount { get; set; }
    }
    public class PRVM_Detail
    {
        public long PrDtlId { get; set; }
        public long PRId { get; set; }
        public int? SKUId { get; set; }
        public string SKU { get; set; }
        public int Qty { get; set; }
        public decimal Price { get; set; }
        public string Remarks { get; set; }
        public long LocId { get; set; }
        public string LocName { get; set; }
        public long? ModelId { get; set; }
        public string Model { get; set; }

    }
    public class PurchaseDetailVM
    {
        public long RowId { get; set; }
        public long TransId { get; set; }
        public int SKUId { get; set; }
        public int ModelId { get; set; }
        public string SKU { get; set; }
        public long ItemId { get; set; }
        public string SrNo { get; set; }
        public string CSrNo { get; set; }
        public int Qty { get; set; }
        public string Product { get; set; }
        public string Company { get; set; }
        public string Type { get; set; }
        public string Model { get; set; }
        public string Discription { get; set; }
        public string Remarks { get; set; }
    }
    public class POPaymentVM
    {
        public long TransId { get; set; }
        public int LocId { get; set; }
        public long POInvId { get; set; }
        public decimal Payment { get; set; }
        public decimal PreBalance { get; set; }
        public decimal Discount { get; set; }
        public decimal Amount { get; set; }
        public string Remarks { get; set; }
        public string PaidBy { get; set; }
    }
    public class PurchaseVM
    {
        public long PInvId { get; set; }
        public string InvNo { get; set; }
        public string Supplier { get; set; }
    }
    public class GRNInvoiceDetailVM
    {
        public long GRNId { get; set; }
        public string PONo { get; set; }
        public string GRNNo { get; set; }
        public System.DateTime GRNDate { get; set; }
        public string DONo { get; set; }
        public Nullable<System.DateTime> DODate { get; set; }
        public string InvNo { get; set; }
        public Nullable<System.DateTime> InvDate { get; set; }
        public string LocCode { get; set; }
        public string LocName { get; set; }
        public int SuppId { get; set; }
        public string SuppName { get; set; }
        public string RcvdBy { get; set; }
        public System.DateTime RcvdDate { get; set; }
        public Nullable<int> Qty { get; set; }
        public string SKUCode { get; set; }
        public string Model { get; set; }
        public decimal MRP { get; set; }
        public decimal RP { get; set; }
        public decimal TP { get; set; }
        public decimal Discount { get; set; }
        public decimal GST { get; set; }
        public decimal WHT { get; set; }
        public decimal NetPrice { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public bool IsEdit { get; set; }
    }
    public class InvForPaymentVM
    {
        public long RowId { get; set; }
        public long PInvId { get; set; }
        public string PONo { get; set; }
        public string GRNNo { get; set; }
        public System.DateTime GRNDate { get; set; }
        public string DONo { get; set; }
        public Nullable<System.DateTime> DODate { get; set; }
        public string InvNo { get; set; }
        public Nullable<System.DateTime> InvDate { get; set; }
        public string LocCode { get; set; }
        public string LocName { get; set; }
        public int SuppId { get; set; }
        public string SuppName { get; set; }
        public string RcvdBy { get; set; }
        public System.DateTime RcvdDate { get; set; }
        public Nullable<int> Qty { get; set; }
        public Nullable<decimal> Rate { get; set; }
        public Nullable<decimal> Discount { get; set; }
        public Nullable<decimal> STax { get; set; }
        public Nullable<decimal> WHT { get; set; }
        public string PaymentStatus { get; set; }
        public Nullable<System.DateTime> DueDate { get; set; }
        public decimal NetPrice { get; set; }
        public decimal Amount { get; set; }
        public decimal PartialPaidAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public bool PaidStatus { get; set; }
    }
    public class SuppPaymentVM
    {
        public long TransId { get; set; }
        public int SuppId { get; set; }
        public string SuppName { get; set; }
        public decimal ClosingBalance { get; set; }
        public decimal Payment { get; set; }
        public decimal Proposed { get; set; }
        public decimal Disc { get; set; }
        public decimal WHT { get; set; }
        public decimal BankAmount { get; set; }
        public long BankAccId { get; set; }
        public decimal CashAmount { get; set; }
        public bool IsWHTPaid { get; set; }
        public string InstrumentNo { get; set; }
        public string Instrument { get; set; }
        //public string InstrumentId { get; set; }
        public string ChequeNo { get; set; }
        public DateTime PaidDate { get; set; }
        [Required]
        public int LocId { get; set; }
        public Nullable<System.DateTime> ChequeDate { get; set; }
        public string Remarks { get; set; }
        public long AccId { get; set; }
        public string AccTitle { get; set; }
    }
    public class POInvoiceDetailVM
    {
        public long RowId { get; set; }
        public long PODtlId { get; set; }
        public string Model { get; set; }
        public int SKUId { get; set; }
        public string SKU { get; set; }
        public int OrderQty { get; set; }
        public int Qty { get; set; }
        public bool IsGiftItem { get; set; }
        public decimal TP { get; set; }
        public decimal RP { get; set; }
        public decimal MRP { get; set; }
        public decimal Discount { get; set; }
        public decimal GST { get; set; }
        public decimal WHT { get; set; }
        public decimal NetPrice { get; set; }
        public decimal Amount { get; set; }
        public bool IsEdit { get; set; }
    }
    public class CustomerVM
    {
        public int CustId { get; set; }
        public string CustName { get; set; }
    }
    public class BlockCustomerVM
    {
        public long RowId { get; set; }
        public string CustomerName { get; set; }
        public string CNIC { get; set; }
        public string MobileNo1 { get; set; }
        public string MobileNo2 { get; set; }
        public string CareOf { get; set; }
        public string Remarks { get; set; }
        public string Location { get; set; }
        public string City { get; set; }
        public bool Status { get; set; }
    }
    public class LseSmsVM
    {
        public long RowId { get; set; }
        public long AccNo { get; set; }
        public int LocId { get; set; }
        public string MobileNo { get; set; }
        public string Category { get; set; }
        public string SMS { get; set; }
        public bool IsUrdu { get; set; }
        public DateTime WorkingDate { get; set; }
    }
    public class CustomerDetailVM
    {
        public int CustId { get; set; }
        public string CustName { get; set; }
        public string Address { get; set; }
        public string Mobile { get; set; }
        public string CNIC { get; set; }
        public string NTN { get; set; }
        public string Email { get; set; }
        public string GLCode { get; set; }
        public int DaysLimit { get; set; }
        public string ChequeNo { get; set; }
        public string BankName { get; set; }
        public string AccountTitle { get; set; }
        public string BusinessRelation { get; set; }
        public string Remarks { get; set; }
        public long AccNo { get; set; }
        public string Message { get; set; }
        public DateTime LoadDate { get; set; }
        public string CustAccountHolder { get; set; }
        public string CustType { get; set; }
    }

    public class CreditInvoiceVM
    {
        public long TransId { get; set; }
        public int CustId { get; set; }
        public string CustName { get; set; }
        public string BillNo { get; set; }

    }

    public class SaleCreditReceiveVM
    {
        public long TransId { get; set; }
        public decimal ReceivedAmount { get; set; }
        public long SaleId { get; set; }
        public string ReceivedFrom { get; set; }
        public string Remarks { get; set; }
        public int LocId { get; set; }
    }
    public class SaleDetailVM
    {
        public long RowId { get; set; }
        public long TransId { get; set; }
        public int SKUId { get; set; }
        public string SKUName { get; set; }
        public long ItemId { get; set; }
        public string SrNo { get; set; }
        public string CSrNo { get; set; }
        public int Qty { get; set; }
        public decimal PPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal SPrice { get; set; }
        public decimal TPrice { get; set; }
        //public double TaxPercentage { get; set; }
        //public decimal TotalTax { get; set; }
        public string Product { get; set; }
        public int TypeId { get; set; }
        public List<long> files { get; set; }
        public string Company { get; set; }
        public string Specification { get; set; }
        public string Reason { get; set; }
        public string CareOf { get; set; }
    }
    public class ProcessVM
    {
        public long AccNo { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Select Location")]
        public int LocId { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Select Category")]
        public int CategoryId { get; set; }
        [Required]
        public System.DateTime ProcessDate { get; set; }
        [Required]
        public decimal ProcessFee { get; set; }
        [Required]
        [StringLength(200, MinimumLength = 3)]
        public string CustName { get; set; }
        [Required]
        [StringLength(200, MinimumLength = 3)]
        public string FName { get; set; }
        [Required]
        [RegularExpression(@"^[0-9]{4}-[0-9]{7}$", ErrorMessage = "Mobile No is not valid.")]
        public string Mobile1 { get; set; }
        public string Mobile2 { get; set; }
        [Required]
        [RegularExpression(@"^[0-9]{5}-[0-9]{7}-[0-9]{1}$", ErrorMessage = "CNIC is not valid.")]
        public string NIC { get; set; }
        [Required]
        public decimal InstPrice { get; set; }
        [Required]
        public decimal Advance { get; set; }
        [Required]
        public decimal ActualAdvance { get; set; }
        [Required]
        public decimal MonthlyInst { get; set; }
        [Required]
        [Range(1, 50, ErrorMessage = "Enter Duration")]
        public int Duration { get; set; }
        [Required]
        public string ProcessAt { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Select MktOfficer")]
        public int MktOfficerId { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Select InqOfficer")]
        public int InqOfficerId { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Select Manager")]
        public int ManagerId { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Select SManager")]
        public int SManagerId { get; set; }
        [Required]
        public System.DateTime EffectiveDate { get; set; }
        public string Remarks { get; set; }
    }
    public class ProcessingVM
    {
        public long AccNo { get; set; }
        public int LocId { get; set; }
        public int CategoryId { get; set; }
        public System.DateTime ProcessDate { get; set; }
        public decimal ProcessFee { get; set; }
        public Nullable<System.DateTime> DeliveryDate { get; set; }
        public Nullable<System.DateTime> CloseDate { get; set; }
        public string CustName { get; set; }
        public string FName { get; set; }
        public string Mobile1 { get; set; }
        public string Mobile2 { get; set; }
        public string NIC { get; set; }
        public decimal InstPrice { get; set; }
        public decimal Advance { get; set; }
        public decimal ActualAdvance { get; set; }
        public decimal MonthlyInst { get; set; }
        public int Duration { get; set; }
        public string ProcessAt { get; set; }
        public int MktOfficerId { get; set; }
        public int InqOfficerId { get; set; }
        public int ManagerId { get; set; }
        public int SManagerId { get; set; }
        public System.DateTime EffectiveDate { get; set; }
        public string Remarks { get; set; }
        public int? OldAccNo { get; set; }
        public int ReturnTypeId { get; set; }
        public string Type { get; set; }
        public decimal ReturnAmount { get; set; }
        public int ReasonId { get; set; }
        public bool IncExempt { get; set; }
        public bool AICReturn { get; set; }
        public List<long> files { get; set; }
        public decimal OrderAmount { get; set; }
        public long OrderId { get; set; }
    }
    public class LseDetailTY
    {

        public long DtlId { get; set; }
        public long AccNo { get; set; }
        public int SKUId { get; set; }
        public long ItemId { get; set; }
        public int Qty { get; set; }
        public decimal Discount { get; set; }
        public decimal InstPrice { get; set; }
        public bool Status { get; set; }
        public decimal dAdvance { get; set; }
        public decimal dInst { get; set; }
        public long InstPlanId { get; set; }
        public string PlanType { get; set; }
    }
    public class LseDetailVM
    {
        public long TransId { get; set; }
        public long DtlId { get; set; }
        //public long AccNo { get; set; }
        public int SKUId { get; set; }
        public string SKUName { get; set; }
        public long ItemId { get; set; }
        public int Qty { get; set; }
        public decimal Discount { get; set; }
        public decimal InstPrice { get; set; }
        public decimal TPrice { get; set; }
        public string SerialNo { get; set; }
        public string CSerialNo { get; set; }
        public bool Status { get; set; }
        public decimal dAdvance { get; set; }
        public decimal dInst { get; set; }
        public long InstPlanId { get; set; }
        public string PlanType { get; set; }
    }
    #endregion

    #region ClosingVoucherVM

    public class ClosingVoucherVM
    {
        public int Sr { get; set; }
        public string Head { get; set; }
        public Nullable<decimal> Amount { get; set; }
    }
    public class ClosingVoucherDatVM
    {
        public List<ClosingVoucherDistVM> GrdVoucher { get; set; }
        public List<ClosingVoucherAdjVM> VoucherAdj { get; set; }
        public List<ClosingVoucherVM> VoucherDet { get; set; }
    }
    public class ClosingVoucherAdjVM
    {
        public Nullable<int> Sr { get; set; }
        public string PaidBy { get; set; }
        public Nullable<decimal> InstCharges { get; set; }
    }

    public class ClosingVoucherDistVM
    {
        public long DsId { get; set; }
        public int DeptId { get; set; }
        public int EmpId { get; set; }
        public string EmpName { get; set; }
        public string CNIC { get; set; }
        public int DesgId { get; set; }
        public string DesgName { get; set; }
        public string EmpStatus { get; set; }
        public int SortOrder { get; set; }
        public Nullable<decimal> NetSalary { get; set; }
        public int EmpLedgerBal { get; set; }
        public decimal Outstand { get; set; }
        public decimal ClosingAdv { get; set; }

        public Nullable<decimal> ApprovedAdv { get; set; }
        public Nullable<int> ApprovedBy { get; set; }
        public Nullable<System.DateTime> ApprovedDate { get; set; }
    }

    #endregion

    #region BankLetter

    public class BankLetterUploader
    {

        public int EmpId { get; set; }
    }
    public class BankLetterVM
    {
        public int BlId { get; set; }
        public int EmpId { get; set; }
        public string EmpName { get; set; }
        public string CNIC { get; set; }
        public string DesgName { get; set; }
        public string EmpStatus { get; set; }
        public System.DateTime DOJ { get; set; }
        public string BLTitle { get; set; }
        public string DeptName { get; set; }
        public decimal BankSalary { get; set; }
        public string AccountNumber { get; set; }
        public string Bank { get; set; }
        public int SortOrder { get; set; }
        public long SheetDtlId { get; set; }
        public int e { get; set; }
        public decimal b { get; set; }
        public long s { get; set; }
        public DateTime SalaryMonth { get; set; }

    }
    public class BankLetterPostingVM
    {
        public int BlId { get; set; }
        public string LetterTitle { get; set; }
        public string Status { get; set; }
        public string PreparedBy { get; set; }
        public DateTime PreparedDate { get; set; }
        public DateTime? PostedDate { get; set; }
        public string PostedBy { get; set; }
    }

    #endregion



    public class EmpPromotionVM
    {
        public long TransId { get; set; }
        public string TransType { get; set; }
        public int EmpId { get; set; }
        public int DeptId { get; set; }
        public int CurrentDesgId { get; set; }
        public int NewDesgId { get; set; }
        public decimal CurrentSalary { get; set; }
        public string Tenure { get; set; }
        public decimal NewSalary { get; set; }
        public System.DateTime EffectiveFrom { get; set; }
        public string Reason { get; set; }
        public int DefinedBy { get; set; }
        public Nullable<int> NewDeptId { get; set; }
        public System.DateTime DefinedDate { get; set; }
        public Nullable<int> ApprovedBy { get; set; }
        public Nullable<System.DateTime> ApprovedDate { get; set; }
    }
    public class CashSaleVM
    {
        public long TransId { get; set; }
        public string BillNo { get; set; }
        public DateTime DateTime { get; set; }
        public string USIN { get; set; }
        public string FBR { get; set; }
        public string IsReturn { get; set; }
        public string Customer { get; set; }
        public string Mobile { get; set; }
        public string CNIC { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; }
    }
    public class UsersNotificationVM
    {
        public long Id { get; set; }
        public string Notification { get; set; }
        public int UserId { get; set; }
        public bool IsSeen { get; set; }
        public Nullable<long> RefId { get; set; }
        public string RefLink { get; set; }
        public Nullable<System.DateTime> SeenDate { get; set; }
        public System.DateTime NotificationDate { get; set; }
        public string RelativeNotificationDate { get; set; }
        public int SentBy { get; set; }
        public string SentByUsername { get; set; }
        public string Message { get; set; }

    }
    public class DocShareVM
    {
        public long DocId { get; set; }
        public long DocDetailId { get; set; }
        public long TransId { get; set; }
        public System.DateTime WorkingDate { get; set; }
        public string DocTitle { get; set; }
        public string Description { get; set; }
        public System.DateTime TransDate { get; set; }
        public List<DocShareDetailVM> DocShareDet { get; set; }
        public string UploadedFiles { get; set; }
        public int UserId { get; set; }
        public string ReceivedBy1 { get; set; }
        public string ReceivedBy2 { get; set; }
        public string ReceivedBy3 { get; set; }

        public int LocId { get; set; }
        public bool IsReceived { get; set; }
    }
    public class DocShareDetailVM
    {
        public long TransDtlId { get; set; }
        public long TransId { get; set; }
        public int LocId { get; set; }
        public string ReceivedBy1 { get; set; }
        public string ReceivedBy2 { get; set; }
        public string ReceivedBy3 { get; set; }
        public Nullable<System.DateTime> TransDate { get; set; }
        public Nullable<int> UserId { get; set; }
    }

    public class OrganizaOrgano
    {
        public string EmployeeName { get; set; }
        public int id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public string DesgName { get; set; }
        public List<OrganizaOrgano> items { get; set; }
    }

    public class InvStatusVM
    {
        public int StatusID { get; set; }
        public string StatusTitle { get; set; }
    }
    public class ItemHistoryVM
    {
        public string LocName { get; set; }
        public DateTime? Date { get; set; }
        public string Action { get; set; }
        public string Username { get; set; }
    }
    public class IncentiveVM
    {
        public long RowId { get; set; }
        public int EmpId { get; set; }
        public string EmpName { get; set; }
        public int DesgId { get; set; }
        public string DesgName { get; set; }
        public string CNIC { get; set; }
        public decimal AchQty { get; set; }
        public decimal AchValue { get; set; }
        public decimal IncentivePercent { get; set; }
        public decimal IncentiveValue { get; set; }
        public string Remarks { get; set; }
        public int LocId { get; set; }
        public string LocName { get; set; }
    }
    public class SaleTargetVM
    {
        public long TargetId { get; set; }
        public int TargetMonth { get; set; }
        public int TargetYear { get; set; }
        public string City { get; set; }
        public int LocId { get; set; }
        public string LocCode { get; set; }
        public string LocName { get; set; }
        public decimal Target { get; set; }
        public decimal TargetPDA { get; set; }
    }
    public class SaleTargetTypeVM
    {
        public string TargetTypeId { get; set; }
        public string TargetType { get; set; }
    }
    public class POReturnDtlVM
    {
        public long PORDtlId { get; set; }
        public long PORId { get; set; }
        public int SKUId { get; set; }
        public long ItemId { get; set; }
        public string SrNo { get; set; }
        public string SKU { get; set; }
        public string Product { get; set; }
        public string Company { get; set; }
        public int Qty { get; set; }
        public string Remarks { get; set; }
        public decimal TP { get; set; }
    }
    public class LocManagersVM
    {
        public Nullable<int> RMId { get; set; }
        public string RM { get; set; }
        public Nullable<int> SRMId { get; set; }
        public string SRM { get; set; }
        public Nullable<int> CRCId { get; set; }
        public string CRC { get; set; }
        public Nullable<int> BDMId { get; set; }
        public string BDM { get; set; }
        public Nullable<int> SSRM { get; set; }
        public Nullable<int> DGM { get; set; }
        public Nullable<int> GM { get; set; }
    }
    public class LeaseCustomerVM
    {
        public int LocId { get; set; }
        public string CustName { get; set; }
        public string FName { get; set; }
        public string Mobile1 { get; set; }
        public string Mobile2 { get; set; }
        public string NIC { get; set; }
        public int MktOfficerId { get; set; }
        public int InqOfficerId { get; set; }
        public int ManagerId { get; set; }
        public int SManagerId { get; set; }
        public long CustId { get; set; }
        public long AccNo { get; set; }
        public string Occupation { get; set; }
        public string ResAddress { get; set; }
        public DateTime OutstandDate { get; set; }
        public string OffAddress { get; set; }
        public string Gender { get; set; }
        public string ResidentialStatus { get; set; }
        public decimal Salary { get; set; }
        public bool Affidavit { get; set; }
        public bool WrantyCard { get; set; }
        public bool Worth { get; set; }
        public bool Defaulter { get; set; }
        public bool AuditStatus { get; set; }
        public string Remarks { get; set; }
        public bool PTO { get; set; }
        public bool SearchStatus { get; set; }
        public string uri { get; set; }
        public string uriG { get; set; }
    }
    public class LseAuditSummaryVM
    {
        public int LocId { get; set; }
        public string LocCode { get; set; }
        public string LocName { get; set; }
        public string AuditorId { get; set; }
        public string Auditor { get; set; }
        public int Audit { get; set; }
        public int Pending { get; set; }
        public int Verified { get; set; }
    }
    public class LseAuditVM
    {
        public long AuditId { get; set; }
        public long AccNo { get; set; }
        public Nullable<System.DateTime> AuditDate { get; set; }
        public bool Guarantor1 { get; set; }
        public bool Guarantor2 { get; set; }
        public bool Mobile { get; set; }
        public bool NIC { get; set; }
        public bool Cheque { get; set; }
        public bool Pic { get; set; }
        public bool Thumb { get; set; }
        public bool Affidavit { get; set; }
        public int Completion { get; set; }
        public bool BMSign { get; set; }
        public bool RMSign { get; set; }
        public bool VisitStatus { get; set; }
        public bool ObserveState { get; set; }
        public Nullable<System.DateTime> ObserveDate { get; set; }
        public bool VerifyStatus { get; set; }
        public string CRCRemarks { get; set; }
        public string ApprovedBy { get; set; }
        public Nullable<System.DateTime> ApprovedDate { get; set; }
        public bool Status { get; set; }
        public string ComName { get; set; }
        public string CustName { get; set; }
        public string ProductName { get; set; }
        public string Model { get; set; }
        public decimal InstPrice { get; set; }
        public bool WrongCase { get; set; }
        public bool WrongProduct { get; set; }
        public bool InvolvementCase { get; set; }
        public bool WrongPTO { get; set; }
        public bool PTOCase { get; set; }
        public bool HomeFake { get; set; }
        public bool OfficialFake { get; set; }
        public bool LoseGuarantee { get; set; }
        public bool FakeGuarantee { get; set; }
        public bool ManageGuarantee { get; set; }
        public bool WithoutVerification { get; set; }
        public bool HomeRental { get; set; }
        public bool PhotoCHQ { get; set; }
    }
    public class LseAuditAVM
    {
        public int LocId { get; set; }
        public string FName { get; set; }
        public string Mobile1 { get; set; }
        public string Mobile2 { get; set; }
        public string NICStatus { get; set; }
        public bool AffidavitStatus { get; set; }
        public int NoOfCheques { get; set; }
        public int NoOfGuarantors { get; set; }
        public long AuditId { get; set; }
        public long AccNo { get; set; }
        public Nullable<System.DateTime> AuditDate { get; set; }
        public bool Guarantor1 { get; set; }
        public bool Guarantor2 { get; set; }
        public bool Mobile { get; set; }
        public bool NIC { get; set; }
        public bool Cheque { get; set; }
        public bool Pic { get; set; }
        public bool Thumb { get; set; }
        public bool Affidavit { get; set; }
        public int Completion { get; set; }
        public bool BMSign { get; set; }
        public bool RMSign { get; set; }
        public bool VisitStatus { get; set; }
        public bool ObserveState { get; set; }
        public Nullable<System.DateTime> ObserveDate { get; set; }
        public bool VerifyStatus { get; set; }
        public string CRCRemarks { get; set; }
        public string ApprovedBy { get; set; }
        public Nullable<System.DateTime> ApprovedDate { get; set; }
        public bool Status { get; set; }
        public string ComName { get; set; }
        public string CustName { get; set; }
        public string ProductName { get; set; }
        public string Model { get; set; }
        public decimal InstPrice { get; set; }
        public bool WrongCase { get; set; }
        public bool WrongProduct { get; set; }
        public bool InvolvementCase { get; set; }
        public bool WrongPTO { get; set; }
        public bool PTOCase { get; set; }
        public bool HomeFake { get; set; }
        public bool OfficialFake { get; set; }
        public bool LoseGuarantee { get; set; }
        public bool FakeGuarantee { get; set; }
        public bool ManageGuarantee { get; set; }
        public bool WithoutVerification { get; set; }
        public bool HomeRental { get; set; }
        public bool PhotoCHQ { get; set; }
    }
    public class GuarantorVM
    {
        public long GuarantorId { get; set; }
        public long AccNo { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string FName { get; set; }
        [Required]
        public string CNIC { get; set; }
        [Required]
        public string Occupation { get; set; }
        [Required]
        public string ResAddress { get; set; }
        [Required]
        public string OffAddress { get; set; }
        [Required]
        public string TelRes { get; set; }
        [Required]
        public string TelOff { get; set; }
        public string GRelation { get; set; }
    }
    public class ChequeVM
    {
        public long ChequeId { get; set; }
        public long AccNo { get; set; }
        [Required]
        public string ChequeType { get; set; }
        [StringLength(25, MinimumLength = 3)]
        public string ChequeNo { get; set; }
        public decimal ChequeAmount { get; set; }
        [StringLength(50, MinimumLength = 3)]
        public string BankName { get; set; }
        [StringLength(50, MinimumLength = 3)]
        public string AccountTitle { get; set; }
        [StringLength(50, MinimumLength = 8)]
        public string BankAccNo { get; set; }
        [StringLength(50, MinimumLength = 3)]
        public string BranchName { get; set; }
        public string ChequeIssueTo { get; set; }
        public string ChequeReturnTo { get; set; }
        [Required]
        public string ChequeCNIC { get; set; }
        [Required]
        public string ChequeBy { get; set; }
        public string Msg { get; set; }
    }
    public class BankVM
    {
        public long BankId { get; set; }
        public string BankName { get; set; }

    }
    public class AdvanceVM
    {
        public long AccNo { get; set; }
        public int LocId { get; set; }
        public long CustId { get; set; }
        //public System.DateTime ProcessDate { get; set; }
        public string CustName { get; set; }
        public string SO { get; set; }
        public int ModelId { get; set; }
        public long ItemId { get; set; }
        //public decimal CashPrice { get; set; }
        public decimal ProcessFee { get; set; }
        //public string Remarks { get; set; }
        public int InspectorId { get; set; }
        public decimal InstPrice { get; set; }
        public string Status { get; set; }
        public decimal ActualAdvance { get; set; }
        public decimal MonthlyInstallment { get; set; }
        public int Duration { get; set; }
        //public bool AmountStatus { get; set; }
        public decimal AmountReceived { get; set; }
        public string ProcessAt { get; set; }
        public int InqOfficerId { get; set; }
        public int MangId { get; set; }
        //public System.DateTime EffectedDate { get; set; }
    }
    public class InstallmentVM
    {
        public long InstId { get; set; }
        public System.DateTime InstDate { get; set; }
        public int LocId { get; set; }
        public long AccNo { get; set; }
        public decimal InstCharges { get; set; }
        public decimal Discount { get; set; }
        public decimal Fine { get; set; }
        public string FineType { get; set; }
        public Nullable<int> RecoveryId { get; set; }
        public string Remarks { get; set; }
        public int PaidBy { get; set; }
        public int UPaidBy { get; set; }
        public bool IsLock { get; set; }
    }
    public class InstDetailVM
    {
        public long TransId { get; set; }
        public long AccNo { get; set; }
        public System.DateTime InstallDate { get; set; }
        public decimal InstCharges { get; set; }
        public decimal Fine { get; set; }
        public string Remarks { get; set; }
        public string CustName { get; set; }
        public string FineType { get; set; }
        public decimal ActualInstallment { get; set; }
        public decimal Discount { get; set; }
        public string RecoveryOff { get; set; }
        public decimal PreBalance { get; set; }
        public decimal Balance { get; set; }
        public int PaidBy { get; set; }
    }
    public class InstallmentDetailVM
    {
        public long TransId { get; set; }
        public DateTime InstallDate { get; set; }
        public long AccNo { get; set; }
        public string BranchName { get; set; }
        public string CustName { get; set; }
        public string FName { get; set; }
        public string Recovery { get; set; }
        public string Company { get; set; }
        public string Product { get; set; }
        public string SKU { get; set; }
        public decimal ActualAdvance { get; set; }
        public decimal Advance { get; set; }
        public decimal PreBalance { get; set; }
        public decimal ActualInstallment { get; set; }
        public decimal ArrearInstallment { get; set; }
        public decimal InstCharges { get; set; }
        public decimal Discount { get; set; }
        public decimal Balance { get; set; }
        public string FineType { get; set; }
        public decimal Fine { get; set; }
        public int PaidBy { get; set; }
        public string Remarks { get; set; }
        public string Comments { get; set; }
        public decimal TotalPrice { get; set; }
    }
    public class AccountSearchVM
    {
        public long AccNo { get; set; }
        public Nullable<System.DateTime> DeliveryDate { get; set; }
        public string Customer { get; set; }
        public string FName { get; set; }
        public string Mobile1 { get; set; }
        public string NIC { get; set; }
        public decimal MonthlyInst { get; set; }
        public decimal InstPrice { get; set; }
        public string Status { get; set; }
        public string Company { get; set; }
        public string Product { get; set; }
        public string Model { get; set; }
        public string SerialNo { get; set; }
        public string SKU { get; set; }

    }

    public class VPNSearchVM
    {
        public long AccNo { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string Customer { get; set; }
        public string FName { get; set; }
        public string Mobile1 { get; set; }
        public string Mobile2 { get; set; }
        public string NIC { get; set; }
        public string MonthlyInst { get; set; }
        public string InstPrice { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public string LocCode { get; set; }
    }
    public class ProfitCentersVM
    {
        public int PCCode { get; set; }
        public string PCDesc { get; set; }
        public string LocCode { get; set; }
    }
    public class CostCentersVM
    {
        public long CCCode { get; set; }
        public string CCDesc { get; set; }
        public int LocId { get; set; }
    }
    public class BankBookVM
    {
        public long TransID { get; set; }
        public long AccId { get; set; }
        public string BankName { get; set; }
        public string BankAccNo { get; set; }
        public string StartChqNo { get; set; }
        public string EndChqNo { get; set; }
        public string CurrentChqNo { get; set; }
        public bool ActiveStatus { get; set; }
        public int DefinedBy { get; set; }
        public System.DateTime DefinitionDate { get; set; }
        public string AccountType { get; set; }
    }
    public class BankBookTransVM
    {
        public long TransID { get; set; }
        public long VrId { get; set; }
        [Required]
        public string ChequeNum { get; set; }
        public int YrCode { get; set; }
        public int PrCode { get; set; }

        public long AccId { get; set; }

        public string ChequeNo { get; set; }
        [Required]
        public string ChequeType { get; set; }
        public Nullable<System.DateTime> ChequeDate { get; set; }

        [Range(typeof(Decimal), "1", "99999999999999")]
        public Nullable<decimal> Amount { get; set; }


        public string PaymentType { get; set; }
        [Required]
        public string Recipient { get; set; }
        public Nullable<System.DateTime> ClearDate { get; set; }
        public Nullable<System.DateTime> VoidDate { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        public Nullable<int> UserId { get; set; }
        public Nullable<System.DateTime> TransDate { get; set; }
        public string InstrumentNo { get; set; }
        public string InstrumentType { get; set; }
    }
    public class COAVM
    {
        public string PCode { get; set; }
        public string Code { get; set; }
        public long PId { get; set; }
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsLocked { get; set; }
        public string Remarks { get; set; }
        public int SubDivId { get; set; }
    }
    public class AcClassesVM
    {
        public int ClsCode { get; set; }
        public string ClsDesc { get; set; }
        public string Behavior { get; set; }
    }
    public class AcGroupsVM
    {
        public string GrCode { get; set; }
        public string GrDesc { get; set; }
        public int ClsCode { get; set; }
    }
    public class AcControlsVM
    {
        public string GrCode { get; set; }
        public string CnCode { get; set; }
        public string CnDesc { get; set; }
        public string BlCodeDr { get; set; }
        public string BlCodeCr { get; set; }
    }
    public class AccountsVM
    {
        public string CnCode { get; set; }
        public string SubCode { get; set; }
        public string SubCodeDesc { get; set; }
        public bool IsLocked { get; set; }
        public string Remarks { get; set; }
    }
    public class VoucherDetailVM
    {
        public long VrDtlId { get; set; }
        public long VrId { get; set; }
        public int PCCode { get; set; }
        public int CCCode { get; set; }
        public long AccId { get; set; }
        public long SubId { get; set; }
        public string SubCode { get; set; }
        public string SubCodeDesc { get; set; }
        public string SubsidaryCode { get; set; }
        public string SubsidaryDesc { get; set; }
        public string Particulars { get; set; }
        public decimal Dr { get; set; }
        public decimal Cr { get; set; }
        public string ChequeNo { get; set; }
        public int TrxSeqId { get; set; }
        public long? RefId { get; set; }

    }
    public class PaymentAdviceDetailVM
    {
        public decimal AdviceID { get; set; }
        public decimal AdviceDtlID { get; set; }
        public int SuppId { get; set; }
        public string Supplier { get; set; }
        public string SubCode { get; set; }
        public string SubCodeDesc { get; set; }
        public string ChequeName { get; set; }
        public string ChequeNo { get; set; }
        public decimal CheqValue { get; set; }
        public decimal LedgerClosingBal { get; set; }
        public decimal PaymentAmount { get; set; }
        public decimal TaxRate { get; set; }
        public decimal TaxAmount { get; set; }
        public bool isMerged { get; set; }
        public decimal MergeValue { get; set; }
    }
    public class IssueV
    {
        public int LocId { get; set; }
        public string LocName { get; set; }
        public int Qty { get; set; }
    }
    public class IssueDetailVM
    {
        public long RowId { get; set; }
        public long TransDtlId { get; set; }
        public long TransId { get; set; }
        public long ItemId { get; set; }
        public string SKU { get; set; }
        public string Model { get; set; }
        public string SrNo { get; set; }
        public int Qty { get; set; }
        public int ReceivedBy { get; set; }
        //public System.DateTime ReceivedDate { get; set; }
        public string Product { get; set; }
        public string Company { get; set; }
        public string Status { get; set; }
    }
    public class SKUItemVM
    {
        public int SKUId { get; set; }
        public string SKUCode { get; set; }
        public string ComName { get; set; }
        public string ProductName { get; set; }
        public string Serial { get; set; }
        public long? ItemId { get; set; }
    }
    public class ItemDetailVM
    {
        public long ItemId { get; set; }
        public string Location { get; set; }
        public string Company { get; set; }
        public string Product { get; set; }
        public string Type { get; set; }
        public string Model { get; set; }
        public string SerialNo { get; set; }
        public string Supplier { get; set; }
        public bool InTransit { get; set; }
        public string Status { get; set; }
        public string SKUName { get; set; }
        public int SKUId { get; set; }
        public string Remarks { get; set; }
    }
    public class StockTransferDashboardVM
    {
        public int SrNo { get; set; }
        public string FromBranch { get; set; }
        public string ToBranch { get; set; }
        public string Remarks { get; set; }
        public long DocNo { get; set; }
        public DateTime DocDate { get; set; }
        public string Status { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int RegionId { get; set; }
    }
    public class VoucherSearchVM
    {
        public long VrId { get; set; }
        public string VrTypeId { get; set; }
        public string VrNo { get; set; }
        public System.DateTime VrDate { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public int UserId { get; set; }
        public string CreateBy { get; set; }
        public string ValidateBy { get; set; }
    }
    public class CashTransferVM
    {
        public long TransId { get; set; }
        public int LocId { get; set; }
        public string Location { get; set; }
        public int ToLocId { get; set; }
        public System.DateTime WorkingDate { get; set; }
        //public decimal CashReceived { get; set; }
        public decimal TransferedCash { get; set; }
        public int ReceivedBy { get; set; }
        //public decimal ReceivedAmount { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
    }
    public class SerialRateVM
    {
        public long RowId { get; set; }
        public long ItemId { get; set; }
        public string SKU { get; set; }
        public DateTime EffectedDate { get; set; }
        public decimal SM { get; set; }
        public string Remarks { get; set; }
        public string SerialNo { get; set; }
    }
    public class SerialPlanVM
    {
        public int RowId { get; set; }
        public long ItemId { get; set; }
        public string SKU { get; set; }
        public DateTime EffectedDate { get; set; }
        public decimal BasePrice { get; set; }
        public decimal InstPrice { get; set; }
        public decimal Advance { get; set; }
        public decimal Inst { get; set; }
        public int Duration { get; set; }
        public string Location { get; set; }
        public string Remarks { get; set; }
        public string SerialNo { get; set; }
    }
    public class SKUPlanVM
    {
        public int RowId { get; set; }
        public int CityId { get; set; }
        public int LocId { get; set; }
        public int SKUId { get; set; }
        public System.DateTime EffectedDate { get; set; }
        public decimal BasePrice { get; set; }
        public decimal InstPrice { get; set; }
        public decimal Advance { get; set; }
        public decimal Inst { get; set; }
        public int Duration { get; set; }
        public string City { get; set; }
        public string Location { get; set; }
        public string Type { get; set; }
        public string Model { get; set; }
        public string SKU { get; set; }
        public decimal CashPrice { get; set; }
    }
    public class OutStandVM
    {
        public long TransId { get; set; }
        public long AccNo { get; set; }
        public int OldAccNo { get; set; }
        public string Category { get; set; }
        public string Customer { get; set; }
        public string Recovery { get; set; }
        public int RecoveryId { get; set; }
        public System.DateTime OutStandDate { get; set; }
        public decimal OutStandAmt { get; set; }
        public decimal Inst { get; set; }
        public decimal RecvAmt { get; set; }
        public decimal Remaning { get; set; }
        public System.DateTime TransDate { get; set; }
        public int UserId { get; set; }
        public string Status { get; set; }
        public bool IsCheck { get; set; }
    }

    public class ExpenseTypeVM
    {
        public int Id { get; set; }

        public string ExpType { get; set; }
    }
    public class SEmployeeVM
    {
        public int? EmpId { get; set; }
        public string EmpName { get; set; }
        public string FName { get; set; }
        public string CNIC { get; set; }
        public string DeptName { get; set; }
        public string DesgName { get; set; }

    }

    public class EmpListCRCVM
    {
        public int EmpId { get; set; }
        public string EmpName { get; set; }
        public string DesgName { get; set; }
        public decimal Amount { get; set; }
    }
    public class EmployeeVM
    {
        public int EmpId { get; set; }
        public string EmpName { get; set; }
        public string FName { get; set; }
        public int DesgId { get; set; }
        public int DeptId { get; set; }
        public int? LocId { get; set; }

        public int ShiftId { get; set; }
        public string CNIC { get; set; }
        public string Address { get; set; }
        public Nullable<System.DateTime> DOB { get; set; }
        public Nullable<System.DateTime> DOJ { get; set; }
        public string Gender { get; set; }
        public string BloodGrp { get; set; }
        public string MaritalStatus { get; set; }
        public string Status { get; set; }
        public string Mobile1 { get; set; }
        public string Mobile2 { get; set; }
        public string Email { get; set; }
        public string Religion { get; set; }
        public string Remarks { get; set; }
        public string RefPerson { get; set; }
        public string RefContactNo { get; set; }
        public string RefCNIC { get; set; }
        public decimal CurrentSalary { get; set; }
        public string RefAddress { get; set; }
        public DateTime TransDate { get; set; }
        public int UserId { get; set; }
        public int HDeptId { get; set; }
        public int? RptTo { get; set; }
        public string DesgName { get; set; }
        public string DeptName { get; set; }
        public string Color { get; set; }
        public Nullable<int> PayScaleId { get; set; }
        public Nullable<int> Weekend { get; set; }
        public Nullable<int> WeeklyHoliday { get; set; }
        public string JobType { get; set; }

        public List<int> EmpRolesIds { get; set; }
        public string FinalReason { get; set; }
        public Nullable<System.DateTime> JoinDate { get; set; }
        public Nullable<int> AppointmentTypeId { get; set; }
        public string JoinReason { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal NewSalary { get; set; }
        public int? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string AStatus { get; set; }
        public bool MSalary { get; set; }
        public bool IsFaceAllow { get; set; }
        public bool IsFinalized { get; set; }
        public bool BlockSalary { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }

        public Nullable<System.DateTime> CNICExpireDate { get; set; }
        public bool BsExist { get; set; }
        public decimal Amount { get; set; }
        public decimal NewAmount { get; set; }

    }
    public class BranchEmployeeVM
    {
        public int? EmpId { get; set; }
        public string EmpName { get; set; }
        public string FName { get; set; }
        public string Status { get; set; }
        public int DesgId { get; set; }
        public int DeptId { get; set; }
        public int ShiftId { get; set; }
        public string CNIC { get; set; }
        public string Address { get; set; }
        public Nullable<System.DateTime> DOB { get; set; }
        public Nullable<System.DateTime> DOJ { get; set; }
        public string Gender { get; set; }
        public string BloodGrp { get; set; }
        public string MaritalStatus { get; set; }
        public string Mobile1 { get; set; }
        public string Mobile2 { get; set; }
        public string Email { get; set; }
        public string Religion { get; set; }
        public string Remarks { get; set; }
        public string RefPerson { get; set; }
        public string RefContactNo { get; set; }
        public string RefCNIC { get; set; }
        public string RefAddress { get; set; }
        public Nullable<int> PayScaleId { get; set; }
        public Nullable<int> Weekend { get; set; }
        public Nullable<int> WeeklyHoliday { get; set; }
        public string JobType { get; set; }
        public Nullable<int> AppointmentTypeId { get; set; }
        public int? LocId { get; set; }
        public int ApprovedBy { get; set; }
        public DateTime ApprovedDate { get; set; }
        public DateTime TransDate { get; set; }
        public int UserId { get; set; }
        public int HDeptId { get; set; }
        public int? RptTo { get; set; }
        public string DesgName { get; set; }
        public string DeptName { get; set; }
        public string Color { get; set; }
        public List<int> EmpRolesIds { get; set; }
        public string FinalReason { get; set; }
        public Nullable<System.DateTime> JoinDate { get; set; }
        public string JoinReason { get; set; }
        public decimal BasicSalary { get; set; }
        public bool AttendanceStatus { get; set; }

    }
    public class EmpRoleVM
    {
        public int EROle { get; set; }
        public string RoleName { get; set; }
    }
    public class BloodGroupVM
    {
        public string id { get; set; }
        public string gendern { get; set; }
    }
    public class EducationVM
    {
        public int RowId { get; set; }
        public int EmpId { get; set; }
        public int QualificationId { get; set; }
        public System.DateTime FromDate { get; set; }
        public System.DateTime ToDate { get; set; }
        public string Institute { get; set; }
    }

    public class ExperienceVM
    {
        public int RowId { get; set; }
        public int EmpId { get; set; }
        public string Company { get; set; }
        public System.DateTime FromDate { get; set; }
        public System.DateTime ToDate { get; set; }
        public string Designation { get; set; }
    }
    public class ModelPairPolicyVM
    {
        public int PolicyId { get; set; }
        public int ModelId { get; set; }
        public int FOCModelId { get; set; }
        public int RatioPurQty { get; set; }
        public int RatioGiftQty { get; set; }
        public decimal ExtraCharges { get; set; }
        public string Remarks { get; set; }
        public System.DateTime FromDate { get; set; }
        public System.DateTime ToDate { get; set; }
    }
    public class ModelDiscountVM
    {
        public int PolicyId { get; set; }
        public System.DateTime StartDate { get; set; }
        public System.DateTime EndDate { get; set; }
        public int TypeId { get; set; }
        public List<int> ModelLst { get; set; }
        public List<ModelDiscountSlabVM> SlabLst { get; set; }
    }
    public class ModelDiscountSlabVM
    {
        public long RowId { get; set; }
        public int PolicyId { get; set; }
        public int MinSlabQty { get; set; }
        public int MaxSlabQty { get; set; }
        public decimal IncentiveAmt { get; set; }
        public decimal ExtraQtyRate { get; set; }
        public string Tour { get; set; }
    }
    public class DiscTypeVM
    {
        public int PDiscTypeId { get; set; }
        public string DiscType { get; set; }
    }
    public class ItmDiscVM
    {
        public long RowId { get; set; }
        public int SuppId { get; set; }
        public string Supplier { get; set; }
        public int ProductId { get; set; }
        public int PDiscTypeId { get; set; }
        public string DiscType { get; set; }
        public string ProductName { get; set; }
        public decimal Amount { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
    public class SaleRateVM
    {
        public long RowId { get; set; }
        public int SKUId { get; set; }
        public DateTime EffectiveDate { get; set; }
        public decimal SM { get; set; }
        public decimal SR { get; set; }
        public decimal SKT { get; set; }
        public decimal Gala { get; set; }
        public DateTime EndDate { get; set; }
    }
    public class PPriceVM
    {
        public long RowId { get; set; }
        public int SuppId { get; set; }
        public string Supplier { get; set; }
        //public int SuppId { get; set; }
        [Required]
        public int SKUId { get; set; }
        [Required]
        public string SKU { get; set; }
        [Required]

        public decimal TP { get; set; }
        [Required]
        public decimal MRP { get; set; }
        public decimal Discount { get; set; }
        public decimal MonthlyIncentive { get; set; }
        public decimal QuarterlyIncentive { get; set; }
        public decimal BiannuallyIncentive { get; set; }
        public decimal AnnuallyIncentive { get; set; }
        //public decimal GST { get; set; }
        //public decimal WHT { get; set; }
        //public decimal NetPrice { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Remarks { get; set; }
    }
    public class ItemCashPriceVM
    {
        public string City { get; set; }
        public string Location { get; set; }
        public string Type { get; set; }
        public string Model { get; set; }
        public int SKUId { get; set; }
        public string SKU { get; set; }
        public decimal CashPrice { get; set; }
        //public decimal MRP { get; set; }
        //public System.DateTime EffectedFrom { get; set; }
    }
    public class TypePlanVM
    {
        public int PolicyId { get; set; }
        public List<int> Type { get; set; }
        public string Title { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Duration { get; set; }
        public decimal MinAdvance { get; set; }
        public decimal MaxAdvance { get; set; }
        public decimal MarkUp { get; set; }
        public bool Status { get; set; }
        public List<int> Loc { get; set; }
        public bool IsLocal { get; set; }
        public decimal RegFee { get; set; }
    }
    public class SubsidaryVM
    {
        public long SubId { get; set; }
        public string SubsidaryCode { get; set; }
        public string SubsidaryName { get; set; }
        public string SubCode { get; set; }
        public int SubTypeId { get; set; }
        public string OldCode { get; set; }
        public string ContactPerson { get; set; }
        public string ContactNo { get; set; }
        public Nullable<int> CityId { get; set; }
        public long AccId { get; set; }
    }
    public class SubsidaryTypeVM
    {
        public int SubTypeId { get; set; }
        public string SubType { get; set; }
        public string AccCode { get; set; }
        public bool Status { get; set; }
        public string OldCode { get; set; }
        public int CatId { get; set; }
        public string Abbr { get; set; }
        public long AccId { get; set; }
    }
    public class SubsiCategoryVM
    {
        public int CatId { get; set; }
        public string Category { get; set; }
    }
    public class PreOrderMobileVM
    {
        public int RowId { get; set; }
        public string City { get; set; }
        public int LocId { get; set; }
        public string LocName { get; set; }
        public int Branches { get; set; }
        public int ModelId1 { get; set; }
        public int ModelId2 { get; set; }
        public int ModelId3 { get; set; }
        public int ModelId4 { get; set; }
        public int ModelId5 { get; set; }
        public int ModelId6 { get; set; }
        public int ModelId7 { get; set; }
        public int ModelId8 { get; set; }
        public int ModelId9 { get; set; }
        public int ModelId10 { get; set; }
        public string Model1 { get; set; }
        public string Model2 { get; set; }
        public string Model3 { get; set; }
        public string Model4 { get; set; }
        public string Model5 { get; set; }
        public string Model6 { get; set; }
        public string Model7 { get; set; }
        public string Model8 { get; set; }
        public string Model9 { get; set; }
        public string Model10 { get; set; }
        public int SKUId1 { get; set; }
        public int SKUId2 { get; set; }
        public int SKUId3 { get; set; }
        public int SKUId4 { get; set; }
        public int SKUId5 { get; set; }
        public int SKUId6 { get; set; }
        public int SKUId7 { get; set; }
        public int SKUId8 { get; set; }
        public int SKUId9 { get; set; }
        public int SKUId10 { get; set; }
        public string SKU1 { get; set; }
        public string SKU2 { get; set; }
        public string SKU3 { get; set; }
        public string SKU4 { get; set; }
        public string SKU5 { get; set; }
        public string SKU6 { get; set; }
        public string SKU7 { get; set; }
        public string SKU8 { get; set; }
        public string SKU9 { get; set; }
        public string SKU10 { get; set; }
        public int Pending1 { get; set; }
        public int Pending2 { get; set; }
        public int Pending3 { get; set; }
        public int Pending4 { get; set; }
        public int Pending5 { get; set; }
        public int Pending6 { get; set; }
        public int Pending7 { get; set; }
        public int Pending8 { get; set; }
        public int Pending9 { get; set; }
        public int Pending10 { get; set; }
        public int Sale1 { get; set; }
        public int Sale2 { get; set; }
        public int Sale3 { get; set; }
        public int Sale4 { get; set; }
        public int Sale5 { get; set; }
        public int Sale6 { get; set; }
        public int Sale7 { get; set; }
        public int Sale8 { get; set; }
        public int Sale9 { get; set; }
        public int Sale10 { get; set; }
        public int Stock1 { get; set; }
        public int Stock2 { get; set; }
        public int Stock3 { get; set; }
        public int Stock4 { get; set; }
        public int Stock5 { get; set; }
        public int Stock6 { get; set; }
        public int Stock7 { get; set; }
        public int Stock8 { get; set; }
        public int Stock9 { get; set; }
        public int Stock10 { get; set; }
        public int Order1 { get; set; }
        public int Order2 { get; set; }
        public int Order3 { get; set; }
        public int Order4 { get; set; }
        public int Order5 { get; set; }
        public int Order6 { get; set; }
        public int Order7 { get; set; }
        public int Order8 { get; set; }
        public int Order9 { get; set; }
        public int Order10 { get; set; }
        public int SOrder1 { get; set; }
        public int SOrder2 { get; set; }
        public int SOrder3 { get; set; }
        public int SOrder4 { get; set; }
        public int SOrder5 { get; set; }
        public int SOrder6 { get; set; }
        public int SOrder7 { get; set; }
        public int SOrder8 { get; set; }
        public int SOrder9 { get; set; }
        public int SOrder10 { get; set; }
    }

    public class LSECERVM
    {
        public long RowId { get; set; }
        public int LocId { get; set; }
        public long AccNo { get; set; }
        public string Customer { get; set; }
        public System.DateTime WorkingDate { get; set; }
        public System.DateTime TransDate { get; set; }
        public int UserId { get; set; }
        public bool Status { get; set; }
        public decimal MonthlyInst { get; set; }
        public string FName { get; set; }
        public decimal InstPrice { get; set; }
        public string LocCode { get; set; }
    }
    public class VoucherTypeVM
    {
        public string VrTypeId { get; set; }
        public string VrTypeDesc { get; set; }
    }
    public class DayClosingVM
    {
        public int TransId { get; set; }
        public int LocId { get; set; }
        public System.DateTime WorkingDate { get; set; }
        public System.DateTime DayStartDate { get; set; }
        public decimal OpeningCash { get; set; }
        public decimal ClosingCash { get; set; }
        public string Status { get; set; }
    }
    public class ExpenseVM
    {
        public int ExpHeadId { get; set; }
        public int ExpTypeId { get; set; }
        public string ExpHead { get; set; }
        public string GLCode { get; set; }
        public decimal MaxLimit { get; set; }
        public string ExpFor { get; set; }
        public string SubCode { get; set; }
    }

    public class SystemIntegrationVM
    {
        public int TransId { get; set; }
        public string TransDescription { get; set; }
        public string GLCode { get; set; }
    }
    public class ExpenseTransactionVM
    {
        public long TransId { get; set; }
        public System.DateTime WorkingDate { get; set; }
        public int LocId { get; set; }
        public int ExpHeadId { get; set; }
        public string Description { get; set; }
        public string RefBillNo { get; set; }
        public decimal Amount { get; set; }
        public long CCCode { get; set; }
        public string UploadedFiles { get; set; }
        public bool IsPosted { get; set; }
        public long? SubId { get; set; }
        public long? TicketId { get; set; }
    }
    public class CashPaymentVM
    {
        public long TransId { get; set; }
        public System.DateTime WorkingDate { get; set; }
        public int LocId { get; set; }
        public long AccId { get; set; }
        public long SubId { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public int UserId { get; set; }
        public System.DateTime TransDate { get; set; }
        public string Status { get; set; }
        public long CCCode { get; set; }
        public bool IsPosted { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<System.DateTime> PostedDate { get; set; }
        public Nullable<int> PostedBy { get; set; }

        public string UploadedFiles { get; set; }
    }
    public class CashReceiveVM
    {
        public long TransId { get; set; }
        //public System.DateTime WorkingDate { get; set; }
        public int LocId { get; set; }
        public long? AccId { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        //public int UserId { get; set; }
        //public System.DateTime TransDate { get; set; }
        //public string Status { get; set; }
        public Nullable<long> CCCode { get; set; }
        public long? SubId { get; set; }
        public bool IsPosted { get; set; }
        //public Nullable<int> ModifiedBy { get; set; }
        //public Nullable<System.DateTime> ModifiedDate { get; set; }
        //public Nullable<System.DateTime> PostedDate { get; set; }
        //public Nullable<int> PostedBy { get; set; }
        public string UploadedFiles { get; set; }
    }
    public class CashCollectionVM
    {
        public long DocDtlId { get; set; }
        public long DocId { get; set; }
        public int CashHeadId { get; set; }
        public string CashHead { get; set; }
        public decimal CashAmount { get; set; }
        public DateTime CashierDate { get; set; }
        //public long AccId { get; set; }

        //public int CashierId { get; set; }
    }
    public class CashCollectionListVM
    {
        public long DocId { get; set; }
        public string LocName { get; set; }
        public DateTime WorkingDate { get; set; }
        public string DocType { get; set; }
        public string RecvBy { get; set; }
        public long CashierId { get; set; }
        public decimal CashPayable { get; set; }
        public decimal CashAmount { get; set; }
        //public decimal ReceivedAmount { get; set; }
        public DateTime CashierDate { get; set; }
        public string Remarks { get; set; }
        //public bool IsReceived { get; set; }
    }

    public class OrderManagerDashboardVM
    {
        public string ComName { get; set; }
        public string SupplierName { get; set; }
        public string PONo { get; set; }
        public Nullable<int> Pending { get; set; }
        public Nullable<int> Recived { get; set; }
        public long POId { get; set; }
        public System.DateTime PODate { get; set; }
        public int POStatusId { get; set; }
        public string POStatus { get; set; }
    }


    #region PayrolVMS
    public class DesignationVM
    {
        public int DesgId { get; set; }
        public string DesgName { get; set; }
        public int? ReportingTo { get; set; }
        public bool Status { get; set; }
        public int? SectionId { get; set; }
    }

    public class DesignationSectionVM
    {
        public int SectionId { get; set; }
        public string SectionTitle { get; set; }
    }
    public class DepartmentVM
    {
        public int DeptId { get; set; }
        public string DeptName { get; set; }
        public int? LocId { get; set; }
        public int HDepId { get; set; }
        public bool status { get; set; }
    }

    public class FinancialPeriodVM
    {
        public int YrCode { get; set; }
        public int PrCode { get; set; }
        public string PrName { get; set; }
        public System.DateTime PrStart { get; set; }
        public System.DateTime PrEnd { get; set; }
        public string PrStatus { get; set; }
    }

    public class BranchStaffStrengthAEVM
    {
        public List<int> LocId { get; set; }
        public int DesgId { get; set; }
        public int StaffStrength { get; set; }
        public int UserId { get; set; }
    }
    public class BranchStrengthVM
    {
        public int DeptId { get; set; }
        public string DeptName { get; set; }
        public int DesgId1 { get; set; }
        public int DesgId2 { get; set; }
        public int DesgId3 { get; set; }
        public int DesgId4 { get; set; }
        public int DesgId5 { get; set; }
        public int DesgId6 { get; set; }
        public int DesgId7 { get; set; }
        public int DesgId8 { get; set; }
        public int DesgId9 { get; set; }
        public int DesgId10 { get; set; }
        public int DesgId11 { get; set; }
        public int DesgId12 { get; set; }
        public int DesgId13 { get; set; }
        public int DesgId14 { get; set; }
        public int DesgId15 { get; set; }
        public int DesgId16 { get; set; }
        public int DesgId17 { get; set; }
        public int DesgId18 { get; set; }

        public string DesgName1 { get; set; }
        public string DesgName2 { get; set; }
        public string DesgName3 { get; set; }
        public string DesgName4 { get; set; }
        public string DesgName5 { get; set; }
        public string DesgName6 { get; set; }
        public string DesgName7 { get; set; }
        public string DesgName8 { get; set; }
        public string DesgName9 { get; set; }
        public string DesgName10 { get; set; }
        public string DesgName11 { get; set; }
        public string DesgName12 { get; set; }
        public string DesgName13 { get; set; }
        public string DesgName14 { get; set; }
        public string DesgName15 { get; set; }
        public string DesgName16 { get; set; }
        public string DesgName17 { get; set; }
        public int ApprovedStrength1 { get; set; }
        public int ApprovedStrength2 { get; set; }
        public int ApprovedStrength3 { get; set; }
        public int ApprovedStrength4 { get; set; }
        public int ApprovedStrength5 { get; set; }
        public int ApprovedStrength6 { get; set; }
        public int ApprovedStrength7 { get; set; }
        public int ApprovedStrength8 { get; set; }
        public int ApprovedStrength9 { get; set; }
        public int ApprovedStrength10 { get; set; }
        public int ApprovedStrength11 { get; set; }
        public int ApprovedStrength12 { get; set; }
        public int ApprovedStrength13 { get; set; }
        public int ApprovedStrength14 { get; set; }
        public int ApprovedStrength15 { get; set; }
        public int ApprovedStrength16 { get; set; }
        public int ApprovedStrength17 { get; set; }
    }

    public class AttendanceExceptionVM
    {
        public Nullable<long> AttnId { get; set; }
        public Nullable<int> EmpId { get; set; }
        public string EmpName { get; set; }
        public string DeptName { get; set; }
        public Nullable<System.DateTime> Attendancedt { get; set; }
        public string Status { get; set; }
        public string AttnType { get; set; }
        //public Nullable<System.DateTime> InTime { get; set; }
        //public Nullable<System.DateTime> OutTime { get; set; }
        public string InTime { get; set; }
        public string OutTime { get; set; }
        public string AutoDescription { get; set; }
        public Nullable<System.DateTime> TransDate { get; set; }
        public Nullable<decimal> WorkingHours { get; set; }
    }
    public class ShiftVM
    {
        public int ShiftId { get; set; }
        public string ShiftTitle { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public Nullable<int> GraceTime { get; set; }
        public bool status { get; set; }
    }

    public class AppointmentTypeVM
    {
        public int AppointmentTypeId { get; set; }
        public string AppointmentType { get; set; }
        public bool Status { get; set; }
    }

    public class HDepartmentVM
    {
        public int HDeptId { get; set; }
        public string HDeptName { get; set; }
    }
    public class Qualification_VM
    {
        public int QualificationId { get; set; }
        public string QualificationTitle { get; set; }
        public int EmpId { get; set; }
        public bool Status { get; set; }
    }
    public class AllowanceVM
    {
        public int AlwId { get; set; }
        public string AlwName { get; set; }
        public decimal Amount { get; set; }
        public string AlwType { get; set; }
        public bool Status { get; set; }
    }
    public class VariableAllowanceVM
    {
        public long TransId { get; set; }
        public int AlwId { get; set; }
        public int EmpId { get; set; }
        public decimal Amount { get; set; }
        public string Remarks { get; set; }
        public string TransType { get; set; }
        public DateTime EffectiveDate { get; set; }
        public int UserId { get; set; }
        public System.DateTime TransDate { get; set; }
        public bool Status { get; set; }
        public string EmployeeName { get; set; }
        public string CNIC { get; set; }
    }
    public class VariableDeductionVM
    {
        public long TransId { get; set; }
        public int DedId { get; set; }
        public int EmpId { get; set; }
        public decimal Amount { get; set; }
        public string Remarks { get; set; }
        public DateTime EffectiveDate { get; set; }
        public int UserId { get; set; }
        public System.DateTime TransDate { get; set; }
        public bool Status { get; set; }
        public string EmployeeName { get; set; }
        public string CNIC { get; set; }
    }
    public class DeductionVM
    {
        public int DedId { get; set; }
        public string DedName { get; set; }
        public decimal Amount { get; set; }
        public bool Status { get; set; }
    }
    public class Allowance_Allocation_VM
    {
        public long AllocId { get; set; }
        public Nullable<int> DesgId { get; set; }
        public Nullable<int> DeptId { get; set; }
        public Nullable<int> EmpId { get; set; }
        public int AlwId { get; set; }
        public int UserId { get; set; }
        public bool Status { get; set; }
        public System.DateTime TransDate { get; set; }
    }
    public class AllowanceTypeVM
    {
        public string AlwType { get; set; }
        public string AlwName { get; set; }

    }

    public class AllowanceAllocationVM
    {
        public long AllocId { get; set; }
        public int AlwId { get; set; }
        public int TypeId { get; set; }
        public int DesgId { get; set; }
        public int DeptId { get; set; }
        public string AlwName { get; set; }
        public int EmpId { get; set; }
        public bool IsSelected { get; set; }
        public int UserId { get; set; }
        public bool Status { get; set; }
    }

    public class DeductionAllocationVM
    {
        public long AllocId { get; set; }
        public int DedId { get; set; }
        public int TypeId { get; set; }
        public int DesgId { get; set; }
        public int DeptId { get; set; }
        public string DedName { get; set; }
        public int EmpId { get; set; }
        public bool IsSelected { get; set; }
        public int UserId { get; set; }
        public bool Status { get; set; }
    }
    public class EmployeeLeaveVM
    {
        public long LeaveId { get; set; }
        public int EmpId { get; set; }
        public string EmpName { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        public string LeaveTypeId { get; set; }
        public string LeaveCatId { get; set; }
        public System.DateTime LeaveFromDate { get; set; }
        public System.DateTime LeaveToDate { get; set; }
        public string LeaveReason { get; set; }
        public int UserId { get; set; }
        public System.DateTime TransDate { get; set; }
        public Nullable<int> ApprovedBy { get; set; }
        public string ApprovedByName { get; set; }
        public bool ApprovedHead { get; set; }
        public bool HeadApproval { get; set; }
        public bool HRApproval { get; set; }
        public bool RejectedHead { get; set; }
        public Nullable<System.DateTime> ApprovedDate { get; set; }
        public string Remarks { get; set; }
        public Nullable<int> ApprovedByHR { get; set; }
        public string ApprovedByHRName { get; set; }
        public bool ApprovedHR { get; set; }

        public bool RejectedHR { get; set; }
        public Nullable<System.DateTime> ApprovedDateHR { get; set; }
        public string RemarksByHR { get; set; }
        public string LeaveStatus { get; set; }

        public bool AlreadyApprovedHead { get; set; }
        public bool AlreadyApprovedHR { get; set; }
    }
    public class LeaveTypeVM
    {
        public string LeaveTypeId { get; set; }
        public string LeaveType { get; set; }
        public int DisplayOrder { get; set; }
    }
    public class LeaveCategoryVM
    {
        public string LeaveCatId { get; set; }
        public string LeaveCat { get; set; }
        public int DisplayOrder { get; set; }
    }

    public class LeaveRosterVM : ISchedulerEvent
    {
        public long TransId { get; set; }
        public int EmpId { get; set; }
        public DateTime TransDate { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsAllDay { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string StartTimezone { get; set; }
        public string EndTimezone { get; set; }
        public string RecurrenceRule { get; set; }
        public string RecurrenceException { get; set; }
        public int[] employees { get; set; }
        public string empname { get; set; }
        public string Color { get; set; }
        public Nullable<int> ApprovedBy { get; set; }
        public Nullable<System.DateTime> ApprovedDate { get; set; }
        public bool HasApprovalRights { get; set; }
    }

    public class EmployeeHirarchyAllocationVM
    {
        public int TransId { get; set; }
        public int? EmpId { get; set; }
        public string EmployeeName { get; set; }
        public int? ReportingTo { get; set; }
        public string ReportingName { get; set; }
        public int DefinedBy { get; set; }
        public DateTime DefinedDate { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
    public class GeoLocationVM
    {
        public int GeoId { get; set; }
        public string GTitle { get; set; }
        public string GLevel { get; set; }
        public int ParentId { get; set; }
        public string ParentTitle { get; set; }
        public int EmpId { get; set; }
        public int DesgId { get; set; }
        public string DesgName { get; set; }
        public string EmployeeName { get; set; }
        public string GLvl { get; set; }
    }
    public class EmployeeGeoLocationMappingVM
    {
        public int TransId { get; set; }
        public int EmpId { get; set; }
        public string EmployeeName { get; set; }
        public List<int> LocId { get; set; }
        public string location { get; set; }
        public int DefinedBy { get; set; }
        public string CNIC { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        //public List<int> LocationIds { get; set; }
        public string LocationName { get; set; }
        public int GeoId { get; set; }

    }
    public class EmployeeLocationMappingVM
    {
        public int EmpId { get; set; }
        public string EmployeeName { get; set; }
        public string CNIC { get; set; }
        public List<int> LocId { get; set; }

        public string LocationName { get; set; }
    }
    public class EmployeeLocationMappingLogVM
    {
        public int LogId { get; set; }
        public int TransId { get; set; }
        public int EmpId { get; set; }
        public int LocId { get; set; }
        public int DefinedBy { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
    public class EmployeeLoanListVM
    {
        public int LoanId { get; set; }
        public int LoanTypeID { get; set; }
        public string LoanType { get; set; }
        public string AdvType { get; set; }
        public int EmpId { get; set; }
        public string EmpName { get; set; }
        public decimal LoanAmt { get; set; }
        public string CNIC { get; set; }
        public decimal Inst { get; set; }
        public decimal LoanBal { get; set; }
        public System.DateTime IssueDate { get; set; }
        public System.DateTime DedStartDate { get; set; }
        public int? ApprovedBy { get; set; }
        public string ApproveBy { get; set; }
        public bool AutoDeduct { get; set; }
        public bool Status { get; set; }
        public string Remarks { get; set; }
        public int UserId { get; set; }
        public bool Approve { get; set; }
        public System.DateTime TransDate { get; set; }
        public bool HasApprovalRights { get; set; }
        public Nullable<int> AdvTypeID { get; set; }
    }
    public class LoanTypesVM
    {
        public int LoanTypeId { get; set; }
        public string LoanType { get; set; }
    }
    public class EmpFineVM
    {
        public string EmployeeName { get; set; }
        public int DocId { get; set; }
        public System.DateTime DocDate { get; set; }
        public System.DateTime FineDate { get; set; }
        public int? CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<int> CheckedBy { get; set; }
        public Nullable<System.DateTime> CheckedDate { get; set; }
        public Nullable<int> ApprovedBy { get; set; }
        public Nullable<System.DateTime> ApprovedDate { get; set; }
        public string Remarks { get; set; }
        public bool Approve { get; set; }
        public string ApprovedName { get; set; }
        public bool HasLeaveApprovalRights { get; set; }
        public List<EmpFineDetailVM> EmpFineDetail { get; set; }
    }
    public class EmpFineDetailVM
    {
        public long DocDtlId { get; set; }
        public int DocId { get; set; }
        public int EmpId { get; set; }
        public int FineTypeId { get; set; }
        public decimal FineActual { get; set; }
        public decimal FineApproved { get; set; }
        public string Remarks { get; set; }
        public string EmployeeName { get; set; }
    }
    public class EmployeeBankSalary
    {
        public int TransId { get; set; }
        public int EmpId { get; set; }
        public string EmpName { get; set; }
        public string CNIC { get; set; }
        public string Designation { get; set; }
        public decimal FixedSalary { get; set; }
        public decimal BankSalary { get; set; }
        public string AccountNumber { get; set; }
        public string AccountTitle { get; set; }
        public string Bank { get; set; }
        public decimal StartRange { get; set; }
        public decimal EndRange { get; set; }
        public bool IsFullBank { get; set; }

    }
    public class FineTypesVM
    {
        public int FineTypeId { get; set; }
        public string FineType { get; set; }
    }
    public class EmployeeSalaryVM
    {
        public string TypeId { get; set; }
        public List<int> EmployeeId { get; set; }
        public int EmployeeIds { get; set; }
        public string SalaryType { get; set; }
        public int AmountPercentage { get; set; }
        public decimal Amount { get; set; }
        public DateTime EffectiveDate { get; set; }
        public string Remarks { get; set; }
        public bool IsPercentage { get; set; }
        public bool IsAmount { get; set; }

    }
    public class EmployeeCalculatedSalaryVM
    {
        public int id { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string Department { get; set; }
        public string CNIC { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal IncrementAmount { get; set; }
        public decimal GrossSalary { get; set; }
        public string SalaryType { get; set; }
        public string Remarks { get; set; }
        public DateTime EffectiveDate { get; set; }
        public int EmployeeIds { get; set; }
    }
    public class SalaryTypeVM
    {
        public string SalTypeId { get; set; }
        public string SalTypeDesc { get; set; }
        public int MF { get; set; }
        public bool Status { get; set; }
    }
    public class EmpTemplateVM
    {
        public int EmpId { get; set; }
        public string Template { get; set; }
    }
    public class SalesVoucherVM
    {
        public string City { get; set; }
        public string Location { get; set; }
        public int LocId { get; set; }
        public decimal Cash { get; set; }
        public decimal Card { get; set; }
        public decimal Credit { get; set; }
        public decimal Inst { get; set; }
    }
    public class SaleForVoucherPostingVM
    {
        public int LocId { get; set; }
        public long TransId { get; set; }
        public string BillNo { get; set; }
        public System.DateTime BillDate { get; set; }
        public long CustId { get; set; }
        public string CustName { get; set; }
        public string CustCellNo { get; set; }
        public decimal PPrice { get; set; }
        public decimal SPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal Advance { get; set; }
    }
    public class SaleByTypeVM
    {
        public long TransId { get; set; }
        public string BillNo { get; set; }
        public string TransactionType { get; set; }
        public string SKUCode { get; set; }
        public string SerialNo { get; set; }
        public Nullable<decimal> SPrice { get; set; }
    }
    public class TransactionTypeVM
    {
        public int TransactionTypeId { get; set; }
        public string TransactionType { get; set; }
        public string GLCode { get; set; }
    }
    public class AttendanceStatusVM
    {
        public string StatusId { get; set; }

        public string Status { get; set; }

    }
    public class PayScaleVM
    {
        public int BPS { get; set; }
        public int MinSalary { get; set; }
        public int Increment { get; set; }
        public int MaxSalary { get; set; }
        public int Stages { get; set; }
    }

    public class PayEmpChequeVM
    {
        public long ChequeId { get; set; }
        public long EmpId { get; set; }
        public string ChequeType { get; set; }
        public string ChequeNo { get; set; }
        public string BankName { get; set; }
        public decimal ChequeAmount { get; set; }
        public bool IsChequeReturn { get; set; }
        public bool ChequeStatus { get; set; }
    }
    public class PayEmpSalaryVM
    {
        public long EmpId { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal BankSalary { get; set; }
        public string department { get; set; }
        public string CNIC { get; set; }
        public DateTime Month { get; set; }
    }
    public class GeoHirVM
    {
        public int GeoId { get; set; }
        public string GLevel { get; set; }
        public string GTitle { get; set; }
        public int ParentId { get; set; }
        public int EmpId { get; set; }
        public List<int> LocId { get; set; }
    }
    public class BasicSalaryPolicyVM
    {
        public int PolicyId { get; set; }
        public string PolicyTitle { get; set; }
        public int DesigId { get; set; }
        public string Designation { get; set; }
        public int SlabId { get; set; }
        public string SlabTitle { get; set; }
        public int DefinedBy { get; set; }
        public DateTime EffectiveDate { get; set; }
        public bool Status { get; set; }
    }
    public class BasicSalarySlabVM
    {
        public int SlabId { get; set; }
        public string SlabTitle { get; set; }
        public bool Status { get; set; }
        public string StatusId { get; set; }
        public int DefinedBy { get; set; }
        public string Defined { get; set; }
        public System.DateTime DefinedDate { get; set; }
        public List<BasicSalarySlabDtlVM> SalarySlabLst { get; set; }
    }

    public class BasicSalarySlabDtlVM
    {
        public int TransId { get; set; }
        public int SlabId { get; set; }
        public decimal SlabStart { get; set; }
        public decimal SlabEnd { get; set; }
        public decimal BasicSalary { get; set; }
    }

    public class BasicSalaryPolicyNVM
    {
        public int PolicyId { get; set; }
        public string PolicyTitle { get; set; }
        public int DesgId { get; set; }
        public int SlabId { get; set; }
        public int PerformanceTypeId { get; set; }
        public System.DateTime EffectiveDate { get; set; }
        public int DefinedBy { get; set; }
        public string Defined { get; set; }
        public System.DateTime DefinedDate { get; set; }
        public Nullable<int> ApprovedBy { get; set; }
        public Nullable<System.DateTime> ApprovedDate { get; set; }

        public string Remarks { get; set; }
        public bool Status { get; set; }
        public string StatusId { get; set; }
        public string CityLst { get; set; }
        public string LocLst { get; set; }
        public int[] CCtyLst { get; set; }
        public int[] CLcLst { get; set; }
    }
    public class BasicSalaryPolicyLocationsVM
    {
        public int RowId { get; set; }
        public int PolicyId { get; set; }
        public int CityId { get; set; }
        public int LocId { get; set; }

    }

    public class PayIncentiveVM
    {
        public bool selectable { get; set; }
        public int PolicyId { get; set; }
        public string PolicyTitle { get; set; }
        public int DesgId { get; set; }
        public int AlwId { get; set; }
        public decimal IncVal { get; set; }
        public decimal IncPer { get; set; }
        public bool Status { get; set; }
        public int DefinedBy { get; set; }
        public string Defined { get; set; }
        public System.DateTime DefinedDate { get; set; }
        public string StatusId { get; set; }
        public int[] CCtyLst { get; set; }
        public int[] CLcLst { get; set; }
    }
    public class PayIncentiveLocationVM
    {
        public int RowId { get; set; }
        public int PolicyId { get; set; }
        public int CityId { get; set; }
        public int LocId { get; set; }
    }
    #endregion

    #region PerformanceType
    public class PerformanceTypeVM
    {
        public int PerformanceTypeId { get; set; }
        public string PerformanceType { get; set; }
    }

    public class EmpPerformanceTypeVM
    {
        public int RowId { get; set; }
        public int TransId { get; set; }
        public System.DateTime PerformanceMonth { get; set; }
        public int PerformanceTypeId { get; set; }
        public string City { get; set; }
        public string Branch { get; set; }

        public int ProcessedBy { get; set; }
        public System.DateTime ProcessedDate { get; set; }
        public Nullable<int> ValidatedBy { get; set; }
        public Nullable<System.DateTime> ValidatedDate { get; set; }
        public Nullable<int> ApprovedBy { get; set; }
        public Nullable<System.DateTime> ApprovedDate { get; set; }
        public long TransDtlId { get; set; }
        public int LocId { get; set; }
        public int EmpId { get; set; }
        public decimal TargetQty { get; set; }
        public decimal TargetValue { get; set; }
        public decimal AchQty { get; set; }
        public decimal AchValue { get; set; }
        public decimal IncentivePercent { get; set; }
        public decimal IncentiveValue { get; set; }
        public decimal ApprovedValue { get; set; }
        public string Remarks { get; set; }
        public string EmpName { get; set; }
        public string CNIC { get; set; }
        public string Designation { get; set; }
        public string PerformanceType { get; set; }
        public string ProcessedByName { get; set; }

        public string ValidatedByName { get; set; }
        public string ApprovedByName { get; set; }
        public string HeaderOne { get; set; }
        public string HeaderTwo { get; set; }
    }
    public class HeadersVM
    {
        public string HeaderOne { get; set; }
        public string HeaderTwo { get; set; }

    }



    #endregion
    public class AttendanceFaceVM
    {
        public int EmpId { get; set; }
        public string EmpName { get; set; }
        public string EmpPic { get; set; }
        public string IN { get; set; }
        public string OUT { get; set; }
        public System.DateTime AttendanceDate { get; set; }
    }
    public class CashSaleSummarySaleManWiseVM
    {
        public string LocName { get; set; }
        public int SalemanId { get; set; }
        public string Saleman { get; set; }
        public Nullable<int> Qty { get; set; }
        public Nullable<decimal> SaleVale { get; set; }
        public Nullable<int> RetQty { get; set; }
        public Nullable<decimal> RetValue { get; set; }
    }
    public class AttendanceVM
    {
        public long TransId { get; set; }
        public int EmpId { get; set; }
        public System.DateTime AttendanceDate { get; set; }
        public string StatusId { get; set; }
        public int UserId { get; set; }
        public int AttLocId { get; set; }
        public DateTime Todate { get; set; }
        public string AttnType { get; set; }
        public string Reason { get; set; }
        public System.DateTime TransDate { get; set; }
        public int WorkingHours { get; set; }
        public string EmpPic { get; set; }
        public string Long { get; set; }

        public string Lat { get; set; }
        public string IP { get; set; }
    }


    public class UserThemeSelVM
    {
        public string ThemeName { get; set; }
        public bool ThemeSel { get; set; }

    }
    public class UserPermissionVM
    {
        public int GroupId { get; set; }
        public string ActionLink { get; set; }
        public bool IsGroup { get; set; }
    }
    public class SalaryTestProcessVM
    {
        public Nullable<System.DateTime> pSalaryMonth { get; set; }
        public Nullable<int> pHDeptId { get; set; }
        public Nullable<int> pUserId { get; set; }
    }

    #region Documents
    public class DocumentTypesVM
    {
        public int Id { get; set; }
        public string DocType { get; set; }
        public string Path { get; set; }
    }

    public class DocumentVM
    {
        public long Id { get; set; }
        public int DocTypeId { get; set; }
        public long RefObjId { get; set; }
        public long Size { get; set; }
        public string Name { get; set; }
        public System.DateTime Created { get; set; }
        public string Path { get; set; }
        public string Extension { get; set; }
        public bool IsDirectory { get; set; }
        public bool HasDirectories { get; set; }
        public DateTime? Modified { get; set; }
        public string Remarks { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public string documentowner { get; set; }

        public bool Status { get; set; }
    }


    public class DocTransferVM
    {
        public int TransId { get; set; }
        public int FromLocId { get; set; }
        public int ToLocId { get; set; }
        public string Location { get; set; }
        public string Title { get; set; }
        public System.DateTime WorkingDate { get; set; }
        public System.DateTime TransDate { get; set; }
        public Nullable<System.DateTime> RecvDate { get; set; }
        public int UserId { get; set; }
        public Nullable<int> RecvBy { get; set; }
        public string Remarks { get; set; }
        public string Status { get; set; }
        public string UploadedFiles { get; set; }
    }
    #endregion

    #region MobileVM
    public class MUserVM
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public int GroupId { get; set; }
        public System.DateTime TransDate { get; set; }
        public bool Status { get; set; }
        public string DeviceId { get; set; }
        public int EmployeeId { get; set; }
        public Nullable<System.DateTime> LastLoginDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public List<LocationVM> LocId { get; set; }
        public string Msg { get; set; }
        public string DeviceName { get; set; }
        public string IP { get; set; }
        public List<MUserMenuInfo> MenuAccess { get; set; }
        public string UserPic { get; set; }
        public Nullable<int> FAcc { get; set; }
        public Nullable<decimal> FOutStand { get; set; }
        public Nullable<decimal> FRecAmt { get; set; }
        public Nullable<decimal> FPenAmt { get; set; }
        public Nullable<int> RAcc { get; set; }
        public Nullable<decimal> ROutStand { get; set; }
        public Nullable<decimal> RRecAmt { get; set; }
        public Nullable<decimal> RPenAmt { get; set; }
        public string EmpPic { get; set; }
        public int DesgId { get; set; }
        public string DesgName { get; set; }

        public string Token { get; set; }

        public bool IsFirstLogin { get; set; }

    }
    public class LocationVM
    {
        public int TransId { get; set; }
        public int LocId { get; set; }
        public string LocCode { get; set; }
        public string LocName { get; set; }
        public int CityId { get; set; }
        public string CityName { get; set; }
        public int EmpId { get; set; }
        public Nullable<decimal> Lat { get; set; }
        public Nullable<decimal> Long { get; set; }
    }
    public class MUserMenuInfo
    {
        public int ActivityId { get; set; }
        public string ActivityName { get; set; }
        public string Link { get; set; }
        public int ParentId { get; set; }
        public string MenuIcon { get; set; }
        public bool IsVisible { get; set; }
        public int AppType { get; set; }
        public Nullable<int> NoOfAcc { get; set; }
        public Nullable<decimal> OutStandAmount { get; set; }
        public Nullable<decimal> ReceivedAmount { get; set; }
        public Nullable<decimal> Pending { get; set; }
    }

    public class MLatLng
    {
        public long AccNo { get; set; }
        public decimal Lat { get; set; }
        public decimal Lng { get; set; }
        public string Type { get; set; }
    }
    public class MGurantorVM
    {
        public int GurantorId { get; set; }
        public long AccNo { get; set; }
        public string Name { get; set; }
        public string FName { get; set; }
        public string NIC { get; set; }
        public string Occupation { get; set; }
        public string ResAddress { get; set; }
        public string OffAddress { get; set; }
        public string TelRes { get; set; }
        public string TelOff { get; set; }
        public string GRelation { get; set; }
    }
    public class MCustomerInstallmentsVM
    {
        public DateTime Date { get; set; }
        public decimal RcvdAmnt { get; set; }
        public decimal Installment { get; set; }
        public decimal Discount { get; set; }
        public decimal Fine { get; set; }
        public string FineType { get; set; }
        public string RecoveryOfficer { get; set; }

    }

    public class MCustomerPurchaseVM
    {
        public string Company { get; set; }
        public string Product { get; set; }
        public string Model { get; set; }
        public string SKU { get; set; }
        public decimal InstallmentPrice { get; set; }
    }


    public class BikeLetterVM
    {
        public string LocCode { get; set; }
        public string SuppName { get; set; }
        public long GRNId { get; set; }
        public string GRNNo { get; set; }
        public string DONo { get; set; }
        public Nullable<System.DateTime> DODate { get; set; }
        public System.DateTime GRNDate { get; set; }
        public string TypeName { get; set; }
        public string SKUName { get; set; }
        public string SerialNo { get; set; }
        public bool LetterStatus { get; set; }
    }
    public class BikeLetterOldVM
    {
        public int RowId { get; set; }
        public string BillNo { get; set; }
        public System.DateTime BillDate { get; set; }
        public string DoNo { get; set; }
        public string LocCode { get; set; }
        public string LocName { get; set; }
        public string SuppName { get; set; }
        public string ItemName { get; set; }
        public string Model { get; set; }
        public string CompName { get; set; }
        public string SrNo { get; set; }
        public Nullable<decimal> PPrice { get; set; }
        public Nullable<decimal> SPrice { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string LetterNumber { get; set; }
        public string LetterStatus { get; set; }
        public Nullable<decimal> EnvelopNo { get; set; }
    }
    public class BikeRegOldVM
    {
        public int RowId { get; set; }
        public string BillNo { get; set; }
        public Nullable<System.DateTime> BillDate { get; set; }
        public string AccNo { get; set; }
        public string LocCode { get; set; }
        public string LocName { get; set; }
        public string ItemName { get; set; }
        public string Model { get; set; }
        public string CompName { get; set; }
        public string SrNo { get; set; }
        public string Saletype { get; set; }
        public string RegFeesStatus { get; set; }
        public Nullable<decimal> RegFeesCharges { get; set; }
        public string NumberPlate { get; set; }
        public string NumberPlateStatus { get; set; }
        public string RegDocsStatus { get; set; }
        public string TransferStatus { get; set; }
        public string TransferTo { get; set; }
        public string MobileNo { get; set; }
        public string Address1 { get; set; }
        public string Remarks { get; set; }
        public string Comments { get; set; }
        public Nullable<System.DateTime> ModificationDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
    }
    public class BikeLetterSaleVM
    {
        public string City { get; set; }
        public string LocCode { get; set; }
        public string SuppName { get; set; }
        public long BillNo { get; set; }
        public Nullable<System.DateTime> BillDate { get; set; }
        public string SaleType { get; set; }
        public string ComName { get; set; }
        public string TypeName { get; set; }
        public string SKUName { get; set; }
        public long ItemId { get; set; }
        public string SerialNo { get; set; }
        public bool LetterStatus { get; set; }
        public Nullable<long> RefTransId { get; set; }
        public bool RegFeeStatus { get; set; }
        public decimal RegFeeCharges { get; set; }
        public string NumberPlate { get; set; }
        public bool NumberPlateStatus { get; set; }
        public bool RegDocStatus { get; set; }
        public string Remarks { get; set; }
        public string GRNNo { get; set; }
        public string DONo { get; set; }
        public string Status { get; set; }
        public Nullable<System.DateTime> DODate { get; set; }
        public Nullable<System.DateTime> GRNDate { get; set; }
    }
    public class MCustomerLedgerVM
    {
        //public long AccNo { get; set; }
        //public string CustomerName { get; set; }
        //public string Father { get; set; }
        //public DateTime AccDate { get; set; }
        //public int Duration { get; set; }
        //public decimal TotalInstallments { get; set; }
        //public decimal ActualIntallment { get; set; }
        //public decimal Advance { get; set; }
        //public decimal ActualAdvance { get; set; }
        //public string MobileOne { get; set; }
        //public string MobileTwo { get; set; }
        //public string ResAddress { get; set; }
        //public string Residence { get; set; }
        //public string OffAddress { get; set; }
        //public string Occupation { get; set; }
        //public string CNIC { get; set; }
        //public decimal Salary { get; set; }
        //public string Gender { get; set; }
        //public bool Worth { get; set; }
        //public string Remarks { get; set; }
        //public bool Affidavid { get; set; }
        //public string ProcessAt { get; set; }
        //public bool Defaulter { get; set; }
        //public string Username { get; set; }

        //public string SManager { get; set; }
        //public string Manager { get; set; }
        //public string Marketing { get; set; }
        //public string Inquiry { get; set; }
        public CustomerDetailRVM CustomerInfo { get; set; }
        public List<InstDetailVM> CustomerInstallments { get; set; }
        public List<MGurantorVM> CustomerGuarantor { get; set; }
        public List<MLatLng> LatLng { get; set; }
    }

    public class MobileMenuVM
    {
        public int ActivityId { get; set; }
        public int ParentId { get; set; }
        public string ActivityTitle { get; set; }
        public string Link { get; set; }
        public string Icon { get; set; }
        public bool IsVisible { get; set; }
        public int AppType { get; set; }
    }

    public class MobileMenuAccessVM
    {
        public int RowId { get; set; }
        public int ActivityId { get; set; }
        public int GroupId { get; set; }
        public int[] ActivityIds { get; set; }
        public string GroupName { get; set; }
    }
    public class ActivityVM
    {
        public int ActivityId { get; set; }
        public string ActivityName { get; set; }

    }

    public class LoginRequestVM
    {
        public int RowId { get; set; }
        public int UserId { get; set; }
        public string CNIC { get; set; }
        public string DeviceId { get; set; }
        public bool Approved { get; set; }
        public string Msg { get; set; }
        public string EmpName { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public int GroupId { get; set; }
        public string Desg { get; set; }
        public string Status { get; set; }
        public string MobileNo { get; set; }
        public DateTime? LastApprovedDate { get; set; }
        public int NoOfRequest { get; set; }
    }
    #endregion

    public class PendingRecoveryVM
    {
        [Required]
        public long AccNo { get; set; }
        public string AccName { get; set; }
        public string CNIC { get; set; }
        public string MobileNumber { get; set; }
        public decimal RemaningAmt { get; set; }
        [Required]
        public long InstId { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public decimal PaidAmt { get; set; }
    }
    public class ComplainVM
    {
        public System.DateTime ComplainDate { get; set; }
        public long ItemId { get; set; }
        public long SKUId { get; set; }
        public string SrNo { get; set; }
        public string SaleType { get; set; }
        public long TransId { get; set; }
        public string Customer { get; set; }
        public string CustomerMobile { get; set; }
        public string Complain { get; set; }
        public string Communicator { get; set; }
        public string CommunicatorMobile { get; set; }
        public string CComplainNo { get; set; }
        public string Action { get; set; }
        public string Status { get; set; }
        public System.DateTime TransDate { get; set; }
        public long UserId { get; set; }
    }

    public class DocumentManagerVM
    {
        public long id { get; set; }
        public long RefObjId { get; set; }
    }


    public class ProcessPerformanceVM
    {
        public int TransId { get; set; }
        public System.DateTime PerformanceMonth { get; set; }
        public string PerformanceType { get; set; }
        public string EmpName { get; set; }
        public string LocName { get; set; }
        public decimal TargetValue { get; set; }
        public decimal TargetQty { get; set; }
        public decimal AchQty { get; set; }
        public decimal AchValue { get; set; }
        public decimal IncentivePercent { get; set; }
        public decimal IncentiveValue { get; set; }
    }


    public class EmpJoiningLogVM
    {
        public long JoinId { get; set; }
        public int EmpId { get; set; }
        public int DeptId { get; set; }

        public string EmpName { get; set; }

        public System.DateTime JoinDate { get; set; }
        public Nullable<System.DateTime> FinalDate { get; set; }
        public int StatusId { get; set; }
        public string Remarks { get; set; }
        public int UserId { get; set; }
        public System.DateTime TransDate { get; set; }
        public string JoinReason { get; set; }
        public string FinalReason { get; set; }
    }
    public class StockTypeWiseRVM
    {
        public string Status { get; set; }
        public string Type { get; set; }
        public string Model { get; set; }
        public long Qty { get; set; }
    }
    public class FinBRSVM
    {
        public long DocId { get; set; }
        public System.DateTime DocDate { get; set; }
        public long AccId { get; set; }
        public System.DateTime TransDate { get; set; }
        public int UserId { get; set; }
    }

    public class FinBRSDetailVM
    {
        public long DocDtlId { get; set; }
        public long DocId { get; set; }
        public Nullable<System.DateTime> TransactionDate { get; set; }
        public string Narration { get; set; }
        public string ChequeNo { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public decimal Balance { get; set; }
        public bool Status { get; set; }
        public long VrDtlId { get; set; }
    }


    public class StockVerificationVM
    {
        public int UserId { get; set; }
        public int LocId { get; set; }
        public long DocId { get; set; }
        public DateTime DocDate { get; set; }
    }
    public class StockVerificationAVM
    {
        public long ItemId { get; set; }
        public string SerialNo { get; set; }
        public int SKUId { get; set; }
        public string Status { get; set; }
        public int UserId { get; set; }
        public int LocId { get; set; }
        public string InputType { get; set; }
        public DateTime? TransDate { get; set; }
        public string Remarks { get; set; }
        public int StatusId { get; set; }
    }

    public class FinalSettlementVM
    {
        public int EmpId { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }
        public DateTime FinalDate { get; set; }
    }

    //public class BasicSalarySlabVM
    //{
    //    public int SlabId { get; set; }
    //    public string SlabTitle { get; set; }
    //    public bool Status { get; set; }
    //    public int DefinedBy { get; set; }
    //    public string UserName { get; set; }
    //    public System.DateTime DefinedDate { get; set; }


    //    public int TransId { get; set; }
    //    public decimal SlabStart { get; set; }
    //    public decimal SlabEnd { get; set; }
    //    public decimal BasicSalary { get; set; }
    //    public List<BasicSalarySlabDtlVM> EmpBasicSalarySlabDetail { get; set; }
    //}

    //public class BasicSalarySlabDtlVM
    //{
    //    public int TransId { get; set; }
    //    public int SlabId { get; set; }
    //    public decimal SlabStart { get; set; }
    //    public decimal SlabEnd { get; set; }
    //    public decimal BasicSalary { get; set; }
    //}

    public class BankEndIncpolicyVM
    {
        public int PolicyId { get; set; }
        public string PolicyTitle { get; set; }
        public System.DateTime FromDate { get; set; }
        public System.DateTime ToDate { get; set; }
        public int SupId { get; set; }
        public string IncTypeId { get; set; }
        public int IncBaseId { get; set; }
        public string Status { get; set; }
        public int DefinedBy { get; set; }
        public System.DateTime DefinedDate { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    }

    public partial class BackendIncBasisVM
    {
        public int IncBaseId { get; set; }
        public string IncBase { get; set; }
    }

    public partial class BackendIncTypesVM
    {
        public string IncTypeId { get; set; }
        public string IncType { get; set; }
    }
    public class BackEndIncPolicyDtlVM
    {
        public long PolicyDtlId { get; set; }
        public int PolicyId { get; set; }
        public int TypeId { get; set; }
        public int ModelId { get; set; }
        public int CompId { get; set; }
        public int ProdId { get; set; }
        public int SkuId { get; set; }
        public string SerialNo { get; set; }
        public decimal BasicTarget { get; set; }
        public decimal IncPercent { get; set; }
        public decimal SalesmanInc { get; set; }
        public decimal LowerPortion { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public string PriceType { get; set; }
        public decimal SpecialInc { get; set; }
    }

    public class BackEndPolicyMobileVM
    {
        public int PolicyId { get; set; }
        public string PolicyTitle { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int SupId { get; set; }
        public string IncType { get; set; }
        public int IncBaseId { get; set; }
        public string Status { get; set; }
    }


    public class LseStatusVM
    {
        public int StatusId { get; set; }
        public string Status { get; set; }
    }


    public class Users_FeedbackVM
    {
        public int RowId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string MobileNo { get; set; }
        public System.DateTime TransDate { get; set; }
        public int UserId { get; set; }
    }

    public class TwoMonthSaleVM
    {
        public string LocName { get; set; }
        public DateTime? Date { get; set; }
        public int Day { get; set; }
        public Decimal? PSale { get; set; }
        public Decimal? CSale { get; set; }
        public Decimal? PYSale { get; set; }
        public Decimal? PInstallmentAmt { get; set; }
        public Decimal? CInstallmentAmt { get; set; }
        public Decimal? PDeliveryAmt { get; set; }
        public Decimal? CDeliveryAmt { get; set; }
    }
    public class Video
    {
        public string title { get; set; }
        public string poster { get; set; }
        public string source { get; set; }
    }
    public class UsersInfoVM
    {
        public int UserId { get; set; }
        public int LocId { get; set; }
    }

    public class EmpTransferLogVM
    {
        public long RowId { get; set; }
        public int EmpId { get; set; }
        public int SEmpId { get; set; }
        public int FLocId { get; set; }
        public int TLocId { get; set; }
        public System.DateTime TransDate { get; set; }
        public int UserId { get; set; }
        public DateTime FDate { get; set; }
        public DateTime TDate { get; set; }
        public string TrasferType { get; set; }
        public string TReason { get; set; }
    }

    public class StockAllVM
    {
        public string SKU { get; set; }
        public string Model { get; set; }
        public string Product { get; set; }
        public string Company { get; set; }
        public string SerialNo { get; set; }
        public string Status { get; set; }
        public string LocCode { get; set; }
        public string CityCode { get; set; }
        public decimal MRP { get; set; }
        public decimal PPrice { get; set; }
        public decimal SPrice { get; set; }
        public string Supplier { get; set; }
        public string Type { get; set; }
        public string SaleType { get; set; }
        public int Qty { get; set; }
        public DateTime TrxDate { get; set; }
    }
    public class POPocketVM
    {
        public long TransId { get; set; }
        public long POId { get; set; }
        public int LocId { get; set; }
        public int SKUId { get; set; }
        public string SerialNo { get; set; }
        public bool Status { get; set; }
        public System.DateTime TransDate { get; set; }
        public int UserId { get; set; }
    }
    public class CrcFinesVM
    {

        public long Id { get; set; }
        public System.DateTime FineDate { get; set; }
        public Nullable<long> AccountNo { get; set; }
        public int FineToEmp { get; set; }
        public decimal FineAmt { get; set; }
        public string FineReason { get; set; }
        public int CRC { get; set; }
        public int PolicyId { get; set; }
        public System.DateTime TransDate { get; set; }
        public Nullable<int> ApprovedBy { get; set; }
        public string ApprovedByName { get; set; }
        public Nullable<System.DateTime> RMApprovalTransDate { get; set; }
        public Nullable<int> RMApprovedBy { get; set; }
        public string RMApprovedName { get; set; }
        public Nullable<System.DateTime> ApprovedDate { get; set; }
        public string EmpName { get; set; }
        public string DesgName { get; set; }
        public int PolicyDetail { get; set; }
        public string DeptName { get; set; }
        public Nullable<int> LocId { get; set; }
    }

    public class SuppTaxExemptionVM
    {
        public int RowId { get; set; }
        public int SuppId { get; set; }
        public System.DateTime FromDate { get; set; }
        public System.DateTime ToDate { get; set; }
        public decimal TaxRate { get; set; }
        public string Remarks { get; set; }
    }


    #region CRM
    public class Crm_TicketVM
    {
        public long TicketId { get; set; }
        public System.DateTime WorkingDate { get; set; }
        public int CategoryId { get; set; }
        public string Complain { get; set; }
        public System.DateTime TransDate { get; set; }
        public int UserId { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public int LocId { get; set; }

        public string UploadedFiles { get; set; }
        public string Category { get; set; }
        public string City { get; set; }
        public string Response { get; set; }
        public string LocCode { get; set; }
        public string LocName { get; set; }
        public string ContactPerson { get; set; }
        public string ContactNo { get; set; }
        public int InvStatus { get; set; }
        public string CompComplainNo { get; set; }
    }

    public class Crm_DamageStockTicketVM
    {
        public long TicketId { get; set; }
        public System.DateTime WorkingDate { get; set; }
        public int CategoryId { get; set; }
        public string Complain { get; set; }
        public System.DateTime TransDate { get; set; }
        public int UserId { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public int LocId { get; set; }

        public string UploadedFiles { get; set; }
        public string Category { get; set; }
        public string City { get; set; }
        public string Response { get; set; }
        public string LocCode { get; set; }
        public string LocName { get; set; }
        public string ContactPerson { get; set; }
        public string ContactNo { get; set; }
        public string CompName { get; set; }
        public string ProductName { get; set; }
        public string Model { get; set; }
        public string SerialNo { get; set; }
        public string Type { get; set; }
        public int InvStatus { get; set; }
        public string CompComplainNo { get; set; }
    }

    public class InvCompalinVM
    {
        public int LocId { get; set; }
        public string Type { get; set; }
        public string Fault { get; set; }
        public int Salesman { get; set; } 
        public string CellNo { get; set; }
        public string Remarks { get; set; }
        //public string Serial { get; set; }
        //public int SKUId { get; set; }
        public int ItemId { get; set; }
        public string Status { get; set; }
        public string CompComplainNo { get; set; }
    }
    public class ResultVM
    {
        public long TransId = 0;
        public long AccNo = 0;
        public string Msg = "";
    }
    public class Crm_CategoryVM
    {
        public int CategoryId { get; set; }
        public string Category { get; set; }
        public bool Status { get; set; }

    }

    public class Comp_StatusVM
    {
        public long StatusId { get; set; }
        public string StatusTitle { get; set; }

    }
    #endregion

    public class SaleCartVM
    {
        public long RowId { get; set; }
        public int LocId { get; set; }
        public long ItemId { get; set; }
        public long SerialNo { get; set; }
        public int SalesmanId { get; set; }
        public decimal Rate { get; set; }
        //public System.DateTime TransDate { get; set; }
        //public string Status { get; set; }
    }


    public class DonationVM
    {
        public int Dtlid { get; set; }
        public int CDId { get; set; }
        public int PersonId { get; set; }
        public string PersonName { get; set; }
        public decimal Amount { get; set; }
        public long CDDtlId { get; set; }
        public Nullable<int> CashierId { get; set; }
        public string EmpName { get; set; }
        public Nullable<int> ApprovedBy { get; set; }

    }

    public class EmpHierarchyVM
    {
        public int LocId { get; set; }
        public int GMId { get; set; }
        public string GM { get; set; }
        public int DGMId { get; set; }
        public string DGM { get; set; }
        public int SSRMId { get; set; }
        public string SSRM { get; set; }
        public int SRMId { get; set; }
        public string SRM { get; set; }
        public int BDMId { get; set; }
        public string BDM { get; set; }
        public int RMId { get; set; }
        public string RM { get; set; }
        public int CashHeadId { get; set; }
        public string CashHead { get; set; }
        public int RegionalCashHeadId { get; set; }
        public string RegionalCashHead { get; set; }
        public int CRCHeadId { get; set; }
        public string CRCHead { get; set; }
        public int RAuditorId { get; set; }
        public string RAuditor { get; set; }
        public int SAuditorId { get; set; }
        public string SAuditor { get; set; }
        public int AuditorId { get; set; }
        public string Auditor { get; set; }
        public int CashSaleCoordinatorId { get; set; }
        public string CashSaleCoordinator { get; set; }
    }

    public class ItemNatureVM
    {
        public int ItemNatureId { get; set; }
        public string ItemNature { get; set; }
    }
    public class ItemCategoryVM
    {
        public int ItemCategoryId { get; set; }
        public string ItemCategory { get; set; }
        public int ItemNatureId { get; set; }
    }
    public class ItemProductVM
    {
        public int ItemProductId { get; set; }
        public int CostTypeId { get; set; }
        public int ItemCategoryId { get; set; }
        public string ItemProduct { get; set; }
    }
    public class ItemBrandVM
    {
        public int ItemBrandId { get; set; }
        public string ItemBrand { get; set; }
    }
    public class ItemVM
    {

        public int ItemId { get; set; }
        public string ItemCode { get; set; }
        public string Item { get; set; }
        public int ItemBrandId { get; set; }
        public int ItemProductId { get; set; }
        public string ItemBrand { get; set; }
        public string ItemProduct { get; set; }
        public int UOMId { get; set; }
        public string UOM { get; set; }
        public string Spec { get; set; }
        public decimal Qty = 0;
    }
    public class MTRVM
    {
        public string MTRNo { get; set; }
        public long MTRId { get; set; }
        public string CostCenter { get; set; }
        public int CCCode { get; set; }
        public string MTRDate { get; set; }
        public string Status { get; set; }
    }
    public class MTRDetailVM
    {
        public long MTRDtlId { get; set; }
        public long MTRId { get; set; }
        public int ItemId { get; set; }
        public string Item { get; set; }
        public decimal Qty { get; set; }
        public string ItemType { get; set; }
        public Nullable<decimal> EstPrice { get; set; }
        public string Remarks { get; set; }
    }
    public class SINDetailVM
    {
        public long SINDtlId { get; set; }
        public long SINId { get; set; }
        public int ItemId { get; set; }
        public string Item { get; set; }
        public decimal InStock { get; set; }
        public decimal RequiredQty { get; set; }
        public decimal Qty { get; set; }
        public string SerialNo { get; set; }
        public string CSerialNo { get; set; }
        public string Remarks { get; set; }
        public string ItemType { get; set; }
        public int CostTypeId { get; set; }
    }
    public class ProPOVM
    {
        public string PONo { get; set; }
        public long POId { get; set; }
        public string SuppName { get; set; }
        public int NatureId { get; set; }
    }

    public class ProPODetailVM
    {
        public long PODtlId { get; set; }
        public long POId { get; set; }
        public string Item { get; set; }
        public int ItemId { get; set; }
        public decimal Qty { get; set; }
        public decimal Rate { get; set; }
        public decimal Tax { get; set; }
        public decimal Disc { get; set; }
        public int LocId { get; set; }
        public int CostTypeId { get; set; }
    }
    public class ProGRNDetailVM
    {
        public long GRNDtlId { get; set; }
        public long PODtlId { get; set; }
        public long GRNId { get; set; }
        public string Item { get; set; }
        public int ItemId { get; set; }
        public decimal OrderQty { get; set; }
        public decimal Qty { get; set; }
        //public decimal Rate { get; set; }
        //public decimal Tax { get; set; }
        //public decimal Disc { get; set; }
    }

    public class UOMVM
    {
        public int UOMId { get; set; }
        public string UOM { get; set; }
    }

    public class GetDailyAttendanceVM
    {
        public string DeptName { get; set; }
        public string DesgName { get; set; }
        public int EmpId { get; set; }
        public string EmpName { get; set; }
        public string Status { get; set; }
    }
    public class CostTypeVM
    {
        public int CostTypeId { get; set; }
        public string CostType { get; set; }
        public Nullable<long> CapitalHOGL { get; set; }
        public Nullable<long> CapitalSHGL { get; set; }
        public Nullable<long> RevenueHOGL { get; set; }
        public Nullable<long> RevenueSHGL { get; set; }
        public bool IsService { get; set; }
        public decimal DeprRate { get; set; }
        public Nullable<long> AccDeprHOGL { get; set; }
        public Nullable<long> DeprHOGL { get; set; }
        public Nullable<long> AccDeprSHGL { get; set; }
        public Nullable<long> DeprSHGL { get; set; }
    }

    public class CharityDonationVM
    {
        public int PersonId { get; set; }
        public string PersonName { get; set; }
        public string CNIC { get; set; }
        public string ContactNo { get; set; }
        public string Reference { get; set; }
        public decimal Amount { get; set; }
        public Nullable<decimal> Tax { get; set; }
        public Nullable<decimal> BankTransfer { get; set; }
        public string BankName { get; set; }
        public string AccountNo { get; set; }
        public int CashierId { get; set; }
        public string Cashier { get; set; }
        public int CCC { get; set; }
        public string CashCenter { get; set; }
        public string Status { get; set; }
        public int UserId { get; set; }
        public System.DateTime TransDate { get; set; }
        public string Remarks { get; set; }
        public string GLCode { get; set; }


    }

    public class FBRResponse
    {
        public string InvoiceNumber { get; set; }
        public string Response { get; set; }
        public int Code { get; set; }
    }
    public class Invoice
    {
        public string InvoiceNumber { get; set; }
        public long POSID { get; set; }
        public string USIN { get; set; }
        public string RefUSIN { get; set; }
        public System.DateTime DateTime { get; set; }
        public string BuyerName { get; set; }
        public string BuyerNTN { get; set; }
        public string BuyerCNIC { get; set; }
        public string BuyerPhoneNumber { get; set; }
        public decimal TotalSaleValue { get; set; }
        public decimal TotalTaxCharged { get; set; }
        public Nullable<decimal> Discount { get; set; }
        public Nullable<decimal> FurtherTax { get; set; }
        public decimal TotalBillAmount { get; set; }
        public decimal TotalQuantity { get; set; }
        public int PaymentMode { get; set; }
        public int InvoiceType { get; set; }
        public List<InvoiceItems> Items { get; set; }
    }
    public class InvoiceItems
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string PCTCode { get; set; }
        public decimal Quantity { get; set; }
        public double TaxRate { get; set; }
        public decimal SaleValue { get; set; }
        public Nullable<decimal> Discount { get; set; }
        public Nullable<decimal> FurtherTax { get; set; }
        public decimal TaxCharged { get; set; }
        public decimal TotalAmount { get; set; }
        public int InvoiceType { get; set; }
        public string RefUSIN { get; set; }
    }
}