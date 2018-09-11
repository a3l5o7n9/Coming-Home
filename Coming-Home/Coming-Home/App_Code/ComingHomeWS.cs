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
        int userId = BLService.Register(userName, userPassword, firstName, lastName);

        return js.Serialize(userId);
    }

    [WebMethod]
    public string Login(string userName, string userPassword)
    {
        JsonData jd = BLService.Login(userName, userPassword);

        if (jd.AU == null)
        {
            jd = new JsonData("No Data");
        }

        return js.Serialize(jd);
    }

    [WebMethod]
    public string CreateHome(string userId, string homeName, string address)
    {
        int homeId = BLService.CreateHome(int.Parse(userId), homeName, address);

        return js.Serialize(homeId);
    }

    [WebMethod]
    public string JoinHome(string userId, string homeName, string address)
    {
        int homeId = BLService.JoinHome(int.Parse(userId), homeName, address);

        return js.Serialize(homeId);
    }

    [WebMethod]
    public string CreateRoom(string roomName, string homeId, string roomTypeName, string isShared, string userId)
    {
        int roomId = BLService.CreateRoom(roomName, int.Parse(homeId), roomTypeName, bool.Parse(isShared), int.Parse(userId));

        return js.Serialize(roomId);
    }

    [WebMethod]
    public string CreateDevice(string deviceName, string homeId, string deviceTypeName, string isDividedIntoRooms, string userId, string roomId)
    {
        int deviceId = BLService.CreateDevice(deviceName, int.Parse(homeId), deviceTypeName, bool.Parse(isDividedIntoRooms), int.Parse(userId), int.Parse(roomId));

        return js.Serialize(deviceId);
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

        return js.Serialize(res);
    }

    [WebMethod]
    public string UpdateUserTypeInHome(string appUserId, string userToUpdateId, string homeId, string appUserTypeName, string currentUserTypeName, string updatedUserTypeName)
    {
        int res = BLService.UpdateUserTypeInHome(int.Parse(appUserId), int.Parse(userToUpdateId), int.Parse(homeId), appUserTypeName, currentUserTypeName, updatedUserTypeName);

        return js.Serialize(res);
    }

    [WebMethod]
    public string UpdateUserDevicePermissions(string appUserId, string userToUpdateId, string homeId, string appUserTypeName, string userToUpdateTypeName, string deviceId, string roomId, string hasPermission)
    {
        int res = BLService.UpdateUserDevicePermissions(int.Parse(appUserId), int.Parse(userToUpdateId), int.Parse(homeId), appUserTypeName, userToUpdateTypeName, int.Parse(deviceId), int.Parse(roomId), bool.Parse(hasPermission));

        return js.Serialize(res);
    }

    [WebMethod]
    public string UpdateUserRoomPermissions(string appUserId, string userToUpdateId, string homeId, string appUserTypeName, string userToUpdateTypeName, string roomId, string hasAccess)
    {
        int res = BLService.UpdateUserRoomPermissions(int.Parse(appUserId), int.Parse(userToUpdateId), int.Parse(homeId), appUserTypeName, userToUpdateTypeName, int.Parse(roomId), bool.Parse(hasAccess));

        return js.Serialize(res);
    }

    [WebMethod]
    public string GetUser(string userId, string homeId)
    {
        JsonData jd = BLService.GetUser(int.Parse(userId), int.Parse(homeId));

        return js.Serialize(jd);
    }

    [WebMethod]
    public string GetHome(string userId, string homeId)
    {
        JsonData jd = BLService.GetHome(int.Parse(userId), int.Parse(homeId));

        return js.Serialize(jd);
    }

    [WebMethod]
    public string GetDevice(string userId, string homeId, string deviceId, string roomId)
    {
        JsonData jd = BLService.GetDevice(int.Parse(userId), int.Parse(homeId), int.Parse(deviceId), int.Parse(roomId));

        return js.Serialize(jd);
    }

    [WebMethod]
    public string GetRoom(string userId, string homeId, string roomId)
    {
        JsonData jd = BLService.GetRoom(int.Parse(userId), int.Parse(homeId), int.Parse(roomId));

        return js.Serialize(jd);
    }

    [WebMethod]
    public string GetUsersInHome(string userId, string homeId)
    {
        JsonData jd = BLService.GetUsersInHome(int.Parse(userId), int.Parse(homeId));

        return js.Serialize(jd);
    }

    [WebMethod]
    public string GetUserRoomsInHome(string userId, string homeId)
    {
        JsonData jd = BLService.GetUserRoomsInHome(int.Parse(userId), int.Parse(homeId));

        return js.Serialize(jd);
    }

    [WebMethod]
    public string GetUserDevicesInHome(string userId, string homeId)
    {
        JsonData jd = BLService.GetUserDevicesInHome(int.Parse(userId), int.Parse(homeId));

        return js.Serialize(jd);
    }
}
