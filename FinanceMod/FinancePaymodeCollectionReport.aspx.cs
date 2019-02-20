using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Collections;
using System.Drawing;

public partial class FinancePaymodeCollectionReport : System.Web.UI.Page
{
    DAccess2 d2 = new DAccess2();
    ReuasableMethods reuse = new ReuasableMethods();
    DataSet ds = new DataSet();
    string usercode = string.Empty;
    string collegecode = string.Empty;
    string singleuser = string.Empty;
    string group_user = string.Empty;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["collegecode"] == null)
            Response.Redirect("Default.aspx");
        usercode = Session["usercode"].ToString();
        singleuser = Session["single_user"].ToString();
        group_user = Session["group_code"].ToString();
        if (!IsPostBack)
        {
            loadmemtype();
            setLabelText();
            loadcollege();
            if (cblclg.Items.Count > 0)
                getCollegecode();
            loadcollege();
            bindsem();
            loadpaid();
            txt_fromdate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            txt_todate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            txt_fromdate.Attributes.Add("readonly", "readonly");
            txt_todate.Attributes.Add("readonly", "readonly");
            LoadIncludeSetting();
            //barath 15.03.17
            #region Excel print settings
            string usertype = "";
            if (usercode.Trim() != "")
                usertype = " and usercode='" + usercode + "'";
            else if (group_user.Trim() != "")
                usertype = " and group_code='" + group_user + "'";
            string printset = d2.GetFunction("select value from Master_Settings where settings='Excel and Pdf Print Settings' " + usertype + " ");
            if (printset != "")
            {
                if (printset.Contains("E"))
                {
                    lblrptname.Visible = true;
                    txtexcelname.Visible = true;
                    btnExcel.Visible = true;
                }
                if (printset.Contains("P"))
                {
                    btnprintmasterhed.Visible = true;
                }
                if (printset == "0")
                {
                    lblrptname.Visible = true;
                    txtexcelname.Visible = true;
                    btnExcel.Visible = true;
                    btnprintmasterhed.Visible = true;
                }
            }
            #endregion
        }
        if (cblclg.Items.Count > 0)
            getCollegecode();
    }

    #region college
    public void loadcollege()
    {
        try
        {
            cblclg.Items.Clear();
            cbclg.Checked = false;
            txtclg.Text = "---Select---";
            ds.Clear();
            string selectQuery = "select cp.college_code,cf.collname from collegeprivilages cp,collinfo cf where user_code=" + usercode + " and cp.college_code=cf.college_code";
            ds = d2.select_method_wo_parameter(selectQuery, "Text");
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                cblclg.DataSource = ds;
                cblclg.DataTextField = "collname";
                cblclg.DataValueField = "college_code";
                cblclg.DataBind();
                if (cblclg.Items.Count > 0)
                {
                    for (int i = 0; i < cblclg.Items.Count; i++)
                    {
                        cblclg.Items[i].Selected = true;
                    }
                    txtclg.Text = lblclg.Text + "(" + cblclg.Items.Count + ")";
                    cbclg.Checked = true;
                }
            }
        }
        catch { }
    }
    protected void cbclg_OnCheckedChanged(object sender, EventArgs e)
    {
        CallCheckboxChange(cbclg, cblclg, txtclg, lblclg.Text, "--Select--");
        getCollegecode();
        bindsem();
    }
    protected void cblclg_OnSelectedIndexChanged(object sender, EventArgs e)
    {
        CallCheckboxListChange(cbclg, cblclg, txtclg, lblclg.Text, "--Select--");
        getCollegecode();
        bindsem();
    }
    #endregion

    private string getCollegecode()
    {
        collegecode = string.Empty;
        if (cblclg.Items.Count > 0)
        {
            for (int i = 0; i < cblclg.Items.Count; i++)
            {
                if (cblclg.Items[i].Selected)
                {
                    if (collegecode == string.Empty)
                        collegecode = Convert.ToString(cblclg.Items[i].Value);
                    else
                        collegecode += "'" + "," + "'" + Convert.ToString(cblclg.Items[i].Value);
                }
            }
        }
        return collegecode;
    }

    #region sem
    protected void cb_sem_OnCheckedChanged(object sender, EventArgs e)
    {
        CallCheckboxChange(cb_sem, cbl_sem, txt_sem, lblsem.Text, "--Select--");
    }
    protected void cbl_sem_OnSelectedIndexChanged(object sender, EventArgs e)
    {
        CallCheckboxListChange(cb_sem, cbl_sem, txt_sem, lblsem.Text, "--Select--");

    }

    protected void bindsem()
    {
        try
        {

            cbl_sem.Items.Clear();
            cb_sem.Checked = false;
            txt_sem.Text = "--Select--";
            ds.Clear();
            string linkName = string.Empty;
            string cbltext = string.Empty;
            int clgCnt = 0;
            bool check = false;
            string collegecodes = string.Empty;
            if (cblclg.Items.Count > 0)
            {
                for (int i = 0; i < cblclg.Items.Count; i++)
                {
                    if (cblclg.Items[i].Selected)
                    {
                        if (collegecodes == string.Empty)
                        {
                            clgCnt++;
                            collegecodes = Convert.ToString(cblclg.Items[i].Value);
                        }
                        else
                        {
                            clgCnt++;
                            collegecodes += "','" + Convert.ToString(cblclg.Items[i].Value);
                        }
                    }
                }
            }
            if (clgCnt > 0)
                check = true;
            else
                check = false;
            //if (clgCnt == 1)
            //{
            //    ds = d2.loadFeecategory(collegecodes, usercode, ref linkName);
            //    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            //    {
            //        if (linkName == "Term")
            //        {
            //            string termStr = " and( textval like'" + linkName + " 1%' or textval like'" + linkName + " 2%' or textval like'" + linkName + " 3%' or textval like'" + linkName + " 4%' or textval like'" + linkName + " 5%' or textval like'" + linkName + " 6%') ";
            //            string selQ = " select  distinct  textval,textcode,len(isnull(textval,1000)) from textvaltable t where college_code='" + collegecodes + "' and textcriteria='FEECA' " + termStr + " order by len(isnull(textval,1000)),textval asc";
            //            DataSet dsval = d2.select_method_wo_parameter(selQ, "Text");
            //            if (dsval.Tables.Count > 0 && dsval.Tables[0].Rows.Count > 0)
            //            {
            //                cbl_sem.DataSource = dsval;
            //                cbl_sem.DataTextField = "TextVal";
            //                cbl_sem.DataValueField = "TextCode";
            //                cbl_sem.DataBind();
            //            }
            //        }
            //        else
            //        {
            //            cbl_sem.DataSource = ds;
            //            cbl_sem.DataTextField = "TextVal";
            //            cbl_sem.DataValueField = "TextCode";
            //            cbl_sem.DataBind();
            //        }
            //    }
            //    else
            //        check = true;
            //}
            //else
            //    check = true;
            // ds = d2.loadFeecategory(collegecode, usercode, ref linkName);
            if (check)
            {
                linkName = getLinkName();
                if (sclSett() == 0)
                {
                    if (linkName == "Term")
                    {
                        string termStr = " and( textval like'" + linkName + " 1%' or textval like'" + linkName + " 2%' or textval like'" + linkName + " 3%' or textval like'" + linkName + " 4%' or textval like'" + linkName + " 5%' or textval like'" + linkName + " 6%') ";
                        string selQ = " select  distinct  textval,len(isnull(textval,1000)) from textvaltable t where college_code in('" + collegecodes + "') and textcriteria='FEECA' " + termStr + " order by len(isnull(textval,1000)),textval asc";
                        DataSet dsval = d2.select_method_wo_parameter(selQ, "Text");
                        if (dsval.Tables.Count > 0 && dsval.Tables[0].Rows.Count > 0)
                        {
                            cbl_sem.DataSource = dsval;
                            cbl_sem.DataTextField = "TextVal";
                            cbl_sem.DataValueField = "TextVal";
                            cbl_sem.DataBind();
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(linkName))
                    {
                        string termStr = " and( textval like'1 " + linkName + "%' or textval like'2 " + linkName + "%' or textval like'3 " + linkName + "%' or textval like'4 " + linkName + "%' or textval like'5 " + linkName + "%' or textval like'6 " + linkName + "%' or textval like'7 " + linkName + "%' or textval like'8 " + linkName + "%' or textval like'9 " + linkName + "%' or textval like'10 " + linkName + "%') ";
                        string selQ = " select  distinct  textval,len(isnull(textval,1000)) from textvaltable t where college_code in('" + collegecodes + "') and textcriteria='FEECA' " + termStr + " order by len(isnull(textval,1000)),textval asc";
                        DataSet dsval = d2.select_method_wo_parameter(selQ, "Text");
                        if (dsval.Tables.Count > 0 && dsval.Tables[0].Rows.Count > 0)
                        {
                            cbl_sem.DataSource = dsval;
                            cbl_sem.DataTextField = "TextVal";
                            cbl_sem.DataValueField = "TextVal";
                            cbl_sem.DataBind();
                        }
                    }
                }
            }
            if (cbl_sem.Items.Count > 0)
            {
                for (int i = 0; i < cbl_sem.Items.Count; i++)
                {
                    cbl_sem.Items[i].Selected = true;
                    cbltext = Convert.ToString(cbl_sem.Items[i].Text);
                }
                if (cbl_sem.Items.Count == 1)
                    txt_sem.Text = "" + linkName + "(" + cbltext + ")";
                else
                    txt_sem.Text = "" + linkName + "(" + cbl_sem.Items.Count + ")";
                cb_sem.Checked = true;
            }
        }
        catch { }
    }

    public string getLinkName()
    {
        string linkName = string.Empty;
        try
        {
            string linkValue = string.Empty;
            linkValue = d2.GetFunction("select LinkValue from New_InsSettings where linkname = 'SemesterandYear' and user_code ='" + usercode + "' ");//and college_code ='" + collegecode + "'
            if (!string.IsNullOrEmpty(linkValue) && linkValue != "0")
                linkName = "SemesterandYear";
            else
            {
                linkValue = d2.GetFunction("select LinkValue from New_InsSettings where linkname = 'Fee Yearwise' and user_code ='" + usercode + "' ");//and college_code ='" + collegecode + "'
                if (!string.IsNullOrEmpty(linkValue) && linkValue == "0")
                    linkName = "Semester";
                else if (!string.IsNullOrEmpty(linkValue) && linkValue == "1")
                    linkName = "Year";
                else if (!string.IsNullOrEmpty(linkValue) && linkValue == "2")
                    linkName = "Term";
            }
        }
        catch { linkName = string.Empty; }
        return linkName;
    }

    //protected void bindsem()
    //{
    //    try
    //    {
    //        cbl_sem.Items.Clear();
    //        cb_sem.Checked = false;
    //        txt_sem.Text = "--Select--";
    //        string linkName = string.Empty;
    //        string cbltext = string.Empty;

    //        string SelQ = " select count(textcode),college_code from textvaltable where TextCriteria = 'FEECA'and textval like '%Term%' and textval not like '-1%' and college_code in('" + collegecode + "') group by college_code order by count(textcode) desc";
    //        DataSet dsval = d2.select_method_wo_parameter(SelQ, "Text");
    //        if (dsval.Tables.Count > 0 && dsval.Tables[0].Rows.Count > 0)
    //        {
    //            string colgcode = Convert.ToString(dsval.Tables[0].Rows[0]["college_code"]);
    //            // ds = d2.loadFeecategory(colgcode, usercode, ref linkName);
    //            string featDegcode = Convert.ToString(getDegreeCode(colgcode));
    //            //  d2.featDegreeCode = featDegcode;
    //            string SelectQ = "";
    //            ds.Clear();
    //            string linkValue = d2.GetFunction("select LinkValue from New_InsSettings where linkname = 'SemesterandYear' and user_code ='" + usercode + "'");
    //            if (!string.IsNullOrEmpty(linkValue) && linkValue != "0")
    //            {
    //                SelectQ = "select * from textvaltable where TextCriteria = 'FEECA'and (textval like '%Semester' or textval like '%Year') and textval not like '-1%' and college_code ='" + colgcode + "' order by len(textval),textval asc";
    //                ds.Clear();
    //                //ds = select_method_wo_parameter(SelectQ, "Text");
    //                linkName = "SemesterandYear";
    //            }
    //            else
    //            {
    //                linkValue = d2.GetFunction("select LinkValue from New_InsSettings where linkname = 'Fee Yearwise' and user_code ='" + usercode + "'");
    //                if (!string.IsNullOrEmpty(linkValue) && linkValue == "0")
    //                {
    //                    SelectQ = "select * from textvaltable where TextCriteria = 'FEECA'and textval like '%Semester' and textval not like '-1%' and college_code ='" + colgcode + "' order by len(textval),textval asc";
    //                    //dsset.Clear();
    //                    //dsset = select_method_wo_parameter(SelectQ, "Text");
    //                    linkName = "Semester";
    //                }
    //                else if (!string.IsNullOrEmpty(linkValue) && linkValue == "1")
    //                {
    //                    SelectQ = "select * from textvaltable where TextCriteria = 'FEECA'and textval like '%Year' and textval not like '-1%' and college_code ='" + colgcode + "' order by len(textval),textval asc";
    //                    //dsset.Clear();
    //                    //dsset = select_method_wo_parameter(SelectQ, "Text");
    //                    linkName = "Year";
    //                }
    //                else if (!string.IsNullOrEmpty(linkValue) && linkValue == "2")
    //                {
    //                    // SelectQ = "select * from textvaltable where TextCriteria = 'FEECA'and textval like '%Term%' and textval not like '-1%' and college_code ='" + colgcode + "' order by len(textval),textval asc";
    //                    SelectQ = "select distinct textval,TextCode,len(textval),t.college_code from textvaltable t,Fee_degree_match f where t.textcode=f.feecategory and t.college_code=f.college_code and  TextCriteria = 'FEECA' and textval like '%Term%' and textval not like '-1%' and t.college_code ='" + collegecode + "' ";
    //                    if (!string.IsNullOrEmpty(featDegcode))
    //                        SelectQ += "  and f.degree_code in('" + featDegcode + "') ";
    //                    SelectQ += " order by len(textval),textval asc";
    //                    //dsset.Clear();
    //                    //dsset = select_method_wo_parameter(SelectQ, "Text");
    //                    linkName = "Term";
    //                }
    //                //ds = d2.select_method_wo_parameter(SelectQ, "Text");
    //            }
    //            ds = d2.select_method_wo_parameter(SelectQ, "Text");
    //            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
    //            {
    //                cbl_sem.DataSource = ds;
    //                cbl_sem.DataTextField = "TextVal";
    //                cbl_sem.DataValueField = "TextCode";
    //                cbl_sem.DataBind();

    //                if (cbl_sem.Items.Count > 0)
    //                {
    //                    for (int i = 0; i < cbl_sem.Items.Count; i++)
    //                    {
    //                        cbl_sem.Items[i].Selected = true;
    //                        cbltext = Convert.ToString(cbl_sem.Items[i].Text);
    //                    }
    //                    if (cbl_sem.Items.Count == 1)
    //                        txt_sem.Text = "" + linkName + "(" + cbltext + ")";
    //                    else
    //                        txt_sem.Text = "" + linkName + "(" + cbl_sem.Items.Count + ")";
    //                    cb_sem.Checked = true;
    //                }
    //            }
    //        }
    //    }
    //    catch { }
    //}

    protected string getDegreeCode(string collegecode)
    {
        string degecode = string.Empty;
        string selqry = " select d.Degree_Code,(c.Course_Name +'-'+ dt.Dept_Name) as degreename,(c.Course_Name +'-'+dt.dept_acronym)as dept_acronym from Degree d,Department dt,Course c where d.Dept_Code =dt.Dept_Code and c.Course_Id =d.Course_Id and d.college_code ='" + collegecode + "'";
        DataSet dsdeg = d2.select_method_wo_parameter(selqry, "Text");
        if (dsdeg.Tables.Count > 0 && dsdeg.Tables[0].Rows.Count > 0)
        {
            for (int i = 0; i < dsdeg.Tables[0].Rows.Count; i++)
            {
                if (degecode == string.Empty)
                    degecode = Convert.ToString(dsdeg.Tables[0].Rows[i]["Degree_Code"]);
                else
                    degecode += "'" + "," + "'" + Convert.ToString(dsdeg.Tables[0].Rows[i]["Degree_Code"]);
            }
        }
        return degecode;
    }
    public void loadFeecategory(ref  Dictionary<string, string> feecatText, ref Dictionary<string, int> feecatValue)
    {
        DataSet dsset = new DataSet();
        try
        {
            if (cblclg.Items.Count > 0)
                collegecode = getCollegecode();
            // Dictionary<string, string> feecatText = new Dictionary<string, string>();
            string linkValue = string.Empty;
            string linkName = string.Empty;
            string SelectQ = string.Empty;
            string sem = Convert.ToString(getCblSelectedText(cbl_sem));
            string SelQ = " select count(textcode),college_code from textvaltable where TextCriteria = 'FEECA'and textval like '%Term%' and textval not like '-1%' and college_code in('" + collegecode + "') group by college_code order by count(textcode) desc";
            DataSet dsval = d2.select_method_wo_parameter(SelQ, "Text");
            if (dsval.Tables.Count > 0 && dsval.Tables[0].Rows.Count > 0)
            {
                string colgcode = Convert.ToString(dsval.Tables[0].Rows[0]["college_code"]);
                string featDegcode = Convert.ToString(getDegreeCode(colgcode));
                //ds = d2.loadFeecategory(colgcode, usercode, ref linkName);
                //if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                //{

                string linkValueNEW = d2.GetFunction("select LinkValue from New_InsSettings where linkname = 'SemesterandYear' and user_code ='" + usercode + "'");

                if (linkValueNEW == "1")
                {
                    SelectQ = "  select textcode,textval,college_code from textvaltable where TextCriteria = 'FEECA' and (textval in('" + sem + "')) and textval not like '-1%' and college_code in('" + collegecode + "') order by college_code,len(textval), textval asc";
                    dsset.Clear();
                    dsset = d2.select_method_wo_parameter(SelectQ, "Text");
                    feecat(dsset, ref feecatText, ref feecatValue);
                }
                else
                {
                    linkValueNEW = d2.GetFunction("select LinkValue from New_InsSettings where linkname = 'Fee Yearwise' and user_code ='" + usercode + "'");
                    if (linkValueNEW == "0")
                    {
                        SelectQ = "  select textcode,textval,college_code from textvaltable where TextCriteria = 'FEECA'and textval in('" + sem + "') and textval not like '-1%' and college_code in('" + collegecode + "') order by college_code,len(textval), textval asc";
                        dsset.Clear();
                        dsset = d2.select_method_wo_parameter(SelectQ, "Text");
                        feecat(dsset, ref feecatText, ref feecatValue);
                    }
                    else if (linkValueNEW == "1")
                    {
                        SelectQ = "  select textcode,textval,college_code from textvaltable where TextCriteria = 'FEECA'and textval in('" + sem + "') and textval not like '-1%' and college_code in('" + collegecode + "') order by college_code,len(textval), textval asc";
                        dsset.Clear();
                        dsset = d2.select_method_wo_parameter(SelectQ, "Text");
                        feecat(dsset, ref feecatText, ref feecatValue);
                    }
                    else if (linkValueNEW == "2")
                    {
                        //  SelectQ = "  select textcode,textval,college_code from textvaltable where TextCriteria = 'FEECA'and textval in('" + sem + "') and textval not like '-1%' and college_code in('" + collegecode + "') order by college_code,len(textval), textval asc";
                        SelectQ = "select distinct textval,TextCode,len(textval),t.college_code from textvaltable t,Fee_degree_match f where t.textcode=f.feecategory and t.college_code=f.college_code and  TextCriteria = 'FEECA' and textval like '%Term%' and textval not like '-1%' and t.college_code ='" + collegecode + "' ";
                        if (!string.IsNullOrEmpty(featDegcode))
                            SelectQ += "  and f.degree_code in('" + featDegcode + "') ";
                        SelectQ += " order by len(textval),textval asc";
                        dsset.Clear();
                        dsset = d2.select_method_wo_parameter(SelectQ, "Text");
                        feecat(dsset, ref feecatText, ref feecatValue);
                    }
                }
                // }
            }
        }
        catch { dsset.Clear(); }

    }

    //protected Dictionary<string, string> feecat(DataSet dsset, ref Dictionary<string, string> feecatText, ref  Dictionary<string, int> feecatValue)
    //{
    //    //Dictionary<string, string> feecatValue = new Dictionary<string, string>();
    //    // Dictionary<string, string> feecatText = new Dictionary<string, string>();
    //    for (int i = 0; i < dsset.Tables[0].Rows.Count; i++)
    //    {
    //        string txt = Convert.ToString(dsset.Tables[0].Rows[i]["textval"]);
    //        string val = Convert.ToString(dsset.Tables[0].Rows[i]["textcode"]);
    //        feecatValue.Add(val, Convert.ToInt32(dsset.Tables[0].Rows[i]["college_code"]));
    //        feecatText.Add(val, txt);
    //    }
    //    return feecatText;
    //}

    protected Dictionary<string, string> feecat(DataSet dsset, ref Dictionary<string, string> feecatText, ref  Dictionary<string, int> feecatValue)
    {
        //Dictionary<string, string> feecatValue = new Dictionary<string, string>();
        // Dictionary<string, string> feecatText = new Dictionary<string, string>();
        for (int i = 0; i < cbl_sem.Items.Count; i++)
        {
            if (cbl_sem.Items[i].Selected)
            {
                string txt = Convert.ToString(cbl_sem.Items[i].Text);
                string val = Convert.ToString(cbl_sem.Items[i].Value);
                feecatValue.Add(val, Convert.ToInt32(dsset.Tables[0].Rows[i]["college_code"]));
                feecatText.Add(val, txt);
            }
        }
        return feecatText;
    }

    #endregion

    #region paymentmode
    public void loadpaid()
    {
        try
        {
            chkl_paid.Items.Clear();
            //chkl_paid.Items.Add(new ListItem("Cash", "1"));
            //chkl_paid.Items.Add(new ListItem("Cheque", "2"));
            //chkl_paid.Items.Add(new ListItem("DD", "3"));
            //chkl_paid.Items.Add(new ListItem("Challan", "4"));
            //chkl_paid.Items.Add(new ListItem("Online", "5"));
            //chkl_paid.Items.Add(new ListItem(" Card", "6"));
            PaymodeToCheckboxList(chkl_paid, usercode, collegecode);
            if (chkl_paid.Items.Count > 0)
            {
                for (int i = 0; i < chkl_paid.Items.Count; i++)
                {
                    chkl_paid.Items[i].Selected = true;
                }
                txt_paid.Text = "Paid(" + chkl_paid.Items.Count + ")";
                chk_paid.Checked = true;
            }
        }
        catch
        {

        }

    }
    public void chk_paid_OnCheckedChanged(object sender, EventArgs e)
    {
        try
        {
            CallCheckboxChange(chk_paid, chkl_paid, txt_paid, "Paid", "--Select--");
        }
        catch
        { }
    }
    public void chkl_paid_OnSelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            CallCheckboxListChange(chk_paid, chkl_paid, txt_paid, "Paid", "--Select--");
        }
        catch
        { }
    }
    #endregion

    #region Common Checkbox and Checkboxlist Event

    private string getCblSelectedValue(CheckBoxList cblSelected)
    {
        System.Text.StringBuilder selectedvalue = new System.Text.StringBuilder();
        try
        {
            for (int sel = 0; sel < cblSelected.Items.Count; sel++)
            {
                if (cblSelected.Items[sel].Selected == true)
                {
                    if (selectedvalue.Length == 0)
                    {
                        selectedvalue.Append(Convert.ToString(cblSelected.Items[sel].Value));
                    }
                    else
                    {
                        selectedvalue.Append("','" + Convert.ToString(cblSelected.Items[sel].Value));
                    }
                }
            }
        }
        catch { cblSelected.Items.Clear(); }
        return selectedvalue.ToString();
    }
    private string getCblSelectedText(CheckBoxList cblSelected)
    {
        System.Text.StringBuilder selectedText = new System.Text.StringBuilder();
        try
        {
            for (int sel = 0; sel < cblSelected.Items.Count; sel++)
            {
                if (cblSelected.Items[sel].Selected == true)
                {
                    if (selectedText.Length == 0)
                    {
                        selectedText.Append(Convert.ToString(cblSelected.Items[sel].Text));
                    }
                    else
                    {
                        selectedText.Append("','" + Convert.ToString(cblSelected.Items[sel].Text));
                    }
                }
            }
        }
        catch { cblSelected.Items.Clear(); }
        return selectedText.ToString();
    }
    private void CallCheckboxChange(CheckBox cb, CheckBoxList cbl, TextBox txt, string dispst, string deft)
    {
        try
        {
            int sel = 0;
            string name = "";
            txt.Text = deft;
            if (cb.Checked == true)
            {
                for (sel = 0; sel < cbl.Items.Count; sel++)
                {
                    cbl.Items[sel].Selected = true;
                    name = Convert.ToString(cbl.Items[sel].Text);
                }
                if (cbl.Items.Count == 1)
                {
                    txt.Text = "" + name + "";
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
    private void CallCheckboxListChange(CheckBox cb, CheckBoxList cbl, TextBox txt, string dipst, string deft)
    {
        try
        {
            int sel = 0;
            int count = 0;
            string name = "";
            cb.Checked = false;
            for (sel = 0; sel < cbl.Items.Count; sel++)
            {
                if (cbl.Items[sel].Selected == true)
                {
                    count++;
                    name = Convert.ToString(cbl.Items[sel].Text);
                }
            }
            if (count > 0)
            {
                if (count == 1)
                {
                    txt.Text = "" + name + "";
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
        }
        catch { }
    }

    #endregion

    protected void btngo_Click(object sender, EventArgs e)
    {
        Dictionary<string, int> feecatValue = new Dictionary<string, int>();
        Dictionary<string, string> feecatText = new Dictionary<string, string>();
        //loadFeecategory(ref  feecatText, ref  feecatValue);
        ds.Clear();
        ds = dsloadDetails(feecatValue);
        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
        {
            spreadLoadDetailed();
            //spreadLoadDetailed(ds, feecatValue, feecatText);
        }
        else
        {
            lblvalidation1.Text = string.Empty;
            txtexcelname.Text = string.Empty;
            spreadDet.Visible = false;
            tblpaymode.Visible = false;
            print.Visible = false;
            lbl_alert.Text = "No Record Found";
            imgdiv2.Visible = true;

        }
        //  spreadLoadDetailed(ds);
    }

    protected DataSet dsloadDetails(Dictionary<string, int> feecatValue)
    {
        DataSet dsload = new DataSet();
        try
        {
            #region get Value
            string feecatg = string.Empty;
            if (feecatValue.Count > 0)
            {
                foreach (KeyValuePair<string, int> value in feecatValue)
                {
                    if (feecatg == string.Empty)
                        feecatg = value.Key.ToString();
                    else
                        feecatg += "'" + "," + "'" + value.Key.ToString();
                }
            }
            string sem = "";
            string paid = "";
            string SelQ = "";
            string strRecon = string.Empty;
            if (cblclg.Items.Count > 0)
                collegecode = getCollegecode();
            //  sem = Convert.ToString(getCblSelectedValue(cbl_sem));
            paid = Convert.ToString(getCblSelectedValue(chkl_paid));
            string memtype = Convert.ToString(getCblSelectedValue(cblmem));
            string fromdate = txt_fromdate.Text;
            string todate = txt_todate.Text;
            string[] frdate = fromdate.Split('/');
            if (frdate.Length == 3)
                fromdate = frdate[1].ToString() + "/" + frdate[0].ToString() + "/" + frdate[2].ToString();
            string[] tdate = todate.Split('/');
            if (tdate.Length == 3)
                todate = tdate[1].ToString() + "/" + tdate[0].ToString() + "/" + tdate[2].ToString();
            string strtype = string.Empty;
            string strReg = string.Empty;
            //" and r.cc=0  and r.Exam_Flag<>'debar' and r.DelFlag=0";
            if (cbbfrecon.Checked)
            {
                strRecon = " and ISNULL(IsCanceled,'0')<>'1'";
            }
            else//added by abarna 30.03.2018
            {
                if (receipt.Checked == true)
                {
                    //strRecon = " and ( f.IsDeposited='1' and ISNULL( f.IsBounced,0)=0 and ISNULL(IsCollected,0)=1)  and ISNULL(IsCanceled,'0')<>'1'";//
                }
                if (payment.Checked == true)
                {
                    // strRecon = "";//and isnull(debit,'0')>0
                }
            }

            string applynStr = " AND r.IsConfirm = 1 and isnull(is_enroll,'0')<>'2'";//AND Admission_Status = 1     
            for (int se = 0; se < cbl_sem.Items.Count; se++)
            {
                if (cbl_sem.Items[se].Selected)
                {
                    if (sem == string.Empty)
                        sem = " and( t.textval like'%" + cbl_sem.Items[se].Text + "%'";
                    else
                        sem += " or t.textval like'%" + cbl_sem.Items[se].Text + "%'";
                }
            }
            if (!string.IsNullOrEmpty(sem))
                sem += ")";
            strReg = getStudCategory();
            #endregion

            #region Query
            //if (memtype == "1" || memtype == "2" || memtype == "3" || memtype == "4")
            //{

            if (memtype.Contains("1"))
            {
                if (rbtype.SelectedItem.Text == "Detailed")
                {
                    SelQ = " select convert(varchar(10),Transdate,103) as Transdate,sum(debit) as debit,sum(credit) as credit,college_code   from (";
                    SelQ += " select convert(varchar(10),Transdate,103) as Transdate, debit,credit,r.college_code   from ft_findailytransaction f ,registration r,textvaltable t where r.app_no=f.app_no and f.feecategory=t.textcode and t.college_code = r.college_code and t.textcriteria='FEECA' and ISNULL(IsCanceled,'0')<>'1' " + sem + " and paymode in('" + paid + "') and memtype in('" + memtype + "') and r.college_code in('" + collegecode + "') " + strReg + " " + strRecon + " and  f.Transdate between '" + fromdate + "' and '" + todate + "'  and isnull(actualfinyearfk,'0')<>'0' ";
                    //SelQ += " union all  select " + selCol + " " + acdYear + " from ft_excessdet ex,ft_excessledgerdet f,registration r where ex.app_no=r.app_no and ex.excessdetpk=f.excessdetfk and ex.feecategory=f.feecategory and ex.excesstransdate between '" + fromdate + "' and '" + todate + "' and memtype='1' and r.college_code in('" + collegecode + "') and f.headerfk in('" + hdFK + "') and f.ledgerfk in('" + ldFK + "') and excesstransdate between '" + fromdate + "' and '" + todate + "' group by " + GrpselCol + " " + acdYearGp + "";//
                }
                else
                {
                    SelQ = " select convert(varchar(10),Transdate,103) as Transdate,sum(debit) as debit,sum(credit) as credit   from (";
                    SelQ += " select convert(varchar(10),Transdate,103) as Transdate, debit,credit   from ft_findailytransaction f ,registration r,textvaltable t where r.app_no=f.app_no and f.feecategory=t.textcode and t.college_code = r.college_code and t.textcriteria='FEECA' and ISNULL(IsCanceled,'0')<>'1' " + sem + " and paymode in('" + paid + "') and memtype in('" + memtype + "') " + strReg + " " + strRecon + " and  f.Transdate between '" + fromdate + "' and '" + todate + "'  and isnull(actualfinyearfk,'0')<>'0' ";//
                }
                if (rbtype.SelectedItem.Text == "Detailed")
                {
                    if (cbbeforeadm.Checked)
                    {
                        SelQ += " union all  select convert(varchar(10),Transdate,103) as Transdate,debit,credit,r.college_code   from ft_findailytransaction f ,applyn r,textvaltable t where r.app_no=f.app_no and f.feecategory=t.textcode and t.college_code = r.college_code and t.textcriteria='FEECA' " + strRecon + " " + sem + " and paymode in('" + paid + "') and r.college_code in('" + collegecode + "') and memtype in('" + memtype + "')   and f.Transdate between '" + fromdate + "' and '" + todate + "' " + applynStr + "  and isnull(actualfinyearfk,'0')<>'0'";// 
                    }
                    SelQ += ")tbl  group by Transdate,college_code ";
                }
                else
                {
                    if (cbbeforeadm.Checked)
                    {
                        SelQ += " union all  select convert(varchar(10),Transdate,103) as Transdate,debit,credit   from ft_findailytransaction f ,applyn r,textvaltable t where r.app_no=f.app_no and f.feecategory=t.textcode and t.college_code = r.college_code and t.textcriteria='FEECA' " + strRecon + " " + sem + " and paymode in('" + paid + "') and r.college_code in('" + collegecode + "') and memtype in('" + memtype + "')   and f.Transdate between '" + fromdate + "' and '" + todate + "' " + applynStr + "  and isnull(actualfinyearfk,'0')<>'0'";// 
                    }
                    SelQ += ")tbl  group by Transdate ";
                }

                // SelQ += " order by cast(transdate as datetime) asc ";
                SelQ += " select convert(varchar(10),Transdate,103) as Transdate,sum(debit) as debit,sum(credit) as credit,textval,paymode,college_code  from (";
                SelQ += " select convert(varchar(10),Transdate,103) as Transdate,debit,credit,t.textval,f.paymode,r.college_code  from ft_findailytransaction f ,registration r,textvaltable t where r.app_no=f.app_no and f.feecategory=t.textcode and t.college_code = r.college_code and t.textcriteria='FEECA' and ISNULL(IsCanceled,'0')<>'1' " + sem + " and paymode in('" + paid + "') and memtype in('" + memtype + "') and r.college_code in('" + collegecode + "') " + strReg + " " + strRecon + " and f.Transdate between '" + fromdate + "' and '" + todate + "'  and isnull(actualfinyearfk,'0')<>'0'";// 
                if (cbbeforeadm.Checked)
                {
                    SelQ += " union all select convert(varchar(10),Transdate,103) as Transdate,debit,credit,t.textval,f.paymode,r.college_code  from ft_findailytransaction f ,applyn r,textvaltable t where r.app_no=f.app_no and f.feecategory=t.textcode and t.college_code = r.college_code and t.textcriteria='FEECA' " + strRecon + " " + sem + " and paymode in('" + paid + "') and memtype in('" + memtype + "') and r.college_code in('" + collegecode + "')   and f.Transdate between '" + fromdate + "' and '" + todate + "' " + applynStr + "  and isnull(actualfinyearfk,'0')<>'0'";//and memtype in('" + memtype + "') 
                }
                SelQ += ") tbl group by Transdate,textval,paymode,college_code";
                // SelQ += " order by cast(transdate as datetime) asc ";
                SelQ += " select convert(varchar(10),Transdate,103) as Transdate,sum(debit) as debit,sum(credit) as credit,textval,paymode,college_code from(";
                SelQ += " select convert(varchar(10),Transdate,103) as Transdate,debit,credit,t.textval,f.paymode,r.college_code  from ft_findailytransaction f ,registration r,textvaltable t where r.app_no=f.app_no and f.feecategory=t.textcode and t.college_code = r.college_code and t.textcriteria='FEECA' and ( f.IsDeposited='1' and ISNULL( f.IsBounced,0)=0 and ISNULL(IsCollected,0)=1)  and ISNULL(IsCanceled,'0')<>'1' " + sem + " and paymode in('" + paid + "') and memtype in('" + memtype + "')   and r.college_code in('" + collegecode + "') " + strReg + " " + strRecon + " and f.Transdate between '" + fromdate + "' and '" + todate + "'  and isnull(actualfinyearfk,'0')<>'0'";//and memtype in('" + memtype + "')
                if (cbbeforeadm.Checked)
                {
                    SelQ += " union all select convert(varchar(10),Transdate,103) as Transdate,debit,credit,t.textval,f.paymode,r.college_code  from ft_findailytransaction f ,applyn r,textvaltable t where r.app_no=f.app_no and f.feecategory=t.textcode and t.college_code = r.college_code and t.textcriteria='FEECA' " + strRecon + " " + sem + " and paymode in('" + paid + "') and memtype in('" + memtype + "') and r.college_code in('" + collegecode + "')  " + applynStr + " and f.Transdate between '" + fromdate + "' and '" + todate + "'   and isnull(actualfinyearfk,'0')<>'0'";//and memtype in('" + memtype + "')
                }
                SelQ += ") tbl group by Transdate,textval,paymode,college_code ";
            }
            // SelQ += " order by cast(transdate as datetime) asc";
            //}
            if (memtype.Contains("2"))
            {
                SelQ += "select convert(varchar(10),Transdate,103) as Transdate,sum(debit) as debit,sum(credit) as credit,college_code,paymode from ( select convert(varchar(10),Transdate,103) as Transdate, debit,credit,sm.college_code,f.paymode  from ft_findailytransaction f ,staffmaster sm,staff_appl_master sa,stafftrans st where sm.appl_no=sa.appl_no and  sm.staff_code=st.staff_code and latestrec='1' and sm.college_code=sa.college_code and sa.appl_id=f.app_no and f.paymode in('" + paid + "') and memtype in('" + memtype + "') and sm.college_code in('" + collegecode + "')   and  f.Transdate between '" + fromdate + "' and '" + todate + "'  and isnull(actualfinyearfk,'0')<>'0' )tbl  group by Transdate,college_code,paymode";


                SelQ += " select convert(varchar(10),Transdate,103) as Transdate,sum(debit) as debit,sum(credit) as credit,paymode,college_code  from ( select convert(varchar(10),Transdate,103) as Transdate,debit,credit,f.paymode,sm.college_code  from ft_findailytransaction f,staffmaster sm,staff_appl_master sa,stafftrans st  where  sm.appl_no=sa.appl_no and  sm.staff_code=st.staff_code and latestrec='1' and sm.college_code=sa.college_code and sa.appl_id=f.app_no and f.paymode in('" + paid + "') and memtype in('" + memtype + "') and sm.college_code in('" + collegecode + "')   and  f.Transdate between '" + fromdate + "' and '" + todate + "'  and isnull(actualfinyearfk,'0')<>'0') tbl group by Transdate,paymode,college_code";
                SelQ += " select convert(varchar(10),Transdate,103) as Transdate,sum(debit) as debit,sum(credit) as credit,paymode,college_code from( select convert(varchar(10),Transdate,103) as Transdate,debit,credit,f.paymode,sm.college_code  from ft_findailytransaction f ,staffmaster sm,staff_appl_master sa,stafftrans st where sm.appl_no=sa.appl_no and  sm.staff_code=st.staff_code and latestrec='1' and sm.college_code=sa.college_code and sa.appl_id=f.app_no and f.paymode in('" + paid + "') and memtype in('" + memtype + "') and sm.college_code in('" + collegecode + "')   and  f.Transdate between '" + fromdate + "' and '" + todate + "'  and isnull(actualfinyearfk,'0')<>'0') tbl group by Transdate,paymode,college_code";
            }

            dsload.Clear();
            dsload = d2.select_method_wo_parameter(SelQ, "Text");
            #endregion
        }
        catch { dsload.Clear(); }
        return dsload;
    }

    protected void spreadLoadDetailed()
    {
        try
        {
            #region design

            spreadDet.Sheets[0].RowCount = 0;
            spreadDet.Sheets[0].ColumnCount = 0;
            spreadDet.CommandBar.Visible = false;
            spreadDet.Sheets[0].AutoPostBack = true;
            spreadDet.Sheets[0].ColumnHeader.RowCount = 3;
            spreadDet.Sheets[0].RowHeader.Visible = false;
            spreadDet.Sheets[0].ColumnCount = 2;
            FarPoint.Web.Spread.StyleInfo darkstyle = new FarPoint.Web.Spread.StyleInfo();
            darkstyle.BackColor = ColorTranslator.FromHtml("#0CA6CA");
            darkstyle.ForeColor = Color.White;
            spreadDet.ActiveSheetView.ColumnHeader.DefaultStyle = darkstyle;

            spreadDet.Sheets[0].ColumnHeader.Cells[0, 0].Text = "S.No";
            spreadDet.Sheets[0].ColumnHeader.Cells[0, 0].ForeColor = ColorTranslator.FromHtml("#000000");
            spreadDet.Sheets[0].ColumnHeader.Cells[0, 0].Font.Bold = true;
            spreadDet.Sheets[0].ColumnHeader.Cells[0, 0].Font.Name = "Book Antiqua";
            spreadDet.Sheets[0].ColumnHeader.Cells[0, 0].Font.Size = FontUnit.Medium;
            spreadDet.Sheets[0].ColumnHeader.Cells[0, 0].HorizontalAlign = HorizontalAlign.Center;
            spreadDet.Sheets[0].Columns[0].HorizontalAlign = HorizontalAlign.Center;

            spreadDet.Sheets[0].ColumnHeader.Cells[0, 1].Text = "Date";
            spreadDet.Sheets[0].ColumnHeader.Cells[0, 1].ForeColor = ColorTranslator.FromHtml("#000000");
            spreadDet.Sheets[0].ColumnHeader.Cells[0, 1].Font.Bold = true;
            spreadDet.Sheets[0].ColumnHeader.Cells[0, 1].Font.Name = "Book Antiqua";
            spreadDet.Sheets[0].ColumnHeader.Cells[0, 1].Font.Size = FontUnit.Medium;
            spreadDet.Sheets[0].ColumnHeader.Cells[0, 1].HorizontalAlign = HorizontalAlign.Center;
            spreadDet.Sheets[0].Columns[1].HorizontalAlign = HorizontalAlign.Left;
            spreadDet.Sheets[0].ColumnHeaderSpanModel.Add(0, 0, 3, 1);
            spreadDet.Sheets[0].ColumnHeaderSpanModel.Add(0, 1, 3, 1);
            string hdrTxtValue = "";
            Hashtable htcolindex = new Hashtable();
            Hashtable htPayCol = new Hashtable();
            //for (int i = 0; i < cbl_sem.Items.Count; i++)
            //{
            //    if (cbl_sem.Items[i].Selected)
            //    {
            int checkva = 0;
            //bool colsem = false;
            //int col = spreadDet.Sheets[0].ColumnCount++;
            //spreadDet.Sheets[0].ColumnHeader.Cells[0, spreadDet.Sheets[0].ColumnCount - 1].Text = Convert.ToString(cbl_sem.Items[i].Text);
            //spreadDet.Sheets[0].ColumnHeader.Cells[0, spreadDet.Sheets[0].ColumnCount - 1].Tag = Convert.ToString(cbl_sem.Items[i].Value);
            //hdrTxtValue = Convert.ToString(cbl_sem.Items[i].Text);
            //spreadDet.Sheets[0].ColumnHeader.Cells[0, spreadDet.Sheets[0].ColumnCount - 1].ForeColor = ColorTranslator.FromHtml("#000000");
            //spreadDet.Sheets[0].ColumnHeader.Cells[0, spreadDet.Sheets[0].ColumnCount - 1].Font.Bold = true;
            //spreadDet.Sheets[0].ColumnHeader.Cells[0, spreadDet.Sheets[0].ColumnCount - 1].Font.Name = "Book Antiqua";
            //spreadDet.Sheets[0].ColumnHeader.Cells[0, spreadDet.Sheets[0].ColumnCount - 1].Font.Size = FontUnit.Medium;
            //spreadDet.Sheets[0].ColumnHeader.Cells[0, spreadDet.Sheets[0].ColumnCount - 1].HorizontalAlign = HorizontalAlign.Center;
            //spreadDet.Sheets[0].Columns[spreadDet.Sheets[0].ColumnCount - 1].HorizontalAlign = HorizontalAlign.Right;
            //spreadDet.Sheets[0].ColumnHeaderSpanModel.Add(0, spreadDet.Sheets[0].ColumnCount - 1, 1, 2);

            int colcount = 0;
            //for (int s = 0; s < chkl_paid.Items.Count; s++)
            //{
            //    if (chkl_paid.Items[s].Selected == true)
            //    {
            //        checkva++;
            //        if (checkva > 1)
            //            spreadDet.Sheets[0].ColumnCount++;

            //        spreadDet.Sheets[0].ColumnHeader.Cells[1, spreadDet.Sheets[0].ColumnCount - 1].Text = Convert.ToString(chkl_paid.Items[s].Text);
            //        spreadDet.Sheets[0].ColumnHeader.Cells[1, spreadDet.Sheets[0].ColumnCount - 1].Tag = Convert.ToString(chkl_paid.Items[s].Value);
            //        htcolindex.Add(Convert.ToString(hdrTxtValue + "-" + chkl_paid.Items[s].Value), spreadDet.Sheets[0].ColumnCount - 1);
            //        if (!htPayCol.ContainsKey(chkl_paid.Items[s].Value))
            //            htPayCol.Add(Convert.ToString(chkl_paid.Items[s].Value), spreadDet.Sheets[0].ColumnCount - 1);
            //        spreadDet.Sheets[0].ColumnHeader.Cells[1, spreadDet.Sheets[0].ColumnCount - 1].ForeColor = ColorTranslator.FromHtml("#000000");
            //        spreadDet.Sheets[0].ColumnHeader.Cells[1, spreadDet.Sheets[0].ColumnCount - 1].Font.Bold = true;
            //        spreadDet.Sheets[0].ColumnHeader.Cells[1, spreadDet.Sheets[0].ColumnCount - 1].Font.Name = "Book Antiqua";
            //        spreadDet.Sheets[0].ColumnHeader.Cells[1, spreadDet.Sheets[0].ColumnCount - 1].Font.Size = FontUnit.Medium;
            //        spreadDet.Sheets[0].ColumnHeader.Cells[1, spreadDet.Sheets[0].ColumnCount - 1].HorizontalAlign = HorizontalAlign.Center;
            //        spreadDet.Sheets[0].Columns[spreadDet.Sheets[0].ColumnCount - 1].HorizontalAlign = HorizontalAlign.Right;

            //        colcount = spreadDet.Sheets[0].ColumnCount - 1;
            //        //spreadDet.Sheets[0].ColumnHeaderSpanModel.Add(1, colcount, 1, 2);
            //      // colsem = true;
            //        if (receipt.Checked == true)
            //        {
            //            spreadDet.Sheets[0].ColumnHeader.Cells[2, spreadDet.Sheets[0].ColumnCount - 1].Text = "Receipt";
            //            spreadDet.Sheets[0].ColumnHeader.Cells[2, spreadDet.Sheets[0].ColumnCount - 1].ForeColor = ColorTranslator.FromHtml("#000000");
            //            spreadDet.Sheets[0].ColumnHeader.Cells[2, spreadDet.Sheets[0].ColumnCount - 1].Font.Bold = true;
            //            spreadDet.Sheets[0].ColumnHeader.Cells[2, spreadDet.Sheets[0].ColumnCount - 1].Font.Name = "Book Antiqua";
            //            spreadDet.Sheets[0].ColumnHeader.Cells[2, spreadDet.Sheets[0].ColumnCount - 1].Font.Size = FontUnit.Medium;

            //        }
            //        if (payment.Checked == true)
            //        {
            //            spreadDet.Sheets[0].ColumnCount++;
            //            spreadDet.Sheets[0].ColumnHeader.Cells[2, spreadDet.Sheets[0].ColumnCount - 1].Text = "Payment";
            //            //spreadDet.Sheets[0].ColumnHeader.Cells[2, spreadDet.Sheets[0].ColumnCount - 1].ForeColor = ColorTranslator.FromHtml("#000000");
            //            spreadDet.Sheets[0].ColumnHeader.Cells[2, spreadDet.Sheets[0].ColumnCount - 1].Font.Bold = true;
            //            spreadDet.Sheets[0].ColumnHeader.Cells[2, spreadDet.Sheets[0].ColumnCount - 1].Font.Name = "Book Antiqua";
            //            spreadDet.Sheets[0].ColumnHeader.Cells[2, spreadDet.Sheets[0].ColumnCount - 1].Font.Size = FontUnit.Medium;
            //        }

            //    }
            //}
            //added by abarna 29.03.2018





            //-------------------------------
            //if (colsem)
            //{
            //    checkva++;
            //    spreadDet.Sheets[0].ColumnCount++;
            //    spreadDet.Sheets[0].ColumnHeader.Cells[1, spreadDet.Sheets[0].ColumnCount - 1].Text = "Total";
            //    htcolindex.Add(Convert.ToString(hdrTxtValue + "-" + "Total"), spreadDet.Sheets[0].ColumnCount - 1);
            //    spreadDet.Sheets[0].ColumnHeader.Cells[1, spreadDet.Sheets[0].ColumnCount - 1].ForeColor = ColorTranslator.FromHtml("#000000");
            //    spreadDet.Sheets[0].ColumnHeader.Cells[1, spreadDet.Sheets[0].ColumnCount - 1].Font.Bold = true;
            //    spreadDet.Sheets[0].ColumnHeader.Cells[1, spreadDet.Sheets[0].ColumnCount - 1].Font.Name = "Book Antiqua";
            //    spreadDet.Sheets[0].ColumnHeader.Cells[1, spreadDet.Sheets[0].ColumnCount - 1].Font.Size = FontUnit.Medium;
            //    spreadDet.Sheets[0].ColumnHeader.Cells[1, spreadDet.Sheets[0].ColumnCount - 1].HorizontalAlign = HorizontalAlign.Center;
            //    spreadDet.Sheets[0].Columns[spreadDet.Sheets[0].ColumnCount - 1].HorizontalAlign = HorizontalAlign.Right;
            //    spreadDet.Sheets[0].ColumnHeaderSpanModel.Add(0, col, 1, checkva);
            //    if (receipt.Checked == true)
            //    {
            //        spreadDet.Sheets[0].ColumnHeader.Cells[2, spreadDet.Sheets[0].ColumnCount - 1].Text = "Receipt";
            //        spreadDet.Sheets[0].ColumnHeader.Cells[2, spreadDet.Sheets[0].ColumnCount - 1].ForeColor = ColorTranslator.FromHtml("#000000");
            //        spreadDet.Sheets[0].ColumnHeader.Cells[2, spreadDet.Sheets[0].ColumnCount - 1].Font.Bold = true;
            //        spreadDet.Sheets[0].ColumnHeader.Cells[2, spreadDet.Sheets[0].ColumnCount - 1].Font.Name = "Book Antiqua";
            //        spreadDet.Sheets[0].ColumnHeader.Cells[2, spreadDet.Sheets[0].ColumnCount - 1].Font.Size = FontUnit.Medium;
            //    }
            //    if (payment.Checked == true)
            //    {
            //        spreadDet.Sheets[0].ColumnCount++;
            //        spreadDet.Sheets[0].ColumnHeader.Cells[2, spreadDet.Sheets[0].ColumnCount - 1].Text = "Payment";
            //        spreadDet.Sheets[0].ColumnHeader.Cells[2, spreadDet.Sheets[0].ColumnCount - 1].Font.Bold = true;
            //        spreadDet.Sheets[0].ColumnHeader.Cells[2, spreadDet.Sheets[0].ColumnCount - 1].Font.Name = "Book Antiqua";
            //        spreadDet.Sheets[0].ColumnHeader.Cells[2, spreadDet.Sheets[0].ColumnCount - 1].Font.Size = FontUnit.Medium;
            //    }
            //}
            ////    }
            //}

            //spreadDet.Sheets[0].ColumnCount++;
            //spreadDet.Sheets[0].ColumnHeader.Cells[0, spreadDet.Sheets[0].ColumnCount - 1].Text = "Total";
            //htcolindex.Add("Total", spreadDet.Sheets[0].ColumnCount - 1);
            //spreadDet.Sheets[0].ColumnHeader.Cells[0, spreadDet.Sheets[0].ColumnCount - 1].ForeColor = ColorTranslator.FromHtml("#000000");
            //spreadDet.Sheets[0].ColumnHeader.Cells[0, spreadDet.Sheets[0].ColumnCount - 1].Font.Bold = true;
            //spreadDet.Sheets[0].ColumnHeader.Cells[0, spreadDet.Sheets[0].ColumnCount - 1].Font.Name = "Book Antiqua";
            //spreadDet.Sheets[0].ColumnHeader.Cells[0, spreadDet.Sheets[0].ColumnCount - 1].Font.Size = FontUnit.Medium;
            //spreadDet.Sheets[0].ColumnHeader.Cells[0, spreadDet.Sheets[0].ColumnCount - 1].HorizontalAlign = HorizontalAlign.Center;
            //spreadDet.Sheets[0].Columns[spreadDet.Sheets[0].ColumnCount - 1].HorizontalAlign = HorizontalAlign.Right;
            //spreadDet.Sheets[0].ColumnHeaderSpanModel.Add(0, spreadDet.Sheets[0].ColumnCount - 1, 2, 1);
            //if (receipt.Checked == true)
            //{
            //    spreadDet.Sheets[0].ColumnHeader.Cells[2, spreadDet.Sheets[0].ColumnCount - 1].Text = "Receipt";
            //    spreadDet.Sheets[0].ColumnHeader.Cells[2, spreadDet.Sheets[0].ColumnCount - 1].ForeColor = ColorTranslator.FromHtml("#000000");
            //    spreadDet.Sheets[0].ColumnHeader.Cells[2, spreadDet.Sheets[0].ColumnCount - 1].Font.Bold = true;
            //    spreadDet.Sheets[0].ColumnHeader.Cells[2, spreadDet.Sheets[0].ColumnCount - 1].Font.Name = "Book Antiqua";
            //    spreadDet.Sheets[0].ColumnHeader.Cells[2, spreadDet.Sheets[0].ColumnCount - 1].Font.Size = FontUnit.Medium;
            //}
            //if (payment.Checked == true)
            //{
            //    spreadDet.Sheets[0].ColumnCount++;
            //    spreadDet.Sheets[0].ColumnHeader.Cells[2, spreadDet.Sheets[0].ColumnCount - 1].Text = "Payment";
            //    spreadDet.Sheets[0].ColumnHeader.Cells[2, spreadDet.Sheets[0].ColumnCount - 1].Font.Bold = true;
            //    spreadDet.Sheets[0].ColumnHeader.Cells[2, spreadDet.Sheets[0].ColumnCount - 1].Font.Name = "Book Antiqua";
            //    spreadDet.Sheets[0].ColumnHeader.Cells[2, spreadDet.Sheets[0].ColumnCount - 1].Font.Size = FontUnit.Medium;
            //}
            Hashtable htPayColFnl = new Hashtable();
            int check = 0;
            int paycols = spreadDet.Sheets[0].ColumnCount++;
            spreadDet.Sheets[0].ColumnHeader.Cells[0, spreadDet.Sheets[0].ColumnCount - 1].Text = "PayMode";
            spreadDet.Sheets[0].ColumnHeader.Cells[0, spreadDet.Sheets[0].ColumnCount - 1].ForeColor = ColorTranslator.FromHtml("#000000");
            spreadDet.Sheets[0].ColumnHeader.Cells[0, spreadDet.Sheets[0].ColumnCount - 1].Font.Bold = true;
            spreadDet.Sheets[0].ColumnHeader.Cells[0, spreadDet.Sheets[0].ColumnCount - 1].Font.Name = "Book Antiqua";
            spreadDet.Sheets[0].ColumnHeader.Cells[0, spreadDet.Sheets[0].ColumnCount - 1].Font.Size = FontUnit.Medium;
            spreadDet.Sheets[0].ColumnHeader.Cells[0, spreadDet.Sheets[0].ColumnCount - 1].HorizontalAlign = HorizontalAlign.Center;

            int checkvas = 0;
            int colFromSpanCnt = 0;

            for (int s = 0; s < chkl_paid.Items.Count; s++)
            {
                int colSpanCnt = 0;
                if (chkl_paid.Items[s].Selected == true)
                {
                    checkvas++;

                    if (checkvas > 1)
                        check = spreadDet.Sheets[0].ColumnCount++;
                    colFromSpanCnt = Convert.ToInt32(spreadDet.Sheets[0].ColumnCount - 1);
                    htPayColFnl.Add(Convert.ToString(chkl_paid.Items[s].Value), spreadDet.Sheets[0].ColumnCount - 1);
                    spreadDet.Sheets[0].ColumnHeader.Cells[1, spreadDet.Sheets[0].ColumnCount - 1].Text = Convert.ToString(chkl_paid.Items[s].Text);
                    spreadDet.Sheets[0].ColumnHeader.Cells[1, spreadDet.Sheets[0].ColumnCount - 1].Tag = Convert.ToString(chkl_paid.Items[s].Value);
                    spreadDet.Sheets[0].ColumnHeader.Cells[1, spreadDet.Sheets[0].ColumnCount - 1].ForeColor = ColorTranslator.FromHtml("#000000");
                    spreadDet.Sheets[0].ColumnHeader.Cells[1, spreadDet.Sheets[0].ColumnCount - 1].Font.Bold = true;
                    spreadDet.Sheets[0].ColumnHeader.Cells[1, spreadDet.Sheets[0].ColumnCount - 1].Font.Name = "Book Antiqua";
                    spreadDet.Sheets[0].ColumnHeader.Cells[1, spreadDet.Sheets[0].ColumnCount - 1].Font.Size = FontUnit.Medium;
                    spreadDet.Sheets[0].ColumnHeader.Cells[1, spreadDet.Sheets[0].ColumnCount - 1].HorizontalAlign = HorizontalAlign.Center;
                    spreadDet.Sheets[0].Columns[spreadDet.Sheets[0].ColumnCount - 1].HorizontalAlign = HorizontalAlign.Right;
                    if (receipt.Checked == true)
                    {
                        colSpanCnt++;
                        spreadDet.Sheets[0].ColumnHeader.Cells[2, spreadDet.Sheets[0].ColumnCount - 1].Text = "Receipt";
                        spreadDet.Sheets[0].ColumnHeader.Cells[2, spreadDet.Sheets[0].ColumnCount - 1].ForeColor = ColorTranslator.FromHtml("#000000");
                        spreadDet.Sheets[0].ColumnHeader.Cells[2, spreadDet.Sheets[0].ColumnCount - 1].Font.Bold = true;
                        spreadDet.Sheets[0].ColumnHeader.Cells[2, spreadDet.Sheets[0].ColumnCount - 1].Font.Name = "Book Antiqua";
                        spreadDet.Sheets[0].ColumnHeader.Cells[2, spreadDet.Sheets[0].ColumnCount - 1].Font.Size = FontUnit.Medium;
                    }
                    if (payment.Checked == true)
                    {
                        colSpanCnt++;
                        spreadDet.Sheets[0].ColumnCount++;
                        spreadDet.Sheets[0].ColumnHeader.Cells[2, spreadDet.Sheets[0].ColumnCount - 1].Text = "Payment";
                        //spreadDet.Sheets[0].ColumnHeader.Cells[2, spreadDet.Sheets[0].ColumnCount - 1].ForeColor = ColorTranslator.FromHtml("#000000");
                        spreadDet.Sheets[0].ColumnHeader.Cells[2, spreadDet.Sheets[0].ColumnCount - 1].Font.Bold = true;
                        spreadDet.Sheets[0].ColumnHeader.Cells[2, spreadDet.Sheets[0].ColumnCount - 1].Font.Name = "Book Antiqua";
                        spreadDet.Sheets[0].ColumnHeader.Cells[2, spreadDet.Sheets[0].ColumnCount - 1].Font.Size = FontUnit.Medium;
                    }
                }
                spreadDet.Sheets[0].ColumnHeaderSpanModel.Add(1, colFromSpanCnt, 1, colSpanCnt);
            }

            spreadDet.Sheets[0].ColumnHeaderSpanModel.Add(0, paycols, 1, spreadDet.Sheets[0].ColumnCount - 1);

            int grandTotcol = spreadDet.Sheets[0].ColumnCount++;
            //spreadDet.Sheets[0].ColumnCount++;
            spreadDet.Sheets[0].ColumnHeader.Cells[0, spreadDet.Sheets[0].ColumnCount - 1].Text = "Grand Total";
            spreadDet.Sheets[0].ColumnHeader.Cells[0, spreadDet.Sheets[0].ColumnCount - 1].ForeColor = ColorTranslator.FromHtml("#000000");
            spreadDet.Sheets[0].ColumnHeader.Cells[0, spreadDet.Sheets[0].ColumnCount - 1].Font.Bold = true;
            spreadDet.Sheets[0].ColumnHeader.Cells[0, spreadDet.Sheets[0].ColumnCount - 1].Font.Name = "Book Antiqua";
            spreadDet.Sheets[0].ColumnHeader.Cells[0, spreadDet.Sheets[0].ColumnCount - 1].Font.Size = FontUnit.Medium;
            spreadDet.Sheets[0].ColumnHeader.Cells[0, spreadDet.Sheets[0].ColumnCount - 1].HorizontalAlign = HorizontalAlign.Center;

            //spreadDet.Sheets[0].ColumnCount++;
            spreadDet.Sheets[0].ColumnHeader.Cells[1, spreadDet.Sheets[0].ColumnCount - 1].Text = "Total";
            //spreadDet.Sheets[0].ColumnHeader.Cells[1, spreadDet.Sheets[0].ColumnCount - 1].Tag = Convert.ToString(chkl_paid.Items[s].Value);
            spreadDet.Sheets[0].ColumnHeader.Cells[1, spreadDet.Sheets[0].ColumnCount - 1].ForeColor = ColorTranslator.FromHtml("#000000");
            spreadDet.Sheets[0].ColumnHeader.Cells[1, spreadDet.Sheets[0].ColumnCount - 1].Font.Bold = true;
            spreadDet.Sheets[0].ColumnHeader.Cells[1, spreadDet.Sheets[0].ColumnCount - 1].Font.Name = "Book Antiqua";
            spreadDet.Sheets[0].ColumnHeader.Cells[1, spreadDet.Sheets[0].ColumnCount - 1].Font.Size = FontUnit.Medium;
            spreadDet.Sheets[0].ColumnHeader.Cells[1, spreadDet.Sheets[0].ColumnCount - 1].HorizontalAlign = HorizontalAlign.Center;
            spreadDet.Sheets[0].Columns[spreadDet.Sheets[0].ColumnCount - 1].HorizontalAlign = HorizontalAlign.Right;

            if (receipt.Checked == true)
            {
                //colSpanCnt++;
                spreadDet.Sheets[0].ColumnHeader.Cells[2, spreadDet.Sheets[0].ColumnCount - 1].Text = "Receipt";
                spreadDet.Sheets[0].ColumnHeader.Cells[2, spreadDet.Sheets[0].ColumnCount - 1].ForeColor = ColorTranslator.FromHtml("#000000");
                spreadDet.Sheets[0].ColumnHeader.Cells[2, spreadDet.Sheets[0].ColumnCount - 1].Font.Bold = true;
                spreadDet.Sheets[0].ColumnHeader.Cells[2, spreadDet.Sheets[0].ColumnCount - 1].Font.Name = "Book Antiqua";
                spreadDet.Sheets[0].ColumnHeader.Cells[2, spreadDet.Sheets[0].ColumnCount - 1].Font.Size = FontUnit.Medium;
            }
            if (payment.Checked == true)
            {
                // colSpanCnt++;
                spreadDet.Sheets[0].ColumnCount++;
                spreadDet.Sheets[0].ColumnHeader.Cells[2, spreadDet.Sheets[0].ColumnCount - 1].Text = "Payment";
                //spreadDet.Sheets[0].ColumnHeader.Cells[2, spreadDet.Sheets[0].ColumnCount - 1].ForeColor = ColorTranslator.FromHtml("#000000");
                spreadDet.Sheets[0].ColumnHeader.Cells[2, spreadDet.Sheets[0].ColumnCount - 1].Font.Bold = true;
                spreadDet.Sheets[0].ColumnHeader.Cells[2, spreadDet.Sheets[0].ColumnCount - 1].Font.Name = "Book Antiqua";
                spreadDet.Sheets[0].ColumnHeader.Cells[2, spreadDet.Sheets[0].ColumnCount - 1].Font.Size = FontUnit.Medium;
            }
            //spreadDet.Sheets[0].ColumnCount++;
            //spreadDet.Sheets[0].ColumnHeader.Cells[0, spreadDet.Sheets[0].ColumnCount - 1].Text = "Total";
            //htcolindex.Add("Total", spreadDet.Sheets[0].ColumnCount - 1);
            //spreadDet.Sheets[0].ColumnHeader.Cells[0, spreadDet.Sheets[0].ColumnCount - 1].ForeColor = ColorTranslator.FromHtml("#000000");
            //spreadDet.Sheets[0].ColumnHeader.Cells[0, spreadDet.Sheets[0].ColumnCount - 1].Font.Bold = true;
            //spreadDet.Sheets[0].ColumnHeader.Cells[0, spreadDet.Sheets[0].ColumnCount - 1].Font.Name = "Book Antiqua";
            //spreadDet.Sheets[0].ColumnHeader.Cells[0, spreadDet.Sheets[0].ColumnCount - 1].Font.Size = FontUnit.Medium;
            //spreadDet.Sheets[0].ColumnHeader.Cells[0, spreadDet.Sheets[0].ColumnCount - 1].HorizontalAlign = HorizontalAlign.Center;
            //spreadDet.Sheets[0].Columns[spreadDet.Sheets[0].ColumnCount - 1].HorizontalAlign = HorizontalAlign.Right;
            //spreadDet.Sheets[0].ColumnHeaderSpanModel.Add(0, spreadDet.Sheets[0].ColumnCount - 1, 2, 1);

            spreadDet.Sheets[0].ColumnHeaderSpanModel.Add(1, grandTotcol, 1, spreadDet.Sheets[0].ColumnCount - 1);
            #endregion
            if (rbtype.SelectedItem.Text == "Detailed")
            {
                #region value
                Hashtable grandtotal = new Hashtable();
                Hashtable grandtotalPay = new Hashtable();
                Hashtable fnlgrandtotal = new Hashtable();
                Hashtable fnlgrandtotalPay = new Hashtable();
                Hashtable htPayAmt = new Hashtable();
                Hashtable htpaymentAmt = new Hashtable();
                string memtype = Convert.ToString(getCblSelectedValue(cblmem));
                int height = 0;
                int row = 0;
                int Memrow = 0;
                for (int mem = 0; mem < cblmem.Items.Count; mem++)
                {
                    if (!cblmem.Items[mem].Selected)
                        continue;
                    Memrow = spreadDet.Sheets[0].RowCount++;
                    string memText = Convert.ToString(cblmem.Items[mem].Value);
                    string memText1 = Convert.ToString(cblmem.Items[mem].Text);
                    spreadDet.Sheets[0].Cells[Memrow, 0].Text = Convert.ToString(memText1);
                    spreadDet.Sheets[0].SpanModel.Add(Memrow, 0, 1, spreadDet.Sheets[0].ColumnCount);
                    spreadDet.Sheets[0].Rows[Memrow].BackColor = Color.MediumVioletRed;
                    spreadDet.Sheets[0].Rows[Memrow].ForeColor = Color.White;
                    string strMemType = memText == "1" ? "Student" : memText == "2" ? "Staff" : memText == "3" ? "Vendor" : memText == "4" ? "Other" : "";
                    for (int i = 0; i < cblclg.Items.Count; i++)
                    {

                        bool clgbool = true;
                        bool clgchkbool = false;

                        int fstrowCnt = 0;
                        DataView dvclg = new DataView();
                        if (cblclg.Items[i].Selected)
                        {
                            if (memText == "1")
                            {
                                ds.Tables[0].DefaultView.RowFilter = "college_code='" + cblclg.Items[i].Value + "'";
                                dvclg = ds.Tables[0].DefaultView;
                            }
                            else if (memText == "2")
                            {
                                ds.Tables[3].DefaultView.RowFilter = "college_code='" + cblclg.Items[i].Value + "'";
                                dvclg = ds.Tables[3].DefaultView;
                            }
                            if (dvclg.Count > 0)
                            {
                                for (int dt = 0; dt < dvclg.Count; dt++)
                                {
                                    bool rowbool = false;
                                    bool cblbool = true;

                                    double fnlTotAmt = 0;
                                    double fnlcretotamt = 0;
                                    string date = Convert.ToString(dvclg[dt]["Transdate"]);
                                    //for (int sem = 0; sem < cbl_sem.Items.Count; sem++)
                                    //{
                                    //    if (cbl_sem.Items[sem].Selected)
                                    //    {
                                    //        string semStr = Convert.ToString(cbl_sem.Items[sem].Text);
                                    double totalAmount = 0;
                                    double cretotamount = 0;
                                    bool paybool = false;
                                    int curColCnt = 0;
                                    string payModeVal = string.Empty;
                                    for (int pay = 0; pay < chkl_paid.Items.Count; pay++)
                                    {
                                        DataView dvfee = new DataView();
                                        if (chkl_paid.Items[pay].Selected)
                                        {
                                            #region paymode

                                            payModeVal = Convert.ToString(chkl_paid.Items[pay].Value);
                                            if (memText == "1")
                                            {
                                                ds.Tables[1].DefaultView.RowFilter = "Transdate='" + date + "' and paymode='" + chkl_paid.Items[pay].Value + "' and college_code='" + cblclg.Items[i].Value + "'";
                                                dvfee = ds.Tables[1].DefaultView;
                                            }
                                            else if (memText == "2")
                                            {
                                                ds.Tables[4].DefaultView.RowFilter = "Transdate='" + date + "' and paymode='" + chkl_paid.Items[pay].Value + "' and college_code='" + cblclg.Items[i].Value + "'";
                                                dvfee = ds.Tables[4].DefaultView;
                                            }

                                            double paidamount = 0;
                                            double clrAmount = 0;
                                            double creditamount = 0;
                                            int.TryParse(Convert.ToString(htcolindex[payModeVal + "-" + chkl_paid.Items[pay].Value]), out curColCnt);
                                            if (dvfee.Count > 0)
                                            {
                                                DataTable dtval = dvfee.ToTable();
                                                double.TryParse(Convert.ToString(dtval.Compute("sum(debit)", "")), out paidamount);
                                                // double.TryParse(Convert.ToString(dvfee[0]["debit"]), out paidamount);
                                                double.TryParse(Convert.ToString(dtval.Compute("sum(credit)", "")), out creditamount);
                                                totalAmount += paidamount;
                                                cretotamount += creditamount;
                                                paybool = true;
                                                rowbool = true;
                                                clgchkbool = true;
                                            }
                                            if (clgbool)
                                                fstrowCnt = spreadDet.Sheets[0].RowCount++;
                                            if (cblbool)
                                                spreadDet.Sheets[0].RowCount++;
                                            if (!grandtotal.ContainsKey(curColCnt))
                                                grandtotal.Add(curColCnt, Convert.ToString(paidamount));
                                            else
                                            {
                                                double amount = 0;
                                                double.TryParse(Convert.ToString(grandtotal[curColCnt]), out amount);
                                                amount += paidamount;
                                                grandtotal.Remove(curColCnt);
                                                grandtotal.Add(curColCnt, Convert.ToString(amount));
                                            }
                                            if (!grandtotalPay.ContainsKey(curColCnt + 1))//payment for grandtot
                                                grandtotalPay.Add(curColCnt + 1, Convert.ToString(creditamount));
                                            else
                                            {
                                                double amount = 0;
                                                double.TryParse(Convert.ToString(grandtotalPay[curColCnt + 1]), out amount);
                                                amount += creditamount;
                                                grandtotalPay.Remove(curColCnt + 1);
                                                grandtotalPay.Add(curColCnt + 1, Convert.ToString(amount));
                                            }
                                            //paymode receipt amt get
                                            if (!htPayAmt.ContainsKey(chkl_paid.Items[pay].Value))
                                                htPayAmt.Add(chkl_paid.Items[pay].Value, Convert.ToString(paidamount));
                                            else
                                            {
                                                double amount = 0;
                                                double.TryParse(Convert.ToString(htPayAmt[chkl_paid.Items[pay].Value]), out amount);
                                                amount += paidamount;
                                                htPayAmt.Remove(chkl_paid.Items[pay].Value);
                                                htPayAmt.Add(chkl_paid.Items[pay].Value, Convert.ToString(amount));
                                            }

                                            //paymode payment amt get
                                            if (!htpaymentAmt.ContainsKey(chkl_paid.Items[pay].Value))
                                                htpaymentAmt.Add(chkl_paid.Items[pay].Value, Convert.ToString(creditamount));
                                            else
                                            {
                                                double amount = 0;
                                                double.TryParse(Convert.ToString(htpaymentAmt[chkl_paid.Items[pay].Value]), out amount);
                                                amount += creditamount;
                                                htpaymentAmt.Remove(chkl_paid.Items[pay].Value);
                                                htpaymentAmt.Add(chkl_paid.Items[pay].Value, Convert.ToString(amount));
                                            }

                                            if (payModeVal == "2" || payModeVal == "3")
                                            {
                                                DataView dvclr = new DataView();
                                                if (memText == "1")
                                                {
                                                    ds.Tables[2].DefaultView.RowFilter = "Transdate='" + date + "' and paymode='" + chkl_paid.Items[pay].Value + "' and college_code='" + cblclg.Items[i].Value + "'";
                                                    dvclr = ds.Tables[2].DefaultView;
                                                }
                                                else if (memText == "2")
                                                {
                                                    ds.Tables[4].DefaultView.RowFilter = "Transdate='" + date + "'  and paymode='" + chkl_paid.Items[pay].Value + "' and college_code='" + cblclg.Items[i].Value + "'";
                                                    dvclr = ds.Tables[2].DefaultView;
                                                }
                                                dvclr = ds.Tables[2].DefaultView;
                                                if (dvclr.Count > 0)
                                                {
                                                    DataTable dtvals = dvclr.ToTable();
                                                    double.TryParse(Convert.ToString(dtvals.Compute("sum(debit)", "")), out paidamount);
                                                    double.TryParse(Convert.ToString(dtvals.Compute("sum(credit)", "")), out creditamount);
                                                    // double.TryParse(Convert.ToString(dvclr[0]["debit"]), out clrAmount);
                                                }

                                                if (paidamount != 0)

                                                    spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt].Text = Convert.ToString(paidamount + "[" + clrAmount + "]");


                                                else

                                                    spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt].Text = "-";


                                                if (creditamount != 0)
                                                    spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt + 1].Text = Convert.ToString(creditamount);
                                                else
                                                    spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt + 1].Text = "-";


                                            }
                                            else
                                            {
                                                if (paidamount != 0)
                                                    spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt].Text = Convert.ToString(paidamount);
                                                else
                                                    spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt].Text = "-";

                                                if (creditamount != 0)
                                                    spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt + 1].Text = Convert.ToString(creditamount);
                                                else
                                                    spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt + 1].Text = "-";



                                            }
                                            cblbool = false;
                                            clgbool = false;
                                            if (payModeVal == "1")
                                            {
                                                spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt].ForeColor = Color.Red;
                                                spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt + 1].ForeColor = Color.Red;
                                            }
                                            else if (payModeVal == "2")
                                            {
                                                spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt].ForeColor = Color.Gray;
                                                spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt + 1].ForeColor = Color.Gray;
                                            }
                                            else if (payModeVal == "3")
                                            {
                                                spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt].ForeColor = Color.OrangeRed;
                                                spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt + 1].ForeColor = Color.OrangeRed;
                                            }
                                            else if (payModeVal == "4")
                                            {
                                                spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt].ForeColor = ColorTranslator.FromHtml("#90EE90");
                                                spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt + 1].ForeColor = ColorTranslator.FromHtml("#90EE90");
                                            }
                                            else if (payModeVal == "5")
                                            {
                                                spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt].ForeColor = ColorTranslator.FromHtml("#FAFAD2");
                                                spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt + 1].ForeColor = ColorTranslator.FromHtml("#FAFAD2");
                                            }
                                            spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt].Font.Bold = true;
                                            spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt + 1].Font.Bold = true;
                                            #endregion
                                        }
                                    }
                                    if (paybool)
                                    {
                                        // int.TryParse(Convert.ToString(htcolindex[Convert.ToString(payModeVal + "-" + "Total")]), out curColCnt);
                                        //   spreadDet.Sheets[0].ColumnCount -1;
                                        spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, spreadDet.Sheets[0].ColumnCount - 2].Text = Convert.ToString(totalAmount);
                                        spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, spreadDet.Sheets[0].ColumnCount - 1].Text = Convert.ToString(cretotamount);//abarna
                                        spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt].ForeColor = ColorTranslator.FromHtml("#CC00FF");
                                        fnlTotAmt += totalAmount;
                                        fnlcretotamt += cretotamount;//added by abarna
                                        if (!grandtotal.ContainsKey(curColCnt))
                                            grandtotal.Add(curColCnt, Convert.ToString(totalAmount));
                                        else
                                        {
                                            double amount = 0;
                                            double.TryParse(Convert.ToString(grandtotal[curColCnt]), out amount);
                                            amount += totalAmount;
                                            grandtotal.Remove(curColCnt);
                                            grandtotal.Add(curColCnt, Convert.ToString(amount));
                                        }
                                        if (!grandtotalPay.ContainsKey(curColCnt + 1))//added for payment
                                            grandtotalPay.Add(curColCnt + 1, Convert.ToString(cretotamount));
                                        else
                                        {
                                            double amount = 0;
                                            double.TryParse(Convert.ToString(grandtotalPay[curColCnt + 1]), out amount);
                                            amount += cretotamount;
                                            grandtotalPay.Remove(curColCnt + 1);
                                            grandtotalPay.Add(curColCnt + 1, Convert.ToString(amount));
                                        }
                                    }
                                    //    }
                                    //}
                                    if (rowbool)
                                    {
                                        //paymode details
                                        curColCnt = 0;
                                        int.TryParse(Convert.ToString(htcolindex[Convert.ToString("Total")]), out curColCnt);
                                        spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt].Text = Convert.ToString(fnlTotAmt);
                                        spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt + 1].Text = Convert.ToString(fnlcretotamt);//abarna
                                        if (!grandtotal.ContainsKey(curColCnt))
                                            grandtotal.Add(curColCnt, Convert.ToString(fnlTotAmt));
                                        else
                                        {
                                            double amount = 0;
                                            double.TryParse(Convert.ToString(grandtotal[curColCnt]), out amount);
                                            amount += fnlTotAmt;
                                            grandtotal.Remove(curColCnt);
                                            grandtotal.Add(curColCnt, Convert.ToString(amount));
                                        }

                                        if (!grandtotalPay.ContainsKey(curColCnt + 1))//added for payment
                                            grandtotalPay.Add(curColCnt + 1, Convert.ToString(fnlcretotamt));
                                        else
                                        {
                                            double amount = 0;
                                            double.TryParse(Convert.ToString(grandtotalPay[curColCnt + 1]), out amount);
                                            amount += fnlcretotamt;
                                            grandtotalPay.Remove(curColCnt + 1);
                                            grandtotalPay.Add(curColCnt + 1, Convert.ToString(amount));
                                        }

                                        for (int pay = 0; pay < chkl_paid.Items.Count; pay++)
                                        {
                                            if (chkl_paid.Items[pay].Selected)
                                            {
                                                payModeVal = Convert.ToString(chkl_paid.Items[pay].Value);
                                                curColCnt = 0;
                                                int.TryParse(Convert.ToString(htPayColFnl[payModeVal]), out curColCnt);
                                                double payAmt = 0;
                                                double.TryParse(Convert.ToString(htPayAmt[payModeVal]), out payAmt);

                                                double paymentAmt = 0;
                                                double.TryParse(Convert.ToString(htpaymentAmt[payModeVal]), out paymentAmt);

                                                spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt].Text = d2.numberformat(Convert.ToString(payAmt));
                                                spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt + 1].Text = d2.numberformat(Convert.ToString(paymentAmt));//abarna

                                                if (!grandtotal.ContainsKey(curColCnt))
                                                    grandtotal.Add(curColCnt, Convert.ToString(payAmt));
                                                else
                                                {
                                                    double amount = 0;
                                                    double.TryParse(Convert.ToString(grandtotal[curColCnt]), out amount);
                                                    amount += payAmt;
                                                    grandtotal.Remove(curColCnt);
                                                    grandtotal.Add(curColCnt, Convert.ToString(amount));
                                                }
                                                if (!grandtotalPay.ContainsKey(curColCnt + 1))//added for payment
                                                    grandtotalPay.Add(curColCnt + 1, Convert.ToString(paymentAmt));
                                                else
                                                {
                                                    double amount = 0;
                                                    double.TryParse(Convert.ToString(grandtotalPay[curColCnt + 1]), out amount);
                                                    amount += paymentAmt;
                                                    grandtotalPay.Remove(curColCnt + 1);
                                                    grandtotalPay.Add(curColCnt + 1, Convert.ToString(amount));
                                                }
                                                if (payModeVal == "1")
                                                    spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt].BackColor = ColorTranslator.FromHtml("#F08080");
                                                else if (payModeVal == "2")
                                                    spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt].BackColor = ColorTranslator.FromHtml("#D3D3D3");
                                                else if (payModeVal == "3")
                                                    spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt].BackColor = ColorTranslator.FromHtml("#FFA500");
                                                else if (payModeVal == "4")
                                                    spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt].BackColor = ColorTranslator.FromHtml("#90EE90");
                                                else if (payModeVal == "5")
                                                    spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt].BackColor = ColorTranslator.FromHtml("#FAFAD2");
                                            }
                                        }
                                        htPayAmt.Clear();
                                        htpaymentAmt.Clear();
                                        row++;
                                        Memrow++;
                                        spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, 0].Text = Convert.ToString(row);
                                        spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, 0].ForeColor = Color.Black;
                                        spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, 1].Text = Convert.ToString(date);
                                        spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, 1].ForeColor = Color.Black;
                                        height += 15;
                                    }
                                }
                            }
                        }
                        if (clgchkbool)
                        {
                            Memrow++;
                            spreadDet.Sheets[0].Cells[fstrowCnt, 0].Text = Convert.ToString(cblclg.Items[i].Text);
                            spreadDet.Sheets[0].SpanModel.Add(fstrowCnt, 0, 1, spreadDet.Sheets[0].ColumnCount - 1);
                            spreadDet.Sheets[0].Rows[fstrowCnt].BackColor = Color.Green;
                            spreadDet.Sheets[0].Rows[fstrowCnt].ForeColor = Color.White;

                            Memrow++;
                            spreadDet.Sheets[0].Rows.Count++;
                            spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].Rows.Count - 1, 0].Text = "Total";
                            spreadDet.Sheets[0].SpanModel.Add(spreadDet.Sheets[0].Rows.Count - 1, 0, 1, 2);
                            spreadDet.Sheets[0].Rows[spreadDet.Sheets[0].Rows.Count - 1].BackColor = Color.Gold;
                            double value = 0;
                            double value2 = 0;
                            double payamountreceipt = 0;
                            double payamountpayment = 0;
                            for (int j = 2; j < (spreadDet.Sheets[0].ColumnCount - 1); j++)
                            {
                                double.TryParse(Convert.ToString(grandtotal[j]), out value);
                                if (grandtotal.ContainsKey(j))
                                    spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].Rows.Count - 1, j].Text = d2.numberformat(Convert.ToString(value));//receipt
                                payamountreceipt += value;
                                double.TryParse(Convert.ToString(grandtotalPay[j + 1]), out value2);
                                if (grandtotalPay.ContainsKey(j + 1))
                                    spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].Rows.Count - 1, j + 1].Text = d2.numberformat(Convert.ToString(value2));//payment
                                payamountpayment += value2;
                                if (!fnlgrandtotal.ContainsKey(j))
                                    fnlgrandtotal.Add(j, Convert.ToString(value));
                                else
                                {
                                    double amount = 0;
                                    double.TryParse(Convert.ToString(fnlgrandtotal[j]), out amount);
                                    amount += value;
                                    fnlgrandtotal.Remove(j);
                                    fnlgrandtotal.Add(j, Convert.ToString(amount));
                                }
                                if (!fnlgrandtotalPay.ContainsKey(j + 1))//payment
                                    fnlgrandtotalPay.Add(j + 1, Convert.ToString(value2));
                                else
                                {
                                    double amount = 0;
                                    double.TryParse(Convert.ToString(fnlgrandtotalPay[j + 1]), out amount);
                                    amount += value2;
                                    fnlgrandtotalPay.Remove(j + 1);
                                    fnlgrandtotalPay.Add(j + 1, Convert.ToString(amount));
                                }
                            }
                            spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, spreadDet.Sheets[0].ColumnCount - 2].Text = Convert.ToString(payamountreceipt);
                            spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, spreadDet.Sheets[0].ColumnCount - 1].Text = Convert.ToString(payamountpayment);//abarna
                            grandtotal.Clear();
                            grandtotalPay.Clear();
                        }


                    }

                }

                #region grandtot
                // spreadDet.Sheets[0].PageSize = ds.Tables[0].Rows.Count + 1;
                spreadDet.Sheets[0].Rows.Count++;
                spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].Rows.Count - 1, 0].Text = "Grand Total";
                spreadDet.Sheets[0].SpanModel.Add(spreadDet.Sheets[0].Rows.Count - 1, 0, 1, 2);
                spreadDet.Sheets[0].Rows[spreadDet.Sheets[0].Rows.Count - 1].BackColor = Color.YellowGreen;
                spreadDet.Sheets[0].Rows[spreadDet.Sheets[0].Rows.Count - 1].ForeColor = Color.Black;
                double grandvalue = 0;
                double grandvaluePay = 0;
                double grandvaluereceipt = 0;
                double grandvaluepayment = 0;
                //for (int j = 2; j < (spreadDet.Sheets[0].ColumnCount - 1); j++)
                //{
                //    double.TryParse(Convert.ToString(fnlgrandtotal[j]), out grandvalue);
                //    spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].Rows.Count - 1, j].Text = d2.numberformat(Convert.ToString(grandvalue));
                //    grandvaluereceipt += grandvalue;
                //    //double.TryParse(Convert.ToString(fnlgrandtotalPay[j + 1]), out grandvaluePay);//payment
                //    //spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].Rows.Count - 1, j + 1].Text = d2.numberformat(Convert.ToString(grandvaluePay));
                //}
                //for (int j = 3; j < (spreadDet.Sheets[0].ColumnCount - 1); j = j + 2)
                //{
                //    double.TryParse(Convert.ToString(fnlgrandtotalPay[j]), out grandvaluePay);//payment
                //    spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].Rows.Count - 1, j + 1].Text = d2.numberformat(Convert.ToString(grandvaluePay));
                //    grandvaluepayment += grandvaluePay;
                //}
                for (int j = 2; j < (spreadDet.Sheets[0].ColumnCount - 1); j++)
                {
                    double.TryParse(Convert.ToString(fnlgrandtotal[j]), out grandvalue);
                    spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].Rows.Count - 1, j].Text = d2.numberformat(Convert.ToString(grandvalue));
                    grandvaluereceipt += grandvalue;
                }
                for (int j = 3; j < (spreadDet.Sheets[0].ColumnCount - 1); j = j + 2)
                {
                    double.TryParse(Convert.ToString(fnlgrandtotalPay[j]), out grandvaluePay);//payment
                    spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].Rows.Count - 1, j].Text = d2.numberformat(Convert.ToString(grandvaluePay));
                    grandvaluepayment += grandvaluePay;
                }
                spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, spreadDet.Sheets[0].ColumnCount - 2].Text = Convert.ToString(grandvaluereceipt);
                spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, spreadDet.Sheets[0].ColumnCount - 1].Text = Convert.ToString(grandvaluepayment);//abarna
                #endregion
                spreadDet.Sheets[0].PageSize = spreadDet.Sheets[0].RowCount;
                lblvalidation1.Text = "";
                txtexcelname.Text = "";
                spreadDet.Visible = true;
                print.Visible = true;
                spreadDet.Height = 100 + height;
                spreadDet.SaveChanges();
                payModeLabels(htPayCol);
                #endregion
            }
            else
            {
                #region value
                Hashtable grandtotal = new Hashtable();
                Hashtable grandtotalPay = new Hashtable();
                Hashtable fnlgrandtotal = new Hashtable();
                Hashtable fnlgrandtotalPay = new Hashtable();
                Hashtable htPayAmt = new Hashtable();
                Hashtable htpaymentAmt = new Hashtable();
                string memtype = Convert.ToString(getCblSelectedValue(cblmem));
                int height = 0;
                int row = 0;
                //for (int i = 0; i < cblclg.Items.Count; i++)
                //{
                bool clgbool = true;
                bool clgchkbool = false;

                int fstrowCnt = 0;
                //    if (cblclg.Items[i].Selected)
                //    {
                //ds.Tables[0].DefaultView.RowFilter = "college_code='" + cblclg.Items[i].Value + "'";
                DataView dvclg = ds.Tables[0].DefaultView;
                if (dvclg.Count > 0)
                {
                    for (int dt = 0; dt < dvclg.Count; dt++)
                    {
                        bool rowbool = false;
                        bool cblbool = true;

                        double fnlTotAmt = 0;
                        double fnlcretotamt = 0;
                        string date = Convert.ToString(dvclg[dt]["Transdate"]);
                        //for (int sem = 0; sem < cbl_sem.Items.Count; sem++)
                        //{
                        //    if (cbl_sem.Items[sem].Selected)
                        //    {
                        //        string semStr = Convert.ToString(cbl_sem.Items[sem].Text);
                        double totalAmount = 0;
                        double cretotamount = 0;
                        bool paybool = false;
                        int curColCnt = 0;
                        string payModeVal = string.Empty;
                        for (int pay = 0; pay < chkl_paid.Items.Count; pay++)
                        {
                            if (chkl_paid.Items[pay].Selected)
                            {
                                #region paymode

                                payModeVal = Convert.ToString(chkl_paid.Items[pay].Value);
                                if (memtype == "1")
                                {
                                    ds.Tables[1].DefaultView.RowFilter = "Transdate='" + date + "' and paymode='" + chkl_paid.Items[pay].Value + "'";// and college_code='" + cblclg.Items[i].Value + "'
                                }
                                else
                                {
                                    ds.Tables[1].DefaultView.RowFilter = "Transdate='" + date + "' and paymode='" + chkl_paid.Items[pay].Value + "'";// and college_code='" + cblclg.Items[i].Value + "'
                                }
                                DataView dvfee = ds.Tables[1].DefaultView;
                                double paidamount = 0;
                                double clrAmount = 0;
                                double creditamount = 0;
                                int.TryParse(Convert.ToString(htcolindex[payModeVal + "-" + chkl_paid.Items[pay].Value]), out curColCnt);
                                if (dvfee.Count > 0)
                                {
                                    DataTable dtval = dvfee.ToTable();
                                    double.TryParse(Convert.ToString(dtval.Compute("sum(debit)", "")), out paidamount);
                                    // double.TryParse(Convert.ToString(dvfee[0]["debit"]), out paidamount);
                                    double.TryParse(Convert.ToString(dtval.Compute("sum(credit)", "")), out creditamount);
                                    totalAmount += paidamount;
                                    cretotamount += creditamount;
                                    paybool = true;
                                    rowbool = true;
                                    clgchkbool = true;
                                }
                                if (clgbool)
                                    fstrowCnt = spreadDet.Sheets[0].RowCount++;
                                if (cblbool)
                                    spreadDet.Sheets[0].RowCount++;
                                if (!grandtotal.ContainsKey(curColCnt))
                                    grandtotal.Add(curColCnt, Convert.ToString(paidamount));
                                else
                                {
                                    double amount = 0;
                                    double.TryParse(Convert.ToString(grandtotal[curColCnt]), out amount);
                                    amount += paidamount;
                                    grandtotal.Remove(curColCnt);
                                    grandtotal.Add(curColCnt, Convert.ToString(amount));
                                }
                                if (!grandtotalPay.ContainsKey(curColCnt + 1))//payment for grandtot
                                    grandtotalPay.Add(curColCnt + 1, Convert.ToString(creditamount));
                                else
                                {
                                    double amount = 0;
                                    double.TryParse(Convert.ToString(grandtotalPay[curColCnt + 1]), out amount);
                                    amount += creditamount;
                                    grandtotalPay.Remove(curColCnt + 1);
                                    grandtotalPay.Add(curColCnt + 1, Convert.ToString(amount));
                                }
                                //paymode receipt amt get
                                if (!htPayAmt.ContainsKey(chkl_paid.Items[pay].Value))
                                    htPayAmt.Add(chkl_paid.Items[pay].Value, Convert.ToString(paidamount));
                                else
                                {
                                    double amount = 0;
                                    double.TryParse(Convert.ToString(htPayAmt[chkl_paid.Items[pay].Value]), out amount);
                                    amount += paidamount;
                                    htPayAmt.Remove(chkl_paid.Items[pay].Value);
                                    htPayAmt.Add(chkl_paid.Items[pay].Value, Convert.ToString(amount));
                                }

                                //paymode payment amt get
                                if (!htpaymentAmt.ContainsKey(chkl_paid.Items[pay].Value))
                                    htpaymentAmt.Add(chkl_paid.Items[pay].Value, Convert.ToString(creditamount));
                                else
                                {
                                    double amount = 0;
                                    double.TryParse(Convert.ToString(htpaymentAmt[chkl_paid.Items[pay].Value]), out amount);
                                    amount += creditamount;
                                    htpaymentAmt.Remove(chkl_paid.Items[pay].Value);
                                    htpaymentAmt.Add(chkl_paid.Items[pay].Value, Convert.ToString(amount));
                                }

                                if (payModeVal == "2" || payModeVal == "3")
                                {

                                    if (memtype == "1")
                                    {
                                        ds.Tables[2].DefaultView.RowFilter = "Transdate='" + date + "' and paymode='" + chkl_paid.Items[pay].Value + "'";// and college_code='" + cblclg.Items[i].Value + "'
                                    }
                                    else
                                    {
                                        ds.Tables[2].DefaultView.RowFilter = "Transdate='" + date + "'  and paymode='" + chkl_paid.Items[pay].Value + "' ";//and college_code='" + cblclg.Items[i].Value + "'
                                    }
                                    DataView dvclr = ds.Tables[2].DefaultView;
                                    if (dvclr.Count > 0)
                                    {
                                        DataTable dtvals = dvclr.ToTable();
                                        double.TryParse(Convert.ToString(dtvals.Compute("sum(debit)", "")), out paidamount);
                                        double.TryParse(Convert.ToString(dtvals.Compute("sum(credit)", "")), out creditamount);
                                        // double.TryParse(Convert.ToString(dvclr[0]["debit"]), out clrAmount);
                                    }

                                    if (paidamount != 0)

                                        spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt].Text = Convert.ToString(paidamount + "[" + clrAmount + "]");


                                    else

                                        spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt].Text = "-";


                                    if (creditamount != 0)
                                        spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt + 1].Text = Convert.ToString(creditamount);
                                    else
                                        spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt + 1].Text = "-";


                                }
                                else
                                {
                                    if (paidamount != 0)
                                        spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt].Text = Convert.ToString(paidamount);
                                    else
                                        spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt].Text = "-";

                                    if (creditamount != 0)
                                        spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt + 1].Text = Convert.ToString(creditamount);
                                    else
                                        spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt + 1].Text = "-";



                                }
                                cblbool = false;
                                clgbool = false;
                                if (payModeVal == "1")
                                {
                                    spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt].ForeColor = Color.Red;
                                    spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt + 1].ForeColor = Color.Red;
                                }
                                else if (payModeVal == "2")
                                {
                                    spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt].ForeColor = Color.Gray;
                                    spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt + 1].ForeColor = Color.Gray;
                                }
                                else if (payModeVal == "3")
                                {
                                    spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt].ForeColor = Color.OrangeRed;
                                    spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt + 1].ForeColor = Color.OrangeRed;
                                }
                                else if (payModeVal == "4")
                                {
                                    spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt].ForeColor = ColorTranslator.FromHtml("#90EE90");
                                    spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt + 1].ForeColor = ColorTranslator.FromHtml("#90EE90");
                                }
                                else if (payModeVal == "5")
                                {
                                    spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt].ForeColor = ColorTranslator.FromHtml("#FAFAD2");
                                    spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt + 1].ForeColor = ColorTranslator.FromHtml("#FAFAD2");
                                }
                                spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt].Font.Bold = true;
                                spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt + 1].Font.Bold = true;
                                #endregion
                            }

                        }
                        if (paybool)
                        {
                            // int.TryParse(Convert.ToString(htcolindex[Convert.ToString(payModeVal + "-" + "Total")]), out curColCnt);
                            //   spreadDet.Sheets[0].ColumnCount -1;
                            spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, spreadDet.Sheets[0].ColumnCount - 2].Text = Convert.ToString(totalAmount);
                            spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, spreadDet.Sheets[0].ColumnCount - 1].Text = Convert.ToString(cretotamount);//abarna
                            spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt].ForeColor = ColorTranslator.FromHtml("#CC00FF");
                            fnlTotAmt += totalAmount;
                            fnlcretotamt += cretotamount;//added by abarna
                            if (!grandtotal.ContainsKey(curColCnt))
                                grandtotal.Add(curColCnt, Convert.ToString(totalAmount));
                            else
                            {
                                double amount = 0;
                                double.TryParse(Convert.ToString(grandtotal[curColCnt]), out amount);
                                amount += totalAmount;
                                grandtotal.Remove(curColCnt);
                                grandtotal.Add(curColCnt, Convert.ToString(amount));
                            }
                            if (!grandtotalPay.ContainsKey(curColCnt + 1))//added for payment
                                grandtotalPay.Add(curColCnt + 1, Convert.ToString(cretotamount));
                            else
                            {
                                double amount = 0;
                                double.TryParse(Convert.ToString(grandtotalPay[curColCnt + 1]), out amount);
                                amount += cretotamount;
                                grandtotalPay.Remove(curColCnt + 1);
                                grandtotalPay.Add(curColCnt + 1, Convert.ToString(amount));
                            }
                        }
                        //    }
                        //}
                        if (rowbool)
                        {
                            //paymode details
                            //curColCnt = 0;

                            //   int.TryParse(Convert.ToString((chkl_paid.Items.Count*2)+1), out curColCnt);
                            int.TryParse(Convert.ToString(htcolindex[Convert.ToString("Total")]), out curColCnt);
                            spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt].Text = Convert.ToString(fnlTotAmt);
                            spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt + 1].Text = Convert.ToString(fnlcretotamt);//abarna
                            if (!grandtotal.ContainsKey(curColCnt))
                                grandtotal.Add(curColCnt, Convert.ToString(fnlTotAmt));
                            else
                            {
                                double amount = 0;
                                double.TryParse(Convert.ToString(grandtotal[curColCnt]), out amount);
                                amount += fnlTotAmt;
                                grandtotal.Remove(curColCnt);
                                grandtotal.Add(curColCnt, Convert.ToString(amount));
                            }

                            if (!grandtotalPay.ContainsKey(curColCnt + 1))//added for payment
                                grandtotalPay.Add(curColCnt + 1, Convert.ToString(fnlcretotamt));
                            else
                            {
                                double amount = 0;
                                double.TryParse(Convert.ToString(grandtotalPay[curColCnt + 1]), out amount);
                                amount += fnlcretotamt;
                                grandtotalPay.Remove(curColCnt + 1);
                                grandtotalPay.Add(curColCnt + 1, Convert.ToString(amount));
                            }

                            for (int pay = 0; pay < chkl_paid.Items.Count; pay++)
                            {
                                if (chkl_paid.Items[pay].Selected)
                                {
                                    payModeVal = Convert.ToString(chkl_paid.Items[pay].Value);
                                    curColCnt = 0;
                                    int.TryParse(Convert.ToString(htPayColFnl[payModeVal]), out curColCnt);
                                    double payAmt = 0;
                                    double.TryParse(Convert.ToString(htPayAmt[payModeVal]), out payAmt);

                                    double paymentAmt = 0;
                                    double.TryParse(Convert.ToString(htpaymentAmt[payModeVal]), out paymentAmt);

                                    spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt].Text = d2.numberformat(Convert.ToString(payAmt));
                                    spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt + 1].Text = d2.numberformat(Convert.ToString(paymentAmt));//abarna

                                    if (!grandtotal.ContainsKey(curColCnt))
                                        grandtotal.Add(curColCnt, Convert.ToString(payAmt));
                                    else
                                    {
                                        double amount = 0;
                                        double.TryParse(Convert.ToString(grandtotal[curColCnt]), out amount);
                                        amount += payAmt;
                                        grandtotal.Remove(curColCnt);
                                        grandtotal.Add(curColCnt, Convert.ToString(amount));
                                    }
                                    if (!grandtotalPay.ContainsKey(curColCnt + 1))//added for payment
                                        grandtotalPay.Add(curColCnt + 1, Convert.ToString(paymentAmt));
                                    else
                                    {
                                        double amount = 0;
                                        double.TryParse(Convert.ToString(grandtotalPay[curColCnt + 1]), out amount);
                                        amount += paymentAmt;
                                        grandtotalPay.Remove(curColCnt + 1);
                                        grandtotalPay.Add(curColCnt + 1, Convert.ToString(amount));
                                    }
                                    if (payModeVal == "1")
                                        spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt].BackColor = ColorTranslator.FromHtml("#F08080");
                                    else if (payModeVal == "2")
                                        spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt].BackColor = ColorTranslator.FromHtml("#D3D3D3");
                                    else if (payModeVal == "3")
                                        spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt].BackColor = ColorTranslator.FromHtml("#FFA500");
                                    else if (payModeVal == "4")
                                        spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt].BackColor = ColorTranslator.FromHtml("#90EE90");
                                    else if (payModeVal == "5")
                                        spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt].BackColor = ColorTranslator.FromHtml("#FAFAD2");
                                }
                            }
                            htPayAmt.Clear();
                            htpaymentAmt.Clear();
                            row++;
                            spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, 0].Text = Convert.ToString(row);
                            spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, 0].ForeColor = Color.Black;
                            spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, 1].Text = Convert.ToString(date);
                            spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, 1].ForeColor = Color.Black;
                            height += 15;
                        }
                    }
                }

                if (clgchkbool)
                {
                    //spreadDet.Sheets[0].Cells[fstrowCnt, 0].Text = Convert.ToString(cblclg.Items[i].Text);
                    //spreadDet.Sheets[0].SpanModel.Add(fstrowCnt, 0, 1, spreadDet.Sheets[0].ColumnCount - 1);
                    //spreadDet.Sheets[0].Rows[fstrowCnt].BackColor = Color.Green;
                    //spreadDet.Sheets[0].Rows[fstrowCnt].ForeColor = Color.White;

                    spreadDet.Sheets[0].Rows.Count++;
                    spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].Rows.Count - 1, 0].Text = "Total";
                    spreadDet.Sheets[0].SpanModel.Add(spreadDet.Sheets[0].Rows.Count - 1, 0, 1, 2);
                    spreadDet.Sheets[0].Rows[spreadDet.Sheets[0].Rows.Count - 1].BackColor = Color.Gold;
                    double value = 0;
                    double value2 = 0;
                    double payamountreceipt = 0;
                    double payamountpayment = 0;
                    for (int j = 2; j < (spreadDet.Sheets[0].ColumnCount - 1); j++)
                    {
                        double.TryParse(Convert.ToString(grandtotal[j]), out value);
                        if (grandtotal.ContainsKey(j))
                            spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].Rows.Count - 1, j].Text = d2.numberformat(Convert.ToString(value));//receipt
                        payamountreceipt += value;
                        double.TryParse(Convert.ToString(grandtotalPay[j + 1]), out value2);
                        if (grandtotalPay.ContainsKey(j + 1))
                            spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].Rows.Count - 1, j + 1].Text = d2.numberformat(Convert.ToString(value2));//payment
                        payamountpayment += value2;
                        if (!fnlgrandtotal.ContainsKey(j))
                            fnlgrandtotal.Add(j, Convert.ToString(value));
                        else
                        {
                            double amount = 0;
                            double.TryParse(Convert.ToString(fnlgrandtotal[j]), out amount);
                            amount += value;
                            fnlgrandtotal.Remove(j);
                            fnlgrandtotal.Add(j, Convert.ToString(amount));
                        }
                        if (!fnlgrandtotalPay.ContainsKey(j + 1))//payment
                            fnlgrandtotalPay.Add(j + 1, Convert.ToString(value2));
                        else
                        {
                            double amount = 0;
                            double.TryParse(Convert.ToString(fnlgrandtotalPay[j + 1]), out amount);
                            amount += value2;
                            fnlgrandtotalPay.Remove(j + 1);
                            fnlgrandtotalPay.Add(j + 1, Convert.ToString(amount));
                        }
                    }
                    spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, spreadDet.Sheets[0].ColumnCount - 2].Text = Convert.ToString(payamountreceipt);
                    spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, spreadDet.Sheets[0].ColumnCount - 1].Text = Convert.ToString(payamountpayment);//abarna
                    grandtotal.Clear();
                    grandtotalPay.Clear();
                    //    }

                    //}

                    #region grandtot
                    // spreadDet.Sheets[0].PageSize = ds.Tables[0].Rows.Count + 1;
                    spreadDet.Sheets[0].Rows.Count++;
                    spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].Rows.Count - 1, 0].Text = "Grand Total";
                    spreadDet.Sheets[0].SpanModel.Add(spreadDet.Sheets[0].Rows.Count - 1, 0, 1, 2);
                    spreadDet.Sheets[0].Rows[spreadDet.Sheets[0].Rows.Count - 1].BackColor = Color.YellowGreen;
                    spreadDet.Sheets[0].Rows[spreadDet.Sheets[0].Rows.Count - 1].ForeColor = Color.Black;
                    double grandvalue = 0;
                    double grandvaluePay = 0;
                    double grandvaluereceipt = 0;
                    double grandvaluepayment = 0;
                    for (int j = 2; j < (spreadDet.Sheets[0].ColumnCount - 1); j++)
                    {
                        double.TryParse(Convert.ToString(fnlgrandtotal[j]), out grandvalue);
                        spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].Rows.Count - 1, j].Text = d2.numberformat(Convert.ToString(grandvalue));
                        grandvaluereceipt += grandvalue;
                    }
                    for (int j = 3; j < (spreadDet.Sheets[0].ColumnCount - 1); j = j + 2)
                    {
                        double.TryParse(Convert.ToString(fnlgrandtotalPay[j]), out grandvaluePay);//payment
                        spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].Rows.Count - 1, j].Text = d2.numberformat(Convert.ToString(grandvaluePay));
                        grandvaluepayment += grandvaluePay;
                    }
                    spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, spreadDet.Sheets[0].ColumnCount - 2].Text = Convert.ToString(grandvaluereceipt);
                    spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, spreadDet.Sheets[0].ColumnCount - 1].Text = Convert.ToString(grandvaluepayment);//abarna
                    #endregion
                    spreadDet.Sheets[0].PageSize = spreadDet.Sheets[0].RowCount;
                    lblvalidation1.Text = "";
                    txtexcelname.Text = "";
                    spreadDet.Visible = true;
                    print.Visible = true;
                    spreadDet.Height = 100 + height;
                    spreadDet.SaveChanges();
                    payModeLabels(htPayCol);

                }
                #endregion
            }
        }
        catch { }
    }

    #region old

    //protected void spreadLoadDetailed(DataSet ds, Dictionary<string, int> feecatValue, Dictionary<string, string> feecatText)
    //{
    //    try
    //    {
    //        #region design

    //        spreadDet.Sheets[0].RowCount = 0;
    //        spreadDet.Sheets[0].ColumnCount = 0;
    //        spreadDet.CommandBar.Visible = false;
    //        spreadDet.Sheets[0].AutoPostBack = true;
    //        spreadDet.Sheets[0].ColumnHeader.RowCount = 2;
    //        spreadDet.Sheets[0].RowHeader.Visible = false;
    //        spreadDet.Sheets[0].ColumnCount = 2;
    //        FarPoint.Web.Spread.StyleInfo darkstyle = new FarPoint.Web.Spread.StyleInfo();
    //        darkstyle.BackColor = ColorTranslator.FromHtml("#0CA6CA");
    //        darkstyle.ForeColor = Color.White;
    //        spreadDet.ActiveSheetView.ColumnHeader.DefaultStyle = darkstyle;

    //        spreadDet.Sheets[0].ColumnHeader.Cells[0, 0].Text = "S.No";
    //        spreadDet.Sheets[0].ColumnHeader.Cells[0, 0].ForeColor = ColorTranslator.FromHtml("#000000");
    //        spreadDet.Sheets[0].ColumnHeader.Cells[0, 0].Font.Bold = true;
    //        spreadDet.Sheets[0].ColumnHeader.Cells[0, 0].Font.Name = "Book Antiqua";
    //        spreadDet.Sheets[0].ColumnHeader.Cells[0, 0].Font.Size = FontUnit.Medium;
    //        spreadDet.Sheets[0].ColumnHeader.Cells[0, 0].HorizontalAlign = HorizontalAlign.Center;
    //        spreadDet.Sheets[0].Columns[0].HorizontalAlign = HorizontalAlign.Center;

    //        spreadDet.Sheets[0].ColumnHeader.Cells[0, 1].Text = "Date";
    //        spreadDet.Sheets[0].ColumnHeader.Cells[0, 1].ForeColor = ColorTranslator.FromHtml("#000000");
    //        spreadDet.Sheets[0].ColumnHeader.Cells[0, 1].Font.Bold = true;
    //        spreadDet.Sheets[0].ColumnHeader.Cells[0, 1].Font.Name = "Book Antiqua";
    //        spreadDet.Sheets[0].ColumnHeader.Cells[0, 1].Font.Size = FontUnit.Medium;
    //        spreadDet.Sheets[0].ColumnHeader.Cells[0, 1].HorizontalAlign = HorizontalAlign.Center;
    //        spreadDet.Sheets[0].Columns[1].HorizontalAlign = HorizontalAlign.Left;
    //        spreadDet.Sheets[0].ColumnHeaderSpanModel.Add(0, 0, 2, 1);
    //        spreadDet.Sheets[0].ColumnHeaderSpanModel.Add(0, 1, 2, 1);
    //        string hdrTxtValue = "";
    //        Hashtable htcolindex = new Hashtable();
    //        Hashtable htPayCol = new Hashtable();
    //        for (int i = 0; i < cbl_sem.Items.Count; i++)
    //        {
    //            if (cbl_sem.Items[i].Selected)
    //            {
    //                int checkva = 0;
    //                bool colsem = false;
    //                int col = spreadDet.Sheets[0].ColumnCount++;
    //                spreadDet.Sheets[0].ColumnHeader.Cells[0, spreadDet.Sheets[0].ColumnCount - 1].Text = Convert.ToString(cbl_sem.Items[i].Text);
    //                spreadDet.Sheets[0].ColumnHeader.Cells[0, spreadDet.Sheets[0].ColumnCount - 1].Tag = Convert.ToString(cbl_sem.Items[i].Value);
    //                hdrTxtValue = Convert.ToString(cbl_sem.Items[i].Text);
    //                spreadDet.Sheets[0].ColumnHeader.Cells[0, spreadDet.Sheets[0].ColumnCount - 1].ForeColor = ColorTranslator.FromHtml("#000000");
    //                spreadDet.Sheets[0].ColumnHeader.Cells[0, spreadDet.Sheets[0].ColumnCount - 1].Font.Bold = true;
    //                spreadDet.Sheets[0].ColumnHeader.Cells[0, spreadDet.Sheets[0].ColumnCount - 1].Font.Name = "Book Antiqua";
    //                spreadDet.Sheets[0].ColumnHeader.Cells[0, spreadDet.Sheets[0].ColumnCount - 1].Font.Size = FontUnit.Medium;
    //                spreadDet.Sheets[0].ColumnHeader.Cells[0, spreadDet.Sheets[0].ColumnCount - 1].HorizontalAlign = HorizontalAlign.Center;
    //                spreadDet.Sheets[0].Columns[spreadDet.Sheets[0].ColumnCount - 1].HorizontalAlign = HorizontalAlign.Right;
    //                for (int s = 0; s < chkl_paid.Items.Count; s++)
    //                {
    //                    if (chkl_paid.Items[s].Selected == true)
    //                    {
    //                        checkva++;
    //                        if (checkva > 1)
    //                            spreadDet.Sheets[0].ColumnCount++;

    //                        spreadDet.Sheets[0].ColumnHeader.Cells[1, spreadDet.Sheets[0].ColumnCount - 1].Text = Convert.ToString(chkl_paid.Items[s].Text);
    //                        spreadDet.Sheets[0].ColumnHeader.Cells[1, spreadDet.Sheets[0].ColumnCount - 1].Tag = Convert.ToString(chkl_paid.Items[s].Value);
    //                        htcolindex.Add(Convert.ToString(hdrTxtValue + "-" + chkl_paid.Items[s].Value), spreadDet.Sheets[0].ColumnCount - 1);
    //                        if (!htPayCol.ContainsKey(chkl_paid.Items[s].Value))
    //                            htPayCol.Add(Convert.ToString(chkl_paid.Items[s].Value), spreadDet.Sheets[0].ColumnCount - 1);
    //                        spreadDet.Sheets[0].ColumnHeader.Cells[1, spreadDet.Sheets[0].ColumnCount - 1].ForeColor = ColorTranslator.FromHtml("#000000");
    //                        spreadDet.Sheets[0].ColumnHeader.Cells[1, spreadDet.Sheets[0].ColumnCount - 1].Font.Bold = true;
    //                        spreadDet.Sheets[0].ColumnHeader.Cells[1, spreadDet.Sheets[0].ColumnCount - 1].Font.Name = "Book Antiqua";
    //                        spreadDet.Sheets[0].ColumnHeader.Cells[1, spreadDet.Sheets[0].ColumnCount - 1].Font.Size = FontUnit.Medium;
    //                        spreadDet.Sheets[0].ColumnHeader.Cells[1, spreadDet.Sheets[0].ColumnCount - 1].HorizontalAlign = HorizontalAlign.Center;
    //                        spreadDet.Sheets[0].Columns[spreadDet.Sheets[0].ColumnCount - 1].HorizontalAlign = HorizontalAlign.Right;
    //                        colsem = true;
    //                    }
    //                }
    //                if (colsem)
    //                {
    //                    checkva++;
    //                    spreadDet.Sheets[0].ColumnCount++;
    //                    spreadDet.Sheets[0].ColumnHeader.Cells[1, spreadDet.Sheets[0].ColumnCount - 1].Text = "Total";
    //                    htcolindex.Add(Convert.ToString(hdrTxtValue + "-" + "Total"), spreadDet.Sheets[0].ColumnCount - 1);
    //                    spreadDet.Sheets[0].ColumnHeader.Cells[1, spreadDet.Sheets[0].ColumnCount - 1].ForeColor = ColorTranslator.FromHtml("#000000");
    //                    spreadDet.Sheets[0].ColumnHeader.Cells[1, spreadDet.Sheets[0].ColumnCount - 1].Font.Bold = true;
    //                    spreadDet.Sheets[0].ColumnHeader.Cells[1, spreadDet.Sheets[0].ColumnCount - 1].Font.Name = "Book Antiqua";
    //                    spreadDet.Sheets[0].ColumnHeader.Cells[1, spreadDet.Sheets[0].ColumnCount - 1].Font.Size = FontUnit.Medium;
    //                    spreadDet.Sheets[0].ColumnHeader.Cells[1, spreadDet.Sheets[0].ColumnCount - 1].HorizontalAlign = HorizontalAlign.Center;
    //                    spreadDet.Sheets[0].Columns[spreadDet.Sheets[0].ColumnCount - 1].HorizontalAlign = HorizontalAlign.Right;
    //                    spreadDet.Sheets[0].ColumnHeaderSpanModel.Add(0, col, 1, checkva);
    //                }
    //            }
    //        }
    //        #endregion

    //        #region value
    //        Hashtable grandtotal = new Hashtable();
    //        Hashtable fnlgrandtotal = new Hashtable();
    //        int height = 0;
    //        int row = 0;
    //        for (int i = 0; i < cblclg.Items.Count; i++)
    //        {
    //            bool clgbool = true;
    //            bool clgchkbool = false;
    //            int fstrowCnt = 0;
    //            if (cblclg.Items[i].Selected)
    //            {
    //                ds.Tables[0].DefaultView.RowFilter = "college_code='" + cblclg.Items[i].Value + "'";
    //                DataView dvclg = ds.Tables[0].DefaultView;
    //                if (dvclg.Count > 0)
    //                {
    //                    for (int dt = 0; dt < dvclg.Count; dt++)
    //                    {
    //                        bool rowbool = false;
    //                        bool cblbool = true;
    //                        string date = Convert.ToString(dvclg[dt]["Transdate"]);
    //                        if (feecatValue.ContainsValue(Convert.ToInt32(cblclg.Items[i].Value)))
    //                        {
    //                            Dictionary<string, int> tempfeecatValue = feecatValue.Where(p => p.Value == Convert.ToInt32(cblclg.Items[i].Value)).ToDictionary(p => p.Key, p => p.Value);
    //                            double totalAmount = 0;
    //                            foreach (KeyValuePair<string, int> fee in tempfeecatValue)
    //                            {
    //                                bool paybool = false;
    //                                int curColCnt = 0;
    //                                string feeCode = Convert.ToString(fee.Key);
    //                                string str = feecatText.ContainsKey(feeCode) ? str = feecatText[feeCode].ToString() : str = "";
    //                                for (int pay = 0; pay < chkl_paid.Items.Count; pay++)
    //                                {
    //                                    if (chkl_paid.Items[pay].Selected)
    //                                    {
    //                                        #region paymode

    //                                        string payModeVal = Convert.ToString(chkl_paid.Items[pay].Value);
    //                                        ds.Tables[1].DefaultView.RowFilter = "Transdate='" + date + "' and feecategory='" + feeCode + "' and paymode='" + chkl_paid.Items[pay].Value + "'";
    //                                        DataView dvfee = ds.Tables[1].DefaultView;
    //                                        double paidamount = 0;
    //                                        double clrAmount = 0;
    //                                        int.TryParse(Convert.ToString(htcolindex[str + "-" + chkl_paid.Items[pay].Value]), out curColCnt);
    //                                        if (dvfee.Count > 0)
    //                                        {
    //                                            double.TryParse(Convert.ToString(dvfee[0]["debit"]), out paidamount);
    //                                            totalAmount += paidamount;
    //                                            paybool = true;
    //                                            rowbool = true;
    //                                            clgchkbool = true;
    //                                        }
    //                                        if (clgbool)
    //                                            fstrowCnt = spreadDet.Sheets[0].RowCount++;
    //                                        if (cblbool)
    //                                            spreadDet.Sheets[0].RowCount++;
    //                                        if (!grandtotal.ContainsKey(curColCnt))
    //                                            grandtotal.Add(curColCnt, Convert.ToString(paidamount));
    //                                        else
    //                                        {
    //                                            double amount = 0;
    //                                            double.TryParse(Convert.ToString(grandtotal[curColCnt]), out amount);
    //                                            amount += paidamount;
    //                                            grandtotal.Remove(curColCnt);
    //                                            grandtotal.Add(curColCnt, Convert.ToString(amount));
    //                                        }
    //                                        if (payModeVal == "2" || payModeVal == "3")
    //                                        {
    //                                            ds.Tables[2].DefaultView.RowFilter = "Transdate='" + date + "' and feecategory='" + feeCode + "' and paymode='" + chkl_paid.Items[pay].Value + "'";
    //                                            DataView dvclr = ds.Tables[2].DefaultView;
    //                                            if (dvclr.Count > 0)
    //                                                double.TryParse(Convert.ToString(dvclr[0]["debit"]), out clrAmount);

    //                                            if (paidamount != 0)
    //                                                spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt].Text = Convert.ToString(paidamount + "[" + clrAmount + "]");
    //                                            else
    //                                                spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt].Text = "-";
    //                                        }
    //                                        else
    //                                        {
    //                                            if (paidamount != 0)
    //                                                spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt].Text = Convert.ToString(paidamount);
    //                                            else
    //                                                spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt].Text = "-";
    //                                        }
    //                                        cblbool = false;
    //                                        clgbool = false;
    //                                        if (payModeVal == "1")
    //                                            spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt].ForeColor = Color.Red;
    //                                        else if (payModeVal == "2")
    //                                            spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt].ForeColor = Color.Gray;
    //                                        else if (payModeVal == "3")
    //                                            spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt].ForeColor = Color.OrangeRed;
    //                                        else if (payModeVal == "4")
    //                                            spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt].ForeColor = ColorTranslator.FromHtml("#90EE90");
    //                                        else if (payModeVal == "5")
    //                                            spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt].ForeColor = ColorTranslator.FromHtml("#FAFAD2");
    //                                        spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt].Font.Bold = true;
    //                                        #endregion
    //                                    }
    //                                }
    //                                if (paybool)
    //                                {
    //                                    int.TryParse(Convert.ToString(htcolindex[Convert.ToString(str + "-" + "Total")]), out curColCnt);
    //                                    spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt].Text = Convert.ToString(totalAmount);
    //                                    spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, curColCnt].ForeColor = ColorTranslator.FromHtml("#CC00FF");
    //                                    if (!grandtotal.ContainsKey(curColCnt))
    //                                        grandtotal.Add(curColCnt, Convert.ToString(totalAmount));
    //                                    else
    //                                    {
    //                                        double amount = 0;
    //                                        double.TryParse(Convert.ToString(grandtotal[curColCnt]), out amount);
    //                                        amount += totalAmount;
    //                                        grandtotal.Remove(curColCnt);
    //                                        grandtotal.Add(curColCnt, Convert.ToString(amount));
    //                                    }
    //                                }
    //                            }
    //                        }
    //                        if (rowbool)
    //                        {
    //                            row++;
    //                            spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, 0].Text = Convert.ToString(row);
    //                            spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, 0].ForeColor = Color.Black;
    //                            spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].RowCount - 1, 1].Text = Convert.ToString(date);
    //                            height += 15;
    //                        }
    //                    }
    //                }
    //            }
    //            if (clgchkbool)
    //            {
    //                spreadDet.Sheets[0].Cells[fstrowCnt, 0].Text = Convert.ToString(cblclg.Items[i].Text);
    //                spreadDet.Sheets[0].SpanModel.Add(fstrowCnt, 0, 1, spreadDet.Sheets[0].ColumnCount - 1);
    //                spreadDet.Sheets[0].Rows[fstrowCnt].BackColor = Color.Green;
    //                spreadDet.Sheets[0].Rows[fstrowCnt].ForeColor = Color.White;

    //                spreadDet.Sheets[0].Rows.Count++;
    //                spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].Rows.Count - 1, 0].Text = "Total";
    //                spreadDet.Sheets[0].SpanModel.Add(spreadDet.Sheets[0].Rows.Count - 1, 0, 1, 2);
    //                spreadDet.Sheets[0].Rows[spreadDet.Sheets[0].Rows.Count - 1].BackColor = Color.Gold;
    //                double value = 0;
    //                for (int j = 2; j < spreadDet.Sheets[0].ColumnCount; j++)
    //                {
    //                    double.TryParse(Convert.ToString(grandtotal[j]), out value);
    //                    spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].Rows.Count - 1, j].Text = Convert.ToString(value);
    //                    if (!fnlgrandtotal.ContainsKey(j))
    //                        fnlgrandtotal.Add(j, Convert.ToString(value));
    //                    else
    //                    {
    //                        double amount = 0;
    //                        double.TryParse(Convert.ToString(fnlgrandtotal[j]), out amount);
    //                        amount += value;
    //                        fnlgrandtotal.Remove(j);
    //                        fnlgrandtotal.Add(j, Convert.ToString(amount));
    //                    }
    //                }
    //            }

    //        }

    //        #region grandtot
    //        // spreadDet.Sheets[0].PageSize = ds.Tables[0].Rows.Count + 1;
    //        spreadDet.Sheets[0].Rows.Count++;
    //        spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].Rows.Count - 1, 0].Text = "Grand Total";
    //        spreadDet.Sheets[0].SpanModel.Add(spreadDet.Sheets[0].Rows.Count - 1, 0, 1, 2);
    //        spreadDet.Sheets[0].Rows[spreadDet.Sheets[0].Rows.Count - 1].BackColor = Color.YellowGreen;
    //        spreadDet.Sheets[0].Rows[spreadDet.Sheets[0].Rows.Count - 1].ForeColor = Color.Black;
    //        double grandvalue = 0;
    //        for (int j = 2; j < spreadDet.Sheets[0].ColumnCount; j++)
    //        {
    //            double.TryParse(Convert.ToString(fnlgrandtotal[j]), out grandvalue);
    //            spreadDet.Sheets[0].Cells[spreadDet.Sheets[0].Rows.Count - 1, j].Text = Convert.ToString(grandvalue);
    //        }
    //        #endregion
    //        spreadDet.Sheets[0].PageSize = spreadDet.Sheets[0].RowCount;
    //        lblvalidation1.Text = "";
    //        txtexcelname.Text = "";
    //        spreadDet.Visible = true;
    //        print.Visible = true;
    //        spreadDet.Height = 100 + height;
    //        spreadDet.SaveChanges();
    //        payModeLabels(htPayCol);
    //        #endregion

    //    }
    //    catch { }
    //}
    #endregion
    protected void payModeLabels(Hashtable htpay)
    {
        lblc2.Visible = false;
        lblc3.Visible = false;
        lblc5.Visible = false;
        lblc1.Visible = false;
        lblc4.Visible = false;
        lblcard.Visible = false;
        foreach (DictionaryEntry row in htpay)
        {
            if (row.Key.ToString() == "1")
                lblc2.Visible = true;
            if (row.Key.ToString() == "2")
                lblc3.Visible = true;
            if (row.Key.ToString() == "3")
                lblc5.Visible = true;
            if (row.Key.ToString() == "4")
                lblc1.Visible = true;
            if (row.Key.ToString() == "5")
                lblc4.Visible = true;
            if (row.Key.ToString() == "6")
                lblcard.Visible = true;
        }
        tblpaymode.Visible = true;
    }
    protected void btn_errorclose_Click(object sender, EventArgs e)
    {
        imgdiv2.Visible = false;
    }

    #region Print
    protected void btnExcel_Click(object sender, EventArgs e)
    {
        try
        {
            string reportname = txtexcelname.Text;
            if (reportname.ToString().Trim() != "")
            {
                d2.printexcelreport(spreadDet, reportname);
                lblvalidation1.Visible = false;
            }
            else
            {
                lblvalidation1.Text = "Please Enter Your  Report Name";
                lblvalidation1.Visible = true;
                txtexcelname.Focus();
            }
        }
        catch
        { }
    }

    public void btnprintmaster_Click(object sender, EventArgs e)
    {
        try
        {
            lblvalidation1.Text = "";
            txtexcelname.Text = "";
            string degreedetails;
            string pagename;
            degreedetails = "Fees Structure Report";
            pagename = "FeesStructureReport.aspx";
            Printcontrolhed.loadspreaddetails(spreadDet, pagename, degreedetails);
            Printcontrolhed.Visible = true;
        }
        catch { }
    }
    #endregion

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

        lbl.Add(lblclg);
        // lbl.Add(lbl_str1);
        //  lbl.Add(lbldeg);
        //   lbl.Add(lbldept);
        lbl.Add(lblsem);
        fields.Add(0);
        // fields.Add(1);
        //fields.Add(2);
        // fields.Add(3);
        fields.Add(4);

        new HeaderLabelText().setLabels(grouporusercode, ref lbl, fields);

    }

    #region paymode setting
    public void PaymodeToCheckboxList(CheckBoxList cblpaymode, string usercode, string collegecode)
    {
        try
        {
            int inclpayRights = 0;
            string payValue = string.Empty;
            Dictionary<int, string> dtpaymode = new Dictionary<int, string>();
            inclpayRights = paymodeRightsCheck(usercode, collegecode, ref  payValue);
            if (inclpayRights == 1 && payValue != "0")
            {
                string[] splvalue = payValue.Split(',');
                if (splvalue.Length > 0)
                {
                    dtpaymode = dtPaymodeValue();
                    for (int row = 0; row < splvalue.Length; row++)
                    {
                        if (dtpaymode.ContainsKey(Convert.ToInt32(splvalue[row])))
                        {
                            string modestr = dtpaymode[Convert.ToInt32(splvalue[row])];
                            cblpaymode.Items.Add(new System.Web.UI.WebControls.ListItem(modestr, Convert.ToString(splvalue[row])));
                        }
                    }
                }
            }
            else
                cblpaymode.Items.Clear();
        }
        catch { cblpaymode.Items.Clear(); }
    }

    private int paymodeRightsCheck(string usercode, string collegecode, ref string payValue)
    {
        int paymodRghts = 0;
        Int32.TryParse(Convert.ToString(d2.GetFunction("select Linkvalue from New_InsSettings where LinkName='IncludePaymodeSettings' and user_code ='" + usercode + "'")), out paymodRghts);
        if (paymodRghts == 1)
        {
            payValue = Convert.ToString(d2.GetFunction("select Linkvalue from New_InsSettings where LinkName='IncludePaymodeSettingsValue' and user_code ='" + usercode + "' "));
        }
        return paymodRghts;
    }

    private Dictionary<int, string> dtPaymodeValue()
    {
        Dictionary<int, string> dtpaymode = new Dictionary<int, string>();
        dtpaymode.Add(1, "Cash");
        dtpaymode.Add(2, "Cheque");
        dtpaymode.Add(3, "DD");
        dtpaymode.Add(4, "Challan");
        dtpaymode.Add(5, "Online");
        dtpaymode.Add(6, "Card");
        dtpaymode.Add(7, "NEFT");//added by abarna 29.03.2018
        return dtpaymode;
    }
    #endregion

    protected double sclSett()
    {
        double sclType = 0;
        double.TryParse(Convert.ToString(d2.GetFunction("select value from Master_Settings where settings='schoolorcollege' and usercode='" + usercode + "'")), out sclType);
        return sclType;
    }

    //added by sudhagar 01.06.2017
    #region Include setting
    protected void checkdicon_Changed(object sender, EventArgs e)
    {
        try
        {
            if (checkdicon.Checked == true)
            {
                txtinclude.Enabled = true;
                LoadIncludeSetting();
            }
            else
            {
                txtinclude.Enabled = false;
                cblinclude.Items.Clear();
                // LoadIncludeSetting();
            }
        }
        catch { }
    }

    private void LoadIncludeSetting()
    {
        try
        {
            cblinclude.Items.Clear();
            cblinclude.Items.Add(new System.Web.UI.WebControls.ListItem("Course Completed", "1"));
            cblinclude.Items.Add(new System.Web.UI.WebControls.ListItem("Debar", "2"));
            cblinclude.Items.Add(new System.Web.UI.WebControls.ListItem("Discontinue", "3"));
            cblinclude.Items.Add(new System.Web.UI.WebControls.ListItem("Cancel", "4"));
            if (cblinclude.Items.Count > 0)
            {
                for (int i = 0; i < cblinclude.Items.Count; i++)
                {
                    cblinclude.Items[i].Selected = true;
                }
                cbinclude.Checked = true;
                txtinclude.Text = "Include Settings(" + cblinclude.Items.Count + ")";
            }
        }
        catch { }
    }


    protected void cbinclude_OnCheckedChanged(object sender, EventArgs e)
    {
        CallCheckboxChange(cbinclude, cblinclude, txtinclude, "Include Setting", "--Select--");
    }
    protected void cblinclude_OnSelectedIndexChanged(object sender, EventArgs e)
    {
        CallCheckboxListChange(cbinclude, cblinclude, txtinclude, "Include Setting", "--Select--");

    }


    #endregion

    //discontinue,delflag
    protected string getStudCategory()
    {
        string strInclude = string.Empty;
        try
        {
            #region includem

            string cc = "";
            string debar = "";
            string disc = "";
            string cancel = "";
            if (cblinclude.Items.Count > 0)
            {
                for (int i = 0; i < cblinclude.Items.Count; i++)
                {
                    if (cblinclude.Items[i].Selected == true)
                    {
                        if (cblinclude.Items[i].Value == "1")
                            cc = " r.cc=1";
                        if (cblinclude.Items[i].Value == "2")
                            debar = " r.Exam_Flag like '%debar'";
                        if (cblinclude.Items[i].Value == "3")
                            disc = "  r.DelFlag=1";
                        if (cblinclude.Items[i].Value == "4")
                            cancel = "  r.DelFlag=2";
                    }
                }
            }
            if (!checkdicon.Checked)
            {
                if (cc != "" && debar == "" && disc == "" && cancel == "")
                    strInclude = " and (" + cc + " or r.cc=0)  and r.Exam_Flag<>'debar' and r.DelFlag=0";
                if (cc == "" && debar != "" && disc == "" && cancel == "")
                    strInclude = " and r.cc=0  and (" + debar + " or r.Exam_Flag<>'debar') and r.DelFlag=0";
                if (cc == "" && debar == "" && disc != "" && cancel == "")
                    strInclude = " and r.cc=0  and r.Exam_Flag<>'debar' and (" + disc + " or r.DelFlag=0)";
                if (cc == "" && debar == "" && disc == "" && cancel != "")
                    strInclude = " and r.cc=0  and r.Exam_Flag<>'debar' and (" + cancel + " or r.DelFlag=0)";
                //2
                if (cc != "" && debar != "" && disc == "" && cancel == "")
                    strInclude = " and (" + cc + " or r.cc=0) and (" + debar + " or r.Exam_Flag<>'debar') and r.DelFlag=0";
                if (cc != "" && debar == "" && disc != "" && cancel == "")
                    strInclude = " and (" + cc + " or r.cc=0) and r.Exam_Flag<>'debar' and (" + disc + " or r.DelFlag=0)";
                if (cc != "" && debar == "" && disc == "" && cancel != "")
                    strInclude = " and (" + cc + " or r.cc=0) and r.Exam_Flag<>'debar' and (" + cancel + " or r.DelFlag=0)";
                //
                if (cc == "" && debar != "" && disc != "" && cancel == "")
                    strInclude = " and r.cc=0 and (" + debar + " or r.Exam_Flag<>'debar') and (" + disc + " or r.DelFlag=0)";
                if (cc == "" && debar != "" && disc == "" && cancel != "")
                    strInclude = " and r.cc=0 and (" + debar + " or r.Exam_Flag<>'debar') and (" + cancel + " or r.DelFlag=0)";
                //
                if (cc == "" && debar == "" && disc != "" && cancel != "")
                    strInclude = " and r.cc=0 and r.Exam_Flag<>'debar' and (" + disc + " or " + cancel + " or r.DelFlag=0)";
                //3
                if (cc != "" && debar != "" && disc != "" && cancel == "")
                    strInclude = " and (" + cc + " or r.cc=0) and (" + debar + " or r.Exam_Flag<>'debar') and (" + disc + " or r.DelFlag=0)";
                if (cc != "" && debar == "" && disc != "" && cancel != "")
                    strInclude = " and (" + cc + " or r.cc=0) and r.Exam_Flag<>'debar' and (" + disc + " or " + cancel + " or r.DelFlag=0)";
                if (cc != "" && debar != "" && disc == "" && cancel != "")
                    strInclude = " and (" + cc + " or r.cc=0) and (" + debar + " or r.Exam_Flag<>'debar') and (" + cancel + " or r.DelFlag=0)";
                if (cc == "" && debar != "" && disc != "" && cancel != "")
                    strInclude = " and r.cc=0 and (" + debar + " or r.Exam_Flag<>'debar') and (" + disc + " or " + cancel + " or r.DelFlag=0)";
                if (cc == "" && debar == "" && disc == "" && cancel == "")
                    strInclude = " and r.cc=0 and r.Exam_Flag<>'debar' and  r.DelFlag=0";
                if (cc != "" && debar != "" && disc != "" && cancel != "")
                    strInclude = "";
            }
            else
            {
                if (cc != "" && debar == "" && disc == "" && cancel == "")
                    strInclude = " and " + cc + "";
                if (cc == "" && debar != "" && disc == "" && cancel == "")
                    strInclude = " and " + debar + "";
                if (cc == "" && debar == "" && disc != "" && cancel == "")
                    strInclude = " and " + disc + "";
                if (cc == "" && debar == "" && disc == "" && cancel != "")
                    strInclude = " and " + cancel + "";
                //2
                if (cc != "" && debar != "" && disc == "" && cancel == "")
                    strInclude = " and( " + cc + " or " + debar + ")";
                if (cc != "" && debar == "" && disc != "" && cancel == "")
                    strInclude = " and (" + cc + " or " + disc + ")";
                if (cc != "" && debar == "" && disc == "" && cancel != "")
                    strInclude = " and (" + cc + " or " + cancel + ")";
                //
                if (cc == "" && debar != "" && disc != "" && cancel == "")
                    strInclude = " and (" + debar + " or " + disc + ")";
                if (cc == "" && debar != "" && disc == "" && cancel != "")
                    strInclude = " and (" + debar + " or " + cancel + ")";
                //
                if (cc == "" && debar == "" && disc != "" && cancel != "")
                    strInclude = " and (" + disc + " or " + cancel + ")";
                //3
                if (cc != "" && debar != "" && disc != "" && cancel == "")
                    strInclude = " and (" + cc + " or " + debar + " or " + disc + ")";
                if (cc != "" && debar == "" && disc != "" && cancel != "")
                    strInclude = " and (" + cc + " or " + disc + " or " + cancel + ")";
                if (cc != "" && debar != "" && disc == "" && cancel != "")
                    strInclude = " and (" + cc + " or " + debar + " or " + cancel + ")";
                if (cc == "" && debar != "" && disc != "" && cancel != "")
                    strInclude = " and (" + debar + " or " + disc + " or " + cancel + ")";
                if (cc == "" && debar == "" && disc == "" && cancel == "")
                    strInclude = " and r.cc=0 and r.Exam_Flag<>'debar' and  r.DelFlag=0";
                if (cc != "" && debar != "" && disc != "" && cancel != "")
                    strInclude = " and (" + cc + " or " + debar + " or " + disc + " or " + cancel + ")";
            }

            #endregion
        }
        catch { }
        return strInclude;
    }
    //--------------------------added by abarna 29.03.2018--------------------------------------
    private void loadmemtype()
    {
        try
        {
            cblmem.Items.Clear();
            cblmem.Items.Add(new ListItem("Student", "1"));
            cblmem.Items.Add(new ListItem("Staff", "2"));
            cblmem.Items.Add(new ListItem("Vendor", "3"));
            cblmem.Items.Add(new ListItem("Others", "4"));
            if (cblmem.Items.Count > 0)
            {
                for (int i = 0; i < cblmem.Items.Count; i++)
                {
                    cblmem.Items[i].Selected = true;
                }
                cbmem.Checked = true;
                txtmem.Text = "MemType(" + cblmem.Items.Count + ")";

            }
        }
        catch { }
    }
    protected void cbmem_OnCheckedChanged(object sender, EventArgs e)
    {
        CallCheckboxChange(cbmem, cblmem, txtmem, "MemType", "--Select--");

    }
    protected void cblmem_OnSelectedIndexChanged(object sender, EventArgs e)
    {
        CallCheckboxListChange(cbmem, cblmem, txtmem, "MemType", "--Select--");

    }
}