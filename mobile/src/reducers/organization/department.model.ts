import {dataMember, required} from 'santee-dcts';
import { Employee } from './employee.model';

export class Department {

    @dataMember()
    @required()
    public departmentId: string;

    @dataMember()
    @required()
    public abbreviation: string;

    @dataMember()
    @required()
    public name: string;

    @dataMember()
    @required({ nullable: true })
    public parentDepartmentId: string;

    @dataMember()
    @required({ nullable: true })
    public chiefId: string;

    @dataMember()
    @required({ nullable: true })
    public isHeadDepartment: boolean;

    public equals(obj: Department): boolean {
        return this.departmentId === obj.departmentId &&
        this.abbreviation === obj.abbreviation &&
        this.name === obj.name &&
        this.parentDepartmentId === obj.parentDepartmentId &&
        this.chiefId === obj.chiefId &&
        this.isHeadDepartment === obj.isHeadDepartment;
    }
}