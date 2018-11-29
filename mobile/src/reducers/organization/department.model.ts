import {dataMember, required} from 'santee-dcts';
import { Employee } from './employee.model';
import { Nullable } from 'types';

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
}
