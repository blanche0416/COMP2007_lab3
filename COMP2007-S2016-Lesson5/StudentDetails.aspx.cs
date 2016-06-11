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
    public partial class StudentDetails : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if ((!IsPostBack) && (Request.QueryString.Count > 0))
            {
                this.GetStudent();
            }
        }
        protected void GetStudent()
        {
            //populate teh form with existing data from the database
            int StudentID = Convert.ToInt32(Request.QueryString["StudentID"]);

            //connect to the EF DB
            using (DefaultConnection db = new DefaultConnection())
            {
                //populate a student object instance with the StudentID from the URL Parameter
                Student updatedStudent = (from student in db.Students
                                          where student.StudentID == StudentID
                                          select student).FirstOrDefault();

                //map the student properties to the form controls
                if (updatedStudent != null)
                {
                    LastNameTextBox.Text = updatedStudent.LastName;
                    FirstNameTextBox.Text = updatedStudent.FirstMidName;
                    EnrollmentDateTextBox.Text = updatedStudent.EnrollmentDate.ToString("yyyy-MM-dd");
                }
            }
        }
        protected void CancelButton_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Students.aspx");
        }
        protected void SaveButton_Click(object sender, EventArgs e)
        {
            //use EF to connect to the server
            using (DefaultConnection db = new DefaultConnection())
            {
                //use the student model to save a new record
                Student newStudent = new Student();

                int StudentID = 0;

                if (Request.QueryString.Count > 0)
                {
                    //get the id from the URL
                    StudentID = Convert.ToInt32(Request.QueryString["StudentID"]);

                    //get the current student from EF DB
                    newStudent = (from student in db.Students
                                  where student.StudentID == StudentID
                                  select student).FirstOrDefault();
                }

                newStudent.LastName = LastNameTextBox.Text;
                newStudent.FirstMidName = FirstNameTextBox.Text;
                newStudent.EnrollmentDate = Convert.ToDateTime(EnrollmentDateTextBox.Text);

                if (StudentID == 0)
                {
                    db.Students.Add(newStudent);
                }

                //run an insert command
                db.SaveChanges();
                //redirect back to the students page
                Response.Redirect("~/Students.aspx");
            }
        }
    }
}