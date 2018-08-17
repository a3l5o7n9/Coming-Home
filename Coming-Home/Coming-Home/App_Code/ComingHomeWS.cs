using BEL;
using BAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Serialization;
using System.Text;
using System.IO;
using System.Net;

/// <summary>
/// Summary description for ComingHomeWS
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
[System.Web.Script.Services.ScriptService]
public class ComingHomeWS : System.Web.Services.WebService
{
    JavaScriptSerializer js = new JavaScriptSerializer();

    public ComingHomeWS()
    {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    public int Register(string userName, string userPassword, string firstName, string lastName)
    {
        return BLService.Register(userName, userPassword, firstName, lastName);
    }

    [WebMethod]
    public string Login(string userName, string userPassword)
    {
        JsonData jd = BLService.Login(userName, userPassword);

        return js.Serialize(jd);
    }

    [WebMethod]
    public string CreateHome(string userId, string homeName, string address)
    {
        return js.Serialize(BLService.CreateHome(int.Parse(userId), homeName, address));
    }

    [WebMethod]
    public string JoinHome(string userId, string homeName, string address)
    {
        JsonData jd = BLService.JoinHome(int.Parse(userId), homeName, address);

        if (jd == null)
        {
            return "Home Not Found";
        }

        return js.Serialize(jd);
    }

    [WebMethod]
    public int UpdateTokenForUserId(string token, string userId)
    {
        return BLService.UpdateTokenForUserId(token, int.Parse(userId));
    }

    [WebMethod]
    public string SendPushNotification()
    {
        // Create a request using a URL that can receive a post.   
        WebRequest request = WebRequest.Create("https://exp.host/--/api/v2/push/send");
        // Set the Method property of the request to POST.  
        request.Method = "POST";
        // Create POST data and convert it to a byte array.  
        var objectToSend = new
        {
            to = "ExponentPushToken[" + "]",
            title = "my title",
            body = "body from WSC#",
            badge = 3,
            data = new { name = "avi", grade = 100 }
        };

        string postData = new JavaScriptSerializer().Serialize(objectToSend);

        byte[] byteArray = Encoding.UTF8.GetBytes(postData);
        // Set the ContentType property of the WebRequest.  
        request.ContentType = "application/json";
        // Set the ContentLength property of the WebRequest.  
        request.ContentLength = byteArray.Length;
        // Get the request stream.  
        Stream dataStream = request.GetRequestStream();
        // Write the data to the request stream.  
        dataStream.Write(byteArray, 0, byteArray.Length);
        // Close the Stream object.  
        dataStream.Close();
        // Get the response.  
        WebResponse response = request.GetResponse();
        // Display the status.  
        string returnStatus = ((HttpWebResponse)response).StatusDescription;
        //Console.WriteLine(((HttpWebResponse)response).StatusDescription);
        // Get the stream containing content returned by the server.  
        dataStream = response.GetResponseStream();
        // Open the stream using a StreamReader for easy access.  
        StreamReader reader = new StreamReader(dataStream);
        // Read the content.  
        string responseFromServer = reader.ReadToEnd();
        // Display the content.  
        //Console.WriteLine(responseFromServer);
        // Clean up the streams.  
        reader.Close();
        dataStream.Close();
        response.Close();

        return "success:) --- " + responseFromServer + ", " + returnStatus;
    }

}
