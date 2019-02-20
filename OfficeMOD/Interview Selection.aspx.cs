using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Drawing;
using System.Collections;
using System.Data.SqlClient;
using System.Configuration;

public partial class Interview_Selection : System.Web.UI.Page
{
    DataSet ds = new DataSet();
    DAccess2 d2 = new DAccess2();
    string usercode = string.Empty;
    string collegecode1 = string.Empty;
    string collegecode = string.Empty;
    string singleuser = string.Empty;
    string group_user = string.Empty;
    Hashtable hat = new Hashtable();
    protected void Page_Load(object sender, EventArgs e)
    {

        if (Session["collegecode"] == null)
        {
            Response.Redirect("~/Default.aspx");
        }
        usercode = Session["usercode"].ToString();
        collegecode1 = Session["collegecode"].ToString();
        singleuser = Session["single_user"].ToString();
        group_user = Session["group_code"].ToString();
        if (!IsPostBack)
        {
            bindCompanyname();
            bindbatch();
            bindedu();
            binddate();
            interviewround();
            gview.Visible = false;
            btnsave.Visible = false;
        }
    }
    public void bindCompanyname()
    {
        try
        {
            gview.Visible = false;
            btnsave.Visible = false;
            ds.Clear();
            drpcompany.Items.Clear();
            string itemname = "select distinct CompanyPK, CompName from CompanyMaster  order by CompanyPK";
            ds.Clear();
            ds = d2.select_method_wo_parameter(itemname, "Text");
            if (ds.Tables[0].Rows.Count > 0)
            {
                drpcompany.DataSource = ds;
                drpcompany.DataTextField = "CompName";
                drpcompany.DataValueField = "CompanyPK";
                drpcompany.DataBind();


            }
            bindedu();
        }
        catch
        {
        }
    }
    public void binddate()
    {
        try
        {
            gview.Visible = false;
            btnsave.Visible = false;
            ddldate.Items.Clear();
            string datebind = "select convert(varchar, interviewdate, 103) as interviewdate  from Company_datails where CompanyFK='" + Convert.ToString(drpcompany.SelectedValue) + "'";
            DataSet dsdate = new DataSet();
            dsdate = d2.select_method_wo_parameter(datebind, "text");
            if (dsdate.Tables[0].Rows.Count > 0)
            {
                ddldate.DataSource = dsdate;
                ddldate.DataTextField = "interviewdate";
                ddldate.DataValueField = "interviewdate";
                ddldate.DataBind();
            }
        }
        catch
        {
        }

    }
    public void drpcompany_SelectedIndexChanged(object sender, EventArgs e)
    {
        bindedu();
        binddate();
        interviewround();
        gview.Visible = false;
        btnsave.Visible = false;
    }
    public void ddldate_SelectedIndexChanged(object sender, EventArgs e)
    {
       
        interviewround();
        gview.Visible = false;
        btnsave.Visible = false;
    }
    public void bindbatch()
    {
        try
        {
            gview.Visible = false;
            btnsave.Visible = false;
            cbl_batch.Items.Clear();
            hat.Clear();
            ds = d2.BindBatch();
            if (ds.Tables[0].Rows.Count > 0)
            {
                cbl_batch.DataSource = ds;
                cbl_batch.DataTextField = "batch_year";
                cbl_batch.DataValueField = "batch_year";
                cbl_batch.DataBind();
                //ddl_batch1.SelectedIndex = 3;
                if (cbl_batch.Items.Count > 0)
                {
                    for (int i = 0; i < cbl_batch.Items.Count; i++)
                    {
                        cbl_batch.Items[0].Selected = true;
                    }
                    // txt_batch.Text = "Batch(" + cbl_batch.Items.Count + ")";
                    txt_batch.Text = "Batch(" + 1 + ")";
                    //cb_batch.Checked = true;
                }
                else
                {
                    txt_batch.Text = "--Select--";
                    cb_batch.Checked = false;
                }
            }
        }
        catch
        {
        }
    }
    public void interviewround()
    {
        try
        {
            btnsave.Visible = false;
            gview.Visible = false;
            ds.Clear();
            Cblround.Items.Clear();

            string itemname = "select * from com_interviewmode where CompanyFK='" + drpcompany.SelectedValue + "' and  convert(varchar,interviewdate,103)='" + ddldate.SelectedValue + "'";
             DataSet insround = new DataSet();
            insround = d2.select_method_wo_parameter(itemname, "text");
            if (insround.Tables[0].Rows.Count > 0)
            {
                int cun=0;
                for (int i = 0; i < insround.Tables[0].Rows.Count; i++)
                {
                    string rounds = Convert.ToString(insround.Tables[0].Rows[i]["mode"]);
                    if (rounds != "")
                    {
                        string[] spl = rounds.Split(',');
                        if (spl.Length > 0)
                        {
                            for (int m = 0; m < spl.Length; m++)
                            {
                                string posi = d2.GetFunction("  select MasterValue from CO_MasterValues where MasterCode ='" + spl[m] + "' and MasterCriteria ='Mode Of Interview' ");
                                Cblround.Items.Insert(cun,posi);
                                cun++;
                            }
                        }
                    }
                }
            }
            else
            {
                txtround.Text = "--Select--";
                Cbround.Checked = false;
            }
     
        }
        catch
        {
        }
    }
    public void cb_batch_CheckedChanged(object sender, EventArgs e)
    {
        try
        {
            btnsave.Visible = false;
            gview.Visible = false;
            string buildvalue1 = string.Empty;
            string build1 = string.Empty;
            if (cb_batch.Checked == true)
            {
                for (int i = 0; i < cbl_batch.Items.Count; i++)
                {
                    if (cb_batch.Checked == true)
                    {
                        cbl_batch.Items[i].Selected = true;
                        txt_batch.Text = "Batch(" + (cbl_batch.Items.Count) + ")";
                        build1 = cbl_batch.Items[i].Value.ToString();
                        if (buildvalue1 == "")
                        {
                            buildvalue1 = build1;
                        }
                        else
                        {
                            buildvalue1 = buildvalue1 + "'" + "," + "'" + build1;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < cbl_batch.Items.Count; i++)
                {
                    cbl_batch.Items[i].Selected = false;
                    txt_batch.Text = "--Select--";
                }
            }

        }
        catch (Exception ex)
        {
        }
    }
    public void cbl_batch_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            gview.Visible = false;
            btnsave.Visible = false;
            int seatcount = 0;
            cb_batch.Checked = false;
            string buildvalue = string.Empty;
            string build = string.Empty;
            for (int i = 0; i < cbl_batch.Items.Count; i++)
            {
                if (cbl_batch.Items[i].Selected == true)
                {
                    seatcount = seatcount + 1;
                    txt_batch.Text = "--Select--";
                    build = cbl_batch.Items[i].Value.ToString();
                    if (buildvalue == "")
                    {
                        buildvalue = build;
                    }
                    else
                    {
                        buildvalue = buildvalue + "'" + "," + "'" + build;
                    }
                }
            }
            if (seatcount == cbl_batch.Items.Count)
            {
                txt_batch.Text = "Batch(" + seatcount.ToString() + ")";
                cb_batch.Checked = true;
            }
            else if (seatcount == 0)
            {
                txt_batch.Text = "--Select--";
                cb_batch.Text = "--Select--";
            }
            else
            {
                txt_batch.Text = "Batch(" + seatcount.ToString() + ")";
            }

        }
        catch (Exception ex)
        {
        }
    }

