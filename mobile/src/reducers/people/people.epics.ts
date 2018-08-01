import { ActionsObservable, ofType, combineEpics } from 'redux-observable';
import { LoadDepartments, loadDepartmentsFinished, LoadDepartmentsFinished, loadDepartments } from './people.action';
import { LoadUserEmployeeFinished } from '../user/user.action';
import { deserializeArray, deserialize } from 'santee-dcts/src/deserializer';
import { Department } from './department.model';
import { AppState, AppEpic, DependenciesContainer } from '../app.reducer';
import { Observable } from 'rxjs/Observable';
import { loadFailedError } from '../errors/errors.action';
import { handleHttpErrors } from '../errors/errors.epics';

export const loadDepartmentsEpic$ = (action$: ActionsObservable<LoadDepartments>, state: AppState, deps: DependenciesContainer) =>
    action$.ofType('LOAD-DEPARTMENTS')
        .switchMap(x => deps.apiClient.getJSON(`/departments`)
            .pipe(handleHttpErrors()))
        .map(x => deserializeArray(x as any, Department))
        .map(x => loadDepartmentsFinished(x));