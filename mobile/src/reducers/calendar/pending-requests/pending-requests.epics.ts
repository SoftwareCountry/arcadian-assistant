/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import { ActionsObservable, StateObservable } from 'redux-observable';
import { LoadPendingRequests, loadPendingRequestsFailed, loadPendingRequestsFinished } from './pending-requests.action';
import { AppState, DependenciesContainer } from '../../app.reducer';
import { PendingRequests } from './pending-requests.model';
import { deserialize } from 'santee-dcts';
import { map, switchMap } from 'rxjs/operators';
import { handleHttpErrorsWithDefaultValue } from '../../../errors/error.operators';
import { Map } from 'immutable';
import { CalendarEvent } from '../calendar-event.model';
import { of } from 'rxjs';

interface PendingRequestsHolder {
    requests?: Map<string, CalendarEvent[]>;
    success: boolean;
}

//----------------------------------------------------------------------------
export const loadPendingRequestsEpic$ = (action$: ActionsObservable<LoadPendingRequests>, state$: StateObservable<AppState>, deps: DependenciesContainer) =>
    action$.ofType('LOAD-PENDING-REQUESTS').pipe(
        switchMap(() => {
                return deps.apiClient.getJSON(`/pending-requests`).pipe(
                    map(obj => deserialize(obj as any, PendingRequests)),
                    map(pendingRequests => {
                        return {
                            requests: pendingRequests.events, success: true
                        };
                    }),
                    handleHttpErrorsWithDefaultValue(of({
                        success: false
                    })),
                );
            },
        ),
        map((result: PendingRequestsHolder) => {
            if (result.success) {
                return loadPendingRequestsFinished(result.requests!);
            }

            return loadPendingRequestsFailed();
        }),
    );