    public void cb_round_CheckedChanged(object sender, EventArgs e)
    {
        try
        {
            btnsave.Visible = false;
            gview.Visible = false;
            string buildvalue1 = string.Empty;
            string build1 = string.Empty;
            if (Cbround.Checked == true)
            {
                for (int i = 0; i < Cblround.Items.Count; i++)
                {
                    if (Cbround.Checked == true)
                    {
                        Cblround.Items[i].Selected = true;
                        txtround.Text = "Batch(" + (Cblround.Items.Count) + ")";
                        build1 = Cblround.Items[i].Value.ToString();
                        if (buildvalue1 == "")
                        {
                            buildvalue1 = build1;
                        }
                        else
                        {
                            buildvalue1 = buildvalue1 + "'" + "," + "'" + build1;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < Cblround.Items.Count; i++)
                {
                    Cblround.Items[i].Selected = false;
                    txtround.Text = "--Select--";
                }
            }

        }
        catch (Exception ex)
        {
        }
    }
    public void cbl_round_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            btnsave.Visible = false;
            gview.Visible = false;
            int seatcount = 0;
            Cbround.Checked = false;
            string buildvalue = string.Empty;
            string build = string.Empty;
            for (int i = 0; i < Cblround.Items.Count; i++)
            {
                if (Cblround.Items[i].Selected == true)
                {
                    seatcount = seatcount + 1;
                    txtround.Text = "--Select--";
                    build = Cblround.Items[i].Value.ToString();
                    if (buildvalue == "")
                    {
                        buildvalue = build;
                    }
                    else
                    {
                        buildvalue = buildvalue + "'" + "," + "'" + build;
                    }
                }
            }
            if (seatcount == Cblround.Items.Count)
            {
                txtround.Text = "Batch(" + seatcount.ToString() + ")";
                Cbround.Checked = true;
            }
            else if (seatcount == 0)
            {
                txtround.Text = "--Select--";
                Cbround.Text = "--Select--";
            }
            else
            {
                txtround.Text = "Batch(" + seatcount.ToString() + ")";
            }

        }
        catch (Exception ex)
        {
        }
    }
    public void btn_go_Click(object sender, EventArgs e)
    {
        try
        {
            #region datatable
            DataRow drrow = null;
            DataTable dtTTDisp = new DataTable();

            dtTTDisp.Columns.Add("App_no");
            dtTTDisp.Columns.Add("SNo.");
            dtTTDisp.Columns.Add("Roll No");
            dtTTDisp.Columns.Add("Reg No");
            dtTTDisp.Columns.Add("Student Name");

            dtTTDisp.Columns.Add("Batch");
            //dtTTDisp.Columns.Add("Degree");

            dtTTDisp.Columns.Add("Section");
            dtTTDisp.Columns.Add("Semester");
            dtTTDisp.Columns.Add("Applied Post");
            int y = dtTTDisp.Columns.Count;
            drrow = dtTTDisp.NewRow();

            drrow["App_no"] = "App_no";
            drrow["SNo."] = "SNo.";
            drrow["Roll No"] = "Roll No";
            drrow["Reg No"] = "Reg No";
            drrow["Student Name"] = "Student Name";
            drrow["Batch"] = "Batch";
            //drrow["Degree"] = "Degree";
            drrow["Semester"] = "Semester";
            drrow["Section"] = "Section";
            drrow["Applied Post"] = "Applied Post";
            if (Cblround.Items.Count > 0)
            {
                int m = 0;
                for (int i = 0; i < Cblround.Items.Count; i++)
                {
                    m++;
                    if (Cblround.Items[i].Selected == true)
                    {
                        // dtTTDisp.Columns.Add("R" + Cblround.Items[i].Text);
                        dtTTDisp.Columns.Add("R" + m, System.Type.GetType("System.Boolean"));
                        //drrow["R" + Cblround.Items[i].Text] = "Stages";

                    }
                }
            }
            dtTTDisp.Rows.Add(drrow);
            drrow = dtTTDisp.NewRow();

            drrow["App_no"] = "App_no";
            drrow["SNo."] = "SNo.";
            drrow["Roll No"] = "Roll No";
            drrow["Reg No"] = "Reg No";
            drrow["Student Name"] = "Student Name";
            drrow["Batch"] = "Batch";
            //drrow["Degree"] = "Degree";
            drrow["Semester"] = "Semester";
            drrow["Section"] = "Section";
            drrow["Applied Post"] = "Applied Post";
            if (Cblround.Items.Count > 0)
            {
                for (int i = 0; i < Cblround.Items.Count; i++)
                {
                    if (Cblround.Items[i].Selected == true)
                    {


                        // drrow["R" + Cblround.Items[i].Text] = "R" + Cblround.Items[i].Text;
                    }
                }

            }
            dtTTDisp.Rows.Add(drrow);
            //drrow = dtTTDisp.NewRow();
            //dtTTDisp.Rows.Add(drrow);
            #endregion
            string Batch_tagvalue = string.Empty;
            for (int i = 0; i < cbl_batch.Items.Count; i++)
            {
                if (cbl_batch.Items[i].Selected == true)
                {
                    string addbatch1 = cbl_batch.Items[i].Value.ToString();
                    if (Batch_tagvalue == "")
                    {
                        Batch_tagvalue = addbatch1;
                    }
                    else
                    {
                        Batch_tagvalue = Batch_tagvalue + "'" + "," + "'" + addbatch1;
                    }
                }
            }
            string branch = string.Empty;
            for (int i = 0; i < cbldepartment.Items.Count; i++)
            {
                if (cbldepartment.Items[i].Selected == true)
                {
                    string branch1 = cbldepartment.Items[i].Value.ToString();
                    if (branch == "")
                    {
                        branch = branch1;
                    }
                    else
                    {
                        branch = branch + "'" + "," + "'" + branch1;
                    }
                }
            }
            string dates = string.Empty;
            dates = Convert.ToString(ddldate.SelectedValue);
            string[] spl = dates.Split('/');
            int getdate = 0;
            int.TryParse(spl[0], out getdate);
            if (getdate < 10)
            {
                String startOfString = spl[0].Remove(0, 1);
                spl[0] = startOfString;
            }
            string colmname = "D" + Convert.ToString(spl[0]);
            if (Batch_tagvalue != "" && drpcompany.SelectedValue != "" && branch != "" && Convert.ToString(ddldate.SelectedValue) != "" && branch != "")
            {
                string qury = "select r.Stud_Name,r.batch_year,r.Roll_No,r.Reg_No,r.App_No,Dept_Name,c.course_name,r.Current_Semester,r.Sections,(select MasterValue from CO_MasterValues where MasterCode=cd.composition) as appposted from  Company_StudentRegistration cr,Cm_Attendance ca, Registration r,degree d,Department dt,Course C,  CompanyMaster cm, Company_datails cd where r.App_No=cr.APP_No and cr.CompanyFK=cm.CompanyPK and cm.CompanyPK=cd.CompanyFK and cm.CompanyPK='" + Convert.ToString(drpcompany.SelectedValue) + "' and r.Batch_Year in('" + Batch_tagvalue + "') and cd.composition=cr.composition and d.Degree_Code =r.degree_code  and d.Dept_Code =dt.Dept_Code and c.Course_Id =d.Course_Id and cd.Companydetailspk=cr.CompanydetailsFk and convert(varchar,cd.interviewdate,103)='" + Convert.ToString(ddldate.SelectedValue) + "' and cd.interviewdate=cr.interviewdate and r.degree_code in('" + branch + "') and ca.App_No=cr.APP_No and ca.CompanyFK=cm.CompanyPK and " + colmname + "='1' order by r.Roll_No";

                qury = qury + " select * from Cm_Studentselection where   CompanyFK='" + Convert.ToString(drpcompany.SelectedValue) + "' and convert(varchar,interviewdate,103)='" + Convert.ToString(ddldate.SelectedValue) + "' ";
                ds.Clear();
                ds = d2.select_method_wo_parameter(qury, "Text");
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count == 0)
                {
                    imgdiv2.Visible = true;
                    lbl_alert.Text = "No Record";
                }
                else
                {
                    int cun = 0;
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        cun++;
                        drrow = dtTTDisp.NewRow();
                        if (i == 0)
                        {
                            drrow["SNo."] = Convert.ToString(ds.Tables[0].Rows[i]["course_name"]) + '-' + Convert.ToString(ds.Tables[0].Rows[i]["Dept_Name"]);
                            dtTTDisp.Rows.Add(drrow);
                        }
                        else
                        {
                            if (Convert.ToString(ds.Tables[0].Rows[i]["course_name"]) + '-' + Convert.ToString(ds.Tables[0].Rows[i - 1]["Dept_Name"]) != Convert.ToString(ds.Tables[0].Rows[i - 1]["course_name"]) + '-' + Convert.ToString(ds.Tables[0].Rows[i - 1]["Dept_Name"]))
                            {
                                drrow = dtTTDisp.NewRow();
                                drrow["SNo."] = Convert.ToString(ds.Tables[0].Rows[i]["course_name"]) + '-' + Convert.ToString(ds.Tables[0].Rows[i]["Dept_Name"]);
                                dtTTDisp.Rows.Add(drrow);
                            }
                        }
                        drrow = dtTTDisp.NewRow();
                        drrow["App_no"] = Convert.ToString(ds.Tables[0].Rows[i]["App_No"]);
                        drrow["SNo."] = cun;
                        drrow["Student Name"] = Convert.ToString(ds.Tables[0].Rows[i]["Stud_Name"]);
                        drrow["Roll No"] = Convert.ToString(ds.Tables[0].Rows[i]["Roll_No"]);
                        drrow["Reg No"] = Convert.ToString(ds.Tables[0].Rows[i]["Reg_No"]);
                        drrow["Batch"] = Convert.ToString(ds.Tables[0].Rows[i]["Batch_year"]);
                        drrow["Semester"] = Convert.ToString(ds.Tables[0].Rows[i]["Current_Semester"]);
                        drrow["Section"] = Convert.ToString(ds.Tables[0].Rows[i]["Sections"]);
                        drrow["Applied Post"] = Convert.ToString(ds.Tables[0].Rows[i]["appposted"]);
                        ds.Tables[1].DefaultView.RowFilter = "app_no='" + Convert.ToString(ds.Tables[0].Rows[i]["App_No"]) + "'";
                        DataView dvStudentAttendance = ds.Tables[1].DefaultView;
                        dtTTDisp.Rows.Add(drrow);
                        if (dvStudentAttendance.Count > 0)
                        {
                            if (Cblround.Items.Count > 0)
                            {
                                int colcun = 0;
                                for (int m = 0; m < Cblround.Items.Count; m++)
                                {
                                    colcun++;
                                    if (Cblround.Items[m].Selected == true)
                                    {

                                        string coln = "R" + colcun;
                                        if (Convert.ToString(dvStudentAttendance[0][coln]) == "1")
                                        {

                                            dtTTDisp.Rows[dtTTDisp.Rows.Count - 1][coln] = true;
                                            // drrow["R" + Cblround.Items[i].Text] = "R" + Cblround.Items[i].Text;
                                        }
                                        else
                                        {
                                            dtTTDisp.Rows[dtTTDisp.Rows.Count - 1][coln] = false;
                                        }
                                    }
                                }

                            }
                        }

                    }
                    if (dtTTDisp.Rows.Count > 1)
                    {
                        //Fpspread2.Sheets[0].SetColumnMerge(u, FarPoint.Web.Spread.Model.MergePolicy.Always);



                        gview.DataSource = dtTTDisp;
                        gview.DataBind();
                        gview.Visible = true;

                        btnsave.Visible = true;
                        for (int i = 2; i < gview.Rows.Count; i++)
                        {
                            int row = 0;
                            for (int c = 0; c < gview.Rows[0].Cells.Count; c++)
                            {

                                string val = string.Empty;
                                if (gview.Rows[0].Cells[c].Text == "")
                                {
                                    if (row == 0)
                                        row = c;
                                    int a = c - row;
                                    if (a < 10)
                                    {


                                        val = "0" + a + "";
                                    }
                                    else
                                    {
                                        val = Convert.ToString(c);
                                    }
                                    string chkname = "ctl" + val + "";
                                    CheckBox stud_rollno = (gview.Rows[i].Cells[c].FindControl(chkname)) as CheckBox;

                                    stud_rollno.Enabled = true;
                                }
                            }
                        }
                        int cblcun = 0;
                        if (gview.Rows.Count > 0)
                        {
                            if (Cblround.Items.Count > 0)
                            {
                                for (int m = 0; m < Cblround.Items.Count; m++)
                                {
                                    int a = 0;
                                    if (Cblround.Items[m].Selected == true)
                                    {
                                        cblcun++;
                                        for (int i = 0; i < gview.Rows[1].Cells.Count; i++)
                                        {
                                            string colname = gview.Rows[0].Cells[i].Text;
                                            if (gview.Rows[0].Cells[i].Text == "")
                                                gview.Rows[0].Cells[i].Text = "Stages";
                                            if (gview.Rows[1].Cells[i].Text == "")
                                            {
                                                if (a != 1)
                                                {
                                                    a = 1;

                                                    gview.Rows[1].Cells[i].Text = Cblround.Items[m].Text;
                                                }

                                            }
                                        }
                                    }

                                }
                            }
                        }

                        #region span
                        for (int i = gview.Rows.Count - 1; i >= 1; i--)
                        {
                            GridViewRow row = gview.Rows[i];
                            GridViewRow previousRow = gview.Rows[i - 1];
                            for (int j = 0; j < row.Cells.Count - cblcun; j++)
                            {


                                string date = row.Cells[j].Text;
                                string predate = previousRow.Cells[j].Text;

                                //string sub = row.Cells[2].Text;
                                //string presube = previousRow.Cells[2].Text;

                                //string hrs = row.Cells[3].Text;
                                //string prehrs = previousRow.Cells[3].Text;
                                //if (gview.HeaderRow.Cells[1].Text == "SNo.")
                                //{
                                if (date == predate)
                                {
                                    if (previousRow.Cells[j].RowSpan == 0)
                                    {
                                        if (row.Cells[j].RowSpan == 0)
                                        {
                                            previousRow.Cells[j].RowSpan += 2;
                                        }
                                        else
                                        {
                                            previousRow.Cells[j].RowSpan = row.Cells[j].RowSpan + 1;
                                        }
                                        row.Cells[j].Visible = false;
                                    }
                                }
                                //}
                                //if (gview.HeaderRow.Cells[2].Text == "Student Name")
                                //{
                                //    if (date == predate)
                                //    {
                                //        if (previousRow.Cells[2].RowSpan == 0)
                                //        {
                                //            if (row.Cells[2].RowSpan == 0)
                                //            {
                                //                previousRow.Cells[2].RowSpan += 2;
                                //            }
                                //            else
                                //            {
                                //                previousRow.Cells[2].RowSpan = row.Cells[2].RowSpan + 1;
                                //            }
                                //            row.Cells[1].Visible = false;
                                //        }
                                //    }
                                //}

                            }

                            for (int j = row.Cells.Count - 1; j >= 1; j--)
                            {
                                GridViewRow rows = gview.Rows[0];
                                GridViewRow previousRows = gview.Rows[0];
                                GridViewRow previousRowss = gview.Rows[2];
                                string date = gview.Rows[0].Cells[j].Text;
                                string predate = gview.Rows[0].Cells[j - 1].Text;
                                if (gview.Rows[0].Cells[j].Text == "Stages" || gview.Rows[0].Cells[j - 1].Text == "Stages")
                                {

                                    gview.Rows[0].Cells[j].ColumnSpan = cblcun;
                                    for (int a = 10; a < cblcun + 9; a++)
                                        rows.Cells[a].Visible = false;
                                    //if (date == predate)
                                    //{
                                    //    if (previousRows.Cells[j].ColumnSpan == 0)
                                    //    {
                                    //        if (rows.Cells[j].ColumnSpan == 0)
                                    //        {
                                    //            previousRows.Cells[j].ColumnSpan += 2;

                                    //        }
                                    //        else
                                    //        {
                                    //            previousRows.Cells[j].ColumnSpan = rows.Cells[j].ColumnSpan + 1;
                                    //        }
                                    //        rows.Cells[j].Visible = false;
                                    //        rows.Cells[j].HorizontalAlign = HorizontalAlign.Center;
                                    //    }
                                    //  }
                                }




                            }
                            for (int m = gview.Rows.Count - 1; m >= 2; m--)
                            {

                                GridViewRow rows = gview.Rows[m];
                                GridViewRow previousRows = gview.Rows[m];
                                GridViewRow previousRowss = gview.Rows[m];
                                gview.Rows[m].Cells[5].HorizontalAlign = HorizontalAlign.Center;
                                gview.Rows[m].Cells[6].HorizontalAlign = HorizontalAlign.Center;
                                gview.Rows[m].Cells[7].HorizontalAlign = HorizontalAlign.Center;
                                gview.Rows[m].Cells[1].HorizontalAlign = HorizontalAlign.Center;
                                string cellte = gview.Rows[m].Cells[1].Text;
                                if (!Convert.ToString(cellte).All(char.IsNumber))
                                {
                                    gview.Rows[m].Cells[1].ColumnSpan = gview.Rows[m].Cells.Count;

                                    gview.Rows[m].Cells[1].ColumnSpan = gview.Rows[m].Cells.Count;
                                    for (int j = 2; j < gview.Rows[m].Cells.Count; j++)
                                    {
                                        gview.Rows[m].Cells[0].Visible = false;
                                        gview.Rows[m].Cells[j].Visible = false;
                                    }
                                }



                                //if (date == predate)
                                //{
                                //    if (previousRows.Cells[j].ColumnSpan == 0)
                                //    {
                                //        if (rows.Cells[j].ColumnSpan == 0)
                                //        {
                                //            previousRows.Cells[j].ColumnSpan += 2;

                                //        }
                                //        else
                                //        {
                                //            previousRows.Cells[j].ColumnSpan = rows.Cells[j].ColumnSpan + 1;
                                //        }
                                //        rows.Cells[j].Visible = false;
                                //    }
                                //}


                            }
                            row.Cells[0].Visible = false;
                            gview.Rows[0].Cells[0].Visible = false;

                        }
                        RowHead(gview);
                        #endregion span

                    }
                }
            }
            else
            {
                imgdiv2.Visible = true;
                lbl_alert.Text = "Please Select All Feild";
            }

        }
        catch
        {
        }
    }
    protected void RowHead(GridView gview)
    {
        for (int head = 0; head < 2; head++)
        {
            gview.Rows[head].BackColor = ColorTranslator.FromHtml("#0CA6CA");
            gview.Rows[head].Font.Bold = true;
            gview.Rows[head].HorizontalAlign = HorizontalAlign.Center;
            gview.Rows[head].Font.Name = "Book Antique";
        }
    }
    public void btnsave_Click(object sender, EventArgs e)
    {
        int vcm = 0;
        string colmname = string.Empty;
        string compk = string.Empty;
        string applpost = string.Empty;
        string app_no = string.Empty;
        string date = string.Empty;
        date = Convert.ToString(ddldate.SelectedValue);
        string[] spl = date.Split('/');
        date = Convert.ToString(Convert.ToString(spl[1]).Trim() + "/" + Convert.ToString(spl[0]).Trim() + "/" + Convert.ToString(spl[2]).Trim());
        if (gview.Rows.Count > 0)
        {
            for (int i = 2; i < gview.Rows.Count; i++)
            {
                string insertcol = string.Empty;
                string insertcolval = string.Empty;
                string insertcolupdate = string.Empty;
                string insertcolvalupdate = string.Empty;
                int row = 0;
                for (int m = 9; m < gview.HeaderRow.Cells.Count; m++)
                {
                    if (Cblround.Items.Count > 0)
                    {
                        int colcun = 0;
                        for (int s = 0; s < Cblround.Items.Count; s++)
                        {
                            colcun++;
                            if (Cblround.Items[s].Text == gview.Rows[1].Cells[m].Text)
                            {
                                colmname = "R" + colcun;
                                s = Cblround.Items.Count;
                            }
                        }
                    }

                    
                    app_no = gview.Rows[i].Cells[0].Text;
                    applpost = gview.Rows[i].Cells[8].Text;
                    compk = Convert.ToString(drpcompany.SelectedValue);
                    if (app_no != "&nbsp;")
                    {
                        string val = string.Empty;
                        if (row == 0)
                            row = m;
                        int a = m - row;
                        if (a < 10)
                        {


                            val = "0" + a + "";
                        }
                        else
                        {
                            val = Convert.ToString(m);
                        }
                        string chkname = "ctl" + val + "";
                        CheckBox stud_rollno = (gview.Rows[i].Cells[m].FindControl(chkname)) as CheckBox;
                        if (stud_rollno.Checked)
                        {
                            if (insertcol == "")
                            {
                                insertcolupdate = colmname + '=' + "1";
                                insertcol = colmname;
                                insertcolval = "1";
                            }

                            else
                            {
                                insertcolupdate = insertcolupdate + ',' + colmname + '=' + "1";
                                insertcol = insertcol + ',' + colmname;
                                insertcolval = insertcolval + ',' + "1";
                            }
                        }
                        else
                        {
                            if (insertcol == "")
                            {
                                insertcolupdate = colmname + '=' + "2";
                                insertcol = colmname;
                                insertcolval = "2";
                            }

                            else
                            {
                                insertcolupdate = insertcolupdate + ',' + colmname + '=' + "2";
                                insertcol = insertcol + ',' + colmname;
                                insertcolval = insertcolval + ',' + "2";
                            }
                        }
                    }
                }
                string stu_selecton = string.Empty;
                if (insertcol != "")
                {
                    stu_selecton = "if exists(select * from Cm_Studentselection where app_no='" + app_no + "' and CompanyFK='" + compk + "' and interviewdate='" + date + "' ) update Cm_Studentselection set " + insertcolupdate + " where app_no='" + app_no + "' and CompanyFK='" + compk + "' and interviewdate='" + date + "' else  insert into Cm_Studentselection (APP_No,composition,interviewdate,CompanyFK," + insertcol + ") values('" + app_no + "','" + applpost + "','" + date + "','" + compk + "'," + insertcolval + ")";
                    vcm = d2.update_method_wo_parameter(stu_selecton, "TEXT");
                }
            }
            if (vcm != 0)
            {
                imgdiv3.Visible = true;
                Label1.Text = "Saved Successfully";
            }
           

            //CREATE TABLE Cm_Studentselection(selectionpk BigInt IDENTITY(1,1),APP_No BigInt, R1 tinyint,R2 tinyint,R3 tinyint,R4 tinyint,R5 tinyint,R6 tinyint, composition nvarchar(max),interviewdate Datetime,CompanyFK BigInt)
        }
    }

    public void binddegree()
    {
        try
        {
            gview.Visible = false;
            btnsave.Visible = false;
            cb_degree.Checked = false;
            string typ = "";
            if (cblcourse.Items.Count > 0)
            {
                for (int i = 0; i < cblcourse.Items.Count; i++)
                {
                    if (cblcourse.Items[i].Selected == true)
                    {
                        if (typ == "")
                        {
                            typ = "" + cblcourse.Items[i].Value + "";
                        }
                        else
                        {
                            typ = typ + "'" + "," + "'" + cblcourse.Items[i].Value + "";
                        }
                    }

                }
            }
            if (typ != "")
            {
                string deptquery = "select distinct d.Course_Id,c.Course_Name from Degree d,course c ,DeptPrivilages p where p.degree_code=d.Degree_Code and d.Course_Id=c.Course_Id and d.college_code=c.college_code and d.college_code='" + collegecode1 + "' and Edu_Level in('" + typ + "') ";
                ds.Clear();
                ds = d2.select_method_wo_parameter(deptquery, "Text");
                if (ds.Tables[0].Rows.Count > 0)
                {
                    cbldegree.DataSource = ds;
                    cbldegree.DataTextField = "Course_Name";
                    cbldegree.DataValueField = "Course_Id";
                    cbldegree.DataBind();
                }
                if (cbldegree.Items.Count > 0)
                {
                    string deu = "select distinct degree from IM_CompanyDept where CompanyFK='" + Convert.ToString(drpcompany.SelectedValue) + "'";
                    ds.Clear();
                    ds = d2.select_method_wo_parameter(deu, "Text");
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        int cun = 0;
                        for (int m = 0; m < ds.Tables[0].Rows.Count; m++)
                        {
                            for (int i = 0; i < cbldegree.Items.Count; i++)
                            {

                                if (Convert.ToString(ds.Tables[0].Rows[m]["degree"]) == cbldegree.Items[i].Value)
                                {
                                    cun++;
                                    cbldegree.Items[i].Enabled = true;
                                    cbldegree.Items[i].Selected = true;
                                }
                                else
                                {
                                    if (cbldegree.Items[i].Selected != true)
                                        cbldegree.Items[i].Enabled = false;
                                }
                            }
                        }
                        txtdegree.Text = "Degree(" + cun + ")";
                    }
                    else
                    {
                        for (int i = 0; i < cbldegree.Items.Count; i++)
                        {
                            cbldegree.Items[i].Enabled = false;
                        }
                        txtdegree.Text = "--Select--";
                    }

                }
            }
            binddepartment();
        }
        catch
        {
        }
    }

    public void binddepartment()
    {
        try
        {
            gview.Visible = false;
            btnsave.Visible = false;
            cb_departemt.Checked = false;
            string typ = "";
            if (cbldegree.Items.Count > 0)
            {
                for (int i = 0; i < cbldegree.Items.Count; i++)
                {
                    if (cbldegree.Items[i].Selected == true)
                    {
                        if (typ == "")
                        {
                            typ = "" + cbldegree.Items[i].Value + "";
                        }
                        else
                        {
                            typ = typ + "'" + "," + "'" + cbldegree.Items[i].Value + "";
                        }
                    }

                }
            }
            if (typ != "")
            {
                string deptquery = " select distinct degree.degree_code,department.dept_name,degree.Acronym  from degree,department,course where course.course_id=degree.course_id  and department.dept_code=degree.dept_code and course.college_code = degree.college_code and department.college_code = degree.college_code and degree.course_id in('" + typ + "') and  degree.college_code='" + collegecode1 + "'";
                ds.Clear();
                ds = d2.select_method_wo_parameter(deptquery, "Text");
                if (ds.Tables[0].Rows.Count > 0)
                {
                    cbldepartment.DataSource = ds;
                    cbldepartment.DataTextField = "dept_name";
                    cbldepartment.DataValueField = "degree_code";
                    cbldepartment.DataBind();
                }
                if (cbldepartment.Items.Count > 0)
                {
                    string deu = "select distinct deptcode from IM_CompanyDept where CompanyFK='" + Convert.ToString(drpcompany.SelectedValue) + "'";
                    ds.Clear();
                    ds = d2.select_method_wo_parameter(deu, "Text");
                    int cun = 0;
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int m = 0; m < ds.Tables[0].Rows.Count; m++)
                        {
                            for (int i = 0; i < cbldepartment.Items.Count; i++)
                            {

                                if (Convert.ToString(ds.Tables[0].Rows[m]["deptcode"]) == cbldepartment.Items[i].Value)
                                {
                                    cun++;
                                    cbldepartment.Items[i].Enabled = true;
                                    cbldepartment.Items[i].Selected = true;
                                }
                                else
                                {
                                    if (cbldepartment.Items[i].Selected != true)
                                        cbldepartment.Items[i].Enabled = false;
                                }
                            }
                        }
                        txtdept.Text = "Branch(" + cun + ")";
                    }
                    else
                    {
                        for (int i = 0; i < cbldepartment.Items.Count; i++)
                        {
                            cbldepartment.Items[i].Enabled = false;
                        }
                        txtdept.Text = "--Select--";
                    }

                }

            }
        }
        catch
        {
        }
    }
    public void bindedu()
    {
        try
        {
            btnsave.Visible = false;
            gview.Visible = false;
            string deptquery = " select distinct course.Edu_Level from degree,course where course.course_id=degree.course_id and course.college_code = degree.college_code and degree.college_code='" + collegecode1 + "'";
            ds.Clear();
            ds = d2.select_method_wo_parameter(deptquery, "Text");
            if (ds.Tables[0].Rows.Count > 0)
            {
                cblcourse.DataSource = ds;
                cblcourse.DataTextField = "Edu_Level";
                cblcourse.DataValueField = "Edu_Level";
                cblcourse.DataBind();
            }
            if (cblcourse.Items.Count > 0)
            {
                string deu = "select distinct course from IM_CompanyDept where CompanyFK='" + Convert.ToString(drpcompany.SelectedValue) + "'";
                ds.Clear();
                ds = d2.select_method_wo_parameter(deu, "Text");
                int cun = 0;
                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int m = 0; m < ds.Tables[0].Rows.Count; m++)
                    {
                        for (int i = 0; i < cblcourse.Items.Count; i++)
                        {

                            if (Convert.ToString(ds.Tables[0].Rows[m]["course"]) == cblcourse.Items[i].Value)
                            {
                                cun++;
                                cblcourse.Items[i].Enabled = true;
                                cblcourse.Items[i].Selected = true;
                            }
                            else
                            {
                                if (cblcourse.Items[i].Selected != true)
                                {
                                    cblcourse.Items[i].Enabled = false;
                                }
                            }
                        }
                    }
                    txtcourse.Text = "course(" + cun + ")";
                }
                else
                {
                    for (int i = 0; i < cblcourse.Items.Count; i++)
                    {
                        cblcourse.Items[i].Enabled = false;
                    }
                    txtcourse.Text = "--Select--";
                }

            }

            binddegree();
        }
        catch
        {
        }

    }
    protected void cb_degree_ChekedChange(object sender, EventArgs e)
    {
        try
        {
            btnsave.Visible = false;
            gview.Visible = false;
            if (cbldegree.Items.Count > 0)
            {
                int cun = 0;
                if (cb_degree.Checked == true)
                {
                    for (int i = 0; i < cbldegree.Items.Count; i++)
                    {
                        if (cbldegree.Items[i].Enabled == true)
                        {
                            cun++;
                            cbldegree.Items[i].Selected = true;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < cbldegree.Items.Count; i++)
                    {
                        if (cbldegree.Items[i].Enabled == true)
                            cbldegree.Items[i].Selected = false;
                    }
                }
                txtdegree.Text = "Degree(" + cun + ")";
            }

            binddepartment();

        }
        catch
        {
        }
    }
    protected void cb_course_ChekedChange(object sender, EventArgs e)
    {
        try
        {
            btnsave.Visible = false;
            gview.Visible = false;
            if (cblcourse.Items.Count > 0)
            {
                int cun = 0;
                if (cb_course.Checked == true)
                {
                    for (int i = 0; i < cblcourse.Items.Count; i++)
                    {
                        if (cblcourse.Items[i].Enabled == true)
                        {
                            cun++;
                            cblcourse.Items[i].Selected = true;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < cblcourse.Items.Count; i++)
                    {
                        if (cblcourse.Items[i].Enabled == true)
                            cblcourse.Items[i].Selected = false;
                    }
                }
                txtcourse.Text = "course(" + cun + ")";
            }

            binddegree();

        }
        catch
        {
        }
    }
    protected void cbdepartment_Change(object sender, EventArgs e)
    {
        try
        {
            gview.Visible = false;
            btnsave.Visible = false;
            if (cbldepartment.Items.Count > 0)
            {
                int cun = 0;
                if (cb_departemt.Checked == true)
                {
                    for (int i = 0; i < cbldepartment.Items.Count; i++)
                    {
                        if (cbldepartment.Items[i].Enabled == true)
                        {
                            cun++;
                            cbldepartment.Items[i].Selected = true;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < cbldepartment.Items.Count; i++)
                    {
                        if (cbldepartment.Items[i].Enabled == true)
                            cbldepartment.Items[i].Selected = false;
                    }
                }
                txtdept.Text = "Branch(" + cun + ")";
            }
        }
        catch
        {
        }
    }
    protected void cblcourse_ChekedChange(object sender, EventArgs e)
    {
        if (cblcourse.Items.Count > 0)
        {
            int cun = 0;
            for (int i = 0; i < cblcourse.Items.Count; i++)
            {
                if (cblcourse.Items[i].Selected == true)
                {
                    cun++;
                }
            }
            txtcourse.Text = "course(" + cun + ")";
        }
        binddegree();
        btnsave.Visible = false;
        gview.Visible = false;
    }
    protected void cbldegree_ChekedChange(object sender, EventArgs e)
    {
        if (cbldegree.Items.Count > 0)
        {
            int cun = 0;
            for (int i = 0; i < cbldegree.Items.Count; i++)
            {
                if (cbldegree.Items[i].Selected == true)
                {
                    cun++;
                }
            }
            txtdegree.Text = "Degree(" + cun + ")";
        }
        binddepartment();
        gview.Visible = false;
        btnsave.Visible = false;
    }
    protected void cbldepartment_ChekedChange(object sender, EventArgs e)
    {
        if (cbldepartment.Items.Count > 0)
        {
            int cun = 0;
            for (int i = 0; i < cbldepartment.Items.Count; i++)
            {
                if (cbldepartment.Items[i].Selected == true)
                {
                    cun++;
                }
            }
            txtdept.Text = "Branch(" + cun + ")";
        }
     
        gview.Visible = false;
        btnsave.Visible = false;
    }

    protected void btn_errorclose_Click(object sender, EventArgs e)
    {
        imgdiv2.Visible = false;
        gview.Visible = false;
    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        imgdiv3.Visible = false;
        gview.Visible = false;
        btn_go_Click(sender, e);
    }
}