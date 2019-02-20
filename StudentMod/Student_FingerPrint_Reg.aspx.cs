using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using InsproDataAccess;
using System.Data;
using System.Collections;
using System.Drawing;

public partial class StudentMod_Student_FingerPrint_Reg : System.Web.UI.Page
{
    #region Field_declaration
    InsproDirectAccess dirAcc = new InsproDirectAccess();
    InsproStoreAccess storeAcc = new InsproStoreAccess();
    DAccess2 d2 = new DAccess2();
    DataSet ds = new DataSet();
    Dictionary<string, string> dicStaffList = new Dictionary<string, string>();
    Dictionary<string, string> dicSQLParameter = new Dictionary<string, string>();
    Dictionary<string, string> dicQueryParameter = new Dictionary<string, string>();
    Dictionary<string, string> dictitle = new Dictionary<string, string>();
    DataSet dsprint = new DataSet();
    ArrayList colord = new ArrayList();
    DAccess2 da = new DAccess2();
    DataView dvhead = new DataView();
    Hashtable has = new Hashtable();
    Hashtable hat = new Hashtable();
    DataTable dtCommon = new DataTable();
    string userCode = string.Empty;
    string userCollegeCode = string.Empty;
    string singleUser = string.Empty;
    string groupUserCode = string.Empty;
    string collcode = string.Empty;
    string libcode = string.Empty;
    string libname = string.Empty;
    string activerow = "";
    string activecol = "";
    int selectedcount = 0;
    Boolean Cellclick = false;
    static string collegecode = string.Empty;
    string batch = "";
    string courseid = "";
    string bran = "";
    string sem = "";
    string sec = "";
    string Section = "";
    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (Session["collegecode"] == null)
            {
                Response.Redirect("~/Default.aspx");
            }
            else
            {
                userCollegeCode = (Session["collegecode"] != null) ? Convert.ToString(Session["collegecode"]).Trim() : "";
                userCode = (Session["usercode"] != null) ? Convert.ToString(Session["usercode"]).Trim() : "";
                singleUser = (Session["single_user"] != null) ? Convert.ToString(Session["single_user"]).Trim() : "";
                groupUserCode = (Session["group_code"] != null) ? Convert.ToString(Session["group_code"]).Trim() : "";
            }
            if (!IsPostBack)
            {
                Bindcollege();
                bindbatch();
                binddegree();
                bindbranch();
                bindsem();
                bindsec();
                stdbindbatch();
                stdbinddegree();
                stdbindbranch();
                stdbindsem();
                stdbindsec();
                studentlist();
                if (ddlcoll.Items.Count > 0)
                {
                    collegecode = Convert.ToString(ddlcoll.SelectedValue);
                }
            }
            if (ddlcoll.Items.Count > 0)
            {
                collegecode = Convert.ToString(ddlcoll.SelectedValue);
            }
        }
        catch
        {

        }
    }

    #region College

    public void Bindcollege()
    {
        try
        {
            ddlCollege.Items.Clear();
            dtCommon.Clear();
            ddlCollege.Enabled = false;
            dsprint.Clear();
            string qryUserCodeOrGroupCode = string.Empty;
            string group_user = ((Session["group_code"] != null) ? Convert.ToString(Session["group_code"]) : string.Empty);
            if (group_user.Contains(';'))
            {
                string[] group_semi = group_user.Split(';');
                group_user = Convert.ToString(group_semi[0]);
            }
            if ((Convert.ToString(group_user).Trim() != "") && Session["group_code"] != null && Session["single_user"] != null && Convert.ToString(Session["single_user"]).Trim() != "1" && Convert.ToString(Session["single_user"]).Trim().ToLower() != "true")
            {
                qryUserCodeOrGroupCode = " and group_code='" + group_user + "'";
            }
            else if (Session["usercode"] != null && !string.IsNullOrEmpty(Convert.ToString(Session["usercode"]).Trim()))
            {
                qryUserCodeOrGroupCode = " and user_code='" + Convert.ToString(Session["usercode"]).Trim() + "'";
            }
            if (!string.IsNullOrEmpty(qryUserCodeOrGroupCode))
            {
                dicQueryParameter.Clear();
                dicQueryParameter.Add("column_field", Convert.ToString(qryUserCodeOrGroupCode));
                dtCommon = storeAcc.selectDataTable("bind_college", dicQueryParameter);
            }
            if (dtCommon.Rows.Count > 0)
            {
                ddlCollege.DataSource = dtCommon;
                ddlCollege.DataTextField = "collname";
                ddlCollege.DataValueField = "college_code";
                ddlCollege.DataBind();
                ddlCollege.SelectedIndex = 0;
                ddlCollege.Enabled = true;


                ddlcoll.DataSource = dtCommon;
                ddlcoll.DataTextField = "collname";
                ddlcoll.DataValueField = "college_code";
                ddlcoll.DataBind();
                ddlcoll.SelectedIndex = 0;
                ddlcoll.Enabled = true;

            }
        }
        catch (Exception ex) { d2.sendErrorMail(ex, userCollegeCode, "Book_Reservation"); }
    }

    protected void ddlcollege_SelectedIndexChanged(object sender, EventArgs e)
    {
        bindbatch();
        binddegree();
        bindbranch();
        bindsem();
        bindsec();
        Fpspreadpop.Visible = false;
        rptprint.Visible = false;
        btndelete.Visible = false;

    }


    protected void ddlcoll_Change(object sender, EventArgs e)
    {
        stdbindbatch();
        stdbinddegree();
        stdbindbranch();
        stdbindsem();
        stdbindsec();
        studentlist();
    }

    #endregion

    #region Batch
    public void bindbatch()
    {
        try
        {

            ddlbatch.Items.Clear();
            ds = da.select_method_wo_parameter("bind_batch", "sp");
            int count = ds.Tables[0].Rows.Count;
            if (count > 0)
            {
                ddlbatch.DataSource = ds;
                ddlbatch.DataTextField = "batch_year";
                ddlbatch.DataValueField = "batch_year";
                ddlbatch.DataBind();

            }
            int count1 = ds.Tables[1].Rows.Count;
            if (count > 0)
            {
                int max_bat = 0;
                max_bat = Convert.ToInt32(ds.Tables[1].Rows[0][0].ToString());
                ddlbatch.SelectedValue = max_bat.ToString();
            }
        }
        catch (Exception ex) { }
    }
    protected void ddlbatch_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            bindbranch();
            binddegree();
            bindsem();
            bindsec();
            Fpspreadpop.Visible = false;
            rptprint.Visible = false;
            btndelete.Visible = false;

        }
        catch (Exception ex) { }

    }
    #endregion

    #region Degree
    public void binddegree()
    {
        try
        {

            ddldegree.Items.Clear();
            userCode = Session["usercode"].ToString();
            userCollegeCode = ddlCollege.SelectedItem.Value;
            singleUser = Session["single_user"].ToString();
            groupUserCode = Session["group_code"].ToString();
            if (groupUserCode.Contains(';'))
            {
                string[] group_semi = groupUserCode.Split(';');
                groupUserCode = group_semi[0].ToString();
            }
            has.Clear();
            has.Add("single_user", singleUser);
            has.Add("group_code", groupUserCode);
            has.Add("college_code", userCollegeCode);
            has.Add("user_code", userCode);
            ds = da.select_method("bind_degree", has, "sp");
            int count1 = ds.Tables[0].Rows.Count;
            if (count1 > 0)
            {
                ddldegree.DataSource = ds;
                ddldegree.DataTextField = "course_name";
                ddldegree.DataValueField = "course_id";
                ddldegree.DataBind();

            }
        }
        catch (Exception ex) { }

    }
    protected void ddldegree_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            bindbranch();
            bindsem();
            bindsec();

            Fpspreadpop.Visible = false;
            rptprint.Visible = false;
            btndelete.Visible = false;
        }
        catch (Exception ex) { }

    }
    #endregion


    #region Branch
    public void bindbranch()
    {
        try
        {

            ddlsem.Items.Clear();
            has.Clear();
            userCode = Session["usercode"].ToString();
            userCollegeCode = ddlCollege.SelectedItem.Value;
            singleUser = Session["single_user"].ToString();
            groupUserCode = Session["group_code"].ToString();
            if (groupUserCode.Contains(';'))
            {
                string[] group_semi = groupUserCode.Split(';');
                groupUserCode = group_semi[0].ToString();
            }
            has.Add("single_user", singleUser);
            has.Add("group_code", groupUserCode);
            has.Add("course_id", ddldegree.SelectedValue);
            has.Add("college_code", userCollegeCode);
            has.Add("user_code", userCode);
            ds = da.select_method("bind_branch", has, "sp");
            int count2 = ds.Tables[0].Rows.Count;
            if (count2 > 0)
            {
                ddlbranch.DataSource = ds;
                ddlbranch.DataTextField = "dept_name";
                ddlbranch.DataValueField = "degree_code";
                ddlbranch.DataBind();

            }
        }
        catch (Exception ex) { }

    }
    protected void ddlbranch_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            bindsem();
            bindsec();
            Fpspreadpop.Visible = false;
            rptprint.Visible = false;
            btndelete.Visible = false;
        }
        catch (Exception ex) { d2.sendErrorMail(ex, userCollegeCode, "Book_Reservation"); }

    }
    #endregion

    #region Sem
    public void bindsem()
    {
        try
        {

            ddlsem.Items.Clear();
            string duration = string.Empty;
            Boolean first_year = false;
            has.Clear();
            userCollegeCode = ddlCollege.SelectedItem.Value;
            has.Add("degree_code", ddlbranch.SelectedValue.ToString());
            has.Add("batch_year", ddlbatch.SelectedValue.ToString());
            has.Add("college_code", userCollegeCode);
            ds = da.select_method("bind_sem", has, "sp");
            int count3 = ds.Tables[0].Rows.Count;
            if (count3 > 0)
            {
                ddlsem.Enabled = true;
                duration = ds.Tables[0].Rows[0][0].ToString();
                first_year = Convert.ToBoolean(ds.Tables[0].Rows[0][1].ToString());
                for (int loop_val = 1; loop_val <= Convert.ToInt16(duration); loop_val++)
                {
                    if (first_year == false)
                    {
                        ddlsem.Items.Add(loop_val.ToString());
                    }
                    else if (first_year == true && loop_val != 2)
                    {
                        ddlsem.Items.Add(loop_val.ToString());
                    }
                }
            }
            else
            {
                count3 = ds.Tables[1].Rows.Count;
                if (count3 > 0)
                {
                    ddlsem.Enabled = true;
                    duration = ds.Tables[1].Rows[0][0].ToString();
                    first_year = Convert.ToBoolean(ds.Tables[1].Rows[0][1].ToString());
                    for (int loop_val = 1; loop_val <= Convert.ToInt16(duration); loop_val++)
                    {
                        if (first_year == false)
                        {
                            ddlsem.Items.Add(loop_val.ToString());
                        }
                        else if (first_year == true && loop_val != 2)
                        {
                            ddlsem.Items.Add(loop_val.ToString());
                        }
                    }
                }
                else
                {
                    ddlsem.Enabled = false;
                }

            }
        }
        catch (Exception ex) { }

    }
    protected void ddlsem_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            bindsec();
            Fpspreadpop.Visible = false;
            rptprint.Visible = false;
            btndelete.Visible = false;
        }
        catch (Exception ex) { }

    }
    #endregion

    #region Sec
    public void bindsec()
    {
        try
        {

            ddlSec.Items.Clear();
            hat.Clear();
            hat.Add("batch_year", ddlbatch.SelectedValue.ToString());
            hat.Add("degree_code", ddlbranch.SelectedValue);
            ds = da.select_method("bind_sec", hat, "sp");
            int count5 = ds.Tables[0].Rows.Count;
            if (count5 > 0)
            {
                ddlSec.DataSource = ds;
                ddlSec.DataTextField = "sections";
                ddlSec.DataValueField = "sections";
                ddlSec.DataBind();
                ddlSec.Enabled = true;
            }
            else
            {
                ddlSec.Enabled = false;
            }
            ddlSec.Items.Add("All");
        }
        catch (Exception ex) { }

    }
    #endregion

    #region Go
    protected void btngo_click(object sender, EventArgs e)
    {


        try
        {
            string selq = "";

            if (ddlCollege.Items.Count > 0)
                collcode = Convert.ToString(ddlCollege.SelectedValue);
            if (dlbatch.Items.Count > 0)
                batch = Convert.ToString(ddlbatch.SelectedValue);
            if (dldegree.Items.Count > 0)
                courseid = Convert.ToString(ddldegree.SelectedValue);
            if (dlbranch.Items.Count > 0)
                bran = Convert.ToString(ddlbranch.SelectedValue);
            if (dlsem.Items.Count > 0)
                sem = Convert.ToString(ddlsem.SelectedValue);
            if (dlsec.Items.Count > 0)
                sec = Convert.ToString(ddlSec.SelectedValue).Trim();

            if (sec == "" || sec == "All")
                Section = "";
            else
                Section = "and R.sections='" + sec + "'";

            if (collcode != "")
            {
                selq = "SELECT distinct R.Roll_No,R.Stud_Name,C.Course_Name + '-' + D.Dept_Name as Degree, R.Current_Semester,R.finger_id FROM Registration R,Degree G,Course C,Department D WHERE R.Degree_Code = G.Degree_Code AND G.Course_ID = C.Course_ID AND G.College_Code = C.College_Code AND G.Dept_Code = D.Dept_Code AND G.College_Code = D.College_Code AND CC = 0 AND DelFlag = 0 AND Exam_Flag = 'OK'  and R.batch_year='" + batch + "' and G.Degree_Code='" + bran + "' AND C.Course_Id='" + courseid + "'  and C.college_code='" + collcode + "' and R.Current_Semester='" + sem + "' " + Section + " ";
                if (rbfingerid.Checked == true)
                    selq = selq + " and ((R.finger_id is not null) and (cast(R.finger_id as varchar)<>''))";
                else
                    selq = selq + " and ((R.finger_id is null) or (cast(R.finger_id as varchar)=''))";
                selq = selq + "order by Roll_No";
            }
            ds.Clear();
            ds = d2.select_method_wo_parameter(selq, "Text");
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                loadrepsprcolumns();
                FarPoint.Web.Spread.TextCellType txtcell = new FarPoint.Web.Spread.TextCellType();
                FarPoint.Web.Spread.CheckBoxCellType cb = new FarPoint.Web.Spread.CheckBoxCellType();
                cb.AutoPostBack = false;
                FarPoint.Web.Spread.CheckBoxCellType cball = new FarPoint.Web.Spread.CheckBoxCellType();
                cball.AutoPostBack = true;


                Fpspreadpop.Sheets[0].RowCount++;
                Fpspreadpop.Sheets[0].Cells[0, 1].CellType = cball;
                Fpspreadpop.Sheets[0].Cells[0, 1].Value = 0;
                Fpspreadpop.Sheets[0].Cells[0, 1].HorizontalAlign = HorizontalAlign.Center;
                Fpspreadpop.Sheets[0].Cells[0, 1].Font.Name = "Book Antiqua";


                for (int ik = 0; ik < ds.Tables[0].Rows.Count; ik++)
                {
                    Fpspreadpop.Sheets[0].RowCount++;
                    Fpspreadpop.Sheets[0].Cells[Fpspreadpop.Sheets[0].RowCount - 1, 0].Text = Convert.ToString(ik + 1);
                    Fpspreadpop.Sheets[0].Cells[Fpspreadpop.Sheets[0].RowCount - 1, 0].HorizontalAlign = HorizontalAlign.Center;
                    Fpspreadpop.Sheets[0].Cells[Fpspreadpop.Sheets[0].RowCount - 1, 0].Font.Name = "Book Antiqua";
                    Fpspreadpop.Sheets[0].Cells[Fpspreadpop.Sheets[0].RowCount - 1, 0].Font.Size = FontUnit.Medium;

                    Fpspreadpop.Sheets[0].Cells[Fpspreadpop.Sheets[0].RowCount - 1, 1].CellType = cb;
                    Fpspreadpop.Sheets[0].Cells[Fpspreadpop.Sheets[0].RowCount - 1, 1].HorizontalAlign = HorizontalAlign.Center;
                    Fpspreadpop.Sheets[0].Cells[Fpspreadpop.Sheets[0].RowCount - 1, 1].Font.Name = "Book Antiqua";
                    Fpspreadpop.Sheets[0].Cells[Fpspreadpop.Sheets[0].RowCount - 1, 1].Font.Size = FontUnit.Medium;

                    Fpspreadpop.Sheets[0].Cells[Fpspreadpop.Sheets[0].RowCount - 1, 2].Text = Convert.ToString(ds.Tables[0].Rows[ik]["Roll_No"]);
                    Fpspreadpop.Sheets[0].Cells[Fpspreadpop.Sheets[0].RowCount - 1, 2].CellType = txtcell;
                    Fpspreadpop.Sheets[0].Cells[Fpspreadpop.Sheets[0].RowCount - 1, 2].HorizontalAlign = HorizontalAlign.Left;
                    Fpspreadpop.Sheets[0].Cells[Fpspreadpop.Sheets[0].RowCount - 1, 2].Font.Name = "Book Antiqua";
                    Fpspreadpop.Sheets[0].Cells[Fpspreadpop.Sheets[0].RowCount - 1, 2].Font.Size = FontUnit.Medium;

                    Fpspreadpop.Sheets[0].Cells[Fpspreadpop.Sheets[0].RowCount - 1, 3].Text = Convert.ToString(ds.Tables[0].Rows[ik]["Stud_Name"]);
                    Fpspreadpop.Sheets[0].Cells[Fpspreadpop.Sheets[0].RowCount - 1, 3].HorizontalAlign = HorizontalAlign.Left;
                    Fpspreadpop.Sheets[0].Cells[Fpspreadpop.Sheets[0].RowCount - 1, 3].Font.Name = "Book Antiqua";
                    Fpspreadpop.Sheets[0].Cells[Fpspreadpop.Sheets[0].RowCount - 1, 3].Font.Size = FontUnit.Medium;

                    Fpspreadpop.Sheets[0].Cells[Fpspreadpop.Sheets[0].RowCount - 1, 4].Text = Convert.ToString(ds.Tables[0].Rows[ik]["finger_id"]);
                    Fpspreadpop.Sheets[0].Cells[Fpspreadpop.Sheets[0].RowCount - 1, 4].HorizontalAlign = HorizontalAlign.Left;
                    Fpspreadpop.Sheets[0].Cells[Fpspreadpop.Sheets[0].RowCount - 1, 4].Font.Name = "Book Antiqua";
                    Fpspreadpop.Sheets[0].Cells[Fpspreadpop.Sheets[0].RowCount - 1, 4].Font.Size = FontUnit.Medium;


                }

                Fpspreadpop.Sheets[0].PageSize = Fpspreadpop.Sheets[0].RowCount;
                Fpspreadpop.Visible = true;
                rptprint.Visible = true;
                btndelete.Visible = true;
            }
            else
            {
                Fpspreadpop.Visible = false;
                rptprint.Visible = false;
                btndelete.Visible = false;
                alertpopwindow.Visible = true;
                lblalerterr.Text = "No Record Found!";

            }
        }
        catch { }
    }


    private void loadrepsprcolumns()
    {
        try
        {
            Fpspreadpop.Sheets[0].RowCount = 0;
            Fpspreadpop.Sheets[0].ColumnCount = 5;
            Fpspreadpop.CommandBar.Visible = false;
            Fpspreadpop.RowHeader.Visible = false;
            Fpspreadpop.Sheets[0].AutoPostBack = false;
            Fpspreadpop.Sheets[0].ColumnHeader.RowCount = 1;
            Fpspreadpop.Sheets[0].FrozenRowCount = 1;

            FarPoint.Web.Spread.StyleInfo darkstyle = new FarPoint.Web.Spread.StyleInfo();
            darkstyle.BackColor = ColorTranslator.FromHtml("#0CA6CA");
            darkstyle.ForeColor = Color.Black;
            Fpspreadpop.ActiveSheetView.ColumnHeader.DefaultStyle = darkstyle;

            Fpspreadpop.Sheets[0].ColumnHeader.Cells[0, 0].Text = "S.No";
            Fpspreadpop.Sheets[0].ColumnHeader.Cells[0, 0].Font.Bold = true;
            Fpspreadpop.Sheets[0].ColumnHeader.Cells[0, 0].HorizontalAlign = HorizontalAlign.Center;
            Fpspreadpop.Sheets[0].ColumnHeader.Cells[0, 0].Font.Name = "Book Antiqua";
            Fpspreadpop.Sheets[0].ColumnHeader.Cells[0, 0].Font.Size = FontUnit.Medium;
            Fpspreadpop.Columns[0].Locked = true;
            Fpspreadpop.Columns[0].Width = 50;




            Fpspreadpop.Sheets[0].ColumnHeader.Cells[0, 1].Text = "Select";
            Fpspreadpop.Sheets[0].ColumnHeader.Cells[0, 1].Font.Bold = true;
            Fpspreadpop.Sheets[0].ColumnHeader.Cells[0, 1].HorizontalAlign = HorizontalAlign.Center;
            Fpspreadpop.Sheets[0].ColumnHeader.Cells[0, 1].Font.Name = "Book Antiqua";
            Fpspreadpop.Sheets[0].ColumnHeader.Cells[0, 1].Font.Size = FontUnit.Medium;
            Fpspreadpop.Columns[1].Width = 75;

            Fpspreadpop.Sheets[0].ColumnHeader.Cells[0, 2].Text = "Roll No";
            Fpspreadpop.Sheets[0].ColumnHeader.Cells[0, 2].Font.Bold = true;
            Fpspreadpop.Sheets[0].ColumnHeader.Cells[0, 2].HorizontalAlign = HorizontalAlign.Center;
            Fpspreadpop.Sheets[0].ColumnHeader.Cells[0, 2].Font.Name = "Book Antiqua";
            Fpspreadpop.Sheets[0].ColumnHeader.Cells[0, 2].Font.Size = FontUnit.Medium;
            Fpspreadpop.Columns[2].Width = 165;
            Fpspreadpop.Columns[2].Locked = true;

            Fpspreadpop.Sheets[0].ColumnHeader.Cells[0, 3].Text = "Student Name";
            Fpspreadpop.Sheets[0].ColumnHeader.Cells[0, 3].Font.Bold = true;
            Fpspreadpop.Sheets[0].ColumnHeader.Cells[0, 3].HorizontalAlign = HorizontalAlign.Center;
            Fpspreadpop.Sheets[0].ColumnHeader.Cells[0, 3].Font.Name = "Book Antiqua";
            Fpspreadpop.Sheets[0].ColumnHeader.Cells[0, 3].Font.Size = FontUnit.Medium;
            Fpspreadpop.Columns[3].Locked = true;
            Fpspreadpop.Columns[3].Width = 220;

            Fpspreadpop.Sheets[0].ColumnHeader.Cells[0, 4].Text = "Finger ID";
            Fpspreadpop.Sheets[0].ColumnHeader.Cells[0, 4].Font.Bold = true;
            Fpspreadpop.Sheets[0].ColumnHeader.Cells[0, 4].HorizontalAlign = HorizontalAlign.Center;
            Fpspreadpop.Sheets[0].ColumnHeader.Cells[0, 4].Font.Name = "Book Antiqua";
            Fpspreadpop.Sheets[0].ColumnHeader.Cells[0, 4].Font.Size = FontUnit.Medium;
            Fpspreadpop.Columns[4].Locked = true;
            Fpspreadpop.Columns[4].Width = 120;

            if (rbnofingerid.Checked == true)
            {

                Fpspreadpop.Columns[4].Visible = false;

            }
            else
            {

                Fpspreadpop.Columns[4].Visible = true;

            }
        }
        catch { }
    }

    #endregion

    #region Print

    protected void btnprintmaster_Click(object sender, EventArgs e)
    {
        try
        {
            string degreedetails = "Student_FingerPrint_Reg_Report";
            string pagename = "Student_FingerPrint_Reg.aspx";
            Printcontrol.loadspreaddetails(Fpspreadpop, pagename, degreedetails);
            Printcontrol.Visible = true;
        }
        catch (Exception ex) { d2.sendErrorMail(ex, userCollegeCode, "Student_FingerPrint_Reg_Report"); }

    }

    protected void btnExcel_Click(object sender, EventArgs e)
    {
        try
        {
            string reportname = txtexcelname.Text;
            if (reportname.ToString().Trim() != "")
            {
                d2.printexcelreport(Fpspreadpop, reportname);
                lblvalidation1.Visible = false;
            }
            else
            {
                lblvalidation1.Text = "Please Enter Your Report Name";
                lblvalidation1.Visible = true;
                txtexcelname.Focus();
            }
        }
        catch (Exception ex) { d2.sendErrorMail(ex, userCollegeCode, "Student_FingerPrint_Reg_Report"); }

    }
    #endregion

    #region Add
    protected void btnAdd_click(object sender, EventArgs e)
    {
        try
        {
            poperrjs.Visible = true;
            stdbindbatch();
            stdbinddegree();
            stdbindbranch();
            stdbindsem();
            stdbindsec();
            studentlist();
            FpSpread.Visible = false;
            btnsave.Visible = false;
            txt_macid.Text = "";
            txt_macid_Change(sender, e);

        }
        catch
        {

        }

    }
    #endregion

    #region Delete
    protected void Fpspreadpop_ButtonCommand(object sender, FarPoint.Web.Spread.SpreadCommandEventArgs e)
    {
        Fpspreadpop.SaveChanges();
        try
        {
            int ik = 0;
            byte check = Convert.ToByte(Fpspreadpop.Sheets[0].Cells[0, 1].Value);
            if (check == 1)
            {
                for (ik = 1; ik < Fpspreadpop.Sheets[0].RowCount; ik++)
                {
                    Fpspreadpop.Sheets[0].Cells[ik, 1].Value = 1;
                }
            }
            else
            {
                for (ik = 1; ik < Fpspreadpop.Sheets[0].RowCount; ik++)
                {
                    Fpspreadpop.Sheets[0].Cells[ik, 1].Value = 0;
                }
            }
        }
        catch { }
    }

    private bool checkedspr()
    {
        bool ok = false;
        Fpspreadpop.SaveChanges();
        try
        {
            for (int ik = 0; ik < Fpspreadpop.Sheets[0].RowCount; ik++)
            {
                byte check = Convert.ToByte(Fpspreadpop.Sheets[0].Cells[ik, 1].Value);
                if (check == 1)
                    ok = true;
            }
        }
        catch { }
        return ok;
    }

    protected void btndelete_Click(object sender, EventArgs e)
    {
        try
        {
            if (checkedspr())
            {
                //lblpoperr.Visible = false;
                Fpspreadpop.SaveChanges();
                string delq = "";
                int delcount = 0;
                for (int ik = 0; ik < Fpspreadpop.Sheets[0].RowCount; ik++)
                {
                    byte check = Convert.ToByte(Fpspreadpop.Sheets[0].Cells[ik, 1].Value);
                    if (check == 1)
                    {
                        string rollno = Convert.ToString(Fpspreadpop.Sheets[0].Cells[ik, 2].Text);
                        delq = "update Registration set finger_id='' where Roll_No='" + rollno + "'";
                        int upcount = d2.update_method_wo_parameter(delq, "Text");
                        if (upcount > 0)
                            delcount++;
                    }
                }
                if (delcount > 0)
                {
                    alertpopwindow.Visible = true;
                    lblalerterr.Text = "Deleted Successfully!";
                    btngo_click(sender, e);
                }
            }
            else
            {
                alertpopwindow.Visible = true;
                lblalerterr.Visible = true;
                lblalerterr.Text = "Please Select Any Student!";
                //lblpoperr.Visible = true;
                //lblpoperr.Text = "Please Select Any Staff!";
            }
        }
        catch { }
    }
    #endregion

    #region FingerID_Match_Popup

    #region Batch
    public void stdbindbatch()
    {
        try
        {

            dlbatch.Items.Clear();
            ds = da.select_method_wo_parameter("bind_batch", "sp");
            int count = ds.Tables[0].Rows.Count;
            if (count > 0)
            {
                dlbatch.DataSource = ds;
                dlbatch.DataTextField = "batch_year";
                dlbatch.DataValueField = "batch_year";
                dlbatch.DataBind();
            }
            int count1 = ds.Tables[1].Rows.Count;
            if (count > 0)
            {
                int max_bat = 0;
                max_bat = Convert.ToInt32(ds.Tables[1].Rows[0][0].ToString());
                dlbatch.SelectedValue = max_bat.ToString();
            }
        }
        catch (Exception ex) { }
    }
    protected void dlbatch_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            stdbindbranch();
            stdbinddegree();
            stdbindsem();
            stdbindsec();
            studentlist();

        }
        catch (Exception ex) { }

    }
    #endregion

    #region Degree
    public void stdbinddegree()
    {
        try
        {

            dldegree.Items.Clear();
            userCode = Session["usercode"].ToString();
            userCollegeCode = ddlcoll.SelectedItem.Value;
            singleUser = Session["single_user"].ToString();
            groupUserCode = Session["group_code"].ToString();
            if (groupUserCode.Contains(';'))
            {
                string[] group_semi = groupUserCode.Split(';');
                groupUserCode = group_semi[0].ToString();
            }
            has.Clear();
            has.Add("single_user", singleUser);
            has.Add("group_code", groupUserCode);
            has.Add("college_code", userCollegeCode);
            has.Add("user_code", userCode);
            ds = da.select_method("bind_degree", has, "sp");
            int count1 = ds.Tables[0].Rows.Count;
            if (count1 > 0)
            {
                dldegree.DataSource = ds;
                dldegree.DataTextField = "course_name";
                dldegree.DataValueField = "course_id";
                dldegree.DataBind();
            }
        }
        catch (Exception ex) { }

    }
    protected void dldegree_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            stdbindbranch();
            stdbindsem();
            stdbindsec();
            studentlist();

        }
        catch (Exception ex) { }

    }
    #endregion


    #region Branch
    public void stdbindbranch()
    {
        try
        {

            dlsem.Items.Clear();
            has.Clear();
            userCode = Session["usercode"].ToString();
            userCollegeCode = ddlcoll.SelectedItem.Value;
            singleUser = Session["single_user"].ToString();
            groupUserCode = Session["group_code"].ToString();
            if (groupUserCode.Contains(';'))
            {
                string[] group_semi = groupUserCode.Split(';');
                groupUserCode = group_semi[0].ToString();
            }
            has.Add("single_user", singleUser);
            has.Add("group_code", groupUserCode);
            has.Add("course_id", dldegree.SelectedValue);
            has.Add("college_code", userCollegeCode);
            has.Add("user_code", userCode);
            ds = da.select_method("bind_branch", has, "sp");
            int count2 = ds.Tables[0].Rows.Count;
            if (count2 > 0)
            {
                dlbranch.DataSource = ds;
                dlbranch.DataTextField = "dept_name";
                dlbranch.DataValueField = "degree_code";
                dlbranch.DataBind();
            }
        }
        catch (Exception ex) { }

    }
    protected void dlbranch_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            stdbindsem();
            stdbindsec();
            studentlist();
        }
        catch (Exception ex) { d2.sendErrorMail(ex, userCollegeCode, "Book_Reservation"); }

    }
    #endregion

    #region Sem
    public void stdbindsem()
    {
        try
        {

            dlsem.Items.Clear();
            string duration = string.Empty;
            Boolean first_year = false;
            has.Clear();
            userCollegeCode = ddlcoll.SelectedItem.Value;
            has.Add("degree_code", dlbranch.SelectedValue.ToString());
            has.Add("batch_year", dlbatch.SelectedValue.ToString());
            has.Add("college_code", userCollegeCode);
            ds = da.select_method("bind_sem", has, "sp");
            int count3 = ds.Tables[0].Rows.Count;
            if (count3 > 0)
            {
                dlsem.Enabled = true;
                duration = ds.Tables[0].Rows[0][0].ToString();
                first_year = Convert.ToBoolean(ds.Tables[0].Rows[0][1].ToString());
                for (int loop_val = 1; loop_val <= Convert.ToInt16(duration); loop_val++)
                {
                    if (first_year == false)
                    {
                        dlsem.Items.Add(loop_val.ToString());
                    }
                    else if (first_year == true && loop_val != 2)
                    {
                        dlsem.Items.Add(loop_val.ToString());
                    }
                }
            }
            else
            {
                count3 = ds.Tables[1].Rows.Count;
                if (count3 > 0)
                {
                    dlsem.Enabled = true;
                    duration = ds.Tables[1].Rows[0][0].ToString();
                    first_year = Convert.ToBoolean(ds.Tables[1].Rows[0][1].ToString());
                    for (int loop_val = 1; loop_val <= Convert.ToInt16(duration); loop_val++)
                    {
                        if (first_year == false)
                        {
                            dlsem.Items.Add(loop_val.ToString());
                        }
                        else if (first_year == true && loop_val != 2)
                        {
                            dlsem.Items.Add(loop_val.ToString());
                        }
                    }
                }
                else
                {
                    dlsem.Enabled = false;
                }
            }
        }
        catch (Exception ex) { }

    }
    protected void dlsem_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            stdbindsec();
            studentlist();
        }
        catch (Exception ex) { }

    }
    #endregion

    #region Sec
    public void stdbindsec()
    {
        try
        {

            dlsec.Items.Clear();
            hat.Clear();
            hat.Add("batch_year", dlbatch.SelectedValue.ToString());
            hat.Add("degree_code", dlbranch.SelectedValue);
            ds = da.select_method("bind_sec", hat, "sp");
            int count5 = ds.Tables[0].Rows.Count;
            if (count5 > 0)
            {
                dlsec.DataSource = ds;
                dlsec.DataTextField = "sections";
                dlsec.DataValueField = "sections";
                dlsec.DataBind();
                dlsec.Enabled = true;
            }
            else
            {
                dlsec.Enabled = false;
            }
            dlsec.Items.Add("All");
        }
        catch (Exception ex) { }

    }


    protected void dlSec_SelectedIndexChanged(object sender, EventArgs e)
    {
        studentlist();
    
    }
    #endregion

    private void studentlist()
    {
        try
        {


            ddlstdlst.Items.Clear();
            if (ddlcoll.Items.Count > 0)
                collcode = Convert.ToString(ddlcoll.SelectedValue);
            if (dlbatch.Items.Count > 0)
                batch = Convert.ToString(dlbatch.SelectedValue);
            if (dldegree.Items.Count > 0)
                courseid = Convert.ToString(dldegree.SelectedValue);
            if (dlbranch.Items.Count > 0)
                bran = Convert.ToString(dlbranch.SelectedValue);
            if (dlsem.Items.Count > 0)
                sem = Convert.ToString(dlsem.SelectedValue);
            if (dlsec.Items.Count > 0)
                sec = Convert.ToString(dlsec.SelectedValue).Trim();

            if (sec == "" || sec == "All")
                Section = "";
            else
                Section = "and R.sections='" + sec + "'";

            string sqlgetstddetails = "SELECT distinct R.Stud_Name+ '$' +R.Roll_No Stud_Name,R.Roll_No, C.Course_Name + '-' + D.Dept_Name as Degree, R.Current_Semester FROM Registration R,Degree G,Course C,Department D WHERE R.Degree_Code = G.Degree_Code AND G.Course_ID = C.Course_ID AND G.College_Code = C.College_Code AND G.Dept_Code = D.Dept_Code AND G.College_Code = D.College_Code AND CC = 0 AND DelFlag = 0 AND Exam_Flag = 'OK' and R.batch_year='" + batch + "' and G.Degree_Code='" + bran + "' AND C.Course_Id='" + courseid + "'  and C.college_code='" + collcode + "' and R.Current_Semester='" + sem + "' " + Section + " order by Stud_Name";

            ds.Clear();
            ds = d2.select_method_wo_parameter(sqlgetstddetails, "Text");
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                ddlstdlst.DataSource = ds;
                ddlstdlst.DataTextField = "Stud_Name";
                ddlstdlst.DataValueField = "Roll_No";
                ddlstdlst.DataBind();
                ddlstdlst.Items.Insert(0, "Select");
            }
            else
            {
                ddlstdlst.Items.Insert(0, "Select");
            }
        }
        catch { }
    }

    protected void ddlstdlst_change(object sender, EventArgs e)
    {
        txt_macid.Text = "";
        txt_macid_Change(sender, e);
    }

    [System.Web.Services.WebMethod]
    [System.Web.Script.Services.ScriptMethod()]
    public static List<string> GetMacID(string prefixText)
    {
        WebService ws = new WebService();
        List<string> name = new List<string>();
        string query = "select distinct MachineNo from DeviceInfo where College_Code='" + collegecode + "' and MachineNo like '" + prefixText + "%'";
        name = ws.Getname(query);
        return name;
    }

    protected void txt_macid_Change(object sender, EventArgs e)
    {
        try
        {
            ddlfingerid.Items.Clear();
            int txtval = 0;
            Int32.TryParse(txt_macid.Text.Trim(), out txtval);
            if (txt_macid.Text.Trim() != "" || txtval != 0)
            {
                //Cmd By SaranyaDevi 16.4.2018

                //string selq = "select distinct cast(Enrollno as bigint) as Enrollno from bio..enrollments where Branchid='" + txt_macid.Text.Trim() + "' order by cast(Enrollno as bigint) asc";

                //Added By Saranyadevi 16.4.2018
                string selq = "select distinct cast(Enrollno as varchar) as Enrollno from bio..enrollments where Branchid='" + txt_macid.Text.Trim() + "' order by cast(Enrollno as varchar) asc";
                ds.Clear();
                ds = d2.select_method_wo_parameter(selq, "Text");
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    ddlfingerid.DataSource = ds;
                    ddlfingerid.DataTextField = "Enrollno";
                    ddlfingerid.DataValueField = "Enrollno";
                    ddlfingerid.DataBind();
                    ddlfingerid.Items.Insert(0, "Select");
                }
                else
                {
                    ddlfingerid.Items.Insert(0, "Select");
                }
            }
            else
            {
                ddlfingerid.Items.Insert(0, "Select");
            }
        }
        catch { }
    }


    protected bool checkstdcode()
    {
        bool chkspr = true;
        string staffcode = Convert.ToString(ddlstdlst.SelectedValue);
        string sprstaffcode = "";
        FpSpread.SaveChanges();
        try
        {
            for (int ik = 0; ik < FpSpread.Sheets[0].RowCount; ik++)
            {
                sprstaffcode = Convert.ToString(FpSpread.Sheets[0].Cells[ik, 1].Text);
                if (staffcode == sprstaffcode)
                    chkspr = false;
            }
        }
        catch { }
        return chkspr;
    }

    protected bool checkvalue()
    {
        bool chkspr = true;
        string sprstaffcode = "";
        FpSpread.SaveChanges();
        try
        {
            for (int ik = 0; ik < FpSpread.Sheets[0].RowCount; ik++)
            {
                sprstaffcode = Convert.ToString(FpSpread.Sheets[0].Cells[ik, 0].Text);
                if (sprstaffcode.Trim() == "")
                    chkspr = false;
            }
        }
        catch { }
        return chkspr;
    }

    private void loadsprcolumns()
    {
        try
        {
            FpSpread.Sheets[0].RowCount = 0;
            FpSpread.Sheets[0].ColumnCount = 5;
            FpSpread.CommandBar.Visible = false;
            FpSpread.RowHeader.Visible = false;
            FpSpread.Sheets[0].AutoPostBack = false;
            FpSpread.Sheets[0].ColumnHeader.RowCount = 1;

            FarPoint.Web.Spread.StyleInfo darkstyle = new FarPoint.Web.Spread.StyleInfo();
            darkstyle.BackColor = ColorTranslator.FromHtml("#0CA6CA");
            darkstyle.ForeColor = Color.Black;
            FpSpread.ActiveSheetView.ColumnHeader.DefaultStyle = darkstyle;

            FpSpread.Sheets[0].ColumnHeader.Cells[0, 0].Text = "S.No";
            FpSpread.Sheets[0].ColumnHeader.Cells[0, 0].Font.Bold = true;
            FpSpread.Sheets[0].ColumnHeader.Cells[0, 0].HorizontalAlign = HorizontalAlign.Center;
            FpSpread.Sheets[0].ColumnHeader.Cells[0, 0].Font.Name = "Book Antiqua";
            FpSpread.Sheets[0].ColumnHeader.Cells[0, 0].Font.Size = FontUnit.Medium;
            FpSpread.Columns[0].Locked = true;
            FpSpread.Columns[0].Width = 50;

            FpSpread.Sheets[0].ColumnHeader.Cells[0, 1].Text = "Roll No";
            FpSpread.Sheets[0].ColumnHeader.Cells[0, 1].Font.Bold = true;
            FpSpread.Sheets[0].ColumnHeader.Cells[0, 1].HorizontalAlign = HorizontalAlign.Center;
            FpSpread.Sheets[0].ColumnHeader.Cells[0, 1].Font.Name = "Book Antiqua";
            FpSpread.Sheets[0].ColumnHeader.Cells[0, 1].Font.Size = FontUnit.Medium;
            FpSpread.Columns[1].Locked = true;
            FpSpread.Columns[1].Width = 150;

            FpSpread.Sheets[0].ColumnHeader.Cells[0, 2].Text = "Student Name";
            FpSpread.Sheets[0].ColumnHeader.Cells[0, 2].Font.Bold = true;
            FpSpread.Sheets[0].ColumnHeader.Cells[0, 2].HorizontalAlign = HorizontalAlign.Center;
            FpSpread.Sheets[0].ColumnHeader.Cells[0, 2].Font.Name = "Book Antiqua";
            FpSpread.Sheets[0].ColumnHeader.Cells[0, 2].Font.Size = FontUnit.Medium;
            FpSpread.Columns[2].Locked = true;
            FpSpread.Columns[2].Width = 300;

            FpSpread.Sheets[0].ColumnHeader.Cells[0, 3].Text = "Device ID";
            FpSpread.Sheets[0].ColumnHeader.Cells[0, 3].Font.Bold = true;
            FpSpread.Sheets[0].ColumnHeader.Cells[0, 3].HorizontalAlign = HorizontalAlign.Center;
            FpSpread.Sheets[0].ColumnHeader.Cells[0, 3].Font.Name = "Book Antiqua";
            FpSpread.Sheets[0].ColumnHeader.Cells[0, 3].Font.Size = FontUnit.Medium;
            FpSpread.Columns[3].Locked = true;
            FpSpread.Columns[3].Width = 150;

            FpSpread.Sheets[0].ColumnHeader.Cells[0, 4].Text = "Finger ID";
            FpSpread.Sheets[0].ColumnHeader.Cells[0, 4].Font.Bold = true;
            FpSpread.Sheets[0].ColumnHeader.Cells[0, 4].HorizontalAlign = HorizontalAlign.Center;
            FpSpread.Sheets[0].ColumnHeader.Cells[0, 4].Font.Name = "Book Antiqua";
            FpSpread.Sheets[0].ColumnHeader.Cells[0, 4].Font.Size = FontUnit.Medium;
            FpSpread.Columns[4].Locked = true;
            FpSpread.Columns[4].Width = 150;
        }
        catch { }
    }

    protected void btnmatch_click(object sender, EventArgs e)
    {
        try
        {
            string getnamecode = Convert.ToString(ddlstdlst.SelectedItem.Text);
            string staffname = "";
            if (getnamecode.Trim() != "Select")
                staffname = getnamecode.Split('$')[0];
            if ((FpSpread.Sheets[0].RowCount == 3 && checkvalue() == false) || FpSpread.Sheets[0].RowCount == 0)
                loadsprcolumns();
            FarPoint.Web.Spread.TextCellType txtcell = new FarPoint.Web.Spread.TextCellType();
            if (checkstdcode() == false)
            {
                lblerr.Visible = true;
                lblerr.Text = "Student Already Exists!";
                return;
            }
            else if (ddlstdlst.SelectedIndex == 0)
            {
                lblerr.Visible = true;
                lblerr.Text = "Please Select Student!";
                return;
            }
            else if (txt_macid.Text.Trim() == "")
            {
                lblerr.Visible = true;
                lblerr.Text = "Please Enter MachineID!";
                return;
            }
            else if (ddlfingerid.SelectedIndex == 0)
            {
                lblerr.Visible = true;
                lblerr.Text = "Please Select FingerID!";
                return;
            }
            else
            {
                FpSpread.Sheets[0].RowCount++;
                FpSpread.Sheets[0].Cells[FpSpread.Sheets[0].RowCount - 1, 0].Text = Convert.ToString(FpSpread.Sheets[0].RowCount);
                FpSpread.Sheets[0].Cells[FpSpread.Sheets[0].RowCount - 1, 0].CellType = txtcell;
                FpSpread.Sheets[0].Cells[FpSpread.Sheets[0].RowCount - 1, 0].HorizontalAlign = HorizontalAlign.Center;
                FpSpread.Sheets[0].Cells[FpSpread.Sheets[0].RowCount - 1, 0].Font.Name = "Book Antiqua";
                FpSpread.Sheets[0].Cells[FpSpread.Sheets[0].RowCount - 1, 0].Font.Size = FontUnit.Medium;

                FpSpread.Sheets[0].Cells[FpSpread.Sheets[0].RowCount - 1, 1].Text = Convert.ToString(ddlstdlst.SelectedValue);
                FpSpread.Sheets[0].Cells[FpSpread.Sheets[0].RowCount - 1, 1].CellType = txtcell;
                FpSpread.Sheets[0].Cells[FpSpread.Sheets[0].RowCount - 1, 1].HorizontalAlign = HorizontalAlign.Center;
                FpSpread.Sheets[0].Cells[FpSpread.Sheets[0].RowCount - 1, 1].Font.Name = "Book Antiqua";
                FpSpread.Sheets[0].Cells[FpSpread.Sheets[0].RowCount - 1, 1].Font.Size = FontUnit.Medium;

                FpSpread.Sheets[0].Cells[FpSpread.Sheets[0].RowCount - 1, 2].Text = Convert.ToString(staffname);
                FpSpread.Sheets[0].Cells[FpSpread.Sheets[0].RowCount - 1, 2].CellType = txtcell;
                FpSpread.Sheets[0].Cells[FpSpread.Sheets[0].RowCount - 1, 2].HorizontalAlign = HorizontalAlign.Left;
                FpSpread.Sheets[0].Cells[FpSpread.Sheets[0].RowCount - 1, 2].Font.Name = "Book Antiqua";
                FpSpread.Sheets[0].Cells[FpSpread.Sheets[0].RowCount - 1, 2].Font.Size = FontUnit.Medium;

                FpSpread.Sheets[0].Cells[FpSpread.Sheets[0].RowCount - 1, 3].Text = Convert.ToString(txt_macid.Text);
                FpSpread.Sheets[0].Cells[FpSpread.Sheets[0].RowCount - 1, 3].CellType = txtcell;
                FpSpread.Sheets[0].Cells[FpSpread.Sheets[0].RowCount - 1, 3].HorizontalAlign = HorizontalAlign.Center;
                FpSpread.Sheets[0].Cells[FpSpread.Sheets[0].RowCount - 1, 3].Font.Name = "Book Antiqua";
                FpSpread.Sheets[0].Cells[FpSpread.Sheets[0].RowCount - 1, 3].Font.Size = FontUnit.Medium;

                FpSpread.Sheets[0].Cells[FpSpread.Sheets[0].RowCount - 1, 4].Text = Convert.ToString(ddlfingerid.SelectedValue);
                FpSpread.Sheets[0].Cells[FpSpread.Sheets[0].RowCount - 1, 4].CellType = txtcell;
                FpSpread.Sheets[0].Cells[FpSpread.Sheets[0].RowCount - 1, 4].HorizontalAlign = HorizontalAlign.Center;
                FpSpread.Sheets[0].Cells[FpSpread.Sheets[0].RowCount - 1, 4].Font.Name = "Book Antiqua";
                FpSpread.Sheets[0].Cells[FpSpread.Sheets[0].RowCount - 1, 4].Font.Size = FontUnit.Medium;

                FpSpread.Sheets[0].PageSize = FpSpread.Sheets[0].RowCount;
                FpSpread.Visible = true;
                lblerr.Visible = false;
                btnsave.Visible = true;
            }
        }
        catch { }
    }


    protected void btnsave_click(object sender, EventArgs e)
    {
        try
        {
            if (checkvalue() == true && FpSpread.Sheets[0].RowCount > 0)
            {
                string rollno = "";
                string fingerid = "";
                string deviceid = "";
                string collcode = Convert.ToString(ddlcoll.SelectedItem.Value);
                string updq = "";
                int upcount = 0;
                for (int ik = 0; ik < FpSpread.Sheets[0].RowCount; ik++)
                {
                    rollno = Convert.ToString(FpSpread.Sheets[0].Cells[ik, 1].Text);
                    fingerid = Convert.ToString(FpSpread.Sheets[0].Cells[ik, 4].Text);
                    deviceid = Convert.ToString(FpSpread.Sheets[0].Cells[ik, 3].Text);
                    updq = "update Registration set finger_id='" + fingerid + "',DeviceID='" + deviceid + "' where Roll_No='" + rollno + "'";
                    int inscount = d2.update_method_wo_parameter(updq, "Text");
                    if (inscount > 0)
                        upcount++;
                }
                if (upcount > 0)
                {
                    alertpopwindow.Visible = true;
                    lblalerterr.Text = "Saved Successfully!";
                }
            }
        }
        catch { }
    }

    protected void btnexit_click(object sender, EventArgs e)
    {
        poperrjs.Visible = false;
    }

    #endregion

    protected void imagebtnpopcloseadd_Click(object sender, EventArgs e)
    {
        poperrjs.Visible = false;
    }

    protected void btnerrclose_Click(object sender, EventArgs e)
    {
        alertpopwindow.Visible = false;
    }

}