import { DepartmentIdToNode, EmployeeIdToNode } from './people.model';

export function buildSelectedDepartmentId(
    departmentIdToNode: DepartmentIdToNode, 
    employeeIdToNode: EmployeeIdToNode, 
    selectedCompanyDepartmentId: string
) {
    if (departmentIdToNode[selectedCompanyDepartmentId]) {
        return selectedCompanyDepartmentId;
    }

    const firstEmployee = employeeIdToNode.first();

    return firstEmployee 
        ? firstEmployee.get('departmentId') as string 
        : null;
}