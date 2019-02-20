using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Drawing;
using System.Web.Services;
using System.Data.SqlClient;

public partial class FinanceYear : System.Web.UI.Page
{
    DataSet ds = new DataSet();
    DAccess2 d2 = new DAccess2();
    static int countnew = 0;
    string collegecode1 = string.Empty;   
    static string col = "";
    bool click = false;
    static string collegestat = string.Empty;
    static string collegestat0 = string.Empty;
    string usercode = string.Empty;
    string collegecode = string.Empty;
    string singleuser = string.Empty;
    string group_user = string.Empty;
    protected void Page_Load(object sender, EventArgs e)
    {     
       
        if (Session["collegecode"] == null)
            Response.Redirect("~/Default.aspx");
        usercode = Session["usercode"].ToString();
        singleuser = Session["single_user"].ToString();
        group_user = Session["group_code"].ToString();
        lblvalidation1.Visible = false;
        if (!IsPostBack)
        {
            setLabelText();
            bindcollege();
            col = "";
            bindloadcol();
            countnew = 0;
            if (ddlcol.Items.Count > 0)
            {
                collegestat = ddlcol.SelectedItem.Value.ToString();
            }
            if (ddlcolload.Items.Count > 0)
            {
                collegestat0 = ddlcolload.SelectedItem.Value.ToString();
            }
            bindaccname();
            cbaccname.Checked = true;
            bttngo_Click(sender, e);
            btn_errorclose_Click(sender, e);
            // col = Convert.ToString(ddlcol.SelectedItem.Value);
            //txtdatestart.Attributes.Add("Readonly", "Readonly");
            //txtdateend.Attributes.Add("Readonly", "Readonly");
        }
        if (ddlcol.Items.Count > 0)
        {
            collegestat = ddlcol.SelectedItem.Value.ToString();
        }
        if (ddlcolload.Items.Count > 0)
        {
            collegestat0 = ddlcolload.SelectedItem.Value.ToString();
        }

    }

