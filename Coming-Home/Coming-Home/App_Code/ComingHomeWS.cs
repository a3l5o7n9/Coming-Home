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
    public string CreateHome(string userId, string homeName, string address, string latitude, string longitude, string altitude, string accuracy)
    {
        int homeId = BLService.CreateHome(int.Parse(userId), homeName, address, double.Parse(latitude), double.Parse(longitude), double.Parse(altitude), double.Parse(accuracy));

        return js.Serialize(homeId);
    }

    [WebMethod]
    public string InviteUserToHome(string userName, string homeName, string address, string latitude, string longitude, string altitude, string accuracy)
    {
        int res = BLService.InviteUserToHome(userName, homeName, address, double.Parse(latitude), double.Parse(longitude), double.Parse(altitude), double.Parse(accuracy));

        return js.Serialize(res);
    }

    [WebMethod]
    public string JoinHome(string userId, string homeName, string address, string latitude, string longitude, string altitude, string accuracy)
    {
        JsonData jd = BLService.JoinHome(int.Parse(userId), homeName, address, double.Parse(latitude), double.Parse(longitude), double.Parse(altitude), double.Parse(accuracy));

        return js.Serialize(jd);
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
    public string CreateActivationCondition(string conditionName, string turnOn, string userId, string homeId, string deviceId, string roomId, string activationMethodName, string distanceOrTimeParam, string activationParam)
    {
        int res = -1;

        res = BLService.CreateActivationCondition(conditionName, bool.Parse(turnOn), int.Parse(userId), int.Parse(homeId), int.Parse(deviceId), int.Parse(roomId), activationMethodName, distanceOrTimeParam, activationParam);

        return js.Serialize(res);
    }

    [WebMethod]
    public int UpdateTokenForUserId(string token, string userId)
    {
        return BLService.UpdateTokenForUserId(token, int.Parse(userId));
    }

    [WebMethod]
    public string SendPushNotification(string token, string userId, string userName, string homeId, string homeName, string roomId, string roomName, string deviceId, string deviceName, string conditionId, string conditionName, string turnOn)
    {
        string newStatus = bool.Parse(turnOn) ? "On" : "Off"; 
        // Create a request using a URL that can receive a post.   
        WebRequest request = WebRequest.Create("https://exp.host/--/api/v2/push/send");
        // Set the Method property of the request to POST.  
        request.Method = "POST";
        // Create POST data and convert it to a byte array.  
        var objectToSend = new
        {
            to = token,
            title = "Coming Home",
            body = "Condition " + conditionName + " has been activated: " + deviceName + " in " + roomName + " has been turned " + newStatus + ".",
            badge = 1,
            data = new { userId, homeId, roomId, deviceId, conditionId }
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
    public void CheckActivationConditions()
    {
        List<ActivationCondition> lActCon = BLService.GetAllActivationConditions();

        foreach (ActivationCondition actCon in lActCon)
        {
            int res = BLService.CheckActivationCondition(actCon);

            if (res == 1)
            {
                JsonData jd = BLService.GetActivationConditionDetails(actCon);
                User u = jd.U;
                Home h = jd.H;
                Room r = jd.R;
                Device d = jd.D;

                if (u.Token != null)
                {
                    SendPushNotification(u.Token, u.UserId.ToString(), u.UserName, h.HomeId.ToString(), h.HomeName, r.RoomId.ToString(), r.RoomName, d.DeviceId.ToString(), d.DeviceName, actCon.ConditionId.ToString(), actCon.ConditionName, actCon.TurnOn.ToString());
                }
            }
        }
    }

    [WebMethod]
    public string BindDeviceToRoom(string roomId, string deviceId, string userId)
    {
        int res = BLService.BindDeviceToRoom(int.Parse(roomId), int.Parse(deviceId), int.Parse(userId));

        return js.Serialize(res);
    }

    [WebMethod]
    public string UpdateUserDetails(string appUserId, string userToUpdateId, string newUserName, string newUserPassword, string newFirstName, string newLastName)
    {
        string res = BLService.UpdateUserDetails(int.Parse(appUserId), int.Parse(userToUpdateId), newUserName, newUserPassword, newFirstName, newLastName);

        return js.Serialize(res);
    }

    [WebMethod]
    public string UpdateHomeDetails(string appUserId, string homeId, string newHomeName, string newAddress, string newLatitude, string newLongitude, string newAltitude, string newAccuracy)
    {
        string res = BLService.UpdateHomeDetails(int.Parse(appUserId), int.Parse(homeId), newHomeName, newAddress, newLatitude, newLongitude, newAltitude, newAccuracy);

        return js.Serialize(res);
    }

    [WebMethod]
    public string UpdateRoomDetails(string appUserId, string homeId, string roomId, string newRoomName, string newRoomTypeCode, string newShareStatus)
    {
        string res = BLService.UpdateRoomDetails(int.Parse(appUserId), int.Parse(homeId), int.Parse(roomId), newRoomName, newRoomTypeCode, newShareStatus);

        return js.Serialize(res);
    }

    [WebMethod]
    public string UpdateDeviceDetails(string appUserId, string homeId, string deviceId, string newDeviceName, string newDeviceTypeCode, string newDivideStatus)
    {
        string res = BLService.UpdateDeviceDetails(int.Parse(appUserId), int.Parse(homeId), int.Parse(deviceId), newDeviceName, newDeviceTypeCode, newDivideStatus);

        return js.Serialize(res);
    }

    [WebMethod]
    public string UpdateActivationConditionDetails(string appUserId, string homeId, string conditionId, string newDeviceId, string newRoomId, string newConditionName, string newStatus, string newActivationMethodCode, string newDistanceOrTimeParam, string newActivationParam)
    {
        string res = BLService.UpdateActivationConditionDetails(int.Parse(appUserId), int.Parse(homeId), int.Parse(conditionId), newDeviceId, newRoomId, newConditionName, newStatus, newActivationMethodCode, newDistanceOrTimeParam, newActivationParam);

        return js.Serialize(res);
    }

    [WebMethod]
    public string UpdateUserTypeInHome(string appUserId, string userToUpdateId, string homeId, string updatedUserTypeName)
    {
        int res = BLService.UpdateUserTypeInHome(int.Parse(appUserId), int.Parse(userToUpdateId), int.Parse(homeId), updatedUserTypeName);

        return js.Serialize(res);
    }

    [WebMethod]
    public string UpdateUserDevicePermissions(string appUserId, string userToUpdateId, string homeId, string deviceId, string roomId, string hasPermission)
    {
        int res = BLService.UpdateUserDevicePermissions(int.Parse(appUserId), int.Parse(userToUpdateId), int.Parse(homeId), int.Parse(deviceId), int.Parse(roomId), bool.Parse(hasPermission));

        return js.Serialize(res);
    }

    [WebMethod]
    public string UpdateUserRoomPermissions(string appUserId, string userToUpdateId, string homeId, string roomId, string hasAccess)
    {
        int res = BLService.UpdateUserRoomPermissions(int.Parse(appUserId), int.Parse(userToUpdateId), int.Parse(homeId), int.Parse(roomId), bool.Parse(hasAccess));

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
    public string GetActivationCondition(string conditionId, string userId, string homeId, string deviceId, string roomId)
    {
        JsonData jd = BLService.GetActivationCondition(int.Parse(conditionId), int.Parse(userId), int.Parse(homeId), int.Parse(deviceId), int.Parse(roomId));

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

    [WebMethod]
    public string GetUserActivationConditionsInHome(string userId, string homeId)
    {
        JsonData jd = BLService.GetUserActivationConditionsInHome(int.Parse(userId), int.Parse(homeId));

        return js.Serialize(jd);
    }

    [WebMethod]
    public string GetAllUserRooms(string userId)
    {
        List<Room> lr = BLService.GetAllUserRooms(int.Parse(userId));

        return js.Serialize(lr);
    }

    [WebMethod]
    public string GetAllUserDevices(string userId)
    {
        List<Device> ld = BLService.GetAllUserDevices(int.Parse(userId));

        return js.Serialize(ld);
    }

    [WebMethod]
    public string GetAllUserActivationConditions(string userId)
    {
        List<ActivationCondition> lActCon = BLService.GetAllUserActivationConditions(int.Parse(userId));

        return js.Serialize(lActCon);
    }

    [WebMethod]
    public string GetUserHomeDetails(string userId, string homeId)
    {
        JsonData jd = BLService.GetUserHomeDetails(int.Parse(userId), int.Parse(homeId));

        return js.Serialize(jd);
    }

    [WebMethod]
    public string ChangeDeviceStatus(string userId, string deviceId, string roomId, string turnOn, string activationMethodCode, string statusDetails, string conditionId)
    {
        int res = -1;

        res = BLService.ChangeDeviceStatus(int.Parse(userId), int.Parse(deviceId), int.Parse(roomId), bool.Parse(turnOn), int.Parse(activationMethodCode), statusDetails, conditionId);

        return js.Serialize(res);
    }

    [WebMethod]
    public string ChangeConditionStatus(string userId, string homeId, string deviceId, string roomId, string conditionId, string newStatus)
    {
        int res = -1;
        
        res = BLService.ChangeConditionStatus(int.Parse(userId), int.Parse(homeId), int.Parse(deviceId), int.Parse(roomId), int.Parse(conditionId), bool.Parse(newStatus));

        return js.Serialize(res);
    }

    [WebMethod]
    public string GetUserTypes()
    {
        return js.Serialize(BLService.GetUserTypes());
    }

    [WebMethod]
    public string GetRoomTypes()
    {
        return js.Serialize(BLService.GetRoomTypes());
    }

    [WebMethod]
    public string GetDeviceTypes()
    {
        return js.Serialize(BLService.GetDeviceTypes());
    }

    [WebMethod]
    public string GetActivationMethods()
    {
        return js.Serialize(BLService.GetActivationMethods());
    }

    [WebMethod]
    public string DeleteUser(string userId)
    {
        int res = BLService.DeleteUser(int.Parse(userId));

        return js.Serialize(res);
    }

    [WebMethod]
    public string DeleteHome(string userId, string homeId)
    {
        int res = BLService.DeleteHome(int.Parse(userId), int.Parse(homeId));

        return js.Serialize(res);
    }

    [WebMethod]
    public string DeleteRoom(string userId, string homeId, string roomId)
    {
        int res = BLService.DeleteRoom(int.Parse(userId), int.Parse(homeId), int.Parse(roomId));

        return js.Serialize(res);
    }

    [WebMethod]
    public string DeleteDevice(string userId, string homeId, string deviceId)
    {
        int res = BLService.DeleteDevice(int.Parse(userId), int.Parse(homeId), int.Parse(deviceId));

        return js.Serialize(res);
    }

    [WebMethod]
    public string DeleteActivationCondition(string userId, string homeId, string conditionId)
    {
        int res = BLService.DeleteActivationCondition(int.Parse(userId), int.Parse(homeId), int.Parse(conditionId));

        return js.Serialize(res);
    }

    [WebMethod]
    public string LeaveHome(string userId, string homeId)
    {
        int res = BLService.LeaveHome(int.Parse(userId), int.Parse(homeId));

        return js.Serialize(res);
    }

    [WebMethod]
    public string UnbindDeviceFromRoom(string roomId, string deviceId, string userId, string homeId)
    {
        int res = BLService.UnbindDeviceFromRoom(int.Parse(roomId), int.Parse(deviceId), int.Parse(userId), int.Parse(homeId));

        return js.Serialize(res);
    }
}
