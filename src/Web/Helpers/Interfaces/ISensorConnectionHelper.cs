using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Data.Models;

namespace Web.Helpers.Interfaces
{
    public interface ISensorConnectionHelper
    {
        Task ConnectAllSensorsAsync();

        void ConnectSensor(Sensor sensor);

        void DisconnectSensor(int id);

        void DisconnectAllSensors();

        bool IsConnected(int id);

        void TriggerChangeState(Sensor sensor);
    }
}
