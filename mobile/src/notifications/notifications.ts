/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

//============================================================================
export enum NotificationType {
    UpdateAvailable = 'UpdateAvailable',

    SickLeaveCreatedManager = 'SickLeaveCreatedManager',
    SickLeaveProlongedManager = 'SickLeaveProlongedManager',
    SickLeaveCancelledManager = 'SickLeaveCancelledManager',
    EventAssignedToApprover = 'EventAssignedToApprover',
    EventStatusChanged = 'EventStatusChanged',
    EventUserGrantedApproval = 'EventUserGrantedApproval',
}
