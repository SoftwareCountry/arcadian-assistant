import { Department } from '../organization/department.model';
import { buildDepartmentIdToNode } from './build-department-id-to-node';
import { DepartmentIdToNode } from './people.model';

const department = (departmentId: string, parentDepartmentId: string, name: string): Department => {
    const newDepartment = new Department();
    newDepartment.departmentId = departmentId;
    newDepartment.parentDepartmentId = parentDepartmentId;
    newDepartment.name = name;
    return newDepartment;
};

describe('build-department-id-to-node', () => {
    let departments: Department[];

    beforeEach(() => {
        departments = [
            department('0', null, 'Head'),
                department('1', '0', 'Foo'),
                    department('3', '1', 'Zoo'),
                department('2', '0', 'Bar'),
        ];
    });
    
    it('should build node by departmentId from departments array', () => {
        /*
            [dep1, dep2, ...]

                    ↓

            {
                [dep1.departmentId]: { dep1.departmentId, dep1.parentDepartmentId, dep1.name },
                [dep2.departmentId]: { dep2.departmentId, dep2.parentDepartmentId, dep2.name },
                ...
            }      
        */        
        const departmentIdToNode = buildDepartmentIdToNode(departments);
        const departmentIds = Object.keys(departmentIdToNode);

        for (let dep of departments) {
            const node = departmentIdToNode[dep.departmentId];
            expect(node.departmentId).toBe(dep.departmentId);
            expect(node.parentId).toBe(dep.parentDepartmentId);
            expect(node.abbreviation).toBe(dep.name);
        }
    });
});