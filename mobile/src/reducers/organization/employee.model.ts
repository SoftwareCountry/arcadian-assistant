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
    public width: number;

    @dataMember()
    public height: number;

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
    @required({nullable: true})
    public photo: Photo;

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

    public equals(obj: Employee): boolean {
        if (!obj) {
            return false;
        }
        if (obj === this) {
            return true;
        }
        
        return this.employeeId === obj.employeeId &&
        this.name === obj.name &&
        this.email === obj.email &&
        this.sex === obj.sex &&
        this.position === obj.position &&
        this.departmentId === obj.departmentId &&
        this.mobilePhone === obj.mobilePhone &&
        this.birthDate === obj.birthDate &&
        this.hireDate === obj.hireDate &&
        this.hoursCredit === obj.hoursCredit &&
        this.vacationDaysLeft === obj.vacationDaysLeft &&
        this.roomNumber === obj.roomNumber;
    }
}