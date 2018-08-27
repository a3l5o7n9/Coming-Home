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
        BLService.InitErrorLogPath(Server.MapPath("~"));
    }

    [WebMethod]
    public string Register(string userName, string userPassword, string firstName, string lastName)
    {
        string res = "";

        int userId = BLService.Register(userName, userPassword, firstName, lastName);

        if (userId == -2)
        {
            res = "Error! User could not be created";
            return res;
        }

        if (userId == -1)
        {
            res = "There is already a user with that UserName";
            return res;
        }

        return js.Serialize(userId);
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
        JsonData jd = BLService.CreateHome(int.Parse(userId), homeName, address);

        if (jd == null)
        {
            return "Error! Home could not be created";
        }

        if (jd.LH[0].HomeId == -1)
        {
            return "There is already a home with those details";
        }

        return js.Serialize(jd);
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
    public string CreateRoom(string roomName, string homeId, string roomTypeName)
    {
        JsonData jd = BLService.CreateRoom(roomName, int.Parse(homeId), roomTypeName);

        if (jd == null)
        {
            return "Error! Room could not be created";
        }

        if (jd.LR[0].RoomId == -1)
        {
            return "There is already a room with those details";
        }

        return js.Serialize(jd);
    }

    [WebMethod]
    public string CreateDevice(string deviceName, string homeId, string deviceTypeName, string userId, string roomId)
    {
        JsonData jd = BLService.CreateDevice(deviceName, int.Parse(homeId), deviceTypeName, int.Parse(userId), int.Parse(roomId));

        if (jd == null)
        {
            return "Device could not be created";
        }

        if (jd.LD[0].DeviceId == -1)
        {
            return "There is already a device with those details";
        }

        return js.Serialize(jd);
    }

    [WebMethod]
    public int UpdateTokenForUserId(string token, string userId)
    {
        return BLService.UpdateTokenForUserId(token, int.Parse(userId));
    }

    [WebMethod]
    public string SendPushNotification(/*JsonData jd*/)
    {
        //if (jd != null ) {

        //}
        // Create a request using a URL that can receive a post.   
        WebRequest request = WebRequest.Create("https://exp.host/--/api/v2/push/send");
        // Set the Method property of the request to POST.  
        request.Method = "POST";
        // Create POST data and convert it to a byte array.  
        var objectToSend = new
        {
            to = "ExponentPushToken[cUsRAzE11M_PB7v9MQIMhj]",
            title = "my title",
            body = "body from WSC#",
            badge = 1,
            data = new { name = "alon", grade = 80 }
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

    [WebMethod]
    public string BindDeviceToRoom(string roomId, string deviceId, string userId)
    {
        int res = BLService.BindDeviceToRoom(int.Parse(roomId), int.Parse(deviceId), int.Parse(userId));
        string resultMessage = "";

        if (res == 0)
        {
            resultMessage = "This device is already bound to this room";
        }

        if (res == -1)
        {
            resultMessage = "You do not have permission to bind this device to this room";
        }

        if (res > 0)
        {
            resultMessage = "Device successfully bound to room";
        }

        return resultMessage;
    }

}
