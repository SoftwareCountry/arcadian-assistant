import { Department } from '../organization/department.model';
import { EmployeeMap, EmployeeIdsGroupMap } from '../organization/employees.reducer';
import { EmployeeIdToNode } from './people.model';

export function filterDepartments(
    departments: Department[],
    employeeIdToNode: EmployeeIdToNode
): Department[] {
    const employeeNodes = employeeIdToNode.toArray();
    
    const filteredDepartments = departments.filter(department => {

        for (let employeeNode of employeeNodes) {
            if (employeeNode.get('departmentId') === department.departmentId) {
                return true;
            }
        }

        return false;
    });

    return filteredDepartments;
}