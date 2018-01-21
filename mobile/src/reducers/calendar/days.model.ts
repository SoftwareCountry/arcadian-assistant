import { dataMember, required } from 'santee-dcts';

// TODO: mock models

export class DaysItem {
    @dataMember()
    @required()
    public leftDays: number;

    @dataMember()
    @required()
    public allDays: number;

    @dataMember()
    @required()
    public title: string;
}

export class Days {
    @dataMember()
    @required()    
    public vacation: DaysItem;

    @dataMember()
    @required()    
    public off: DaysItem;

    @dataMember()
    @required()    
    public sick: DaysItem;
}