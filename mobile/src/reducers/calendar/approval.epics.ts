/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import { ActionsObservable, ofType, StateObservable } from 'redux-observable';
import { flatMap, map, mergeMap, tap } from 'rxjs/operators';
import { handleHttpErrors } from '../../errors/error.operators';
import { deserializeArray } from 'santee-dcts';
import { ApprovalActionType, Approve, approveFinished, LoadApprovals, loadApprovalsFinished } from './approval.action';
import { AppState, DependenciesContainer } from '../app.reducer';
import { Approval } from './approval.model';
import { Map as ImmutableMap, Set } from 'immutable';
import { forkJoin } from 'rxjs';

//----------------------------------------------------------------------------
export const loadApprovals$ = (action$: ActionsObservable<LoadApprovals>, _: StateObservable<AppState>, deps: DependenciesContainer) => action$.pipe(
    ofType(ApprovalActionType.loadApprovals),
    mergeMap(action => {
        const employeeId = action.employeeId;
        const eventIds = Set(action.eventIds);
        const requests = eventIds.toArray().map(eventId => {
            return deps.apiClient.getJSON(`/employees/${employeeId}/events/${eventId}/approvals`).pipe(
                map(obj => deserializeArray(obj as any, Approval)),
                tap(approvals => {
                    approvals.forEach(approval => {
                        approval.eventId = eventId;
                    });
                }),
                map(approvals => {
                    return { eventId: eventId, approvals: approvals };
                }),
                handleHttpErrors(),
            );
        });
        return forkJoin(requests).pipe(
            map(approvalsForEventId => {
                const approvals = new Map();
                approvalsForEventId.forEach(x => {
                    approvals.set(x.eventId, x.approvals);
                });
                return { employeeId: employeeId, approvals: ImmutableMap(approvals) };
            }),
        );
    }),
    map(holder => loadApprovalsFinished(holder.employeeId, holder.approvals)),
);

//----------------------------------------------------------------------------
export const approve$ = (action$: ActionsObservable<Approve>, state$: StateObservable<AppState>, deps: DependenciesContainer) => action$.pipe(
    ofType(ApprovalActionType.approve),
    flatMap(action => {
        const body = {
            approverId: action.approverId,
        };
        const headers = {
            'Content-Type': 'application/json',
        };
        return deps.apiClient.post(`/employees/${action.employeeId}/events/${action.eventId}/approvals`, body, headers).pipe(
            handleHttpErrors(),
            map(x => {
                const approval = new Approval(action.approverId, action.eventId);
                return approveFinished(approval);
            }),
        );
    }),
);
