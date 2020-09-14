﻿using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Web.Application.Readings.Notifications;
using Web.Areas.Admin.Helpers.Interfaces;

namespace Web.Areas.Admin.Application.Readings.Notifications
{
    public class PortableSensorReadingCreatedNotificationHandler : INotificationHandler<PortableReadingCreatedNotification>
    {
        
        private readonly IAdminDispatchHelper _adminDispatchHelper;
        

        public PortableSensorReadingCreatedNotificationHandler(IAdminDispatchHelper adminDispatchHelper)
        {
            _adminDispatchHelper = adminDispatchHelper ?? throw new ArgumentNullException(nameof(adminDispatchHelper));
        }
        public async Task Handle(PortableReadingCreatedNotification notification, CancellationToken cancellationToken)
        {
            await _adminDispatchHelper.DispatchReadingsForPortableSensorAsync(notification.SensorId, notification.Reading);
            await _adminDispatchHelper.DispatchCoordinatesForPortableSensorAsync(notification.SensorId, notification.Reading.Latitude, notification.Reading.Longitude);
        }
    }
}