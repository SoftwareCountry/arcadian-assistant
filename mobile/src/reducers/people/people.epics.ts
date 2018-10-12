import { ActionsObservable } from 'redux-observable';
import { AppState, DependenciesContainer } from '../app.reducer';
import { PeopleActions, SelectCompanyDepartment, RedirectToEmployeeDetails } from './people.action';
import { map } from 'rxjs/operators';
import { loadEmployeesForDepartment } from '../organization/organization.action';
import { openEmployeeDetailsAction } from '../../employee-details/employee-details-dispatcher';
import { MiddlewareAPI } from 'redux';

export const companyDepartmentSelected$ = (action$: ActionsObservable<SelectCompanyDepartment>) =>
    action$.ofType('SELECT-COMPANY-DEPARTMENT')
        .pipe(
            map(x => loadEmployeesForDepartment(x.departmentId))
        );

export const redirectToEmployeeDetails$ = (action$: ActionsObservable<RedirectToEmployeeDetails>, store: MiddlewareAPI<AppState>) =>
    action$.ofType('REDIRECT-TO-EMPLOYEE-DETAILS')
        .pipe(
            map(x => {
                const employee = store.getState().organization.employees.employeesById.get(x.employeeId);
                return openEmployeeDetailsAction(employee);
            })
        );