import { dataMember, required } from 'santee-dcts';
import { DataMemberDecoratorParams } from 'santee-dcts/src/dataMemberDecorator';
import moment, { Moment } from 'moment';
import { Nullable, Optional } from 'types';

const dateDecoratorParams: DataMemberDecoratorParams = {
    customDeserializer: (value: string) => moment(value)
};

export type EmployeeId = string;

export class Employee {
    @dataMember()
    @required()
    public employeeId: string = '';

    @dataMember()
    @required()
    public name: string = '';

    @dataMember()
    @required({ nullable: true })
    public email: Nullable<string> = '';

    @dataMember()
    public sex: number = 1;

    @dataMember()
    public photoUrl: string = '';

    @dataMember()
    public position: string = '';

    @dataMember()
    public departmentId: string = '';

    @dataMember()
    public mobilePhone: string = '';

    @dataMember(dateDecoratorParams)
    public birthDate: Moment = moment();

    @dataMember(dateDecoratorParams)
    public hireDate: Moment = moment();

    @dataMember()
    @required({ nullable: true })
    public hoursCredit: Nullable<number> = null;

    @dataMember()
    @required({ nullable: true })
    public vacationDaysLeft: Nullable<number> = null;

    @dataMember()
    @required({ nullable: true })
    public roomNumber: Nullable<string> = null;

    public equals(obj: Optional<Employee>): boolean {
        if (!obj) {
            return false;
        }

        if (obj === this) {
            return true;
        }

        return obj.employeeId === this.employeeId
            && obj.name === this.name
            && obj.email === this.email
            && obj.sex === this.sex
            && obj.photoUrl === this.photoUrl
            && obj.position === this.position
            && obj.departmentId === this.departmentId
            && obj.mobilePhone === this.mobilePhone
            && obj.birthDate.isSame(this.birthDate, 'day')
            && obj.hireDate.isSame(this.hireDate, 'day')
            && obj.hoursCredit === this.hoursCredit
            && obj.vacationDaysLeft === this.vacationDaysLeft
            && obj.roomNumber === this.roomNumber;
    }
}
