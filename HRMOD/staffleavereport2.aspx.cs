using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Data.SqlClient;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using FarPoint.Web.Spread;

public partial class staffleavereport2 : System.Web.UI.Page
{
    DAccess2 d2 = new DAccess2();
    DataSet ds = new DataSet();
    DataSet ds1 = new DataSet();
    DataSet ds2 = new DataSet();
    Boolean cellflag = false;

    Hashtable totalleave = new Hashtable();
    static DataSet staffdetails_ds = new DataSet();
    string strstaffcode = "";
    string q1 = "";
    string staffdept = "";
    int monthcnt;
    static int seatcnt = 0;
    static int bloodcnt = 0;
    string usercode = string.Empty;
    string collegecode1 = string.Empty;
    string singleuser = string.Empty;
    string group_user = string.Empty;
    string dateto;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["collegecode"] == null)
        {
            Response.Redirect("~/Default.aspx");
        }
        lblvalidation1.Visible = false;
        usercode = Session["usercode"].ToString();
        collegecode1 = Session["collegecode"].ToString();
        singleuser = Session["single_user"].ToString();
        group_user = Session["group_code"].ToString();
        if (Session["collegecode"] == null) //Aruna For Back Button
        {
            Response.Redirect("~/Default.aspx");
        }
        try
        {
            strstaffcode = "" + Session["Staff_Code"].ToString();

            if (staffdetails_ds.Tables.Count == 0)
            {
                q1 = "  SELECT M.Staff_Code,Staff_Name,p.Dept_Name,g.Desig_Name,Category_Name, P.dept_acronym,c.category_code,p.dept_code,ap.appl_id  FROM StaffMaster M,StaffTrans T,HrDept_Master P,Desig_Master G,StaffCategorizer C,staff_appl_master ap Where ap.appl_no=m.appl_no and m.staff_code = t.staff_code  AND T.Dept_Code = P.Dept_Code AND M.College_Code = P.College_Code AND T.Desig_Code = G.Desig_Code AND M.College_Code = G.CollegeCode AND T.Category_Code = C.Category_Code AND M.College_Code = C.College_Code AND M.College_Code ='" + collegecode1 + "' AND T.Latestrec = 1 AND ((M.Resign=0 AND M.Settled=0) and (M.Discontinue =0 or M.Discontinue is null)) ORDER BY p.Dept_Name,t.stftype desc,g.print_pri desc,g.priority";
                staffdetails_ds.Clear();
                staffdetails_ds = d2.select_method_wo_parameter(q1, "text");
            }
            if (!IsPostBack)
            {

                lblto.Visible = false;
                Txtentryto.Visible = false;
                lblda.Visible = false;
                Txtentryfrom.Visible = false;
                string fdate = d2.GetFunction("select top 1 convert(nvarchar(15),From_Date,103) as fdate from hrpaymonths where college_code='" + Session["collegecode"].ToString() + "' order by From_Date");
                string tdate = d2.GetFunction("select top 1 convert(nvarchar(15),To_Date,103) as tdate from hrpaymonths where college_code='" + Session["collegecode"].ToString() + "' order by To_Date desc");
                Txtentryfrom.Text = fdate;
                Txtentryto.Text = tdate;
                load_leavetype();
                load_year();
                FpSpreadvisiblefalse();
                if (strstaffcode == "")
                {
                    load_dept();
                    load_category();
                    load_staffname(staffdept);
                    tbseattype.Enabled = true;
                    tbblood.Enabled = true;
                    cbostaffname.Enabled = true;
                }
                else
                {
                    tbseattype.Enabled = false;
                    tbblood.Enabled = false;
                    cbostaffname.Enabled = false;
                    btngo_Click(sender, e);
                }
            }
        }
        catch (Exception ex)
        {
            lblnorec.Visible = true;
            lblnorec.Text = ex.ToString();
        }
    }
    void load_category()
    {
        try
        {
            cblcategory.Visible = true;
            cblcategory.Items.Clear();
            q1 = "select distinct category_code,category_name from staffcategorizer where college_code='" + Session["collegecode"].ToString() + "'";
            ds.Clear();
            ds = d2.select_method_wo_parameter(q1, "text");
            if (ds.Tables[0].Rows.Count > 0)
            {
                cblcategory.DataSource = ds.Tables[0];
                cblcategory.DataTextField = "category_name";
                cblcategory.DataValueField = "category_code";
                cblcategory.DataBind();
            }
            load_staffname(staffdept);
        }
        catch
        { }
    }
    void load_leavetype()
    {
        try
        {
            cblleavetype.Visible = true;
            ds.Clear();
            q1 = " Select category from leave_category where college_code='" + Session["collegecode"].ToString() + "'";
            ds = d2.select_method_wo_parameter(q1, "text");
            if (ds.Tables[0].Rows.Count > 0)
            {
                cblleavetype.DataSource = ds.Tables[0];
                cblleavetype.DataTextField = "category";
                cblleavetype.DataValueField = "category";
                cblleavetype.DataBind();
                cblleavetype.Items.Insert(0, "All");
                //cblleavetype.Items.Insert(1, "ABSENT");
                //cblleavetype.Items.Insert(1, "PERMISSION");
                //cblleavetype.Items.Insert(1, "LATE");
            }
        }
        catch { }
    }

    void load_staffname(string staffdept)
    {
        string derpatment = "";
        string staff_cat = "";
        cbostaffname.Items.Clear();
        string staffcat_selected = returnwithsinglecodevalue(cblcategory);
        if (staffdept != "")
        {
            derpatment = "and dept_code in ('" + staffdept + "')";
        }
        if (staffdept == "")
        {
            staffdept = returnwithsinglecodevalue(cbldepttype);
        }
        if (staffdept != "")
        {
            derpatment = "and dept_code in ('" + staffdept + "')";
        }
        if (staffcat_selected != "" && staffdept != "")
        {
            staff_cat = "and t.category_code in('" + staffcat_selected + "')";
        }
        ds.Clear();
        q1 = " Select distinct m.Staff_code,Staff_name from staffmaster m,stafftrans t where resign=0 and settled=0 and m.staff_code = t.staff_code " + staff_cat + " and t.latestrec = 1 " + derpatment + " and college_code='" + Session["collegecode"].ToString() + "' order by staff_name ";
        ds = d2.select_method_wo_parameter(q1, "text");
        if (ds.Tables[0].Rows.Count > 0)
        {
            cbostaffname.DataSource = ds;
            cbostaffname.DataTextField = "Staff_name";
            cbostaffname.DataValueField = "Staff_code";
            cbostaffname.DataBind();
            cbostaffname.Items.Insert(0, "All");
        }
    }
    public void load_year()
    {
        q1 = " select year(min(From_Date)) as startyear,year(max(To_Date)) as endyear from HrPayMonths where College_Code='" + Session["collegecode"].ToString() + "' and year(From_Date)<>'' and year(From_Date) is not null and Year(To_Date)<>'' and Year(To_Date) is not null";
        ds.Clear();
        ds = d2.select_method_wo_parameter(q1, "text");
        int end_year = 0;
        int startyear = 0;
        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        {
            int end_cnt = ds.Tables[0].Rows.Count;
            Int32.TryParse(ds.Tables[0].Rows[end_cnt - 1]["endyear"].ToString(), out end_year);
            Int32.TryParse(ds.Tables[0].Rows[end_cnt - 1]["startyear"].ToString(), out startyear);
            for (int i = startyear; i <= end_year; i++)
            {
                if (startyear <= end_year)
                {
                    ddlyear.Items.Add(Convert.ToString(startyear));
                    startyear++;
                }
            }
        }
        else
        {
            lblnorec.Visible = true;
            lblnorec.Text = "Please Update the HR Year!";
        }
    }
    void load_dept()
    {
        cbldepttype.Visible = true;
        cbldepttype.Items.Clear();
        ds.Clear();
        string deptquery = "";
        string singleuser = Session["single_user"].ToString();
        if (singleuser == "True")
        {
            deptquery = "SELECT DISTINCT hp.dept_code,dept_name from hr_privilege hp,hrdept_master hr  where user_code=" + Session["usercode"].ToString() + " and hr.dept_code=hp.dept_code  and hp.dept_code in (select dept_code from hrdept_master where college_code='" + Session["collegecode"].ToString() + "') order by dept_name";
        }
        else
        {
            group_user = Session["group_code"].ToString();
            if (group_user.Contains(';'))
            {
                string[] group_semi = group_user.Split(';');
                group_user = group_semi[0].ToString();
            }
            deptquery = "SELECT DISTINCT hp.dept_code,dept_name from hr_privilege hp,hrdept_master hr  where group_code='" + group_user + "' and hr.dept_code=hp.dept_code  and hp.dept_code in (select dept_code from hrdept_master where college_code='" + Session["collegecode"] + "') order by dept_name ";
        }
        if (deptquery != "")
        {
            ds = d2.select_method_wo_parameter(deptquery, "Text");
            if (ds.Tables[0].Rows.Count > 0)
            {
                cbldepttype.DataSource = ds.Tables[0];
                cbldepttype.DataTextField = "dept_name";
                cbldepttype.DataValueField = "dept_code";
                cbldepttype.DataBind();
            }
        }
    }
    protected void ddlyear_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            FpSpreadvisiblefalse();
        }
        catch (Exception ex)
        {
            lblnorec.Visible = true;
            lblnorec.Text = ex.ToString();
        }
    }
    protected void chkmonth_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            FpSpreadvisiblefalse();
            pmonth.Focus();
            int monthcount = 0;
            string value = "";
            string code = "";
            for (int i = 0; i < chkmonth.Items.Count; i++)
            {
                if (chkmonth.Items[i].Selected == true)
                {
                    value = chkmonth.Items[i].Text;
                    code = chkmonth.Items[i].Value.ToString();
                    monthcount = monthcount + 1;
                    txtmonth.Text = "Month(" + monthcount.ToString() + ")";
                }
            }
            if (monthcount == 0)
                txtmonth.Text = "---Select---";
            else
            {
            }
            monthcnt = monthcount;
        }
        catch (Exception ex)
        {
            lblnorec.Visible = true;
            lblnorec.Text = ex.ToString();
        }
    }
    protected string returnwithsinglecodevalue(CheckBoxList cb)
    {
        string empty = "";
        for (int i = 0; i < cb.Items.Count; i++)
        {
            if (cb.Items[i].Selected == true)
            {
                if (empty == "")
                {
                    empty = Convert.ToString(cb.Items[i].Value);
                }
                else
                {
                    empty = empty + "','" + Convert.ToString(cb.Items[i].Value);
                }
            }
        }
        return empty;
    }
    protected void Txtentryto_TextChanged(object sender, EventArgs e)
    {
        try
        {
            FpSpreadvisiblefalse();
        }
        catch (Exception ex)
        {
            lblnorec.Visible = true;
            lblnorec.Text = ex.ToString();
        }
    }
    protected void chkselect_CheckedChanged(object sender, EventArgs e)
    {
        try
        {
            FpSpreadvisiblefalse();
            CallCheckboxChange(chkselect, cbldepttype, tbseattype, lbldept.Text, "---Select---");
            staffdept = "";
            load_staffname(staffdept);
        }
        catch (Exception ex)
        {
            lblnorec.Visible = true;
            lblnorec.Text = ex.ToString();
        }
    }
    protected void cbldepttype_SelectedIndexChanged(object sender, EventArgs e)
    {
        //CallCheckboxListChange(chkselect, cbldepttype, tbseattype, lbldept.Text);
        try
        {
            pseattype.Focus();
            int seatcount = 0;
            string value = "";
            string code = "";
            FpSpreadvisiblefalse();
            for (int i = 0; i < cbldepttype.Items.Count; i++)
            {
                if (cbldepttype.Items[i].Selected == true)
                {
                    value = cbldepttype.Items[i].Text;
                    code = cbldepttype.Items[i].Value.ToString();
                    seatcount = seatcount + 1;
                    tbseattype.Text = "Department(" + seatcount.ToString() + ")";
                    if ((staffdept == ""))
                    {
                        staffdept = cbldepttype.Items[i].Value.ToString();
                    }
                    else
                    {
                        staffdept = staffdept + "," + cbldepttype.Items[i].Value.ToString();
                    }
                }
            }
            if (seatcount == 0)
                tbseattype.Text = "---Select---";
            else
            {
            }
            seatcnt = seatcount;
            load_staffname(staffdept);
        }
        catch (Exception ex)
        {
            lblnorec.Visible = true;
            lblnorec.Text = ex.ToString();
        }
    }
    protected void chkcategory_CheckedChanged(object sender, EventArgs e)
    {
        try
        {
            FpSpreadvisiblefalse();
            CallCheckboxChange(chkcategory, cblcategory, tbblood, "Category", "---Select---");
        }
        catch (Exception ex)
        {
            lblnorec.Visible = true;
            lblnorec.Text = ex.ToString();
        }
    }
    protected void cblcategory_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            int bloodcount = 0;
            string value = "";
            string code = "";
            FpSpreadvisiblefalse();
            for (int i = 0; i < cblcategory.Items.Count; i++)
            {
                if (cblcategory.Items[i].Selected == true)
                {
                    value = cblcategory.Items[i].Text;

                    code = cblcategory.Items[i].Value.ToString();
                    bloodcount = bloodcount + 1;
                    tbblood.Text = "Category(" + bloodcount.ToString() + ")";
                }
            }
            if (bloodcount == 0)
            {
                tbblood.Text = "---Select---";
            }
            else
            {
            }
            bloodcnt = bloodcount;
            load_staffname(staffdept);
        }
        catch (Exception ex)
        {
            lblnorec.Visible = true;
            lblnorec.Text = ex.ToString();
        }
    }
    public void FpSpreadvisiblefalse()
    {
        fpsalary.Visible = false;
        rptprint.Visible = false;
    }
    protected void cblleavetype_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            FpSpreadvisiblefalse();
        }
        catch (Exception ex)
        {
            lblnorec.Visible = true;
            lblnorec.Text = ex.ToString();
        }
    }
    protected void cbostaffname_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            if (cbostaffname.Items.Count == 0)
            {
                cbostaffname.Items.Insert(0, "---Select---");
            }
            FpSpreadvisiblefalse();
        }
        catch (Exception ex)
        {
            lblnorec.Visible = true;
            lblnorec.Text = ex.ToString();
        }
    }
    protected void rdoyearlywise_CheckedChanged(object sender, EventArgs e)
    {
        try
        {
            chkstaffleave.Checked = false;
            if (rdoyearlywise.Checked == true)
            {
                lblda.Text = "Date";
                Txtentryfrom.Visible = true;
                Txtentryfrom.Enabled = false;
                lblto.Visible = true;
                Txtentryto.Visible = true;
                txtmonth.Visible = false;
                chkmonth.Visible = false;
                FpSpreadvisiblefalse();
                pmonth.Visible = false;
                Label13.Visible = true;
                cblleavetype.Visible = true;
                lblyear.Visible = false;
                ddlyear.Visible = false;
                chkstaffleave.Visible = true;
                lblda.Visible = true;
                lblto.Text = "To";
                lblto.Visible = false;
                Txtentryto.Visible = false;
                lblda.Visible = false;
                Txtentryfrom.Visible = false;
                string fdate = d2.GetFunction("select top 1 convert(nvarchar(15),From_Date,103) as fdate from hrpaymonths where college_code='" + Session["collegecode"].ToString() + "' order by From_Date");
                string tdate = d2.GetFunction("select top 1 convert(nvarchar(15),To_Date,103) as tdate from hrpaymonths where college_code='" + Session["collegecode"].ToString() + "' order by To_Date desc");
                Txtentryfrom.Text = fdate;
                Txtentryto.Text = tdate;
            }
            else if (rdomonthlywise.Checked == true)
            {
                lblda.Text = "Month";
                Txtentryfrom.Visible = false;
                Txtentryto.Visible = false;
                chkstaffleave.Visible = true;
            }
            else
            {
                lblda.Visible = false;
                Txtentryfrom.Visible = false;
                lblto.Visible = true;
                Txtentryto.Visible = true;
                lblto.Text = "Date";
                txtmonth.Visible = false;
                chkmonth.Visible = false;
                pmonth.Visible = false;
            }
        }
        catch (Exception ex)
        {
            lblnorec.Visible = true;
            lblnorec.Text = ex.ToString();
        }
    }
    private void CallCheckboxListChange(CheckBox cb, CheckBoxList cbl, TextBox txt, string dipst)
    {
        try
        {
            int sel = 0;
            int count = 0;
            cb.Checked = false;
            for (sel = 0; sel < cbl.Items.Count; sel++)
            {
                if (cbl.Items[sel].Selected == true)
                {
                    count++;
                }
            }
            if (count > 0)
            {
                if (count == 1)
                {
                    txt.Text = dipst + "(" + count + ")";
                }
                else
                {
                    txt.Text = dipst + "(" + count + ")";
                }
                if (cbl.Items.Count == count)
                {
                    cb.Checked = true;
                }
            }
            else
            {
                txt.Text = "--Select--";
            }
        }
        catch { }
    }
    private void CallCheckboxChange(CheckBox cb, CheckBoxList cbl, TextBox txt, string dispst, string deft)
    {
        try
        {
            int sel = 0;
            txt.Text = deft;
            if (cb.Checked == true)
            {
                for (sel = 0; sel < cbl.Items.Count; sel++)
                {
                    cbl.Items[sel].Selected = true;
                }
                if (cbl.Items.Count == 1)
                {
                    txt.Text = dispst + "(" + cbl.Items.Count + ")";
                }
                else
                {
                    txt.Text = dispst + "(" + cbl.Items.Count + ")";
                }
            }
            else
            {
                for (sel = 0; sel < cbl.Items.Count; sel++)
                {
                    cbl.Items[sel].Selected = false;
                }
                txt.Text = deft;
            }
        }
        catch { }
    }
    protected void rdomonthlywise_CheckedChanged(object sender, EventArgs e)
    {
        try
        {
            FpSpreadvisiblefalse();
            chkstaffleave.Checked = false;
            if (rdomonthlywise.Checked == true)
            {
                lblda.Text = "Month";
                Txtentryfrom.Visible = false;
                Txtentryto.Visible = false;
                lblto.Visible = false;
                txtmonth.Visible = true;
                chkmonth.Visible = true;
                pmonth.Visible = true;
                Label13.Visible = true;
                cblleavetype.Visible = true;
                lblda.Visible = true;
                lblyear.Visible = true;
                ddlyear.Visible = true;
                chkstaffleave.Visible = true;
            }
            else if (rdoyearlywise.Checked == true)
            {
            }
            else
            {
                lblda.Text = "Date";
                Txtentryfrom.Visible = true;
                lblto.Visible = false;
                Txtentryfrom.Enabled = true;
                txtmonth.Visible = false;
                chkmonth.Visible = false;
                pmonth.Visible = false;
            }
        }
        catch (Exception ex)
        {
            lblnorec.Visible = true;
            lblnorec.Text = ex.ToString();
        }
    }
    protected void rdodaywise_CheckedChanged(object sender, EventArgs e)
    {
        FpSpreadvisiblefalse();
        chkstaffleave.Checked = false;
        if (rdoyearlywise.Checked == true)
        {
            lblda.Text = "Date";
            Txtentryfrom.Visible = true;
            Txtentryfrom.Enabled = true;
            lblto.Visible = true;
            Txtentryto.Visible = true;
            txtmonth.Visible = false;
            chkmonth.Visible = false;
            txtmonth.Visible = false;
            chkmonth.Visible = false;
            pmonth.Visible = false;
            Label13.Visible = true;
            cblleavetype.Visible = true;
            txtmonth.Visible = false;
        }
        else if (rdomonthlywise.Checked == true)
        {
            lblda.Text = "Month";
            Txtentryfrom.Visible = false;
            Txtentryto.Visible = false;
            Label13.Visible = true;
            cblleavetype.Visible = true;
        }
        else
        {
            lblda.Visible = false;
            Txtentryfrom.Visible = false;
            lblto.Visible = true;
            Txtentryto.Visible = true;
            lblto.Text = "Date";
            txtmonth.Visible = false;
            chkmonth.Visible = false;
            pmonth.Visible = false;
            Label13.Visible = true;
            cblleavetype.Visible = true;
            Label13.Visible = true;
            cblleavetype.Visible = true;
            lblyear.Visible = false;
            ddlyear.Visible = false;
            chkstaffleave.Visible = false;
        }
    }
    protected void btngo_Click(object sender, EventArgs e)
    {
        try
        {
            lblnorec.Visible = false;
            load_btnclick();
        }
        catch (Exception ex)
        {
            lblnorec.Visible = true;
            lblnorec.Text = ex.ToString();
            d2.sendErrorMail(ex, Convert.ToString(Session["collegecode"]), "staffleavereport.aspx");
        }
    }
    protected void load_btnclick()
    {
        try
        {
            Hashtable leavetypehash = new Hashtable();
            Hashtable totalbind = new Hashtable();
            Printcontrol.Visible = false;
            fpsalary.Sheets[0].RowCount = 0;
            fpsalary.Sheets[0].ColumnCount = 0;
            fpsalary.CommandBar.Visible = false;
            fpsalary.Sheets[0].AutoPostBack = true;
            fpsalary.Sheets[0].ColumnHeader.RowCount = 2;
            fpsalary.Sheets[0].RowHeader.Visible = false;
            fpsalary.Sheets[0].Columns.Count = 6;

            FarPoint.Web.Spread.StyleInfo darkstyle = new FarPoint.Web.Spread.StyleInfo();
            darkstyle.BackColor = ColorTranslator.FromHtml("#0CA6CA");
            darkstyle.ForeColor = Color.White;
            fpsalary.ActiveSheetView.ColumnHeader.DefaultStyle = darkstyle;

            fpsalary.Sheets[0].ColumnHeader.Cells[0, 0].Text = "S.No";
            fpsalary.Sheets[0].ColumnHeader.Cells[0, 1].Text = "Staff Name";
            fpsalary.Sheets[0].ColumnHeader.Cells[0, 2].Text = "Dept Acronym";
            fpsalary.Sheets[0].ColumnHeader.Cells[0, 3].Text = "Designation";
            fpsalary.Sheets[0].ColumnHeader.Cells[0, 4].Text = "Category Name";
            fpsalary.Sheets[0].ColumnHeader.Cells[0, 5].Text = "Month";

            fpsalary.Sheets[0].ColumnHeader.Cells[0, 0].Font.Bold = true;
            fpsalary.Sheets[0].ColumnHeader.Cells[0, 0].Font.Name = "Book Antiqua";
            fpsalary.Sheets[0].ColumnHeader.Cells[0, 0].Font.Size = FontUnit.Medium;
            fpsalary.Sheets[0].ColumnHeader.Cells[0, 0].HorizontalAlign = HorizontalAlign.Center;

            fpsalary.Sheets[0].ColumnHeader.Cells[0, 1].Font.Bold = true;
            fpsalary.Sheets[0].ColumnHeader.Cells[0, 1].Font.Name = "Book Antiqua";
            fpsalary.Sheets[0].ColumnHeader.Cells[0, 1].Font.Size = FontUnit.Medium;
            fpsalary.Sheets[0].ColumnHeader.Cells[0, 1].HorizontalAlign = HorizontalAlign.Center;

            fpsalary.Sheets[0].ColumnHeader.Cells[0, 2].Font.Bold = true;
            fpsalary.Sheets[0].ColumnHeader.Cells[0, 2].Font.Name = "Book Antiqua";
            fpsalary.Sheets[0].ColumnHeader.Cells[0, 2].Font.Size = FontUnit.Medium;
            fpsalary.Sheets[0].ColumnHeader.Cells[0, 2].HorizontalAlign = HorizontalAlign.Center;

            fpsalary.Sheets[0].ColumnHeader.Cells[0, 3].Font.Bold = true;
            fpsalary.Sheets[0].ColumnHeader.Cells[0, 3].Font.Name = "Book Antiqua";
            fpsalary.Sheets[0].ColumnHeader.Cells[0, 3].Font.Size = FontUnit.Medium;
            fpsalary.Sheets[0].ColumnHeader.Cells[0, 3].HorizontalAlign = HorizontalAlign.Center;

            fpsalary.Sheets[0].ColumnHeader.Cells[0, 4].Font.Bold = true;
            fpsalary.Sheets[0].ColumnHeader.Cells[0, 4].Font.Name = "Book Antiqua";
            fpsalary.Sheets[0].ColumnHeader.Cells[0, 4].Font.Size = FontUnit.Medium;
            fpsalary.Sheets[0].ColumnHeader.Cells[0, 4].HorizontalAlign = HorizontalAlign.Center;

            fpsalary.Sheets[0].ColumnHeader.Cells[0, 5].Font.Bold = true;
            fpsalary.Sheets[0].ColumnHeader.Cells[0, 5].Font.Name = "Book Antiqua";
            fpsalary.Sheets[0].ColumnHeader.Cells[0, 5].Font.Size = FontUnit.Medium;
            fpsalary.Sheets[0].ColumnHeader.Cells[0, 5].HorizontalAlign = HorizontalAlign.Center;

            fpsalary.Sheets[0].ColumnHeaderSpanModel.Add(0, 0, 2, 1);
            fpsalary.Sheets[0].ColumnHeaderSpanModel.Add(0, 1, 2, 1);
            fpsalary.Sheets[0].ColumnHeaderSpanModel.Add(0, 2, 2, 1);
            fpsalary.Sheets[0].ColumnHeaderSpanModel.Add(0, 3, 2, 1);
            fpsalary.Sheets[0].ColumnHeaderSpanModel.Add(0, 4, 2, 1);
            fpsalary.Sheets[0].ColumnHeaderSpanModel.Add(0, 5, 2, 1);

            string query = "";
            if (!String.IsNullOrEmpty(strstaffcode))
            {
                query = "select staff_code,leavetype,category_code,college_code,permission,MaxEarnLeave from individual_leave_type where college_code='" + collegecode1 + "' and staff_code='" + strstaffcode + "'";
                query += " Select category,shortname,LeaveMasterPK  from leave_category where college_code='" + collegecode1 + "' ";
                query += " select * from hrpaymonths where college_code='" + collegecode1 + "' and SelStatus='1'";
            }
            else
            {
                query = "select staff_code,leavetype,category_code,college_code,permission,MaxEarnLeave from individual_leave_type where college_code='" + collegecode1 + "'";
                query += " Select category,shortname,LeaveMasterPK  from leave_category where college_code='" + collegecode1 + "' ";
                query += " select * from hrpaymonths where college_code='" + collegecode1 + "' and SelStatus='1'"; //
            }
            ds2.Clear();
            ds2 = d2.select_method_wo_parameter(query, "Text");
            if (ds2.Tables.Count > 0 && ds2.Tables[2].Rows.Count > 0)
            {
                if (ds2.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds2.Tables[1].Rows.Count; i++)
                    {
                        fpsalary.Sheets[0].ColumnCount++;
                        fpsalary.Sheets[0].ColumnHeader.Cells[1, fpsalary.Sheets[0].ColumnCount - 1].Text = Convert.ToString(ds2.Tables[1].Rows[i]["shortname"]);
                        fpsalary.Sheets[0].ColumnHeader.Cells[1, fpsalary.Sheets[0].ColumnCount - 1].Tag = Convert.ToString(ds2.Tables[1].Rows[i]["LeaveMasterPK"]);
                        fpsalary.Sheets[0].ColumnHeader.Cells[1, fpsalary.Sheets[0].ColumnCount - 1].Font.Bold = true;
                        fpsalary.Sheets[0].ColumnHeader.Cells[1, fpsalary.Sheets[0].ColumnCount - 1].Font.Name = "Book Antiqua";
                        fpsalary.Sheets[0].ColumnHeader.Cells[1, fpsalary.Sheets[0].ColumnCount - 1].Font.Size = FontUnit.Medium;
                        fpsalary.Sheets[0].ColumnHeader.Cells[1, fpsalary.Sheets[0].ColumnCount - 1].HorizontalAlign = HorizontalAlign.Center;
                        if (!leavetypehash.Contains(Convert.ToString(ds2.Tables[1].Rows[i]["category"])))
                        {
                            int h = 6;
                            leavetypehash.Add(Convert.ToString(ds2.Tables[1].Rows[i]["category"]), h + i);
                        }
                        if (i == 0)
                        {
                            fpsalary.Sheets[0].ColumnHeader.Cells[0, fpsalary.Sheets[0].ColumnCount - 1].Text = "Leave Type";
                            fpsalary.Sheets[0].ColumnHeader.Cells[0, fpsalary.Sheets[0].ColumnCount - 1].Font.Bold = true;
                            fpsalary.Sheets[0].ColumnHeader.Cells[0, fpsalary.Sheets[0].ColumnCount - 1].Font.Name = "Book Antiqua";
                            fpsalary.Sheets[0].ColumnHeader.Cells[0, fpsalary.Sheets[0].ColumnCount - 1].Font.Size = FontUnit.Medium;
                            fpsalary.Sheets[0].ColumnHeader.Cells[0, fpsalary.Sheets[0].ColumnCount - 1].HorizontalAlign = HorizontalAlign.Center;
                        }
                    }
                    fpsalary.Sheets[0].ColumnHeaderSpanModel.Add(0, 6, 1, ds2.Tables[1].Rows.Count);
                }
                string deptcode = returnwithsinglecodevalue(cbldepttype);
                string catagorycode = returnwithsinglecodevalue(cblcategory);
                string leavetype = Convert.ToString(cblleavetype.SelectedValue);
                string staffcodemul = Convert.ToString(cbostaffname.SelectedValue);
                string filter = "";
                string staffcode = "";
                string Appl_ID = "";
                bool leavedayscheckcount = false;
                if (staffdetails_ds.Tables[0].Rows.Count > 0)
                {
                    if (String.IsNullOrEmpty(strstaffcode))
                    {
                        if (deptcode.Trim().ToLower() != "")
                        {
                            filter = " dept_code in ('" + deptcode + "')";
                        }
                        if (catagorycode.Trim().ToLower() != "")
                        {
                            if (filter.Trim() != "")
                                filter += " and";
                            filter += "  category_code in ('" + catagorycode + "')";
                        }
                        if (staffcodemul.Trim().ToLower() != "all")
                        {
                            if (filter.Trim() != "")
                                filter += " and";
                            filter += "  Staff_Code in ('" + staffcodemul + "')";
                        }
                    }
                    else
                    {
                        if (filter.Trim() != "")
                            filter += " and";
                        filter += "  Staff_Code='" + strstaffcode + "'";
                    }
                    if (leavetype.Trim().ToLower() != "all")
                    {

                    }
                    bool leavesingle = false;
                    staffdetails_ds.Tables[0].DefaultView.RowFilter = filter;
                    DataView staffdetails_dv = staffdetails_ds.Tables[0].DefaultView;

                    if (staffdetails_dv.Count > 0)
                    {
                        for (int i = 0; i < staffdetails_dv.Count; i++)
                        {
                            Hashtable totalvalue_dic = new Hashtable();
                            fpsalary.Sheets[0].RowCount++;
                            fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 1, 0].Text = Convert.ToString(i + 1);
                            fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 1, 1].Text = Convert.ToString(staffdetails_dv[i]["Staff_Name"]);
                            fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 1, 1].Tag = Convert.ToString(staffdetails_dv[i]["Staff_Code"]);
                            staffcode = Convert.ToString(staffdetails_dv[i]["Staff_Code"]);
                            Appl_ID = Convert.ToString(staffdetails_dv[i]["Appl_ID"]);
                            fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 1, 2].Text = Convert.ToString(staffdetails_dv[i]["dept_acronym"]);//Dept_Name
                            fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 1, 2].Tag = Convert.ToString(staffdetails_dv[i]["Appl_ID"]);
                            fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 1, 3].Text = Convert.ToString(staffdetails_dv[i]["Desig_Name"]);
                            fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 1, 4].Text = Convert.ToString(staffdetails_dv[i]["Category_Name"]);
                            fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 1, 0].Font.Bold = true;
                            fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 1, 0].Font.Name = "Book Antiqua";
                            fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 1, 0].Font.Size = FontUnit.Medium;
                            fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 1, 0].HorizontalAlign = HorizontalAlign.Center;

                            fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 1, 1].Font.Bold = true;
                            fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 1, 1].Font.Name = "Book Antiqua";
                            fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 1, 1].Font.Size = FontUnit.Medium;
                            fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 1, 1].HorizontalAlign = HorizontalAlign.Left;

                            fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 1, 2].Font.Bold = true;
                            fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 1, 2].Font.Name = "Book Antiqua";
                            fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 1, 2].Font.Size = FontUnit.Medium;
                            fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 1, 2].HorizontalAlign = HorizontalAlign.Left;

                            fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 1, 3].Font.Bold = true;
                            fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 1, 3].Font.Name = "Book Antiqua";
                            fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 1, 3].Font.Size = FontUnit.Medium;
                            fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 1, 3].HorizontalAlign = HorizontalAlign.Left;

                            fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 1, 4].Font.Bold = true;
                            fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 1, 4].Font.Name = "Book Antiqua";
                            fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 1, 4].Font.Size = FontUnit.Medium;
                            fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 1, 4].HorizontalAlign = HorizontalAlign.Left;

                            //paymonth
                            if (ds2.Tables[2].Rows.Count > 0)
                            {
                                DataView ds2table2_dv = new DataView();
                                string ds2tablefilter = "";
                                if (rdomonthlywise.Checked == true)
                                {
                                    string selectedmonth = returnwithsinglecodevalue(chkmonth);
                                    if (String.IsNullOrEmpty(selectedmonth.Trim()))
                                    {
                                        lblnorec.Visible = true;
                                        lblnorec.Text = "Please Select Month!";
                                        fpsalary.Visible = false;
                                        rptprint.Visible = false;
                                        return;
                                    }
                                    ds2tablefilter = " PayMonthNum in('" + selectedmonth + "') and PayYear in('" + ddlyear.SelectedItem.Value.ToString() + "')";
                                }

                                if (rdodaywise.Checked == true)
                                {
                                    string month = ""; string year = "";
                                    string date2 = Txtentryto.Text.ToString();
                                    string[] split1 = date2.Split(new Char[] { '/' });
                                    dateto = split1[1].ToString() + "/" + split1[0].ToString() + "/" + split1[2].ToString();
                                    month = split1[1].TrimStart('0').ToString();
                                    year = split1[2].ToString();

                                    ds2tablefilter = " PayMonthNum in('" + month + "') and PayYear in('" + year + "')";

                                }
                                ds2.Tables[2].DefaultView.RowFilter = ds2tablefilter;
                                ds2table2_dv = ds2.Tables[2].DefaultView;
                                DataTable ds2table2_dt = ds2table2_dv.ToTable();


                                if (ds2table2_dt.Rows.Count > 0)
                                {
                                    for (int p = 0; p < ds2table2_dt.Rows.Count; p++)
                                    {
                                        if (p != 0)
                                        {
                                            fpsalary.Sheets[0].RowCount++;
                                        }
                                        fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 1, 5].Text = Convert.ToString(ds2table2_dt.Rows[p]["PayMonth"]) + "-" + Convert.ToString(ds2table2_dt.Rows[p]["Payyear"]);
                                        fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 1, 5].Tag = Convert.ToString(ds2table2_dt.Rows[p]["PayMonthNum"]);
                                        fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 1, 5].Note = Convert.ToString(ds2table2_dt.Rows[p]["Payyear"]);
                                        fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 1, 5].Font.Bold = true;
                                        fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 1, 5].Font.Name = "Book Antiqua";
                                        fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 1, 5].Font.Size = FontUnit.Medium;
                                        fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 1, 5].HorizontalAlign = HorizontalAlign.Left;
                                        double llcount = 0;
                                        totalleave.Clear();
                                        totalbind.Clear();
                                        double addtot = 0;
                                        string actual = "";
                                        double tot_leave = 0;
                                        string leavefromdate = "";
                                        string leavetodate = "";
                                        string ishalfdate = "";
                                        string halfdaydate = "";
                                        int finaldate = 0;
                                        string sleave = "";
                                        if (ds2.Tables.Count > 0 && ds2.Tables[0].Rows.Count > 0)
                                        {
                                            ds2.Tables[0].DefaultView.RowFilter = " Staff_Code= '" + staffcode + "'";
                                            DataView ds2table0_dv = ds2.Tables[0].DefaultView;
                                            if (ds2table0_dv.Count > 0)
                                            {
                                                string[] spl_type = ds2table0_dv[0]["leavetype"].ToString().Split(new Char[] { '\\' });
                                                for (int k = 0; k < ds2table0_dv.Count; k++)
                                                {
                                                    int col = 6;
                                                    for (int l = 0; spl_type.GetUpperBound(0) >= l; l++)
                                                    {
                                                        string leave = "";
                                                        if (spl_type[l].Trim() != "")
                                                        {
                                                            col++;
                                                            tot_leave = 0;
                                                            string[] split_leave = spl_type[l].Split(';');
                                                            leave = split_leave[0];
                                                            if (split_leave.Length >= 2)
                                                            {
                                                                double.TryParse(Convert.ToString(split_leave[1]), out addtot);
                                                            }
                                                            if (leavetype.Trim().ToLower() != "all")
                                                            {
                                                                if (leave.ToLower().Trim() == Convert.ToString(cblleavetype.SelectedItem.Text).ToLower())
                                                                {
                                                                    leavesingle = true;
                                                                }
                                                                else
                                                                {
                                                                    leavesingle = false;
                                                                }
                                                            }
                                                            else { leavesingle = true; }
                                                            if (leavesingle == true)
                                                            {
                                                                string leavepk = "";
                                                                ds2.Tables[1].DefaultView.RowFilter = " category='" + leave + "'";
                                                                DataView leavepk_dv = ds2.Tables[1].DefaultView;
                                                                if (leavepk_dv.Count > 0)
                                                                {
                                                                    leavepk = Convert.ToString(leavepk_dv[0]["LeaveMasterPK"]);
                                                                }
                                                                ds2.Tables[2].DefaultView.RowFilter = " PayMonthNum='" + Convert.ToString(ds2table2_dt.Rows[p]["PayMonthNum"]) + "' and PayYear ='" + Convert.ToString(ds2table2_dt.Rows[p]["Payyear"]) + "'";
                                                                DataView Lfromlto_dv = ds2.Tables[2].DefaultView;
                                                                if (Lfromlto_dv.Count > 0)
                                                                {
                                                                    string dt_get_leave = string.Empty;
                                                                    if (rdodaywise.Checked == true)
                                                                    {
                                                                        string month = ""; string year = "";
                                                                        string date2 = Txtentryto.Text.ToString();
                                                                        string[] split1 = date2.Split(new Char[] { '/' });
                                                                        dateto = split1[1].ToString() + "/" + split1[0].ToString() + "/" + split1[2].ToString();
                                                                        month = split1[1].TrimStart('0').ToString();
                                                                        year = split1[2].ToString();

                                                                        dt_get_leave = "select * from RQ_Requisition r,leave_category l where RequestType=5 and LeaveFrom='" + Convert.ToString(dateto) + "' and LeaveTo='" + Convert.ToString(dateto) + "' and ReqAppStatus='1' and l.LeaveMasterPK=r.LeaveMasterFK and r.LeaveMasterFK='" + leavepk + "' and ReqAppNo='" + Appl_ID + "'  ";
                                                                    }
                                                                    else
                                                                    {
                                                                        dt_get_leave = "select * from RQ_Requisition r,leave_category l where RequestType=5 and LeaveFrom>='" + Lfromlto_dv[k]["From_Date"].ToString() + "' and LeaveTo<='" + Lfromlto_dv[k]["To_Date"].ToString() + "' and ReqAppStatus='1' and l.LeaveMasterPK=r.LeaveMasterFK and r.LeaveMasterFK='" + leavepk + "' and ReqAppNo='" + Appl_ID + "'  ";
                                                                    }
                                                                    ds1 = d2.select_method_wo_parameter(dt_get_leave, "Text");
                                                                    if (ds1.Tables[0].Rows.Count > 0)//delsi0314
                                                                    {
                                                                        for (int g = 0; g < ds1.Tables[0].Rows.Count; g++)
                                                                        {
                                                                            leavefromdate = Convert.ToString(ds1.Tables[0].Rows[g]["LeaveFrom"]);
                                                                            leavetodate = Convert.ToString(ds1.Tables[0].Rows[g]["LeaveTo"]);
                                                                            ishalfdate = Convert.ToString(ds1.Tables[0].Rows[g]["IsHalfDay"]);
                                                                            if (leavefromdate != "" && leavetodate != "")
                                                                            {
                                                                                string dtT = leavefromdate;
                                                                                string[] Split = dtT.Split('/');
                                                                                string enddt = leavetodate;
                                                                                Split = enddt.Split('/');
                                                                                DateTime fromdate = Convert.ToDateTime(dtT);
                                                                                DateTime todate = Convert.ToDateTime(enddt);
                                                                                TimeSpan days = todate - fromdate;
                                                                                string ndate = Convert.ToString(days);
                                                                                Split = ndate.Split('.');
                                                                                string getdate = Split[0];
                                                                                llcount = 0;
                                                                                if (fromdate != todate)
                                                                                {
                                                                                    for (; fromdate <= todate; )
                                                                                    {
                                                                                        string dayy = fromdate.ToString("dddd");
                                                                                        leavedayscheckcount = false;
                                                                                        if (dayy == "Sunday")
                                                                                        {
                                                                                            if (split_leave[3] == "0")
                                                                                                leavedayscheckcount = true;
                                                                                            else
                                                                                                leavedayscheckcount = false;
                                                                                        }
                                                                                        if (leavedayscheckcount == false)
                                                                                        {
                                                                                            llcount++;
                                                                                        }
                                                                                        fromdate = fromdate.AddDays(1);
                                                                                    }
                                                                                }
                                                                                else
                                                                                {
                                                                                    llcount++;
                                                                                }
                                                                                if (ishalfdate == "True")
                                                                                {
                                                                                    halfdaydate = Convert.ToString(ds1.Tables[0].Rows[g]["HalfDate"]);
                                                                                    if (tot_leave == 0)
                                                                                    {
                                                                                        tot_leave = llcount;
                                                                                        tot_leave = tot_leave - 0.5;
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        tot_leave = tot_leave + llcount;
                                                                                        tot_leave = tot_leave - 0.5;
                                                                                    }
                                                                                    sleave = leave + "-" + tot_leave;
                                                                                }
                                                                                else
                                                                                {
                                                                                    if (tot_leave == 0)
                                                                                    {
                                                                                        tot_leave = tot_leave + llcount;
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        tot_leave = tot_leave + llcount;
                                                                                    }
                                                                                    sleave = leave + "-" + tot_leave;
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                    if (spl_type[l].Contains(";"))
                                                                    {
                                                                        string sp = split_leave[0].ToString();
                                                                        actual = split_leave[2].ToString();
                                                                        if (actual == "")
                                                                        {
                                                                            actual = "0";
                                                                        }
                                                                        string[] iii = sleave.Split('-');
                                                                        string sp1 = iii[0];
                                                                        if (sp != sp1)
                                                                        {
                                                                            tot_leave = 0;
                                                                        }
                                                                        string tt = Convert.ToString(tot_leave + "/" + actual);
                                                                        if (!totalleave.Contains(Convert.ToString(leave)))
                                                                            totalleave.Add(Convert.ToString(leave), Convert.ToString(tt));
                                                                        else
                                                                        {
                                                                            string getvalue = Convert.ToString(totalleave[Convert.ToString(leave)]);
                                                                            if (getvalue.Trim() != "")
                                                                            {
                                                                                getvalue = getvalue + "," + tt;
                                                                                totalleave.Remove(Convert.ToString(leave));
                                                                                if (getvalue.Trim() != "")
                                                                                    totalleave.Add(Convert.ToString(leave), Convert.ToString(getvalue));
                                                                            }
                                                                        }
                                                                        int colcount = Convert.ToInt32(leavetypehash[leave]);
                                                                        if (colcount != 0)
                                                                        {
                                                                            // poomalar 06.12.17                                                                            
                                                                            #region for table merge
                                                                            string fistcasual = "HalfDay@fh@" + leave + ""; double monthcount = 0; // poo
                                                                            string secondcasual = "HalfDay@sh@" + leave + ""; //poo
                                                                            string paymonth = Convert.ToString(fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 1, 5].Tag);
                                                                            string dbyear = Convert.ToString(ds2.Tables[2].Rows[p]["payyear"]);
                                                                            if (rdoyearlywise.Checked == true)
                                                                            {

                                                                             //   string sqlpaymonth = "select sum(no_days) leave,month(fdate) month,year(fdate) year from staff_leave_details where (lt_taken ='" + leave + "' or lt_taken ='" + secondcasual + "' or lt_taken ='" + fistcasual + "') and staff_code='" + staffcode + "' and apply_approve=1 and college_code ='" + Session["collegecode"] + "'  and fdate >=   (SELECT top 1 CONVERT(VARCHAR(2),DATEPART(MONTH, From_Date)) + '/'+ CONVERT(VARCHAR(2),DATEPART(DAY, From_Date)) + '/' + CONVERT(VARCHAR(4),DATEPART(YEAR, From_Date)) from hrpaymonths order by From_Date) and tdate <= (SELECT top 1 CONVERT(VARCHAR(2),DATEPART(MONTH, To_Date)) + '/'+ CONVERT(VARCHAR(2),DATEPART(DAY, To_Date)) + '/' + CONVERT(VARCHAR(4),DATEPART(YEAR, To_Date)) from hrpaymonths order by To_Date DESC) group by month(fdate),year(fdate)";

                                                                                string sqlpaymonth = "select sum(no_days) leave from staff_leave_details where (lt_taken ='" + leave + "' or lt_taken ='" + secondcasual + "' or lt_taken ='" + fistcasual + "') and staff_code='" + staffcode + "' and apply_approve=1 and college_code ='" + Session["collegecode"] + "' and fdate>='" + Lfromlto_dv[k]["From_Date"].ToString() + "' and tdate<='" + Lfromlto_dv[k]["To_Date"].ToString() + "'  and fdate >=   (SELECT top 1 CONVERT(VARCHAR(2),DATEPART(MONTH, From_Date)) + '/'+ CONVERT(VARCHAR(2),DATEPART(DAY, From_Date)) + '/' + CONVERT(VARCHAR(4),DATEPART(YEAR, From_Date)) from hrpaymonths order by From_Date) and tdate <= (SELECT top 1 CONVERT(VARCHAR(2),DATEPART(MONTH, To_Date)) + '/'+ CONVERT(VARCHAR(2),DATEPART(DAY, To_Date)) + '/' + CONVERT(VARCHAR(4),DATEPART(YEAR, To_Date)) from hrpaymonths order by To_Date DESC)";

                                                                                DataSet dspaymonth = new DataSet();
                                                                                dspaymonth = d2.select_method_wo_parameter(sqlpaymonth, "Text");

                                                                                if (dspaymonth.Tables[0].Rows.Count > 0)
                                                                                {
                                                                                    //for (int pay = 0; pay < dspaymonth.Tables[0].Rows.Count; pay++)
                                                                                    //{
                                                                                    //    string curmonth = Convert.ToString(dspaymonth.Tables[0].Rows[pay]["month"]);
                                                                                    //    string curyear = Convert.ToString(dspaymonth.Tables[0].Rows[pay]["year"]);
                                                                                    //    if (dbyear == curyear)
                                                                                    //    {
                                                                                    //        if (paymonth == curmonth)
                                                                                    //        {
                                                                                    //            double.TryParse(Convert.ToString(dspaymonth.Tables[0].Rows[pay]["leave"]), out monthcount);
                                                                                    //            tot_leave += monthcount;
                                                                                    //        }
                                                                                    //    }

                                                                                    //}
                                                                                    double.TryParse(Convert.ToString(dspaymonth.Tables[0].Rows[0]["leave"]), out monthcount);
                                                                                    tot_leave += monthcount;//delsi11/05/2018

                                                                                }
                                                                            }

                                                                            string sql = string.Empty; string leaveold = string.Empty; double oldleave = 0;
                                                                            if (rdomonthlywise.Checked == true)
                                                                            {

                                                                                string selectedmonth = returnwithsinglecodevalue(chkmonth);
                                                                                string monthselec = Convert.ToString(ds2table2_dt.Rows[p]["PayMonthNum"]); ;
                                                                                if (String.IsNullOrEmpty(selectedmonth.Trim()))
                                                                                {
                                                                                    lblnorec.Visible = true;
                                                                                    lblnorec.Text = "Please Select Month!";
                                                                                    fpsalary.Visible = false;
                                                                                    rptprint.Visible = false;
                                                                                    return;
                                                                                }
                                                                                sql = "select sum(no_days) from staff_leave_details where (lt_taken ='" + leave + "' or lt_taken ='" + secondcasual + "' or lt_taken ='" + fistcasual + "') and staff_code='" + staffcode + "' and apply_approve=1 and college_code ='" + Session["collegecode"] + "'  and fdate >=   (SELECT top 1 CONVERT(VARCHAR(2),DATEPART(MONTH, From_Date)) + '/'+ CONVERT(VARCHAR(2),DATEPART(DAY, From_Date)) + '/' + CONVERT(VARCHAR(4),DATEPART(YEAR, From_Date)) from hrpaymonths order by From_Date) and tdate <= (SELECT top 1 CONVERT(VARCHAR(2),DATEPART(MONTH, To_Date)) + '/'+ CONVERT(VARCHAR(2),DATEPART(DAY, To_Date)) + '/' + CONVERT(VARCHAR(4),DATEPART(YEAR, To_Date)) from hrpaymonths order by To_Date DESC) and month(staff_leave_details.fdate) in('" + monthselec + "') and month  (staff_leave_details.tdate) in('" + monthselec + "') and  year(staff_leave_details.fdate)='" + ddlyear.SelectedItem.Text + "' and year (staff_leave_details.tdate)='" + ddlyear.SelectedItem.Text + "'"; // poo 06.12.17 //paymonth

                                                                                DataSet dstab = new DataSet();
                                                                                dstab = d2.select_method_wo_parameter(sql, "Text");
                                                                                leaveold = d2.GetFunction(sql); double.TryParse(leaveold, out oldleave);
                                                                                tot_leave += oldleave;


                                                                            }
                                                                            if (rdodaywise.Checked == true)
                                                                            {
                                                                                string month = ""; string year = "";
                                                                                string date2 = Txtentryto.Text.ToString();
                                                                                string[] split1 = date2.Split(new Char[] { '/' });
                                                                                dateto = split1[1].ToString() + "/" + split1[0].ToString() + "/" + split1[2].ToString();
                                                                                month = split1[1].TrimStart('0').ToString();
                                                                                year = split1[2].ToString();
                                                                                string dayquery = "select sum(no_days) from staff_leave_details where (lt_taken ='" + leave + "' or lt_taken ='" + secondcasual + "' or lt_taken ='" + fistcasual + "') and staff_code='" + staffcode + "' and apply_approve=1 and college_code ='" + Session["collegecode"] + "'  and fdate >=   (SELECT top 1 CONVERT(VARCHAR(2),DATEPART(MONTH, From_Date)) + '/'+ CONVERT(VARCHAR(2),DATEPART(DAY, From_Date)) + '/' + CONVERT(VARCHAR(4),DATEPART(YEAR, From_Date)) from hrpaymonths order by From_Date) and tdate <= (SELECT top 1 CONVERT(VARCHAR(2),DATEPART(MONTH, To_Date)) + '/'+ CONVERT(VARCHAR(2),DATEPART(DAY, To_Date)) + '/' + CONVERT(VARCHAR(4),DATEPART(YEAR, To_Date)) from hrpaymonths order by To_Date DESC) and fdate='" + dateto + "' and tdate='" + dateto + "' ";
                                                                                DataSet dsday = new DataSet();
                                                                                dsday = d2.select_method_wo_parameter(dayquery, "Text");
                                                                                leaveold = d2.GetFunction(dayquery); double.TryParse(leaveold, out oldleave);
                                                                                tot_leave += oldleave;
                                                                            }

                                                                            //DataTable dstab_dt = dstab_dv.ToTable();


                                                                            #endregion


                                                                            fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 1, colcount].Text = Convert.ToString(tot_leave + "/" + actual);
                                                                            fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 1, colcount].Tag = staffcode;
                                                                            fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 1, colcount].Note = Appl_ID;
                                                                            fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 1, colcount].Font.Bold = true;
                                                                            fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 1, colcount].Font.Name = "Book Antiqua";
                                                                            fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 1, colcount].Font.Size = FontUnit.Medium;
                                                                            fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 1, colcount].HorizontalAlign = HorizontalAlign.Center;
                                                                            if (totalvalue_dic.Contains(leave))
                                                                            {
                                                                                string value = totalvalue_dic[leave].ToString();
                                                                                string[] leavecount = value.Split('/');
                                                                                totalvalue_dic.Remove(leave);
                                                                                double leavecnt = 0;//barath 19.06.17
                                                                                double.TryParse(leavecount[0], out leavecnt);
                                                                                //leavecnt = leavecnt+oldleave;
                                                                                double total = (leavecnt + tot_leave);
                                                                                //total += oldleave;
                                                                                //int total = Convert.ToInt32(leavecount[0]) + Convert.ToInt32(tot_leave);                  
                                                                                totalvalue_dic.Add(leave, total + "/" + addtot);
                                                                            }
                                                                            else
                                                                            {
                                                                                //tot_leave =tot_leave+ oldleave;
                                                                                totalvalue_dic.Add(leave, Convert.ToString(tot_leave) + "/" + addtot);
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    fpsalary.Sheets[0].SpanModel.Add(fpsalary.Sheets[0].RowCount - ds2table2_dt.Rows.Count, 0, ds2table2_dt.Rows.Count, 1);
                                    fpsalary.Sheets[0].SpanModel.Add(fpsalary.Sheets[0].RowCount - ds2table2_dt.Rows.Count, 1, ds2table2_dt.Rows.Count, 1);
                                    fpsalary.Sheets[0].SpanModel.Add(fpsalary.Sheets[0].RowCount - ds2table2_dt.Rows.Count, 2, ds2table2_dt.Rows.Count, 1);
                                    fpsalary.Sheets[0].SpanModel.Add(fpsalary.Sheets[0].RowCount - ds2table2_dt.Rows.Count, 3, ds2table2_dt.Rows.Count, 1);
                                    fpsalary.Sheets[0].SpanModel.Add(fpsalary.Sheets[0].RowCount - ds2table2_dt.Rows.Count, 4, ds2table2_dt.Rows.Count, 1);
                                    fpsalary.Sheets[0].RowCount++;
                                    fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 1, 5].Text = "Total";
                                    fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 1, 5].Font.Bold = true;
                                    fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 1, 5].Font.Name = "Book Antiqua";
                                    fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 1, 5].Font.Size = FontUnit.Medium;
                                    fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 1, 5].HorizontalAlign =
HorizontalAlign.Right;
                                    fpsalary.Sheets[0].SpanModel.Add(fpsalary.Sheets[0].RowCount - 1, 0, 1, 5);
                                    fpsalary.Sheets[0].RowCount++;
                                    fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 1, 5].Text = "Taken";
                                    fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 1, 5].Font.Bold = true;
                                    fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 1, 5].Font.Name = "Book Antiqua";
                                    fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 1, 5].Font.Size = FontUnit.Medium;
                                    fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 1, 5].HorizontalAlign =