    [WebMethod]
    public static string CheckAcronym(string Acronym)
    {
        string returnValue = "1";
        try
        {
            DAccess2 dd = new DAccess2();
            string acr_name = Acronym;
            if (acr_name.Trim() != "" && acr_name != null)
            {
                string queryacr = dd.GetFunction("select distinct FinYearAcr from FM_FinYearMaster where FinYearAcr='" + acr_name + "'  and CollegeCode=" + collegestat + "");
                if (queryacr.Trim() == "" || queryacr == null || queryacr == "0" || queryacr == "-1")
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
    public static string checkAcctName(string acname)
    {
        string returnValue = "1";
        try
        {
            DAccess2 dd = new DAccess2();
            string ac_name = acname;
            if (ac_name.Trim() != "" && ac_name != null)
            {
                string queryacname = dd.GetFunction("select distinct FinYearName from FM_FinYearMaster where FinYearName='" + ac_name + "' and CollegeCode=" + collegestat + "");
                if (queryacname.Trim() == "" || queryacname == null || queryacname == "0" || queryacname == "-1")
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

    protected void btnhelp_Click(object sender, EventArgs e)
    {
        Printcontrol.Visible = false;
    }
    protected void btnyear_Click(object sender, EventArgs e)
    {
        Printcontrol.Visible = false;
    }
    protected void btnsltyear_Click(object sender, EventArgs e)
    {
        Printcontrol.Visible = false;
    }
    protected void btndel_Click(object sender, EventArgs e)
    {
        Printcontrol.Visible = false;
    }
    protected void btn_errorclose_Click(object sender, EventArgs e)
    {
        imgdiv2.Visible = false;
        countnew = 0;
    }
    protected void btnprintmaster_Click(object sender, EventArgs e)
    {
        try
        {
            string degreedetails = "Financial Year Report";
            string pagename = "FinanceYear.aspx";
            Printcontrol.loadspreaddetails(Fpspread1, pagename, degreedetails);
            Printcontrol.Visible = true;
        }
        catch
        {

        }
    }

    protected void bttngo_Click(object sender, EventArgs e)
    {
        Printcontrol.Visible = false;
        btns.Visible = false;
        try
        {
            string acccode = "";
            for (int i = 0; i < cblaccname.Items.Count; i++)
            {
                if (cblaccname.Items[i].Selected == true)
                {
                    if (acccode == "")
                    {
                        acccode = "" + cblaccname.Items[i].Value.ToString() + "";
                    }
                    else
                    {
                        acccode = acccode + "'" + "," + "'" + cblaccname.Items[i].Value.ToString() + "";
                    }
                }

            }

            if (acccode.Trim() != "")
            {
                string selectqurey = "";
                selectqurey = "select FinYearPK,FinYearAcr,FinYearName,(CONVERT(varchar(10), FinYearStart,103)+' - '+CONVERT(varchar(10), FinYearEnd,103))as Finyear,CollegeCode from FM_FinYearMaster where CollegeCode ='" + collegestat0 + "' AND FinYearPK in('" + acccode + "')";
                ds.Clear();
                ds = d2.select_method_wo_parameter(selectqurey, "Text");
                if (ds.Tables[0].Rows.Count > 0)
                {
                    Fpspread1.Sheets[0].RowCount = 0;
                    Fpspread1.Sheets[0].ColumnCount = 0;
                    Fpspread1.CommandBar.Visible = false;
                    Fpspread1.Sheets[0].AutoPostBack = false;
                    Fpspread1.Sheets[0].ColumnHeader.RowCount = 1;
                    Fpspread1.Sheets[0].RowHeader.Visible = false;
                    Fpspread1.Sheets[0].ColumnCount = 5;

                    FarPoint.Web.Spread.StyleInfo darkstyle = new FarPoint.Web.Spread.StyleInfo();
                    darkstyle.BackColor = ColorTranslator.FromHtml("#0CA6CA");
                    darkstyle.ForeColor = Color.Black;
                    Fpspread1.ActiveSheetView.ColumnHeader.DefaultStyle = darkstyle;

                    Fpspread1.Sheets[0].ColumnHeader.Cells[0, 0].Text = "S.No";
                    Fpspread1.Sheets[0].ColumnHeader.Cells[0, 0].Font.Bold = true;
                    Fpspread1.Sheets[0].ColumnHeader.Cells[0, 0].Font.Name = "Book Antiqua";
                    // Fpspread1.Sheets[0].ColumnHeader.Cells[0, 0].ForeColor = System.Drawing.Color.Black;
                    Fpspread1.Sheets[0].ColumnHeader.Cells[0, 0].Font.Size = FontUnit.Medium;
                    Fpspread1.Sheets[0].ColumnHeader.Cells[0, 0].HorizontalAlign = HorizontalAlign.Center;
                    Fpspread1.Columns[0].Width = 65;
                    Fpspread1.Columns[0].Locked = true;

                    Fpspread1.Sheets[0].ColumnHeader.Cells[0, 1].Text = "Account Name";
                    Fpspread1.Sheets[0].ColumnHeader.Cells[0, 1].Font.Bold = true;
                    Fpspread1.Sheets[0].ColumnHeader.Cells[0, 1].Font.Name = "Book Antiqua";
                    // Fpspread1.Sheets[0].ColumnHeader.Cells[0, 1].ForeColor = System.Drawing.Color.Black;
                    Fpspread1.Sheets[0].ColumnHeader.Cells[0, 1].Font.Size = FontUnit.Medium;
                    Fpspread1.Sheets[0].ColumnHeader.Cells[0, 1].HorizontalAlign = HorizontalAlign.Center;
                    Fpspread1.Columns[1].Width = 300;
                    Fpspread1.Columns[1].Locked = true;

                    Fpspread1.Sheets[0].ColumnHeader.Cells[0, 2].Text = "Account Acronym";
                    Fpspread1.Sheets[0].ColumnHeader.Cells[0, 2].Font.Bold = true;
                    Fpspread1.Sheets[0].ColumnHeader.Cells[0, 2].Font.Name = "Book Antiqua";
                    //Fpspread1.Sheets[0].ColumnHeader.Cells[0, 2].ForeColor = System.Drawing.Color.Black;
                    Fpspread1.Sheets[0].ColumnHeader.Cells[0, 2].Font.Size = FontUnit.Medium;
                    Fpspread1.Sheets[0].ColumnHeader.Cells[0, 2].HorizontalAlign = HorizontalAlign.Center;
                    Fpspread1.Columns[2].Width = 100;
                    Fpspread1.Columns[2].Locked = true;

                    Fpspread1.Sheets[0].ColumnHeader.Cells[0, 3].Text = "Financial Year";
                    Fpspread1.Sheets[0].ColumnHeader.Cells[0, 3].Font.Bold = true;
                    Fpspread1.Sheets[0].ColumnHeader.Cells[0, 3].Column.Width = 150;
                    Fpspread1.Sheets[0].ColumnHeader.Cells[0, 3].Font.Name = "Book Antiqua";
                    // Fpspread1.Sheets[0].ColumnHeader.Cells[0, 3].ForeColor = System.Drawing.Color.Black;
                    Fpspread1.Sheets[0].ColumnHeader.Cells[0, 3].Font.Size = FontUnit.Medium;
                    Fpspread1.Sheets[0].ColumnHeader.Cells[0, 3].HorizontalAlign = HorizontalAlign.Center;
                    Fpspread1.Columns[3].Width = 250;
                    Fpspread1.Columns[3].Locked = true;

                    Fpspread1.Sheets[0].ColumnHeader.Cells[0, 4].Text = "Select";
                    Fpspread1.Sheets[0].ColumnHeader.Cells[0, 4].Font.Bold = true;
                    Fpspread1.Sheets[0].ColumnHeader.Cells[0, 4].Font.Name = "Book Antiqua";
                    //Fpspread1.Sheets[0].ColumnHeader.Cells[0, 4].ForeColor = System.Drawing.Color.Black;
                    Fpspread1.Sheets[0].ColumnHeader.Cells[0, 4].Font.Size = FontUnit.Medium;
                    Fpspread1.Sheets[0].ColumnHeader.Cells[0, 4].HorizontalAlign = HorizontalAlign.Center;
                    Fpspread1.Columns[4].Width = 65;

                    FarPoint.Web.Spread.CheckBoxCellType chkall = new FarPoint.Web.Spread.CheckBoxCellType();
                    chkall.AutoPostBack = true;

                    string acctid = d2.getCurrentFinanceYear(usercode, collegestat0);

                    for (int row = 0; row < ds.Tables[0].Rows.Count; row++)
                    {
                        Fpspread1.Sheets[0].RowCount++;
                        Fpspread1.Sheets[0].Cells[Fpspread1.Sheets[0].RowCount - 1, 0].Text = Convert.ToString(row + 1);
                        Fpspread1.Sheets[0].Cells[Fpspread1.Sheets[0].RowCount - 1, 0].HorizontalAlign = HorizontalAlign.Center;
                        Fpspread1.Sheets[0].Cells[Fpspread1.Sheets[0].RowCount - 1, 0].Font.Size = FontUnit.Medium;
                        Fpspread1.Sheets[0].Cells[Fpspread1.Sheets[0].RowCount - 1, 0].Font.Name = "Book Antiqua";

                        Fpspread1.Sheets[0].Cells[Fpspread1.Sheets[0].RowCount - 1, 1].Text = Convert.ToString(ds.Tables[0].Rows[row]["FinYearName"]);
                        Fpspread1.Sheets[0].Cells[Fpspread1.Sheets[0].RowCount - 1, 1].Tag = Convert.ToString(ds.Tables[0].Rows[row]["FinYearPK"]);
                        Fpspread1.Sheets[0].Cells[Fpspread1.Sheets[0].RowCount - 1, 1].HorizontalAlign = HorizontalAlign.Left;
                        Fpspread1.Sheets[0].Cells[Fpspread1.Sheets[0].RowCount - 1, 1].Font.Size = FontUnit.Medium;
                        Fpspread1.Sheets[0].Cells[Fpspread1.Sheets[0].RowCount - 1, 1].Font.Name = "Book Antiqua";

                        Fpspread1.Sheets[0].Cells[Fpspread1.Sheets[0].RowCount - 1, 2].Text = Convert.ToString(ds.Tables[0].Rows[row]["FinYearAcr"]);
                        Fpspread1.Sheets[0].Cells[Fpspread1.Sheets[0].RowCount - 1, 2].Tag = Convert.ToString(ds.Tables[0].Rows[row]["CollegeCode"]);
                        Fpspread1.Sheets[0].Cells[Fpspread1.Sheets[0].RowCount - 1, 2].HorizontalAlign = HorizontalAlign.Left;
                        Fpspread1.Sheets[0].Cells[Fpspread1.Sheets[0].RowCount - 1, 2].Font.Size = FontUnit.Medium;
                        Fpspread1.Sheets[0].Cells[Fpspread1.Sheets[0].RowCount - 1, 2].Font.Name = "Book Antiqua";

                        Fpspread1.Sheets[0].Cells[Fpspread1.Sheets[0].RowCount - 1, 3].Text = Convert.ToString(ds.Tables[0].Rows[row]["Finyear"]);
                        Fpspread1.Sheets[0].Cells[Fpspread1.Sheets[0].RowCount - 1, 3].HorizontalAlign = HorizontalAlign.Center;
                        Fpspread1.Sheets[0].Cells[Fpspread1.Sheets[0].RowCount - 1, 3].Font.Size = FontUnit.Medium;
                        Fpspread1.Sheets[0].Cells[Fpspread1.Sheets[0].RowCount - 1, 3].Font.Name = "Book Antiqua";

                        Fpspread1.Sheets[0].Cells[Fpspread1.Sheets[0].RowCount - 1, 4].CellType = chkall;
                        Fpspread1.Sheets[0].Cells[Fpspread1.Sheets[0].RowCount - 1, 4].HorizontalAlign = HorizontalAlign.Center;
                        Fpspread1.Sheets[0].Cells[Fpspread1.Sheets[0].RowCount - 1, 4].Font.Size = FontUnit.Medium;
                        Fpspread1.Sheets[0].Cells[Fpspread1.Sheets[0].RowCount - 1, 4].Font.Name = "Book Antiqua";

                        if (acctid.Trim() == Convert.ToString(ds.Tables[0].Rows[row]["FinYearPK"]))
                        {
                            countnew = 1;
                            Fpspread1.Sheets[0].Cells[Fpspread1.Sheets[0].RowCount - 1, 4].Value = 1;
                            for (int k = 0; k < Fpspread1.Columns.Count; k++)
                            {
                                Fpspread1.Sheets[0].Cells[Fpspread1.Sheets[0].RowCount - 1, k].BackColor = ColorTranslator.FromHtml("#00CC00");
                            }

                        }
                    }
                    Fpspread1.Visible = true;
                    rptprint.Visible = true;
                    //div1.Visible = true;
                    lblerr.Visible = false;
                    Fpspread1.Sheets[0].PageSize = Fpspread1.Sheets[0].RowCount;
                    Fpspread1.Width = 850;
                    Fpspread1.Height = 300;
                    btns.Visible = true;
                }
                else
                {
                    Fpspread1.Visible = false;
                    rptprint.Visible = false;
                    //div1.Visible = false;
                    lblerr.Visible = true;
                    lblerr.Text = "No Records Found";
                    //btns.Visible = false;
                }
            }
            else
            {
                Fpspread1.Visible = false;
                rptprint.Visible = false;
                //div1.Visible = false;
                lblerr.Visible = true;
                lblerr.Text = "No Records Found!";
                //btns.Visible = false;
            }
        }
        catch
        {

        }
    }
    protected void bttnNew_Click(object sender, EventArgs e)
    {
        Printcontrol.Visible = false;
        bindcollege();
        btnsave.Visible = true;
        btnupdate.Visible = false;
        btndelete.Visible = false;
        popper1.Visible = true;
        lbldateerr.Visible = false;
        txtdatestart.Enabled = true;
        txtdateend.Enabled = true;
        clear();
    }
    protected void cbselectall_CheckedChanged(object sender, EventArgs e)
    {

    }
    protected void chk_CheckedChanged(object sender, EventArgs e)
    {

    }
    //protected void lb2_Click(object sender, EventArgs e)
    //{
    //    try
    //    {
    //        Session.Abandon();
    //        Session.Clear();
    //        Session.RemoveAll();
    //        System.Web.Security.FormsAuthentication.SignOut();
    //        Response.Redirect("Default.aspx", false);
    //    }
    //    catch
    //    {
    //    }
    //}
    protected void btnExcel_Click(object sender, EventArgs e)
    {
        try
        {
            string reportname = txtexcelname.Text;
            if (reportname.ToString().Trim() != "")
            {
                d2.printexcelreport(Fpspread1, reportname);
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
    protected void imagebtnpopclose1_Click(object sender, ImageClickEventArgs e)
    {
        popper1.Visible = false;
    }
    protected void btnupdate_Click(object sender, EventArgs e)
    {
        try
        {
            Fpspread1.SaveChanges();
            bool modcheck = false;
            if (Fpspread1.Sheets[0].RowCount > 0)
            {
                for (int row = 0; row < Fpspread1.Sheets[0].RowCount; row++)
                {
                    int val = Convert.ToInt32(Fpspread1.Sheets[0].Cells[row, 4].Value);
                    if (val == 1)
                    {
                        lbldateerr.Visible = false;
                        string genaccid = Convert.ToString(Fpspread1.Sheets[0].Cells[Convert.ToInt32(row), 1].Tag);
                        string acr = txtacr.Text.ToString();

                        //  string collcode = Convert.ToString(Fpspread1.Sheets[0].Cells[Convert.ToInt32(row), 2].Tag);
                        string collcode = ddlcol.SelectedItem.Value;
                        ddlcol.SelectedValue = collcode;
                        string accname = txtacc.Text.ToString();
                        string yearstart = txtdatestart.Text;
                        string yearend = txtdateend.Text;
                        string[] split = yearstart.Split('/');
                        DateTime dt = Convert.ToDateTime(split[1] + "/" + split[0] + "/" + split[2]);
                        split = yearend.Split('/');
                        DateTime dt1 = Convert.ToDateTime(split[1] + "/" + split[0] + "/" + split[2]);

                        accname = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(accname);

                        string selquery = "select * from FM_FinYearMaster where FinYearStart='" + dt.ToString("MM/dd/yyyy") + "' and FinYearPK not in('" + genaccid + "') and CollegeCode='" + collegestat + "'";
                        ds.Clear();
                        ds = d2.select_method_wo_parameter(selquery, "Text");
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            lbldateerr.Visible = true;
                            lbldateerr.Text = "Financial Year Already Exist";
                        }
                        else if (dt == dt1)
                        {
                            lbldateerr.Visible = true;
                            lbldateerr.Text = "Year must be exactly one year";
                        }
                        else
                        {
                            string insquery = "update FM_FinYearMaster set FinYearAcr='" + acr + "',FinYearName='" + accname + "',FinYearStart='" + dt.ToString("MM/dd/yyyy") + "',FinYearEnd='" + dt1.ToString("MM/dd/yyyy") + "',CollegeCode='" + collegestat + "' where FinYearPK='" + genaccid + "' ";
                            int ins = d2.update_method_wo_parameter(insquery, "Text");
                            bindaccname();
                            bttngo_Click(sender, e);
                            popper1.Visible = false;
                            imgdiv2.Visible = true;
                            lbl_alerterr.Text = "Updated Sucessfully";
                        }
                    }
                }
            }
        }
        catch
        {

        }
    }

    protected void btnyes_Click(object sender, EventArgs e)
    {
        try
        {
            Fpspread1.SaveChanges();
            if (Fpspread1.Sheets[0].RowCount > 0)
            {
                for (int row = 0; row < Fpspread1.Sheets[0].RowCount; row++)
                {
                    int val = Convert.ToInt32(Fpspread1.Sheets[0].Cells[row, 4].Value);
                    if (val == 1)
                    {
                        string genaccid = Convert.ToString(Fpspread1.Sheets[0].Cells[Convert.ToInt32(row), 1].Tag);
                        string collcode = ddlcol.SelectedValue;
                        ddlcol.SelectedValue = collcode;

                        string select = d2.GetFunction("select distinct FinYearFK from FT_FeeAllot where FinYearFK ='" + genaccid + "'");
                        if (select.Trim() == "0")
                        {
                            string delquery = "DELETE from FM_FinYearMaster WHERE FinYearPK = '" + genaccid + "' AND CollegeCode = '" + collegestat + "'";
                            int count = d2.update_method_wo_parameter(delquery, "Text");
                            if (count != 0)
                            {
                                bindaccname();
                                bttngo_Click(sender, e);
                                popper1.Visible = false;
                                imgdiv2.Visible = true;
                                imgdiv1.Visible = false;
                                lblalert.Visible = false;
                                lbl_alerterr.Text = "Deleted Successfully";
                            }
                            else
                            {
                                imgdiv2.Visible = true;
                                imgdiv1.Visible = false;
                                lblalert.Visible = false;
                                lbl_alerterr.Text = "Financial year is in Use,Can not Delete!";
                            }
                        }
                        else
                        {
                            imgdiv2.Visible = true;
                            imgdiv1.Visible = false;
                            lblalert.Visible = false;
                            lbl_alerterr.Text = "Financial year is in Use,Can not Delete!";
                        }
                    }
                }
            }
        }
        catch
        {

        }
    }

    protected void btnno_Click(object sender, EventArgs e)
    {
        imgdiv1.Visible = false;
        lblalert.Visible = false;
    }

    protected void btndelete_Click(object sender, EventArgs e)
    {
        if (txtacc.Text.Trim() != "" && txtacr.Text.Trim() != "")
        {
            imgdiv1.Visible = true;
            lblalert.Visible = true;
            lblalert.Text = "Do You Want To Delete This Record?";
        }
        else
        {
            imgdiv2.Visible = true;
            lbl_alerterr.Text = "Enter Account Name And Acronym";
        }
    }

    protected void btnsave_Click(object sender, EventArgs e)
    {
        try
        {
            string accname = txtacc.Text.ToString();
            string acronym = txtacr.Text.ToUpper().ToString();
            int count = 0;
            string collcode = ddlcol.SelectedItem.Value;

            string yearstart = txtdatestart.Text;
            string yearend = txtdateend.Text;

            string[] split = yearstart.Split('/');
            DateTime dt = Convert.ToDateTime(split[1] + "/" + split[0] + "/" + split[2]);
            int year = dt.Year;
            DateTime currdt = DateTime.Now;
            int curryear = currdt.Year;
            split = yearend.Split('/');
            DateTime newdt = Convert.ToDateTime(split[1] + "/" + split[0] + "/" + split[2]);

            accname = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(accname);

            string seldatequery = "select FinYearStart from FM_FinYearMaster where CollegeCode='" + collegestat + "'";
            DataSet dsdate = new DataSet();
            dsdate.Clear();
            dsdate = d2.select_method_wo_parameter(seldatequery, "Text");
            if (dsdate.Tables.Count > 0)
            {
                if (dsdate.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < dsdate.Tables[0].Rows.Count; i++)
                    {
                        DateTime getdt = Convert.ToDateTime(dsdate.Tables[0].Rows[i]["FinYearStart"]);
                        int getyear = getdt.Year;
                        if (getyear == year)
                        {
                            count++;
                        }
                    }
                }
            }
            string selquery = "select * from FM_FinYearMaster where FinYearStart='" + dt.ToString("MM/dd/yyyy") + "' and CollegeCode='" + collegestat + "'";
            ds.Clear();
            ds = d2.select_method_wo_parameter(selquery, "Text");
            if (ds.Tables[0].Rows.Count > 0)
            {
                lbldateerr.Visible = true;
                lbldateerr.Text = "Financial Year Already Exists!";
            }
            else if (count > 0)
            {
                imgdiv2.Visible = true;
                lbl_alerterr.Visible = true;
                lbl_alerterr.Text = "Financial Year Already Exists!";
            }
            else if (dt == newdt)
            {
                lbldateerr.Visible = true;
                lbldateerr.Text = "Year Must Be Exactly One Year";
            }
            else
            {
                string chk = "select FinYearAcr,FinYearName from FM_FinYearMaster where (FinYearAcr='" + acronym + "' or FinYearName='" + accname + "') and CollegeCode='" + collegestat + "'";
                ds.Clear();
                ds = d2.select_method_wo_parameter(chk, "Text");
                if (ds.Tables[0].Rows.Count > 0)
                {
                    if (acronym == ds.Tables[0].Rows[0]["FinYearAcr"].ToString())
                    {
                        imgdiv2.Visible = true;
                        lbl_alerterr.Text = "Account Acronym Already Exists";
                    }
                    else if (accname == ds.Tables[0].Rows[0]["FinYearName"].ToString())
                    {
                        imgdiv2.Visible = true;
                        lbl_alerterr.Text = "Account Name Already Exists";
                    }
                }
                else
                {
                    string insertcol = "Insert into FM_FinYearMaster (FinYearAcr,FinYearName,FinYearStart,FinYearEnd,CollegeCode)";
                    insertcol = insertcol + "values('" + acronym.Trim().ToUpper() + "','" + accname.Trim() + "','" + dt.ToString("MM/dd/yyyy") + "','" + newdt.ToString("MM/dd/yyyy") + "','" + collegestat + "')";
                    int colins = d2.update_method_wo_parameter(insertcol, "Text");
                    bindaccname();
                    bttngo_Click(sender, e);
                    clear();
                    imgdiv2.Visible = true;
                    lbl_alerterr.Text = "Saved Successfully";
                    popper1.Visible = true;
                }
            }
        }
        catch
        {
        }
    }
    protected void btnexit_Click(object sender, EventArgs e)
    {
        popper1.Visible = false;
    }
    protected void Fpspread1_render(object sender, EventArgs e)
    {
        try
        {
            if (click == true)
            {
                bindcollege();
                bindloadcol();
                popper1.Visible = true;
                btnsave.Visible = false;
                btnupdate.Visible = true;
                btndelete.Visible = true;
                lbldateerr.Visible = false;
                string currrow = "";
                string currcol = "";
                currrow = Fpspread1.ActiveSheetView.ActiveRow.ToString();
                currcol = Fpspread1.ActiveSheetView.ActiveColumn.ToString();
                if (currrow.Trim() != "")
                {
                    string actid = Convert.ToString(Fpspread1.Sheets[0].Cells[Convert.ToInt32(currrow), 1].Tag);
                    string colcode = Convert.ToString(Fpspread1.Sheets[0].Cells[Convert.ToInt32(currrow), 2].Tag);
                    string selectload = "select FinYearAcr,FinYearName,(Convert(Varchar(10),FinYearStart,103)) as finyearstart,(Convert(varchar(10),FinYearEnd,103)) as finyearend from FM_FinYearMaster where and FinYearPK='" + actid + "' and CollegeCode='" + colcode + "'";
                    ds.Clear();
                    ds = d2.select_method_wo_parameter(selectload, "Text");
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        ddlcol.SelectedValue = colcode;
                        ddlcolload.SelectedValue = colcode;
                        txtacr.Text = ds.Tables[0].Rows[0]["FinYearAcr"].ToString();
                        txtacc.Text = ds.Tables[0].Rows[0]["FinYearName"].ToString();
                        txtdatestart.Text = ds.Tables[0].Rows[0]["finyearstart"].ToString();
                        txtdateend.Text = ds.Tables[0].Rows[0]["finyearend"].ToString();
                    }
                }
            }
        }
        catch
        {
        }
    }
    protected void Cellcontent_Click(object sender, EventArgs e)
    {
        try
        {
            click = true;
        }
        catch
        {
        }
    }

    protected void bindcollege()
    {
        try
        {
            string strUser = d2.getUserCode(Convert.ToString(Session["group_code"]), Convert.ToString(Session["usercode"]),1);
            ds.Clear();
            ddlcol.Items.Clear();
            string query = "select cp.college_code,cf.collname from collegeprivilages cp,collinfo cf where " + strUser + " and cp.college_code=cf.college_code";
            ds = d2.select_method_wo_parameter(query, "Text");
            if (ds.Tables[0].Rows.Count > 0)
            {
                ddlcol.DataSource = ds;
                ddlcol.DataTextField = "collname";
                ddlcol.DataValueField = "college_code";
                ddlcol.DataBind();

                if (ddlcol.Items.Count > 0)
                {
                    collegestat = ddlcol.SelectedItem.Value.ToString();
                }
            }
        }
        catch
        {
        }
    }

    protected void clear()
    {
        try
        {
            ddlcol.SelectedIndex = 0;
            txtacc.Text = "";
            txtacr.Text = "";
            txtdatestart.Text = DateTime.Now.ToString("dd/MM/yyyy");
            txtdateend.Text = DateTime.Now.ToString("dd/MM/yyyy");
            collegestat = "";
        }
        catch
        {
        }
    }
    protected void ddlcol_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            collegestat = ddlcol.SelectedItem.Value.ToString();
            lbldateerr.Visible = false;
            ds.Clear();
            if (ddlcol.SelectedItem.Text != "Select")
            {
                //string selectquery = "select address1,address2,address3 ,district,state,pincode ,phoneno,faxno,website,email from collinfo where college_code = '" + ddlcol.SelectedItem.Value + "'";
                //ds = d2.select_method_wo_parameter(selectquery, "Text");
                //if (ds.Tables[0].Rows.Count > 0)
                //{

                //}
            }
            else
            {
                clear();
            }
        }
        catch
        {
        }
    }
    protected void txtdateend_Change(object sender, EventArgs e)
    {
        try
        {
            string yearstart = txtdatestart.Text;
            string yearend = txtdateend.Text;
            string[] split = yearstart.Split('/');
            DateTime dt = Convert.ToDateTime(split[1] + "/" + split[0] + "/" + split[2]);
            DateTime newdt = dt.AddMonths(12).AddDays(-1);
            txtdateend.Text = newdt.ToString("dd/MM/yyyy");

            int startyear = dt.Year;
            if (newdt < dt)
            {
                lbldateerr.Visible = true;
                lbldateerr.Text = "End Date Less Than Start Date";
            }
            else
            {
                lbldateerr.Visible = false;
            }
        }
        catch
        {

        }
    }
    protected void txtdatestart_Change(object sender, EventArgs e)
    {
        try
        {
            string yearstart = txtdatestart.Text;
            string yearend = txtdateend.Text;
            string[] split = yearstart.Split('/');
            DateTime dt = Convert.ToDateTime(split[1] + "/" + split[0] + "/" + split[2]);
            int year = dt.Year;
            DateTime currdt = DateTime.Now;
            int curryear = currdt.Year;
            DateTime newdt = dt.AddMonths(12).AddDays(-1);
            txtdateend.Text = newdt.ToString("dd/MM/yyyy");
            int count = 0;

            string selquery = "select * from FM_FinYearMaster where FinYearStart='" + dt.ToString("MM/dd/yyyy") + "' and CollegeCode='" + collegestat + "'";
            selquery = selquery + " select FinYearEnd from FM_FinYearMaster where CollegeCode='" + collegestat + "'";
            ds.Clear();
            ds = d2.select_method_wo_parameter(selquery, "Text");
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    lbldateerr.Visible = true;
                    lbldateerr.Text = "Financial Year Already Exist!";
                }
                else
                {
                    lbldateerr.Visible = false;
                }
                if (ds.Tables[1].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                    {
                        //string enddate = Convert.ToString(ds.Tables[1].Rows[i]["FinYearEnd"]);
                        //split = enddate.Split('/');
                        //DateTime dtend = Convert.ToDateTime(split[0] + "/" + split[1] + "/" + split[2]);
                        //if (dtend > dt)
                        //{
                        //    count++;
                        //}
                    }
                    if (count > 0)
                    {
                        lbldateerr.Visible = true;
                        lbldateerr.Text = "Financial Year Already Exist!";
                        // txtdatestart.Text = DateTime.Now.ToString("dd/MM/yyyy");
                        // txtdateend.Text = DateTime.Now.ToString("dd/MM/yyyy");
                    }
                    else
                    {
                        lbldateerr.Visible = false;
                    }
                }
            }
        }
        catch
        {

        }
    }
    protected void cblaccname_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            string fnalyr = "";
            txt_accname.Text = "--Select--";
            cbaccname.Checked = false;
            int count = 0;
            for (int i = 0; i < cblaccname.Items.Count; i++)
            {
                if (cblaccname.Items[i].Selected == true)
                {
                    count = count + 1;
                    fnalyr = Convert.ToString(cblaccname.Items[i].Text);
                }
            }
            if (count > 0)
            {
                // txt_accname.Text = "Account Name(" + count.ToString() + ")";
                if (count == cblaccname.Items.Count)
                {
                    cbaccname.Checked = true;
                }
                if (count == 1)
                {
                    txt_accname.Text = "" + fnalyr + "";
                }
                else
                {
                    txt_accname.Text = "Account Name(" + (cblaccname.Items.Count) + ")";
                }
            }
        }
        catch
        {
        }
    }
    protected void cbaccname_CheckedChanged(object sender, EventArgs e)
    {
        try
        {
            string fnalyr = "";
            if (cbaccname.Checked == true)
            {
                for (int i = 0; i < cblaccname.Items.Count; i++)
                {
                    cblaccname.Items[i].Selected = true;
                    fnalyr = Convert.ToString(cblaccname.Items[i].Text);
                }
                //  txt_accname.Text = "Account Name(" + cblaccname.Items.Count + ")";
                if (cblaccname.Items.Count == 1)
                {
                    txt_accname.Text = "" + fnalyr + "";
                }
                else
                {
                    txt_accname.Text = "Account Name(" + (cblaccname.Items.Count) + ")";
                }
            }
            else
            {
                for (int i = 0; i < cblaccname.Items.Count; i++)
                {
                    cblaccname.Items[i].Selected = false;
                }
                txt_accname.Text = "--Select--";
            }
        }
        catch
        {
        }
    }
    protected void bindaccname()
    {
        try
        {
            ds.Clear();
            cblaccname.Items.Clear();
            //  string slctquery = "select distinct FinYearName,FinYearPK from FM_FinYearMaster where CollegeCode='" + ddlcolload.SelectedItem.Value + "'";
            string slctquery = " select distinct FinYearName,FinYearPK from FM_FinYearMaster FM,FS_FinYearPrivilage FP where fm.CollegeCode ='" + collegestat0 + "' and fm.FinYearPK =fp.FinYearFK and UserCode ='" + usercode + "'";
            ds = d2.select_method_wo_parameter(slctquery, "Text");
            if (ds.Tables[0].Rows.Count > 0)
            {
                cblaccname.DataSource = ds;
                cblaccname.DataTextField = "FinYearName";
                cblaccname.DataValueField = "FinYearPK";
                cblaccname.DataBind();
                if (cblaccname.Items.Count > 0)
                {
                    for (int i = 0; i < cblaccname.Items.Count; i++)
                    {
                        cblaccname.Items[i].Selected = true;
                    }
                    txt_accname.Text = "Account Name(" + cblaccname.Items.Count + ")";
                }
            }
        }
        catch
        {
        }
    }

