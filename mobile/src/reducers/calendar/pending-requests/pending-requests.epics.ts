import { ActionsObservable, StateObservable } from 'redux-observable';
import { LoadPendingRequests, loadPendingRequestsFinished } from './pending-requests.action';
import { AppState, DependenciesContainer } from '../../app.reducer';
import { PendingRequests } from './pending-requests.model';
import { deserialize } from 'santee-dcts';
import { flatMap, map } from 'rxjs/operators';
import { handleHttpErrors } from '../../../errors/error.operators';

export const loadPendingRequestsEpic$ = (action$: ActionsObservable<LoadPendingRequests>, _: StateObservable<AppState>, deps: DependenciesContainer) =>
    action$.ofType('LOAD-PENDING-REQUESTS').pipe(
        flatMap(() =>
            deps.apiClient.getJSON(`/pending-requests`).pipe(
                map(obj => deserialize(obj as any, PendingRequests)),
                handleHttpErrors(),
            )
        ),
        map(y => loadPendingRequestsFinished(y.events)),
    );
