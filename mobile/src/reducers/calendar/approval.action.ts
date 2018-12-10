/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import { Action } from 'redux';
import { CalendarEventId } from './calendar-event.model';
import { Approval } from './approval.model';
import { Map } from 'immutable';

//============================================================================
export enum ApprovalActionType {
    loadApprovals = 'ApprovalActionType.loadApprovals',
    loadApprovalsFinished = 'ApprovalActionType.loadApprovalsFinished',
    approve = 'ApprovalActionType.approve',
    approveFinished = 'ApprovalActionType.approveFinished',
}


//----------------------------------------------------------------------------
// - Actions
//----------------------------------------------------------------------------

export interface LoadApprovals extends Action<ApprovalActionType.loadApprovals> {
    employeeId: string;
    eventIds: string[];
}

export interface LoadApprovalsFinished extends Action<ApprovalActionType.loadApprovalsFinished> {
    employeeId: string;
    approvals: Map<CalendarEventId, Approval[]>;
}

export interface Approve extends Action<ApprovalActionType.approve> {
    employeeId: string;
    eventId: string;
}

export interface ApproveFinished extends Action<ApprovalActionType.approveFinished> {
    approval: Approval;
}

export type ApprovalAction = LoadApprovals | LoadApprovalsFinished | Approve | ApproveFinished;

//----------------------------------------------------------------------------
// - Action creators
//----------------------------------------------------------------------------

export const loadApprovals = (employeeId: string, eventIds: string[]): LoadApprovals => {
    return {
        type: ApprovalActionType.loadApprovals,
        employeeId,
        eventIds,
    };
};

export const loadApprovalsFinished = (employeeId: string, approvals: Map<CalendarEventId, Approval[]>): LoadApprovalsFinished => {
    return {
        type: ApprovalActionType.loadApprovalsFinished,
        employeeId,
        approvals,
    };
};

export const approve = (employeeId: string, eventId: string): Approve => {
    return {
        type: ApprovalActionType.approve,
        employeeId,
        eventId,
    };
};

export const approveFinished = (approval: Approval): ApproveFinished => {
    return {
        type: ApprovalActionType.approveFinished,
        approval,
    };
};
