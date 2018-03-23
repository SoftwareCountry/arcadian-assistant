import { Employee } from '../../reducers/organization/employee.model';
import { EmployeeMap } from '../../reducers/organization/employees.reducer';
import { Department } from '../../reducers/organization/department.model';

export class DepartmentsTreeNode {
    public departmentId: string;
    public departmentAbbreviation: string;
    public departmentChiefId: string;
    public head: Employee;
    public parent: Department;
    public children: DepartmentsTreeNode[];
    public subordinates: Employee[];
}