HorizontalAlign.Right;
                                    fpsalary.Sheets[0].SpanModel.Add(fpsalary.Sheets[0].RowCount - 1, 0, 1, 5);
                                    fpsalary.Sheets[0].RowCount++;
                                    fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 1, 5].Text = "Available";
                                    fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 1, 5].Font.Bold = true;
                                    fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 1, 5].Font.Name = "Book Antiqua";
                                    fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 1, 5].Font.Size = FontUnit.Medium;
                                    fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 1, 5].HorizontalAlign =
HorizontalAlign.Right;
                                    fpsalary.Sheets[0].SpanModel.Add(fpsalary.Sheets[0].RowCount - 1, 0, 1, 5);
                                    if (totalvalue_dic.Count > 0)
                                    {
                                        foreach (DictionaryEntry item in totalvalue_dic)
                                        {
                                            string leave_key = Convert.ToString(item.Key);
                                            string Value = Convert.ToString(item.Value);
                                            string[] total = Value.Split('/');
                                            int colcount1 = Convert.ToInt32(leavetypehash[leave_key]);
                                            if (total.Length > 1)
                                            {
                                                double tot = 0;
                                                double.TryParse(total[0].ToString(), out tot);
                                                fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 3, colcount1].Text = tot + "/" + total[1].ToString();
                                                fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 2, colcount1].Text = Convert.ToString(tot);
                                                fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 1, colcount1].Text = Convert.ToString(Convert.ToDouble(total[1]) - Convert.ToDouble(tot));
                                                fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 3, colcount1].Font.Bold = true;
                                                fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 3, colcount1].Font.Name = "Book Antiqua";
                                                fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 3, colcount1].Font.Size = FontUnit.Medium;
                                                fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 3, colcount1].HorizontalAlign =
            HorizontalAlign.Center;
                                                fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 2, colcount1].Font.Bold = true;
                                                fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 2, colcount1].Font.Name = "Book Antiqua";
                                                fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 2, colcount1].Font.Size = FontUnit.Medium;
                                                fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 2, colcount1].HorizontalAlign =
            HorizontalAlign.Center;
                                                fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 1, colcount1].Font.Bold = true;
                                                fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 1, colcount1].Font.Name = "Book Antiqua";
                                                fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 1, colcount1].Font.Size = FontUnit.Medium;
                                                fpsalary.Sheets[0].Cells[fpsalary.Sheets[0].RowCount - 1, colcount1].HorizontalAlign =
            HorizontalAlign.Center;

                                                fpsalary.Sheets[0].Rows[fpsalary.Sheets[0].RowCount - 1].BackColor = Color.Bisque;
                                                fpsalary.Sheets[0].Rows[fpsalary.Sheets[0].RowCount - 1].ForeColor = Color.IndianRed;
                                                fpsalary.Sheets[0].Rows[fpsalary.Sheets[0].RowCount - 2].BackColor = Color.Bisque;
                                                fpsalary.Sheets[0].Rows[fpsalary.Sheets[0].RowCount - 2].ForeColor = Color.IndianRed;
                                                fpsalary.Sheets[0].Rows[fpsalary.Sheets[0].RowCount - 3].BackColor = Color.Bisque;
                                                fpsalary.Sheets[0].Rows[fpsalary.Sheets[0].RowCount - 3].ForeColor = Color.IndianRed;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                fpsalary.Sheets[0].PageSize = fpsalary.Sheets[0].RowCount;
                fpsalary.Visible = true;
                rptprint.Visible = true;
                lblnorec.Visible = false;

            }
            fpsalary.Visible = true;
        }
        catch (Exception ex)
        {
            lblnorec.Visible = true;
            lblnorec.Text = ex.ToString();
            d2.sendErrorMail(ex, Convert.ToString(Session["collegecode"]), "staffleavereport.aspx");
        }
    }

    protected void btnExcel_Click(object sender, EventArgs e)
    {
        try
        {
            string reportname = txtexcelname.Text;
            if (reportname.ToString().Trim() != "")
            {
                d2.printexcelreport(fpsalary, reportname);
                lblvalidation1.Visible = false;
            }
            else
            {
                lblvalidation1.Text = "Please Enter Your Report Name";
                lblvalidation1.Visible = true;
                txtexcelname.Focus();
            }
        }
        catch
        {
        }
    }
    protected void btnprintmaster_Click(object sender, EventArgs e)
    {
        try
        {
            string page_name = string.Empty;
            string degreedetails = string.Empty;
            string date = "";
            if (rdoyearlywise.Checked == true)
            {
                page_name = "Yearly Wise Staff Cumulative Report";
                date = "";
            }
            else if (rdomonthlywise.Checked == true)
            {
                page_name = "Monthly Wise Staff Cumulative Report";
                date = "";
            }
            if (rdodaywise.Checked == true)
            {
                page_name = "Day Wise Staff Cumulative Report";
                date = "@Date :" + Txtentryfrom.Text.ToString();// +" To :" + Txtentryto.Text.ToString();
            }

            Session["column_header_row_count"] = fpsalary.Sheets[0].SheetCorner.RowCount;

            degreedetails = page_name;
            string pagename = "staffleavereport.aspx";

            Printcontrol.loadspreaddetails(fpsalary, pagename, degreedetails);
            Printcontrol.Visible = true;
        }
        catch
        { }
    }
    protected void fpsalary_CellClick(Object sender, EventArgs e)
    {
        cellflag = true;
    }
    protected void fpsalary_PreRender(Object sender, EventArgs e)
    {
        try
        {

            if (cellflag == true)//delsis
            {
                lblnorec.Visible = true;
                DataSet newdataset = new DataSet();
                newdataset.Clear();
                int sno = 0;
                ViewReport.Visible = true;
                FpstaffLeave.Visible = true;
                string activerow = fpsalary.ActiveSheetView.ActiveRow.ToString();
                string activecol = fpsalary.ActiveSheetView.ActiveColumn.ToString();
                int Month = Convert.ToInt32(fpsalary.Sheets[0].Cells[Convert.ToInt32(activerow), 5].Tag);
                int year = Convert.ToInt32(fpsalary.Sheets[0].Cells[Convert.ToInt32(activerow), 5].Note);

                string querys = string.Empty;
                DataSet getmonthds = new DataSet();
                querys = "select * from hrpaymonths where college_code='" + collegecode1 + "' and SelStatus='1' and PayMonthNum='" + Month + "' and PayYear='" + year + "'";

                getmonthds = d2.select_method_wo_parameter(querys, "text");
                DateTime frmdate = new DateTime();
                DateTime todate = new DateTime();
                if (getmonthds.Tables[0].Rows.Count > 0)
                {
                    frmdate = Convert.ToDateTime(getmonthds.Tables[0].Rows[0]["From_Date"]);
                    todate = Convert.ToDateTime(getmonthds.Tables[0].Rows[0]["To_Date"]);
                
                }

                int lastDay = DateTime.DaysInMonth(year, Month);
                int FirstDay = 1;

            //   string LDate = Convert.ToString(Month + "/" + lastDay + "/" + year);
             //   DateTime L_Date = Convert.ToDateTime(LDate);

              //  string FDate = Convert.ToString(Month + "/" + FirstDay + "/" + year);
                // DateTime F_Date = Convert.ToDateTime(FDate); changed query F_Date to frmdate and lastDay to t0date
             
                string stf_code = Convert.ToString(fpsalary.Sheets[0].Cells[Convert.ToInt32(activerow), Convert.ToInt32(activecol)].Tag);
                string stf_name = d2.GetFunction("select staff_name from staffmaster where staff_code='" + stf_code + "'");
                string leave_fk = Convert.ToString(fpsalary.Sheets[0].ColumnHeader.Cells[1, Convert.ToInt32(activecol)].Tag);
                string Staff_appNo = Convert.ToString(fpsalary.Sheets[0].Cells[Convert.ToInt32(activerow), Convert.ToInt32(activecol)].Note);
                string Query = "select * from RQ_Requisition r,leave_category l where RequestType=5 and r.LeaveFrom>='" + frmdate + "' and LeaveTo<='" + todate + "' and ReqAppStatus='1' and l.LeaveMasterPK=r.LeaveMasterFK and r.LeaveMasterFK='" + leave_fk + "' and reqappNo='" + Staff_appNo + "'";
                newdataset = d2.select_method_wo_parameter(Query, "text");

                FpstaffLeave.Sheets[0].RowCount = 0;
                FpstaffLeave.Sheets[0].ColumnCount = 6;
                FpstaffLeave.SaveChanges();
                FpstaffLeave.SheetCorner.ColumnCount = 0;
                FpstaffLeave.CommandBar.Visible = false;
                FarPoint.Web.Spread.StyleInfo darkstyle = new FarPoint.Web.Spread.StyleInfo();
                darkstyle.BackColor = ColorTranslator.FromHtml("#0CA6CA");
                darkstyle.ForeColor = Color.White;
                FpstaffLeave.ActiveSheetView.ColumnHeader.DefaultStyle = darkstyle;
                FpstaffLeave.ActiveSheetView.SelectionBackColor = Color.Coral;
                FpstaffLeave.Sheets[0].AutoPostBack = false;
                FpstaffLeave.Sheets[0].ColumnHeader.Cells[0, 0].Text = "S.No";
                FpstaffLeave.Sheets[0].ColumnHeader.Cells[0, 0].Font.Bold = true;
                FpstaffLeave.Sheets[0].ColumnHeader.Cells[0, 0].Font.Name = "Book Antiqua";
                FpstaffLeave.Sheets[0].ColumnHeader.Cells[0, 0].HorizontalAlign = HorizontalAlign.Center;
                FpstaffLeave.Sheets[0].Columns[0].Locked = true;
                FpstaffLeave.Columns[0].Width = 80;
                FpstaffLeave.Sheets[0].ColumnHeader.Cells[0, 1].Text = "Staff Name";
                FpstaffLeave.Sheets[0].ColumnHeader.Cells[0, 1].Font.Bold = true;
                FpstaffLeave.Sheets[0].ColumnHeader.Cells[0, 1].Font.Name = "Book Antiqua";
                FpstaffLeave.Sheets[0].ColumnHeader.Cells[0, 1].HorizontalAlign = HorizontalAlign.Center;
                FpstaffLeave.Sheets[0].Columns[1].Locked = true;
                FpstaffLeave.Columns[1].Width = 100;
                FpstaffLeave.Sheets[0].ColumnHeader.Cells[0, 2].Text = "From Date";
                FpstaffLeave.Sheets[0].ColumnHeader.Cells[0, 2].Font.Bold = true;
                FpstaffLeave.Sheets[0].ColumnHeader.Cells[0, 2].Font.Name = "Book Antiqua";
                FpstaffLeave.Sheets[0].ColumnHeader.Cells[0, 2].HorizontalAlign = HorizontalAlign.Center;
                FpstaffLeave.Sheets[0].Columns[2].Locked = true;
                FpstaffLeave.Columns[2].Width = 100;
                FpstaffLeave.Sheets[0].ColumnHeader.Cells[0, 3].Text = "To Date";
                FpstaffLeave.Sheets[0].ColumnHeader.Cells[0, 3].Font.Bold = true;
                FpstaffLeave.Sheets[0].ColumnHeader.Cells[0, 3].Font.Name = "Book Antiqua";
                FpstaffLeave.Sheets[0].ColumnHeader.Cells[0, 3].HorizontalAlign = HorizontalAlign.Center;
                FpstaffLeave.Sheets[0].Columns[3].Locked = true;
                FpstaffLeave.Columns[3].Width = 100;
                FpstaffLeave.Sheets[0].ColumnHeader.Cells[0, 4].Text = "Leave Type";
                FpstaffLeave.Sheets[0].ColumnHeader.Cells[0, 4].Font.Bold = true;
                FpstaffLeave.Sheets[0].ColumnHeader.Cells[0, 4].Font.Name = "Book Antiqua";
                FpstaffLeave.Sheets[0].ColumnHeader.Cells[0, 4].HorizontalAlign = HorizontalAlign.Center;
                FpstaffLeave.Sheets[0].Columns[4].Locked = true;
                FpstaffLeave.Columns[4].Width = 100;

                FpstaffLeave.Sheets[0].ColumnHeader.Cells[0, 5].Text = "No of Days";
                FpstaffLeave.Sheets[0].ColumnHeader.Cells[0, 5].Font.Bold = true;
                FpstaffLeave.Sheets[0].ColumnHeader.Cells[0, 5].Font.Name = "Book Antiqua";
                FpstaffLeave.Sheets[0].ColumnHeader.Cells[0, 5].HorizontalAlign = HorizontalAlign.Center;
                FpstaffLeave.Sheets[0].Columns[5].Locked = true;
                FpstaffLeave.Columns[5].Width = 100;

                if (newdataset.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < newdataset.Tables[0].Rows.Count; i++)
                    {
                        sno++;
                        double val = 0;
                        string LeaveDay = Convert.ToString(newdataset.Tables[0].Rows[i]["LeaveSession"]);
                        if (LeaveDay == "0")
                        {
                            val = 1;

                        }
                        if (LeaveDay == "1" || LeaveDay == "2")
                        {
                            val = 0.5;
                        }
                        string LeaveFromDate = Convert.ToString(newdataset.Tables[0].Rows[i]["LeaveFrom"]);
                        string[] split = LeaveFromDate.Split(new Char[] { ' ' });
                        string Leave_fdate = split[0];
                        string[] splits = Leave_fdate.Split(new Char[] { '/' });
                        string fdate = splits[1] + "/" + splits[0] + "/" + splits[2];

                        string LeaveToDate = Convert.ToString(newdataset.Tables[0].Rows[i]["LeaveTo"]);
                        string[] split1 = LeaveToDate.Split(new Char[] { ' ' });
                        string Leave_tdate = split1[0];
                        string[] split2 = Leave_tdate.Split(new Char[] { '/' });
                        string tdate = split2[1] + "/" + split2[0] + "/" + split2[2];


                        FpstaffLeave.Sheets[0].RowCount = FpstaffLeave.Sheets[0].RowCount + 1;
                        FpstaffLeave.Sheets[0].Cells[FpstaffLeave.Sheets[0].RowCount - 1, 0].Text = sno.ToString();
                        FpstaffLeave.Sheets[0].Cells[FpstaffLeave.Sheets[0].RowCount - 1, 0].HorizontalAlign = HorizontalAlign.Center;
                        FpstaffLeave.Sheets[0].Cells[FpstaffLeave.Sheets[0].RowCount - 1, 0].Font.Size = FontUnit.Medium;
                        FpstaffLeave.Sheets[0].Cells[FpstaffLeave.Sheets[0].RowCount - 1, 0].Font.Name = "Book Antiqua";
                        FpstaffLeave.Sheets[0].Cells[FpstaffLeave.Sheets[0].RowCount - 1, 1].Text = stf_name;
                        FpstaffLeave.Sheets[0].Cells[FpstaffLeave.Sheets[0].RowCount - 1, 1].HorizontalAlign = HorizontalAlign.Center;
                        FpstaffLeave.Sheets[0].Cells[FpstaffLeave.Sheets[0].RowCount - 1, 1].Font.Size = FontUnit.Medium;
                        FpstaffLeave.Sheets[0].Cells[FpstaffLeave.Sheets[0].RowCount - 1, 1].Font.Name = "Book Antiqua";

                        FpstaffLeave.Sheets[0].Cells[FpstaffLeave.Sheets[0].RowCount - 1, 2].Text = fdate;
                        FpstaffLeave.Sheets[0].Cells[FpstaffLeave.Sheets[0].RowCount - 1, 2].HorizontalAlign = HorizontalAlign.Center;
                        FpstaffLeave.Sheets[0].Cells[FpstaffLeave.Sheets[0].RowCount - 1, 2].Font.Size = FontUnit.Medium;
                        FpstaffLeave.Sheets[0].Cells[FpstaffLeave.Sheets[0].RowCount - 1, 2].Font.Name = "Book Antiqua";
                        FpstaffLeave.Sheets[0].Cells[FpstaffLeave.Sheets[0].RowCount - 1, 3].Text = tdate;
                        FpstaffLeave.Sheets[0].Cells[FpstaffLeave.Sheets[0].RowCount - 1, 3].HorizontalAlign = HorizontalAlign.Center;
                        FpstaffLeave.Sheets[0].Cells[FpstaffLeave.Sheets[0].RowCount - 1, 3].Font.Size = FontUnit.Medium;
                        FpstaffLeave.Sheets[0].Cells[FpstaffLeave.Sheets[0].RowCount - 1, 3].Font.Name = "Book Antiqua";
                        FpstaffLeave.Sheets[0].Cells[FpstaffLeave.Sheets[0].RowCount - 1, 4].Text = Convert.ToString(newdataset.Tables[0].Rows[i]["category"]);
                        FpstaffLeave.Sheets[0].Cells[FpstaffLeave.Sheets[0].RowCount - 1, 4].HorizontalAlign = HorizontalAlign.Center;
                        FpstaffLeave.Sheets[0].Cells[FpstaffLeave.Sheets[0].RowCount - 1, 4].Font.Size = FontUnit.Medium;
                        FpstaffLeave.Sheets[0].Cells[FpstaffLeave.Sheets[0].RowCount - 1, 4].Font.Name = "Book Antiqua";
                        FpstaffLeave.Sheets[0].Cells[FpstaffLeave.Sheets[0].RowCount - 1, 5].Text = Convert.ToString(val);
                        FpstaffLeave.Sheets[0].Cells[FpstaffLeave.Sheets[0].RowCount - 1, 5].HorizontalAlign = HorizontalAlign.Center;
                        FpstaffLeave.Sheets[0].Cells[FpstaffLeave.Sheets[0].RowCount - 1, 5].Font.Size = FontUnit.Medium;
                        FpstaffLeave.Sheets[0].Cells[FpstaffLeave.Sheets[0].RowCount - 1, 5].Font.Name = "Book Antiqua";

                    }
                    int rowcount = FpstaffLeave.Sheets[0].RowCount;
                    FpstaffLeave.Height = 300;
                    FpstaffLeave.Sheets[0].PageSize = 25 + (rowcount * 20);
                    FpstaffLeave.SaveChanges();
                }
                else
                {
                   
                    ViewReport.Visible = false;
                    FpstaffLeave.Visible = false;
                    lblnorec.Visible = true;
                    lblnorec.Text = "No Records Found";

                    


                }
                  
                    

                

            }
        }
        catch (Exception ex)
        {

            //d2.sendErrorMail(ex, collegecode1, "staffleavereport2");
        }
    }

    protected void imagebtnpopclose1_Click(object sender, EventArgs e)
    {
        ViewReport.Visible = false;
    }
}