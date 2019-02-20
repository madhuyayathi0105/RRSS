using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using InsproDataAccess;
using System.Collections;

public partial class Attendanceduplicateentryremove : System.Web.UI.Page
{
    string q1 = "";
    DataSet ds = new DataSet();
    DataSet ds1 = new DataSet();
    DAccess2 d2 = new DAccess2();
    InsproDirectAccess dir = new InsproDirectAccess();
    protected void Page_Load(object sender, EventArgs e)
    {
        //btn_update_click(sender, e);
    }
    protected void btn_update_click(object sender, EventArgs e)
    {
        try
        {
            q1 = " select COUNT(a.roll_no) as rollnocount,a.roll_no,a.month_year,a.roll_no+'-'+CONVERT(varchar(20), a.month_year)as rollmonthyear from attendance a,Registration r where a.roll_no=r.Roll_No and r.Batch_Year in (2013,2014,2015,2016,2017,2018)  group by a.roll_no,a.month_year  having count(a.roll_no) > 1";//and month_year =24205 and r.roll_no in('15CS002','13CS005','13CS034','15CS001','15CS002') 
            ds.Clear(); int rowaffected = 0;
            ds = d2.select_method_wo_parameter(q1, "text");
            if (ds.Tables[0].Rows.Count > 0)
            {
                string duplicaterollno = GetSelectedItemsValueAsString(ds, "Roll_no");
                string duplicaterollnocomma = GetSelectedItemsValueAsStringcomma(ds, "rollmonthyear");
                string monthandyearSingle = GetSelectedItemsValueAsString(ds, "month_year");
                string monthandyearcomma = GetSelectedItemsValueAsStringcomma(ds, "month_year");

                DataView dv = new DataView(); DataView dv1 = new DataView();
                q1 = " select CONVERT(varchar(10), a.Att_CollegeCode) Att_CollegeCode,convert(varchar(10),r.college_code) college_code, * from attendance a,Registration r where a.roll_no=r.Roll_No and r.Batch_Year in (2013,2014,2015,2016,2017,2018)  and r.roll_no in('" + duplicaterollno + "') and a.month_year in('" + monthandyearSingle + "') order by r.roll_no";
                q1 += " select column_name from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='attendance' and column_name <>'roll_no' and column_name<>'month_year' and column_name<>'Att_App_no' and column_name<>'Att_CollegeCode'";
                ds.Clear();
                ds = d2.select_method_wo_parameter(q1, "text");
                if (ds.Tables[0].Rows.Count > 0)
                {
                    string[] rollnoA = duplicaterollnocomma.Split(',');
                    string sqlcolum = ""; string updatemonthyear = "";
                    string regclgcode = "";
                    foreach (string Rno in rollnoA)
                    {
                        string[] rollmonyear = Rno.Split('-'); sqlcolum = ""; updatemonthyear = "";
                        if (rollmonyear.Length > 1)
                        {
                            ds.Tables[0].DefaultView.RowFilter = "roll_no ='" + rollmonyear[0].ToString() + "' and month_year='" + rollmonyear[1].ToString() + "'";//Convert.ToString(monthyear[m])
                            dv = ds.Tables[0].DefaultView;
                            DataTable temp = dv.ToTable();
                            if (temp.Rows.Count > 0)
                            {
                                foreach (DataRow dr1 in temp.Rows)
                                {
                                    string attclgcode = Convert.ToString(dr1["Att_CollegeCode"]);
                                    regclgcode = Convert.ToString(dr1["college_code"]);
                                    string monthandyear = Convert.ToString(dr1["month_year"]);
                                    if (attclgcode != regclgcode)
                                    {
                                        if (ds.Tables[1].Rows.Count > 0)
                                        {
                                            foreach (DataRow colname in ds.Tables[1].Rows)
                                            {
                                                string col = Convert.ToString(colname["column_name"]);
                                                temp.DefaultView.RowFilter = col + " is not null and Att_CollegeCode<>college_code";// OR " + col + "='2' OR " + col + "='3' ";
                                                dv1 = temp.DefaultView;
                                                if (dv1.Count > 0)
                                                {
                                                    sqlcolum += "," + col + "='" + Convert.ToString(dv1[0][col]).Trim() + "'";
                                                    updatemonthyear = "," + monthandyear;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            if (sqlcolum.Trim() != "")
                            {
                                q1 = " update attendance set " + sqlcolum.TrimStart(',') + " where roll_no='" + rollmonyear[0].ToString() + "' and month_year in(" + updatemonthyear.TrimStart(',') + ") and Att_CollegeCode='" + regclgcode + "'";
                                rowaffected += d2.update_method_wo_parameter(q1, "text");
                            }
                        }
                        q1 = "";
                    }
                }
            }
            lbl_error.Text = Convert.ToString("No of Rows Affected (" + rowaffected + ")");
            lbl_error.ForeColor = System.Drawing.Color.Green;
        }
        catch (Exception ex)
        {
            lbl_error.Text = Convert.ToString(ex);
            lbl_error.ForeColor = System.Drawing.Color.Red;
        }
    }
    protected void btn_update2_click(object sender, EventArgs e)
    {
        try
        {
            q1 = " select COUNT(a.roll_no) as rollnocount,a.roll_no,a.month_year,a.roll_no+'-'+CONVERT(varchar(20), a.month_year)as rollmonthyear from attendance a,Registration r where a.roll_no=r.Roll_No and r.Batch_Year in (2013,2014,2015,2016,2017,2018) group by a.roll_no,a.month_year  having count(a.roll_no) > 1";
            // //and a.roll_no in('13UCER162','14CE105','15EC057','13UCER136','14CE049')
            //and month_year =24205 and r.roll_no in('15CS002','13CS005','13CS034','15CS001','15CS002') 
            ds.Clear(); int rowaffected = 0;
            ds = d2.select_method_wo_parameter(q1, "text");
            if (ds.Tables[0].Rows.Count > 0)
            {
                string duplicaterollno = GetSelectedItemsValueAsString(ds, "Roll_no");
                string duplicaterollnocomma = GetSelectedItemsValueAsStringcomma(ds, "rollmonthyear");
                string monthandyearSingle = GetSelectedItemsValueAsString(ds, "month_year");
                string monthandyearcomma = GetSelectedItemsValueAsStringcomma(ds, "month_year");

                DataView dv = new DataView(); DataView dv1 = new DataView();
                q1 = " select CONVERT(varchar(10), a.Att_CollegeCode) Att_CollegeCode,convert(varchar(10),r.college_code) college_code, * from attendance a,Registration r where a.roll_no=r.Roll_No and r.Batch_Year in (2013,2014,2015,2016,2017,2018)  and r.roll_no in('" + duplicaterollno + "') and a.month_year in('" + monthandyearSingle + "')  order by r.roll_no";
                q1 += " select column_name from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='attendance' and column_name <>'roll_no' and column_name<>'month_year' and column_name<>'Att_App_no' and column_name<>'Att_CollegeCode'";
                ds.Clear();
                ds = d2.select_method_wo_parameter(q1, "text");
                if (ds.Tables[0].Rows.Count > 0)
                {
                    string[] rollnoA = duplicaterollnocomma.Split(',');
                    string sqlcolum = ""; string updatemonthyear = "";
                    string regclgcode = "";
                    foreach (string Rno in rollnoA)
                    {
                        string[] rollmonyear = Rno.Split('-'); sqlcolum = ""; updatemonthyear = "";
                        if (rollmonyear.Length > 1)
                        {
                            ds.Tables[0].DefaultView.RowFilter = "roll_no ='" + rollmonyear[0].ToString() + "' and month_year='" + rollmonyear[1].ToString() + "'";
                            dv = ds.Tables[0].DefaultView;
                            DataTable temp = dv.ToTable();
                            if (temp.Rows.Count > 0)
                            {
                                foreach (DataRow dr1 in temp.Rows)
                                {
                                    sqlcolum = ""; updatemonthyear = "";
                                    string attclgcode = Convert.ToString(dr1["Att_CollegeCode"]);
                                    regclgcode = Convert.ToString(dr1["college_code"]);
                                    string monthandyear = Convert.ToString(dr1["month_year"]);
                                    if (attclgcode == regclgcode)
                                    {
                                        if (ds.Tables[1].Rows.Count > 0)
                                        {
                                            DataTable filterrowdt = new DataTable();
                                            filterrowdt = temp;
                                            foreach (DataRow colname in ds.Tables[1].Rows)
                                            {
                                                string col = Convert.ToString(colname["column_name"]);
                                                filterrowdt.DefaultView.RowFilter = col + " is not null and Att_CollegeCode=college_code";
                                                dv1 = filterrowdt.DefaultView;
                                                if (dv1.Count > 0)
                                                {
                                                    sqlcolum += "," + col + "='" + Convert.ToString(dv1[0][col]).Trim() + "'";
                                                    updatemonthyear = "," + monthandyear;
                                                }
                                            }
                                            if (sqlcolum.Trim() != "")
                                            {
                                                q1 = " update attendance set " + sqlcolum.TrimStart(',') + " where roll_no='" + rollmonyear[0].ToString() + "' and month_year in(" + updatemonthyear.TrimStart(',') + ") and Att_CollegeCode='" + regclgcode + "'";
                                                rowaffected += d2.update_method_wo_parameter(q1, "text");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        q1 = "";
                    }
                }
            }
            lbl_error.Text = Convert.ToString("No of Rows Affected (" + rowaffected + ")");
            lbl_error.ForeColor = System.Drawing.Color.Green;
        }
        catch (Exception ex)
        {
            lbl_error.Text = Convert.ToString(ex);
            lbl_error.ForeColor = System.Drawing.Color.Red;
        }
    }
    //protected void btn_remove_click(object sender, EventArgs e)
    //{
    //    q1 = "delete a from attendance a,Registration r where a.roll_no=r.Roll_No and r.Batch_Year in (2013,2014,2015,2016)  and a.Att_CollegeCode!=r.college_code";
    //    int del = d2.update_method_wo_parameter(q1, "text");

    //    lbl_error.Text = Convert.ToString("No of Rows Affected (" + del + ")");
    //    lbl_error.ForeColor = System.Drawing.Color.Green;



    //    /*DELETE LU FROM   (SELECT  roll_no, month_year,Row_number() OVER ( partition BY roll_no, month_year ORDER BY roll_no DESC) [Row] FROM   attendance) LU WHERE  [row] > 1 */
    //}
    public string GetSelectedItemsValueAsString(DataSet dummy, string collname)
    {
        System.Text.StringBuilder sbSelected = new System.Text.StringBuilder();
        try
        {
            foreach (DataRow dr in dummy.Tables[0].Rows)
            {
                if (sbSelected.Length == 0)
                {
                    sbSelected.Append(Convert.ToString(dr[collname]));
                }
                else
                {
                    sbSelected.Append("','" + Convert.ToString(dr[collname]));
                }
            }
        }
        catch { sbSelected.Clear(); }
        return sbSelected.ToString();
    }
    public string GetSelectedItemsValueAsStringcomma(DataSet dummy, string colname)
    {
        System.Text.StringBuilder sbSelected = new System.Text.StringBuilder();
        try
        {
            foreach (DataRow dr in dummy.Tables[0].Rows)
            {
                if (sbSelected.Length == 0)
                {
                    sbSelected.Append(Convert.ToString(dr[colname]));
                }
                else
                {
                    sbSelected.Append("," + Convert.ToString(dr[colname]));
                }
            }
        }
        catch { sbSelected.Clear(); }
        return sbSelected.ToString();
    }

    protected void Button1_click(object sender, EventArgs e)
    {
        try
        {
            lbl_error.Visible = false;
            int rowaffected = 0;
            string SlectAll = "select * from holidayStudents where  degree_code in(select  distinct degree_code  from Registration where cc=0 and delflag<>1 and exam_flag<>'debar' ) and semester in(select  distinct Current_Semester  from Registration where cc=0 and delflag<>1 and exam_flag<>'debar' )";
            DataTable dtAll = dir.selectDataTable(SlectAll);
            string SelectQ = "select  COUNT(h.holiday_date) as date,h.semester,h.degree_code,h.holiday_date from holidayStudents h where  degree_code in(select  distinct degree_code  from Registration where cc=0 and delflag<>1 and exam_flag<>'debar' ) and semester in(select  distinct Current_Semester  from Registration where cc=0 and delflag<>1 and exam_flag<>'debar' ) group by h.semester,h.degree_code,holiday_date  having count(h.holiday_date) > 1";
            DataTable dtselect = dir.selectDataTable(SelectQ);
            if (dtselect.Rows.Count > 0 && dtAll.Rows.Count > 0)
            {

                foreach (DataRow dr in dtselect.Rows)
                {
                    string DegeCode = Convert.ToString(dr["degree_code"]);
                    string sem = Convert.ToString(dr["semester"]);
                    string date = Convert.ToString(dr["semester"]);
                    dtAll.DefaultView.RowFilter = "degree_code='" + DegeCode + "' and semester='" + sem + "' and holiday_date='" + date + "'";
                    DataTable dvCount = dtAll.DefaultView.ToTable();
                    if (dvCount.Rows.Count > 0 && dvCount.Rows.Count > 1)
                    {
                        string dicId = string.Empty;
                        bool isVal = false;
                        foreach (DataRow dr1 in dvCount.Rows)
                        {
                            string id = Convert.ToString(dr1["id"]);
                            if (isVal)
                            {
                                isVal = true;
                                if (string.IsNullOrEmpty(dicId))
                                    dicId = id;
                                else
                                    dicId = dicId + "," + id;
                            }
                        }
                        int del = d2.update_method_wo_parameter("delete from holidayStudents where ID in(" + dicId + ")", "text");
                        if (del > 0)
                            rowaffected = rowaffected + del;
                    }
                }
            }
            lbl_error.Visible = true;
            lbl_error.Text = Convert.ToString("No of Rows Affected (" + rowaffected + ")");
            lbl_error.ForeColor = System.Drawing.Color.Green;
        }
        catch
        {
        }
    }
    protected void Button2_click(object sender, EventArgs e)
    {
        try
        {
            int i = 0;
            string SyllCode = "select distinct Criteria_no ,syll_code  from CriteriaForInternal order by syll_code,Criteria_no";
            DataTable dtsyllCode = dir.selectDataTable(SyllCode);
            {
                foreach (DataRow dr in dtsyllCode.Rows)
                {
                    string syllCode = Convert.ToString(dr["syll_code"]);
                    string CriNo = Convert.ToString(dr["Criteria_no"]);
                    if (!string.IsNullOrEmpty(syllCode) && !string.IsNullOrEmpty(CriNo))
                    {
                        DataTable dtExamtype = dir.selectDataTable("select exam_code from exam_type where Criteria_no= '" + CriNo + "' and subject_no not in(select subject_no from subject where syll_code='" + syllCode + "')");
                        if (dtExamtype.Rows.Count > 0)
                        {
                            foreach (DataRow dr1 in dtExamtype.Rows)
                            {
                                string examtype = Convert.ToString(dr1["exam_code"]);
                                int delMark = d2.update_method_wo_parameter("delete  from Result where exam_code='" + examtype + "'", "text");
                                int delExam = d2.update_method_wo_parameter("delete  from Exam_type where exam_code='" + examtype + "'", "text");
                                i++;
                            }

                        }
                    }
                }
            }
            lbl_error.Visible = true;
            lbl_error.Text = Convert.ToString("Deleted" + i);
            lbl_error.ForeColor = System.Drawing.Color.Green;
        }
        catch
        {

        }
    }
    protected void Button4_click(object sender, EventArgs e)
    {
        try
        {
            string stuSubjectInfo = "select r.roll_no,s.syll_code,s.subject_code,s.subject_no,s.subtype_no,current_semester from registration  r,subject s,syllabus_master sy where s.syll_code=sy.syll_code and sy.batch_year=r.batch_year and sy.degree_code=r.degree_code and sy.semester=r.current_semester and r.batch_year in(2016,2018,2017,2015)  order by r.roll_no ";/// -- and roll_no='16MECBE083'  
            DataTable dtstuSubjectInfo = dir.selectDataTable(stuSubjectInfo);

            if (dtstuSubjectInfo.Rows.Count > 0)
            {
                string syllCode = string.Empty;
                DataTable dicRollNo = dtstuSubjectInfo.DefaultView.ToTable(true, "roll_no", "current_semester");

                foreach (DataRow dr1 in dicRollNo.Rows)
                {
                    string roll_no = Convert.ToString(dr1["roll_no"]);
                    string sem = Convert.ToString(dr1["current_semester"]);
                    string subChooser = "select sc.subject_no,roll_no,subject_code,syll_code from SubjectChooser sc,subject s where s.subject_no=sc.subject_no and roll_no='" + roll_no + "' and semester='" + sem + "'  order by subject_code";
                    DataTable dtSubjectChooser = dir.selectDataTable(subChooser);
                    if (dtSubjectChooser.Rows.Count > 0)
                    {
                        foreach (DataRow dr2 in dtSubjectChooser.Rows)
                        {
                            string subCode = Convert.ToString(dr2["subject_code"]);
                            string subNo = Convert.ToString(dr2["subject_no"]);
                            dtstuSubjectInfo.DefaultView.RowFilter = "roll_no='" + roll_no + "' and subject_no='" + subNo + "'";
                            DataView dvcount = dtstuSubjectInfo.DefaultView;
                            if (dvcount.Count > 0)
                            {
                                string dupRow = "select COUNT(subject_no) as rollnocount,roll_no from SubjectChooser where roll_no='" + roll_no + "' and subject_no='" + subNo + "'  group by roll_no having count(subject_no) > 1";
                                DataTable dtDupCout = dir.selectDataTable(dupRow);
                                if (dtDupCout.Rows.Count > 1)
                                {
                                    string del = "delete t1 from SubjectChooser t1, SubjectChooser t2 where  t1.id>t2.id  and t1.roll_no=t2.roll_no and t1.subject_no=t2.subject_no and t1.roll_no='" + roll_no + "'   and t1.subject_no='" + subNo + "'";
                                    int delcount = d2.update_method_wo_parameter(del, "Text");
                                }
                            }
                            else
                            {
                                dtstuSubjectInfo.DefaultView.RowFilter = "roll_no='" + roll_no + "' and subject_code='" + subCode + "'";
                                DataView dvcount1 = dtstuSubjectInfo.DefaultView;
                                if (dvcount1.Count > 0)
                                {
                                    string subject = Convert.ToString(dvcount1[0]["subject_no"]);
                                    string subtype_no = Convert.ToString(dvcount1[0]["subtype_no"]);
                                    if (subject != subNo)
                                    {
                                        string upd = "update SubjectChooser SET subject_no='" + subject + "',subtype_no='" + subtype_no + "' where roll_no='" + roll_no + "' and subject_no='" + subNo + "' and semester='" + sem + "'";
                                        int delcount = d2.update_method_wo_parameter(upd, "Text");
                                    }

                                    string dupRow = "select COUNT(subject_no) as rollnocount,roll_no from SubjectChooser where roll_no='" + roll_no + "' and subject_no='" + subject + "'  group by roll_no having count(subject_no) > 1";
                                    DataTable dtDupCout = dir.selectDataTable(dupRow);
                                    if (dtDupCout.Rows.Count > 1)
                                    {
                                        string del = "delete t1 from SubjectChooser t1, SubjectChooser t2 where  t1.id>t2.id and t1.roll_no=t2.roll_no and t1.subject_no=t2.subject_no and t1.roll_no='" + roll_no + "'   and t1.subject_no='" + subject + "'";
                                        int delcount = d2.update_method_wo_parameter(del, "Text");
                                    }
                                }
                                else
                                {
                                    string del = "delete SubjectChooser where roll_no='" + roll_no + "' and subject_no='" + subNo + "' and semester='" + sem + "'";
                                    int delcount = d2.update_method_wo_parameter(del, "Text");
                                }
                            }

                        }
                    }
                }
            }
            else
            {
                lbl_error.Visible = true;
                lbl_error.Text = "No Student Info found";
                lbl_error.ForeColor = System.Drawing.Color.Green;
            }
            lbl_error.Visible = true;
            lbl_error.Text = "Completed.!";
            lbl_error.ForeColor = System.Drawing.Color.Green;

        }
        catch (Exception ex)
        {
            lbl_error.Visible = true;
            lbl_error.Text = ex.ToString();
            lbl_error.ForeColor = System.Drawing.Color.Green;
        }

    }
    protected void Button3_click(object sender, EventArgs e)
    {

        if (!string.IsNullOrEmpty(txtDate.Text) && !string.IsNullOrEmpty(txttoDate.Text))
        {
            string selectedDate = txtDate.Text.ToString();
            string[] splitSelectedDate = selectedDate.Split(new Char[] { '/' });
            selectedDate = splitSelectedDate[1].ToString() + "/" + splitSelectedDate[0].ToString() + "/" + splitSelectedDate[2].ToString();
            DateTime dtSelectedDate = Convert.ToDateTime(selectedDate.ToString());

            string selectedToDate = txttoDate.Text.ToString();
            string[] splitSelectedToDate = selectedToDate.Split(new Char[] { '/' });
            selectedDate = splitSelectedToDate[1].ToString() + "/" + splitSelectedToDate[0].ToString() + "/" + splitSelectedToDate[2].ToString();
            DateTime dtSelectedToDate = Convert.ToDateTime(selectedDate.ToString());

            if (dtSelectedDate <= dtSelectedToDate)
            {
                int insert = 0;
                string nohrs = d2.GetFunction("select MAX(No_of_hrs_per_day) from PeriodAttndSchedule");
                int hrs = 0;
                int.TryParse(nohrs, out hrs);
                string sel_date = string.Empty;
                Hashtable hat = new Hashtable();
                string stuCon = " select  convert(varchar(15),dateadd(day,tot_days-1,ack_date),1) as action_days,isnull(feeonrolldate,'') as feeonrolldate,ack_date,tot_days,s.roll_no,r.App_No,r.college_code from stucon s,Registration r where s.roll_no=r.Roll_No and   curr_date>='" + dtSelectedDate.ToString("MM/dd/yyyy") + "' and curr_date<='" + dtSelectedToDate.ToString("MM/dd/yyyy") + "'";
                DataTable dtStuCon = dir.selectDataTable(stuCon);
                if (dtStuCon.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtStuCon.Rows)
                    {
                        string rollNo = Convert.ToString(dr["roll_no"]);
                        string appNo = Convert.ToString(dr["App_No"]);
                        string collCode = Convert.ToString(dr["college_code"]);
                        DateTime dt_curr = Convert.ToDateTime(Convert.ToString(dr["ack_date"]));
                        DateTime dt_act = Convert.ToDateTime(Convert.ToString(dr["action_days"]));
                        TimeSpan t_con = dt_act.Subtract(dt_curr);
                        long daycon = t_con.Days;
                        DateTime dt_curr1 = Convert.ToDateTime(Convert.ToString(dr["ack_date"]));
                        DateTime dt_act1 = Convert.ToDateTime(Convert.ToString(dr["ack_date"]));
                        DateTime dtFeeOnRoll = Convert.ToDateTime(Convert.ToString(dr["feeonrolldate"]));
                        TimeSpan t_con1 = dt_act1.Subtract(dt_curr1);
                        string stuFeeRoll = dtFeeOnRoll.ToString("dd/MM/yyyy");
                        if (stuFeeRoll == "01/01/1900")
                            dtFeeOnRoll = dtSelectedToDate;
                        long daycon1 = t_con1.Days;
                        long totalactdays = Convert.ToInt32(Convert.ToInt32(dr["tot_days"]));
                        //if ((Convert.ToInt32(daycon + daycon1) == totalactdays - 1) && totalactdays > 0)// && (daycon >= 0)
                        {
                            while (dt_curr1 < dtFeeOnRoll)
                            {
                                string curdate = dt_curr1.ToString("MM-dd-yyyy");
                                string[] split = curdate.Split(new Char[] { '-' });
                                string str_day = (Convert.ToInt16(split[1].ToString())).ToString();

                                string Atmonth = (Convert.ToInt16(split[0].ToString())).ToString();
                                string Atyear = split[2].ToString();
                                int strdate = (Convert.ToInt32(Atmonth) + Convert.ToInt32(Atyear) * 12);
                                string dcolumn = string.Empty;
                                string val = string.Empty;
                                string colVal = string.Empty;
                                for (int hr = 1; hr <= hrs; hr++)
                                {
                                    if (string.IsNullOrEmpty(dcolumn))
                                        dcolumn = "d" + str_day + "d" + hr.ToString();
                                    else
                                        dcolumn =dcolumn+","+ "d" + str_day + "d" + hr.ToString();
                                    if (string.IsNullOrEmpty(val))
                                        val = "2";
                                    else
                                        val = val + "," + "2";
                                    if (string.IsNullOrEmpty(colVal))
                                        colVal = "d" + str_day + "d" + hr.ToString()+"="+"2";
                                    else
                                        colVal = colVal + "," + "d" + str_day + "d" + hr.ToString() + "=" + "2"; 
                                    //hat.Clear();
                                    //hat.Add("Att_App_no", appNo);
                                    //hat.Add("Att_CollegeCode", collCode);
                                    //hat.Add("columnname", dcolumn);
                                    //hat.Add("roll_no", rollNo);
                                    //hat.Add("month_year", strdate);
                                    //hat.Add("values", "2");
                                    //string strquery = "sp_ins_upd_student_attendance";
                                    //insert = d2.insert_method(strquery, hat, "sp");
                                }
                                hat.Clear();
                                hat.Add("Att_App_no", appNo);
                                hat.Add("Att_CollegeCode", collCode);
                                hat.Add("rollno", rollNo);
                                hat.Add("monthyear", strdate);
                                hat.Add("columnname", dcolumn);//col
                                hat.Add("colvalues", val);//val
                                hat.Add("coulmnvalue", colVal);//colval
                                insert = d2.insert_method("sp_ins_upd_student_attendance_Dead", hat, "sp");
                                
                                dt_curr1 = dt_curr1.AddDays(1);
                            }
                        }
                    }
                }
                else
                {
                    lbl_error.Visible = true;
                    lbl_error.Text = "No record found";
                    lbl_error.ForeColor = System.Drawing.Color.Green;
                }
                if (insert > 0)
                {
                    lbl_error.Visible = true;
                    lbl_error.Text = "Update Successfully";
                    lbl_error.ForeColor = System.Drawing.Color.Green;
                }
            }
            else
            {
                lbl_error.Visible = true;
                lbl_error.Text = "Pls Select Correct Date";
                lbl_error.ForeColor = System.Drawing.Color.Green;
            }
        }

    }
}
