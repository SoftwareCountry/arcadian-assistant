import { dataMember, required } from 'santee-dcts';
import { DataMemberDecoratorParams } from 'santee-dcts/src/dataMemberDecorator';
import moment, { Moment } from 'moment';

const dateDecoratorParams: DataMemberDecoratorParams  = {
    customDeserializer: (value: string) => moment(value)
};

export class Photo {
    @dataMember()
    public mimeType: string;

    @dataMember()
    public base64: string;
}

export class Employee {
    @dataMember()
    @required()
    public employeeId: string;

    @dataMember()
    @required()
    public name: string;

    @dataMember()
    @required({nullable: true})
    public email: string;

    @dataMember()
    public sex: number;

    @dataMember()
    public photoUrl: string;

    @dataMember()
    public position: string;

    @dataMember()
    public departmentId: string;

    @dataMember()
    public mobilePhone: string;

    @dataMember(dateDecoratorParams)
    public birthDate: Moment;

    @dataMember(dateDecoratorParams)
    public hireDate: Moment;

    @dataMember()
    @required({nullable: true})
    public hoursCredit: number;

    @dataMember()
    @required({nullable: true})
    public vacationDaysLeft: number;
    
    @dataMember()
    @required({nullable: true})
    public roomNumber: string;
}