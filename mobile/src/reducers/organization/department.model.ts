import { dataMember, required } from 'santee-dcts';
import { Nullable, Optional } from 'types';

export class Department {

    @dataMember()
    @required()
    public departmentId: string = '';

    @dataMember()
    @required()
    public abbreviation: string = '';

    @dataMember()
    @required()
    public name: string = '';

    @dataMember()
    @required({ nullable: true })
    public parentDepartmentId: Nullable<string> = null;

    @dataMember()
    @required({ nullable: true })
    public chiefId: Nullable<string> = null;

    @dataMember()
    @required({ nullable: true })
    public isHeadDepartment: Nullable<boolean> = null;

    public equals(obj: Optional<Department>): boolean {
        if (!obj) {
            return false;
        }

        if (obj === this) {
            return true;
        }

        return obj.departmentId === this.departmentId
            && obj.abbreviation === this.abbreviation
            && obj.name === this.name
            && obj.parentDepartmentId === this.parentDepartmentId
            && obj.chiefId === this.chiefId
            && obj.isHeadDepartment === this.isHeadDepartment;
    }
}
