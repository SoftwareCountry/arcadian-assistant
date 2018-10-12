import { Department } from '../organization/department.model';
import { EmployeeMap, EmployeeIdsGroupMap } from '../organization/employees.reducer';

// TODO: pass lambda instead of term
export function filterDepartments(
    term: string,
    departments: Department[],
    employeesById: EmployeeMap,
    employeeIdsByDepartmentId: EmployeeIdsGroupMap
): Department[] {
    if (!term) {
        return departments;
    }

    const filteredDepartments = departments.filter(x => {
        const employeeIds = employeeIdsByDepartmentId.get(x.departmentId);

        if (!employeeIds) {
            return false;
        }

        const employeeIdsArray = employeeIds.toArray();

        for (let employeeId of employeeIdsArray) {
            const employee = employeesById.get(employeeId);

            if (!employee) {
                return false;
            }

            if (employee.name.toLowerCase().includes(term.toLowerCase())) {
                return true;
            }
        }

        return false;
    });

    return filteredDepartments;
}