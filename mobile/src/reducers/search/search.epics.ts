import { Employee } from '../organization/employee.model';
import { EmployeesStore } from '../organization/employees.reducer';
import { Map, Set } from 'immutable';

export function filterEmployees(employees: EmployeesStore, filter: string) {
    const lowercasedFilter = filter.toLowerCase();
    const employeesPredicate = (employee: Employee) => {
        if (!filter) {
            return true;
        }

        if (employee.name && employee.name.toLowerCase().includes(lowercasedFilter)) {
            return true;
        }

        if (employee.position && employee.position.toLowerCase().includes(lowercasedFilter)) {
            return true;
        }

        return false;
    };
    // filter employees
    const filteredEmployeesById: Map<string, Employee> = employees.employeesById.filter(employeesPredicate) as Map<string, Employee>;
    let filteredEmployeesByDep: Map<string, Set<string>> = 
        employees.employeeIdsByDepartment.map(d => d.filter(e => filteredEmployeesById.has(e))) as Map<string, Set<string>>;
    // clear empty departments
    filteredEmployeesByDep = filteredEmployeesByDep.filter(e => !e.isEmpty()) as Map<string, Set<string>>;
    return {employeesById: filteredEmployeesById, employeeIdsByDepartment: filteredEmployeesByDep};
}