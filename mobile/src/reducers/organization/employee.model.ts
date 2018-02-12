import { dataMember, required } from 'santee-dcts';

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

    @dataMember()
    public birthDate: string;

    @dataMember()
    public hireDate: string;

    @dataMember()
    @required()
    public hoursCredit: number;

    @dataMember()
    @required()
    public vacationDaysLeft: number;
    
    @dataMember()
    @required({nullable: true})
    public roomNumber: string;
}