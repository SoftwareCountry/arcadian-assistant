import { SearchActions } from '../search.action';
import { Employee } from '../organization/employee.model';
import { EmployeesStore } from '../organization/employees.reducer';
import { ActionsObservable, ofType } from 'redux-observable';
import { AppState, DependenciesContainer } from '../app.reducer';
import { MiddlewareAPI } from 'redux';
import { Map, Set } from 'immutable';
import { updateDepartmentsBranch } from './people.action';
import { updateTopOfBranch, updateLeaves } from './people.reducer';

export const updateDepartmentsBranchEpic$ = (action$: ActionsObservable<SearchActions>, appState: MiddlewareAPI<AppState>, depends: DependenciesContainer) =>
    action$.ofType('SEARCH-BY-TEXT-FILTER')
    .map(action => {
        const employees = appState.getState().organization.employees;
        const employeesPredicate = (employee: Employee) => {
            return (employee.name && employee.name.includes(action.filter) ||
                    employee.email && employee.email.includes(action.filter) || 
                    employee.position && employee.position.includes(action.filter)
            );
        };
        // filter employees
        const filteredEmployeesById: Map<string, Employee> = employees.employeesById.filter(employeesPredicate) as Map<string, Employee>;
        let filteredEmployeesByDep: Map<string, Set<string>> = 
            employees.employeeIdsByDepartment.map(d => d.filter(e => filteredEmployeesById.has(e))) as Map<string, Set<string>>;
        // clear empty departments
        filteredEmployeesByDep = filteredEmployeesByDep.filter(e => !e.isEmpty()) as Map<string, Set<string>>;

        const people = appState.getState().people;
        const branch = people.departmentsBranch;
        // filter departments
        const filteredDeps = people.departments.filter((d) => filteredEmployeesByDep.has(d.departmentId));
        let deps = filteredDeps;
        filteredDeps.forEach(dep => {
            let curDep = dep;
            while (!curDep.isHeadDepartment) {
                curDep = people.departments.filter(d => d.departmentId === curDep.parentDepartmentId)[0];
                if (deps.findIndex(e => e === curDep) === -1) {
                    deps.push(curDep);
                }
            }
        });
        if (branch && branch.length > 0) {
            // recalculate department branch
            const res = updateTopOfBranch(branch[branch.length - 1].departmentId, deps);
            return updateDepartmentsBranch(res.departmentsLineup, res.departmentsLists, deps);
        } else {
            // calculate new branch from top
            const head = people.departments.filter(d => d.isHeadDepartment)[0];
            const res = updateLeaves([], [], 0, head.departmentId, deps);
            return updateDepartmentsBranch(res.departmentsLineup, res.departmentsLists, deps);
        }
    });