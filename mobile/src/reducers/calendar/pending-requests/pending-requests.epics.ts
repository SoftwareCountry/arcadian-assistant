import { ActionsObservable } from 'redux-observable';
import { LoadPendingRequests, loadPendingRequestsFinished } from './pending-requests.action';
import { AppState, DependenciesContainer } from '../../app.reducer';
import { PendingRequests } from './pending-requests.model';
import { deserialize } from 'santee-dcts';
import { loadFailedError } from '../../errors/errors.action';
import { Observable } from 'rxjs';

export const loadPendingRequestsEpic$ = (action$: ActionsObservable<LoadPendingRequests>, state: AppState, deps: DependenciesContainer) =>
    action$.ofType('LOAD-PENDING-REQUESTS')
        .flatMap(x =>
            deps.apiClient.getJSON(`/pending-requests`)
                .map(obj => deserialize(obj as any, PendingRequests))
        )
        .map(y => loadPendingRequestsFinished(y.events))
        .catch((e: Error) => Observable.of(loadFailedError(e.message)));