    protected void ddlcolload_SelectedIndexChanged(object sender, EventArgs e)
    {
        bindaccname();
        bttngo_Click(sender, e);

    }

    protected void bindloadcol()
    {
        try
        {
            string strUser = d2.getUserCode(Convert.ToString(Session["group_code"]), Convert.ToString(Session["usercode"]),1);
            ds.Clear();
            ddlcolload.Items.Clear();
            string query = "select cp.college_code,cf.collname from collegeprivilages cp,collinfo cf where " + strUser + " and cp.college_code=cf.college_code";
            ds = d2.select_method_wo_parameter(query, "Text");
            if (ds.Tables[0].Rows.Count > 0)
            {
                ddlcolload.DataSource = ds;
                ddlcolload.DataTextField = "collname";
                ddlcolload.DataValueField = "college_code";
                ddlcolload.DataBind();
            }
        }
        catch
        {

        }
    }

    protected void Fpspread1_Command(object sender, FarPoint.Web.Spread.SpreadCommandEventArgs e)
    {
        try
        {
            Fpspread1.SaveChanges();
            string activerow = "";
            string activecol = "";
            activerow = Fpspread1.ActiveSheetView.ActiveRow.ToString();
            activecol = Fpspread1.ActiveSheetView.ActiveColumn.ToString();
            countnew = 0;

            if (activecol == "4")
            {
                for (int i = 0; i < Fpspread1.Sheets[0].Rows.Count; i++)
                {
                    if (Convert.ToInt32(Fpspread1.Sheets[0].Cells[Convert.ToInt32(i), Convert.ToInt32(activecol)].Value) == 1)
                    {
                        countnew++;
                    }
                    else
                    {
                        //countnew--;
                        for (int k = 0; k < Fpspread1.Columns.Count; k++)
                        {
                            Fpspread1.Sheets[0].Cells[Convert.ToInt32(i), k].BackColor = ColorTranslator.FromHtml("White");
                        }
                    }
                }
            }
            if (countnew > 1)
            {
                countnew--;
                Fpspread1.Sheets[0].Cells[Convert.ToInt32(activerow), Convert.ToInt32(activecol)].Value = 0;
                imgdiv2.Visible = true;
                lbl_alerterr.Text = "Only One Finance Year Is Allowed";
            }
        }
        catch
        {

        }
    }

