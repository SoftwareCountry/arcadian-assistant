import { DepartmentNode } from '../reducers/people/people.model';

export function departmentNodeComparer (first: DepartmentNode, second: DepartmentNode) {
    if (first.abbreviation && second.abbreviation) {
        return azComparer(first.abbreviation, second.abbreviation);
    }

    return azComparer(first.departmentId, second.departmentId);
}

export function azComparer (first: string, second: string) {
    if (first < second) {
        return -1;
    } else if (first > second) {
        return 1;
    }

    return 0;
}
