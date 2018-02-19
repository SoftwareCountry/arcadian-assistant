import { ajaxGetJSON } from 'rxjs/observable/dom/AjaxObservable';
import { deserialize } from 'santee-dcts/src/deserializer';
import { loadEmployee, LoadEmployeeFinished, OrganizationActions, LoadEmployeesForDepartment } from '../organization/organization.action';
import { ActionsObservable } from 'redux-observable';
import { User } from '../user/user.model';
import { LoadUser, loadUserFinished, LoadUserFinished, loadUserEmployeeFinished, LoadUserEmployeeFinished } from '../user/user.action';
import { loadUserDepartmentEmployeesFinished } from './people.action';
import { Observable } from 'rxjs/Observable';
import { loadFailedError } from '../errors/errors.action';
import { apiUrl as url } from '../const';

export const loadUserDepartmentEmployeesEpic$ = (action$: ActionsObservable<LoadUserEmployeeFinished | LoadEmployeeFinished>) =>
     Observable.combineLatest<LoadUserEmployeeFinished, LoadEmployeeFinished>(
         action$.ofType('LOAD-USER-EMPLOYEE-FINISHED'),
         action$.ofType('LOAD_EMPLOYEE_FINISHED')
     ).filter(([userEmployee, loadedEmployee]) => userEmployee.employee.departmentId === loadedEmployee.employee.departmentId)
      .map(([userEmployee, loadedEmployee]) => loadedEmployee.employee)
      .map(x => loadUserDepartmentEmployeesFinished(x));