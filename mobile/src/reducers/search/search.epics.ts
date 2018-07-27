import { Employee } from '../organization/employee.model';
import { EmployeesStore } from '../organization/employees.reducer';
import { ActionsObservable, ofType } from 'redux-observable';
import { AppState } from '../app.reducer';
import { MiddlewareAPI } from 'redux';
import { Map, Set } from 'immutable';
import { departmentsBranchFromDepartmentWithId } from '../people/people.reducer';
import { Department } from '../organization/department.model';
import { filterDepartmentsFinished, SetFilter } from './search.action';

export function filterEmployees(employees: EmployeesStore, filter: string) {
    const employeesPredicate = (employee: Employee) => {
        return (employee.name && employee.name.includes(filter) ||
                employee.email && employee.email.includes(filter) || 
                employee.position && employee.position.includes(filter)
        );
    };
    // filter employees
    const filteredEmployeesById: Map<string, Employee> = employees.employeesById.filter(employeesPredicate) as Map<string, Employee>;
    let filteredEmployeesByDep: Map<string, Set<string>> = 
        employees.employeeIdsByDepartment.map(d => d.filter(e => filteredEmployeesById.has(e))) as Map<string, Set<string>>;
    // clear empty departments
    filteredEmployeesByDep = filteredEmployeesByDep.filter(e => !e.isEmpty()) as Map<string, Set<string>>;
    return {employeesById: filteredEmployeesById, employeeIdsByDepartment: filteredEmployeesByDep};
}

export function recountBranch(departments: Department[], currentBranch: Department[], filteredDeps: Department[]) {
    if (currentBranch && currentBranch.length > 0) {
        // recalculate department branch
        let leave = filteredDeps.find(d => d.departmentId === currentBranch[currentBranch.length - 1].departmentId);
        if (leave) {
            // if found - recalculate branch
            return departmentsBranchFromDepartmentWithId(currentBranch[currentBranch.length - 1].departmentId, filteredDeps);
        } else {
            // else - find the smallest department in branch, which was filtered
            let cur = currentBranch[currentBranch.length - 1];
            while (!leave && !cur.isHeadDepartment) {
                leave = filteredDeps.find(d => d.departmentId === cur.parentDepartmentId);
                cur = departments.find(d => d.departmentId === cur.parentDepartmentId);
            }
            return departmentsBranchFromDepartmentWithId(cur.isHeadDepartment ? cur.departmentId : leave.departmentId, filteredDeps);
        }
    } else {
        // calculate new branch from top
        const head = departments.filter(d => d.isHeadDepartment)[0];
        return departmentsBranchFromDepartmentWithId(head.departmentId, filteredDeps);
    }
}

export function recountDepartments(departments: Department[], employees: EmployeesStore) {
    const filteredDeps = departments.filter((d) => employees.employeeIdsByDepartment.has(d.departmentId));
    let deps = filteredDeps;
    filteredDeps.forEach(dep => {
        let curDep = dep;
        while (!curDep.isHeadDepartment) {
            curDep = departments.find(d => d.departmentId === curDep.parentDepartmentId);
            if (curDep && !deps.find(e => e === curDep)) {
                deps.push(curDep);
            }
        }
    });
    return deps;
}

export const updateDepartmentsBranchEpic$ = (action$: ActionsObservable<SetFilter>, appState: MiddlewareAPI<AppState>) =>
    action$.ofType('SEARCH-BY-TEXT-FILTER')
    .map(action => {
        const filteredEmployees = filterEmployees(appState.getState().organization.employees, action.filter);
        const deps = recountDepartments(appState.getState().people.departments, filteredEmployees);
        const newBranch = recountBranch(appState.getState().people.departments, appState.getState().people.departmentsBranch, deps);
        return filterDepartmentsFinished(deps, newBranch.departmentsLineup, newBranch.departmentsLists);
    });