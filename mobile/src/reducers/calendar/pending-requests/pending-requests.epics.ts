/******************************************************************************
 * Copyright (c) Arcadia, Inc. All rights reserved.
 ******************************************************************************/

import { ActionsObservable, StateObservable } from 'redux-observable';
import { LoadPendingRequests, loadPendingRequestsFinished } from './pending-requests.action';
import { AppState, DependenciesContainer } from '../../app.reducer';
import { PendingRequests } from './pending-requests.model';
import { deserialize } from 'santee-dcts';
import { flatMap, map } from 'rxjs/operators';
import { handleHttpErrorsWithDefaultValue } from '../../../errors/error.operators';
import { Map } from 'immutable';
import { CalendarEvent } from '../calendar-event.model';
import { of } from 'rxjs';

//----------------------------------------------------------------------------
export const loadPendingRequestsEpic$ = (action$: ActionsObservable<LoadPendingRequests>, state$: StateObservable<AppState>, deps: DependenciesContainer) =>
    action$.ofType('LOAD-PENDING-REQUESTS').pipe(
        flatMap(() => {
                const currentRequests = state$.value.calendar ? state$.value.calendar.pendingRequests.requests : Map<string, CalendarEvent[]>();
                return deps.apiClient.getJSON(`/pending-requests`).pipe(
                    map(obj => deserialize(obj as any, PendingRequests)),
                    map(pendingRequests => pendingRequests.events),
                    handleHttpErrorsWithDefaultValue(of(currentRequests)),
                );
            },
        ),
        map(requests => loadPendingRequestsFinished(requests)),
    );
