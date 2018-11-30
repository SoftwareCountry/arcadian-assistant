import {dataMember, required} from 'santee-dcts';
import { Employee } from './employee.model';
import { Nullable } from 'types';

export class Department {

    @dataMember()
    @required()
    public departmentId = '';

    @dataMember()
    @required()
    public abbreviation = '';

    @dataMember()
    @required()
    public name = '';

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
