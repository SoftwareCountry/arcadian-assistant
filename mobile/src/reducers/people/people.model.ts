export class DepartmentNode {
    constructor(
        public departmentId: string,
        public parentId: string,
        public abbreviation: string,
        public chiefId: string,
        public staffDepartmentId: string
    ) {}

    public equals(obj: DepartmentNode) {
        if (!obj) {
            return false;
        }

        if (obj === this) {
            return true;
        }

        return this.departmentId === obj.departmentId 
            && this.parentId === obj.parentId
            && this.abbreviation === obj.abbreviation
            && this.chiefId === obj.chiefId
            && this.staffDepartmentId === obj.staffDepartmentId;
    }
}

export interface DepartmentIdToChildren {
    [departmentId: string]: DepartmentNode[];
}

export interface DepartmentIdToSelectedId {
    [departmentId: string]: string;
}

export type DepartmentIdToNode = Map<string, DepartmentNode>;