    protected void btnSelect_Click(object sender, EventArgs e)
    {
        Fpspread1.SaveChanges();
        bool check = false;
        if (Fpspread1.Sheets[0].RowCount > 0)
        {
            for (int row = 0; row < Fpspread1.Sheets[0].RowCount; row++)
            {
                int val = Convert.ToInt32(Fpspread1.Sheets[0].Cells[row, 4].Value);
                if (val == 1)
                {
                    string actid = Convert.ToString(Fpspread1.Sheets[0].Cells[row, 1].Tag);
                    string insertquery = "if exists (select Linkvalue from InsSettings where LinkName = 'Current Financial Year' and college_code ='" + collegestat0 + "' and FinuserCode='" + usercode + "') update InsSettings set LinkValue ='" + actid.ToString() + "' where LinkName ='Current Financial Year' and college_code ='" + collegestat0 + "' and FinuserCode='" + usercode + "' else insert into  InsSettings (LinkName,LinkValue,college_code, FinuserCode) values('Current Financial Year','" + actid.ToString() + "','" + collegestat0 + "','" + usercode + "')";
                    int inscount = d2.update_method_wo_parameter(insertquery, "Text");
                    if (inscount > 0)
                    {
                        check = true;
                    }
                }

            }

            if (check == true)
            {
                lbl_alerterr.Text = "Financial Year Is Selected Successfully";
                imgdiv2.Visible = true;
                bttngo_Click(sender, e);
            }
            else
            {
                imgdiv2.Visible = true;
                lbl_alerterr.Text = "Please Select Any One Year";
            }
        }
    }

