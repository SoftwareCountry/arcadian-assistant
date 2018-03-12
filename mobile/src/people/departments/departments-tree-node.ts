import { Employee } from '../../reducers/organization/employee.model';

export class DepartmentsTreeNode {
    public departmentId: string;
    public head: Employee;
    public parent: DepartmentsTreeNode;
    public children: DepartmentsTreeNode[];
}
