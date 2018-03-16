import { Employee } from '../../reducers/organization/employee.model';
import { EmployeeMap } from '../../reducers/organization/employees.reducer';
import { Iterable } from 'immutable';

export class DepartmentsTreeNode {
    public departmentId: string;
    public head: Employee;
    public parent: DepartmentsTreeNode;
    public children: DepartmentsTreeNode[];
    public subordinates: Employee[];
}
