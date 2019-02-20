using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Drawing;
using InsproDataAccess;
using System.Collections;
using System.Globalization;
using wc = System.Web.UI.WebControls;

public partial class ScheduleMOD_NewAlternateSchedule : System.Web.UI.Page
{
    #region Variable Declaration

    DataSet ds = new DataSet();
    Hashtable hatholiday = new Hashtable();
    ReuasableMethods rs = new ReuasableMethods();
    DataSet degreeDataset = new DataSet();
    DataTable dtCommon = new DataTable();
    Dictionary<string, string> dicQueryParameter = new Dictionary<string, string>();
    InsproStoreAccess storeAcc = new InsproStoreAccess();
    InsproDirectAccess dirAcc = new InsproDirectAccess();
    DAccess2 dacess = new DAccess2();
    SqlConnection getcon = new SqlConnection(ConfigurationManager.AppSettings["DSN"]);
    SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["DSN"]);
    SqlConnection cona = new SqlConnection(ConfigurationManager.AppSettings["DSN"]);
    SqlConnection con1a = new SqlConnection(ConfigurationManager.AppSettings["DSN"]);
    SqlConnection con2a = new SqlConnection(ConfigurationManager.AppSettings["DSN"]);
    SqlConnection con4a = new SqlConnection(ConfigurationManager.AppSettings["DSN"]);
    SqlConnection tempcon = new SqlConnection(ConfigurationManager.AppSettings["DSN"]);
    SqlCommand cmd;
    SqlCommand cmda;
    SqlCommand cmd1a;
    Hashtable hat = new Hashtable();
    Dictionary<string, Dictionary<int, string>> dicScheduledData = new Dictionary<string, Dictionary<int, string>>();
    Dictionary<string, Dictionary<int, string>> dicAlternateScheduledData = new Dictionary<string, Dictionary<int, string>>();
    Dictionary<string, Dictionary<string, string[]>> dicAlternateSubjectList = new Dictionary<string, Dictionary<string, string[]>>();
    ArrayList allotedstaff = new ArrayList();
    string start_dayorder = string.Empty;
    string RequestFromDate = string.Empty;
    string RequestToDate = string.Empty;
    string cellTagValue = string.Empty;
    static int noOfHoursPerDay = 0;
    int noScheduleCnt = 0;
    int checkSemDateCnt = 0;
    bool holiday = false;
    string holidayDescription = "";
    bool isBussyStaff = false;
    bool isnew = false;

    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["collegecode"] == null) //Aruna For Back Button
        {
            Response.Redirect("~/Default.aspx");
        }
        //magesh 13/2/18
        if (Session["forrequest"] != null)
        {
            isnew = true;
        }
        if (Session["forreqstaff"] != null)
        {
            isnew = true;
        }//magesh 13/2/18
        //magesh 8.3.18
        if (Session["staffalter"] != null)
        {
            Session["staconform"] = "yes";
        }

        if (!IsPostBack)
        {
            Bindcollege();
            bindbatch();
            bindBranch();
            txtDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            txtDate.Attributes.Add("readonly", "readonly");
            txttoDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
            txttoDate.Attributes.Add("readonly", "readonly");
            subDiv.Visible = false;
            btnMasterSave.Visible = false;
            BindCollege();
            BindAlterStaffDepartment((ddlAlterFreeCollege.Items.Count > 0) ? Convert.ToString(ddlAlterFreeCollege.SelectedValue).Trim() : "");

            if (Request.QueryString["@@BB$$"] != null)
            {
                string RequestPageReDirectValue = Request.QueryString["@@BB$$"];//1@01/12/2017$05/12/2017
                Session["RequestPageReDirect"] = RequestPageReDirectValue;
                string[] RequestValue = RequestPageReDirectValue.Split('@');
                if (RequestValue.Length == 2)
                {
                    string[] Date1 = RequestValue[1].Split('$');
                    RequestFromDate = Convert.ToString(Date1[0]);
                    RequestToDate = Convert.ToString(Date1[1]);
                    txtDate.Text = RequestFromDate;
                    txttoDate.Text = RequestToDate;
                    txtDate.Enabled = false;
                    txttoDate.Enabled = false;
                }
            }

        }

    }

    #region college

    public void Bindcollege()
    {
        try
        {
            ddlCollege.Items.Clear();
            dtCommon.Clear();
            ddlCollege.Enabled = false;
            DataSet dsprint = new DataSet();
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
            }
        }
        catch (Exception ex)
        {

        }
    }

    private void BindCollege()
    {
        try
        {
            ddlAlterFreeCollege.Items.Clear();
            string qry = "select collname,college_code from collinfo";
            DataTable dtCollege = dirAcc.selectDataTable(qry);
            if (dtCollege.Rows.Count > 0)
            {
                ddlAlterFreeCollege.DataSource = dtCollege;
                ddlAlterFreeCollege.DataTextField = "collname";
                ddlAlterFreeCollege.DataValueField = "college_code";
                ddlAlterFreeCollege.DataBind();
            }
        }
        catch
        {
        }
    }
    private void BindAlterStaffDepartment(string collegeCode)
    {
        try
        {
            ddlAlterFreeDepartment.Items.Clear();
            DataTable dtDept = new DataTable();
            string qry = "";
            if (!string.IsNullOrEmpty(collegeCode))
            {
                qry = "select distinct dept_name,dept_code from hrdept_master where college_code='" + collegeCode + "'";
                dtDept = dirAcc.selectDataTable(qry);
            }
            if (dtDept.Rows.Count > 0)
            {
                ddlAlterFreeDepartment.DataSource = dtDept;
                ddlAlterFreeDepartment.DataTextField = "dept_name";
                ddlAlterFreeDepartment.DataValueField = "dept_code";
                ddlAlterFreeDepartment.DataBind();
                ddlAlterFreeDepartment.Items.Insert(0, new ListItem("All", ""));
            }
        }
        catch
        {
        }
    }
    #endregion

    #region Batch
    public void bindbatch()
    {
        cbl_batch.Items.Clear();
        ds = dacess.select_method_wo_parameter("bind_batch", "sp");
        int count = ds.Tables[0].Rows.Count;
        if (count > 0)
        {
            cbl_batch.DataSource = ds;
            cbl_batch.DataTextField = "batch_year";
            cbl_batch.DataValueField = "batch_year";
            cbl_batch.DataBind();
        }
        if (cbl_batch.Items.Count > 0)
        {
            for (int i = 0; i < cbl_batch.Items.Count; i++)
            {
                cbl_batch.Items[i].Selected = true;
            }
            txt_batch.Text = lblbatch.Text + "(" + cbl_batch.Items.Count + ")";
            cb_batch.Checked = true;
        }

    }

    public void bindBranch()
    {
        try
        {

            ds.Clear();
            txtBranch.Text = "---Select---";
            string batchCode = string.Empty;
            chkBranch.Checked = false;
            cblBranch.Items.Clear();
            string collegeCode = string.Empty;
            if (ddlCollege.Items.Count > 0)
                collegeCode = ddlCollege.SelectedValue.ToString().Trim();

            string columnfield = string.Empty;
            string group_user = ((Session["group_code"] != null) ? Convert.ToString(Session["group_code"]) : string.Empty);
            if (group_user.Contains(';'))
            {
                string[] group_semi = group_user.Split(';');
                group_user = Convert.ToString(group_semi[0]);
            }
            if ((Convert.ToString(group_user).Trim() != "") && Session["single_user"] != null && (Convert.ToString(Session["single_user"]) != "1" && Convert.ToString(Session["single_user"]) != "true" && Convert.ToString(Session["single_user"]) != "TRUE" && Convert.ToString(Session["single_user"]) != "True"))
            {
                columnfield = " and dp.group_code='" + group_user + "'";
            }
            else if (Session["usercode"] != null)
            {
                columnfield = " and dp.user_code='" + Convert.ToString(Session["usercode"]).Trim() + "'";
            }
            string valBatch = string.Empty;
            if (cbl_batch.Items.Count > 0)
                valBatch = rs.GetSelectedItemsValueAsString(cbl_batch);

            if (!string.IsNullOrEmpty(collegeCode) && !string.IsNullOrEmpty(valBatch))
            {
                string selBranch = "SELECT DISTINCT dg.Degree_Code,(c.Course_Name+'-'+dt.Dept_Name) as Dept_Name,CASE WHEN c.Priority IS NULL THEN c.Course_Id ELSE c.Priority END OrderBy FROM Degree dg,Course c,Department dt,DeptPrivilages dp,Registration r WHERE r.degree_code = dg.Degree_Code AND dp.degree_code = dg.Degree_Code AND dg.Course_Id = c.Course_Id AND dg.Dept_Code = dt.Dept_Code AND r.college_code = c.college_code AND r.college_code = dg.college_code AND dt.college_code = r.college_code AND c.college_code = dg.college_code AND r.CC='0' and r.DelFlag='0' and r.Exam_Flag<>'debar' AND r.college_code in('" + collegeCode + "') AND r.Batch_Year in('" + valBatch + "')" + columnfield + " ORDER BY dg.Degree_Code, CASE WHEN c.Priority IS NULL THEN c.Course_Id ELSE c.Priority END ";
                ds = dacess.select_method_wo_parameter(selBranch, "Text");

            }
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                cblBranch.DataSource = ds;
                cblBranch.DataTextField = "Dept_Name";
                cblBranch.DataValueField = "Degree_Code";
                cblBranch.DataBind();
                checkBoxListselectOrDeselect(cblBranch, true);
                CallCheckboxListChange(chkBranch, cblBranch, txtBranch, lblBranch.Text, "--Select--");
            }
        }
        catch
        {
        }
    }

    protected void cb_batch_OnCheckedChanged(object sender, EventArgs e)
    {
        CallCheckboxChange(cb_batch, cbl_batch, txt_batch, lblbatch.Text, "--Select--");
        bindBranch();
    }
    protected void cbl_batch_OnSelectedIndexChanged(object sender, EventArgs e)
    {
        CallCheckboxListChange(cb_batch, cbl_batch, txt_batch, lblbatch.Text, "--Select--");
        bindBranch();
    }
    protected void chkBranch_OnCheckedChanged(object sender, EventArgs e)
    {
        CallCheckboxChange(chkBranch, cblBranch, txtBranch, lblBranch.Text, "--Select--");
    }
    protected void cblBranch_OnSelectedIndexChanged(object sender, EventArgs e)
    {
        CallCheckboxListChange(chkBranch, cblBranch, txtBranch, lblBranch.Text, "--Select--");

    }
    #endregion

    public string GetFunction(string Att_strqueryst)
    {
        try
        {
            string sqlstr;
            sqlstr = Att_strqueryst;
            getcon.Close();
            getcon.Open();
            SqlDataReader drnew;
            SqlCommand cmd = new SqlCommand(sqlstr, getcon);
            drnew = cmd.ExecuteReader();
            drnew.Read();
            if (drnew.HasRows == true)
            {
                return drnew[0].ToString();
            }
            else
            {
                return string.Empty;
            }
        }
        catch
        {
            return string.Empty;
        }
    }
    protected void btnGo_Click(object sender, EventArgs e)
    {
        try
        {

            btnMasterSave.Visible = false;
            norecordlbl.Visible = false;
            norecordlbl.Text = "";
            string StaffCode = string.Empty;
            degreeDataset.Clear();
            dicScheduledData.Clear();
            dicAlternateScheduledData.Clear();
            string valDegree = string.Empty;
            int hrsPerDay = 0;

            if (cblBranch.Items.Count == 0)
            {
                Label12.Visible = true;
                Label12.Text = "No " + lblBranch.Text + " Found";
                Div3.Visible = true;
                return;
            }
            else
            {
                valDegree = rs.GetSelectedItemsValueAsString(cblBranch);
                if (string.IsNullOrEmpty(valDegree))
                {
                    Label12.Visible = true;
                    Label12.Text = "Select Atleast One " + lblBranch.Text + "";
                    Div3.Visible = true;
                    return;
                }
            }
            string selectedDate = txtDate.Text.ToString();
            string[] splitSelectedDate = selectedDate.Split(new Char[] { '/' });
            selectedDate = splitSelectedDate[1].ToString() + "/" + splitSelectedDate[0].ToString() + "/" + splitSelectedDate[2].ToString();
            DateTime dtSelectedDate = Convert.ToDateTime(selectedDate.ToString());

            string selectedToDate = txttoDate.Text.ToString();
            string[] splitSelectedToDate = selectedToDate.Split(new Char[] { '/' });
            selectedDate = splitSelectedToDate[1].ToString() + "/" + splitSelectedToDate[0].ToString() + "/" + splitSelectedToDate[2].ToString();
            DateTime dtSelectedToDate = Convert.ToDateTime(selectedDate.ToString());

            StaffCode = Session["staff_code"].ToString().Trim();
            string selectedBatchYears = Convert.ToString(getCblSelectedValue(cbl_batch));
            string qry = " select distinct (CONVERT(varchar,r.Batch_Year)+' - '+c.Course_Name+' ('+de.dept_acronym+') - '+CONVERT(varchar, r.Current_Semester)+' '+ISNULL(r.Sections,''))Degree ,r.Batch_Year,r.degree_code,r.Current_Semester,ISNULL(r.Sections,'')Section,(CONVERT(varchar,r.Batch_Year)+' - '+CONVERT(varchar,r.degree_code)+' - '+CONVERT(varchar, r.Current_Semester)+' - '+ISNULL(r.Sections,''))Code from Registration r,Degree d,Department de,course c where r.degree_code=d.Degree_Code and d.Dept_Code=de.Dept_Code and d.Course_Id=c.Course_Id and r.cc=0  and r.Exam_Flag<>'debar' and r.DelFlag=0 and r.Batch_Year in ('" + selectedBatchYears + "') and r.degree_code in('" + valDegree + "') and r.college_code='" + ddlCollege.SelectedValue + "' order by r.Batch_Year desc";
            degreeDataset = dacess.select_method_wo_parameter(qry, "Text");

            //get No_of_hrs_per_day,schorder,nodays
            if (degreeDataset.Tables.Count > 0 && degreeDataset.Tables[0].Rows.Count > 0)
            {

                for (int i = 0; i < degreeDataset.Tables[0].Rows.Count; i++)
                {
                    for (DateTime caldate = dtSelectedDate; caldate <= dtSelectedToDate; caldate = caldate.AddDays(1))
                    {
                        string[] caldtesplit = Convert.ToString(caldate).Split(' ');
                        string[] datesplit = Convert.ToString(caldtesplit[0]).Split('/');
                        string date = datesplit[1] + '/' + datesplit[0] + '/' + datesplit[2];
                        string curDate = datesplit[0] + '/' + datesplit[1] + '/' + datesplit[2];
                        DateTime dt;
                        DateTime.TryParse(curDate, out dt);

                        Dictionary<int, string> dic_scheduleddata = new Dictionary<int, string>();
                        Dictionary<int, string> dic_alterScheduleddata = new Dictionary<int, string>();

                        int schOrder = 0;
                        int noOfDays = 0;
                        int frstHlfHour = 0;
                        string semStartdate = string.Empty;
                        string semEnddate = string.Empty;
                        string start_dayorder = string.Empty;
                        string selectedDate_day = string.Empty;
                        string holidyreasn = string.Empty;
                        Boolean noflag = false;
                        string splValNew = string.Empty;
                        string splval = string.Empty;
                        string setcellnote = string.Empty;
                        Boolean alterflag = true;
                        int rowval = 0;
                        string printDegree = Convert.ToString(degreeDataset.Tables[0].Rows[i]["Degree"]);
                        string degCode = Convert.ToString(degreeDataset.Tables[0].Rows[i]["degree_code"]);
                        string sem = Convert.ToString(degreeDataset.Tables[0].Rows[i]["Current_Semester"]);
                        string batchYear = Convert.ToString(degreeDataset.Tables[0].Rows[i]["Batch_Year"]);
                        string sec = Convert.ToString(degreeDataset.Tables[0].Rows[i]["Section"]);
                        string str_sec;

                        if (sec == "" || sec == "-1")
                        {
                            str_sec = string.Empty;
                        }
                        else
                        {
                            str_sec = " and sections='" + sec + "'";
                        }

                        string qry1 = "Select No_of_hrs_per_day,schorder,nodays,no_of_hrs_I_half_day from periodattndschedule where degree_code=" + degCode + " and semester = " + sem + "";
                        DataSet dataSet = dacess.select_method_wo_parameter(qry1, "Text");
                        if (dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
                        {
                            hrsPerDay = Convert.ToInt32(dataSet.Tables[0].Rows[0]["No_of_hrs_per_day"]);
                            schOrder = Convert.ToInt32(dataSet.Tables[0].Rows[0]["schorder"]);
                            noOfDays = Convert.ToInt32(dataSet.Tables[0].Rows[0]["nodays"]);
                            frstHlfHour = Convert.ToInt32(dataSet.Tables[0].Rows[0]["no_of_hrs_I_half_day"]);
                            noOfHoursPerDay = hrsPerDay;
                        }
                        //--------------get semester information ie schedule
                        string semInfoQry = "select * from seminfo where degree_code='" + degCode + "' and semester='" + sem + "' and batch_year='" + batchYear + "'";
                        DataSet semInfoDataSet = dacess.select_method_wo_parameter(semInfoQry, "Text");
                        if (semInfoDataSet.Tables.Count > 0 && semInfoDataSet.Tables[0].Rows.Count > 0)
                        {
                            if ((semInfoDataSet.Tables[0].Rows[0]["start_date"].ToString()) != "" && (semInfoDataSet.Tables[0].Rows[0]["start_date"].ToString()) != "\0")
                            {
                                string[] tmpdate = Convert.ToString(semInfoDataSet.Tables[0].Rows[0]["start_date"]).Split(new char[] { ' ' });
                                semStartdate = tmpdate[0].ToString();
                                string[] enddate = Convert.ToString(semInfoDataSet.Tables[0].Rows[0]["end_date"]).Split(new char[] { ' ' });
                                semEnddate = enddate[0].ToString();

                                if (Convert.ToString(semInfoDataSet.Tables[0].Rows[0]["starting_dayorder"]) != "")
                                {
                                    start_dayorder = Convert.ToString(semInfoDataSet.Tables[0].Rows[0]["starting_dayorder"]);
                                }
                                else
                                {
                                    start_dayorder = "1";
                                }
                            }
                            else
                            {
                                norecordlbl.Visible = true;
                                norecordlbl.Text = "Update semester Information";
                                norecordlbl.ForeColor = Color.Red;
                                return;
                            }
                        }
                        else
                        {
                            norecordlbl.Visible = true;
                            norecordlbl.Text = "Update semester Information";
                            norecordlbl.ForeColor = Color.Red;
                            return;
                        }
                        // Day Order Change=======Start====================
                        string dayorderQry = "Select * from tbl_consider_day_order where degree_code='" + degCode + "' and semester='" + sem + "' and batch_year='" + batchYear + "'  and ((From_Date between '" + dt.ToString("yyyy-MM-dd") + "' and '" + dt.ToString("yyyy-MM-dd") + "') or (To_Date between '" + dt.ToString("yyyy-MM-dd") + "' and '" + dt.ToString("yyyy-MM-dd") + "'))";

                        DataSet dayorderDataSet = dacess.select_method_wo_parameter(dayorderQry, "Text");
                        Hashtable hatDayOrderChange = new Hashtable();
                        for (int doc = 0; doc < dayorderDataSet.Tables[0].Rows.Count; doc++)
                        {
                            DateTime dtFromDate = Convert.ToDateTime(dayorderDataSet.Tables[0].Rows[doc]["from_date"].ToString());
                            DateTime dtEndDate = Convert.ToDateTime(dayorderDataSet.Tables[0].Rows[doc]["to_date"].ToString());
                            string reason = dayorderDataSet.Tables[0].Rows[doc]["Reason"].ToString();
                            for (DateTime dtChangeDate = dtFromDate; dtChangeDate <= dtEndDate; dtChangeDate = dtChangeDate.AddDays(1))
                            {
                                if (!hatDayOrderChange.Contains(dtChangeDate))
                                {
                                    hatDayOrderChange.Add(dtChangeDate, reason);
                                }
                            }
                        }
                        //=================================End======================================          
                        //------------find schedule order type
                        if (hrsPerDay > 0)
                        {
                            if (schOrder != 0)
                            {
                                selectedDate_day = dt.ToString("ddd");
                            }
                            else
                            {
                                selectedDate_day = dacess.findday(selectedDate.ToString(), degCode, sem, batchYear, semStartdate.ToString(), noOfDays.ToString(), start_dayorder.ToString());
                            }
                        }
                        if (semStartdate != "" && semEnddate != "")
                        {
                            if ((dt >= Convert.ToDateTime(semStartdate) && dt <= Convert.ToDateTime(semEnddate)))
                            {
                                if (selectedDate_day != "Sun")
                                {
                                    string sqlsrt = "select top 1 ";
                                    string noOfAlterQry = "select no_of_alter,";

                                    for (int j = 1; j <= hrsPerDay; j++)
                                    {
                                        sqlsrt = sqlsrt + selectedDate_day + j.ToString() + ",";
                                        noOfAlterQry = noOfAlterQry + selectedDate_day + j.ToString() + ",";
                                    }

                                    string holidayStudentsQry = "select * from holidaystudents  where degree_code=" + degCode + " and semester=" + sem + " and holiday_date ='" + dt.ToString() + "'";
                                    string holidayReason = string.Empty;
                                    Boolean morleave = false;
                                    Boolean eveleave = false;
                                    DataSet holidayStudentsDataSet = dacess.select_method_wo_parameter(holidayStudentsQry, "Text");
                                    if (holidayStudentsDataSet.Tables.Count > 0 && holidayStudentsDataSet.Tables[0].Rows.Count > 0)
                                    {
                                        holidayReason = Convert.ToString(holidayStudentsDataSet.Tables[0].Rows[0]["holiday_desc"]);
                                        holidayDescription = Convert.ToString(holidayStudentsDataSet.Tables[0].Rows[0]["holiday_desc"]);
                                        string hlfOrFull = Convert.ToString(holidayStudentsDataSet.Tables[0].Rows[0]["halforfull"]);
                                        if (hlfOrFull.Trim() == "1" || hlfOrFull.Trim().ToLower() == "true")
                                        {
                                            if (Convert.ToString(holidayStudentsDataSet.Tables[0].Rows[0]["morning"]).Trim() == "1" || Convert.ToString(holidayStudentsDataSet.Tables[0].Rows[0]["morning"]).Trim().ToLower() == "true")
                                            {
                                                morleave = true;
                                            }
                                            if (Convert.ToString(holidayStudentsDataSet.Tables[0].Rows[0]["evening"]).Trim() == "1" || Convert.ToString(holidayStudentsDataSet.Tables[0].Rows[0]["evening"]).Trim().ToLower() == "true")
                                            {
                                                eveleave = true;
                                            }
                                            if (!hatholiday.Contains(degCode + "-" + batchYear + "-" + dt.ToString()))
                                                hatholiday.Add(degCode + "-" + batchYear + "-" + dt.ToString(), holidayDescription + "-" + eveleave + "-" + morleave);
                                        }
                                        else
                                        {
                                            morleave = true;
                                            eveleave = true;
                                            holiday = true;
                                            if (!hatholiday.Contains(degCode + "-" + batchYear + "-" + dt.ToString()))
                                                hatholiday.Add(degCode + "-" + batchYear + "-" + dt.ToString(), holidayDescription + "-" + eveleave + "-" + morleave);
                                        }
                                    }

                                    string sqlQry = sqlsrt + " degree_code,semester,batch_year from semester_schedule where batch_year=" + batchYear + " and degree_code = " + degCode + " and semester = " + sem + " and FromDate<= ' " + dt + " ' " + str_sec + " order by fromdate desc";

                                    DataSet sqlQryDataSet = dacess.select_method_wo_parameter(sqlQry, "Text");

                                    string alternateValueQry = sqlsrt + " degree_code , semester , batch_year from Alternate_schedule where batch_year=" + batchYear + " and degree_code = " + degCode + " and semester = " + sem + " and FromDate= '" + dt + "' " + str_sec + "";
                                    DataSet alternateValueDataSet = dacess.select_method(alternateValueQry, hat, "Text");

                                    if (sqlQryDataSet.Tables.Count > 0 && sqlQryDataSet.Tables[0].Rows.Count > 0)
                                    {
                                        if (holidyreasn == "")
                                        {
                                            holidyreasn = dt.ToString("dd/MM/yyyy") + " is Holiday- " + holidayReason;
                                        }
                                        else
                                        {
                                            holidyreasn = holidyreasn + ',' + dt.ToString("dd/MM/yyyy") + " is Holiday- " + holidayReason;
                                        }
                                        for (int hr = 1; hr <= hrsPerDay; hr++)
                                        {

                                            Boolean leavefa = false;
                                            if (morleave == true)
                                            {
                                                if (hr < frstHlfHour + 1)
                                                {
                                                    leavefa = true;
                                                }
                                            }
                                            if (eveleave == true)
                                            {
                                                if (hr > frstHlfHour)
                                                {
                                                    leavefa = true;
                                                }
                                            }
                                            if (leavefa == true)
                                            {
                                                if (holidayReason != "" && holidayReason != null)
                                                {
                                                    if ((Convert.ToString(sqlQryDataSet.Tables[0].Rows[0][hr - 1])) != "" && (Convert.ToString(sqlQryDataSet.Tables[0].Rows[0][hr - 1])) != "\0")
                                                    {
                                                        splValNew = holidayReason;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                noflag = true;
                                                if ((Convert.ToString(sqlQryDataSet.Tables[0].Rows[0][hr - 1])) != "" && (Convert.ToString(sqlQryDataSet.Tables[0].Rows[0][hr - 1])) != "\0")
                                                {
                                                    //============Day Order Change =================
                                                    if (hatDayOrderChange.Contains(dt))
                                                    {
                                                        splValNew = hatDayOrderChange[dt].ToString();
                                                        //spreadDet.Sheets[0].Cells[hr - 1, 0].Locked = true;
                                                    }
                                                    else
                                                    {
                                                        string[] subjnew = (Convert.ToString(sqlQryDataSet.Tables[0].Rows[0][hr - 1])).Split(new Char[] { ';' });
                                                        string txt = string.Empty;
                                                        string Valu = Convert.ToString(sqlQryDataSet.Tables[0].Rows[0][hr - 1]);
                                                        for (int l = 0; l <= subjnew.GetUpperBound(0); l++)
                                                        {
                                                            txt = string.Empty;
                                                            if (subjnew.GetUpperBound(0) >= 0)
                                                            {
                                                                string[] subjstr = subjnew[l].Split(new Char[] { '-' });
                                                                txt = Convert.ToString(subjnew[l]);
                                                                if (subjstr.GetUpperBound(0) >= 2)
                                                                {
                                                                    if (!string.IsNullOrEmpty(StaffCode))
                                                                    {
                                                                        for (int m = 0; m < subjstr.Length; m++)
                                                                        {
                                                                            if (StaffCode == subjstr[m].ToString().Trim())
                                                                            {
                                                                                string strsub = GetFunction("select subject_name from subject where subject_no=" + subjstr[0] + " ");
                                                                                getcon.Close();
                                                                                if (!splValNew.Contains(StaffCode))
                                                                                {
                                                                                    if (!string.IsNullOrEmpty(txt))
                                                                                    {
                                                                                        txt = txt.Replace(subjstr[0], strsub);
                                                                                        splValNew = splValNew + (txt) + ";";
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        splValNew = splValNew + ((strsub.ToString()) + "-" + subjstr[1] + "-" + subjstr[2]) + ";";
                                                                                    }

                                                                                    //splValNew = splValNew + ((strsub.ToString()) + "-" + subjstr[1] + "-" + subjstr[2]) + ";";
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        string strsub = GetFunction("select subject_name from subject where subject_no=" + subjstr[0] + " ");
                                                                        getcon.Close();
                                                                        splValNew = splValNew + ((strsub.ToString()) + "-" + subjstr[1] + "-" + subjstr[2]) + ";";

                                                                    }
                                                                }
                                                            }
                                                        }
                                                        dic_scheduleddata.Add(hr, splValNew + "@" + Valu);
                                                    }
                                                }
                                            }
                                            splValNew = string.Empty;

                                            if (alterflag) //Alternate Schedule Details
                                            {

                                                alterflag = false;
                                                string alternateDetailsQry = alternateValueQry;
                                                int noaltval = 1;
                                                if (noaltval > 1)
                                                {
                                                    alternateDetailsQry = noOfAlterQry + "degree_code,semester,batch_year from tbl_alter_schedule_Details where batch_year=" + batchYear + " and degree_code = " + degCode + " and semester = " + sem + " and FromDate= ' " + dt.ToString() + " ' " + str_sec + " order by no_of_alter, fromdate desc";
                                                }

                                                DataSet alternateDetailsDataSet = dacess.select_method(alternateDetailsQry, hat, "Text");
                                                if (alternateDetailsDataSet.Tables[0].Rows.Count > 0)
                                                {
                                                    for (int hour = 1; hour <= hrsPerDay; hour++)
                                                    {
                                                        for (int alternateHour = 0; alternateHour < alternateDetailsDataSet.Tables[0].Rows.Count; alternateHour++)
                                                        {
                                                            if (alternateHour + 1 <= 1)
                                                            {
                                                                string column = selectedDate_day + hour;
                                                                string value = alternateDetailsDataSet.Tables[0].Rows[alternateHour]["" + column + ""].ToString().Trim();
                                                                splval = string.Empty;
                                                                leavefa = false;
                                                                if (morleave == true)
                                                                {
                                                                    if (hour < frstHlfHour + 1)
                                                                    {
                                                                        leavefa = true;
                                                                    }
                                                                }
                                                                if (eveleave == true)
                                                                {
                                                                    if (hour > frstHlfHour)
                                                                    {
                                                                        leavefa = true;
                                                                    }
                                                                }
                                                                if (leavefa == true)
                                                                {
                                                                    if (holidayReason != "" && holidayReason != null)
                                                                    {
                                                                        if (value != "" && value != "\0")
                                                                        {
                                                                            splval = string.Empty;
                                                                        }
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    if (value != "" && value != "\0")
                                                                    {
                                                                        if (hatDayOrderChange.Contains(dt))
                                                                        {

                                                                        }
                                                                        else
                                                                        {
                                                                            setcellnote = value;
                                                                            string[] sple = (value).Split(new Char[] { ';' });
                                                                            string txt = string.Empty;
                                                                            for (int ii = 0; ii <= sple.GetUpperBound(0); ii++)
                                                                            {
                                                                                txt = string.Empty;
                                                                                int isalter = 0;
                                                                                if (sple.GetUpperBound(0) >= 0)
                                                                                {
                                                                                    string[] sp1 = (sple[ii].ToString()).Split(new Char[] { '-' });
                                                                                    txt = sple[ii].ToString();
                                                                                    if (sp1.GetUpperBound(0) >= 2)
                                                                                    {
                                                                                        if (!string.IsNullOrEmpty(StaffCode))
                                                                                        {
                                                                                            //for (int m = 0; m < sp1.Length; m++)
                                                                                            //{
                                                                                            //    if (StaffCode == sp1[m].ToString().Trim())
                                                                                            //    {
                                                                                            //        if (!string.IsNullOrEmpty(txt))
                                                                                            //        {
                                                                                            //            string subName = string.Empty;
                                                                                            //            subName = (GetFunction("select subject_name from subject where subject_no=" + sp1[0].ToString() + " "));
                                                                                            //            txt = txt.Replace(sp1[0].ToString(), subName);
                                                                                            //            //splval = splval + (GetFunction("select subject_name from subject where subject_no=" + sp1[0].ToString() + " ") + "-" + sp1[1].ToString() + "-" + sp1[2].ToString()) + ";";
                                                                                            //            splval = splval + txt;
                                                                                            //        }
                                                                                            //        else
                                                                                            //        {
                                                                                            //            splval = splval + (GetFunction("select subject_name from subject where subject_no=" + sp1[0].ToString() + " ") + "-" + sp1[1].ToString() + "-" + sp1[2].ToString()) + ";";
                                                                                            //        }
                                                                                            //    }
                                                                                            //    else
                                                                                            //    {
                                                                                            //        if (hour != isalter)
                                                                                            //        {
                                                                                            //            isalter = hour;
                                                                                            //            string alternatestaff = dacess.GetFunction("select alterStaffCode from alternateStaffDetails where alternateDate='" + dt.ToString() + "' and actualStaffCode='" + StaffCode + "'  and alterHour='" + hour + "' and subjectNo='" + sp1[0].ToString() + "'");
                                                                                            //            if (!string.IsNullOrEmpty(alternatestaff) && alternatestaff != "0")
                                                                                            //                splval = splval + (GetFunction("select subject_name from subject where subject_no=" + sp1[0].ToString() + " ") + "-" + sp1[1].ToString() + "-" + sp1[2].ToString()) + ";";
                                                                                            //        }

                                                                                            //    }
                                                                                            //}
                                                                                            if (hour != isalter)
                                                                                            {

                                                                                                string subName = string.Empty;
                                                                                                subName = (GetFunction("select subject_name from subject where subject_no=" + sp1[0].ToString() + " "));
                                                                                                txt = txt.Replace(sp1[0].ToString(), subName);
                                                                                                //splval = splval + (GetFunction("select subject_name from subject where subject_no=" + sp1[0].ToString() + " ") + "-" + sp1[1].ToString() + "-" + sp1[2].ToString()) + ";";
                                                                                                splval = splval + txt;
                                                                                            }
                                                                                            //splval = splval + (GetFunction("select subject_name from subject where subject_no=" + sp1[0].ToString() + " ") + "-" + sp1[1].ToString() + "-" + sp1[2].ToString()) + ";";
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            if (sp1.GetUpperBound(0) >= 2)
                                                                                            {
                                                                                                splval = splval + (GetFunction("select subject_name from subject where subject_no=" + sp1[0].ToString() + " ") + "-" + sp1[1].ToString() + "-" + sp1[2].ToString()) + ";";
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                        dic_alterScheduleddata.Add(hour, (splval + "@" + value));
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {

                                        dic_scheduleddata.Add(1, "No Schedule");
                                        noScheduleCnt++;
                                    }
                                }
                                else
                                {
                                    for (int j = 1; j <= hrsPerDay; j++)
                                    {
                                        dic_alterScheduleddata.Add(j, ("Sunday Holiday" + "@" + "Sunday Holiday"));
                                        dic_scheduleddata.Add(j, "Sunday Holiday" + "@" + "Sunday Holiday");
                                    }
                                }
                            }
                            else
                            {
                                dic_scheduleddata.Add(1, "The selected date must be between Semester date");
                                checkSemDateCnt++;
                                gridTimeTable.Visible = false;
                                //return "";
                            }
                        }
                        else
                        {
                            dic_scheduleddata.Add(1, "Update semester Information");
                            checkSemDateCnt++;
                            gridTimeTable.Visible = false;
                        }
                        rowval = 0;
                        splValNew = string.Empty;
                        splval = string.Empty;
                        if (dic_scheduleddata.Count != 0)
                            dicScheduledData.Add(printDegree + "$" + curDate, dic_scheduleddata);
                        if (dic_alterScheduleddata.Count != 0)
                            dicAlternateScheduledData.Add(printDegree + "$" + curDate, dic_alterScheduleddata);

                    }
                }
            }
            if (true)//holiday == false
            {
                if ((dicScheduledData.Count != 0 || dicAlternateScheduledData.Count != 0) && (noScheduleCnt != dicScheduledData.Count && checkSemDateCnt != dicScheduledData.Count))//
                {
                    subDiv.Visible = true;
                    //loadspreadDetails(noOfHours);
                    loadspreadDetails();
                }
                else
                {
                    if (noScheduleCnt != 0)
                        norecordlbl.Text = "No Schedule for selected date";
                    else
                        norecordlbl.Text = "The Selected date must be between Semester date";
                    norecordlbl.Visible = true;
                    subDiv.Visible = false;
                    gridTimeTable.Visible = false;

                }
            }
            else
            {
                norecordlbl.Text = "The Selected date is Holiday";
                norecordlbl.Visible = true;
            }



        }
        catch (Exception ex) { }
    }
    protected void loadspreadDetails()
    {
        try
        {
            DataTable dtTTDisp = new DataTable();
            dtTTDisp.Columns.Add("DateDisp");
            dtTTDisp.Columns.Add("DateVal");
            dtTTDisp.Columns.Add("DayOrder");
            dtTTDisp.Columns.Add("Degree");
            dtTTDisp.Columns.Add("DegreeVal");
            dtTTDisp.Columns.Add("P1ValDisp");
            dtTTDisp.Columns.Add("P1Val");
            dtTTDisp.Columns.Add("TT_1");
            dtTTDisp.Columns.Add("P2ValDisp");
            dtTTDisp.Columns.Add("P2Val");
            dtTTDisp.Columns.Add("TT_2");
            dtTTDisp.Columns.Add("P3ValDisp");
            dtTTDisp.Columns.Add("P3Val");
            dtTTDisp.Columns.Add("TT_3");
            dtTTDisp.Columns.Add("P4ValDisp");
            dtTTDisp.Columns.Add("P4Val");
            dtTTDisp.Columns.Add("TT_4");
            dtTTDisp.Columns.Add("P5ValDisp");
            dtTTDisp.Columns.Add("P5Val");
            dtTTDisp.Columns.Add("TT_5");
            dtTTDisp.Columns.Add("P6ValDisp");
            dtTTDisp.Columns.Add("P6Val");
            dtTTDisp.Columns.Add("TT_6");
            dtTTDisp.Columns.Add("P7ValDisp");
            dtTTDisp.Columns.Add("P7Val");
            dtTTDisp.Columns.Add("TT_7");
            dtTTDisp.Columns.Add("P8ValDisp");
            dtTTDisp.Columns.Add("P8Val");
            dtTTDisp.Columns.Add("TT_8");
            dtTTDisp.Columns.Add("P9ValDisp");
            dtTTDisp.Columns.Add("P9Val");
            dtTTDisp.Columns.Add("TT_9");
            dtTTDisp.Columns.Add("P10ValDisp");
            dtTTDisp.Columns.Add("P10Val");
            dtTTDisp.Columns.Add("TT_10");
            DataRow drSch = null;

            Dictionary<int, string> dic = new Dictionary<int, string>();
            Dictionary<int, string> alterDic = new Dictionary<int, string>();

            string selectedDate = txtDate.Text.ToString();
            string[] splitSelectedDate = selectedDate.Split(new Char[] { '/' });
            selectedDate = splitSelectedDate[1].ToString() + "/" + splitSelectedDate[0].ToString() + "/" + splitSelectedDate[2].ToString();
            DateTime dtSelectedDate = Convert.ToDateTime(selectedDate.ToString());

            string selectedToDate = txttoDate.Text.ToString();
            string[] splitSelectedToDate = selectedToDate.Split(new Char[] { '/' });
            selectedDate = splitSelectedToDate[1].ToString() + "/" + splitSelectedToDate[0].ToString() + "/" + splitSelectedToDate[2].ToString();
            DateTime dtSelectedToDate = Convert.ToDateTime(selectedDate.ToString());


            for (int cnt = 0; cnt < degreeDataset.Tables[0].Rows.Count; cnt++)
            {
                for (DateTime caldate = dtSelectedDate; caldate <= dtSelectedToDate; caldate = caldate.AddDays(1))
                {
                    dic.Clear();
                    alterDic.Clear();
                    string[] caldtesplit = Convert.ToString(caldate).Split(' ');
                    string[] datesplit = Convert.ToString(caldtesplit[0]).Split('/');
                    string date = datesplit[1] + '/' + datesplit[0] + '/' + datesplit[2];
                    string curDate = datesplit[0] + '/' + datesplit[1] + '/' + datesplit[2];
                    DateTime dt;

                    DateTime.TryParse(curDate, out dt);
                    if (dicScheduledData.ContainsKey(Convert.ToString(degreeDataset.Tables[0].Rows[cnt]["Degree"]) + "$" + curDate))
                        dic = dicScheduledData[Convert.ToString(degreeDataset.Tables[0].Rows[cnt]["Degree"]) + "$" + curDate];

                    if (dicAlternateScheduledData.ContainsKey(Convert.ToString(degreeDataset.Tables[0].Rows[cnt]["Degree"]) + "$" + curDate))
                        alterDic = dicAlternateScheduledData[Convert.ToString(degreeDataset.Tables[0].Rows[cnt]["Degree"]) + "$" + curDate];

                    drSch = dtTTDisp.NewRow();
                    drSch["DateDisp"] = dt.ToString("dd-MM-yyyy");
                    drSch["DateVal"] = curDate;
                    drSch["DayOrder"] = curDate;
                    //spreadDet.Sheets[0].Cells[cnt, 1].Text = Convert.ToString(degreeDataset.Tables[0].Rows[cnt]["Degree"]);
                    drSch["Degree"] = Convert.ToString(Convert.ToString(degreeDataset.Tables[0].Rows[cnt]["Degree"]));//
                    drSch["DegreeVal"] = Convert.ToString(Convert.ToString(degreeDataset.Tables[0].Rows[cnt]["Code"]));

                    foreach (KeyValuePair<int, string> dicKeyValue in dic)
                    {
                        int dicKey = dicKeyValue.Key;
                        string dicValue = dicKeyValue.Value;
                        string celltxt = string.Empty;
                        string val = string.Empty;
                        if (dicValue.Contains('@'))
                        {
                            string[] cellval = dicValue.Split('@');
                            celltxt = Convert.ToString(dicValue.Split('@')[0]);
                            val = Convert.ToString(dicValue.Split('@')[1]);
                        }
                        else
                        {
                            celltxt = Convert.ToString(dicValue);
                            val = Convert.ToString(dicValue);
                        }
                        if (Convert.ToString(dicValue) != "No Schedule" && Convert.ToString(dicValue) != "The selected date must be between Semester date" && Convert.ToString(dicValue) != "Update semester Information")
                        {
                            drSch["P" + (dicKey) + "ValDisp"] = Convert.ToString(celltxt);
                            drSch["P" + (dicKey) + "Val"] = val;
                            drSch["TT_" + (dicKey)] = Convert.ToString(degreeDataset.Tables[0].Rows[cnt]["Code"]);
                        }
                        else
                        {
                            drSch["P" + (dicKey) + "ValDisp"] = Convert.ToString(dic[1]);

                        }
                    }
                    dtTTDisp.Rows.Add(drSch);
                    drSch = dtTTDisp.NewRow();
                    drSch["DateDisp"] = dt.ToString("dd-MM-yyyy");
                    drSch["DateVal"] = curDate;
                    drSch["DayOrder"] = curDate;
                    //spreadDet.Sheets[0].Cells[cnt, 1].Text = Convert.ToString(degreeDataset.Tables[0].Rows[cnt]["Degree"]);
                    drSch["Degree"] = Convert.ToString(Convert.ToString(degreeDataset.Tables[0].Rows[cnt]["Degree"]));//
                    drSch["DegreeVal"] = Convert.ToString(Convert.ToString(degreeDataset.Tables[0].Rows[cnt]["Code"]));
                    if (alterDic.Count > 0)
                    {
                        foreach (KeyValuePair<int, string> dicKeyValuealter in alterDic)
                        {
                            int dicKey = dicKeyValuealter.Key;
                            string dicValue = dicKeyValuealter.Value;
                            string celltxt = string.Empty;
                            string val = string.Empty;
                            if (dicValue.Contains('@'))
                            {
                                string[] cellval = dicValue.Split('@');
                                celltxt = Convert.ToString(dicValue.Split('@')[0]);
                                val = Convert.ToString(dicValue.Split('@')[1]);
                            }
                            else
                            {
                                celltxt = Convert.ToString(dicValue);
                                val = Convert.ToString(dicValue);
                            }

                            if (Convert.ToString(dicValue) != "No Schedule" && Convert.ToString(dicValue) != "The selected date must be between Semester date" && Convert.ToString(dicValue) != "Update semester Information")
                            {
                                drSch["P" + (dicKey) + "ValDisp"] = Convert.ToString(celltxt);
                                drSch["P" + (dicKey) + "Val"] = val;
                                drSch["TT_" + (dicKey)] = Convert.ToString(degreeDataset.Tables[0].Rows[cnt]["Code"]);
                            }
                            else
                            {
                                //drSch["P" + (dicKey) + "ValDisp"] = Convert.ToString(alterDic[1]);

                            }
                        }
                    }
                    else
                    {

                    }
                    dtTTDisp.Rows.Add(drSch);

                }
            }

            if (dtTTDisp.Rows.Count > 0)
            {
                gridTimeTable.DataSource = dtTTDisp;
                gridTimeTable.DataBind();
                gridTimeTable.Visible = true;
                btnMasterSave.Visible = true;
            }
            int cell = gridTimeTable.Columns.Count;
            string sql = dacess.GetFunction("select max(No_of_hrs_per_day)HoursPerDay,MAX(nodays)NoOfDays from PeriodAttndSchedule");
            int NoHrs = Convert.ToInt32(sql);
            if (NoHrs != 0)
            {
                for (int i = 0; i < cell; i++)
                {
                    if (i < NoHrs + 2)
                        gridTimeTable.Columns[i].Visible = true;
                    else
                        gridTimeTable.Columns[i].Visible = false;
                }

            }
        }
        catch (Exception ex) { }

    }
    protected void OnDataBound(object sender, EventArgs e)
    {
        try
        {
            for (int rowIndex = gridTimeTable.Rows.Count - 2; rowIndex >= 0; rowIndex--)
            {
                GridViewRow row = gridTimeTable.Rows[rowIndex];
                GridViewRow previousRow = gridTimeTable.Rows[rowIndex + 1];

                string l1 = (row.FindControl("lblDegree") as Label).Text;
                string l2 = (previousRow.FindControl("lblDegree") as Label).Text;
                if (l1 == l2)
                {
                    row.Cells[1].RowSpan = previousRow.Cells[1].RowSpan < 2 ? 2 :
                                           previousRow.Cells[1].RowSpan + 1;
                    previousRow.Cells[1].Visible = false;
                }
                string l1D = (row.FindControl("lblDateDisp") as Label).Text;
                string l2D = (previousRow.FindControl("lblDateDisp") as Label).Text;
                if (l1D == l2D)
                {
                    row.Cells[0].RowSpan = previousRow.Cells[0].RowSpan < 2 ? 2 :
                                           previousRow.Cells[0].RowSpan + 1;
                    previousRow.Cells[0].Visible = false;
                }

            }
        }
        catch
        {
        }
    }
    protected void OnRowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string txt = (e.Row.FindControl("lnkPeriod_1") as LinkButton).Text;
                if (e.Row.RowIndex % 2 == 1)
                {
                    e.Row.BackColor = Color.LightBlue;
                    //e.Row.Enabled = false;
                    //e.Row.ForeColor = Color.LightPink;
                }
                if ((e.Row.FindControl("lnkPeriod_1") as LinkButton).Text.Trim().ToLower().Contains("holiday"))
                    (e.Row.FindControl("lnkPeriod_1") as LinkButton).ForeColor = Color.Red;
                if ((e.Row.FindControl("lnkPeriod_2") as LinkButton).Text.Trim().ToLower().Contains("holiday"))
                    (e.Row.FindControl("lnkPeriod_2") as LinkButton).ForeColor = Color.Red;
                if ((e.Row.FindControl("lnkPeriod_3") as LinkButton).Text.Trim().ToLower().Contains("holiday"))
                    (e.Row.FindControl("lnkPeriod_3") as LinkButton).ForeColor = Color.Red;
                if ((e.Row.FindControl("lnkPeriod_4") as LinkButton).Text.Trim().ToLower().Contains("holiday"))
                    (e.Row.FindControl("lnkPeriod_4") as LinkButton).ForeColor = Color.Red;
                if ((e.Row.FindControl("lnkPeriod_5") as LinkButton).Text.Trim().ToLower().Contains("holiday"))
                    (e.Row.FindControl("lnkPeriod_5") as LinkButton).ForeColor = Color.Red;
                if ((e.Row.FindControl("lnkPeriod_6") as LinkButton).Text.Trim().ToLower().Contains("holiday"))
                    (e.Row.FindControl("lnkPeriod_6") as LinkButton).ForeColor = Color.Red;
                if ((e.Row.FindControl("lnkPeriod_7") as LinkButton).Text.Trim().ToLower().Contains("holiday"))
                    (e.Row.FindControl("lnkPeriod_7") as LinkButton).ForeColor = Color.Red;
                if ((e.Row.FindControl("lnkPeriod_8") as LinkButton).Text.Trim().ToLower().Contains("holiday"))
                    (e.Row.FindControl("lnkPeriod_8") as LinkButton).ForeColor = Color.Red;
                if ((e.Row.FindControl("lnkPeriod_9") as LinkButton).Text.Trim().ToLower().Contains("holiday"))
                    (e.Row.FindControl("lnkPeriod_9") as LinkButton).ForeColor = Color.Red;
                if ((e.Row.FindControl("lnkPeriod_10") as LinkButton).Text.Trim().ToLower().Contains("holiday"))
                    (e.Row.FindControl("lnkPeriod_10") as LinkButton).ForeColor = Color.Red;


            }
        }
        catch
        {
        }
    }
    protected void btnBatchAllocation_Click(object sender, EventArgs e)
    {
        try
        {
            Session["Batch_ReDir"] = "FromNewAlternateSchedule";
            Response.Redirect("~/ScheduleMOD/Batchallocation.aspx");
        }
        catch { }
    }
    private void loadTree()
    {
        spcellClickPopup.Visible = true;
        subjtree.Visible = true;
        altersp_td.Visible = false;
        btnOk.Visible = false;
        chkForAlternateStaff.Checked = false;
        chkappend.Checked = false;

        string batch = Convert.ToString(Session["batch"]);
        string degcode = Convert.ToString(Session["degcode"]);
        string sem = Convert.ToString(Session["sem"]);
        string sec = "";
        if (Convert.ToString(Session["sec"]) != "")
            sec = Convert.ToString(Session["sec"]);

        string Syllabus_year = string.Empty;
        Syllabus_year = GetSyllabusYear(degcode, batch, sem);
        subjtree.Nodes.Clear();
        if (Syllabus_year != "-1")
        {
            //--------------get subject type and subjects
            cona.Close();
            cona.Open();
            SqlDataReader subTypeRs;
            cmda = new SqlCommand("select distinct subject.subtype_no,subject_type from subject,sub_sem where sub_sem.subtype_no=subject.subtype_no and subject.syll_code=(select syll_code from syllabus_master where degree_code=" + degcode + " and semester=" + sem + " and syllabus_year = " + Syllabus_year + " and batch_year = " + batch + ") order by subject.subtype_no", cona);
            subTypeRs = cmda.ExecuteReader();
            TreeNode node;
            int rec_count = 0;
            while (subTypeRs.Read())
            {
                if ((subTypeRs["subject_type"].ToString()) != "0")
                {
                    SqlDataReader subTypeRs1;
                    con1a.Close();
                    con1a.Open();
                    cmd1a = new SqlCommand("select subject.subtype_no,subject_type,subject_no,subject_name,subject_code from subject,sub_sem where sub_sem.subtype_no=subject.subtype_no and subject.syll_code=(select syll_code from syllabus_master where degree_code=" + degcode + " and semester=" + sem + " and syllabus_year = " + Syllabus_year + " and batch_year = " + batch + ") and subject.subtype_no=" + subTypeRs["subtype_no"] + " order by subject.subtype_no,subject.subject_no", con1a);
                    subTypeRs1 = cmd1a.ExecuteReader();
                    node = new TreeNode(subTypeRs["subject_type"].ToString(), rec_count.ToString());
                    while (subTypeRs1.Read())//-------------set to tree
                    {
                        node.ChildNodes.Add(new TreeNode(subTypeRs1["subject_name"].ToString(), subTypeRs1["subject_no"].ToString()));
                        rec_count = rec_count + 1;

                    }
                    subjtree.Nodes.Add(node);
                }
            }
            cona.Close();
            con1a.Close();
        }

    }
    protected void subjtree_SelectedNodeChanged(object sender, EventArgs e)
    {
        string strsec;
        string batch = Convert.ToString(Session["batch"]);
        string degcode = Convert.ToString(Session["degcode"]);
        string sem = Convert.ToString(Session["sem"]);
        string sec = "";
        errmsg.Visible = false;
        if (Convert.ToString(Session["sec"]) != "")
            sec = Convert.ToString(Session["sec"]);

        if (sec != "0" && sec != "\0")
        {
            strsec = string.Empty;
        }
        else
        {
            strsec = " and sections='" + sec + "'";
        }
        chkmulstaff_ChekedChange(sender, e);
        chkmullsstaff_SelectedIndexChanged(sender, e);
        int parent_count = subjtree.Nodes.Count;//----------TreeView Nodes count
        if (!chkappend.Checked)
        {
            DataTable dtAlter = new DataTable();
            dtAlter.Columns.Add("SubName");
            dtAlter.Columns.Add("SubNo");
            DataRow dr = null;
            if (chkForAlternateStaff.Checked)
            {
                for (int parentNodeCnt = 0; parentNodeCnt < parent_count; parentNodeCnt++)
                {
                    for (int childNodeCnt = 0; childNodeCnt < subjtree.Nodes[parentNodeCnt].ChildNodes.Count; childNodeCnt++)//-------count child node
                    {
                        if (subjtree.Nodes[parentNodeCnt].ChildNodes[childNodeCnt].Selected == true)
                        {

                            string temp_sec = string.Empty;
                            if (sec == "")
                            {
                                temp_sec = string.Empty;
                            }
                            else
                            {
                                temp_sec = " and Sections='" + sec + "'";
                            }

                            string staffNamesQry = "select staff_code,staff_name from staffmaster where staff_code in (select staff_code from staff_selector where subject_no = '" + Convert.ToString(subjtree.Nodes[parentNodeCnt].ChildNodes[childNodeCnt].Value) + "' and batch_year=" + batch + " " + temp_sec + ")";
                            DataTable staf_set = dirAcc.selectDataTable(staffNamesQry);
                            if (staf_set.Rows.Count > 0)
                            {
                                foreach (DataRow drss in staf_set.Rows)
                                {
                                    dr = dtAlter.NewRow();
                                    dr["SubName"] = subjtree.Nodes[parentNodeCnt].ChildNodes[childNodeCnt].Text;
                                    dr["SubNo"] = subjtree.Nodes[parentNodeCnt].ChildNodes[childNodeCnt].Value;
                                    dtAlter.Rows.Add(dr);
                                }
                            }
                            btnOk.Visible = true;
                            chkappend.Visible = true;
                            altersp_td.Visible = true;

                        }
                    }
                }
                if (dtAlter.Rows.Count > 0)
                {
                    ViewState["CurrentTable"] = dtAlter;
                    GridView5.DataSource = dtAlter;
                    GridView5.DataBind();
                    GridView5.Visible = true;
                }
            }
            else
            {
                for (int parentNodeCnt = 0; parentNodeCnt < parent_count; parentNodeCnt++)
                {
                    for (int childNodeCnt = 0; childNodeCnt < subjtree.Nodes[parentNodeCnt].ChildNodes.Count; childNodeCnt++)//-------count child node
                    {
                        if (subjtree.Nodes[parentNodeCnt].ChildNodes[childNodeCnt].Selected == true)
                        {
                            dr = dtAlter.NewRow();
                            string temp_sec = string.Empty;
                            if (sec == "")
                            {
                                temp_sec = string.Empty;
                            }
                            else
                            {
                                temp_sec = " and Sections='" + sec + "'";
                            }
                            if (chkappend.Checked == true)
                            {
                            }
                            else
                            {
                                dr["SubName"] = subjtree.Nodes[parentNodeCnt].ChildNodes[childNodeCnt].Text;
                                dr["SubNo"] = subjtree.Nodes[parentNodeCnt].ChildNodes[childNodeCnt].Value;
                            }
                            btnOk.Visible = true;
                            chkappend.Visible = true;
                            altersp_td.Visible = true;
                            dtAlter.Rows.Add(dr);
                        }
                    }
                }
                if (dtAlter.Rows.Count > 0)
                {
                    ViewState["CurrentTable"] = dtAlter;
                    GridView5.DataSource = dtAlter;
                    GridView5.DataBind();
                    GridView5.Visible = true;
                }
            }
        }
        else if (ViewState["CurrentTable"] != null)
        {
            DataTable dtCurrentTable = (DataTable)ViewState["CurrentTable"];
            DataRow dr = null;
            for (int parentNodeCnt = 0; parentNodeCnt < parent_count; parentNodeCnt++)
            {
                for (int childNodeCnt = 0; childNodeCnt < subjtree.Nodes[parentNodeCnt].ChildNodes.Count; childNodeCnt++)//-------count child node
                {
                    if (subjtree.Nodes[parentNodeCnt].ChildNodes[childNodeCnt].Selected == true)
                    {
                        dr = dtCurrentTable.NewRow();
                        string temp_sec = string.Empty;
                        if (sec == "")
                        {
                            temp_sec = string.Empty;
                        }
                        else
                        {
                            temp_sec = " and Sections='" + sec + "'";
                        }

                        dr["SubName"] = subjtree.Nodes[parentNodeCnt].ChildNodes[childNodeCnt].Text;
                        dr["SubNo"] = subjtree.Nodes[parentNodeCnt].ChildNodes[childNodeCnt].Value;

                        btnOk.Visible = true;
                        chkappend.Visible = true;
                        altersp_td.Visible = true;
                        dtCurrentTable.Rows.Add(dr);
                    }
                }
            }
            if (dtCurrentTable.Rows.Count > 0)
            {
                ViewState["CurrentTable"] = null;
                ViewState["CurrentTable"] = dtCurrentTable;
                GridView5.DataSource = dtCurrentTable;
                GridView5.DataBind();
                GridView5.Visible = true;
            }
        }

    }
    protected void GridView5_OnDataBound(object sender, EventArgs e)
    {
        try
        {
            string strsec;
            string batch = Convert.ToString(Session["batch"]);
            string degcode = Convert.ToString(Session["degcode"]);
            string sem = Convert.ToString(Session["sem"]);
            string sec = "";
            if (Convert.ToString(Session["sec"]) != "")
                sec = Convert.ToString(Session["sec"]);

            if (sec != "0" && sec != "\0")
            {
                strsec = string.Empty;
            }
            else
            {
                strsec = "  and ss.sections='" + sec + "'";
            }
            string staffNamesQry = "select ss.staff_code,sm.staff_name,ss.subject_no from staffmaster sm,staff_selector  ss where ss.staff_code=sm.staff_code and ss.batch_year='" + batch + "'" + strsec;
            DataTable dtStaff = dirAcc.selectDataTable(staffNamesQry);
            if (!chkForAlternateStaff.Checked)
            {
                foreach (GridViewRow gr in GridView5.Rows)
                {
                    DropDownList ddlStaff = (gr.FindControl("ddlStaff") as DropDownList);
                    DropDownList ddlAltStaff = (gr.FindControl("ddlAlterStaff") as DropDownList);
                    string subNo = (gr.FindControl("lblSubjectNo") as Label).Text;
                    dtStaff.DefaultView.RowFilter = "subject_no = '" + subNo + "'";
                    DataTable dtTemp = dtStaff.DefaultView.ToTable();
                    dtTemp = dtTemp.DefaultView.ToTable(true, "staff_code", "staff_name");

                    chkmullsstaff.Items.Clear();

                    if (dtTemp.Rows.Count > 0)
                    {

                        chkmullsstaff.Items.Clear();
                        chkmullsstaff.DataSource = dtTemp;
                        chkmullsstaff.DataTextField = "staff_name";
                        chkmullsstaff.DataValueField = "staff_code";
                        chkmullsstaff.DataBind();
                        ddlStaff.DataSource = dtTemp;
                        ddlStaff.DataTextField = "staff_name";
                        ddlStaff.DataValueField = "staff_code";
                        ddlStaff.DataBind();

                        ddlAltStaff.DataSource = dtTemp;
                        ddlAltStaff.DataTextField = "staff_name";
                        ddlAltStaff.DataValueField = "staff_code";
                        ddlAltStaff.DataBind();
                    }
                    ddlStaff.Items.Insert(0, " ");
                    ddlStaff.Items.Insert(1, "All");
                    ddlAltStaff.Items.Insert(0, " ");
                    //ddlAltStaff.Items.Insert(1, "All");
                }
            }
            else
            {
                int j = 0;
                foreach (GridViewRow gr in GridView5.Rows)
                {
                    DropDownList ddlStaff = (gr.FindControl("ddlStaff") as DropDownList);
                    DropDownList ddlAltStaff = (gr.FindControl("ddlAlterStaff") as DropDownList);
                    string subNo = (gr.FindControl("lblSubjectNo") as Label).Text;
                    dtStaff.DefaultView.RowFilter = "subject_no = '" + subNo + "'";
                    DataTable dtTemp = dtStaff.DefaultView.ToTable();
                    dtTemp = dtTemp.DefaultView.ToTable(true, "staff_code", "staff_name");

                    chkmullsstaff.Items.Clear();

                    if (dtTemp.Rows.Count > 0)
                    {
                        chkmullsstaff.Items.Clear();
                        chkmullsstaff.DataSource = dtTemp;
                        chkmullsstaff.DataTextField = "staff_name";
                        chkmullsstaff.DataValueField = "staff_code";
                        chkmullsstaff.DataBind();

                        string StaffCode = Convert.ToString(dtTemp.Rows[j]["staff_name"]) + "-" + Convert.ToString(dtTemp.Rows[j]["staff_code"]);
                        ddlStaff.Items.Insert(0, StaffCode);

                        ddlAltStaff.DataSource = dtTemp;
                        ddlAltStaff.DataTextField = "staff_name";
                        ddlAltStaff.DataValueField = "staff_code";
                        ddlAltStaff.DataBind();
                        ddlAltStaff.Items.Insert(0, " ");
                        //ddlAltStaff.Items.Insert(1, "All");
                    }
                    j++;
                }
            }
        }
        catch
        {
        }
    }
    private string GetSyllabusYear(string degree_code, string batch_year, string sem)
    {
        try
        {
            string syl_year = string.Empty;
            con2a.Close();
            con2a.Open();
            SqlCommand cmd2a;
            SqlDataReader get_syl_year;
            cmd2a = new SqlCommand("select syllabus_year from syllabus_master where degree_code=" + degree_code + " and semester =" + sem + " and batch_year=" + batch_year + " ", con2a);
            get_syl_year = cmd2a.ExecuteReader();
            get_syl_year.Read();
            if (get_syl_year.HasRows == true)
            {
                if (get_syl_year[0].ToString() == "\0")
                {
                    syl_year = "-1";
                }
                else
                {
                    syl_year = get_syl_year[0].ToString();
                }
            }
            else
            {
                syl_year = "-1";
            }
            return syl_year;
            con2a.Close();
        }
        catch
        {
            return string.Empty;
        }
    }
    protected void btnOKsave_Clik(object sender, EventArgs e)
    {
        Div3.Visible = false;
    }
    protected void bt_closedalter_Clik(object sender, EventArgs e)
    {
        btnGo_Click(sender, e);
        Div3.Visible = false;
    }
    protected void chkPerDAySched_OnCheckedChanged(object sender, EventArgs e)
    {
    }
    protected void chkmullsstaff_SelectedIndexChanged(object sender, EventArgs e)
    {
        txtmulstaff.Text = "---Select---";
        chkmulstaff.Checked = false;
        int cou = 0;
        for (int i = 0; i < chkmullsstaff.Items.Count; i++)
        {
            if (chkmullsstaff.Items[i].Selected == true)
            {
                cou++;
            }
        }
        if (cou > 0)
        {
            txtmulstaff.Text = "Staff (" + cou + ")";
            if (chkmullsstaff.Items.Count == cou)
            {
                chkmulstaff.Checked = true;
            }
        }
    }
    protected void spcellClickPopupclose_Click(object sender, EventArgs e)
    {
        spcellClickPopup.Visible = false;
    }
    protected void chkmulstaff_ChekedChange(object sender, EventArgs e)
    {
        txtmulstaff.Text = "---Select---";
        if (chkmulstaff.Checked == true)
        {
            if (chkmullsstaff.Items.Count > 0)
            {
                for (int i = 0; i < chkmullsstaff.Items.Count; i++)
                {
                    chkmullsstaff.Items[i].Selected = true;
                }
                txtmulstaff.Text = "Staff (" + chkmullsstaff.Items.Count + ")";
            }
        }
        else
        {
            for (int i = 0; i < chkmullsstaff.Items.Count; i++)
            {
                chkmullsstaff.Items[i].Selected = false;
            }
        }
    }
    protected void btnmulstaff_Click(object sender, EventArgs e)
    {
        try
        {
            bool ischeck = false;
            lblAlterFreeStaffError.Visible = false;
            string Row = Convert.ToString(Session["rowVal"]);
            int ActiveRow = 0;
            int.TryParse(Row, out ActiveRow);

            foreach (GridViewRow grid in GridView5.Rows)
            {
                DropDownList ddl = (grid.FindControl("ddlAlterStaff") as DropDownList);
                DropDownList ddl1 = (grid.FindControl("ddlStaff") as DropDownList);
                if (!chkForAlternateStaff.Checked)
                    ddl1.Items.Clear();

                ddl.Items.Clear();
                ischeck = false;
                for (int chki = 0; chki < chkmullsstaff.Items.Count; chki++)
                {
                    if (chkmullsstaff.Items[chki].Selected)
                    {
                        ischeck = true;
                        string selStaff = (chkmullsstaff.Items[chki].Text) + "-" + (chkmullsstaff.Items[chki].Value);
                        if (!chkForAlternateStaff.Checked)
                            ddl1.Items.Add(selStaff);

                        ddl.Items.Add(selStaff);
                    }
                }
                if (!chkForAlternateStaff.Checked)
                {
                    ddl1.Items.Insert(0, " ");
                    ddl1.Items.Insert(1, "All");
                }
                ddl.Items.Insert(0, " ");
                ddl.Items.Insert(1, "All");
            }
            if (ischeck)
            {
                if (!chkForAlternateStaff.Checked)
                {
                    GridView5.Columns[4].Visible = false;
                    GridView5.Columns[3].Visible = false;
                    GridView5.Columns[5].Visible = false;
                }
                else
                {
                    GridView5.Columns[4].Visible = true;
                    GridView5.Columns[3].Visible = true;
                    GridView5.Columns[5].Visible = true;
                }

                divAlterFreeStaffDetails.Visible = false;
                lblAlterFreeStaffError.Text = string.Empty;
                lblAlterFreeStaffError.Visible = false;
                txtAlterFreeStaffSearch.Text = string.Empty;
            }
            else
            {
                lblAlterFreeStaffError.Visible = true;
                lblAlterFreeStaffError.Text = "Choose Atleast One Staff!";
            }
        }
        catch
        {
        }
    }
    protected void chkSelectAlterStaff_CheckedChanged(object sender, EventArgs e)
    {
        try
        {
            chkForAlternateStaff.Checked = false;
            //chkForAlternateStaff_CheckedChanged(sender, e);
        }
        catch
        {
        }
    }
    protected void chkForAlternateStaff_CheckedChanged(object sender, EventArgs e)
    {
        if (chkForAlternateStaff.Checked)
        {
            chkappend.Checked = false;
            GridView5.Columns[3].Visible = true;
            GridView5.Columns[5].Visible = true;
            GridView5.Columns[4].Visible = true;
        }
        else
        {
            GridView5.Columns[3].Visible = false;
            GridView5.Columns[5].Visible = false;
            GridView5.Columns[4].Visible = false;
        }

        subjtree_SelectedNodeChanged(sender, e);
    }
    protected void lnkAttMark(object sender, EventArgs e)
    {
        LinkButton lnkSelected = (LinkButton)sender;
        string rowIndxS = lnkSelected.UniqueID.ToString().Split('$')[3].Replace("ctl", string.Empty);
        int rowIndx = Convert.ToInt32(rowIndxS) - 2;
        string colIndxS = lnkSelected.UniqueID.ToString().Split('$')[4].Replace("lnkPeriod_", string.Empty);
        int colIndx = Convert.ToInt32(colIndxS);
        Session["Row"] = rowIndx;
        Session["Col"] = colIndx;
        ViewState["CurrentTable"] = null;
        // btnFreeStaffList.Visible = true;
        spcellClickPopup.Visible = false;
        subjtree.Visible = false;
        altersp_td.Visible = false;
        btnOk.Visible = false;
        chkForAlternateStaff.Checked = false;
        chkappend.Checked = false;
        GridView5.Columns[3].Visible = false;
        GridView5.Columns[5].Visible = false;
        GridView5.Columns[4].Visible = false;
        chkappend.Checked = false;
        chkForAlternateStaff.Checked = false;

        cellTagValue = Convert.ToString((gridTimeTable.Rows[rowIndx].FindControl("lblTT_" + colIndx) as Label).Text);

        if (cellTagValue != "")
        {
            string[] splitTagValue = cellTagValue.Split(new Char[] { '-' });

            Session["batch"] = Convert.ToString(splitTagValue[0]);
            lbtxtbatch.Text = ": " + Convert.ToString(splitTagValue[0]);
            Session["degcode"] = Convert.ToString(splitTagValue[1]);
            lbltxtDeg.Text = ": " + Convert.ToString(dacess.GetFunction(" select de.Dept_Name from Degree d,Department de where d.Dept_Code=de.Dept_Code and d.Degree_Code='" + Convert.ToString(splitTagValue[1]) + "'"));
            Session["sem"] = Convert.ToString(splitTagValue[2]);
            lbltxtSem.Text = ": " + Convert.ToString(splitTagValue[2]);
            Session["sec"] = "";
            if (Convert.ToString(splitTagValue[3]) != "")
            {
                Session["sec"] = Convert.ToString(splitTagValue[3]).Trim();
                lbltxtSec.Text = ": " + Convert.ToString(splitTagValue[3]).Trim();
            }
            else
            {
                lbltxtSec.Text = "-";
            }
            Session["period"] = Convert.ToString(colIndx);
            Session["spreadDetcellTagValue"] = cellTagValue;
            //free_staff();

            if (rowIndx % 2 == 1)
            {
                Div5.Visible = true;
                lblAlrt.Visible = true;
                lblAlrt.Text = "Do you want Remove?";
            }
            else
            {
                if (!chkPerDAySched.Checked)
                {
                    loadTree();
                    //btnAsPerDaySchedule_Click();
                }
                else
                {
                    loadschedule();
                }
            }


        }


    }
    protected void btnFreeStaff_Click(object sender, EventArgs e)
    {
        try
        {
            isBussyStaff = false;
            lblAlterFreeStaffError.Visible = false;
            Button lnkSelected = (Button)sender;
            string rowIndxS = lnkSelected.UniqueID.ToString().Split('$')[3].Replace("ctl", string.Empty);
            int rowIndx = Convert.ToInt32(rowIndxS) - 2;
            Session["rowVal"] = rowIndx.ToString();
            divAlterFreeStaffDetails.Visible = true;
            lblAlterFreeStaffError.Text = string.Empty;
            lblAlterFreeStaffError.Visible = false;
            txtAlterFreeStaffSearch.Text = string.Empty;
            GetStaffDetailsNEW(rowIndx);
            //  fpSpreadTreeNode.Sheets[0].Columns[3].Visible = true;


        }
        catch
        {
        }
    }
    protected void btnBussyStaff_Click(object sender, EventArgs e)
    {
        try
        {
            isBussyStaff = true;
            lblAlterFreeStaffError.Visible = false;
            Button lnkSelected = (Button)sender;
            string rowIndxS = lnkSelected.UniqueID.ToString().Split('$')[3].Replace("ctl", string.Empty);
            int rowIndx = Convert.ToInt32(rowIndxS) - 2;
            Session["rowVal"] = rowIndx.ToString();
            divAlterFreeStaffDetails.Visible = true;
            lblAlterFreeStaffError.Text = string.Empty;
            lblAlterFreeStaffError.Visible = false;
            txtAlterFreeStaffSearch.Text = string.Empty;
            GetStaffDetailsNEW(rowIndx);
        }
        catch
        {
        }
    }
    protected void btnRemove_Click(object sender, EventArgs e)
    {
        Button lnkSelected = (Button)sender;
        string rowIndxS = lnkSelected.UniqueID.ToString().Split('$')[3].Replace("ctl", string.Empty);
        int rowIndx = Convert.ToInt32(rowIndxS) - 2;

        //GridView5.DeleteRow(rowIndx);
        GridView5.Rows[rowIndx].Visible = false;
    }
    public DataTable getFreeStaffListNew(DateTime dtAlterDate, int period, byte type = 0, string searchValue = null)
    {
        DataTable dtFreeStaffList = new DataTable();
        string qry = string.Empty;
        try
        {
            string qryStaffFilter = string.Empty;
            string qryDeptFilter = string.Empty;
            string qryCollegeFilter = string.Empty;

            string qryStaffFilter1 = string.Empty;
            string qryDeptFilter1 = string.Empty;
            string qryCollegeFilter1 = string.Empty;
            if (type == 0)
            {

            }
            else
            {
                if (ddlAlterFreeCollege.Items.Count > 0)
                {
                    qryCollegeFilter = " and sfm.college_code ='" + Convert.ToString(ddlAlterFreeCollege.SelectedValue).Trim() + "'";
                    qryCollegeFilter1 = " and sfm1.college_code='" + Convert.ToString(ddlAlterFreeCollege.SelectedValue).Trim() + "'";
                }
                if (ddlAlterFreeDepartment.Items.Count > 0)
                {
                    if (!string.IsNullOrEmpty(Convert.ToString(ddlAlterFreeDepartment.SelectedValue).Trim()) && Convert.ToString(ddlAlterFreeDepartment.SelectedValue).Trim().ToLower() != "all")
                    {
                        qryDeptFilter = " and hr.dept_code='" + Convert.ToString(ddlAlterFreeDepartment.SelectedValue).Trim() + "'";
                        qryDeptFilter1 = "  and hr1.dept_code='" + Convert.ToString(ddlAlterFreeDepartment.SelectedValue).Trim() + "'";
                    }
                }
                if (!string.IsNullOrEmpty(searchValue))
                {
                    if (ddlAlterFreeStaff.Items.Count > 0)
                    {
                        if (ddlAlterFreeStaff.SelectedIndex == 0)
                        {
                            qryStaffFilter = " and sfm.staff_name like '%" + searchValue + "%'";
                            qryStaffFilter1 = " and sfm.staff_name like '%" + searchValue + "%'";
                        }
                        else
                        {
                            qryStaffFilter = " and sfm.staff_code like '%" + searchValue + "%'";
                            qryStaffFilter1 = " and sfm.staff_code like '%" + searchValue + "%'";
                        }
                    }
                }
            }

            if (period != 0)
            {
                if (!isBussyStaff)
                {
                    qry = " select distinct sfm.staff_code,sfm.staff_name from staffmaster sfm inner join stafftrans sts on sts.staff_code=sfm.staff_code inner join hrdept_master hr on hr.dept_code=sts.dept_code where sts.latestrec='1' and sfm.resign=0 and sfm.settled=0 and sfm.college_code=hr.college_code " + qryCollegeFilter + qryDeptFilter + " and sfm.staff_code not in( select distinct sfm1.staff_code from Semester_Schedule sch,Registration r,seminfo si,staffmaster sfm1 inner join stafftrans sts1 on sts1.staff_code=sfm1.staff_code inner join hrdept_master hr1 on hr1.dept_code=sts1.dept_code where sts1.latestrec='1' and sfm1.resign=0 and sfm1.settled=0 and sfm1.college_code=hr1.college_code " + qryCollegeFilter1 + qryDeptFilter1 + " and r.Batch_Year=sch.batch_year and r.degree_code=sch.degree_code and r.Current_Semester=sch.semester and r.Batch_Year=si.batch_year and r.degree_code=si.degree_code and r.Current_Semester=si.semester and si.batch_year=sch.batch_year and sch.degree_code=si.degree_code and si.semester=sch.semester and LTRIM(RTRIM(ISNULL(sch.Sections,'')))=LTRIM(RTRIM(ISNULL(r.Sections,'')))  and r.CC='0' and r.DelFlag='0' and r.Exam_Flag<>'debar' and sch.FromDate between si.start_date and si.end_date and '" + dtAlterDate.ToString("MM/dd/yyyy") + "' between si.start_date and si.end_date and sch.FromDate<='" + dtAlterDate.ToString("MM/dd/yyyy") + "' and sch." + Convert.ToString((DayOfWeek)dtAlterDate.DayOfWeek).Substring(0, 3) + period.ToString() + " like '%'+sfm1.staff_code+'%') and sfm.staff_code not in( select distinct sfm1.staff_code from Alternate_Schedule sch,Registration r,seminfo si,staffmaster sfm1 inner join stafftrans sts1 on sts1.staff_code=sfm1.staff_code inner join hrdept_master hr1 on hr1.dept_code=sts1.dept_code where sts1.latestrec='1' and sfm1.resign=0 and sfm1.settled=0 and sfm1.college_code=hr1.college_code " + qryCollegeFilter1 + qryDeptFilter1 + " and r.Batch_Year=sch.batch_year and r.degree_code=sch.degree_code and r.Current_Semester=sch.semester and r.Batch_Year=si.batch_year and r.degree_code=si.degree_code and r.Current_Semester=si.semester and si.batch_year=sch.batch_year and sch.degree_code=si.degree_code and si.semester=sch.semester and LTRIM(RTRIM(ISNULL(sch.Sections,'')))=LTRIM(RTRIM(ISNULL(r.Sections,'')))  and r.CC='0' and r.DelFlag='0' and r.Exam_Flag<>'debar' and sch.FromDate between si.start_date and si.end_date and '" + dtAlterDate.ToString("MM/dd/yyyy") + "' between si.start_date and si.end_date and sch.FromDate='" + dtAlterDate.ToString("MM/dd/yyyy") + "' and sch." + Convert.ToString((DayOfWeek)dtAlterDate.DayOfWeek).Substring(0, 3) + period.ToString() + " like '%'+sfm1.staff_code+'%') " + qryStaffFilter + " order by sfm.staff_name,sfm.staff_code";
                    dtFreeStaffList = dirAcc.selectDataTable(qry);

                    DataTable dtAlterScheduleFreeStaff = new DataTable();
                    qry = "select distinct sfm.staff_code,sfm.staff_name from staffmaster sfm inner join stafftrans sts on sts.staff_code=sfm.staff_code inner join hrdept_master hr on hr.dept_code=sts.dept_code where sts.latestrec='1' and sfm.resign=0 and sfm.settled=0 and sfm.college_code=hr.college_code " + qryCollegeFilter + qryDeptFilter + " and sfm.staff_code not in( select distinct sfm1.staff_code from Alternate_Schedule sch,Registration r,seminfo si,staffmaster sfm1 inner join stafftrans sts1 on sts1.staff_code=sfm1.staff_code inner join hrdept_master hr1 on hr1.dept_code=sts1.dept_code where sts1.latestrec='1' and sfm1.resign=0 and sfm1.settled=0 and sfm1.college_code=hr1.college_code " + qryCollegeFilter1 + qryDeptFilter1 + " and r.Batch_Year=sch.batch_year and r.degree_code=sch.degree_code and r.Current_Semester=sch.semester and r.Batch_Year=si.batch_year and r.degree_code=si.degree_code and r.Current_Semester=si.semester and si.batch_year=sch.batch_year and sch.degree_code=si.degree_code and si.semester=sch.semester and LTRIM(RTRIM(ISNULL(sch.Sections,'')))=LTRIM(RTRIM(ISNULL(r.Sections,'')))  and r.CC='0' and r.DelFlag='0' and r.Exam_Flag<>'debar' and sch.FromDate between si.start_date and si.end_date and '" + dtAlterDate.ToString("MM/dd/yyyy") + "' between si.start_date and si.end_date and sch.FromDate='" + dtAlterDate.ToString("MM/dd/yyyy") + "' and sch." + Convert.ToString((DayOfWeek)dtAlterDate.DayOfWeek).Substring(0, 3) + period.ToString() + " like '%'+sfm1.staff_code+'%') " + qryStaffFilter + " order by sfm.staff_name,sfm.staff_code";
                }
                else
                {
                    qry = string.Empty;
                    qry = "select distinct sfm1.staff_code,sfm1.staff_name from Semester_Schedule sch,Registration r,seminfo si,staffmaster sfm1 inner join stafftrans sts1 on sts1.staff_code=sfm1.staff_code inner join hrdept_master hr1 on hr1.dept_code=sts1.dept_code where sts1.latestrec='1' and sfm1.resign=0 and sfm1.settled=0 and sfm1.college_code=hr1.college_code " + qryCollegeFilter1 + qryDeptFilter1 + " and r.Batch_Year=sch.batch_year and r.degree_code=sch.degree_code and r.Current_Semester=sch.semester and r.Batch_Year=si.batch_year and r.degree_code=si.degree_code and r.Current_Semester=si.semester and si.batch_year=sch.batch_year and sch.degree_code=si.degree_code and si.semester=sch.semester and LTRIM(RTRIM(ISNULL(sch.Sections,'')))=LTRIM(RTRIM(ISNULL(r.Sections,'')))  and r.CC='0' and r.DelFlag='0' and r.Exam_Flag<>'debar' and sch.FromDate between si.start_date and si.end_date and '" + dtAlterDate.ToString("MM/dd/yyyy") + "' between si.start_date and si.end_date and sch.FromDate<='" + dtAlterDate.ToString("MM/dd/yyyy") + "' and sch." + Convert.ToString((DayOfWeek)dtAlterDate.DayOfWeek).Substring(0, 3) + period.ToString() + " like '%'+sfm1.staff_code+'%'";
                    qry = qry + "  union";
                    qry = qry + "  select distinct sfm1.staff_code,sfm1.staff_name from Alternate_Schedule sch,Registration r,seminfo si,staffmaster sfm1 inner join stafftrans sts1 on sts1.staff_code=sfm1.staff_code inner join hrdept_master hr1 on hr1.dept_code=sts1.dept_code where sts1.latestrec='1' and sfm1.resign=0 and sfm1.settled=0 and sfm1.college_code=hr1.college_code " + qryCollegeFilter1 + qryDeptFilter1 + " and r.Batch_Year=sch.batch_year and r.degree_code=sch.degree_code and r.Current_Semester=sch.semester and r.Batch_Year=si.batch_year and r.degree_code=si.degree_code and r.Current_Semester=si.semester and si.batch_year=sch.batch_year and sch.degree_code=si.degree_code and si.semester=sch.semester and LTRIM(RTRIM(ISNULL(sch.Sections,'')))=LTRIM(RTRIM(ISNULL(r.Sections,'')))  and r.CC='0' and r.DelFlag='0' and r.Exam_Flag<>'debar' and sch.FromDate between si.start_date and si.end_date and '" + dtAlterDate.ToString("MM/dd/yyyy") + "' between si.start_date and si.end_date and sch.FromDate='" + dtAlterDate.ToString("MM/dd/yyyy") + "' and sch." + Convert.ToString((DayOfWeek)dtAlterDate.DayOfWeek).Substring(0, 3) + period.ToString() + " like '%'+sfm1.staff_code+'%'";
                    dtFreeStaffList = dirAcc.selectDataTable(qry);
                }

            }

        }
        catch
        {
        }
        return dtFreeStaffList;
    }
    protected void ddlAlterFreeCollege_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            BindAlterStaffDepartment((ddlAlterFreeCollege.Items.Count > 0) ? Convert.ToString(ddlAlterFreeCollege.SelectedValue).Trim() : "");
            GetStaffDetails();
        }
        catch
        {

        }
    }
    private void GetStaffDetailsNEW(int ActRow)
    {
        try
        {
            DataTable dtStaffDetails = new DataTable();
            DateTime dtAlterDate = new DateTime();
            DateTime.TryParseExact(txtDate.Text, "dd/MM/yyyy", null, DateTimeStyles.None, out dtAlterDate);
            int period = 0;
            int.TryParse(Convert.ToString(Session["period"]), out period);
            dtStaffDetails = getFreeStaffListNew(dtAlterDate: dtAlterDate, period: period, type: 1, searchValue: txtAlterFreeStaffSearch.Text.Trim());

            int sno = 0;
            if (dtStaffDetails.Rows.Count > 0)
            {
                GridFreeStaff.DataSource = dtStaffDetails;
                GridFreeStaff.DataBind();
                GridFreeStaff.Visible = true;
            }
            else
            {
            }
        }
        catch
        {
        }
    }
    private void GetStaffDetails()
    {
        try
        {
            DataTable dtStaffDetails = new DataTable();
            DateTime dtAlterDate = new DateTime();
            DateTime.TryParseExact(txtDate.Text, "dd/MM/yyyy", null, DateTimeStyles.None, out dtAlterDate);
            int period = 0;
            int.TryParse(Convert.ToString(Session["period"]), out period);
            dtStaffDetails = getFreeStaffListNew(dtAlterDate: dtAlterDate, period: period, type: 1, searchValue: txtAlterFreeStaffSearch.Text.Trim());
            //dtStaffDetails = getFreeStaffListNew(dtAlterDate, period, type, searchValue);


            int sno = 0;
            if (dtStaffDetails.Rows.Count > 0)
            {

                GridFreeStaff.DataSource = dtStaffDetails;
                GridFreeStaff.DataBind();
                GridFreeStaff.Visible = true;
            }
            else
            {
            }
        }
        catch
        {
        }
    }
    protected void ddlAlterFreeDepartment_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            GetStaffDetails();
        }
        catch
        {

        }
    }
    protected void ddlAlterFreeStaff_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            GetStaffDetails();
        }
        catch
        {

        }
    }
    protected void txtAlterFreeStaffSearch_TextChanged(object sender, EventArgs e)
    {
        try
        {
            GetStaffDetails();
        }
        catch
        {

        }
    }
    protected void btnSelectStaff_Click(object sender, EventArgs e)
    {
        try
        {
            lblAlterFreeStaffError.Visible = false;
            string Row = Convert.ToString(Session["rowVal"]);
            int ActiveRow = 0;
            int.TryParse(Row, out ActiveRow);
            DropDownList ddl = (GridView5.Rows[ActiveRow].FindControl("ddlAlterStaff") as DropDownList);
            ddl.Items.Clear();
            bool ischeck = false;
            foreach (GridViewRow grv in GridFreeStaff.Rows)
            {
                if ((grv.FindControl("Chk") as CheckBox).Checked)
                {
                    ischeck = true;
                    string selStaff = (grv.FindControl("lblStaffName") as Label).Text + "-" + (grv.FindControl("lblstaffCode") as Label).Text;
                    ddl.Items.Add(selStaff);
                }
            }
            if (ischeck)
            {
                GridView5.Columns[4].Visible = true;
                ddl.Items.Insert(0, " ");
                divAlterFreeStaffDetails.Visible = false;
                lblAlterFreeStaffError.Text = string.Empty;
                lblAlterFreeStaffError.Visible = false;
                txtAlterFreeStaffSearch.Text = string.Empty;
            }
            else
            {
                lblAlterFreeStaffError.Visible = true;
                lblAlterFreeStaffError.Text = "Choose Atleast One Staff!";
            }
        }
        catch
        {

        }
    }
    protected void btnFreeStaffExit_Click(object sender, EventArgs e)
    {
        try
        {
            divAlterFreeStaffDetails.Visible = false;
            lblAlterFreeStaffError.Text = string.Empty;
            lblAlterFreeStaffError.Visible = false;
            txtAlterFreeStaffSearch.Text = string.Empty;
        }
        catch
        {

        }
    }
    public void btnOk_Click(object sender, EventArgs e)
    {
        try
        {
            errmsg.Visible = false;
            string colIndex = Convert.ToString(Session["Col"]);
            string Rowindex = Convert.ToString(Session["Row"]);
            int r = 0;
            int.TryParse(Rowindex, out r);
            bool ischk = false;
            string columnVal = string.Empty;
            string columntxt = string.Empty;
            string alterStaffDet = string.Empty;
            if (chkForAlternateStaff.Checked)//Alternate Staff
            {
                string sCode = string.Empty;
                string islab = string.Empty;
                string alterStaff = string.Empty;
                string SubName = string.Empty;
                string selectedSubject = string.Empty;
                string selectedStaff = string.Empty;

                for (int alt = 0; alt < GridView5.Rows.Count; alt++)//=========================Need To All Selection insert option
                {
                    if (GridView5.Rows[alt].Visible == true)//
                    {
                        string alterScode = string.Empty;
                        ischk = true;
                        DropDownList ddlalt = (GridView5.Rows[alt].FindControl("ddlAlterStaff") as DropDownList);
                        DropDownList ddlSel = (GridView5.Rows[alt].FindControl("ddlStaff") as DropDownList);
                        alterStaff = Convert.ToString((GridView5.Rows[alt].FindControl("ddlAlterStaff") as DropDownList).SelectedItem.Text);
                        selectedStaff = Convert.ToString((GridView5.Rows[alt].FindControl("ddlStaff") as DropDownList).SelectedItem.Text);
                        selectedSubject = Convert.ToString((GridView5.Rows[alt].FindControl("lblSubjectNo") as Label).Text);
                        islab = dacess.GetFunction("select ss.Lab from subject s,sub_sem ss where s.subType_no=ss.subType_no and s.subject_no='" + selectedSubject + "'");
                        SubName = dacess.GetFunction("select s.subject_name from subject s where s.subject_no='" + selectedSubject + "'");
                        if (islab.Trim() == "1" || islab.Trim().ToLower() == "true")
                            islab = "L";
                        else
                            islab = "S";

                        if (alterStaff.ToLower() != "all" && !string.IsNullOrEmpty(alterStaff.Trim()))
                        {
                            if (!string.IsNullOrEmpty(alterStaff.Trim()) && alterStaff.Trim() != "")
                            {
                                if (alterStaff.Contains("-"))
                                {
                                    if (string.IsNullOrEmpty(sCode))
                                        sCode = alterStaff.Split('-')[1];

                                    else if (!sCode.Contains(alterStaff.Split('-')[1]))
                                        sCode = sCode + "-" + alterStaff.Split('-')[1];
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(sCode))
                                        sCode = Convert.ToString((GridView5.Rows[alt].FindControl("ddlAlterStaff") as DropDownList).SelectedItem.Value);
                                    else if (!sCode.Contains(Convert.ToString((GridView5.Rows[alt].FindControl("ddlAlterStaff") as DropDownList).SelectedItem.Value)))
                                        sCode = sCode + "-" + Convert.ToString((GridView5.Rows[alt].FindControl("ddlAlterStaff") as DropDownList).SelectedItem.Value);
                                }



                                if (!string.IsNullOrEmpty(alterStaff) && alterStaff.Trim() != "" && !string.IsNullOrEmpty(selectedStaff) && selectedStaff.Trim() != "")
                                {
                                    if (alterStaff.Contains("-"))
                                    {
                                        if (string.IsNullOrEmpty(alterScode))
                                            alterScode = alterStaff.Split('-')[1];
                                        else
                                            alterScode = alterScode + "-" + alterStaff.Split('-')[1];
                                    }
                                    else
                                    {
                                        if (string.IsNullOrEmpty(alterScode))
                                            alterScode = Convert.ToString((GridView5.Rows[alt].FindControl("ddlAlterStaff") as DropDownList).SelectedItem.Value);
                                        else
                                            alterScode = alterScode + "-" + Convert.ToString((GridView5.Rows[alt].FindControl("ddlAlterStaff") as DropDownList).SelectedItem.Value);
                                    }
                                    if (selectedStaff.Contains("-"))
                                    {
                                        if (string.IsNullOrEmpty(alterScode))
                                            alterScode = selectedStaff.Split('-')[1];

                                        else
                                            alterScode = alterScode + "-" + selectedStaff.Split('-')[1];
                                    }
                                    else
                                    {
                                        if (string.IsNullOrEmpty(alterScode))
                                            alterScode = Convert.ToString((GridView5.Rows[alt].FindControl("ddlStaff") as DropDownList).SelectedItem.Value);
                                        else
                                            alterScode = alterScode + "-" + Convert.ToString((GridView5.Rows[alt].FindControl("ddlStaff") as DropDownList).SelectedItem.Value);
                                    }
                                    if (!string.IsNullOrEmpty(alterStaffDet))
                                        alterStaffDet = alterStaffDet + "$" + alterScode;
                                    else
                                        alterStaffDet = alterScode;
                                }
                            }
                        }

                        else
                        {
                            if (selectedStaff.ToLower() != "all" && !string.IsNullOrEmpty(selectedStaff))
                            {
                                if (selectedStaff.Contains("-"))
                                {
                                    if (string.IsNullOrEmpty(sCode))
                                        sCode = selectedStaff.Split('-')[1];
                                    else if (!sCode.Contains(selectedStaff.Split('-')[1]))
                                        sCode = sCode + "-" + selectedStaff.Split('-')[1];
                                }
                                else
                                {
                                    if (string.IsNullOrEmpty(sCode))
                                        sCode = Convert.ToString((GridView5.Rows[alt].FindControl("ddlStaff") as DropDownList).SelectedItem.Value);
                                    else if (!sCode.Contains(Convert.ToString((GridView5.Rows[alt].FindControl("ddlStaff") as DropDownList).SelectedItem.Value)))
                                        sCode = sCode + "-" + Convert.ToString((GridView5.Rows[alt].FindControl("ddlStaff") as DropDownList).SelectedItem.Value);

                                }
                            }
                        }
                    }
                }
                if (!string.IsNullOrEmpty(sCode))
                {
                    if (string.IsNullOrEmpty(columnVal))
                    {
                        columnVal = selectedSubject + "-" + sCode + "-" + islab + "#" + alterStaffDet;
                        columntxt = SubName + "-" + sCode + "-" + islab;
                    }
                    else
                    {
                        columnVal = columnVal + ";" + selectedSubject + "-" + sCode + "-" + islab + "#" + alterStaffDet;
                        columntxt = columntxt + ";" + SubName + "-" + sCode + "-" + islab;
                    }
                }
            }
            else
            {
                string sCode = string.Empty;
                string islab = string.Empty;
                string alterStaff = string.Empty;
                string SubName = string.Empty;
                string selectedSubject = string.Empty;
                string selectedStaff = string.Empty;

                for (int alt = 0; alt < GridView5.Rows.Count; alt++)
                {
                    sCode = string.Empty;
                    islab = string.Empty;
                    alterStaff = string.Empty;
                    SubName = string.Empty;
                    selectedSubject = string.Empty;
                    selectedStaff = string.Empty;
                    if (GridView5.Rows[alt].Visible == true)//
                    {
                        string alterScode = string.Empty;
                        ischk = true;
                        DropDownList ddlalt = (GridView5.Rows[alt].FindControl("ddlAlterStaff") as DropDownList);
                        DropDownList ddlSel = (GridView5.Rows[alt].FindControl("ddlStaff") as DropDownList);
                        alterStaff = Convert.ToString((GridView5.Rows[alt].FindControl("ddlAlterStaff") as DropDownList).SelectedItem.Text);
                        selectedStaff = Convert.ToString((GridView5.Rows[alt].FindControl("ddlStaff") as DropDownList).SelectedItem.Text);
                        selectedSubject = Convert.ToString((GridView5.Rows[alt].FindControl("lblSubjectNo") as Label).Text);
                        islab = dacess.GetFunction("select ss.Lab from subject s,sub_sem ss where s.subType_no=ss.subType_no and s.subject_no='" + selectedSubject + "'");
                        SubName = dacess.GetFunction("select s.subject_name from subject s where s.subject_no='" + selectedSubject + "'");
                        if (islab.Trim() == "1" || islab.Trim().ToLower() == "true")
                            islab = "L";
                        else
                            islab = "S";
                        if (selectedStaff.ToLower() != "all" && !string.IsNullOrEmpty(selectedStaff))
                        {
                            if (selectedStaff.Contains("-"))
                            {
                                if (string.IsNullOrEmpty(sCode))
                                    sCode = selectedStaff.Split('-')[1];
                                else if (!sCode.Contains(selectedStaff.Split('-')[1]))
                                    sCode = sCode + "-" + selectedStaff.Split('-')[1];
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(sCode))
                                    sCode = Convert.ToString((GridView5.Rows[alt].FindControl("ddlStaff") as DropDownList).SelectedItem.Value);
                                else if (!sCode.Contains(Convert.ToString((GridView5.Rows[alt].FindControl("ddlStaff") as DropDownList).SelectedItem.Value)))
                                    sCode = sCode + "-" + Convert.ToString((GridView5.Rows[alt].FindControl("ddlStaff") as DropDownList).SelectedItem.Value);
                            }
                        }
                        else
                        {
                            for (int all = 0; all < ddlalt.Items.Count; all++)
                            {
                                if (Convert.ToString(ddlalt.Items[all].Text).ToLower() != "all" && !string.IsNullOrEmpty(Convert.ToString(ddlalt.Items[all].Text.Trim())))
                                {
                                    if (ddlalt.Items[all].Text.Contains("-"))
                                    {
                                        if (string.IsNullOrEmpty(sCode))
                                            sCode = ddlalt.Items[all].Text.Split('-')[1];
                                        else if (!sCode.Contains(ddlalt.Items[all].Text.Split('-')[1]))
                                            sCode = sCode + "-" + selectedStaff.Split('-')[1];
                                    }
                                    else
                                    {
                                        if (string.IsNullOrEmpty(sCode))
                                            sCode = Convert.ToString((ddlalt.Items[all].Value));
                                        else if (!sCode.Contains(ddlalt.Items[all].Value))
                                            sCode = sCode + "-" + Convert.ToString((ddlalt.Items[all].Value));
                                    }
                                }
                            }
                        }
                        if (!string.IsNullOrEmpty(sCode) && !string.IsNullOrEmpty(selectedSubject))
                        {
                            if (string.IsNullOrEmpty(columnVal))
                            {
                                columnVal = selectedSubject + "-" + sCode + "-" + islab;
                                columntxt = SubName + "-" + sCode + "-" + islab;
                            }
                            else
                            {
                                columnVal = columnVal + ";" + selectedSubject + "-" + sCode + "-" + islab;
                                columntxt = columntxt + ";" + SubName + "-" + sCode + "-" + islab;
                            }
                        }
                        else
                        {
                            errmsg.Visible = true;
                            errmsg.Text = "Please Select Subject/Staff !";
                            return;
                        }
                    }
                }
            }
            if (!ischk)
            {
                errmsg.Visible = true;
                errmsg.Text = "Please Select Subject !";
            }

            LinkButton lnk = (gridTimeTable.Rows[r + 1].FindControl("lnkPeriod_" + colIndex) as LinkButton);
            lnk.Text = columntxt;
            Label lbl = (gridTimeTable.Rows[r + 1].FindControl("lblPeriod_" + colIndex) as Label);
            lbl.Text = columnVal;

            if (!string.IsNullOrEmpty(columnVal) && !string.IsNullOrEmpty(columntxt))
            {

            }

        }
        catch
        {
        }
    }
    public void btnMasterSave_Click(object sender, EventArgs e)
    {
        try
        {
            if (gridTimeTable.Rows.Count > 0)
            {
                string StaffCode = Session["staff_code"].ToString().Trim();
                int intNHrs = 0;
                int SchOrder = 0;
                string strDay = string.Empty;
                int nodays = 0;
                Dictionary<string, string> dicParam = new Dictionary<string, string>();
                string getday = string.Empty;
                string code_value = string.Empty;
                int insert = 0;
                string acDet = string.Empty;
                string updatetxt = string.Empty;
                for (int row = 1; row < gridTimeTable.Rows.Count; row += 2)//
                {
                    string Date = (gridTimeTable.Rows[row].FindControl("lblDateDisp") as Label).Text;
                    getday = string.Empty;
                    code_value = string.Empty;
                    string dateVal = (gridTimeTable.Rows[row].FindControl("lblDate") as Label).Text;
                    DateTime dt = Convert.ToDateTime(dateVal);
                    string degVal = (gridTimeTable.Rows[row].FindControl("lblDegreeval") as Label).Text;
                    string batch = string.Empty;
                    string degCode = string.Empty;
                    string sem = string.Empty;
                    string sec = string.Empty;
                    string strsec = string.Empty;
                    string startdate = string.Empty;
                    DataTable dtAlterInfo = dirAcc.selectDataTable("select * from alterschedule where  alternateDate='" + dt.ToString("MM/dd/yyyy") + "'");
                    if (degVal.Contains('-'))
                    {
                        string[] Input = degVal.Split('-');
                        batch = Convert.ToString(Input[0]);
                        degCode = Convert.ToString(Input[1]);
                        sem = Convert.ToString(Input[2]);
                        if (!string.IsNullOrEmpty(Convert.ToString(Input[3]).Trim()) && Convert.ToString(Input[2]).Trim() != "" && Convert.ToString(Input[3]).Trim() != null)
                        {
                            sec = Convert.ToString(Input[3]).Trim();
                            strsec = " and sections='" + Convert.ToString(Input[3]).Trim() + "'";
                        }
                    }
                    string periodSchedQry = "Select No_of_hrs_per_day,schorder,nodays from periodattndschedule  where degree_code='" + degCode + "' and semester = '" + sem + "'";
                    DataSet periodSchedDataSet = dacess.select_method_wo_parameter(periodSchedQry, "Text");

                    if (periodSchedDataSet.Tables.Count > 0 && periodSchedDataSet.Tables[0].Rows.Count > 0)
                    {
                        if (Convert.ToString(periodSchedDataSet.Tables[0].Rows[0]["No_of_hrs_per_day"]) != "")
                        {
                            intNHrs = Convert.ToInt32(periodSchedDataSet.Tables[0].Rows[0]["No_of_hrs_per_day"]);
                            SchOrder = Convert.ToInt32(periodSchedDataSet.Tables[0].Rows[0]["schorder"]);
                            nodays = Convert.ToInt32(periodSchedDataSet.Tables[0].Rows[0]["nodays"]);
                        }
                    }
                    string semDetailsQry = "select * from seminfo where degree_code=" + degCode + " and semester=" + sem + " and batch_year=" + batch + " ";
                    DataSet semDetailsDataSet = dacess.select_method_wo_parameter(semDetailsQry, "Text");
                    if (semDetailsDataSet.Tables.Count > 0 && semDetailsDataSet.Tables[0].Rows.Count > 0)
                    {
                        if ((Convert.ToString(semDetailsDataSet.Tables[0].Rows[0]["start_date"])) != "" && (Convert.ToString(semDetailsDataSet.Tables[0].Rows[0]["start_date"])) != "\0")
                        {
                            string[] tmpdate = Convert.ToString(semDetailsDataSet.Tables[0].Rows[0]["start_date"]).Split(new char[] { ' ' });
                            startdate = tmpdate[0].ToString();
                            if (Convert.ToString(semDetailsDataSet.Tables[0].Rows[0]["starting_dayorder"]) != "")
                            {
                                start_dayorder = Convert.ToString(semDetailsDataSet.Tables[0].Rows[0]["starting_dayorder"]);
                            }
                            else
                            {
                                start_dayorder = "1";
                            }
                        }
                        else
                        {
                            norecordlbl.Visible = true;
                            norecordlbl.Text = "Update semester Information";
                            norecordlbl.ForeColor = Color.Red;
                        }
                    }
                    else
                    {
                        norecordlbl.Visible = true;
                        norecordlbl.Text = "Update semester Information";
                        norecordlbl.ForeColor = Color.Red;
                    }

                    if (SchOrder != 0)
                    {
                        strDay = dt.ToString("ddd");
                    }
                    else
                    {
                        strDay = dacess.findday(dt.ToString(), degCode, sem, batch, startdate.ToString(), nodays.ToString(), start_dayorder.ToString());
                    }
                    string ttname = dacess.GetFunction("select  top 1 ttname from Semester_Schedule where batch_year=" + batch + " and degree_code = " + degCode + " and semester = " + sem + " and FromDate <='" + dt.ToString("MM/dd/yyyy") + "'" + strsec + " order by FromDate desc");
                    if (ttname.Trim() != "" && ttname != null && ttname.Trim() != "0")
                    {
                        ttname = " and Timetablename='" + ttname + "'";
                    }
                    string delsql = "delete from Alternate_schedule where batch_year=" + batch + " and degree_code = " + degCode + " and semester = " + sem + " and FromDate ='" + dateVal + "'" + strsec + string.Empty;
                    int del11 = dacess.update_method_wo_parameter(delsql, "text");

                    for (int cell = 2; cell < gridTimeTable.Columns.Count; cell++)
                    {
                        if (gridTimeTable.Columns[cell].Visible == true)
                        {
                            string period = (gridTimeTable.HeaderRow.Cells[cell].Text).Split(' ')[1];
                            string AlterDet = (gridTimeTable.Rows[row].FindControl("lblPeriod_" + period) as Label).Text;
                            acDet = (gridTimeTable.Rows[row - 1].FindControl("lblPeriod_" + period) as Label).Text;
                            string ActSubject = string.Empty;
                            string altsubName = string.Empty;
                            string altStaffCode = string.Empty;
                            if (!string.IsNullOrEmpty(acDet))
                            {
                                string[] rec = acDet.Split(';');
                                for (int r = 0; r < rec.Length; r++)
                                {
                                    if (Convert.ToString(rec[r]).Contains(StaffCode))
                                    {
                                        ActSubject = Convert.ToString(rec[r]).Split('-')[0];
                                    }
                                }
                            }

                            if (!string.IsNullOrEmpty(AlterDet))
                            {
                                string[] rec = AlterDet.Split(';');
                                for (int r = 0; r < rec.Length; r++)
                                {
                                    //if (Convert.ToString(rec[r]).Contains(StaffCode))
                                    //{
                                    altsubName = Convert.ToString(rec[r]).Split('-')[0];
                                    string[] recstaff = Convert.ToString(rec[r]).Split('-');
                                    for (int st = 0; st < recstaff.Length; st++)
                                    {
                                        if (st > 0 && Convert.ToString(recstaff[st]).Trim().ToLower() != "s" && Convert.ToString(recstaff[st]).Trim().ToLower() != "l" && Convert.ToString(recstaff[st]).Trim().ToLower() != "c")
                                        {
                                            if (string.IsNullOrEmpty(altStaffCode))
                                                altStaffCode = Convert.ToString(recstaff[st]);
                                            else
                                                altStaffCode = altStaffCode + ";" + Convert.ToString(recstaff[st]);
                                        }
                                    }
                                    //}
                                }
                            }
                            if (!string.IsNullOrEmpty(altStaffCode) && !string.IsNullOrEmpty(altsubName) && !string.IsNullOrEmpty(StaffCode) && !string.IsNullOrEmpty(ActSubject))
                            {

                                int delQ = dacess.update_method_wo_parameter("delete AlternateDetails where batch_year='" + Convert.ToString(batch) + "' and degree_code='" + Convert.ToString(degCode) + "' and semester='" + Convert.ToString(sem) + "' and sections='" + Convert.ToString(sec) + "' and AlternateDate='" + dt.ToString("MM/dd/yyyy") + "' and AlterHour='" + period.ToString() + "'  and Noalter='1' and ActstaffCode='" + StaffCode.ToString() + "' ", "text");
                                if (!string.IsNullOrEmpty(StaffCode))
                                {
                                    string insertQ = "if not exists(select * from AlternateDetails where batch_year='" + Convert.ToString(batch) + "' and degree_code='" + Convert.ToString(degCode) + "' and semester='" + Convert.ToString(sem) + "' and sections='" + Convert.ToString(sec) + "' and AlternateDate='" + dt.ToString("MM/dd/yyyy") + "' and AlterHour='" + period.ToString() + "'  and Noalter='1' and ActstaffCode='" + StaffCode.ToString() + "')   insert into AlternateDetails (AlternateDate,AlterHour,Noalter ,ActSubNo ,ActstaffCode ,AlterSubNo , alterStaffCode ,batch_Year ,Degree_code ,Semester ,Sections) values ('" + dt.ToString("MM/dd/yyyy") + "','" + period.ToString() + "','1','" + ActSubject + "','" + StaffCode + "','" + altsubName + "','" + altStaffCode + "','" + Convert.ToString(batch) + "','" + Convert.ToString(degCode) + "','" + Convert.ToString(sem) + "','" + Convert.ToString(sec) + "')";

                                    int ins = dacess.update_method_wo_parameter(insertQ, "text");
                                }
                            }


                            if (string.IsNullOrEmpty(getday))
                            {
                                getday = strDay + period.ToString();
                            }
                            else
                            {
                                getday = getday + "," + strDay + period.ToString();
                            }
                            if (AlterDet.Contains('#'))//10714-MATT003-S#MATT003-MATT004$MATT003-MATT518
                            {
                                dicParam.Clear();
                                dicParam.Add("alternateDate", dt.ToString("MM/dd/yyyy"));
                                dicParam.Add("alternateHour", period.ToString());
                                int del = storeAcc.deleteData("uspDeleteAlternateStaffByDateHour", dicParam);

                                string[] alter = AlterDet.Split('#');
                                string Schdule = Convert.ToString(alter[0]);
                                string subNo = Convert.ToString(Schdule.Split('-')[0]);

                                string altr = Convert.ToString(alter[1]);
                                string Acstaff = string.Empty;
                                string AlStaff = string.Empty;
                                if (altr.Contains('$'))
                                {
                                    string[] altstaff = altr.Split('$');
                                    for (int no = 0; no < altstaff.Length; no++)
                                    {
                                        string staffCode = Convert.ToString(altstaff[no]);
                                        Acstaff = Convert.ToString(staffCode.Split('-')[1]);
                                        AlStaff = Convert.ToString(staffCode.Split('-')[0]);
                                        dicParam.Clear();
                                        dicParam.Add("alternateDate", dt.ToString("MM/dd/yyyy"));
                                        dicParam.Add("alternateHour", period.ToString());
                                        dicParam.Add("subjectNo", subNo.ToString());
                                        dicParam.Add("actualStaffCode", Acstaff.ToString());
                                        dicParam.Add("alterStaffCode", AlStaff.ToString());
                                        int insAlter = storeAcc.insertData("uspInsertAlternateStaffDetail", dicParam);
                                    }
                                }
                                else
                                {
                                    Acstaff = Convert.ToString(altr.Split('-')[1]);
                                    AlStaff = Convert.ToString(altr.Split('-')[0]);
                                    dicParam.Clear();
                                    dicParam.Add("alternateDate", dt.ToString("MM/dd/yyyy"));
                                    dicParam.Add("alternateHour", period.ToString());
                                    dicParam.Add("subjectNo", subNo.ToString());
                                    dicParam.Add("actualStaffCode", Acstaff.ToString());
                                    dicParam.Add("alterStaffCode", AlStaff.ToString());
                                    int insAlter = storeAcc.insertData("uspInsertAlternateStaffDetail", dicParam);
                                }
                            }
                            string AltCo = string.Empty;
                            if (AlterDet.Contains('#'))//10714-MATT003-S#MATT003-MATT004$MATT003-MATT518
                                AltCo = Convert.ToString(AlterDet.Split('#')[0]);
                            else
                                AltCo = Convert.ToString(AlterDet);

                            if (string.IsNullOrEmpty(code_value) || code_value == "")
                            {
                                code_value = "'" + AltCo + "'";
                            }
                            else
                            {
                                code_value = code_value + ",'" + AltCo + "'";
                            }

                        }
                    }

                    if (code_value != "" || string.IsNullOrEmpty(code_value))
                    {
                        string strinsert = "insert into Alternate_schedule(degree_code,semester,batch_year,fromdate,lastrec,sections," + getday + ") values(" + degCode + "," + sem + "," + batch + ",'" + dateVal + "',0,'" + sec + "'," + code_value + ")";
                        insert = dacess.update_method_wo_parameter(strinsert, "text");
                    }
                }
                if (insert != 0)
                {
                    Div7.Visible = true;
                    Label1.Visible = true;
                    Label1.Text = "Saved Sucessfully.!";
                    return;
                }
            }


        }
        catch
        {

        }
    }
    public void loadschedule()
    {
        try
        {
            string strsec = string.Empty;
            int intNHrs = 0;
            int SchOrder = 0;
            int nodays = 0;
            string srt_day = string.Empty;
            int order = 0;
            int insert_val = 0;
            string sunjno_staffno = string.Empty;
            int subj_no = 0;
            string acronym_val = string.Empty;
            int day_list = 0;
            string day_order = string.Empty;
            int ind_subj = 0;
            string sunjno_staffno_s = string.Empty;
            string acro = string.Empty;
            string acronym = string.Empty;
            string alt_sched = string.Empty;
            string shed_list = string.Empty;
            int spreadDet_ac = 0;
            string todate = string.Empty;

            DataTable dtTTDisp = new DataTable();
            dtTTDisp.Columns.Add("DateDisp");
            dtTTDisp.Columns.Add("DateVal");
            dtTTDisp.Columns.Add("P1Val");
            dtTTDisp.Columns.Add("PVal1");
            dtTTDisp.Columns.Add("TT_1");
            dtTTDisp.Columns.Add("P2Val");
            dtTTDisp.Columns.Add("PVal2");
            dtTTDisp.Columns.Add("TT_2");
            dtTTDisp.Columns.Add("P3Val");
            dtTTDisp.Columns.Add("PVal3");
            dtTTDisp.Columns.Add("TT_3");
            dtTTDisp.Columns.Add("P4Val");
            dtTTDisp.Columns.Add("PVal4");
            dtTTDisp.Columns.Add("TT_4");
            dtTTDisp.Columns.Add("P5Val");
            dtTTDisp.Columns.Add("PVal5");
            dtTTDisp.Columns.Add("TT_5");
            dtTTDisp.Columns.Add("P6Val");
            dtTTDisp.Columns.Add("PVal6");
            dtTTDisp.Columns.Add("TT_6");
            dtTTDisp.Columns.Add("P7Val");
            dtTTDisp.Columns.Add("PVal7");
            dtTTDisp.Columns.Add("TT_7");
            dtTTDisp.Columns.Add("P8Val");
            dtTTDisp.Columns.Add("PVal8");
            dtTTDisp.Columns.Add("TT_8");
            dtTTDisp.Columns.Add("P9Val");
            dtTTDisp.Columns.Add("PVal9");
            dtTTDisp.Columns.Add("TT_9");
            dtTTDisp.Columns.Add("P10Val");
            dtTTDisp.Columns.Add("PVal10");
            dtTTDisp.Columns.Add("TT_10");
            GridView1.Visible = false;
            DataRow drNew = null;
            //-------------date
            string date1;
            string selectedDate;
            date1 = txtDate.Text.ToString();
            string[] split = date1.Split(new Char[] { '/' });
            selectedDate = split[1].ToString() + "/" + split[0].ToString() + "/" + split[2].ToString();
            string[] DaysAcronym = new string[7] { "mon", "tue", "wed", "thu", "fri", "sat", "sun" };
            string[] DaysName = new string[7] { "Monday", "Tuesday", "wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
            DateTime dtSelectedDate = Convert.ToDateTime(selectedDate.ToString());

            string semStartdate = string.Empty;
            //-------------start date

            string qry = "select start_date from seminfo where degree_code=" + Convert.ToString(Session["degcode"]) + " and semester=" + Convert.ToString(Session["sem"]) + " and batch_year=" + Convert.ToString(Session["batch"]) + " ";

            DataSet qryDataSet = dacess.select_method_wo_parameter(qry, "Text");

            if (qryDataSet.Tables.Count > 0 && qryDataSet.Tables[0].Rows.Count > 0)
            {
                semStartdate = Convert.ToString(qryDataSet.Tables[0].Rows[0]["start_date"]);
            }
            //-------section
            if (Convert.ToString(Session["sec"]) == " ")
            {
                strsec = string.Empty;
            }
            else
            {
                if (Convert.ToString(Session["sec"]) == "-1")
                {
                    strsec = string.Empty;
                }
                else
                {
                    strsec = " and sections='" + Convert.ToString(Session["sec"]) + "'";
                }
            }


            string periodDetailsQry = "Select No_of_hrs_per_day,schorder,nodays from periodattndschedule where degree_code=" + Convert.ToString(Session["degcode"]) + " and semester = " + Convert.ToString(Session["sem"]) + "";
            DataSet periodDetailsDataSet = dacess.select_method_wo_parameter(periodDetailsQry, "Text");
            if (periodDetailsDataSet.Tables.Count > 0 && periodDetailsDataSet.Tables[0].Rows.Count > 0)
            {
                if (Convert.ToString(periodDetailsDataSet.Tables[0].Rows[0]["No_of_hrs_per_day"]) != "")
                {
                    intNHrs = Convert.ToInt32(periodDetailsDataSet.Tables[0].Rows[0]["No_of_hrs_per_day"]);
                    SchOrder = Convert.ToInt32(periodDetailsDataSet.Tables[0].Rows[0]["schorder"]);
                    nodays = Convert.ToInt32(periodDetailsDataSet.Tables[0].Rows[0]["nodays"]);

                }
            }
            //------------------------dayorder

            string[] daylist = { "mon", "tue", "wed", "thu", "fri", "sat", "sun" };

            string semScheduleQry = "select top 1 * from semester_schedule where batch_year=" + Convert.ToString(Session["batch"]) + " and degree_code = " + Convert.ToString(Session["degcode"]) + " and semester = " + Convert.ToString(Session["sem"]) + " and FromDate<= ' " + Convert.ToString(selectedDate) + " ' " + strsec + " order by fromdate desc";
            DataSet semScheduleDataSet = dacess.select_method_wo_parameter(semScheduleQry, "Text");

            if (semScheduleDataSet.Tables.Count > 0 && semScheduleDataSet.Tables[0].Rows.Count > 0)
            {

                for (day_list = 0; day_list < nodays; day_list++)
                {
                    drNew = dtTTDisp.NewRow();
                    for (insert_val = 1; insert_val <= intNHrs; insert_val++)
                    {
                        //semspread.Sheets[0].ColumnHeader.Cells[0, insert_val - 1].Text = "Period " + insert_val.ToString();

                        acro = string.Empty;
                        shed_list = string.Empty;
                        day_order = daylist[day_list] + insert_val.ToString();
                        string dayName = DaysName[day_list];
                        string dayAcronym = DaysAcronym[day_list];

                        if (SchOrder == 1)
                        {
                            drNew["DateDisp"] = dayName;
                            drNew["DateVal"] = dayAcronym;
                        }
                        else
                        {
                            int dayNo = day_list + 1;
                            drNew["DateDisp"] = "Day " + dayNo;
                            drNew["DateVal"] = dayNo;
                        }
                        sunjno_staffno = Convert.ToString(semScheduleDataSet.Tables[0].Rows[0][day_order]);

                        //---------------getupper bound for many subject
                        string[] many_subj = sunjno_staffno.Split(new Char[] { ';' });
                        for (ind_subj = 0; ind_subj <= many_subj.GetUpperBound(0); ind_subj++)
                        {
                            if (many_subj.GetUpperBound(0) >= 0)
                            {
                                sunjno_staffno_s = many_subj[ind_subj];
                                if (sunjno_staffno_s.Trim() != "")
                                {
                                    //---------------------------
                                    string[] subjno_staffno_splt = sunjno_staffno_s.Split(new Char[] { '-' });
                                    subj_no = Convert.ToInt32(subjno_staffno_splt[0].ToString());
                                    //---------tag
                                    SqlDataReader sub_dr;
                                    SqlCommand sub_cmd;
                                    con2a.Close();
                                    con2a.Open();
                                    sub_cmd = new SqlCommand("select subject_name from subject where subject_no=" + subj_no.ToString() + "", con2a);
                                    sub_dr = sub_cmd.ExecuteReader();
                                    sub_dr.Read();
                                    if (sub_dr.HasRows == true)
                                    {
                                        alt_sched = sub_dr[0].ToString() + "-" + subjno_staffno_splt[1].ToString() + "-" + subjno_staffno_splt[2].ToString();
                                    }
                                    //------------------
                                    cona.Close();
                                    cona.Open();
                                    acronym_val = "select isnull(acronym,subject_code) acronym from subject where subject_no=" + subj_no.ToString() + " ";
                                    SqlCommand ac_cmd = new SqlCommand(acronym_val, cona);
                                    SqlDataReader ac_dr;
                                    ac_dr = ac_cmd.ExecuteReader();
                                    ac_dr.Read();
                                    if (ac_dr.HasRows == true)
                                    {
                                        acronym = ac_dr["acronym"].ToString();
                                        if (acro == "")
                                        {
                                            acro = acro + acronym;
                                        }
                                        else
                                        {
                                            acro = acro + "," + acronym;
                                        }
                                        if (shed_list == "")
                                        {
                                            shed_list = shed_list + alt_sched;
                                        }
                                        else
                                        {
                                            shed_list = shed_list + ";" + alt_sched;
                                        }
                                    }
                                }
                            }
                        }

                        string lbl1 = "P" + insert_val + "Val";
                        string lbl3 = "PVal" + insert_val;
                        string lbl2 = "TT_" + insert_val;

                        drNew[lbl1] = acro;
                        drNew[lbl2] = shed_list;
                        drNew[lbl3] = sunjno_staffno;
                    }
                    dtTTDisp.Rows.Add(drNew);
                }
                if (dtTTDisp.Rows.Count > 0)
                {
                    GridView1.DataSource = dtTTDisp;
                    GridView1.DataBind();
                    GridView1.Visible = true;
                    div1.Visible = true;
                    if (intNHrs > 0)
                    {
                        for (int i = 1; i <= intNHrs; i++)
                        {
                            GridView1.Columns[i].Visible = true;
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {

        }
    }
    public void btnAsPerDaySchedule_Click()
    {
        try
        {

            string Syllabus_year = string.Empty;
            Syllabus_year = GetSyllabusYear(Convert.ToString(Session["degcode"]), Convert.ToString(Session["batch"]), Convert.ToString(Session["sem"]));
            if ((Syllabus_year).ToString() != "0" && cellTagValue != "")
            {
                loadschedule();

                GridView1.Visible = true;
                semmsglbl.Visible = false;
            }
            if (cellTagValue == "")
            {
                GridView1.Visible = false;
                semmsglbl.Visible = true;
                semmsglbl.Text = "Please select valid cell!";
            }
        }
        catch (Exception ex) { }
    }
    protected void lnkAttMark11(object sender, EventArgs e)
    {
        LinkButton lnkSelected = (LinkButton)sender;
        string rowIndxS = lnkSelected.UniqueID.ToString().Split('$')[3].Replace("ctl", string.Empty);
        int rowIndx = Convert.ToInt32(rowIndxS) - 2;
        string colIndxS = lnkSelected.UniqueID.ToString().Split('$')[4].Replace("lnkPeriod_", string.Empty);
        int colIndx = Convert.ToInt32(colIndxS);
        string colIndex = Convert.ToString(Session["Col"]);
        string Rowindex = Convert.ToString(Session["Row"]);
        int r = 0;
        int.TryParse(Rowindex, out r);
        for (int a = 1; a < GridView1.Columns.Count; a++)
        {
            string disptext = (GridView1.Rows[rowIndx].FindControl("lblTT_" + a) as Label).Text;
            string tagtxt = (GridView1.Rows[rowIndx].FindControl("lblPeriod_" + a) as Label).Text;

            LinkButton lnk = (gridTimeTable.Rows[r + 1].FindControl("lnkPeriod_" + a) as LinkButton);
            lnk.Text = disptext;
            Label lbl = (gridTimeTable.Rows[r + 1].FindControl("lblPeriod_" + a) as Label);
            lbl.Text = tagtxt;

        }

    }
    protected void Button6_Clik(object sender, EventArgs e)
    {
        div1.Visible = false;
    }
    protected void Button1_Clik(object sender, EventArgs e)
    {
        try
        {
            int intNHrs = 0;
            int SchOrder = 0;
            string strDay = string.Empty;
            int nodays = 0;
            int del = 0;
            int row = Convert.ToInt16(Session["Row"]);
            int col = Convert.ToInt16(Session["Col"]);
            string dateVal = (gridTimeTable.Rows[row].FindControl("lblDate") as Label).Text;
            DateTime dt = Convert.ToDateTime(dateVal);
            string degVal = (gridTimeTable.Rows[row].FindControl("lblDegreeval") as Label).Text;
            string batch = string.Empty;
            string degCode = string.Empty;
            string sem = string.Empty;
            string sec = string.Empty;
            string strsec = string.Empty;
            string startdate = string.Empty;
            string columnValue = string.Empty;
            if (degVal.Contains('-'))
            {
                string[] Input = degVal.Split('-');
                batch = Convert.ToString(Input[0]);
                degCode = Convert.ToString(Input[1]);
                sem = Convert.ToString(Input[2]);
                if (!string.IsNullOrEmpty(Convert.ToString(Input[3]).Trim()) && Convert.ToString(Input[2]).Trim() != "" && Convert.ToString(Input[3]).Trim() != null)
                {
                    sec = Convert.ToString(Input[3]).Trim();
                    strsec = "  and sections='" + Convert.ToString(Input[3]).Trim() + "'";
                }
            }
            string periodSchedQry = "Select No_of_hrs_per_day,schorder,nodays from periodattndschedule  where degree_code='" + degCode + "' and semester = '" + sem + "'";
            DataSet periodSchedDataSet = dacess.select_method_wo_parameter(periodSchedQry, "Text");

            if (periodSchedDataSet.Tables.Count > 0 && periodSchedDataSet.Tables[0].Rows.Count > 0)
            {
                if (Convert.ToString(periodSchedDataSet.Tables[0].Rows[0]["No_of_hrs_per_day"]) != "")
                {
                    intNHrs = Convert.ToInt32(periodSchedDataSet.Tables[0].Rows[0]["No_of_hrs_per_day"]);
                    SchOrder = Convert.ToInt32(periodSchedDataSet.Tables[0].Rows[0]["schorder"]);
                    nodays = Convert.ToInt32(periodSchedDataSet.Tables[0].Rows[0]["nodays"]);
                }
            }
            if (SchOrder != 0)
            {
                strDay = dt.ToString("ddd");
            }
            else
            {
                strDay = dacess.findday(dt.ToString(), degCode, sem, batch, startdate.ToString(), nodays.ToString(), start_dayorder.ToString());
            }
            string period = strDay + col;
            string deleteQ = string.Empty;
            string StaffCode = Session["staff_code"].ToString().Trim();
            if (!string.IsNullOrEmpty(StaffCode))
            {
                string actSch = dacess.GetFunction("select " + period + " from Semester_Schedule where batch_year='" + batch + "' and degree_code='" + degCode + "' and semester='" + sem + "' " + strsec + " and FromDate='" + dt.ToString("MM/dd/yyyy") + "'");
                string alternate = dacess.GetFunction("select " + period + " from Alternate_Schedule where batch_year='" + batch + "' and degree_code='" + degCode + "' and semester='" + sem + "' " + strsec + " and FromDate='" + dt.ToString("MM/dd/yyyy") + "'");
                if (alternate.Contains(StaffCode))
                {
                    if (alternate != "" && alternate != "0" && alternate != null)
                    {

                        if (alternate.Contains(';'))
                        {
                            string temp = "";
                            string[] arrVal = alternate.Split(';');
                            for (int i = 0; i < arrVal.Length; i++)
                            {
                                string val = arrVal[i];

                                if (val.Contains(StaffCode))
                                {

                                }
                                else
                                {
                                    if (temp == "")
                                        temp = val;
                                    else
                                        temp = temp + ";" + val;
                                }
                            }
                            columnValue = temp;
                        }
                        else
                        {
                            columnValue = null;
                        }
                    }
                    else
                    {
                        columnValue = alternate;
                    }
                    deleteQ = "update Alternate_Schedule SET " + period + "='" + columnValue + "' where batch_year='" + batch + "' and degree_code='" + degCode + "' and semester='" + sem + "' " + strsec + " and FromDate='" + dt.ToString("MM/dd/yyyy") + "'";
                }
            }
            else
            {
                deleteQ = "update Alternate_Schedule SET " + period + "='' where batch_year='" + batch + "' and degree_code='" + degCode + "' and semester='" + sem + "' " + strsec + " and FromDate='" + dt.ToString("MM/dd/yyyy") + "'";
            }
            if (!string.IsNullOrEmpty(deleteQ))
                del = dacess.update_method_wo_parameter(deleteQ, "text");
            else
            {
                Div7.Visible = false;
                Label1.Visible = true;
                Label1.Text = "Not Deleted.!";
            }

            if (del != 0)
            {

                Div7.Visible = true;
                Label1.Visible = true;
                Label1.Text = "Deleted.!";
                Div5.Visible = false;
                lblAlrt.Visible = false;
                lblAlrt.Text = "";
            }
            else
            {
                Div7.Visible = false;
                Label1.Visible = true;
                Label1.Text = "Not Deleted.!";
            }


        }
        catch
        {
        }
    }
    protected void Button2_Clik(object sender, EventArgs e)
    {
        Div5.Visible = false;
        lblAlrt.Visible = false;
        lblAlrt.Text = "";
    }
    protected void Button4_Clik(object sender, EventArgs e)
    {
        Div7.Visible = false;
        Label1.Visible = false;
        Label1.Text = "";
        btnGo_Click(sender, e);
    }

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

    private void checkBoxListselectOrDeselect(CheckBoxList cbl, bool selected = true)
    {
        try
        {
            foreach (wc.ListItem li in cbl.Items)
            {
                li.Selected = selected;
            }
        }
        catch
        {
        }
    }

    #endregion
}