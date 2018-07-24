import { SearchActions } from '../search.action';
import { Employee } from '../organization/employee.model';
import { EmployeesStore } from '../organization/employees.reducer';
import { ActionsObservable, ofType } from 'redux-observable';
import { AppState, DependenciesContainer } from '../app.reducer';
import { MiddlewareAPI } from 'redux';
import { Map, Set } from 'immutable';
import { updateDepartmentsBranch } from './people.action';
import { updateTopOfBranch, updateLeaves, departmentsBranchFromDepartmentWithId } from './people.reducer';

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
                curDep = people.departments.find(d => d.departmentId === curDep.parentDepartmentId);
                if (curDep && !deps.find(e => e === curDep)) {
                    deps.push(curDep);
                }
            }
        });

        if (branch && branch.length > 0) {
            // recalculate department branch
            let leave = deps.find(d => d.departmentId === branch[branch.length - 1].departmentId);
            if (leave) {
                // if found - recalculate branch
                const res = updateTopOfBranch(branch[branch.length - 1].departmentId, deps);
                return updateDepartmentsBranch(res.departmentsLineup, res.departmentsLists, deps);
            } else {
                // else - find the smallest department in branch, which was filtered
                let cur = branch[branch.length - 1];
                while (!leave && !cur.isHeadDepartment) {
                    leave = deps.find(d => d.departmentId === cur.parentDepartmentId);
                    cur = people.departments.find(d => d.departmentId === cur.parentDepartmentId);
                }
                const res = departmentsBranchFromDepartmentWithId(cur.isHeadDepartment ? cur.departmentId : leave.departmentId, deps);
                return updateDepartmentsBranch(res.departmentsLineup, res.departmentsLists, deps);
            }
        } else {
            // calculate new branch from top
            const head = people.departments.filter(d => d.isHeadDepartment)[0];
            const res = updateLeaves([], [], 0, head.departmentId, deps);
            return updateDepartmentsBranch(res.departmentsLineup, res.departmentsLists, deps);
        }
    });