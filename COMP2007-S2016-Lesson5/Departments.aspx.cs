using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

//requested for connected to EF db
using COMP2007_S2016_Lesson5.Models;
using System.Web.ModelBinding;
using System.Linq.Dynamic;

namespace COMP2007_S2016_Lesson5
{
    public partial class Departments : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session["SortColumn"] = "DepartmentID";
                Session["SortDirection"] = "ASC";
                //set departments from EF db
                this.GetDepartments();
            }
        }
        protected void GetDepartments()
        {
            //connect to EF
            using (DefaultConnection db = new DefaultConnection())
            {
                string SortString = Session["SortColumn"].ToString() + " " + Session["SortDirection"].ToString();

                //query the Students table using EF and 
                var Departments = (from allDepartments in db.Departments
                                   select allDepartments);

                //find the result o the GridView
                DepartmentsGridView.DataSource = Departments.AsQueryable().OrderBy(SortString).ToList();
                DepartmentsGridView.DataBind();
            }
        }
        protected void DepartmentsGridView_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            //store which row was selected for deletion
            int selectedRow = e.RowIndex;

            //get the selected StudentID using the grid's Datakey collection
            int DepartmentID = Convert.ToInt32(DepartmentsGridView.DataKeys[selectedRow].Values["DepartmentID"]);

            //use EF to find the selected student from DB and Remove it
            using (DefaultConnection db = new DefaultConnection())
            {
                Department deletedDepartment = (from departmentRecords in db.Departments
                                                where departmentRecords.DepartmentID == DepartmentID
                                                select departmentRecords).FirstOrDefault();

                //remove the student record from the database
                db.Departments.Remove(deletedDepartment);

                //save changes to the db
                db.SaveChanges();

                //refresh the grid
                this.GetDepartments();
            }
        }
        protected void DepartmentsGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            //set the new page number
            DepartmentsGridView.PageIndex = e.NewPageIndex;

            //refresh the grid
            this.GetDepartments();
        }
        protected void PageSizeDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            //set the new Page size
            DepartmentsGridView.PageSize = Convert.ToInt32(PageSizeDropDownList.SelectedValue);

            //refresh the grid
            this.GetDepartments();
        }
        protected void DepartmentsGridView_Sorting(object sender, GridViewSortEventArgs e)
        {
            //get the column to sort by
            Session["SortColumn"] = e.SortExpression;

            //refresh the Grid
            this.GetDepartments();

            //toggle the direction
            Session["SortDirection"] = Session["SortDirection"].ToString() == "ASC" ? "DESC" : "ASC";
        }
        protected void DepartmentsGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (IsPostBack)
            {
                if (e.Row.RowType == DataControlRowType.Header)
                {
                    LinkButton linkbutton = new LinkButton();

                    for (int index = 0; index < DepartmentsGridView.Columns.Count - 1; index++)
                    {
                        if (DepartmentsGridView.Columns[index].SortExpression == Session["SortColumn"].ToString())
                        {
                            if (Session["SortDirection"].ToString() == "ASC")
                            {
                                linkbutton.Text = " <i class='fa fa-caret-up fa-lg'></i>";
                            }
                            else
                            {
                                linkbutton.Text = " <i class='fa fa-caret-down fa-lg'></i>";
                            }

                            e.Row.Cells[index].Controls.Add(linkbutton);
                        }
                    }
                }
            }
        }
    }
}