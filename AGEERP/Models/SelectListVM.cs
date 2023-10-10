using System.Collections.Generic;
using System.Web.Mvc;

namespace AGEERP.Models
{
    public static class SelectListVM
    {

        public static List<SelectListItem> RZL = new List<SelectListItem>()
        {
            new SelectListItem(){ Value = "0", Text = "National" },
            new SelectListItem(){ Value = "1", Text = "Region" },
            new SelectListItem(){ Value = "2", Text = "Zone" },
            new SelectListItem(){ Value = "3", Text = "Area" },
            new SelectListItem(){ Value = "4", Text = "Territory" }
        };

        public static List<SelectListItem> AllowancesDeductionsType = new List<SelectListItem>()
        {
            new SelectListItem(){ Value = "A", Text = "Allowance" },
            new SelectListItem(){ Value = "D", Text = "Deduction" }
        };

        public static List<SelectListItem> BankAccountType = new List<SelectListItem>()
        {
            new SelectListItem(){ Value = "Current", Text = "Current" },
            new SelectListItem(){ Value = "Saving", Text = "Saving" }
        };

        public static List<SelectListItem> ChequeType = new List<SelectListItem>()
        {
            new SelectListItem(){ Value = "Security", Text = "Security" },
            new SelectListItem(){ Value = "Presentable", Text = "Presentable" }
        };


        public static List<SelectListItem> PriceType = new List<SelectListItem>()
        {
            new SelectListItem(){ Value = "PP", Text = "PP" },
            new SelectListItem(){ Value = "TP", Text = "TP" },
            new SelectListItem(){ Value = "MRP", Text = "MRP" }
        };



        public static List<SelectListItem> ChequeStatus = new List<SelectListItem>()
        {
            new SelectListItem(){ Value = "V", Text = "Void" },
            new SelectListItem(){ Value = "C", Text = "Cancelled" },
            new SelectListItem(){ Value = "U", Text = "Un Presented" },
            new SelectListItem(){ Value = "L", Text = "Cleared" },
            new SelectListItem(){ Value = "P", Text = "Post Dated Cheque" },
            new SelectListItem(){ Value = "R", Text = "Cheque For Resolving Gurantees" }
        };


        public static List<SelectListItem> InstrumentType = new List<SelectListItem>()
        {
            new SelectListItem(){ Value = "Cheque", Text = "Cheque" },
            new SelectListItem(){ Value = "PayOrder", Text = "Pay Order" },
            new SelectListItem(){ Value = "OnlineTransfer", Text = "Online Transfer" }
        };



        public static List<SelectListItem> EmployeeIncentive = new List<SelectListItem>()
        {
            new SelectListItem(){ Value = "BM", Text = "BM Special Target Incentives" },
            new SelectListItem(){ Value = "RM", Text = "RM Special Target Incentives" }
        };

        public static List<SelectListItem> ProdIncentiveCrit = new List<SelectListItem>()
        {
   
            new SelectListItem(){ Value = ">", Text = ">" },
            new SelectListItem(){ Value = "<", Text = "<" },
            new SelectListItem(){ Value = "=", Text = "=" }
        };

        public static List<SelectListItem> IncBy = new List<SelectListItem>()
        {
            new SelectListItem(){ Value = "0", Text = "Value" },
            new SelectListItem(){ Value = "1", Text = "Percentage" }
        };

        public static List<SelectListItem> FinStatus = new List<SelectListItem>()
        {
            new SelectListItem(){ Value = "O", Text = "Open" },
            new SelectListItem(){ Value = "C", Text = "Close" }
        };
        public static List<SelectListItem> TicketStatus = new List<SelectListItem>()
        {
             new SelectListItem(){ Value = "", Text = "All " },
            new SelectListItem(){ Value = "O", Text = "Open" },
            new SelectListItem(){ Value = "C", Text = "Close" }
        };
        public static List<SelectListItem> AttendanceStatus = new List<SelectListItem>()
        {
            new SelectListItem(){ Value = "A", Text = "Active" },
            new SelectListItem(){ Value = "B", Text = "Block" }
        };

