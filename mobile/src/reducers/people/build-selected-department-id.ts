import { DepartmentIdToNode } from './people.model';
import { EmployeeMap } from '../organization/employees.reducer';

export function buildSelectedDepartmentId(
    departmentIdToNode: DepartmentIdToNode, 
    employeesById: EmployeeMap, 
    selectedCompanyDepartmentId: string
) {
    if (departmentIdToNode[selectedCompanyDepartmentId]) {
        return selectedCompanyDepartmentId;
    }

    const firstEmployee = employeesById.first();

    return firstEmployee 
        ? firstEmployee.departmentId
        : null;
}