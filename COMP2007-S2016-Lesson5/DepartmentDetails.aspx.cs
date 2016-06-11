using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

//requested for connected to EF db
using COMP2007_S2016_Lesson5.Models;
using System.Web.ModelBinding;

namespace COMP2007_S2016_Lesson5
{
    public partial class DepartmentDetails : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if ((!IsPostBack) && (Request.QueryString.Count > 0))
            {
                this.GetDepartment();
            }
        }
        protected void GetDepartment()
        {
            //populate teh form with existing data from the database
            int DepartmentID = Convert.ToInt32(Request.QueryString["DepartmentID"]);

            //connect to the EF DB
            using (DefaultConnection db = new DefaultConnection())
            {
                //populate a student object instance with the StudentID from the URL Parameter
                Department updatedDepartment = (from department in db.Departments
                                                where department.DepartmentID == DepartmentID
                                                select department).FirstOrDefault();

                //map the student properties to the form controls
                if (updatedDepartment != null)
                {
                    DepartmentNameTextBox.Text = updatedDepartment.Name;
                    BudgetTextBox.Text = updatedDepartment.Budget.ToString("#.##");
                }
            }
        }
        protected void CancelButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Departments.aspx");
        }
        protected void SaveButton_Click(object sender, EventArgs e)
        {
            //use EF to connect to the Server
            using (DefaultConnection db = new DefaultConnection())
            {
                //use the student model to save a new record
                Department newDepartment = new Department();

                int DepartmentID = 0;

                if (Request.QueryString.Count > 0)
                {
                    //get the id from the URL
                    DepartmentID = Convert.ToInt32(Request.QueryString["DepartmentID"]);

                    //get the current department from EF db
                    newDepartment = (from department in db.Departments
                                     where department.DepartmentID == DepartmentID
                                     select department).FirstOrDefault();
                }

                newDepartment.Name = DepartmentNameTextBox.Text;
                newDepartment.Budget = Convert.ToDecimal(BudgetTextBox.Text);

                if (DepartmentID == 0)
                {
                    db.Departments.Add(newDepartment);
                }

                //run an insert command
                db.SaveChanges();
                //redirect back to the departments page
                Response.Redirect("~/Departments.aspx");
            }
        }
    }
}