        public static List<SelectListItem> StatusDDL = new List<SelectListItem>()
          {
              new SelectListItem() {
                Text = "Active", Value ="1"
              },
              new SelectListItem() {
                Text = "In Active", Value ="2"
              }
          };
        public static List<SelectListItem> StatusDDLAll = new List<SelectListItem>()
          {
            new SelectListItem() {
                Text = "All", Value ="0"
              },
              new SelectListItem() {
                Text = "Open", Value ="3"
              },
              new SelectListItem() {
                Text = "Close", Value ="4"
              }
          };

        public static List<SelectListItem> RZLIH = new List<SelectListItem>()
        {
            //new SelectListItem(){ Value = "0", Text = "Region" },
            //new SelectListItem(){ Value = "1", Text = "Zone" },
            //new SelectListItem(){ Value = "2", Text = "Area" },
            //new SelectListItem(){ Value = "3", Text = "Territory" }
             new SelectListItem(){ Value = "1", Text = "Region" },
            new SelectListItem(){ Value = "2", Text = "Zone" },
            new SelectListItem(){ Value = "3", Text = "Area" },
            new SelectListItem(){ Value = "4", Text = "Territory" }
        };
        public static List<SelectListItem> IsShopHead = new List<SelectListItem>()
        {
            new SelectListItem(){ Value = "S", Text = "Shop" },
            new SelectListItem(){ Value = "H", Text = "Head Office" },
            new SelectListItem(){ Value = "B", Text = "Both" }
        };
        public static List<SelectListItem> IsMRPTP = new List<SelectListItem>()
        {
            new SelectListItem(){ Value = "M", Text = "MRP" },
            new SelectListItem(){ Value = "T", Text = "TP" },
            new SelectListItem(){ Value = "T-D", Text = "TP-Disc" },
            new SelectListItem(){ Value = "M-D", Text = "MRP-Disc" },
            new SelectListItem(){ Value = "R", Text = "RP" },
            new SelectListItem(){ Value = "R-D", Text = "RP-Disc" },
            new SelectListItem(){ Value = "D-D", Text = "DP-Disc" },
            new SelectListItem(){ Value = "D", Text = "DP" }
        };

        public static List<SelectListItem> PerAmountTypeWise = new List<SelectListItem>()
        {
            new SelectListItem()
            {
                Text = "Percentage",
                Value = "1"
            },
              new SelectListItem()
            {
                Text = "Amount",
                Value = "2"
            }

        };

        public static List<SelectListItem> SaleTypeSL = new List<SelectListItem>()
        {
            new SelectListItem()
            {
                Text = "Both",
                Value = "B"
            },
              new SelectListItem()
            {
                Text = "Installment",
                Value = "I"
            },
               new SelectListItem()
            {
                Text = "Cash",
                Value = "C"
            }

        };
        public static List<SelectListItem> LocationTypeSL = new List<SelectListItem>()
        {
            new SelectListItem()
            {
                Text = "All Location",
                Value = "A"
            },
              new SelectListItem()
            {
                Text = "Purchase Center",
                Value = "P"
            }

        };
        public static List<SelectListItem> AdvanceTypes = new List<SelectListItem>()
        {
            new SelectListItem()
            {
                Text = "Employee Advance",
                Value = "1"
            },
              new SelectListItem()
            {
                Text = "Closing Advance",
                Value = "2"
            }

        };
        public static List<SelectListItem> AmountTypeWise = new List<SelectListItem>()
        {
              new SelectListItem()
            {
                Text = "Amount",
                Value = "2"
            }

        };
        public static List<SelectListItem> UserThemes = new List<SelectListItem>()
        {
            new SelectListItem()
            {
                Text = "Default Theme",
                Value = "Material.css"
            },
            new SelectListItem()
            {
                Text = "Flat Theme",
                Value = "Flat.css"
            },
              new SelectListItem()
            {
                Text = "Fiori Theme",
                Value = "Fiori.css"
            },
                new SelectListItem()
            {
                Text = "Blue Opal",
                Value = "blueopal.css"
            },
                 new SelectListItem()
            {
                Text = "Dark Mode",
                Value = "MaterialDark.css"
            }
        };

