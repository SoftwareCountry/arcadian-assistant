import { ActionsObservable } from 'redux-observable';
import { LoadDepartments, loadDepartmentsFinished, DepartmentsActions } from './departments.action';
import { ajaxGetJSON } from 'rxjs/observable/dom/AjaxObservable';
import { deserializeArray } from 'santee-dcts/src/deserializer';
import { Department } from './department.model';
import { Reducer } from 'redux';

export const loadDepartmentsEpic$ = (action$: ActionsObservable<LoadDepartments>) =>
action$.ofType('LOAD-DEPARTMENTS')
    .first()
    .flatMap(x => ajaxGetJSON('http://localhost:5000/api/departments'))
    .map(x => deserializeArray(x as any, Department))
    .map(x => loadDepartmentsFinished(x));


export const departmentsReducer: Reducer<Department[]> = (state = [], action: DepartmentsActions) => {
switch (action.type) {
    case 'LOAD-DEPARTMENTS-FINISHED':
        return [...action.departments];

    default:
        return state;
}
};