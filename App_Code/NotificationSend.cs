using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;
using System.Collections;

/// <summary>
/// Summary description for NotificationSend
/// </summary>
/// ///created by abarna 13.12.2018
public class NotificationSend
{
    DAccess2 d2 = new DAccess2();

    public NotificationSend()
    {
        //
        // TODO: Add constructor logic here
        //

    }
    public string SendMessage(string tok, string head, string body)
    {
        string serverKey = "AAAA4ix2emU:APA91bFYYMjJxFuGWDUvrXBEMGP17ef_tTNv2c795Db9CbfRMwhHc0R4GxUDGlShYr12GtMOTT0h5LrOPJAAOR2Wq9TGx76NmQSjX_VJDmbMUw96xmIdiD4kGqyK0_DoN7PmNxGpSkOy";

        try
        {
            var result = "-1";
            var webAddr = "https://fcm.googleapis.com/fcm/send";

            // var regID = "dVslEdedDCI:APA91bF_3gFCA6HpJ9UFD_bJUZZKw0QqTCsB3YoAbaYf-o2XEUkrJJ81Kir_PcdjsyfN8fB-yWVUpoWKEpOvzhSgJZ2I3DZb3a8DuMjTVWEnLlSXUUpN7SKXeas5Ikz0VJI2nbcBHT17";

            //for(int k=0;k<ar.)
            //{
            //            var regID = d2.GetFunction("select fcm_token from Registration where app_no='6463'");
            string reg = string.Empty;
            //for (int i = 0; i < ar.Count; i++)
            //{
            //    string a = Convert.ToString(ar[i]);
            //    reg += " \"" + a + "\",";
            //}
            //reg = reg.TrimEnd(',');



            //for (int i = 0; i < ar.Count; i++)
            //{
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddr);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Headers.Add("Authorization:key=" + serverKey);
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                // string json = "{\"to\": \"" + regID + "\",\"notification\": {\"title\": \"New deal\",\"body\": \"Welcome!\"},\"priority\":10}";

                //reg = Convert.ToString(ar[i]);
                //  string json = "{\"registration_ids\":[" + reg + "],\"notification\": {\"title\": \"" + head + "\",\"body\": \"" + body + "\"},\"priority\":10}";
                string json = "{\"to\": \"" + tok + "\",\"notification\": {\"title\": \"" + head + "\",\"body\": \"" + body + "\"},\"priority\":10}";
                //registration_ids, array of strings -  to, single recipient
                streamWriter.Write(json);
                streamWriter.Flush();


            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }
            //}
            //}

            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return "err";
        }
    }
}