        public static List<SelectListItem> GenderSL = new List<SelectListItem>()
        {
            new SelectListItem()
            {
                Text = "Male",
                Value = "M"
            },
            new SelectListItem()
            {
                Text = "Female",
                Value = "F"
            }
        };
        public static List<SelectListItem> AllowanceType = new List<SelectListItem>()
        {
            new SelectListItem()
            {
                Text = "Daily",
                Value = "D"
            },
            new SelectListItem()
            {
                Text = "Monthly",
                Value = "M"
            }
        };

        //public static List<SelectListItem> AllowancesDeductionsType = new List<SelectListItem>()
        //{
        //    new SelectListItem()
        //    {
        //        Text = "Allowances",
        //        Value = "A"
        //    },
        //    new SelectListItem()
        //    {
        //        Text = "Deductions",
        //        Value = "D"
        //    }
        //};

        public static List<SelectListItem> LeaveStatusSelList = new List<SelectListItem>()
        {
            new SelectListItem()
            {
                Text = "Approved",
                Value = "1"
            },
            new SelectListItem()
            {
                Text = "Rejected",
                Value = "2"
            },
             new SelectListItem()
            {
                Text = "Pending",
                Value = "3"
            }
        };
        public static List<SelectListItem> AllowanceAllocationWise = new List<SelectListItem>()
        {
            new SelectListItem()
            {
                Text = "Select the option",
                Value = "0"
            },
            new SelectListItem()
            {
                Text = "Employee",
                Value = "1"
            },

              new SelectListItem()
            {
                Text = "Designation",
                Value = "2"
            },
            new SelectListItem()
            {
                Text = "Department",
                Value = "3"
            }

        };

        public static List<SelectListItem> SalaryAllocations = new List<SelectListItem>()
        {
            new SelectListItem()
            {
                Text = "Select the option",
                Value = "0"
            },

              new SelectListItem()
            {
                Text = "Designation",
                Value = "1"
            },
            new SelectListItem()
            {
                Text = "Department",
                Value = "2"
            }

        };

        public static List<SelectListItem> PolicySL = new List<SelectListItem>()
        {
            new SelectListItem()
            {
                Text = "Discount Policy",
                Value = "D"
            },
            new SelectListItem()
            {
                Text = "Pair Policy",
                Value = "P"
            }
        };
        public static List<SelectListItem> JobTypeSL = new List<SelectListItem>()
        {
            new SelectListItem()
            {
                Text = "In Door",
                Value = "I"
            },
            new SelectListItem()
            {
                Text = "Out Door",
                Value = "O"
            }
        };
        public static List<SelectListItem> BloodGroupSL = new List<SelectListItem>()
        {
            new SelectListItem()
            {
                Text = "A Positive",
                Value = "1"
            },
            new SelectListItem()
            {
                Text = "A Negative",
                Value = "2"
            },
            new SelectListItem()
            {
                Text = "B Positive",
                Value = "3"
            },
            new SelectListItem()
            {
                Text = "B Negative",
                Value = "4"
            },
            new SelectListItem()
            {
                Text = "AB Positive",
                Value = "5"
            },
            new SelectListItem()
            {
                Text = "AB Negative",
                Value = "6"
            },
            new SelectListItem()
            {
                Text = "O Positive",
                Value = "7"
            },
            new SelectListItem()
            {
                Text = "O Negative",
                Value = "8"
            }
        };
        public static List<SelectListItem> MaritalStatusSL = new List<SelectListItem>()
        {
            new SelectListItem()
            {
                Text = "Single",
                Value = "S"
            },
            new SelectListItem()
            {
                Text = "Married",
                Value = "M"
            },
            new SelectListItem()
            {
                Text = "Divorsed",
                Value = "D"
            },
            new SelectListItem()
            {
                Text = "Widowed",
                Value = "W"
            },
        };
        public static List<SelectListItem> WeekDaySL = new List<SelectListItem>()
        {
            new SelectListItem()
            {
                Text = "Sunday",
                Value = "1"
            },
            new SelectListItem()
            {
                Text = "Monday",
                Value = "2"
            },
            new SelectListItem()
            {
                Text = "Tuesday",
                Value = "3"
            },
            new SelectListItem()
            {
                Text = "Wednesday",
                Value = "4"
            },
            new SelectListItem()
            {
                Text = "Thursday",
                Value = "5"
            },
            new SelectListItem()
            {
                Text = "Friday",
                Value = "6"
            },
            new SelectListItem()
            {
                Text = "Saturday",
                Value = "7"
            }
        };
        public static List<SelectListItem> EmpStatusSL = new List<SelectListItem>()
        {
            new SelectListItem()
            {
                Text = "Active",
                Value = "A"
            },
            new SelectListItem()
            {
                Text = "InActive",
                Value = "I"
            },
            new SelectListItem()
            {
                Text = "Resign",
                Value = "R"
            },
            new SelectListItem()
            {
                Text = "Terminate",
                Value = "T"
            },
        };

