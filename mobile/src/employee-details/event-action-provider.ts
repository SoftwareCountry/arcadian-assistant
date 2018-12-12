/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import { CalendarEvent, CalendarEventId, CalendarEventStatus } from '../reducers/calendar/calendar-event.model';
import { Employee, EmployeeId } from '../reducers/organization/employee.model';
import { Permission, UserEmployeePermissions } from '../reducers/user/user-employee-permissions.model';
import { Map, Set } from 'immutable';
import { Approval } from '../reducers/calendar/approval.model';
import { Optional } from 'types';

//============================================================================
export interface EventAction {
    readonly handler: () => void;
    readonly name: string;
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
                private readonly eventApprove: (employeeId: EmployeeId, calendarEvent: CalendarEvent) => void) {
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
        let positiveAction: Optional<EventAction>;
        let negativeAction: Optional<EventAction>;

        const isOwn = this.userId === employee.employeeId;

        if (event.status === CalendarEventStatus.Requested) {
            if (permissions.has(Permission.approveCalendarEvents)) {
                if (!this.approvedByMe(event, approvals)) {
                    positiveAction = {
                        handler: () => this.eventApprove(employee.employeeId, event),
                        name: 'approve',
                    };
                }
            }

            if (isOwn) {
                if (permissions.has(Permission.cancelPendingCalendarEvents)) {
                    negativeAction = {
                        handler: () => this.eventSetStatus(employee.employeeId, event, CalendarEventStatus.Cancelled),
                        name: 'discard',
                    };
                }
            } else {
                if (permissions.has(Permission.rejectCalendarEvents)) {
                    negativeAction = {
                        handler: () => this.eventSetStatus(employee.employeeId, event, CalendarEventStatus.Rejected),
                        name: 'reject',
                    };
                }
            }
        } else if (event.status === CalendarEventStatus.Approved) {
            if (permissions.has(Permission.cancelApprovedCalendarEvents)) {
                negativeAction = {
                    handler: () => this.eventSetStatus(employee.employeeId, event, CalendarEventStatus.Cancelled),
                    name: 'discard',
                };
            }
        }

        return {
            event,
            employee,
            positiveAction,
            negativeAction,
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
    public static compareContainers(left: EventActionContainer, right: EventActionContainer): number {
        const nameCompareResult = left.employee.name.localeCompare(right.employee.name);
        if (nameCompareResult !== 0) {
            return nameCompareResult;
        }

        return right.event.dates.startDate.valueOf() - left.event.dates.startDate.valueOf();
    }
}
