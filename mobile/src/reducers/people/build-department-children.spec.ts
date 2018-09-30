import { Department } from '../organization/department.model';
import { buildDepartmentIdToNode } from './build-department-id-to-node';
import { buildDepartmentIdToChildren } from './build-department-children';
import { DepartmentIdToNode, DepartmentNode, MapDepartmentNode } from './people.model';
import { Set, Map } from 'immutable';

const department = (departmentId: string, parentDepartmentId: string, name: string): Department => {
    const newDepartment = new Department();
    newDepartment.departmentId = departmentId;
    newDepartment.parentDepartmentId = parentDepartmentId;
    newDepartment.name = name;
    return newDepartment;
};

describe('build-department-childrned', () => {
    let departments: Department[];
    let departmentIdToNode: DepartmentIdToNode;
    let head: Department;
    let foo: Department;
    let zoo: Department;
    let bar: Department;
    let baz: Department;

    beforeEach(() => {
        departments = [
            department('0', null, 'Head'),
                department('1', '0', 'Foo'),
                    department('3', '1', 'Zoo'),
                department('2', '0', 'Bar'),
                    department('4', '2', 'Baz'),
        ];

        [head, foo, zoo, bar, baz] = departments;
    });

    beforeEach(() => {
        departmentIdToNode = buildDepartmentIdToNode(departments);
    });

    it('should build child nodes by departmentId', () => {
        /*
            {
                dep1: { departmentId: dep1, parentDepartmentId: null, ... },
                dep2: { departmentId: dep2, parentDepartmentId: dep1, ... },
                dep3: { departmentId: dep3, parentDepartmentId: dep2, ... }
            }   

                                                ↓

            {
                dep1: Set<Map<{ departmentId: dep2, parentDepartmentId, dep1, ... }>>,
                dep2: Set<Map<{ departmentId: dep3, parentDepartmentId: dep2, ... }>>
            }                                                

        */
        const children = buildDepartmentIdToChildren(departmentIdToNode);
        
        const headChildren = Set<MapDepartmentNode>([
            Map(departmentIdToNode[foo.departmentId]), 
            Map(departmentIdToNode[bar.departmentId])
        ]);

        expect(children[head.departmentId].equals(headChildren)).toBeTruthy();

        const fooChildren = Set<MapDepartmentNode>([
            Map(departmentIdToNode[zoo.departmentId]), 
        ]);     
        
        expect(children[foo.departmentId].equals(fooChildren)).toBeTruthy();

        const barChildren = Set<MapDepartmentNode>([
            Map(departmentIdToNode[baz.departmentId]), 
        ]);     
        
        expect(children[bar.departmentId].equals(barChildren)).toBeTruthy();        
    });
});