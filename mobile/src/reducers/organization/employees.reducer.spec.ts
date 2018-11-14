import { employeesReducer, EmployeesStore } from './employees.reducer';
import { Employee } from './employee.model';
import { loadEmployeesFinished } from './organization.action';

describe('employees reducer', () => {

    it('should return the initial state', () => {

        const defaultState = employeesReducer(undefined, { type: ''});
        expect(defaultState).toBeDefined();
        expect(defaultState.employeesById).toBeDefined();
        expect(defaultState.employeeIdsByDepartment).toBeDefined();
    });

    it('should add employee to the list when loaded', () => {

        const employee = new Employee();
        const id = 'test_id';
        employee.employeeId = id;

        const state = employeesReducer(undefined, loadEmployeesFinished([ employee ]));
        expect(state.employeesById.get(id)).toEqual(employee);
    });

    it('should add employee id in its department list', () => {
        const employee = new Employee();
        const id = 'test_id';
        const depId = 'dep_id';
        employee.employeeId = id;
        employee.departmentId = depId;

        const state = employeesReducer(undefined, loadEmployeesFinished([ employee ]));
        expect(state.employeeIdsByDepartment.get(depId).has(id)).toBeTruthy();
    });

    it('should move employee id in department lists if it were moved in another department', () => {
        const oldEmployee = new Employee();
        const id = 'test_id';
        const depId1 = 'dep_id1';
        oldEmployee.employeeId = id;
        oldEmployee.departmentId = depId1;

        const updatedEmployee = new Employee();
        const depId2 = 'dep_id2';
        updatedEmployee.employeeId = id;
        updatedEmployee.departmentId = depId2;

        let state = employeesReducer(undefined, loadEmployeesFinished([ oldEmployee ]));
        state = employeesReducer(state, loadEmployeesFinished([ updatedEmployee ]));

        expect(state.employeesById.get(id)).toEqual(updatedEmployee);
        expect(state.employeeIdsByDepartment.get(depId1).has(id)).toBeFalsy();
        expect(state.employeeIdsByDepartment.get(depId2).has(id)).toBeTruthy();
    });

});
