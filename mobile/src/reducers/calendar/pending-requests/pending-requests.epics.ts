import { ActionsObservable } from 'redux-observable';
import { LoadPendingRequests, loadPendingRequestsFinished } from './pending-requests.action';
import { AppState, DependenciesContainer } from '../../app.reducer';
import { PendingRequests } from './pending-requests.model';
import { deserialize } from 'santee-dcts';
import { loadFailedError } from '../../errors/errors.action';
import {Observable, of, pipe} from 'rxjs';
import { catchError, flatMap, map } from 'rxjs/operators';

export const loadPendingRequestsEpic$ = (action$: ActionsObservable<LoadPendingRequests>, state: AppState, deps: DependenciesContainer) =>
    action$.ofType('LOAD-PENDING-REQUESTS').pipe(
        flatMap(() =>
            deps.apiClient.getJSON(`/pending-requests`).pipe(map(obj => deserialize(obj as any, PendingRequests)))
        ),
        map(y => loadPendingRequestsFinished(y.events)),
        catchError((e: Error) => of(loadFailedError(e.message)))
    );