    protected void btnmod_Click(object sender, EventArgs e)
    {
        try
        {
            Fpspread1.SaveChanges();
            bool modcheck = false;
            if (Fpspread1.Sheets[0].RowCount > 0)
            {
                for (int row = 0; row < Fpspread1.Sheets[0].RowCount; row++)
                {
                    int val = Convert.ToInt32(Fpspread1.Sheets[0].Cells[row, 4].Value);
                    if (val == 1)
                    {
                        bindcollege();
                        // bindloadcol();
                        popper1.Visible = true;
                        btnsave.Visible = false;
                        btnupdate.Visible = true;
                        btndelete.Visible = true;
                        lbldateerr.Visible = false;

                        string curractid = Convert.ToString(Fpspread1.Sheets[0].Cells[row, 1].Tag);
                        string colcode = ddlcolload.SelectedValue;
                        string selectload = "select FinYearAcr,FinYearName,(Convert(Varchar(10),FinYearStart,103)) as finyearstart,(Convert(varchar(10),FinYearEnd,103)) as finyearend from FM_FinYearMaster where FinYearPK='" + curractid + "' and CollegeCode='" + collegestat0 + "'";
                        ds.Clear();
                        ds = d2.select_method_wo_parameter(selectload, "Text");
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            modcheck = true;
                            if (ddlcol.Items.Count > 0)
                            {
                                ddlcol.SelectedValue = colcode;
                                txtacr.Text = ds.Tables[0].Rows[0]["FinYearAcr"].ToString();
                                txtacc.Text = ds.Tables[0].Rows[0]["FinYearName"].ToString();
                                txtdatestart.Text = ds.Tables[0].Rows[0]["finyearstart"].ToString();
                                txtdatestart.Enabled = false;
                                txtdateend.Text = ds.Tables[0].Rows[0]["finyearend"].ToString();
                                txtdateend.Enabled = false;
                            }
                        }
                    }
                }
                if (modcheck == true)
                {

                }
                else
                {
                    imgdiv2.Visible = true;
                    lbl_alerterr.Text = "Please Select Any One Year";
                }
            }
        }
        catch
        {

        }
    }

    private void setLabelText()
    {
        string grouporusercode = string.Empty;
        if (Session["group_code"] != null && (Convert.ToString(Session["group_code"]).Trim() != "") && (Convert.ToString(Session["group_code"]).Trim() != "0") && (Convert.ToString(Session["group_code"]).Trim() != "-1"))
        {
            grouporusercode = " group_code=" + Convert.ToString(Session["group_code"]).Trim() + "";
        }
        else if (Session["usercode"] != null)
        {
            grouporusercode = " usercode=" + Convert.ToString(Session["usercode"]).Trim() + "";
        }
        List<Label> lbl = new List<Label>();
        List<byte> fields = new List<byte>();

        lbl.Add(lblcol);
        lbl.Add(lblCollege);
        fields.Add(0);
        fields.Add(0);
        new HeaderLabelText().setLabels(grouporusercode, ref lbl, fields);
    }
    // last modified 04-10-2016 sudhagar
}