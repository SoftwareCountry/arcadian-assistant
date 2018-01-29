import { ajaxGetJSON } from 'rxjs/observable/dom/AjaxObservable';
import { deserialize } from 'santee-dcts/src/deserializer';
import { loadEmployee } from '../organization/organization.action';
import { ActionsObservable } from 'redux-observable';
import { User } from './user.model';
import { url } from '../organization/organization.epics';
import { LoadUser, loadUserFinished, LoadUserFinished } from './user.action';

// TODO: Handle error, display some big alert blocking app...
export const loadUserEpic$ = (action$: ActionsObservable<LoadUser>) =>
    action$.ofType('LOAD-USER')
        .switchMap(x => ajaxGetJSON(`${url}/user`))
        .map(x => deserialize(x, User))
        .map(x => loadUserFinished(x));

export const loadUserFinishedEpic$ = (action$: ActionsObservable<LoadUserFinished>) =>
    action$.ofType('LOAD-USER-FINISHED')
        .map(x => loadEmployee(x.user.employeeId));