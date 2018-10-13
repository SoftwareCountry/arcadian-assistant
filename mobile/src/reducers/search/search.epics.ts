import { Employee } from '../organization/employee.model';
import { EmployeesStore } from '../organization/employees.reducer';
import { Map, Set } from 'immutable';

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