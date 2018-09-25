using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BEL
{
    public class ActivationCondition
    {
        public int ConditionId { get; set; }
        public string ConditionName { get; set; }
        public int CreatedByUserId { get; set; }
        public int HomeId { get; set; }
        public int DeviceId { get; set; }
        public int RoomId { get; set; }
        public string ActivationMethodName { get; set; }
        public bool IsActive { get; set; }
        public string DistanceOrTimeParam { get; set; }
        public string ActivationParam { get; set; }

        public ActivationCondition(int conditionId, string conditionName, int createdByUserId, int homeId, int deviceId, int roomId, string activationMethodName, bool isActive)
        {
            ConditionId = conditionId;
            ConditionName = conditionName;
            CreatedByUserId = createdByUserId;
            HomeId = homeId;
            DeviceId = deviceId;
            RoomId = roomId;
            ActivationMethodName = activationMethodName;
            IsActive = isActive;
        }

        public ActivationCondition(int conditionId, string conditionName, int createdByUserId, int homeId, int deviceId, int roomId, string activationMethodName, bool isActive, string distanceOrTimeParam) : this(conditionId, conditionName, createdByUserId, homeId, deviceId, roomId, activationMethodName, isActive)
        {
            DistanceOrTimeParam = distanceOrTimeParam;
        }

        public ActivationCondition(int conditionId, string conditionName, int createdByUserId, int homeId, int deviceId, int roomId, string activationMethodName, bool isActive, string distanceOrTimeParam, string activationParam) : this(conditionId, conditionName, createdByUserId, homeId, deviceId, roomId, activationMethodName, isActive, distanceOrTimeParam)
        {
            ActivationParam = activationParam;
        }
    }
}
