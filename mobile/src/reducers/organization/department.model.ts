import {dataMember, required} from 'santee-dcts';

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
}