        public static List<SelectListItem> EmpStatusSeperationSL = new List<SelectListItem>()
        {
            new SelectListItem()
            {
                Text = "InActive",
                Value = "I"
            },
            new SelectListItem()
            {
                Text = "Resign",
                Value = "R"
            },
            new SelectListItem()
            {
                Text = "Terminate",
                Value = "T"
            },
        };

        public static List<SelectListItem> LetStatus = new List<SelectListItem>()
        {

            new SelectListItem()
            {
                Text = "UnPosted",
                Value = "D"
            },
            new SelectListItem()
            {
                Text = "Posted",
                Value = "P"
            },
            new SelectListItem()
            {
                Text = "Cancelled",
                Value = "C"
            },
        };

        public static List<SelectListItem> PendingEmpSL = new List<SelectListItem>()
        {
            new SelectListItem()
            {
                Text = "Approved",
                Value = "A"
            },
            new SelectListItem()
            {
                Text = "Pending",
                Value = "P"
            }
        };
        public static List<SelectListItem> VrStatusSL = new List<SelectListItem>()
        {
            new SelectListItem()
            {
                Text = "Posted",
                Value = "P"
            },
            new SelectListItem()
            {
                Text = "UnPosted",
                Value = "U"
            }
        };
        public static List<SelectListItem> ChequeTypeSL = new List<SelectListItem>()
        {
            new SelectListItem()
            {
                Text = "Self",
                Value = "Self"
            },
            new SelectListItem()
            {
                Text = "Cross",
                Value = "Cross"
            }
        };

        public static List<SelectListItem> SubDivSL = new List<SelectListItem>()
        {
            new SelectListItem()
            {
                Text = "Subsidiary",
                Value = "1"
            },
            new SelectListItem()
            {
                Text = "Supplier",
                Value = "2"
            },
            new SelectListItem()
            {
                Text = "Customer",
                Value = "3"
            },
            new SelectListItem()
            {
                Text = "Employee",
                Value = "4"
            }
        };

      
        public static List<SelectListItem> GRNStatusSL = new List<SelectListItem>()
        {
            new SelectListItem()
            {
                Text = "All",
                Value = ""
            },
            new SelectListItem()
            {
                Text = "Not Invoiced",
                Value = "G"
            },
            new SelectListItem()
            {
                Text = "Invoiced",
                Value = "I"
            }
        };
        public static List<SelectListItem> PaymentStatusSL = new List<SelectListItem>()
        {
            new SelectListItem()
            {
                Text = "All",
                Value = ""
            },
            new SelectListItem()
            {
                Text = "Paid",
                Value = "C"
            },
            new SelectListItem()
            {
                Text = "Partial",
                Value = "P"
            },
            new SelectListItem()
            {
                Text = "Not Paid",
                Value = "N"
            }
        };
        public static List<SelectListItem> ItemTypeSL = new List<SelectListItem>()
        {
            new SelectListItem
            {
                Text = "Procurement",
                Value = "P"
            },
            new SelectListItem
            {
                Text = "SKU",
                Value = "S"
            }
        };
    }
}
