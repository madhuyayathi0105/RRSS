using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Configuration;
using System.Drawing;
using System.Web.Services;

public partial class LeaveMaster_Alter : System.Web.UI.Page
{
    bool cellclick = false;
    string usercode = string.Empty;
    string collegecode1 = string.Empty;
    string collegecode = string.Empty;
    string singleuser = string.Empty;
    string group_user = string.Empty;
    static string autocol = string.Empty;

    string selectQuery = "";
    DAccess2 d2 = new DAccess2();
    DataSet ds = new DataSet();

    DAccess2 queryObject = new DAccess2();
    DAccess2 da = new DAccess2();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["collegecode"] == null)
        {
            Response.Redirect("~/Default.aspx");
        }
        usercode = Session["usercode"].ToString();
        //collegecode1 = Session["collegecode"].ToString();
        singleuser = Session["single_user"].ToString();
        group_user = Session["group_code"].ToString();
        collegecode1 = Session["collegecode"].ToString();

        if (!IsPostBack)
        {
            hide();
            bindclg();
            bindLeaveReasonMapping();
            if (ddl_college.Items.Count > 0)
            {
                collegecode1 = Convert.ToString(ddl_college.SelectedItem.Value);
            }
            if (ddl_popclg.Items.Count > 0)
            {
                collegecode = Convert.ToString(ddl_popclg.SelectedItem.Value);
                autocol = Convert.ToString(ddl_popclg.SelectedItem.Value);
            }
            btn_go_Click(sender, e);
        }
        if (ddl_college.Items.Count > 0)
        {
            collegecode1 = Convert.ToString(ddl_college.SelectedItem.Value);
        }
        if (ddl_popclg.Items.Count > 0)
        {
            collegecode = Convert.ToString(ddl_popclg.SelectedItem.Value);
            autocol = Convert.ToString(ddl_popclg.SelectedItem.Value);
        }
        lbl_validation.Visible = false;
    }

    protected void lb3_Click(object sender, EventArgs e)
    {
        Session.Abandon();
        Session.Clear();
        Session.RemoveAll();
        System.Web.Security.FormsAuthentication.SignOut();
        Response.Redirect("~/Default.aspx", false);
    }

    protected void ddl_college_OnSelectedIndexChanged(object sender, EventArgs e)
    {
        collegecode1 = Convert.ToString(ddl_college.SelectedItem.Value);
        btn_go_Click(sender, e);
    }

    protected void ddl_popclg_Change(object sender, EventArgs e)
    {
        collegecode = Convert.ToString(ddl_popclg.SelectedItem.Value);
        autocol = Convert.ToString(ddl_popclg.SelectedItem.Value);
        string cat_name = Convert.ToString(txt_leavename.Text);
        string acronym = Convert.ToString(txt_shrtfm.Text);

        string catname = d2.GetFunction("select distinct category from leave_category where category='" + cat_name + "' and college_code='" + collegecode + "'");
        if (catname.Trim() != "" && catname.Trim() != "0")
        {
            imgdiv2.Visible = true;
            lbl_alert.Visible = true;
            lbl_alert.Text = "Leave Name Already Exist!";
            txt_leavename.Text = "";
        }
        string acronym1 = d2.GetFunction("select distinct shortname from leave_category where shortname='" + acronym + "' and college_code='" + collegecode + "'");
        if (acronym1.Trim() != "" && acronym1.Trim() != "0")
        {
            imgdiv2.Visible = true;
            lbl_alert.Visible = true;
            lbl_alert.Text = "Leave Acronym Already Exist!";
            txt_shrtfm.Text = "";
        }
    }

    public void bindclg()
    {
        try
        {
            ds.Clear();
            ddl_college.Items.Clear();
            ddl_popclg.Items.Clear();

            selectQuery = "select cp.college_code,cf.collname from collegeprivilages cp,collinfo cf where user_code=" + Session["usercode"] + " and cp.college_code=cf.college_code";
            ds = d2.select_method_wo_parameter(selectQuery, "Text");
            if (ds.Tables[0].Rows.Count > 0)
            {
                ddl_college.DataSource = ds;
                ddl_college.DataTextField = "collname";
                ddl_college.DataValueField = "college_code";
                ddl_college.DataBind();

                ddl_popclg.DataSource = ds;
                ddl_popclg.DataTextField = "collname";
                ddl_popclg.DataValueField = "college_code";
                ddl_popclg.DataBind();
            }
        }
        catch (Exception ex) { }
    }

    protected void btn_addnew_Click(object sender, EventArgs e)
    {
        ddl_popclg.SelectedIndex = ddl_popclg.Items.IndexOf(ddl_popclg.Items.FindByValue(ddl_college.SelectedItem.Value));
        collegecode = Convert.ToString(ddl_popclg.SelectedItem.Value);
        autocol = Convert.ToString(ddl_popclg.SelectedItem.Value);
        ddl_popclg.Enabled = true;
        addnew.Visible = true;
        rb_earn.Checked = false;
        rb_tpres.Checked = false;
        rb_gnrl.Checked = false;
        txt_leavename.Enabled = true;
        txt_shrtfm.Enabled = true;
        txt_leavename.Text = "";
        txt_shrtfm.Text = "";
        btn_save.Visible = true;
        btndel.Visible = false;
        btn_save.Text = "Save";
    }

    protected void imagebtnpopclose1_Click(object sender, EventArgs e)
    {
        addnew.Visible = false;
    }

    protected void btn_go_Click(object sender, EventArgs e)
    {
        try
        {
            hide();

            FpSpread1.Sheets[0].RowCount = 0;
            FpSpread1.Sheets[0].ColumnCount = 0;
            FpSpread1.CommandBar.Visible = false;
            FpSpread1.Sheets[0].AutoPostBack = true;
            FpSpread1.Height = 1000;
            FpSpread1.Width = 600;
            FpSpread1.Sheets[0].ColumnHeader.RowCount = 1;
            FpSpread1.Sheets[0].RowHeader.Visible = false;
            FpSpread1.Sheets[0].ColumnCount = 8;
            FarPoint.Web.Spread.StyleInfo darkstyle = new FarPoint.Web.Spread.StyleInfo();
            darkstyle.BackColor = ColorTranslator.FromHtml("#0CA6CA");
            darkstyle.ForeColor = Color.White;
            FpSpread1.ActiveSheetView.ColumnHeader.DefaultStyle = darkstyle;
            FpSpread1.Visible = true;
            FarPoint.Web.Spread.CheckBoxCellType chkall = new FarPoint.Web.Spread.CheckBoxCellType();
            FarPoint.Web.Spread.CheckBoxCellType chk = new FarPoint.Web.Spread.CheckBoxCellType();
            chkall.AutoPostBack = false;

            string selqry = "select category,shortname,status,LeaveMasterPK from leave_category where college_Code='" + collegecode1 + "'";
            ds.Clear();
            ds = d2.select_method_wo_parameter(selqry, "text");

            if (ds.Tables[0].Rows.Count > 0)
            {
                FpSpread1.Sheets[0].Rows.Count = ds.Tables[0].Rows.Count;
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    FpSpread1.Sheets[0].Cells[i, 3].CellType = chk;
                    FpSpread1.Sheets[0].Cells[i, 4].CellType = chk;
                    FpSpread1.Sheets[0].Cells[i, 5].CellType = chk;
                    FpSpread1.Sheets[0].Cells[i, 6].CellType = chk;
                    FpSpread1.Sheets[0].Cells[i, 3].Locked = true;
                    FpSpread1.Sheets[0].Cells[i, 4].Locked = true;
                    FpSpread1.Sheets[0].Cells[i, 5].Locked = true;
                    FpSpread1.Sheets[0].Cells[i, 6].Locked = true;

                    FpSpread1.Sheets[0].Cells[i, 0].Text = Convert.ToString(i + 1);
                    FpSpread1.Sheets[0].Cells[i, 0].Font.Name = "Book Antiqua";
                    FpSpread1.Sheets[0].Cells[i, 1].Text = ds.Tables[0].Rows[i]["category"].ToString();
                    FpSpread1.Sheets[0].Cells[i, 1].Font.Name = "Book Antiqua";
                    FpSpread1.Sheets[0].Cells[i, 2].Text = ds.Tables[0].Rows[i]["shortname"].ToString();
                    FpSpread1.Sheets[0].Cells[i, 2].Font.Name = "Book Antiqua";
                    FpSpread1.Sheets[0].Cells[i, 7].Value = ds.Tables[0].Rows[i]["LeaveMasterPK"].ToString();
                    FpSpread1.Sheets[0].Cells[i, 7].Font.Name = "Book Antiqua";

                    FpSpread1.Sheets[0].Cells[i, 3].Font.Name = "Book Antiqua";
                    FpSpread1.Sheets[0].Cells[i, 4].Font.Name = "Book Antiqua";
                    FpSpread1.Sheets[0].Cells[i, 5].Font.Name = "Book Antiqua";
                    string statusss = ds.Tables[0].Rows[i]["status"].ToString();
                    if (statusss.Trim() == "2")
                    {
                        FpSpread1.Sheets[0].Cells[i, 3].Value = 1;
                    }
                    if (statusss.Trim() == "0")
                    {
                        FpSpread1.Sheets[0].Cells[i, 4].Value = 1;
                    }
                    if (statusss.Trim() == "1")
                    {
                        FpSpread1.Sheets[0].Cells[i, 5].Value = 1;
                    }
                }
                FpSpread1.Sheets[0].ColumnHeader.Columns[0].Width = 60;
                FpSpread1.Sheets[0].ColumnHeader.Columns[1].Width = 244;
                FpSpread1.Sheets[0].ColumnHeader.Columns[2].Width = 150;
                FpSpread1.Sheets[0].ColumnHeader.Columns[3].Width = 102;
                FpSpread1.Sheets[0].ColumnHeader.Columns[4].Width = 102;
                FpSpread1.Sheets[0].ColumnHeader.Columns[5].Width = 102;
                FpSpread1.Sheets[0].ColumnHeader.Columns[6].Width = 102;

                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 0].Text = "S.No";
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 0].Font.Bold = true;
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 0].Font.Name = "Book Antiqua";
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 1].Text = "Leave Name";
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 1].Font.Bold = true;
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 1].Font.Name = "Book Antiqua";
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 2].Text = "Short Form";
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 2].Font.Bold = true;
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 2].Font.Name = "Book Antiqua";
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 3].Text = "Earn Leave";
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 3].Font.Bold = true;
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 3].Font.Name = "Book Antiqua";
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 4].Text = "Treated As Present";
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 4].Font.Bold = true;
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 4].Font.Name = "Book Antiqua";
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 5].Text = "Treated As LOP";
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 5].Font.Bold = true;
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 5].Font.Name = "Book Antiqua";
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 6].Text = "Select";
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 6].Font.Bold = true;
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 6].Font.Name = "Book Antiqua";
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 7].Text = "Id";
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 7].Font.Bold = true;
                FpSpread1.Sheets[0].ColumnHeader.Cells[0, 7].Font.Name = "Book Antiqua";
                FpSpread1.Sheets[0].Columns[7].Visible = false;
                FpSpread1.Sheets[0].Columns[6].Visible = false;
                for (int i = 0; i < FpSpread1.Sheets[0].Columns.Count; i++)
                {
                    FpSpread1.Sheets[0].ColumnHeader.Columns[i].HorizontalAlign = HorizontalAlign.Center;
                    FpSpread1.Sheets[0].ColumnHeader.Columns[i].Font.Name = "Book Antiqua";
                    FpSpread1.Sheets[0].ColumnHeader.Columns[i].Font.Bold = true;
                    FpSpread1.Sheets[0].ColumnHeader.Columns[i].Font.Size = FontUnit.Medium;
                    FpSpread1.Sheets[0].Columns[i].HorizontalAlign = HorizontalAlign.Center;

                }
                FpSpread1.Sheets[0].Columns[1].HorizontalAlign = HorizontalAlign.Left;
                FpSpread1.Sheets[0].Columns[2].HorizontalAlign = HorizontalAlign.Left;

                FpSpread1.SaveChanges();
                FpSpread1.Sheets[0].PageSize = ds.Tables[0].Rows.Count;
                FpSpread1.Visible = true;
                addnew.Visible = false;
                div1.Visible = true;
                rptprint.Visible = true;
                lbl_error.Visible = false;
            }
            else
            {
                lbl_error.Visible = true;
                lbl_error.Text = "No Records Found";
            }
        }
        catch (Exception ex)
        {
            d2.sendErrorMail(ex, collegecode1, "LeaveMaster_Alter.aspx");
        }
    }

    protected void FpSpread1_OnCellClick(object sender, EventArgs e)
    {
        string activerow = FpSpread1.ActiveSheetView.ActiveRow.ToString();
        string activecol = FpSpread1.ActiveSheetView.ActiveColumn.ToString();
        cellclick = true;
        addnew.Visible = true;
    }

    protected void FpSpread1_Selectedindexchange(object sender, EventArgs e)
    {
        try
        {
            FpSpread1.SaveChanges();
            if (cellclick == true)
            {
                rb_earn.Checked = false;
                rb_tpres.Checked = false;
                rb_gnrl.Checked = false;

                ddl_popclg.SelectedIndex = ddl_popclg.Items.IndexOf(ddl_popclg.Items.FindByValue(ddl_college.SelectedItem.Value));
                ddl_popclg.Enabled = false;
                btn_save.Text = "Update";
                btndel.Visible = true;
                string activerow = FpSpread1.ActiveSheetView.ActiveRow.ToString();
                string activecol = FpSpread1.ActiveSheetView.ActiveColumn.ToString();
                string leavename = FpSpread1.Sheets[0].Cells[Convert.ToInt32(activerow), 1].Text.ToString();
                txt_leavename.Text = leavename;
                string shortform = FpSpread1.Sheets[0].Cells[Convert.ToInt32(activerow), 2].Text.ToString();
                txt_shrtfm.Text = shortform;
                string lblid = FpSpread1.Sheets[0].Cells[Convert.ToInt32(activerow), 7].Text.ToString();
                oldlname.Text = lblid;
                txt_leavename.Enabled = false;
                txt_shrtfm.Enabled = false;

                for (int i = 0; i < FpSpread1.Sheets[0].Rows.Count; i++)
                {
                    if (Convert.ToInt32(FpSpread1.Sheets[0].Cells[Convert.ToSByte(activerow), 3].Value) == 1)
                    {
                        rb_earn.Checked = true;
                        rb_tpres.Checked = false;
                        rb_gnrl.Checked = false;
                    }
                    if (Convert.ToInt32(FpSpread1.Sheets[0].Cells[Convert.ToSByte(activerow), 4].Value) == 1)
                    {
                        rb_earn.Checked = false;
                        rb_tpres.Checked = true;
                        rb_gnrl.Checked = false;
                    }
                    if (Convert.ToInt32(FpSpread1.Sheets[0].Cells[Convert.ToSByte(activerow), 5].Value) == 1)
                    {
                        rb_earn.Checked = false;
                        rb_tpres.Checked = false;
                        rb_gnrl.Checked = true;
                    }
                }
            }
        }
        catch { }
    }

    protected void btn_save_Click(object sender, EventArgs e)
    {
        string collcode = "";
        try
        {
            string category = txt_leavename.Text;
            string shortname = (txt_shrtfm.Text).Trim().ToUpper();
            string leave = "";
            string lblid = Convert.ToString(oldlname.Text.ToString());

            if (rb_earn.Checked == true)
            {
                leave = "2";
            }
            else if (rb_tpres.Checked == true)
            {
                leave = "0";
            }
            else if (rb_gnrl.Checked == true)
            {
                leave = "1";
            }
            if (leave.Trim() != "")
            {
                if (btn_save.Text.Trim().ToLower() == "save")
                {
                    collcode = collegecode;
                    string sql = "insert into leave_category (category,shortname,status,college_code) values ('" + category + "','" + shortname + "','" + leave + "','" + collegecode + "')";
                    ds = d2.select_method_wo_parameter(sql, "Text");
                    imgdiv2.Visible = true;
                    lbl_alert.Visible = true;
                    lbl_alert.Text = "Saved Successfully";
                }
                else if (btn_save.Text.Trim().ToLower() == "update")
                {
                    collcode = collegecode1;
                    string sql = "update  leave_category set category='" + category + "',shortname='" + shortname + "',status='" + leave + "',college_code='" + collegecode1 + "' where LeaveMasterPK='" + lblid + "'";
                    ds = d2.select_method_wo_parameter(sql, "Text");
                    imgdiv2.Visible = true;
                    lbl_alert.Visible = true;
                    lbl_alert.Text = "Updated Successfully";
                }
            }
            else
            {
                imgdiv2.Visible = true;
                lbl_alert.Visible = true;
                lbl_alert.Text = "Please Select Any One Leave Type!";
                return;
            }

            addnew.Visible = false;
            btn_go_Click(sender, e);
            div1.Visible = true;
            rptprint.Visible = true;
            FpSpread1.Visible = true;
        }
        catch (Exception ex)
        {
            d2.sendErrorMail(ex, collcode, "LeaveMaster_Alter.aspx");
        }
    }

    protected void btn_exit_Click(object sender, EventArgs e)
    {
        addnew.Visible = false;
    }

    protected void btn_printmaster_Click(object sender, EventArgs e)
    {
        try
        {
            string Leavemaster = "Leavemaster Report";
            string pagename = "LeaveMaster_Alter.aspx";
            Printcontrol.loadspreaddetails(FpSpread1, pagename, Leavemaster);
            Printcontrol.Visible = true;
        }
        catch { }
    }

    protected void btn_excel_Click(object sender, EventArgs e)
    {
        try
        {
            string reportname = txt_excelname.Text;
            if (reportname.ToString().Trim() != "")
            {
                d2.printexcelreport(FpSpread1, reportname);
                lbl_validation.Visible = false;
            }
            else
            {
                lbl_validation.Text = "Please Enter Your Report Name";
                lbl_validation.Visible = true;
                txt_excelname.Focus();
            }
        }
        catch { }
    }

    public void btn_errorclose_Click(object sender, EventArgs e)
    {
        imgdiv2.Visible = false;
    }

    public void hide()
    {
        lbl_validation.Visible = false;
        Printcontrol.Visible = false;
        div1.Visible = false;
        rptprint.Visible = false;
    }

    public void btndel_Click(object sender, EventArgs e)
    {
        try
        {
            int savecc = 0;
            FpSpread1.SaveChanges();

            string activerow = FpSpread1.ActiveSheetView.ActiveRow.ToString();
            string sql = "delete  from leave_category where  LeaveMasterPK = '" + FpSpread1.Sheets[0].Cells[Convert.ToInt32(activerow), 7].Text.ToString() + "' and college_code= '" + collegecode + "'";
            int qry = d2.update_method_wo_parameter(sql, "Text");
            savecc++;
            if (savecc > 0)
            {
                lbl_alert.Text = "Deleted Successfully";
                lbl_alert.Visible = true;
                imgdiv2.Visible = true;
                btn_go_Click(sender, e);
            }
        }
        catch (Exception ex)
        {
            d2.sendErrorMail(ex, collegecode, "LeaveMaster_Alter.aspx");
        }
    }

    [WebMethod]
    public static string checkCatName(string CatName)
    {
        string returnValue = "1";
        try
        {
            DAccess2 dd = new DAccess2();
            string cat_name = CatName;
            if (cat_name.Trim() != "" && cat_name != null)
            {
                string querycatname = dd.GetFunction("select distinct shortname,category from leave_category where category='" + cat_name + "' and college_code='" + autocol + "'");
                if (querycatname.Trim() == "" || querycatname == null || querycatname == "0" || querycatname == "-1")
                {
                    returnValue = "0";
                }
            }
            else
            {
                returnValue = "2";
            }
        }
        catch (SqlException ex)
        {
            returnValue = "error" + ex.ToString();
        }
        return returnValue;
    }

    [WebMethod]
    public static string checkCatAcr(string CatAcr)
    {
        string returnValue = "1";
        try
        {
            DAccess2 dd = new DAccess2();
            string cat_acr = CatAcr;
            if (cat_acr.Trim() != "" && cat_acr != null)
            {
                string querycatacr = dd.GetFunction("select distinct shortname,category from leave_category where shortname='" + cat_acr + "' and college_Code='" + autocol + "'");
                if (querycatacr.Trim() == "" || querycatacr == null || querycatacr == "0" || querycatacr == "-1")
                {
                    returnValue = "0";
                }
            }
            else
            {
                returnValue = "2";
            }
        }
        catch (SqlException ex)
        {
            returnValue = "error" + ex.ToString();
        }
        return returnValue;
    }
    protected void btn_streamplus_OnClick(object sender, EventArgs e)
    {
        Plusapt.Visible = true;
        btn_plusAdd.Visible = true;
        txt_addstream.Text = "";
    }
    protected void btn_streamminus_OnClick(object sender, EventArgs e)
    {
        string LeaveMapping = Convert.ToString(ddl_leave.SelectedItem);
      

        string query = "delete from TextValTable where TextVal='" + LeaveMapping + "' and college_code='" + collegecode1 + "'";
        int count = d2.update_method_wo_parameter(query, "Text");
        bindLeaveReasonMapping();

    }
    protected void btn_plusAdd_OnClick(object sender, EventArgs e)
    {
        string stream = txt_addstream.Text;
        string criteria = "LveMp";
        if (stream.Trim() != "")
        {
                string LeaveTypeM = Convert.ToString(ddlleavemapping.SelectedItem.Text);
                string query = "insert into TextValTable(TextVal,TextCriteria,college_code )values ('" + stream + "','" + criteria + "','" + collegecode1 + "')";
                int count = d2.update_method_wo_parameter(query, "Text");

                bindLeaveReasonMapping();
                if (count > 0)
                {
                    ScriptManager.RegisterStartupScript(base.Page, this.GetType(), ("dialogJavascript" + this.ID), "alert(\"Saved Successfully\");", true);
                
                }
             
            
           
        }
    }
    protected void btn_Plusexit_OnClick(object sender, EventArgs e)
    {
        Plusapt.Visible = false;
        btn_plusAdd.Visible = false;
    }
    protected void ddl_leavereason_click(object sender, EventArgs e)
    {
        if (ddlleavemapping.Text != "Select")
        {
            btn_streamplus.Visible = true;
            ddl_leave.Visible = true;
            btn_streamminus.Visible = true;
            btn_saveval.Visible = true;


        }
        else
        {
            btn_streamplus.Visible = false;
            ddl_leave.Visible = false;
            btn_streamminus.Visible = false;
            btn_saveval.Visible = false;
        
        }
    
    }
    protected void bindLeaveReasonMapping()
    {
        ds.Clear();
        ddl_leave.Items.Clear();
        string item = "select distinct TextVal,TextCode from TextValTable where TextCriteria='LveMp' ";
        ds = d2.select_method_wo_parameter(item, "Text");
        if (ds.Tables[0].Rows.Count > 0)
        {
            ddl_leave.DataSource = ds;
            ddl_leave.DataTextField = "TextVal";
            ddl_leave.DataValueField = "TextCode";
            ddl_leave.DataBind();
            ddl_leave.Items.Insert(0, "Select");
        }
        else
        {
            ddl_leave.Items.Insert(0, "Select");
        }
    }
    protected void btn_Save_OnClick(object sender, EventArgs e)
    {
        string criteria = "ReaMp";
        if (ddlleavemapping.Text != "Select" && ddl_leave.Text != "Select")
        {
            string type = Convert.ToString(ddlleavemapping.SelectedItem.Text);
            string reason = Convert.ToString(ddl_leave.SelectedItem.Text);
            reason = reason + "-" + type;

            string sql = "if exists ( select * from TextValTable where TextVal ='" + reason + "' and TextCriteria ='ReaMp' and TextCriteria2='" + type + "' and college_code ='" + ddl_college.SelectedItem.Value + "') update TextValTable set TextVal ='" + reason + "' where TextCriteria ='ReaMp' and TextCriteria2='" + type + "' and college_code ='" + ddl_college.SelectedItem.Value + "' else insert into TextValTable (TextVal,TextCriteria,college_code,TextCriteria2) values ('" + reason + "','ReaMp','" + ddl_college.SelectedItem.Value + "','" + type + "')";
             int insert = d2.update_method_wo_parameter(sql, "TEXT");
             if (insert != 0)
             {
                 ScriptManager.RegisterStartupScript(base.Page, this.GetType(), ("dialogJavascript" + this.ID), "alert(\"Saved Successfully\");", true);
             }

        }
        else
        {
            ScriptManager.RegisterStartupScript(base.Page, this.GetType(), ("dialogJavascript" + this.ID), "alert(\"Please Select Leave Reason Mapping\");", true);
        }

    }
}