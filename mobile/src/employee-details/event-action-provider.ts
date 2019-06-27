/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import {
    CalendarEvent,
    CalendarEventId,
    CalendarEventStatus,
    GeneralCalendarEventStatus
} from '../reducers/calendar/calendar-event.model';
import { Employee, EmployeeId } from '../reducers/organization/employee.model';
import { Permission, UserEmployeePermissions } from '../reducers/user/user-employee-permissions.model';
import { Map, Set } from 'immutable';
import { Approval } from '../reducers/calendar/approval.model';
import { Optional } from 'types';

//============================================================================
export enum EventActionType {
    approve = 'EventActionType.approve',
    reject = 'EventActionType.reject',
    cancel = 'EventActionType.cancel',
}

//============================================================================
export interface EventAction {
    readonly type: EventActionType;
    readonly handler: () => void;
}

//============================================================================
export interface EventActionContainer {
    readonly event: CalendarEvent;
    readonly employee: Employee;
    readonly positiveAction?: EventAction;
    readonly negativeAction?: EventAction;
}

//============================================================================
export class EventActionProvider {
    //----------------------------------------------------------------------------
    constructor(private readonly userId: EmployeeId,
                private readonly eventSetStatus: (employeeId: EmployeeId, calendarEvent: CalendarEvent, status: CalendarEventStatus) => void,
                private readonly eventApprove: (approverId: EmployeeId, employeeId: EmployeeId, calendarEvent: CalendarEvent) => void) {
    }

    //----------------------------------------------------------------------------
    public getEventActions(events: CalendarEvent[],
                           employee: Employee,
                           permissions: UserEmployeePermissions,
                           approvals: Map<CalendarEventId, Set<Approval>>): EventActionContainer[] {

        const eventContainers: EventActionContainer[] = [];

        for (const event of events) {
            eventContainers.push(this.evaluateEvent(event, employee, permissions, approvals));
        }

        return eventContainers;
    }

    //----------------------------------------------------------------------------
    public getRequestActions(events: CalendarEvent[],
                             employee: Employee,
                             approvals: Map<CalendarEventId, Set<Approval>>): EventActionContainer[] {

        const permissions = new UserEmployeePermissions();
        permissions.permissionsNames = Set([Permission.approveCalendarEvents, Permission.rejectCalendarEvents]);
        return this.getEventActions(events, employee, permissions, approvals);
    }

    //----------------------------------------------------------------------------
    private evaluateEvent(event: CalendarEvent,
                          employee: Employee,
                          permissions: UserEmployeePermissions,
                          approvals: Map<CalendarEventId, Set<Approval>>): EventActionContainer {

        const positiveAction = this.positiveAction(event, employee, permissions, approvals);
        const negativeAction = this.negativeAction(event, employee, permissions);

        return {
            event,
            employee,
            positiveAction,
            negativeAction,
        };
    }

    //----------------------------------------------------------------------------
    private positiveAction(event: CalendarEvent,
                           employee: Employee,
                           permissions: UserEmployeePermissions,
                           approvals: Map<CalendarEventId, Set<Approval>>): Optional<EventAction> {

        // Only the event creator should be able to cancel the sick leave. See:
        // https://github.com/SoftwareCountry/arcadian-assistant/issues/712
        const isOwnEvent = this.isCurrentUser(employee);
        if (event.isSickLeave && !isOwnEvent) {
            return undefined;
        }

        switch (event.status) {
            case GeneralCalendarEventStatus.Requested:
                const permissionApprove = permissions.has(Permission.approveCalendarEvents);
                const approvedByMe = this.approvedByMe(event, approvals);

                if (permissionApprove && !approvedByMe) {
                    return this.approveAction(event, employee);
                }
                break;

            default:
                break;
        }

        return undefined;
    }

    //----------------------------------------------------------------------------
    private negativeAction(event: CalendarEvent,
                           employee: Employee,
                           permissions: UserEmployeePermissions): Optional<EventAction> {

        // Only the event creator should be able to cancel the sick leave. See:
        // https://github.com/SoftwareCountry/arcadian-assistant/issues/712
        const isOwnEvent = this.isCurrentUser(employee);
        if (event.isSickLeave && !isOwnEvent) {
            return undefined;
        }

        switch (event.status) {
            case GeneralCalendarEventStatus.Requested:
                if (isOwnEvent) {
                    if (permissions.has(Permission.cancelPendingCalendarEvents)) {
                        return this.cancelAction(event, employee);
                    }
                } else {
                    if (permissions.has(Permission.rejectCalendarEvents)) {
                        return this.rejectAction(event, employee);
                    }
                }
                break;

            case GeneralCalendarEventStatus.Approved:
                if (permissions.has(Permission.cancelApprovedCalendarEvents)) {
                    return this.cancelAction(event, employee);
                }
                break;

            default:
                break;
        }

        return undefined;
    }

    //----------------------------------------------------------------------------
    private isCurrentUser(employee: Employee) {
        return this.userId === employee.employeeId;
    }

    //----------------------------------------------------------------------------
    private approveAction(event: CalendarEvent,
                          employee: Employee): EventAction {
        return {
            handler: () => this.eventApprove(this.userId, employee.employeeId, event),
            type: EventActionType.approve,
        };
    }

    //----------------------------------------------------------------------------
    private rejectAction(event: CalendarEvent,
                         employee: Employee): EventAction {
        return {
            handler: () => this.eventSetStatus(employee.employeeId, event, GeneralCalendarEventStatus.Rejected),
            type: EventActionType.reject,
        };
    }

    //----------------------------------------------------------------------------
    private cancelAction(event: CalendarEvent,
                         employee: Employee): EventAction {
        return {
            handler: () => this.eventSetStatus(employee.employeeId, event, GeneralCalendarEventStatus.Cancelled),
            type: EventActionType.cancel,
        };
    }

    //----------------------------------------------------------------------------
    private approvedByMe(event: CalendarEvent, approvals: Map<CalendarEventId, Set<Approval>>): boolean {
        const eventApprovals = approvals.get(event.calendarEventId);
        if (!eventApprovals) {
            return false;
        }

        return !!eventApprovals.find(approval => approval.approverId === this.userId);
    }

    //----------------------------------------------------------------------------
    public static compareEventActionContainers(left: EventActionContainer, right: EventActionContainer): number {
        const nameCompareResult = left.employee.name.localeCompare(right.employee.name);
        if (nameCompareResult !== 0) {
            return nameCompareResult;
        }

        return right.event.dates.startDate.valueOf() - left.event.dates.startDate.valueOf();
    }
}
